using Microsoft.AspNetCore.Mvc;
using ShoppingApp.DTOs;
using ShoppingApp.Wrappers;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDto userDto)
        {
            userDto.UserId = id;
            var response = await _userService.UpdateUserAsync(userDto);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
