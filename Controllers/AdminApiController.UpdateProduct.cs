using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] Product product, IFormFile? image)
        {
            var existing = await _productService.GetProductEntityByIdAsync(id);
            if (existing == null) return NotFound(new ApiResponse<string>("Product not found"));

            if (image != null)
            {
                if (image.Length > 5 * 1024 * 1024) return BadRequest(new ApiResponse<string>("File too large (max 5MB)"));
                product.ImageUrl = await _fileService.SaveImageAsync(image);
            }
            else
            {
                product.ImageUrl = existing.ImageUrl;
            }

            product.ProductId = id;
            var response = await _productService.UpdateProductAsync(product);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
