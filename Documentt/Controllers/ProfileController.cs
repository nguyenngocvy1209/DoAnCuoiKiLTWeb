using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Documentt.Data;
using Documentt.Models; // Đảm bảo có ApplicationUser
using System.ComponentModel.DataAnnotations;

namespace Documentt.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager; // Sửa IdentityUser thành ApplicationUser
        private readonly SignInManager<ApplicationUser> _signInManager; // Sửa IdentityUser thành ApplicationUser
        private readonly TailieuContextDB _dbContext;

        public ProfileController(
            UserManager<ApplicationUser> userManager, // Sửa IdentityUser thành ApplicationUser
            SignInManager<ApplicationUser> signInManager, // Sửa IdentityUser thành ApplicationUser
            TailieuContextDB dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        // GET: /Profile/Index - Hiển thị hồ sơ và lịch sử mua hàng
        public async Task<IActionResult> Index()
        {
            // Lấy ApplicationUser để truy cập Balance
            var user = await _userManager.GetUserAsync(User) as ApplicationUser;

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Lấy lịch sử mua hàng của người dùng
            var purchaseHistory = await _dbContext.OrderDetails
                .Where(od => od.Order.Email == user.Email) // Lọc theo email của người dùng
                .Include(od => od.Product) // Eager load thông tin Product
                .OrderByDescending(od => od.Order.OrderPlaced) // Sắp xếp theo ngày đặt hàng gần nhất
                .Select(od => new PurchaseHistoryItemViewModel
                {
                    ProductId = od.Product.ProductId,
                    ProductName = od.Product.Name,
                    ProductLink = od.Product.link // Chú ý: Đảm bảo thuộc tính này là `Link` không phải `link` (chữ thường) trong Model
                })
                .Distinct() // Đảm bảo chỉ hiển thị mỗi sản phẩm một lần nếu được mua nhiều lần
                .ToListAsync();

            var profileViewModel = new ProfileViewModel
            {
                Email = user.Email,
                Balance = user.Balance, // Lấy số dư từ ApplicationUser
                PurchaseHistory = purchaseHistory
            };

            return View(profileViewModel);
        }

        // Action cho nút "Chỉnh sửa hồ sơ" (Bạn có thể thêm logic sau)
        public IActionResult Edit()
        {
            return View();
        }

        // GET: /Profile/TopUp - Hiển thị form nạp tiền
        public IActionResult TopUp()
        {
            // Truyền ViewModel rỗng cho form
            return View(new TopUpViewModel());
        }

        // POST: /Profile/TopUp - Xử lý việc nạp tiền
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TopUp(TopUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu dữ liệu không hợp lệ, trả về form với lỗi
                return View(model);
            }

            if (model.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Số tiền nạp phải lớn hơn 0.");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User) as ApplicationUser;
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Thực hiện nạp tiền (mô phỏng)
            user.Balance += model.Amount; // Cộng số tiền nạp vào số dư
            var result = await _userManager.UpdateAsync(user); // Cập nhật người dùng vào DB

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Bạn đã nạp thành công {model.Amount:N0} vào tài khoản!"; // :C để định dạng tiền tệ
                return RedirectToAction(nameof(Index)); // Chuyển hướng về trang hồ sơ
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi nạp tiền.";
                return View(model);
            }
        }
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        // POST: /Profile/ChangePassword - Xử lý đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu dữ liệu không hợp lệ, trả về form với lỗi
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User) as ApplicationUser;
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Đổi mật khẩu sử dụng UserManager
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user); // Cập nhật cookie xác thực sau khi đổi mật khẩu
                TempData["SuccessMessage"] = "Mật khẩu của bạn đã được thay đổi thành công.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "Không thể thay đổi mật khẩu. Vui lòng kiểm tra mật khẩu cũ.";
                return View(model);
            }
        }
        // Action cho nút "Đăng xuất"
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }

    // VIEW MODELS CHO PROFILE CONTROLLER
    public class ProfileViewModel
    {
        public string Email { get; set; } = string.Empty;
        public decimal Balance { get; set; } // Thêm Balance
        public List<PurchaseHistoryItemViewModel> PurchaseHistory { get; set; } = new List<PurchaseHistoryItemViewModel>();
        public ChangePasswordViewModel ChangePasswordForm { get; set; } = new ChangePasswordViewModel();

    }
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mật khẩu cũ là bắt buộc.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu cũ")]
        public string OldPassword { get; set; } // This is correct

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
        [StringLength(100, ErrorMessage = "{0} phải dài ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; } // This is correct

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không khớp.")]
        public string ConfirmNewPassword { get; set; } // This is correct
    }
    public class PurchaseHistoryItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductLink { get; set; }
    }

    // VIEW MODEL MỚI CHO TRANG NẠP TIỀN
    public class TopUpViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập số tiền muốn nạp.")]
        [Range(1000, 10000000, ErrorMessage = "Số tiền nạp phải từ 1,000 đến 10,000,000.")] // Ví dụ, giới hạn min/max
        [Display(Name = "Số tiền nạp")]
        public decimal Amount { get; set; }
    }
}