using Microsoft.AspNetCore.Mvc;
using ShoppingApp.DTOs;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public partial class ReviewsController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitReview(ReviewCreateUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Products", new { id = model.ProductId });
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Users");
            }

            var response = await _reviewService.AddOrUpdateReviewAsync(model, userId);
            
            TempData["ReviewMessage"] = response.Message;
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }
    }
}
