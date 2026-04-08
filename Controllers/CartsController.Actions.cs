using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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

        [AllowAnonymous]
        public async Task<IActionResult> UpdateQuantity(int productId, int change)
        {
            if (!User.Identity.IsAuthenticated)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
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
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var cartItems = await _cartService.GetCartItemsAsync(userId);
                    var newQuantity = cartItems.FirstOrDefault(c => c.ProductId == productId)?.Quantity ?? 0;
                    return Json(new { success = true, newQuantity = newQuantity });
                }
            }
            
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(referer);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _cartService.RemoveFromCartAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> AddToCart(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, redirect = Url.Action("Login", "Users") });
                }
                return RedirectToAction("Login", "Users", new { returnUrl = Request.Path + Request.QueryString });
            }

            var userIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                await _cartService.AddToCartAsync(productId, userId);
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, newQuantity = 1 });
                }
            }
            
            return RedirectToAction("Index");
        }
    }
}
