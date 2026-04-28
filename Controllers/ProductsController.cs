using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingApp.Services;
using ShoppingApp.DTOs;
using ShoppingApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ShoppingApp.Controllers
{
    public class ProductsController(IProductService productService, ICartService cartService) : Controller
    {
        private readonly IProductService _productService = productService;
        private readonly ICartService _cartService = cartService;


        public async Task<IActionResult> Index(string? searchTerm, string? category)
        {
            var response = await _productService.GetAllProductsAsync(searchTerm, category);
            
            List<Cart> userCart = [];
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                userCart = cartItems.ToList();
            }

            var products = response.Data;
            if (User.IsInRole("Customer"))
            {
                products = products?.Where(p => p.Stock > 0).ToList();
            }

            ViewData["CurrentSearch"] = searchTerm;
            ViewData["CurrentCategory"] = category;
            ViewData["UserCart"] = userCart;
            
            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? currentUserId = int.TryParse(userIdStr, out int userId) ? userId : null;

            var response = await _productService.GetProductByIdAsync(id.Value, currentUserId);
            if (!response.Success || response.Data == null) return NotFound();

            List<Cart> userCart = [];
            if (currentUserId.HasValue)
            {
                var cartItems = await _cartService.GetCartItemsAsync(currentUserId.Value);
                userCart = cartItems.ToList();
            }
            ViewData["UserCart"] = userCart;

            return View(response.Data);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.AllCategories = await _productService.GetCategoriesAsync();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                ModelState.AddModelError("categoryName", "Category is required");
            }

            // Server-side check for duplicate Name globally
            var existingProducts = await _productService.GetAllProductsAsync();
            if (existingProducts.Data != null && existingProducts.Data.Any(p => 
                p.Name?.Trim().Equals(product.Name?.Trim(), StringComparison.OrdinalIgnoreCase) == true))
            {
                ModelState.AddModelError("", "Rejected: A product with this name already exists in the database.");
                TempData["ErrorMessage"] = "Duplicate Detected: Product name already exists globally.";
            }

            if (ModelState.IsValid)
            {
                var category = await _productService.GetOrCreateCategoryByNameAsync(categoryName.Trim());
                product.CategoryId = category.CategoryId;
                
                await _productService.CreateProductAsync(product);
                TempData["SuccessMessage"] = $"Product '{product.Name}' added successfully!";
                return RedirectToAction("Dashboard", "Admin");
            }
            
            TempData["ErrorMessage"] = "Failed to add product. Please check the details.";
            var categories = await _productService.GetCategoriesAsync();
            ViewBag.AllCategories = categories;
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _productService.GetProductEntityByIdAsync(id.Value);
            if (product == null) return NotFound();
            
            ViewBag.AllCategories = await _productService.GetCategoriesAsync();
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, string categoryName)
        {
            if (id != product.ProductId) return NotFound();

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                ModelState.AddModelError("categoryName", "Category is required");
            }

            if (ModelState.IsValid)
            {
                var category = await _productService.GetOrCreateCategoryByNameAsync(categoryName.Trim());
                product.CategoryId = category.CategoryId;
                
                await _productService.UpdateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.AllCategories = await _productService.GetCategoriesAsync();
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _productService.GetProductEntityByIdAsync(id.Value);
            if (product == null) return NotFound();
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction("Dashboard", "Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStock(int productId, int stock)
        {
            var success = await _productService.UpdateStockAsync(productId, stock);
            return Json(new { success });
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdatePrice(int productId, decimal price)
        {
            var success = await _productService.UpdatePriceAsync(productId, price);
            return Json(new { success });
        }

        [HttpGet]
        public async Task<IActionResult> GetSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json((List<string>)[]);
            
            var productsResponse = await _productService.GetAllProductsAsync(searchTerm: term);
            var products = productsResponse.Data;
            if (User.IsInRole("Customer"))
            {
                products = products?.Where(p => p.Stock > 0);
            }

            var suggestions = products?
                .Select(p => p.Name)
                .Where(name => name != null)
                .Take(5)
                .Select(n => n!)
                .ToList() ?? new List<string>();
                
            return Json(suggestions);
        }

        [HttpGet]
        public async Task<IActionResult> CheckDuplicate(string name, string? category)
        {
            var response = await _productService.GetAllProductsAsync();
            var allProducts = response.Data ?? new List<ProductDto>();

            bool isCategoryDuplicate = false;
            bool isGlobalDuplicate = false;

            if (!string.IsNullOrWhiteSpace(name))
            {
                var lowerName = name.ToLower().Trim();
                
                // Global check
                isGlobalDuplicate = allProducts.Any(p => p.Name?.ToLower().Trim() == lowerName);

                // Category specific check
                if (!string.IsNullOrWhiteSpace(category))
                {
                    var lowerCat = category.ToLower().Trim();
                    isCategoryDuplicate = allProducts.Any(p => 
                        p.Name?.ToLower().Trim() == lowerName && 
                        p.CategoryName?.ToLower().Trim() == lowerCat);
                }
            }

            return Json(new { isCategoryDuplicate, isGlobalDuplicate });
        }
    }
}
