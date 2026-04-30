using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.DTOs;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;

namespace ShoppingApp.Services
{
    public partial class UserService
    {
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
    }
}
