using Documentt.Data;
using Documentt.Models;
using Documentt.Models.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Documentt.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly IHomeRepository homeRepository;
        public HomeController(IHomeRepository homeRepository)
        {
            this.homeRepository = homeRepository;
        }
        public IActionResult Index()
        {
            ViewBag.New = homeRepository.New();
            ViewBag.Trending = homeRepository.Trending();
            ViewBag.noibat = homeRepository.noibat();

            return View();
        }
       
    }
}
