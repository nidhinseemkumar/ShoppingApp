using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ShoppingApp.Services;
using ShoppingApp.DTOs;
using ShoppingApp.Wrappers;
using ShoppingApp.Models;

namespace ShoppingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminApiController(
        IProductService productService,
        IUserService userService,
        IOrderService orderService,
        IFileService fileService,
        ILogger<AdminApiController> logger) : ControllerBase
    {
        private readonly IProductService _productService = productService;
        private readonly IUserService _userService = userService;
        private readonly IOrderService _orderService = orderService;
        private readonly IFileService _fileService = fileService;
        private readonly ILogger<AdminApiController> _logger = logger;


        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var productsResponse = await _productService.GetAllProductsAsync();
                var users = await _userService.GetAllUsersAsync();
                var totalSales = await _orderService.GetTotalSalesAsync();
                var totalOrders = await _orderService.GetTotalOrdersCountAsync();

                var stats = new
                {
                    ProductCount = productsResponse?.Data?.Count() ?? 0,
                    UserCount = users?.Count() ?? 0,
                    TotalRevenue = totalSales,
                    OrderCount = totalOrders,
                    RecentOrders = await _orderService.GetRecentOrdersAsync(5) ?? [],
                    SalesTrend = await _orderService.GetSalesTrendAsync(7) ?? [],
                    TopProducts = await _orderService.GetTopSellingProductsAsync(5) ?? [],
                    CategorySales = await _orderService.GetSalesByCategoryAsync() ?? []
                };

                return Ok(new ApiResponse<object>(stats, "Stats retrieved successfully"));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin stats");
                return StatusCode(500, new ApiResponse<object>("An error occurred while fetching dashboard stats"));
            }
        }

        // --- PRODUCTS ---

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10, string? search = null)
        {
            var response = await _productService.GetAllProductsAsync(search);
            if (!response.Success || response.Data == null) return BadRequest(response);

            var data = response.Data!;
            var items = data.Skip((page - 1) * pageSize).Take(pageSize);
            var result = new { items, totalCount = data.Count() };

            return Ok(new ApiResponse<object>(result, "Products retrieved successfully"));
        }

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromForm] Product product, IFormFile? image)
        {
            if (image != null)
            {
                if (image.Length > 5 * 1024 * 1024) return BadRequest(new ApiResponse<string>("File too large (max 5MB)"));
                product.ImageUrl = await _fileService.SaveImageAsync(image);
            }

            var response = await _productService.CreateProductAsync(product);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] Product product, IFormFile? image)
        {
            var existing = await _productService.GetProductEntityByIdAsync(id);
            if (existing == null) return NotFound(new ApiResponse<string>("Product not found"));

            if (image != null)
            {
                if (image.Length > 5 * 1024 * 1024) return BadRequest(new ApiResponse<string>("File too large (max 5MB)"));
                product.ImageUrl = await _fileService.SaveImageAsync(image);
            }
            else
            {
                product.ImageUrl = existing.ImageUrl;
            }

            product.ProductId = id;
            var response = await _productService.UpdateProductAsync(product);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProductAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // --- CATEGORIES ---

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _productService.GetCategoriesAsync();
            return Ok(new ApiResponse<IEnumerable<Category>>(categories, "Categories retrieved successfully"));
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (string.IsNullOrEmpty(category.CategoryName)) return BadRequest(new ApiResponse<string>("Category name is required"));
            var result = await _productService.GetOrCreateCategoryByNameAsync(category.CategoryName);
            return Ok(new ApiResponse<Category>(result, "Category created successfully"));
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _productService.DeleteCategoryAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // --- USERS ---

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new ApiResponse<IEnumerable<UserDto>>(users, "Users retrieved successfully"));
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser(UserRegisterDto userDto)
        {
            var response = await _userService.RegisterUserAsync(userDto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDto userDto)
        {
            userDto.UserId = id;
            var response = await _userService.UpdateUserAsync(userDto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = await _userService.DeleteUserAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // --- ORDERS ---

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(new ApiResponse<IEnumerable<Order>>(orders, "Orders retrieved successfully"));
        }

        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return order != null ? Ok(new ApiResponse<Order>(order)) : NotFound(new ApiResponse<string>("Order not found"));
        }

        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status);
            return result ? Ok(new ApiResponse<bool>(true, "Order status updated")) : BadRequest(new ApiResponse<string>("Failed to update status"));
        }

        // --- IMPORT / EXPORT ---

        [HttpPost("import/products")]
        public async Task<IActionResult> ImportProducts(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new ApiResponse<string>("No file uploaded"));
            if (file.Length > 5 * 1024 * 1024) return BadRequest(new ApiResponse<string>("File too large (max 5MB)"));

            using var stream = file.OpenReadStream();
            int count = 0;
            if (file.FileName.EndsWith(".csv")) count = await _fileService.ImportProductsFromCsv(stream);
            else if (file.FileName.EndsWith(".xlsx")) count = await _fileService.ImportProductsFromExcel(stream);
            else return BadRequest(new ApiResponse<string>("Invalid file format (.csv or .xlsx only)"));

            return Ok(new ApiResponse<int>(count, $"{count} products imported successfully"));
        }

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
