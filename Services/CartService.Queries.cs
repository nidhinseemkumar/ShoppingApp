using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Services
{
    public partial class CartService
    {
        public async Task<IEnumerable<Cart>> GetCartItemsAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .Include(c => c.User)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Cart?> GetCartItemAsync(int id)
        {
            return await _context.Carts.Include(c => c.Product).FirstOrDefaultAsync(c => c.CartId == id);
        }

        public async Task<int> GetCartCountAsync(int userId)
        {
            return await _context.Carts.Where(c => c.UserId == userId).SumAsync(c => c.Quantity) ?? 0;
        }
    }
}
