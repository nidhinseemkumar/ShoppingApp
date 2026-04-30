using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ShoppingApp.DTOs;
using ShoppingApp.Models;

namespace ShoppingApp.Services
{
    public partial class FileService
    {
        public byte[] ExportProductsToCsv(IEnumerable<ProductDto> products)
        {
            var builder = new StringBuilder();
            builder.AppendLine("ProductId,Name,Price,Stock,CategoryName,Description,ImageUrl");

            foreach (var p in products)
            {
                builder.AppendLine($"{p.ProductId},\"{p.Name}\",{p.Price},{p.Stock},\"{p.CategoryName}\",\"{p.Description}\",\"{p.ImageUrl}\"");
            }

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public byte[] ExportUsersToCsv(IEnumerable<UserDto> users)
        {
            var builder = new StringBuilder();
            builder.AppendLine("UserId,FirstName,LastName,Email,Phone,Role,Address");

            foreach (var u in users)
            {
                builder.AppendLine($"{u.UserId},\"{u.FirstName}\",\"{u.LastName}\",\"{u.Email}\",\"{u.Phone}\",\"{u.Role}\",\"{u.Address}\"");
            }

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public byte[] ExportProductsToExcel(IEnumerable<ProductDto> products)
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Products");
            worksheet.Cell(1, 1).Value = "ProductId";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Price";
            worksheet.Cell(1, 4).Value = "Stock";
            worksheet.Cell(1, 5).Value = "Category";
            worksheet.Cell(1, 6).Value = "Description";

            int row = 2;
            foreach (var p in products)
            {
                worksheet.Cell(row, 1).Value = p.ProductId;
                worksheet.Cell(row, 2).Value = p.Name;
                worksheet.Cell(row, 3).Value = p.Price;
                worksheet.Cell(row, 4).Value = p.Stock;
                worksheet.Cell(row, 5).Value = p.CategoryName;
                worksheet.Cell(row, 6).Value = p.Description;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportUsersToExcel(IEnumerable<UserDto> users)
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Users");
            worksheet.Cell(1, 1).Value = "UserId";
            worksheet.Cell(1, 2).Value = "FirstName";
            worksheet.Cell(1, 3).Value = "LastName";
            worksheet.Cell(1, 4).Value = "Email";
            worksheet.Cell(1, 5).Value = "Phone";
            worksheet.Cell(1, 6).Value = "Role";

            int row = 2;
            foreach (var u in users)
            {
                worksheet.Cell(row, 1).Value = u.UserId;
                worksheet.Cell(row, 2).Value = u.FirstName;
                worksheet.Cell(row, 3).Value = u.LastName;
                worksheet.Cell(row, 4).Value = u.Email;
                worksheet.Cell(row, 5).Value = u.Phone;
                worksheet.Cell(row, 6).Value = u.Role;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportOrdersToCsv(IEnumerable<Order> orders)
        {
            var builder = new StringBuilder();
            builder.AppendLine("OrderId,CustomerName,Email,OrderDate,TotalAmount,Status,Items");

            foreach (var o in orders)
            {
                var customerName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : "Unknown";
                var items = string.Join(" | ", o.OrderItems.Select(oi => $"{oi.Product?.Name} (x{oi.Quantity})"));
                builder.AppendLine($"{o.OrderId},\"{customerName}\",\"{o.User?.Email}\",\"{o.OrderDate}\",{o.TotalAmount},\"{o.Status}\",\"{items}\"");
            }

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public byte[] ExportOrdersToExcel(IEnumerable<Order> orders)
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Orders");
            worksheet.Cell(1, 1).Value = "OrderId";
            worksheet.Cell(1, 2).Value = "Customer";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Date";
            worksheet.Cell(1, 5).Value = "Amount";
            worksheet.Cell(1, 6).Value = "Status";
            worksheet.Cell(1, 7).Value = "Items";

            int row = 2;
            foreach (var o in orders)
            {
                worksheet.Cell(row, 1).Value = o.OrderId;
                worksheet.Cell(row, 2).Value = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : "Unknown";
                worksheet.Cell(row, 3).Value = o.User?.Email;
                worksheet.Cell(row, 4).Value = o.OrderDate;
                worksheet.Cell(row, 5).Value = o.TotalAmount;
                worksheet.Cell(row, 6).Value = o.Status;
                worksheet.Cell(row, 7).Value = string.Join(", ", o.OrderItems.Select(oi => $"{oi.Product?.Name} (x{oi.Quantity})"));
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
