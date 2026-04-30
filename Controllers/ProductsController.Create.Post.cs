using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class ProductsController : Controller
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, string categoryName, List<string> imageUrls)
        {
            if (imageUrls == null || !imageUrls.Any(u => !string.IsNullOrWhiteSpace(u)))
            {
                ModelState.AddModelError("imageUrls", "At least one product image link is required.");
            }
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

                // Handle images from URLs
                if (imageUrls != null && imageUrls.Any())
                {
                    foreach (var url in imageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                    {
                        product.Images.Add(new ProductImage 
                        { 
                            ImageUrl = url.Trim(),
                            IsPrimary = product.Images.Count == 0 // Set first image as primary
                        });
                    }
                    // Set legacy ImageUrl for backward compatibility
                    product.ImageUrl = product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl ?? product.Images.FirstOrDefault()?.ImageUrl;
                }
                
                var result = await _productService.CreateProductAsync(product);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = $"Product '{product.Name}' added successfully!";
                    return RedirectToAction("Dashboard", "Admin");
                }
                ModelState.AddModelError("", result.Message);
            }
            
            var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            TempData["ErrorMessage"] = string.IsNullOrEmpty(errors) ? "Failed to add product. Please check the details." : $"Error: {errors}";
            ViewBag.AllCategories = (await _productService.GetCategoriesAsync()).ToList();
            return View(product);
        }
    }
}
