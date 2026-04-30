using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ShoppingApp.Models;

namespace ShoppingApp.Controllers
{
    public partial class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var productsResponse = await _productService.GetAllProductsAsync();
            var categories = await _productService.GetCategoriesAsync();
            
            var userCart = new List<Cart>();
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                userCart = cartItems.ToList();
            }

            var products = productsResponse.Data;
            if (User.IsInRole("Customer"))
            {
                products = products?.Where(p => p.Stock > 0).ToList();
            }

            ViewData["Categories"] = categories;
            ViewData["UserCart"] = userCart;
            return View(products?.Take(12));
        }
    }
}
