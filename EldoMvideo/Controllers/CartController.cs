using Microsoft.AspNetCore.Mvc;

namespace EldoMvideo.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Cart()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add()
        {
            return RedirectToAction("Catalog", "Home");
        }
    }
}
