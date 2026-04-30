using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(new ApiResponse<IEnumerable<Order>>(orders, "Orders retrieved successfully"));
        }
    }
}
