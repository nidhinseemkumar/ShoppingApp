using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public partial class OrdersController : Controller
    {
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
