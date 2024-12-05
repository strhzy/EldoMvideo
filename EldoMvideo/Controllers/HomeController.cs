using EldoMvideo.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories =await ApiHelper.Get<List<Category>>("categories");
            List<Product> hot_prod = await (ApiHelper.Get<List<Product>>("products"));
            ViewBag.Products = await ApiHelper.Get<List<Product>>("products");

            ViewBag.HotProducts = hot_prod.Where(p => p.hot == true).ToList();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Catalog(int? category_id)
        {
            bool check_cat = false;
            if (category_id == null)
            {
                check_cat = false;
            }
            else { check_cat = true; }

            ViewBag.CategoryId = category_id;
            ViewBag.check = check_cat;

            var products = await ApiHelper.Get<List<Product>>("products");

            ViewBag.Products = products;
            string path = Request.Path + Request.QueryString;

            ViewBag.Path = path;
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
        public async Task<IActionResult> AddToCart()
        {
            int ID = Convert.ToInt32(Request.Query["ID"]);
            string path = Request.Query["Path"];
            path = path == null ? "/Home/Index" : path;

            Cart cart = new Cart();
            if (Request.Cookies["Cart"] != null)
                cart = JsonConvert.DeserializeObject<Cart>(Request.Cookies["Cart"]);
            cart.CartLines.Add(await ApiHelper.Get<Product>("products", ID));

            Response.Cookies.Append("Cart", JsonConvert.SerializeObject(cart));

            return Redirect(path);
        }

        public async Task<IActionResult> RemoveFromCart()
        {
            int number = Convert.ToInt32(Request.Query["Number"]);

            Cart cart = new Cart();
            if (Request.Cookies["Cart"] != null)
                cart = JsonConvert.DeserializeObject<Cart>(Request.Cookies["Cart"]);
            cart.CartLines.RemoveAt(number);

            Response.Cookies.Append("Cart", JsonConvert.SerializeObject(cart));

            return RedirectToAction("Cart", "Home");
        }

        public async Task<IActionResult> RemoveAllFromCart()
        {
            int ID = Convert.ToInt32(Request.Query["ID"]);

            Cart cart = new Cart();
            if (Request.Cookies["Cart"] != null)
                cart = JsonConvert.DeserializeObject<Cart>(Request.Cookies["Cart"]);
            cart.CartLines.Clear();
            Response.Cookies.Append("Cart", JsonConvert.SerializeObject(cart));

            return RedirectToAction("Cart", "Home");
        }

        public async Task<IActionResult> Cart()
        {
            Cart cart = new Cart();
            if (Request.Cookies["Cart"] != null)
                cart = JsonConvert.DeserializeObject<Cart>(Request.Cookies["Cart"]);

            ViewBag.Cart = cart;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MakeOrder(Delivery delivery)
        {
            Account account = JsonConvert.DeserializeObject<Account>(Request.Cookies["account"]);
            ProductOrder productOrder = new ProductOrder();
            Cart cart = new Cart();

            if (Request.Cookies["Cart"] != null)
                cart = JsonConvert.DeserializeObject<Cart>(Request.Cookies["Cart"]);

            Order order = new Order();

            List<Order> orders = await ApiHelper.Get<List<Order>>("orders");
            List<Delivery> deliveries = await ApiHelper.Get<List<Delivery>>("deiveries");

            int count_ords = orders.Last().id;
            int count_dels = deliveries.Last().id;

            delivery.id = count_dels + 1;

            bool success = await ApiHelper.Post<Delivery>(JsonConvert.SerializeObject(delivery),"deliveries");

            order.id = count_ords+1;
            order.account_id = account.id;
            order.delivery_id = delivery.id;
            order.order_date = DateTime.Now;
            order.total_sum = cart.FinalPrice;

            success = await ApiHelper.Post<Order>(JsonConvert.SerializeObject(order), "orders");

            foreach (var item in cart.CartLines)
            {
                productOrder.product_id = item.id;
                productOrder.order_id = order.id;
                productOrder.quantity = 1;
                ApiHelper.Post<ProductOrder>(JsonConvert.SerializeObject(productOrder), "productorders");
            }

            return RedirectToAction("RemoveAllFromCart", "Home");
        }
    }
}
