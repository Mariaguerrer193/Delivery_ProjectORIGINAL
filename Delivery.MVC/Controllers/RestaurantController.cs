using Microsoft.AspNetCore.Mvc;

namespace Delivery.MVC.Controllers
{
    public class RestaurantController : Controller
    {
        public IActionResult Start() { return View(); }

        public IActionResult ReporteFinanciero() { return View(); }
    }
}