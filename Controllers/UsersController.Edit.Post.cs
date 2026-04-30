using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using ShoppingApp.DTOs;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class UsersController : Controller
    {
        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,Email,Phone,Address,Role")] User user)
        {
            if (id != user.UserId) return NotFound();

            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            // Security: Allow if editing self OR if user is admin
            if (currentUserIdStr != user.UserId.ToString() && !isAdmin)
            {
                return Forbid();
            }
            
            // Remove Password from validation
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                var userDto = _mapper.Map<UserDto>(user);
                var response = await _userService.UpdateUserAsync(userDto);

                if (!response.Success)
                {
                    ModelState.AddModelError(string.Empty, response.Message ?? "Update failed");
                    return View(user);
                }

                // Only refresh cookie if the user edited their OWN profile
                if (currentUserIdStr == user.UserId.ToString())
                {
                    // Refresh the authentication cookie with the new name
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                        new Claim(ClaimTypes.Email, user.Email ?? ""),
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Role, user.Role ?? "Customer")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction(nameof(Profile));
                }

                // If Admin edited another user, go back to Admin Users list
                return RedirectToAction("Users", "Admin");
            }
            return View(user);
        }
    }
}
