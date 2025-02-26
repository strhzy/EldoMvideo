using EldoMvideo.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using EldoMvideoAPI.Models;
using Microsoft.Extensions.Primitives;

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
            ViewBag.Categories = await ApiHelper.Get<List<Category>>("categories");
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
            string type = products[0].GetType().Name;
            Console.WriteLine(type);

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
            if (Request.Cookies["Cart"] != null &&
                cart.CartLines.Where(cpr => cpr.id == ID).FirstOrDefault() != null)
            {
                cart.CartLines.Where(cpr => cpr.id == ID).FirstOrDefault().quantity += 1;
            }
            else
            {
                Product pr = new Product();
                pr = await ApiHelper.Get<Product>("products", ID);
                pr.quantity = 1;
                cart.CartLines.Add(pr);
            }

            Response.Cookies.Append("Cart", JsonConvert.SerializeObject(cart));

            return Redirect(path);
        }

        public async Task<IActionResult> RemoveFromCart()
        {
            int number = Convert.ToInt32(Request.Query["Number"]);
            string path = Request.Query["Path"];

            Cart cart = new Cart();
            if (Request.Cookies["Cart"] != null)
                cart = JsonConvert.DeserializeObject<Cart>(Request.Cookies["Cart"]);
            
            if (cart.CartLines[number].quantity == 1)
            {
                cart.CartLines.RemoveAt(number);
            }
            else
            {
                cart.CartLines[number].quantity -= 1;
            }

            Response.Cookies.Append("Cart", JsonConvert.SerializeObject(cart));

            return RedirectToAction("Cart", "Home");
        }

        public async Task<IActionResult> RemoveAllFromCart()
        {
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
            Cart cart = new Cart();
            if (Request.Cookies["Cart"] != null)
            {
                cart = JsonConvert.DeserializeObject<Cart>(Request.Cookies["Cart"]);
                if (cart.CartLines.Count == 0)
                {
                    return RedirectToAction("Cart", "Home");
                }
                Account account = JsonConvert.DeserializeObject<Account>(Request.Cookies["account"]);
                delivery.delivery_date = delivery.delivery_date.ToUniversalTime();
                string json = JsonConvert.SerializeObject(delivery);
                bool success = await ApiHelper.Post<Delivery>(json,"deliveries");
                List<Delivery> deliveries = await ApiHelper.Get<List<Delivery>>("deliveries");
                Delivery delivery2 = deliveries.Where(d =>
                    d.delivery_date == delivery.delivery_date &&
                    d.address == delivery.address).FirstOrDefault();
                
                Order order = new Order();
                order.order_date = (DateTime.Now.ToUniversalTime());
                order.delivery_id = delivery2.id;
                order.account_id = account.id;
                order.total_sum = cart.FinalPrice;

                success = await ApiHelper.Post<Order>(JsonConvert.SerializeObject(order), "orders");
                
                List<Order> orders = await ApiHelper.Get<List<Order>>("orders");
                Order order2 = orders
                    .Where(o => 
                        o.delivery_id == order.delivery_id &&
                        o.account_id == order.account_id &&
                        Math.Abs(o.total_sum - order.total_sum) < 0.0001 &&
                        o.order_date.Date == order.order_date.Date)
                    .FirstOrDefault();
                
                ProductOrder productOrder = new ProductOrder();
                productOrder.order_id = order2.id;
                foreach (var item in cart.CartLines)
                {
                    productOrder.product_id = item.id;
                    productOrder.quantity = item.quantity;
                    
                    success = await ApiHelper.Post<ProductOrder>(JsonConvert.SerializeObject(productOrder), "productorders");
                }
                
                if (Request.Cookies["Cart"] != null)
                    cart = JsonConvert.DeserializeObject<Cart>(Request.Cookies["Cart"]);
                cart.CartLines.Clear();
                Response.Cookies.Append("Cart", JsonConvert.SerializeObject(cart));
                
                return RedirectToAction("Account", "Account");
                
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            Product product = await ApiHelper.Get<Product>("products", id);
            
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        
        [HttpPost]
        public IActionResult AddReview(Review model)
        {
            if (ModelState.IsValid)
            {
                model.date = DateTime.UtcNow;
                model.user_id = (JsonConvert.DeserializeObject<User>(Request.Cookies["account"])).id;
                
                ApiHelper.Post<Review>(JsonConvert.SerializeObject(model), "reviews");
                
                return RedirectToAction("Details", "Home", new { id = model.product_id });
            }

            return PartialView("ReviewAdd", model);
        }

        public IActionResult _Footer()
        {
            return PartialView();
        }
    }
}
