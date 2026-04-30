using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class FilesController : Controller
    {
        public async Task<IActionResult> ExportOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var csvData = _fileService.ExportOrdersToCsv(orders);
            return File(csvData, "text/csv", "Orders_Export.csv");
        }
    }
}
