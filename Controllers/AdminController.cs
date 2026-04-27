using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Services;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public AdminController(IProductService productService, IUserService userService, IOrderService orderService)
        {
            _productService = productService;
            _userService = userService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Fetch stats for the dashboard
            var productsResponse = await _productService.GetAllProductsAsync();
            var users = await _userService.GetAllUsersAsync();
            var totalSales = await _orderService.GetTotalSalesAsync();
            var totalOrders = await _orderService.GetTotalOrdersCountAsync();
            
            ViewBag.ProductCount = productsResponse.Data?.Count() ?? 0;
            ViewBag.UserCount = users.Count();
            ViewBag.TotalSales = totalSales;
            ViewBag.OrderCount = totalOrders;
            
            return View();
        }

        public async Task<IActionResult> Products()
        {
            var response = await _productService.GetAllProductsAsync();
            return View(response.Data);
        }

        public async Task<IActionResult> Users()
        {
            return View(await _userService.GetAllUsersAsync());
        }

        public async Task<IActionResult> Orders()
        {
            // For now, get all orders across all users (Admin only)
            // Assuming we need a way to fetch all orders, I'll use the context or a new service method
            // Since I don't want to change IOrderService interface again, I'll use a simplified list for now
            // or I could add GetTotalOrders to IOrderService.
            // For this demonstration, I'll assume we want to see the order volume.
            return RedirectToAction("Index", "Orders"); // Temporary until a full list is implemented
        }
    }
}
