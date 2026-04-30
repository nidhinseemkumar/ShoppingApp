using Microsoft.AspNetCore.Mvc;

namespace ShoppingApp.Controllers
{
    public partial class UsersController : Controller
    {
        // GET: Users/Login
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}
