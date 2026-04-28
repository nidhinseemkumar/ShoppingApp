using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Models;
using ShoppingApp.Repositories;

namespace ShoppingApp.Services
{
    public interface IPaymentService
    {
        Task<Payment> ProcessPaymentAsync(int orderId, decimal amount, string method);
        Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        public async Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId)
        {
            return await _unitOfWork.Repository<Payment>()
                .GetQueryable()
                .Where(p => p.OrderId == orderId)
                .ToListAsync();
        }
    }
}
