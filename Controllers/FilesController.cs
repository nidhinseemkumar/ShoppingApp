using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Services;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FilesController(IFileService fileService, IProductService productService, IUserService userService, IOrderService orderService) : Controller
    {
        private readonly IFileService _fileService = fileService;
        private readonly IProductService _productService = productService;
        private readonly IUserService _userService = userService;
        private readonly IOrderService _orderService = orderService;


        public async Task<IActionResult> ExportProducts()
        {
            var response = await _productService.GetAllProductsAsync();
            if (!response.Success || response.Data == null)
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            var csvData = _fileService.ExportProductsToCsv(response.Data);
            return File(csvData, "text/csv", "Products_Export.csv");
        }

        public async Task<IActionResult> ExportUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var csvData = _fileService.ExportUsersToCsv(users);
            return File(csvData, "text/csv", "Users_Export.csv");
        }

        public async Task<IActionResult> ExportOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var csvData = _fileService.ExportOrdersToCsv(orders);
            return File(csvData, "text/csv", "Orders_Export.csv");
        }

        public async Task<IActionResult> ExportOrdersExcel()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var excelData = _fileService.ExportOrdersToExcel(orders);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders_Export.xlsx");
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
