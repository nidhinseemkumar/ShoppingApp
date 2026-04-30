using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;

namespace ShoppingApp.Services
{
    public partial class ProductService
    {
        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                var isUsed = await _unitOfWork.Repository<Product>().ExistsAsync(p => p.CategoryId == id);
                if (isUsed) return new ApiResponse<bool>("Cannot delete category linked to products");

                var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
                if (category == null) return new ApiResponse<bool>("Category not found");

                _unitOfWork.Repository<Category>().Delete(category);
                await _unitOfWork.CompleteAsync();
                _cache.Remove("all_categories");
                return new ApiResponse<bool>(true, "Category deleted successfully");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                return new ApiResponse<bool>("Failed to delete category");
            }
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

        public async Task<ApiResponse<bool>> UpdateCategoryAsync(int id, string newName)
        {
            try
            {
                var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
                if (category == null) return new ApiResponse<bool>("Category not found");

                category.CategoryName = newName;
                _unitOfWork.Repository<Category>().Update(category);
                await _unitOfWork.CompleteAsync();
                _cache.Remove("all_categories");
                return new ApiResponse<bool>(true, "Category updated successfully");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                return new ApiResponse<bool>("Failed to update category");
            }
        }
    }
}
