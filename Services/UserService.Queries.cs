using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.DTOs;
using ShoppingApp.Models;

namespace ShoppingApp.Services
{
    public partial class UserService
    {
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(string? searchTerm = null)
        {
            var query = _unitOfWork.Repository<User>().GetQueryable();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u => 
                    (u.FirstName != null && u.FirstName.Contains(searchTerm)) || 
                    (u.LastName != null && u.LastName.Contains(searchTerm)) || 
                    (u.Email != null && u.Email.Contains(searchTerm)) ||
                    (u.Phone != null && u.Phone.Contains(searchTerm)));
            }

            var users = await query.ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users) ?? [];
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<User?> GetUserEntityByIdAsync(int id)
        {
            return await _unitOfWork.Repository<User>().GetByIdAsync(id);
        }
    }
}
