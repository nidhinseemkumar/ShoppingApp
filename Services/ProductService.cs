using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using ShoppingApp.DTOs;
using ShoppingApp.Models;
using ShoppingApp.Repositories;
using ShoppingApp.Wrappers;

namespace ShoppingApp.Services
{
    public interface IProductService
    {
        Task<ApiResponse<IEnumerable<ProductDto>>> GetAllProductsAsync(string? searchTerm = null, string? category = null);
        Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id);
        Task<ApiResponse<bool>> CreateProductAsync(Product product);
        Task<ApiResponse<bool>> UpdateProductAsync(Product product);
        Task<ApiResponse<bool>> DeleteProductAsync(int id);
        Task<bool> ProductExists(int id);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetOrCreateCategoryByNameAsync(string name);
        Task<bool> UpdateStockAsync(int productId, int stock);
        Task<bool> UpdatePriceAsync(int productId, decimal price);
        Task<Product?> GetProductEntityByIdAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly IMemoryCache _cache;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllProductsAsync(string? searchTerm = null, string? category = null)
        {
            try
            {
                var query = _unitOfWork.Repository<Product>()
                    .GetQueryable()
                    .Include(p => p.Category)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(p => (p.Name != null && p.Name.Contains(searchTerm)) || (p.Description != null && p.Description.Contains(searchTerm)));
                }

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(p => p.Category != null && p.Category.CategoryName == category);
                }

                var products = await query.ToListAsync();

                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products) ?? Enumerable.Empty<ProductDto>();
                return new ApiResponse<IEnumerable<ProductDto>>(productDtos);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return new ApiResponse<IEnumerable<ProductDto>>("An error occurred while fetching products") { Data = Enumerable.Empty<ProductDto>() };
            }
        }

        public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Repository<Product>()
                .GetQueryable()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return new ApiResponse<ProductDto>("Product not found");

            var productDto = _mapper.Map<ProductDto>(product);
            if (productDto == null) return new ApiResponse<ProductDto>("Error mapping product data");
            
            return new ApiResponse<ProductDto>(productDto);
        }

        public async Task<ApiResponse<bool>> CreateProductAsync(Product product)
        {
            try
            {
                await _unitOfWork.Repository<Product>().AddAsync(product);
                await _unitOfWork.CompleteAsync();
                return new ApiResponse<bool>(true, "Product created successfully");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return new ApiResponse<bool>("Failed to create product");
            }
        }

        public async Task<ApiResponse<bool>> UpdateProductAsync(Product product)
        {
            try
            {
                _unitOfWork.Repository<Product>().Update(product);
                await _unitOfWork.CompleteAsync();
                return new ApiResponse<bool>(true, "Product updated successfully");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return new ApiResponse<bool>("Failed to update product");
            }
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
                if (product == null) return new ApiResponse<bool>("Product not found");

                _unitOfWork.Repository<Product>().Delete(product);
                await _unitOfWork.CompleteAsync();
                return new ApiResponse<bool>(true, "Product deleted successfully");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                return new ApiResponse<bool>("Failed to delete product");
            }
        }

        public async Task<bool> ProductExists(int id)
        {
            return await _unitOfWork.Repository<Product>().ExistsAsync(p => p.ProductId == id);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            string cacheKey = "all_categories";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Category>? categories))
            {
                categories = await _unitOfWork.Repository<Category>().GetAllAsync();
                
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(1))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(12));

                _cache.Set(cacheKey, categories, cacheOptions);
            }
            return categories ?? Enumerable.Empty<Category>();
        }

        public async Task<Category> GetOrCreateCategoryByNameAsync(string name)
        {
            var category = await _unitOfWork.Repository<Category>()
                .GetQueryable()
                .FirstOrDefaultAsync(c => c.CategoryName != null && c.CategoryName.ToLower() == name.ToLower());

            if (category == null)
            {
                category = new Category { CategoryName = name };
                await _unitOfWork.Repository<Category>().AddAsync(category);
                await _unitOfWork.CompleteAsync();
                
                // Invalidate categories cache
                _cache.Remove("all_categories");
            }

            return category;
        }

        public async Task<bool> UpdateStockAsync(int productId, int stock)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product == null) return false;
            product.Stock = stock;
            _unitOfWork.Repository<Product>().Update(product);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdatePriceAsync(int productId, decimal price)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product == null) return false;
            product.Price = price;
            _unitOfWork.Repository<Product>().Update(product);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<Product?> GetProductEntityByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        }
    }
}
