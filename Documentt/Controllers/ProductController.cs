using Documentt.Data;
using Documentt.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Documentt.Controllers
{
    public class ProductController : Controller
    {
        private TailieuContextDB contextDB;
        public ProductController(TailieuContextDB contextDB)
        {
            this.contextDB = contextDB;
        }
        public IActionResult Index(string sort)
        {
            var products = contextDB.Products
                .Include(p => p.Category)
                .AsQueryable();
             var categories = contextDB.Categories.ToList();
    ViewBag.Categories = categories;
            switch (sort)
            {
                case "price-asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price-desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductId);
                    break;
            }

            return View(products.ToList());
        }

        public IActionResult Getcategory(int cateId)
        {
            var products = contextDB.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == cateId)
                .ToList();

            var categories = contextDB.Categories.ToList(); // lấy tất cả category
            ViewBag.Categories = categories;

            return View("Index", products);
        }
        public IActionResult Detail(int id)
        {
            var product = contextDB.Products
                .Include(p => p.Category) 
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
