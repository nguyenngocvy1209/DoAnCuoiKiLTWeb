using Documentt.Data;
using Documentt.Models.Interface;
using Documentt.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Documentt.Controllers
{
    public class AuthourController : Controller
    {
        private readonly IAuthourRepository ARepository;
        public AuthourController(IAuthourRepository ARepository)
        {
            this.ARepository = ARepository;
        }
        public IActionResult Index()
        {
            return View(ARepository.GetAll()); 
        }
        public IActionResult tacgia(int aId)
        {
            var product = ARepository.GetAuthour(aId);
            if (product != null)
            {
                return View(product);
            }
            return NotFound();
        }
    }
}
