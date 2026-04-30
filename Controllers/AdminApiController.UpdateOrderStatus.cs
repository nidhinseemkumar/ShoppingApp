using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Wrappers;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status);
            return result ? Ok(new ApiResponse<bool>(true, "Order status updated")) : BadRequest(new ApiResponse<string>("Failed to update status"));
        }
    }
}
