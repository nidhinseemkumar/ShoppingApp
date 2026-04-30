using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ShoppingApp.Controllers
{
    public partial class FilesController : Controller
    {
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
