using System;
using System.Threading.Tasks;
using ShoppingApp.Models;

namespace ShoppingApp.Services
{
    public partial class PaymentService
    {
        public async Task<Payment> ProcessPaymentAsync(int orderId, decimal amount, string method)
        {
            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                PaymentMethod = method,
                PaymentDate = DateTime.Now,
                PaymentStatus = method == "COD" ? "Pending Collection" : "Completed"
            };

            await _unitOfWork.Repository<Payment>().AddAsync(payment);
            
            // Update Order Status
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (order != null)
            {
                order.Status = "Pending";
                _unitOfWork.Repository<Order>().Update(order);
            }

            await _unitOfWork.CompleteAsync();
            return payment;
        }
    }
}
