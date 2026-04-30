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

    public partial class CartService(ShoppingDbContext context, ILogger<CartService> logger) : ICartService
    {
        private readonly ShoppingDbContext _context = context;
        private readonly ILogger<CartService> _logger = logger;
    }
}
