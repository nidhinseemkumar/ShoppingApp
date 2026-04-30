using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class ProductsController : Controller
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdatePrice(int productId, decimal price)
        {
            var success = await _productService.UpdatePriceAsync(productId, price);
            return Json(new { success });
        }
    }
}
