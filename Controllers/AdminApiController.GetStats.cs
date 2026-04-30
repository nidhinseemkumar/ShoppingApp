using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingApp.Wrappers;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var productsResponse = await _productService.GetAllProductsAsync();
                var users = await _userService.GetAllUsersAsync();
                var totalSales = await _orderService.GetTotalSalesAsync();
                var totalOrders = await _orderService.GetTotalOrdersCountAsync();

                var stats = new
                {
                    ProductCount = productsResponse?.Data?.Count() ?? 0,
                    UserCount = users?.Count() ?? 0,
                    TotalRevenue = totalSales,
                    OrderCount = totalOrders,
                    RecentOrders = await _orderService.GetRecentOrdersAsync(5) ?? [],
                    SalesTrend = await _orderService.GetSalesTrendAsync(7) ?? [],
                    TopProducts = await _orderService.GetTopSellingProductsAsync(5) ?? [],
                    CategorySales = await _orderService.GetSalesByCategoryAsync() ?? []
                };

                return Ok(new ApiResponse<object>(stats, "Stats retrieved successfully"));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin stats");
                return StatusCode(500, new ApiResponse<object>("An error occurred while fetching dashboard stats"));
            }
        }
    }
}
