using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public partial class CartsController : Controller
    {
        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                return View(await _cartService.GetCartItemsAsync(userId));
            }
            return RedirectToAction("Login", "Users");
        }
    }
}
