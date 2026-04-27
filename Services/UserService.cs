using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.DTOs;
using ShoppingApp.Models;
using ShoppingApp.Repositories;

namespace ShoppingApp.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(string? searchTerm = null);
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> RegisterDtoAsync(UserRegisterDto registerDto);
        Task<User?> LoginAsync(string email, string password);
        Task RegisterAsync(User user);
        Task UpdateUserAsync(User user);
        Task<User?> GetUserEntityByIdAsync(int id);
        Task DeleteUserAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

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
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> RegisterDtoAsync(UserRegisterDto registerDto)
        {
            var existingUser = await _unitOfWork.Repository<User>()
                .GetQueryable()
                .AnyAsync(u => u.Email == registerDto.Email || u.Phone == registerDto.Phone);

            if (existingUser)
            {
                throw new System.Exception("User with this email or phone already exists.");
            }

            var user = _mapper.Map<User>(registerDto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            user.Role = "Customer";

            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserDto>(user);
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

        public async Task RegisterAsync(User user)
        {
            var existingUser = await _unitOfWork.Repository<User>()
                .GetQueryable()
                .AnyAsync(u => u.Email == user.Email || u.Phone == user.Phone);

            if (existingUser)
            {
                throw new System.Exception("User with this email or phone already exists.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Role = "Customer";
            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _unitOfWork.Repository<User>().GetByIdAsync(user.UserId);
            if (existingUser != null)
            {
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Phone = user.Phone;
                existingUser.Address = user.Address;
                // We don't update Email or Password here as they are sensitive/readonly in this flow
                
                _unitOfWork.Repository<User>().Update(existingUser);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<User?> GetUserEntityByIdAsync(int id)
        {
            return await _unitOfWork.Repository<User>().GetByIdAsync(id);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(id);
            if (user != null)
            {
                _unitOfWork.Repository<User>().Delete(user);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
