using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminController : Controller
    {
        public async Task<IActionResult> Categories()
        {
            var categories = await _productService.GetCategoriesAsync();
            return View(categories ?? []);
        }
    }
}
