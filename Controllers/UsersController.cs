using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using ShoppingApp.Services;
using ShoppingApp.Wrappers;
using ShoppingApp.DTOs;
using AutoMapper;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public class UsersController(IUserService userService, ITokenService tokenService, IMapper mapper) : Controller
    {
        private readonly IUserService _userService = userService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IMapper _mapper = mapper;


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

            var user = await _userService.GetUserEntityByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = _mapper.Map<UserDto>(user);
            return View(userDto);
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
                    var response = await _userService.RegisterAsync(user);
                    if (response.Success) return RedirectToAction(nameof(Login));
                    ModelState.AddModelError(string.Empty, response.Message ?? "Registration failed");
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
                    IsPersistent = false
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View();
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

        // POST: Users/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _userService.DeleteUserAsync(id);
            if (!response.Success)
            {
                // Optionally handle error (e.g., TempData["Error"] = response.Message)
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
