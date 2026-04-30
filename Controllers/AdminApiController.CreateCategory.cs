using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (string.IsNullOrEmpty(category.CategoryName)) return BadRequest(new ApiResponse<string>("Category name is required"));
            var result = await _productService.GetOrCreateCategoryByNameAsync(category.CategoryName);
            return Ok(new ApiResponse<Category>(result, "Category created successfully"));
        }
    }
}
