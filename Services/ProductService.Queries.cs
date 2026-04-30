using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShoppingApp.DTOs;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;

namespace ShoppingApp.Services
{
    public partial class ProductService
    {
        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold)
        {
            return await _unitOfWork.Repository<Product>()
                .GetQueryable()
                .Where(p => p.Stock <= threshold)
                .OrderBy(p => p.Stock)
                .ToListAsync();
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllProductsAsync(string? searchTerm = null, string? category = null)
        {
            try
            {
                var query = _unitOfWork.Repository<Product>()
                    .GetQueryable()
                    .Include(p => p.Category)
                    .Include(p => p.Images)
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

        public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id, int? userId = null)
        {
            var product = await _unitOfWork.Repository<Product>()
                .GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return new ApiResponse<ProductDto>("Product not found");

            var productDto = _mapper.Map<ProductDto>(product);
            if (productDto == null) return new ApiResponse<ProductDto>("Error mapping product data");

            if (product.Reviews.Any())
            {
                productDto.AverageRating = Math.Round(product.Reviews.Average(r => r.Rating), 1);
                productDto.ReviewCount = product.Reviews.Count;
                productDto.Reviews = _mapper.Map<List<ReviewDto>>(product.Reviews.OrderByDescending(r => r.CreatedAt));
                
                if (userId.HasValue)
                {
                    productDto.UserReview = productDto.Reviews.FirstOrDefault(r => r.UserId == userId.Value);
                }
            }
            
            return new ApiResponse<ProductDto>(productDto);
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
                categories = await _unitOfWork.Repository<Category>()
                    .GetQueryable()
                    .Include(c => c.Products)
                    .ToListAsync();
                
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(1))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(12));

                _cache.Set(cacheKey, categories, cacheOptions);
            }
            return categories ?? Enumerable.Empty<Category>();
        }

        public async Task<Product?> GetProductEntityByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Product>()
                .GetQueryable()
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }
    }
}
