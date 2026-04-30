using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class CartsController : Controller
    {
        // Instant Delete
        public async Task<IActionResult> Delete(int id)
        {
            await _cartService.RemoveFromCartAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
