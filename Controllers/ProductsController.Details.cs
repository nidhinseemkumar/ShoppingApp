using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public partial class ProductsController : Controller
    {
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? currentUserId = int.TryParse(userIdStr, out int userId) ? userId : null;

            var response = await _productService.GetProductByIdAsync(id.Value, currentUserId);
            if (!response.Success || response.Data == null) return NotFound();

            List<Cart> userCart = [];
            if (currentUserId.HasValue)
            {
                var cartItems = await _cartService.GetCartItemsAsync(currentUserId.Value);
                userCart = cartItems.ToList();
            }
            ViewData["UserCart"] = userCart;

            return View(response.Data);
        }
    }
}
