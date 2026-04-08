using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;

namespace ShoppingApp.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    bool UserExists(int id);
    Task<User?> LoginAsync(string email, string password);
    Task RegisterAsync(User user);
}

public class UserService : IUserService
{
    private readonly ShoppingDbContext _context;

    public UserService(ShoppingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
    }

    public async Task CreateUserAsync(User user)
    {
        _context.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
        }
        await _context.SaveChangesAsync();
    }

    public bool UserExists(int id)
    {
        return _context.Users.Any(e => e.UserId == id);
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
    }

    public async Task RegisterAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
