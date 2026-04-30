using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _productService.GetCategoriesAsync();
            return Ok(new ApiResponse<IEnumerable<Category>>(categories, "Categories retrieved successfully"));
        }
    }
}
