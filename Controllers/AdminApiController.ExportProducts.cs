using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Wrappers;
using System;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpGet("export/products")]
        public async Task<IActionResult> ExportProducts(string format = "csv")
        {
            var response = await _productService.GetAllProductsAsync();
            if (!response.Success || response.Data == null) return BadRequest(response);

            byte[] data;
            string contentType;
            string fileName = $"Products_{DateTime.Now:yyyyMMdd}";

            if (format.ToLower() == "excel")
            {
                data = _fileService.ExportProductsToExcel(response.Data!);
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileName += ".xlsx";
            }
            else
            {
                data = _fileService.ExportProductsToCsv(response.Data!);
                contentType = "text/csv";
                fileName += ".csv";
            }

            return File(data, contentType, fileName);
        }
    }
}
