using Documentt.Models.Interface;
using Documentt.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Cần thiết cho UserManager
using Documentt.Models;
using Documentt.Data;
namespace Documentt.Controllers
{
    [Authorize] // Đảm bảo người dùng phải đăng nhập để truy cập giỏ hàng và thanh toán
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartRepository shoppingCartRepository;
        private readonly IHomeRepository productRepository;
        private readonly UserManager<ApplicationUser> _userManager; // Inject UserManager
        private readonly TailieuContextDB _dbContext; // Inject DbContext để lưu Order

        public ShoppingCartController(
            IShoppingCartRepository shoppingCartRepository,
            IHomeRepository productRepository,
            UserManager<ApplicationUser> userManager, // Thêm UserManager vào constructor
            TailieuContextDB dbContext) // Thêm DbContext vào constructor
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
            _userManager = userManager; // Gán UserManager
            _dbContext = dbContext; // Gán DbContext
        }

        public async Task<IActionResult> Index()
        {
            var items = shoppingCartRepository.GetAllShoppingCartItems();
            // shoppingCartRepository.shopingcartItems = items; // Dòng này có thể không cần thiết nữa
            ViewBag.TotalCart = shoppingCartRepository.GetShoppingCartTotal();

            // Lấy số dư hiện tại của người dùng để hiển thị trên trang giỏ hàng
            var user = await _userManager.GetUserAsync(User) as ApplicationUser;
            ViewBag.UserBalance = user?.Balance ?? 0.00m; // Hiển thị 0 nếu không tìm thấy user

            return View(items);
        }

        // Thay đổi sang IActionResult để có thể Redirect(Referer)
        public IActionResult AddToShoppingCart(int pId)
        {
            var product = productRepository.GetAllProducts().FirstOrDefault(p => p.ProductId == pId);
            if (product != null)
            {
                shoppingCartRepository.AddToCart(product);
                int cartCount = shoppingCartRepository.GetAllShoppingCartItems().Count();
                HttpContext.Session.SetInt32("CartCount", cartCount);
            }

            // Chuyển hướng người dùng về trang trước đó
            if (Request.Headers["Referer"].Count > 0)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return RedirectToAction("Index", "Home"); // Fallback
        }
        public IActionResult BuyToCart(int pId)
        {
            var product = productRepository.GetAllProducts().FirstOrDefault(p => p.ProductId == pId);
            if (product != null)
            {
                shoppingCartRepository.AddToCart(product);
                int cartCount = shoppingCartRepository.GetAllShoppingCartItems().Count();
                HttpContext.Session.SetInt32("CartCount", cartCount);
            }

            // Chuyển hướng người dùng về trang trước đó
            
            return RedirectToAction("Index"); // Fallback
        }
        public RedirectToActionResult RemoveFromShoppingCart(int pId)
        {
            var product = productRepository.GetAllProducts().FirstOrDefault(p => p.ProductId == pId);
            if (product != null)
            {
                shoppingCartRepository.RemoveFromCart(product);
                int cartCount = shoppingCartRepository.GetAllShoppingCartItems().Count();
                HttpContext.Session.SetInt32("CartCount", cartCount);
            }
            return RedirectToAction("Index");
        }

        public RedirectToActionResult ClearShoppingCart()
        {
            shoppingCartRepository.ClearCart();
            HttpContext.Session.SetInt32("CartCount", 0);
            return RedirectToAction("Index");
        }

        // NEW: Action để xử lý thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User) as ApplicationUser;
            if (user == null)
            {
                // Nếu người dùng không đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var shoppingCartItems = shoppingCartRepository.GetAllShoppingCartItems();
            if (!shoppingCartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index");
            }

            decimal totalCart = shoppingCartRepository.GetShoppingCartTotal();

            // Kiểm tra số dư
            if (user.Balance < totalCart)
            {
                TempData["ErrorMessage"] = $"Số dư của bạn không đủ để thanh toán. Bạn cần thêm {totalCart - user.Balance:C} nữa.";
                return RedirectToAction("Index");
            }

            // --- Thực hiện thanh toán ---

            // 1. Trừ tiền từ số dư người dùng
            user.Balance -= totalCart;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật số dư. Vui lòng thử lại.";
                // Cân nhắc roll back giỏ hàng nếu lỗi ở đây (tùy thuộc vào mức độ phức tạp của hệ thống)
                return RedirectToAction("Index");
            }

            // 2. Tạo đơn hàng và chi tiết đơn hàng (giả định Order và OrderDetail models đã tồn tại)
            var order = new Order
            {
                OrderPlaced = DateTime.Now,
                OrderTotal = totalCart,
                Email = user.Email, // Lưu email người dùng
                // Thêm các thông tin khác của Order nếu có (ví dụ: Name, Address, Phone)
                // Name = user.FullName, // Nếu bạn có FullName trong ApplicationUser
                // Address = "...",
                // PhoneNumber = "...",
            };
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync(); // Lưu Order trước để có OrderId

            foreach (var item in shoppingCartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.Product.ProductId, // Đảm bảo ProductId là int
                    Quantity = item.Qty, // Sẽ luôn là 1 trong trường hợp này
                    Price = item.Product.Price // Giá tại thời điểm mua
                };
                _dbContext.OrderDetails.Add(orderDetail);
            }
            await _dbContext.SaveChangesAsync(); // Lưu OrderDetails

            // 3. Xóa giỏ hàng
            shoppingCartRepository.ClearCart();
            HttpContext.Session.SetInt32("CartCount", 0); // Cập nhật số lượng giỏ hàng trong session

            TempData["SuccessMessage"] = "Thanh toán thành công! Đơn hàng của bạn đã được xử lý.";
            return RedirectToAction("Index", "Profile"); // Chuyển hướng đến trang hồ sơ để xem lịch sử mua hàng
        }
    }
}