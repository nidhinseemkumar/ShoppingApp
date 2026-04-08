using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using ShoppingApp.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public partial class CartsController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public CartsController(ICartService cartService, IProductService productService, IUserService userService)
        {
            _cartService = cartService;
            _productService = productService;
            _userService = userService;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                return View(await _cartService.GetCartItemsAsync(userId));
            }
            return RedirectToAction("Login", "Users");
        }

        public async Task<IActionResult> Checkout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Users");
            }

            var userIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                await _cartService.CheckoutAsync(userId);
            }
            
            return RedirectToAction("Index", "Orders");
        }
    }
}
