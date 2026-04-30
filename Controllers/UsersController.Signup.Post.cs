using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class UsersController : Controller
    {
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
    }
}
