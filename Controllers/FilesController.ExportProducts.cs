using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class FilesController : Controller
    {
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
    }
}
