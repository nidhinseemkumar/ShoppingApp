using Microsoft.AspNetCore.Mvc;

namespace ShoppingApp.Controllers
{
    public partial class CartsController : Controller
    {
        public IActionResult Checkout()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Users");
            }
            return RedirectToAction("Index", "Payments");
        }
    }
}
