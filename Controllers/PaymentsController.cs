using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingApp.Models;
using ShoppingApp.DTOs;
using ShoppingApp.Wrappers;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public class PaymentsController(
        IPaymentService paymentService, 
        ICartService cartService, 
        IOrderService orderService, 
        IProductService productService) : Controller
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly ICartService _cartService = cartService;
        private readonly IOrderService _orderService = orderService;
        private readonly IProductService _productService = productService;


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
