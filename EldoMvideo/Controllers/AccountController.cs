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
        public async Task<IActionResult> LoginAsync(string email, string password)
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
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Неправильный логин или пароль");
                return View();
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Account()
        {
            return View();
        }
    }
}
