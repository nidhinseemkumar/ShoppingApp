using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class ProductsController : Controller
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStock(int productId, int stock)
        {
            var success = await _productService.UpdateStockAsync(productId, stock);
            return Json(new { success });
        }
    }
}
