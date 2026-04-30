using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using System.Threading.Tasks;

namespace ShoppingApp.Services
{
    public partial class CartService
    {
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
    }
}
