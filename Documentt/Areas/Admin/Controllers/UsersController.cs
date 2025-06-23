// File: Documentt/Areas/Admin/Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // Quan trọng: import IdentityUser
using Documentt.Models;

namespace Documentt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Đảm bảo chỉ Admin mới có quyền truy cập
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager; // Sử dụng IdentityUser
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/Users
        // Hiển thị danh sách tất cả người dùng
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    Roles = roles.ToList()
                });
            }
            return View(userViewModels);
        }

        // GET: Admin/Users/Details/5
        // Hiển thị thông tin chi tiết của một người dùng
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Roles = roles.ToList()
            };

            return View(userViewModel);
        }

        // GET: Admin/Users/Delete/5
        // Hiển thị trang xác nhận xóa người dùng
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Ngăn chặn xóa người dùng hiện tại đang đăng nhập
            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["ErrorMessage"] = "Bạn không thể xóa tài khoản của chính mình!";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Roles = roles.ToList()
            };

            return View(userViewModel);
        }

        // POST: Admin/Users/Delete/5
        // Xử lý yêu cầu xóa người dùng sau khi xác nhận
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng để xóa.";
                return RedirectToAction(nameof(Index));
            }

            // Đảm bảo không xóa người dùng Admin duy nhất
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole != null)
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync(adminRole.Name!);
                if (adminUsers.Count == 1 && adminUsers.Any(u => u.Id == user.Id))
                {
                    TempData["ErrorMessage"] = "Không thể xóa người dùng Admin duy nhất trong hệ thống.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Ngăn chặn xóa người dùng hiện tại đang đăng nhập (kiểm tra lại lần nữa phòng trường hợp race condition)
            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["ErrorMessage"] = "Bạn không thể xóa tài khoản của chính mình!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Người dùng đã được xóa thành công!";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa người dùng.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}