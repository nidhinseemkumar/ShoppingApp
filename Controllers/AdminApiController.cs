using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShoppingApp.Services;
using ShoppingApp.DTOs;
using ShoppingApp.Wrappers;
using ShoppingApp.Models;

namespace ShoppingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public partial class AdminApiController(
        IProductService productService,
        IUserService userService,
        IOrderService orderService,
        IFileService fileService,
        ILogger<AdminApiController> logger) : ControllerBase
    {
        private readonly IProductService _productService = productService;
        private readonly IUserService _userService = userService;
        private readonly IOrderService _orderService = orderService;
        private readonly IFileService _fileService = fileService;
        private readonly ILogger<AdminApiController> _logger = logger;
    }
}
