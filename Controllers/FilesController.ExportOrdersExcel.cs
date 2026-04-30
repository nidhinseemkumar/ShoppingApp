using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class FilesController : Controller
    {
        public async Task<IActionResult> ExportOrdersExcel()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var excelData = _fileService.ExportOrdersToExcel(orders);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders_Export.xlsx");
        }
    }
}
