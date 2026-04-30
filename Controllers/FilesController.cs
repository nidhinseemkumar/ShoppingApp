using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Services;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public partial class FilesController(IFileService fileService, IProductService productService, IUserService userService, IOrderService orderService) : Controller
    {
        private readonly IFileService _fileService = fileService;
        private readonly IProductService _productService = productService;
        private readonly IUserService _userService = userService;
        private readonly IOrderService _orderService = orderService;



    }
}
