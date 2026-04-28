using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Claims;
using ShoppingApp.Services;

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
            var productsResponse = await _productService.GetAllProductsAsync();
            var categories = await _productService.GetCategoriesAsync();
            
            var userCart = new List<Cart>();
            var userIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("Home/Error/{statusCode?}")]
        public IActionResult Error(int? statusCode)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            if (statusCode == 404)
            {
                ViewData["ErrorMessage"] = "Oops! The page you're looking for doesn't exist.";
            }
            else if (statusCode == 403)
            {
                ViewData["ErrorMessage"] = "You don't have permission to access this resource.";
            }
            else
            {
                ViewData["ErrorMessage"] = "Something went wrong on our end. We're working on it!";
            }

            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
