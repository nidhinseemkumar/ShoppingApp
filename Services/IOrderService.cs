using ShoppingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingApp.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetUserOrdersAsync(int userId, string? searchTerm = null, DateTime? startDate = null, DateTime? endDate = null, decimal? minPrice = null, decimal? maxPrice = null, string? sortBy = null, bool isDescending = true);
        Task<Order?> GetOrderByIdAsync(int orderId, int userId);
        Task<Order?> GetOrderByIdAsync(int orderId); // Admin version
        Task<IEnumerable<Order>> GetAllOrdersAsync(string? searchTerm = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task BuyNowAsync(int productId, int userId);
        Task<decimal> GetTotalSalesAsync();
        Task<int> GetTotalOrdersCountAsync();
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count);
        Task<IEnumerable<object>> GetSalesTrendAsync(int days);
        Task<IEnumerable<object>> GetTopSellingProductsAsync(int count);
        Task<IEnumerable<object>> GetSalesByCategoryAsync();
        Task<IEnumerable<string>> GetOrderHistorySuggestionsAsync(int userId, string term);
    }
}
