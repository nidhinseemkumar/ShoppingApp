using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Models;
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
        public async Task<IActionResult> Edit(int id, Product product, string categoryName, List<string> imageUrls)
        {
            if (id != product.ProductId) return NotFound();

            if (imageUrls == null || !imageUrls.Any(u => !string.IsNullOrWhiteSpace(u)))
            {
                ModelState.AddModelError("imageUrls", "At least one product image link is required.");
            }

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                ModelState.AddModelError("categoryName", "Category is required");
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
                            ProductId = id
                        });
                    }
                    product.ImageUrl = product.Images.FirstOrDefault()?.ImageUrl;
                }
                
                var result = await _productService.UpdateProductAsync(product);
                if (result.Success) return RedirectToAction(nameof(Index));
                
                ModelState.AddModelError("", result.Message);
            }
            
            ViewBag.AllCategories = await _productService.GetCategoriesAsync();
            return View(product);
        }
    }
}
