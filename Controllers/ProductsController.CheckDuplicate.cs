using Microsoft.AspNetCore.Mvc;
using ShoppingApp.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class ProductsController : Controller
    {
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
