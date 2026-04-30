using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class UsersController : Controller
    {
        // GET: Users (Admin only functionality)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? searchTerm)
        {
            ViewBag.CurrentSearch = searchTerm;
            return View(await _userService.GetAllUsersAsync(searchTerm));
        }
    }
}
