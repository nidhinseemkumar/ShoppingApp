using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingApp.Models;
using ShoppingApp.DTOs;
using ShoppingApp.Wrappers;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public partial class PaymentsController(
        IPaymentService paymentService, 
        ICartService cartService, 
        IOrderService orderService, 
        IProductService productService) : Controller
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly ICartService _cartService = cartService;
        private readonly IOrderService _orderService = orderService;
        private readonly IProductService _productService = productService;
    }
}
