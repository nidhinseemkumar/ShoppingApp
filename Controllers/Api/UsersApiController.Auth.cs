using Microsoft.AspNetCore.Mvc;
using ShoppingApp.DTOs;
using ShoppingApp.Wrappers;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers.Api
{
    public partial class UsersApiController : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Register(UserRegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var response = await _userService.RegisterUserAsync(registerDto);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
