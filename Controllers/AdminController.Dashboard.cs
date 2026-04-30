using Microsoft.AspNetCore.Mvc;

namespace ShoppingApp.Controllers
{
    public partial class AdminController : Controller
    {
        public async Task<IActionResult> Dashboard()
        {
            // Fetch stats for the dashboard
            var productsResponse = await _productService.GetAllProductsAsync();
            var users = await _userService.GetAllUsersAsync();
            var totalSales = await _orderService.GetTotalSalesAsync();
            var totalOrders = await _orderService.GetTotalOrdersCountAsync();
            
            ViewBag.ProductCount = productsResponse?.Data?.Count() ?? 0;
            ViewBag.UserCount = users?.Count() ?? 0;
            ViewBag.TotalSales = totalSales;
            ViewBag.OrderCount = totalOrders;
            
            // New Insights
            ViewBag.RecentOrders = await _orderService.GetRecentOrdersAsync(5) ?? [];
            ViewBag.SalesTrend = await _orderService.GetSalesTrendAsync(7) ?? [];
            ViewBag.TopProducts = await _orderService.GetTopSellingProductsAsync(5) ?? [];
            ViewBag.CategorySales = await _orderService.GetSalesByCategoryAsync() ?? [];
            ViewBag.LowStockProducts = await _productService.GetLowStockProductsAsync(10) ?? [];
            
            return View();
        }
    }
}
