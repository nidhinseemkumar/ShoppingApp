using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> CreateCategory(string categoryName)
        {
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                await _productService.GetOrCreateCategoryByNameAsync(categoryName.Trim());
                TempData["Message"] = "Category created successfully.";
            }
            return RedirectToAction(nameof(Categories));
        }
    }
}
