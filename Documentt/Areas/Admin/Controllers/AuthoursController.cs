// File: Documentt/Areas/Admin/Controllers/AuthoursController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Documentt.Data; // Import TailieuContextDB
using Documentt.Models; // Import Authour model

namespace Documentt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền truy cập
    public class AuthoursController : Controller
    {
        private readonly TailieuContextDB _context;

        public AuthoursController(TailieuContextDB context)
        {
            _context = context;
        }

        // GET: Admin/Authours
        public async Task<IActionResult> Index()
        {
            // Bao gồm Products nếu bạn muốn hiển thị thông tin sản phẩm liên quan
            // var authours = await _context.Authours.Include(a => a.Products).ToListAsync();
            // Nếu không cần Products ở trang Index để tăng hiệu suất, bỏ .Include()
            var authours = await _context.Authours.ToListAsync();
            return View(authours);
        }

        // GET: Admin/Authours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authour = await _context.Authours
                .Include(a => a.Products) // Bao gồm Products để hiển thị các sản phẩm của tác giả
                .FirstOrDefaultAsync(m => m.AuthourId == id);
            if (authour == null)
            {
                return NotFound();
            }

            return View(authour);
        }

        // GET: Admin/Authours/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Authours/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Điều chỉnh [Bind] để phù hợp với các trường mới
        public async Task<IActionResult> Create([Bind("Img,AuthourName,Detail,noibat")] Authour authour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(authour);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tác giả đã được thêm thành công!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm tác giả. Vui lòng kiểm tra lại thông tin.";
            return View(authour);
        }

        // GET: Admin/Authours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authour = await _context.Authours.FindAsync(id);
            if (authour == null)
            {
                return NotFound();
            }
            return View(authour);
        }

        // POST: Admin/Authours/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Điều chỉnh [Bind] để phù hợp với các trường mới
        public async Task<IActionResult> Edit(int id, [Bind("AuthourId,Img,AuthourName,Detail,noibat")] Authour authour)
        {
            if (id != authour.AuthourId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(authour);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Tác giả đã được cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthourExists(authour.AuthourId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật tác giả. Vui lòng kiểm tra lại thông tin.";
            return View(authour);
        }

        // GET: Admin/Authours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authour = await _context.Authours
                .FirstOrDefaultAsync(m => m.AuthourId == id);
            if (authour == null)
            {
                return NotFound();
            }

            return View(authour);
        }

        // POST: Admin/Authours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var authour = await _context.Authours.FindAsync(id);
            if (authour != null)
            {
                _context.Authours.Remove(authour);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tác giả đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy tác giả để xóa.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AuthourExists(int id)
        {
            return _context.Authours.Any(e => e.AuthourId == id);
        }
    }
}