using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public partial class PaymentsController : Controller
    {
        public async Task<IActionResult> Index(int? orderId, int? productId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Users");

            decimal amount = 0;
            if (orderId.HasValue)
            {
                var order = await _orderService.GetOrderByIdAsync(orderId.Value, userId);
                if (order == null || order.UserId != userId) return NotFound();
                amount = order.TotalAmount ?? 0;
            }
            else if (productId.HasValue)
            {
                var productResponse = await _productService.GetProductByIdAsync(productId.Value);
                if (!productResponse.Success) return NotFound();
                amount = productResponse.Data?.Price ?? 0;
            }
            else
            {
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                amount = cartItems.Sum(c => (c.Product?.Price ?? 0) * (c.Quantity ?? 0));
            }

            ViewBag.OrderId = orderId;
            ViewBag.ProductId = productId;
            ViewBag.Amount = amount;
            return View();
        }
    }
}
