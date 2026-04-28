using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.DTOs;
using ShoppingApp.Models;
using ShoppingApp.Repositories;
using ShoppingApp.Wrappers;

namespace ShoppingApp.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(string? searchTerm = null);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<ApiResponse<UserDto>> RegisterUserAsync(UserRegisterDto registerDto);
        Task<User?> LoginAsync(string email, string password);
        Task<ApiResponse<bool>> RegisterAsync(User user);
        Task<ApiResponse<bool>> UpdateUserAsync(UserDto userDto);
        Task<User?> GetUserEntityByIdAsync(int id);
        Task<ApiResponse<bool>> DeleteUserAsync(int id);
    }

    public class UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger) : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UserService> _logger = logger;

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

        public async Task<ApiResponse<UserDto>> RegisterUserAsync(UserRegisterDto registerDto)
        {
            try
            {
                var existingUser = await _unitOfWork.Repository<User>()
                    .GetQueryable()
                    .AnyAsync(u => u.Email == registerDto.Email || u.Phone == registerDto.Phone);

                if (existingUser) return new ApiResponse<UserDto>("User with this email or phone already exists.");

                var user = _mapper.Map<User>(registerDto);
                user.Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                user.Role = "Customer";

                await _unitOfWork.Repository<User>().AddAsync(user);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<UserDto>(_mapper.Map<UserDto>(user)!, "Registration successful");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return new ApiResponse<UserDto>("An error occurred during registration");
            }
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _unitOfWork.Repository<User>()
                .GetQueryable()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || string.IsNullOrEmpty(user.Password)) return null;

            bool isVerified = false;
            try
            {
                // Try to verify using BCrypt
                isVerified = BCrypt.Net.BCrypt.Verify(password, user.Password);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Fallback for migration: Check if it's plain text (Development only)
                if (user.Password == password)
                {
                    isVerified = true;
                    // Auto-upgrade to BCrypt hash now that we have the plain text
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    await _unitOfWork.CompleteAsync();
                }
            }

            return isVerified ? user : null;
        }

        public async Task<ApiResponse<bool>> RegisterAsync(User user)
        {
            try
            {
                var existingUser = await _unitOfWork.Repository<User>()
                    .GetQueryable()
                    .AnyAsync(u => u.Email == user.Email || u.Phone == user.Phone);

                if (existingUser) return new ApiResponse<bool>("User with this email or phone already exists.");

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Role = "Customer";
                await _unitOfWork.Repository<User>().AddAsync(user);
                await _unitOfWork.CompleteAsync();
                return new ApiResponse<bool>(true, "User registered successfully");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterAsync");
                return new ApiResponse<bool>("Failed to register user");
            }
        }

        public async Task<ApiResponse<bool>> UpdateUserAsync(UserDto userDto)
        {
            try
            {
                var existingUser = await _unitOfWork.Repository<User>().GetByIdAsync(userDto.UserId);
                if (existingUser == null) return new ApiResponse<bool>("User not found");

                existingUser.FirstName = userDto.FirstName;
                existingUser.LastName = userDto.LastName;
                existingUser.Phone = userDto.Phone;
                existingUser.Address = userDto.Address;
                if (!string.IsNullOrEmpty(userDto.Role)) existingUser.Role = userDto.Role!;
                
                _unitOfWork.Repository<User>().Update(existingUser);
                await _unitOfWork.CompleteAsync();
                return new ApiResponse<bool>(true, "User updated successfully");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", userDto.UserId);
                return new ApiResponse<bool>("Failed to update user");
            }
        }

        public async Task<User?> GetUserEntityByIdAsync(int id)
        {
            return await _unitOfWork.Repository<User>().GetByIdAsync(id);
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
                if (user == null) return new ApiResponse<bool>("User not found");

                _unitOfWork.Repository<User>().Delete(user);
                await _unitOfWork.CompleteAsync();
                return new ApiResponse<bool>(true, "User deleted successfully");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return new ApiResponse<bool>("Failed to delete user");
            }
        }
    }
}
