using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Services;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FilesController : Controller
    {
        private readonly IFileService _fileService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public FilesController(IFileService fileService, IProductService productService, IUserService userService)
        {
            _fileService = fileService;
            _productService = productService;
            _userService = userService;
        }

        public async Task<IActionResult> ExportProducts()
        {
            var response = await _productService.GetAllProductsAsync();
            var csvData = _fileService.ExportProductsToCsv(response.Data);
            return File(csvData, "text/csv", "Products_Export.csv");
        }

        public async Task<IActionResult> ExportUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var csvData = _fileService.ExportUsersToCsv(users);
            return File(csvData, "text/csv", "Users_Export.csv");
        }

        [HttpPost]
        public async Task<IActionResult> ImportProducts(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var count = await _fileService.ImportProductsFromCsv(stream);
                TempData["Message"] = $"{count} products imported successfully.";
            }
            return RedirectToAction("Dashboard", "Admin");
        }
    }
}
