using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public partial class CartsController : Controller
    {
        public async Task<IActionResult> BuyNow(int productId)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Users", new { returnUrl = Request.Path + Request.QueryString });
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                // We create a temporary order or just pass the product ID to the payment page
                // For simplicity, we'll let the user choose payment for this product
                return RedirectToAction("Index", "Payments", new { productId = productId });
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
