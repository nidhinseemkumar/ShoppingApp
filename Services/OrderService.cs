using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly ShoppingDbContext _context;

        public OrderService(ShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId, int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
        }

        public async Task BuyNowAsync(int productId, int userId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return;

            var newOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = product.Price,
                Status = "Paid"
            };
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            var orderDetail = new OrderItem
            {
                OrderId = newOrder.OrderId,
                ProductId = productId,
                Quantity = 1,
                Price = product.Price
            };
            _context.OrderItems.Add(orderDetail);
            
            // Decrement Stock
            product.Stock -= 1;
            _context.Products.Update(product);
            
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalSalesAsync()
        {
            return await _context.Orders.SumAsync(o => o.TotalAmount ?? 0);
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }
    }
}
