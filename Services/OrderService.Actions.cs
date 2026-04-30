using ShoppingApp.Models;
using System;
using System.Threading.Tasks;

namespace ShoppingApp.Services
{
    public partial class OrderService
    {
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;
            
            if (order.Status == status) return true; // Already set

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
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
                Status = "Pending"
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
    }
}
