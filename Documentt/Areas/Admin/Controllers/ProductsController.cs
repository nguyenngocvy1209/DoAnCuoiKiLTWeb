// File: Documentt/Areas/Admin/Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

using Documentt.Data;
using Documentt.Models;

namespace Documentt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly TailieuContextDB _dbContext;

        public ProductsController(TailieuContextDB dbContext)
        {
            _dbContext = dbContext;
        }

        private async Task PopulateDropdowns(int? selectedCategoryId = null, int? selectedAuthourId = null)
        {
            ViewBag.CategoryId = new SelectList(await _dbContext.Categories.OrderBy(c => c.CategoryName).ToListAsync(), "CategoryId", "CategoryName", selectedCategoryId);
            ViewBag.AuthourId = new SelectList(await _dbContext.Authours.OrderBy(a => a.AuthourName).ToListAsync(), "AuthourId", "AuthourName", selectedAuthourId);
        }


        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var products = await _dbContext.Products
                                           .Include(p => p.Category)
                                           .Include(p => p.Authour)
                                           .ToListAsync(); // Không còn OrderByDescending(p => p.DateAdded)
            return View(products);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _dbContext.Products
                                           .Include(p => p.Category)
                                           .Include(p => p.Authour)
                                           .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            // Cập nhật tất cả các thuộc tính có trong form vào Bind attribute
            [Bind("Name,Description,Price,New,Trending,CategoryId,AuthourId,link,img,img1,img2,img3,img4,img5")] Product product)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Add(product);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tài liệu đã được thêm thành công!";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(product.CategoryId, product.AuthourId);
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm tài liệu. Vui lòng kiểm tra lại thông tin.";
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await PopulateDropdowns(product.CategoryId, product.AuthourId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            // Cập nhật tất cả các thuộc tính có trong form vào Bind attribute
            [Bind("ProductId,Name,Description,Price,New,Trending,CategoryId,AuthourId,link,img,img1,img2,img3,img4,img5")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các thuộc tính từ đối tượng 'product' (nhận từ form) vào 'existingProduct'
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.New = product.New;
                    existingProduct.Trending = product.Trending;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.AuthourId = product.AuthourId;
                    existingProduct.link = product.link;
                    existingProduct.img = product.img;
                    existingProduct.img1 = product.img1;
                    existingProduct.img2 = product.img2;
                    existingProduct.img3 = product.img3;
                    existingProduct.img4 = product.img4;
                    existingProduct.img5 = product.img5;

                    _dbContext.Entry(existingProduct).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Tài liệu đã được cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExists(product.ProductId))
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

            await PopulateDropdowns(product.CategoryId, product.AuthourId);
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật tài liệu. Vui lòng kiểm tra lại thông tin.";
            return View(product);
        }

        // GET: Admin/Products/Delete/5 (Giữ nguyên)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) { return NotFound(); }
            var product = await _dbContext.Products
                                          .Include(p => p.Category)
                                          .Include(p => p.Authour)
                                          .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null) { return NotFound(); }
            return View(product);
        }

        // POST: Admin/Products/Delete/5 (Giữ nguyên, hoặc điều chỉnh logic xóa OrderDetails nếu cần)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Logic kiểm tra OrderDetails nếu bạn có (đã comment out trong code mẫu)
            // var hasOrderDetails = await _dbContext.OrderDetails.AnyAsync(od => od.ProductId == id);
            // if (hasOrderDetails)
            // {
            //     TempData["ErrorMessage"] = "Không thể xóa tài liệu này vì nó đã được đặt trong các đơn hàng.";
            //     return RedirectToAction(nameof(Index));
            // }

            var product = await _dbContext.Products.FindAsync(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tài liệu đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài liệu để xóa.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(int id)
        {
            return await _dbContext.Products.AnyAsync(e => e.ProductId == id);
        }
    }
}