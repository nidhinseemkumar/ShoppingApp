using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromForm] Product product, IFormFile? image)
        {
            if (image != null)
            {
                if (image.Length > 5 * 1024 * 1024) return BadRequest(new ApiResponse<string>("File too large (max 5MB)"));
                product.ImageUrl = await _fileService.SaveImageAsync(image);
            }

            var response = await _productService.CreateProductAsync(product);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
