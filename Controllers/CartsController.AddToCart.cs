using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public partial class CartsController : Controller
    {
        [AllowAnonymous]
        public async Task<IActionResult> AddToCart(int productId)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                {
                    return Json(new { success = false, redirect = Url.Action("Login", "Users") });
                }
                return RedirectToAction("Login", "Users", new { returnUrl = Request.Path + Request.QueryString });
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                await _cartService.AddToCartAsync(productId, userId);

                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                {
                    return Json(new { success = true, newQuantity = 1 });
                }
            }

            var referer = Request.Headers.Referer.ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("Index", "Products");
            }
            return Redirect(referer);
        }
    }
}
