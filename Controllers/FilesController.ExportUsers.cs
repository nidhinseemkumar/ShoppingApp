using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class FilesController : Controller
    {
        public async Task<IActionResult> ExportUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var csvData = _fileService.ExportUsersToCsv(users);
            return File(csvData, "text/csv", "Users_Export.csv");
        }
    }
}
