using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Claims;
using ShoppingApp.Services;

namespace ShoppingApp.Controllers
{
    public partial class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }
    }
}
