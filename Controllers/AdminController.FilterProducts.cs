using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> FilterProducts(string? searchTerm, string? category)
        {
            var response = await _productService.GetAllProductsAsync(searchTerm, category);
            var products = response?.Data ?? [];
            return Json(products);
        }
    }
}
