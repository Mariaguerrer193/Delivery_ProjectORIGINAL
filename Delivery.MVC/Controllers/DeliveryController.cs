using Microsoft.AspNetCore.Mvc;

namespace Delivery.MVC.Controllers
{
    public class DeliveryController : Controller
    {
        public IActionResult Start() { return View(); }
        public IActionResult Earnings() { return View(); }
        public IActionResult Reviews() { return View(); }
        public IActionResult OrderDetail() { return View(); }
        public IActionResult Reports() { return View(); }

        public IActionResult Vehicle() { return View(); }

        [HttpGet("Delivery/Tracking")]
        public IActionResult Tracking([FromQuery] int orderId)
        {
            // Esto busca y renderiza de forma correcta el archivo Views/Delivery/DeliveryTracking.cshtml
            return View("DeliveryTracking");
        }
    }

}
