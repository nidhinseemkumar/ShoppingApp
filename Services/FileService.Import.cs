using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ShoppingApp.Models;

namespace ShoppingApp.Services
{
    public partial class FileService
    {
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
    }
}
