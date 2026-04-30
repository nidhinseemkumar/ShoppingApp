using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return order != null ? Ok(new ApiResponse<Order>(order)) : NotFound(new ApiResponse<string>("Order not found"));
        }
    }
}
