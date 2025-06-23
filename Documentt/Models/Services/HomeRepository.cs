using Documentt.Data;
using Documentt.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace Documentt.Models.Services
{
    public class HomeRepository:IHomeRepository
    {
        private TailieuContextDB db;
        public HomeRepository(TailieuContextDB db)
        {
            this.db = db;
        }
        public IEnumerable<Product> GetAllProducts()
        {
            return db.Products;
        }
        public IEnumerable<Authour> noibat()
        {
            return db.Authours.Where(p => p.noibat);
        }
        public IEnumerable<Product> Trending()
        {
            return db.Products
                .Where(p => p.Trending)
                .Include(p => p.Category)
                .ToList();
                
        }
        public IEnumerable<Product> New()
        {
            return db.Products
                 .Where(p => p.New)
                 .Include(p => p.Category)
                 .ToList();
        }
       
    }
}
