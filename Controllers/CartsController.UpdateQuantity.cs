using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public partial class CartsController : Controller
    {
        [AllowAnonymous]
        public async Task<IActionResult> UpdateQuantity(int productId, int change)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                {
                    return Json(new { success = false, redirect = Url.Action("Login", "Users") });
                }
                return RedirectToAction("Login", "Users");
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                await _cartService.UpdateQuantityAsync(productId, userId, change);

                // If it's an AJAX request, return JSON
                if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                {
                    var cartItems = await _cartService.GetCartItemsAsync(userId);
                    var newQuantity = cartItems.FirstOrDefault(c => c.ProductId == productId)?.Quantity ?? 0;
                    return Json(new { success = true, newQuantity = newQuantity });
                }
            }

            var referer = Request.Headers.Referer.ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(referer);
        }
    }
}
