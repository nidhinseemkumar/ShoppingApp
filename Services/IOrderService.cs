using ShoppingApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingApp.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int orderId, int userId);
        Task BuyNowAsync(int productId, int userId);
        Task<decimal> GetTotalSalesAsync();
        Task<int> GetTotalOrdersCountAsync();
    }
}
