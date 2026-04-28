using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;

namespace ShoppingApp.Services
{
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetCartItemsAsync(int userId);
        Task AddToCartAsync(Cart cart);
        Task RemoveFromCartAsync(int cartId);
        Task<Cart?> GetCartItemAsync(int id);
        Task UpdateCartAsync(Cart cart);
        Task AddToCartAsync(int productId, int userId);
        Task UpdateQuantityAsync(int productId, int userId, int change);
        Task<int> GetCartCountAsync(int userId);
        Task CheckoutAsync(int userId);
    }

    public class CartService(ShoppingDbContext context, ILogger<CartService> logger) : ICartService
    {
        private readonly ShoppingDbContext _context = context;
        private readonly ILogger<CartService> _logger = logger;


        public async Task<IEnumerable<Cart>> GetCartItemsAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .Include(c => c.User)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task AddToCartAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int cartId)
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Cart?> GetCartItemAsync(int id)
        {
            return await _context.Carts.Include(c => c.Product).FirstOrDefaultAsync(c => c.CartId == id);
        }

        public async Task UpdateCartAsync(Cart cart)
        {
            _context.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task AddToCartAsync(int productId, int userId)
        {
            var existingItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
                _context.Update(existingItem);
            }
            else
            {
                var cartItem = new Cart
                {
                    ProductId = productId,
                    UserId = userId,
                    Quantity = 1
                };
                _context.Carts.Add(cartItem);
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int productId, int userId, int change)
        {
            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);
            if (cartItem != null)
            {
                cartItem.Quantity += change;
                if (cartItem.Quantity <= 0)
                {
                    _context.Carts.Remove(cartItem);
                }
                else
                {
                    _context.Update(cartItem);
                }
                await _context.SaveChangesAsync();
            }
            else if (change > 0)
            {
                await AddToCartAsync(productId, userId);
            }
        }

        public async Task<int> GetCartCountAsync(int userId)
        {
            return await _context.Carts.Where(c => c.UserId == userId).SumAsync(c => c.Quantity) ?? 0;
        }

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
