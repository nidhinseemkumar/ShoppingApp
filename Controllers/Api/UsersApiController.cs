using Microsoft.AspNetCore.Mvc;
using ShoppingApp.DTOs;
using ShoppingApp.Services;
using ShoppingApp.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersApiController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;


        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new ApiResponse<IEnumerable<UserDto>>(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new ApiResponse<UserDto>("User not found"));
            return Ok(new ApiResponse<UserDto>(user));
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Register(UserRegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var response = await _userService.RegisterUserAsync(registerDto);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
