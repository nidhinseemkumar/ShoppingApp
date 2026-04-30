using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;

namespace ShoppingApp.Services
{
    public partial class ProductService
    {
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
                if (product == null) return new ApiResponse<bool>("Product cannot be null");

                var existingProduct = await _unitOfWork.Repository<Product>()
                    .GetQueryable()
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

                if (existingProduct == null) return new ApiResponse<bool>("Product not found");

                // Update basic properties
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;

                // Sync Images
                var imagesToRemove = existingProduct.Images
                    .Where(ei => !product.Images.Any(pi => pi.ImageUrl == ei.ImageUrl))
                    .ToList();
                
                foreach (var img in imagesToRemove)
                {
                    existingProduct.Images.Remove(img);
                }

                var imagesToAdd = product.Images
                    .Where(pi => !existingProduct.Images.Any(ei => ei.ImageUrl == pi.ImageUrl))
                    .ToList();

                foreach (var img in imagesToAdd)
                {
                    existingProduct.Images.Add(img);
                }

                if (existingProduct.Images.Any() && !existingProduct.Images.Any(i => i.IsPrimary))
                {
                    existingProduct.Images.First().IsPrimary = true;
                }

                _unitOfWork.Repository<Product>().Update(existingProduct);
                await _unitOfWork.CompleteAsync();
                return new ApiResponse<bool>(true, "Product updated successfully");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return new ApiResponse<bool>("Failed to update product: " + ex.Message);
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
    }
}
