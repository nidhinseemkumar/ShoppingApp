using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ShoppingApp.Models;
using ShoppingApp.DTOs;

namespace ShoppingApp.Services
{
    public interface IFileService
    {
        byte[] ExportProductsToCsv(IEnumerable<ProductDto> products);
        byte[] ExportUsersToCsv(IEnumerable<UserDto> users);
        Task<int> ImportProductsFromCsv(Stream fileStream);
        Task<int> ImportProductsFromExcel(Stream fileStream);
        byte[] ExportProductsToExcel(IEnumerable<ProductDto> products);
        byte[] ExportUsersToExcel(IEnumerable<UserDto> users);
        byte[] ExportOrdersToCsv(IEnumerable<Order> orders);
        byte[] ExportOrdersToExcel(IEnumerable<Order> orders);
        Task<string?> SaveImageAsync(IFormFile file);
    }

    public class FileService : IFileService
    {
        private readonly IProductService _productService;

        public FileService(IProductService productService)
        {
            _productService = productService;
        }

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

        public async Task<int> ImportProductsFromCsv(Stream fileStream)
        {
            int count = 0;
            using var reader = new StreamReader(fileStream);
            await reader.ReadLineAsync(); // Skip header

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var values = line.Split(',');
                if (values.Length >= 7)
                {
                    var product = new Product
                    {
                        Name = values[1].Trim('\"'),
                        Price = decimal.TryParse(values[2], out decimal p) ? p : 0,
                        Stock = int.TryParse(values[3], out int s) ? s : 0,
                        Description = values[5].Trim('\"'),
                        ImageUrl = values[6].Trim('\"')
                    };

                    var categoryName = values[4].Trim('\"');
                    var category = await _productService.GetOrCreateCategoryByNameAsync(categoryName);
                    product.CategoryId = category.CategoryId;

                    await _productService.CreateProductAsync(product);
                    count++;
                }
            }
            return count;
        }
        public async Task<string?> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/uploads/" + uniqueFileName;
        }

        public async Task<int> ImportProductsFromExcel(Stream fileStream)
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook(fileStream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed()?.RowsUsed()?.Skip(1) ?? Enumerable.Empty<ClosedXML.Excel.IXLRangeRow>();

            int count = 0;
            foreach (var row in rows)
            {
                var productName = row.Cell(2).GetValue<string>() ?? "Unnamed Product";
                var product = new Product
                {
                    Name = productName,
                    Price = row.Cell(3).GetValue<decimal>(),
                    Stock = row.Cell(4).GetValue<int>(),
                    Description = row.Cell(6).GetValue<string>() ?? "",
                    ImageUrl = row.Cell(7).GetValue<string>() ?? ""
                };

                var categoryName = row.Cell(5).GetValue<string>() ?? "General";
                var category = await _productService.GetOrCreateCategoryByNameAsync(categoryName);
                product.CategoryId = category.CategoryId;

                await _productService.CreateProductAsync(product);
                count++;
            }
            return count;
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
