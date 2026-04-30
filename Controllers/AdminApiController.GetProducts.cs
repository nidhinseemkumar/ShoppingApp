using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Wrappers;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10, string? search = null)
        {
            var response = await _productService.GetAllProductsAsync(search);
            if (!response.Success || response.Data == null) return BadRequest(response);

            var data = response.Data!;
            var items = data.Skip((page - 1) * pageSize).Take(pageSize);
            var result = new { items, totalCount = data.Count() };

            return Ok(new ApiResponse<object>(result, "Products retrieved successfully"));
        }
    }
}
