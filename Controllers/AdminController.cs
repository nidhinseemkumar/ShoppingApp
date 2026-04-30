using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Services;
using ShoppingApp.DTOs;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ShoppingApp.Wrappers;

namespace ShoppingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public partial class AdminController(
        IProductService productService, 
        IUserService userService, 
        IOrderService orderService,
        IReviewService reviewService) : Controller
    {
        private readonly IProductService _productService = productService;
        private readonly IUserService _userService = userService;
        private readonly IOrderService _orderService = orderService;
        private readonly IReviewService _reviewService = reviewService;
    }
}
