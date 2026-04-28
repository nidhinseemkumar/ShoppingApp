using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Services;
using ShoppingApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

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

        [HttpGet]
        public async Task<IActionResult> Suggestions(string term)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var suggestions = await _orderService.GetOrderHistorySuggestionsAsync(userId, term);
                return Json(suggestions);
            }
            return BadRequest();
        }
        public async Task<IActionResult> Details(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                Order? order;
                if (User.IsInRole("Admin"))
                {
                    order = await _orderService.GetOrderByIdAsync(id);
                }
                else
                {
                    order = await _orderService.GetOrderByIdAsync(id, userId);
                }

                if (order == null)
                {
                    return NotFound();
                }

                return View(order);
            }
            return RedirectToAction("Login", "Users");
        }
    }
}
