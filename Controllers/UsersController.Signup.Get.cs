using Microsoft.AspNetCore.Mvc;

namespace ShoppingApp.Controllers
{
    public partial class UsersController : Controller
    {
        // GET: Users/Signup
        public IActionResult Signup()
        {
            return View();
        }
    }
}
