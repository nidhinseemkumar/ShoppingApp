using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(int id, string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName)) return BadRequest("Name required");
            var result = await _productService.UpdateCategoryAsync(id, categoryName.Trim());
            if (result.Success) TempData["SuccessMessage"] = result.Message;
            else TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Categories));
        }
    }
}
