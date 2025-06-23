// File: Documentt/Areas/Admin/Controllers/AdminController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Để sử dụng [Authorize]

namespace Documentt.Areas.Admin.Controllers
{
    [Area("Admin")] // Đánh dấu đây là một Controller trong Area "Admin"
    [Authorize(Roles = "Admin")] // Chỉ người dùng có vai trò "Admin" mới có thể truy cập
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Trả về Admin Dashboard View
        }
    }
}