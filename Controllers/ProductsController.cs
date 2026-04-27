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
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public ProductsController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? category)
        {
            var response = await _productService.GetAllProductsAsync(searchTerm, category);
            
            var userCart = new List<Cart>();
            var userIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                userCart = cartItems.ToList();
            }

            ViewData["CurrentSearch"] = searchTerm;
            ViewData["CurrentCategory"] = category;
            ViewData["UserCart"] = userCart;
            
            return View(response.Data);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var response = await _productService.GetProductByIdAsync(id.Value);
            if (!response.Success) return NotFound();

            var userCart = new List<Cart>();
            var userIdStr = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var cartItems = await _cartService.GetCartItemsAsync(userId);
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

            if (ModelState.IsValid)
            {
                var category = await _productService.GetOrCreateCategoryByNameAsync(categoryName.Trim());
                product.CategoryId = category.CategoryId;
                
                await _productService.CreateProductAsync(product);
                return RedirectToAction("Dashboard", "Admin");
            }
            
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

        // Add Delete actions similarly...

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStock(int productId, int stock)
        {
            var success = await _productService.UpdateStockAsync(productId, stock);
            return Json(new { success });
        }

        [HttpGet]
        public async Task<IActionResult> GetSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new List<string>());
            
            var productsResponse = await _productService.GetAllProductsAsync(searchTerm: term);
            var suggestions = productsResponse.Data?
                .Select(p => p.Name)
                .Where(name => name != null)
                .Take(5)
                .ToList() ?? new List<string?>();
                
            return Json(suggestions);
        }
    }
}
