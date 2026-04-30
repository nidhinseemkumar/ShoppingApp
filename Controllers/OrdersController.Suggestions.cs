using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class OrdersController : Controller
    {
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
    }
}
