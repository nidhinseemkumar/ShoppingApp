using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class PaymentsController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Process(int? orderId, int? productId, string paymentMethod)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Users");

            int finalOrderId;
            decimal amount;

            if (orderId.HasValue)
            {
                finalOrderId = orderId.Value;
                var order = await _orderService.GetOrderByIdAsync(finalOrderId, userId);
                amount = order?.TotalAmount ?? 0;
            }
            else if (productId.HasValue)
            {
                // Buy Now: Create order for single product
                await _orderService.BuyNowAsync(productId.Value, userId);
                var orders = await _orderService.GetUserOrdersAsync(userId);
                var latestOrder = orders.OrderByDescending(o => o.OrderDate).FirstOrDefault();
                if (latestOrder == null) return RedirectToAction("Index", "Home");
                finalOrderId = latestOrder.OrderId;
                amount = latestOrder.TotalAmount ?? 0;
            }
            else
            {
                // Create order from cart
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                if (!cartItems.Any()) return RedirectToAction("Index", "Home");
                
                amount = cartItems.Sum(c => (c.Product?.Price ?? 0) * (c.Quantity ?? 0));
                await _cartService.CheckoutAsync(userId);
                
                var orders = await _orderService.GetUserOrdersAsync(userId);
                var latestOrder = orders.OrderByDescending(o => o.OrderDate).FirstOrDefault();
                if (latestOrder == null) return RedirectToAction("Index", "Home");
                finalOrderId = latestOrder.OrderId;
            }

            await _paymentService.ProcessPaymentAsync(finalOrderId, amount, paymentMethod);

            return RedirectToAction("Index", "Orders", new { message = "Order placed successfully!" });
        }
    }
}
