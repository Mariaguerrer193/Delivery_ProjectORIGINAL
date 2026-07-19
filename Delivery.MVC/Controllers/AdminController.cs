using Microsoft.AspNetCore.Mvc;

namespace Delivery.MVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Start() { return View(); }
        public IActionResult CouponManagement() { return View(); }
        public IActionResult ReviewModeration() { return View(); }
        public IActionResult ReportManagement() { return View(); }

        
        public IActionResult UserManagement() { return View(); }
    }
}