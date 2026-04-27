using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using ShoppingApp.Services;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public UsersController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        // GET: Users (Admin only functionality)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? searchTerm)
        {
            ViewBag.CurrentSearch = searchTerm;
            return View(await _userService.GetAllUsersAsync(searchTerm));
        }

        // GET: Users/Profile
        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Signup
        public IActionResult Signup()
        {
            return View();
        }

        // POST: Users/Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup([Bind("FirstName,LastName,Email,Password,Phone,Address")] User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.RegisterAsync(user);
                    return RedirectToAction(nameof(Login));
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(user);
        }

        // GET: Users/Login
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userService.LoginAsync(email, password);
            if (user != null)
            {
                // Generate JWT Token
                var token = _tokenService.CreateToken(user);
                
                // Store JWT in a cookie for future API use / security branding
                Response.Cookies.Append("jwt_token", token, new CookieOptions { HttpOnly = true, Secure = true });

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role ?? "Customer")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View(new User { Email = email });
        }

        // POST: Users/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("jwt_token");
            return RedirectToAction("Index", "Home");
        }

        // Standard CRUD (Edits for profile, etc)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userService.GetUserByIdAsync(id.Value);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,Email,Phone,Address")] User user)
        {
            if (id != user.UserId) return NotFound();
            
            // Remove Password and Email from validation as they are not being edited here
            ModelState.Remove("Password");
            ModelState.Remove("Email");

            if (ModelState.IsValid)
            {
                await _userService.UpdateUserAsync(user);

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
            return View(user);
        }

        // POST: Users/ToggleRole
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRole(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var userEntity = await _userService.GetUserEntityByIdAsync(id);
            if (userEntity != null)
            {
                userEntity.Role = userEntity.Role == "Admin" ? "Customer" : "Admin";
                await _userService.UpdateUserAsync(userEntity);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Users/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
