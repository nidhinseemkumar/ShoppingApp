using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.Services;
using System.Security.Claims;
using System.Diagnostics;

namespace ShoppingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            
            if (User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out int userId))
                {
                    var cartItems = await _cartService.GetCartItemsAsync(userId);
                    ViewBag.CartItems = cartItems.ToDictionary(c => c.ProductId.Value, c => c.Quantity.Value);
                }
            }
            
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
