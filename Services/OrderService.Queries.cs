using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Services
{
    public partial class OrderService
    {
        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId, string? searchTerm = null, DateTime? startDate = null, DateTime? endDate = null, decimal? minPrice = null, decimal? maxPrice = null, string? sortBy = null, bool isDescending = true)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .Where(o => o.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(o => 
                    o.OrderId.ToString().Contains(searchTerm) ||
                    o.OrderItems.Any(oi => oi.Product != null && oi.Product.Name != null && oi.Product.Name.ToLower().Contains(searchTerm))
                );
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var nextDay = endDate.Value.Date.AddDays(1);
                query = query.Where(o => o.OrderDate < nextDay);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(o => o.TotalAmount >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(o => o.TotalAmount <= maxPrice.Value);
            }

            // Sorting
            query = sortBy switch
            {
                "price" => isDescending ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount),
                "date" => isDescending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate),
                _ => query.OrderByDescending(o => o.OrderDate) // Default sort
            };

            return await query.ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId, int userId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p!.Category)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p!.Category)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(string? searchTerm = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(o => 
                    o.OrderId.ToString().Contains(searchTerm) ||
                    (o.User != null && (o.User.FirstName + " " + o.User.LastName).ToLower().Contains(searchTerm)) ||
                    o.OrderItems.Any(oi => oi.Product != null && oi.Product.Name != null && oi.Product.Name.ToLower().Contains(searchTerm))
                );
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var nextDay = endDate.Value.Date.AddDays(1);
                query = query.Where(o => o.OrderDate < nextDay);
            }

            return await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalSalesAsync()
        {
            return await _context.Orders.SumAsync(o => o.TotalAmount ?? 0);
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count)
        {
            return await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetSalesTrendAsync(int days)
        {
            var startDate = DateTime.Now.Date.AddDays(-days);
            return await _context.Orders
                .Where(o => o.OrderDate >= startDate)
                .GroupBy(o => o.OrderDate!.Value.Date)
                .Select(g => new { Date = g.Key.ToString("MMM dd"), Sales = g.Sum(o => o.TotalAmount ?? 0) })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetTopSellingProductsAsync(int count)
        {
            return await _context.OrderItems
                .Where(oi => oi.Product != null)
                .GroupBy(oi => oi.Product!.Name ?? "Unknown")
                .Select(g => new { Name = g.Key, Quantity = g.Sum(oi => oi.Quantity ?? 0) })
                .OrderByDescending(g => g.Quantity)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetSalesByCategoryAsync()
        {
            return await _context.OrderItems
                .Where(oi => oi.Product != null && oi.Product.Category != null)
                .GroupBy(oi => oi.Product!.Category!.CategoryName ?? "Other")
                .Select(g => new { Category = g.Key, Sales = g.Sum(oi => oi.Price * (oi.Quantity ?? 0)) })
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetOrderHistorySuggestionsAsync(int userId, string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return [];

            term = term.Trim().ToLower();

            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Where(oi => oi.Order != null && oi.Order.UserId == userId && 
                             oi.Product != null && oi.Product.Name != null && oi.Product.Name.ToLower().Contains(term))
                .Select(oi => oi.Product!.Name!)
                .Distinct()
                .Take(10)
                .ToListAsync();
        }
    }
}
