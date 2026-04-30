using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _productService.DeleteCategoryAsync(id);
            if (result.Success) TempData["SuccessMessage"] = result.Message;
            else TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Categories));
        }
    }
}
