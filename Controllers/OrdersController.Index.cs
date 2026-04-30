using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Services;
using ShoppingApp.Models;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public partial class OrdersController : Controller
    {
        public async Task<IActionResult> Index(string? searchTerm, DateTime? startDate, DateTime? endDate, decimal? minPrice, decimal? maxPrice, string? sortBy = "date", bool isDescending = true)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var orders = await _orderService.GetUserOrdersAsync(userId, searchTerm, startDate, endDate, minPrice, maxPrice, sortBy, isDescending);
                
                ViewBag.CurrentSearch = searchTerm;
                ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;
                ViewBag.SortBy = sortBy;
                ViewBag.IsDescending = isDescending;

                return View(orders);
            }
            return RedirectToAction("Login", "Users");
        }
    }
}
