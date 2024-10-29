using EldoMvideo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EldoMvideo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DataBaseContext _context;

        public HomeController(ILogger<HomeController> logger, DataBaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Catalog(int? category_id)
        {
            bool check_cat = false;
            if(category_id == null)
            {
                check_cat = false;
            }
            else { check_cat = true; }

            ViewBag.CategoryId = category_id;
            ViewBag.check = check_cat;

            var products = _context.Products.ToList();

            ViewBag.Products = products;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

 

        
    }
}
