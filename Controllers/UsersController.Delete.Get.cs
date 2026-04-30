using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.DTOs;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class UsersController : Controller
    {
        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userService.GetUserEntityByIdAsync(id.Value);
            if (user == null) return NotFound();
            var userDto = _mapper.Map<UserDto>(user);
            return View(userDto);
        }
    }
}
