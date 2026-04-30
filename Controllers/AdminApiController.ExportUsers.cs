using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class AdminApiController : ControllerBase
    {
        [HttpGet("export/users")]
        public async Task<IActionResult> ExportUsers(string format = "csv")
        {
            var users = await _userService.GetAllUsersAsync();
            if (users == null) users = [];
            
            byte[] data;
            string contentType;
            string fileName = $"Users_{DateTime.Now:yyyyMMdd}";

            if (format.ToLower() == "excel")
            {
                data = _fileService.ExportUsersToExcel(users);
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileName += ".xlsx";
            }
            else
            {
                data = _fileService.ExportUsersToCsv(users);
                contentType = "text/csv";
                fileName += ".csv";
            }

            return File(data, contentType, fileName);
        }
    }
}
