using Microsoft.AspNetCore.Mvc;

namespace Delivery.MVC.Controllers
{
    public class ClientController : Controller
    {
        public IActionResult Start() { return View(); }
        public IActionResult Search() { return View(); }
        public IActionResult Orders() { return View(); }
        public IActionResult Reviews() { return View(); }
        public IActionResult Coupons() { return View(); }
        public IActionResult Cart() { return View(); }
        public IActionResult Tracking() { return View(); }

        // NUEVAS
        public IActionResult Menu() { return View(); }
        public IActionResult OrderDetail() { return View(); }
    }
}