using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminController : Controller
    {
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
    }
}
