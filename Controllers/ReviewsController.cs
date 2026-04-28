using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.DTOs;
using ShoppingApp.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public class ReviewsController(IReviewService reviewService) : Controller
    {
        private readonly IReviewService _reviewService = reviewService;

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
