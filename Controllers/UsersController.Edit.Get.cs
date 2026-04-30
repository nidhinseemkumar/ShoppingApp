using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class UsersController : Controller
    {
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            
            if (currentUserIdStr != id.ToString() && !isAdmin)
            {
                return Forbid();
            }

            var user = await _userService.GetUserEntityByIdAsync(id.Value);
            if (user == null) return NotFound();
            return View(user);
        }
    }
}
