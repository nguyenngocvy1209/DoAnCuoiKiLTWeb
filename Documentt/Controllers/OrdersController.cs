// File: Documentt/Controllers/OrdersController.cs
using Microsoft.AspNetCore.Mvc;
using Documentt.Models.Interface;
using Documentt.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Documentt.Data;
using Microsoft.AspNetCore.Http; // Để sử dụng HttpContext.Session

namespace Documentt.Controllers
{
     [Authorize] 
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly TailieuContextDB _context; 

        public OrdersController(IOrderRepository orderRepository,
                                IShoppingCartRepository shoppingCartRepository,
                                TailieuContextDB context)
        {
            _orderRepository = orderRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _context = context;
        }

       
        [HttpGet] // Đảm bảo đây là HTTP GET
        public IActionResult Checkout()
        {
            var shoppingCartItems = _shoppingCartRepository.GetAllShoppingCartItems();

            
            if (shoppingCartItems == null || !shoppingCartItems.Any())
            {
                
                return RedirectToAction("Index", "ShoppingCart");
            }

           
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" }); 
            }

           
            var order = new Order
            {
                Email = userEmail,
               
            };

            try
            {
               
                _orderRepository.PlaceOrder(order);


                _shoppingCartRepository.ClearCart();
                HttpContext.Session.SetInt32("CartCount", 0); 

                
                return View("Checkout", order); 
            }
            catch (Exception ex)
            {
               
                return RedirectToAction("Index", "ShoppingCart"); 
            }
        }

       
       
    }
}