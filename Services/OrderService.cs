using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApp.Services
{
    public partial class OrderService(ShoppingDbContext context) : IOrderService
    {
        private readonly ShoppingDbContext _context = context;
    }
}
