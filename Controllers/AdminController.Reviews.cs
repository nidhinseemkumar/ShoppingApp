using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminController : Controller
    {
        public async Task<IActionResult> Reviews()
        {
            var response = await _reviewService.GetAllRecentReviewsAsync(100);
            return View(response.Data ?? []);
        }
    }
}
