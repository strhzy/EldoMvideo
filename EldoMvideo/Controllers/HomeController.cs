using EldoMvideo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;

namespace EldoMvideo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Categories = ApiHelper.Get<List<Category>>("categories");
            ViewBag.HotProducts = ApiHelper.Get<List<Product>>("products").Where(p => p.hot == true).ToList();
            ViewBag.Products = ApiHelper.Get<List<Product>>("products");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Catalog(int? category_id)
        {
            bool check_cat = false;
            if (category_id == null)
            {
                check_cat = false;
            }
            else { check_cat = true; }

            ViewBag.CategoryId = category_id;
            ViewBag.check = check_cat;

            var products = ApiHelper.Get<List<Product>>("products");

            ViewBag.Products = products;
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

 

        
    }
}
