using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminController : Controller
    {
        public async Task<IActionResult> Products(string? searchTerm, string? category)
        {
            var response = await _productService.GetAllProductsAsync(searchTerm, category);
            var products = response?.Data ?? [];
            ViewBag.CurrentSearch = searchTerm;
            ViewBag.CurrentCategory = category;
            ViewBag.Categories = await _productService.GetCategoriesAsync();
            return View(products);
        }
    }
}
