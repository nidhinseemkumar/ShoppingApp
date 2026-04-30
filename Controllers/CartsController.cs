using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using ShoppingApp.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public partial class CartsController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public CartsController(ICartService cartService, IProductService productService, IUserService userService, IOrderService orderService)
        {
            _cartService = cartService;
            _productService = productService;
            _userService = userService;
            _orderService = orderService;
        }
    }
}
