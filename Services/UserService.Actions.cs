using System.Threading.Tasks;
using ShoppingApp.DTOs;
using ShoppingApp.Models;
using ShoppingApp.Wrappers;

namespace ShoppingApp.Services
{
    public partial class UserService
    {
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
