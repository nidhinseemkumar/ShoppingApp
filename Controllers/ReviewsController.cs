using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.DTOs;
using ShoppingApp.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public partial class ReviewsController(IReviewService reviewService) : Controller
    {
        private readonly IReviewService _reviewService = reviewService;


    }
}
