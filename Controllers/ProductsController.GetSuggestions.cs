using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    public partial class ProductsController : Controller
    {
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
    }
}
