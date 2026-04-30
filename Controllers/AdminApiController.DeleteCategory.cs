using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Wrappers;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _productService.DeleteCategoryAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
