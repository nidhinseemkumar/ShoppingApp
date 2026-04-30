using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoppingApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Services
{
    public partial class CartService
    {
        public async Task CheckoutAsync(int userId)
        {
            try
            {
                var cartItems = await _context.Carts
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (cartItems.Any())
                {
                    decimal totalAmount = cartItems.Sum(c => (c.Product?.Price ?? 0) * (c.Quantity ?? 0));
                    
                    var newOrder = new Order
                    {
                        UserId = userId,
                        OrderDate = DateTime.Now,
                        TotalAmount = totalAmount,
                        Status = "Pending"
                    };
                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();

                    foreach (var item in cartItems)
                    {
                        var orderDetail = new OrderItem
                        {
                            OrderId = newOrder.OrderId,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Price = item.Product?.Price ?? 0
                        };
                        _context.OrderItems.Add(orderDetail);
                        _context.Carts.Remove(item);
                        
                        // Decrement Stock
                        if (item.Product != null)
                        {
                            item.Product.Stock -= item.Quantity ?? 0;
                            _context.Products.Update(item.Product);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during checkout for user ID: {UserId}", userId);
                throw;
            }
        }
    }
}
