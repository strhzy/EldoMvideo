using EldoMvideo.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web;
using RestSharp;
using Microsoft.AspNetCore.Http;
using System.Net;
using Newtonsoft.Json;

namespace EldoMvideo.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            
            List<Account> accounts = ApiHelper.Get<List<Account>>("accounts");
            Account acc = accounts.FirstOrDefault(account => account.acc_login == email);

            if (acc != null && acc.password == password)
            {
                Response.Cookies.Append("account", JsonConvert.SerializeObject(acc), new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddYears(1),
                    IsEssential = true,
                    Secure = true,
                    HttpOnly = true
                });
                var cookie = Request.Cookies[acc.acc_login];
                return RedirectToAction("Account", "Account");
            }
            else
            {
                ViewBag.Error = "Неправильный логин или пароль";
                return View();
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("account");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Account()
        {
            Account account = JsonConvert.DeserializeObject<Account>(Request.Cookies["account"]);
            User user = ApiHelper.Get<User>("users", account.user_id);
            Role role = ApiHelper.Get<Role>("roles", account.role_id);

            ViewBag.ProductOrders = ApiHelper.Get<List<ProductOrder>>("productorders");
            ViewBag.Orders = ApiHelper.Get<List<Order>>("orders");
            ViewBag.Products = ApiHelper.Get<List<Product>>("products");

            ViewBag.Categories = ApiHelper.Get<List<Category>>("categories");

            ViewBag.Account = account;
            ViewBag.Role = role;
            ViewBag.User = user;


            return View();
        }

        [HttpPost]
        public IActionResult Add(Product product)
        {

            string json = JsonConvert.SerializeObject(product);

            ApiHelper.Post<Product>(json, "products");

            return RedirectToAction("Account", "Account");
        }

        [HttpPost]
        public IActionResult Update(Product product)
        {
            Product old_product = ApiHelper.Get<Product>("products", product.id);

            product.product_name = product.product_name ?? old_product.product_name;
            product.category_id = product.category_id > 0 ? product.category_id : old_product.category_id;
            product.pic_link = product.pic_link ?? old_product.pic_link;
            product.price = product.price > 0 ? product.price : old_product.price;
            product.hot = product.hot || old_product.hot;

            string json = JsonConvert.SerializeObject(product);

            bool access = ApiHelper.Put<Product>(json, "products", product.id);

            return RedirectToAction("Account", "Account");
        }

        [HttpPost]
        public IActionResult Delete(int productselectdel)
        {
            bool access = ApiHelper.Delete<Product>("products", productselectdel);

            return RedirectToAction("Account", "Account");
        }
    }
}
