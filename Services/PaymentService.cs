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

    public partial class PaymentService(IUnitOfWork unitOfWork) : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
    }
}
