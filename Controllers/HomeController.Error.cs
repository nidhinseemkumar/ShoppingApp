using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using System.Diagnostics;

namespace ShoppingApp.Controllers
{
    public partial class HomeController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("Home/Error/{statusCode?}")]
        public IActionResult Error(int? statusCode)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            if (statusCode == 404)
            {
                ViewData["ErrorMessage"] = "Oops! The page you're looking for doesn't exist.";
            }
            else if (statusCode == 403)
            {
                ViewData["ErrorMessage"] = "You don't have permission to access this resource.";
            }
            else
            {
                ViewData["ErrorMessage"] = "Something went wrong on our end. We're working on it!";
            }

            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
