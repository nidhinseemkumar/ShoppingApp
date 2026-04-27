using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using ShoppingApp.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public partial class CartsController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public CartsController(ICartService cartService, IProductService productService, IUserService userService, IOrderService orderService)
        {
            _cartService = cartService;
            _productService = productService;
            _userService = userService;
            _orderService = orderService;
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

        public IActionResult Checkout()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Users");
            }
            return RedirectToAction("Index", "Payments");
        }

        public async Task<IActionResult> BuyNow(int productId)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Users", new { returnUrl = Request.Path + Request.QueryString });
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                // We create a temporary order or just pass the product ID to the payment page
                // For simplicity, we'll let the user choose payment for this product
                return RedirectToAction("Index", "Payments", new { productId = productId });
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
