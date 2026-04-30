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
        Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id, int? userId = null);
        Task<ApiResponse<bool>> CreateProductAsync(Product product);
        Task<ApiResponse<bool>> UpdateProductAsync(Product product);
        Task<ApiResponse<bool>> DeleteProductAsync(int id);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
        Task<bool> ProductExists(int id);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetOrCreateCategoryByNameAsync(string name);
        Task<bool> UpdateStockAsync(int productId, int stock);
        Task<bool> UpdatePriceAsync(int productId, decimal price);
        Task<Product?> GetProductEntityByIdAsync(int id);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);
        Task<ApiResponse<bool>> UpdateCategoryAsync(int id, string newName);
    }

    public partial class ProductService(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        ILogger<ProductService> logger, 
        IMemoryCache cache) : IProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProductService> _logger = logger;
        private readonly IMemoryCache _cache = cache;
    }
}
