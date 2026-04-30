using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Wrappers;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpPost("import/products")]
        public async Task<IActionResult> ImportProducts(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new ApiResponse<string>("No file uploaded"));
            if (file.Length > 5 * 1024 * 1024) return BadRequest(new ApiResponse<string>("File too large (max 5MB)"));

            using var stream = file.OpenReadStream();
            int count = 0;
            if (file.FileName.EndsWith(".csv")) count = await _fileService.ImportProductsFromCsv(stream);
            else if (file.FileName.EndsWith(".xlsx")) count = await _fileService.ImportProductsFromExcel(stream);
            else return BadRequest(new ApiResponse<string>("Invalid file format (.csv or .xlsx only)"));

            return Ok(new ApiResponse<int>(count, $"{count} products imported successfully"));
        }
    }
}
