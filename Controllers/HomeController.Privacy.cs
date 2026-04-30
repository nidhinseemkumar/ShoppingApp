using Microsoft.AspNetCore.Mvc;

namespace ShoppingApp.Controllers
{
    public partial class HomeController : Controller
    {
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
