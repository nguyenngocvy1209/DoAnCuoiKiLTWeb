using Documentt.Data;
using Documentt.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace Documentt.Models.Services
{
    public class AuthourRepository: IAuthourRepository
    {
        private TailieuContextDB db;
        public AuthourRepository(TailieuContextDB db)
        {
            this.db = db;
        }

        public IEnumerable<Authour> GetAll()
        {
            return db.Authours;
        }
        public Authour? GetAuthour(int aId)
        {
            return db.Authours
                .Include(p=>p.Products)
                .FirstOrDefault(p => p.AuthourId == aId);
        }
    }
}
