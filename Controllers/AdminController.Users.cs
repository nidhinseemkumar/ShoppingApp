using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminController : Controller
    {
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users ?? []);
        }
    }
}
