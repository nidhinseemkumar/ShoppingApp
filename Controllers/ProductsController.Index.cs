using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public partial class ProductsController : Controller
    {
        public async Task<IActionResult> Index(string? searchTerm, string? category)
        {
            var response = await _productService.GetAllProductsAsync(searchTerm, category);
            
            List<Cart> userCart = [];
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                userCart = cartItems.ToList();
            }

            var products = response.Data;
            if (User.IsInRole("Customer"))
            {
                products = products?.Where(p => p.Stock > 0).ToList();
            }

            ViewData["CurrentSearch"] = searchTerm;
            ViewData["CurrentCategory"] = category;
            ViewData["UserCart"] = userCart;
            
            return View(products);
        }
    }
}
