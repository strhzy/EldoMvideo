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
        public async Task<IActionResult> Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            
            List<Account> accounts = await ApiHelper.Get<List<Account>>("accounts");
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

        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("account");
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        public async Task<IActionResult> Account()
        {
            Account account = JsonConvert.DeserializeObject<Account>(Request.Cookies["account"]);
            User user = await ApiHelper.Get<User>("users", account.user_id);
            Role role = await ApiHelper.Get<Role>("roles", account.role_id);

            ViewBag.ProductOrders = await ApiHelper.Get<List<ProductOrder>>("productorders");
            ViewBag.Orders = await ApiHelper.Get<List<Order>>("orders");
            ViewBag.Products = await ApiHelper.Get<List<Product>>("products");
            ViewBag.Users = ApiHelper.Get<List<User>>("users");
            ViewBag.Accounts = ApiHelper.Get<List<Account>>("accounts");
            ViewBag.Deliveries = ApiHelper.Get<List<Delivery>>("deliveries");
            ViewBag.Roles = ApiHelper.Get<List<Role>>("roles");

            ViewBag.Categories = ApiHelper.Get<List<Category>>("categories");

            ViewBag.Account = account;
            ViewBag.Role = role;
            ViewBag.User = user;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {

            string json = JsonConvert.SerializeObject(product);

            ApiHelper.Post<Product>(json, "products");

            return RedirectToAction("Account", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            Product old_product = await ApiHelper.Get<Product>("products", product.id);

            product.product_name = product.product_name ?? old_product.product_name;
            product.category_id = product.category_id > 0 ? product.category_id : old_product.category_id;
            product.pic_link = product.pic_link ?? old_product.pic_link;
            product.price = product.price > 0 ? product.price : old_product.price;
            product.hot = product.hot || old_product.hot;

            string json = JsonConvert.SerializeObject(product);

            bool access = await ApiHelper.Put<Product>(json, "products", product.id);

            return RedirectToAction("Account", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Product product)
        {
            bool access = await ApiHelper.Delete<Product>("products", product.id);

            return RedirectToAction("Account", "Account");
        }

        // Account
        [HttpPost]
        public async Task<IActionResult> Add(Account account)
        {
            string json = JsonConvert.SerializeObject(account);
            ApiHelper.Post<Account>(json, "accounts");
            return RedirectToAction("Account", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Update(Account account)
        {
            Account old_account = await ApiHelper.Get<Account>("accounts", account.id);
            account.acc_login = account.acc_login ?? old_account.acc_login;
            account.password = account.password ?? old_account.password;
            account.role_id = account.role_id > 0 ? account.role_id : old_account.role_id;
            account.user_id = account.user_id > 0 ? account.user_id : old_account.user_id;
            string json = JsonConvert.SerializeObject(account);
            bool access = await ApiHelper.Put<Account>(json, "accounts", account.id);
            return RedirectToAction("Account", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Account account)
        {
            bool access = await ApiHelper.Delete<Account>("accounts", account.id);
            return RedirectToAction("Account", "Account");
        }

        // Category
        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            string json = JsonConvert.SerializeObject(category);
            ApiHelper.Post<Category>(json, "categories");
            return RedirectToAction("Category", "Category");
        }

        [HttpPost]
        public async Task<IActionResult>Update(Category category)
        {
            Category old_category = await ApiHelper.Get<Category>("categories", category.id);
            category.category = category.category ?? old_category.category;
            category.descript = category.descript ?? old_category.descript;
            string json = JsonConvert.SerializeObject(category);
            bool access = await ApiHelper.Put<Category>(json, "categories", category.id);
            return RedirectToAction("Category", "Category");
        }

        [HttpPost]
        public async Task<IActionResult>Delete(Category category)
        {
            bool access = await ApiHelper.Delete<Category>("categories", category.id);
            return RedirectToAction("Category", "Category");
        }

        // Delivery
        [HttpPost]
        public async Task<IActionResult>Add(Delivery delivery)
        {
            string json = JsonConvert.SerializeObject(delivery);
            ApiHelper.Post<Delivery>(json, "deliveries");
            return RedirectToAction("Delivery", "Delivery");
        }

        [HttpPost]
        public async Task<IActionResult>Update(Delivery delivery)
        {
            Delivery old_delivery = await ApiHelper.Get<Delivery>("deliveries", delivery.id);
            delivery.address = delivery.address ?? old_delivery.address;
            delivery.delivery_date = delivery.delivery_date != DateTime.MinValue ? delivery.delivery_date : old_delivery.delivery_date;
            delivery.delivery_time = delivery.delivery_time != TimeSpan.Zero ? delivery.delivery_time : old_delivery.delivery_time;
            string json = JsonConvert.SerializeObject(delivery);
            bool access = await ApiHelper.Put<Delivery>(json, "deliveries", delivery.id);
            return RedirectToAction("Delivery", "Delivery");
        }

        [HttpPost]
        public async Task<IActionResult>Delete(Delivery delivery)
        {
            bool access = await ApiHelper.Delete<Delivery>("deliveries", delivery.id);
            return RedirectToAction("Delivery", "Delivery");
        }

        // Order
        [HttpPost]
        public async Task<IActionResult>Add(Order order)
        {
            string json = JsonConvert.SerializeObject(order);
            ApiHelper.Post<Order>(json, "orders");
            return RedirectToAction("Order", "Order");
        }

        [HttpPost]
        public async Task<IActionResult>Update(Order order)
        {
            Order old_order = await ApiHelper.Get<Order>("orders", order.id);
            order.account_id = order.account_id > 0 ? order.account_id : old_order.account_id;
            order.delivery_id = order.delivery_id > 0 ? order.delivery_id : old_order.delivery_id;
            order.order_date = order.order_date != DateTime.MinValue ? order.order_date : old_order.order_date;
            order.total_sum = order.total_sum > 0 ? order.total_sum : old_order.total_sum;
            string json = JsonConvert.SerializeObject(order);
            bool access = await ApiHelper.Put<Order>(json, "orders", order.id);
            return RedirectToAction("Order", "Order");
        }

        [HttpPost]
        public async Task<IActionResult>Delete(Order order)
        {
            bool access = await ApiHelper.Delete<Order>("orders", order.id);
            return RedirectToAction("Order", "Order");
        }

        // ProductOrder
        [HttpPost]
        public async Task<IActionResult>Add(ProductOrder productOrder)
        {
            string json = JsonConvert.SerializeObject(productOrder);
            ApiHelper.Post<ProductOrder>(json, "productorders");
            return RedirectToAction("ProductOrder", "ProductOrder");
        }

        [HttpPost]
        public async Task<IActionResult>Update(ProductOrder productOrder)
        {
            ProductOrder old_productOrder = await ApiHelper.Get<ProductOrder>("productorders", productOrder.id);
            productOrder.product_id = productOrder.product_id > 0 ? productOrder.product_id : old_productOrder.product_id;
            productOrder.order_id = productOrder.order_id > 0 ? productOrder.order_id : old_productOrder.order_id;
            productOrder.quantity = productOrder.quantity > 0 ? productOrder.quantity : old_productOrder.quantity;
            string json = JsonConvert.SerializeObject(productOrder);
            bool access = await ApiHelper.Put<ProductOrder>(json, "productorders", productOrder.id);
            return RedirectToAction("ProductOrder", "ProductOrder");
        }

        [HttpPost]
        public async Task<IActionResult>Delete(ProductOrder productOrder)
        {
            bool access = await ApiHelper.Delete<ProductOrder>("productorders", productOrder.id);
            return RedirectToAction("ProductOrder", "ProductOrder");
        }

        // Role
        [HttpPost]
        public async Task<IActionResult>Add(Role role)
        {
            string json = JsonConvert.SerializeObject(role);
            ApiHelper.Post<Role>(json, "roles");
            return RedirectToAction("Role", "Role");
        }

        [HttpPost]
        public async Task<IActionResult>Update(Role role)
        {
            Role old_role = await ApiHelper.Get<Role>("roles", role.id);
            role.role_name = role.role_name ?? old_role.role_name;
            string json = JsonConvert.SerializeObject(role);
            bool access = await ApiHelper.Put<Role>(json, "roles", role.id);
            return RedirectToAction("Role", "Role");
        }

        [HttpPost]
        public async Task<IActionResult>Delete(Role role)
        {
            bool access = await ApiHelper.Delete<Role>("roles", role.id);
            return RedirectToAction("Role", "Role");
        }

        // User
        [HttpPost]
        public async Task<IActionResult>Add(User user)
        {
            string json = JsonConvert.SerializeObject(user);
            ApiHelper.Post<User>(json, "users");
            return RedirectToAction("User", "User");
        }

        [HttpPost]
        public async Task<IActionResult>Update(User user)
        {
            User old_user = await ApiHelper.Get<User>("users", user.id);
            user.first_name = user.first_name ?? old_user.first_name;
            user.middle_name = user.middle_name ?? old_user.middle_name;
            user.last_name = user.last_name ?? old_user.last_name;
            string json = JsonConvert.SerializeObject(user);
            bool access = await ApiHelper.Put<User>(json, "users", user.id);
            return RedirectToAction("User", "User");
        }

        [HttpPost]
        public async Task<IActionResult>Delete(User user)
        {
            bool access = await ApiHelper.Delete<User>("users", user.id);
            return RedirectToAction("User", "User");
        }
    }
}
