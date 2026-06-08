using Microsoft.AspNetCore.Mvc;

namespace BrewPoint.Controllers
{
    [Route("Order")]
    public class OrderViewController : Controller
    {
        [Route("MyOrders")]
        public IActionResult MyOrders() => View("~/Views/Order/MyOrders.cshtml");
    }
}