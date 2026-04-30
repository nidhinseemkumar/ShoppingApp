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

    public partial class UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger) : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UserService> _logger = logger;
    }
}
