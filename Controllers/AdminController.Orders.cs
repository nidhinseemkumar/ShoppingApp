using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminController : Controller
    {
        public async Task<IActionResult> Orders(string? searchTerm, string? status, DateTime? startDate, DateTime? endDate)
        {
            var orders = await _orderService.GetAllOrdersAsync(searchTerm, status, startDate, endDate);
            ViewBag.CurrentSearch = searchTerm;
            ViewBag.CurrentStatus = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            return View(orders);
        }
    }
}
