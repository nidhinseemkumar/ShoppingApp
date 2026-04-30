using Microsoft.AspNetCore.Mvc;
using ShoppingApp.DTOs;
using ShoppingApp.Wrappers;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser(UserRegisterDto userDto)
        {
            var response = await _userService.RegisterUserAsync(userDto);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
