using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Services;
using ShoppingApp.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoppingApp.Controllers
{
    [Authorize]
    public partial class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
    }
}
