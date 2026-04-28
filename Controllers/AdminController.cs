using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Services;
using ShoppingApp.DTOs;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ShoppingApp.Wrappers;

namespace ShoppingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(
        IProductService productService, 
        IUserService userService, 
        IOrderService orderService,
        IReviewService reviewService) : Controller
    {
        private readonly IProductService _productService = productService;
        private readonly IUserService _userService = userService;
        private readonly IOrderService _orderService = orderService;
        private readonly IReviewService _reviewService = reviewService;


        public async Task<IActionResult> Dashboard()
        {
            // Fetch stats for the dashboard
            var productsResponse = await _productService.GetAllProductsAsync();
            var users = await _userService.GetAllUsersAsync();
            var totalSales = await _orderService.GetTotalSalesAsync();
            var totalOrders = await _orderService.GetTotalOrdersCountAsync();
            
            ViewBag.ProductCount = productsResponse?.Data?.Count() ?? 0;
            ViewBag.UserCount = users?.Count() ?? 0;
            ViewBag.TotalSales = totalSales;
            ViewBag.OrderCount = totalOrders;
            
            // New Insights
            ViewBag.RecentOrders = await _orderService.GetRecentOrdersAsync(5) ?? [];
            ViewBag.SalesTrend = await _orderService.GetSalesTrendAsync(7) ?? [];
            ViewBag.TopProducts = await _orderService.GetTopSellingProductsAsync(5) ?? [];
            ViewBag.CategorySales = await _orderService.GetSalesByCategoryAsync() ?? [];
            ViewBag.LowStockProducts = await _productService.GetLowStockProductsAsync(10) ?? [];
            
            return View();
        }

        public async Task<IActionResult> Products(string? searchTerm, string? category)
        {
            var response = await _productService.GetAllProductsAsync(searchTerm, category);
            var products = response?.Data ?? [];
            ViewBag.CurrentSearch = searchTerm;
            ViewBag.CurrentCategory = category;
            ViewBag.Categories = await _productService.GetCategoriesAsync();
            return View(products);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users ?? []);
        }

        public async Task<IActionResult> Orders(string? searchTerm, string? status, DateTime? startDate, DateTime? endDate)
        {
            var orders = await _orderService.GetAllOrdersAsync(searchTerm, status, startDate, endDate);
            ViewBag.CurrentSearch = searchTerm;
            ViewBag.CurrentStatus = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus([FromForm] int orderId, [FromForm] string status)
        {
            try
            {
                var success = await _orderService.UpdateOrderStatusAsync(orderId, status);
                return success ? Ok() : BadRequest(new { error = "Order not found or update failed" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _productService.GetCategoriesAsync();
            return View(categories ?? []);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(string categoryName)
        {
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                await _productService.GetOrCreateCategoryByNameAsync(categoryName.Trim());
                TempData["Message"] = "Category created successfully.";
            }
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(int id, string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName)) return BadRequest("Name required");
            var result = await _productService.UpdateCategoryAsync(id, categoryName.Trim());
            if (result.Success) TempData["SuccessMessage"] = result.Message;
            else TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _productService.DeleteCategoryAsync(id);
            if (result.Success) TempData["SuccessMessage"] = result.Message;
            else TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet]
        public async Task<IActionResult> FilterProducts(string? searchTerm, string? category)
        {
            var response = await _productService.GetAllProductsAsync(searchTerm, category);
            var products = response?.Data ?? [];
            return Json(products);
        }

        public async Task<IActionResult> Reviews()
        {
            var response = await _reviewService.GetAllRecentReviewsAsync(100);
            return View(response.Data ?? []);
        }
    }
}
