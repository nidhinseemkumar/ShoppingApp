using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingApp.Services;
using ShoppingApp.DTOs;
using ShoppingApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public partial class ProductsController(IProductService productService, ICartService cartService, IFileService fileService) : Controller
    {
        private readonly IProductService _productService = productService;
        private readonly ICartService _cartService = cartService;
        private readonly IFileService _fileService = fileService;
    }
}
