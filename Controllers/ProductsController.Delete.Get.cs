using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class ProductsController : Controller
    {
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _productService.GetProductEntityByIdAsync(id.Value);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}
