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
            var header = await reader.ReadLineAsync(); // Skip header

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
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
    }
}
