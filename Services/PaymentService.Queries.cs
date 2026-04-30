using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;

namespace ShoppingApp.Services
{
    public partial class PaymentService
    {
        public async Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId)
        {
            return await _unitOfWork.Repository<Payment>()
                .GetQueryable()
                .Where(p => p.OrderId == orderId)
                .ToListAsync();
        }
    }
}
