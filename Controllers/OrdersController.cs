using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Services;
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

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var orders = await _orderService.GetUserOrdersAsync(userId);
                return View(orders);
            }
            return RedirectToAction("Login", "Users");
        }
    }
}
