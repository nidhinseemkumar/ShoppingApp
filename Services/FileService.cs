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

    public partial class FileService(IProductService productService) : IFileService
    {
        private readonly IProductService _productService = productService;
    }
}
