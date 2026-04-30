using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.DTOs;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class UsersController : Controller
    {
        // POST: Users/ToggleRole
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRole(int id)
        {
            var user = await _userService.GetUserEntityByIdAsync(id);
            if (user == null) return NotFound();

            var userEntity = await _userService.GetUserEntityByIdAsync(id);
            if (userEntity != null)
            {
                userEntity.Role = userEntity.Role == "Admin" ? "Customer" : "Admin";
                var userDto = _mapper.Map<UserDto>(userEntity);
                await _userService.UpdateUserAsync(userDto);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
