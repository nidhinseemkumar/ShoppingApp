using ShoppingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoppingApp.Services;

public interface ICartService
{
    Task<IEnumerable<Cart>> GetCartItemsAsync(int userId);
    Task AddToCartAsync(Cart cart);
    Task RemoveFromCartAsync(int cartId);
    Task<Cart?> GetCartItemAsync(int id);
    Task UpdateCartAsync(Cart cart);
    Task AddToCartAsync(int productId, int userId);
    Task UpdateQuantityAsync(int productId, int userId, int change);
    Task CheckoutAsync(int userId);
}

public class CartService : ICartService
{
    private readonly ShoppingDbContext _context;

    public CartService(ShoppingDbContext context)
    {
        _context = context;
    }

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

    public async Task CheckoutAsync(int userId)
    {
        var cartItems = await _context.Carts.Where(c => c.UserId == userId).ToListAsync();

        if (cartItems.Any())
        {
            var newOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = 0, // Should be calculated
                Status = "Completed"
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
                    Price = 0 // Should pull from Product
                };
                _context.OrderItems.Add(orderDetail);
                _context.Carts.Remove(item);
            }
            await _context.SaveChangesAsync();
        }
    }
}
