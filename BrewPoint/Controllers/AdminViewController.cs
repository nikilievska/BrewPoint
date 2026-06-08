using Microsoft.AspNetCore.Mvc;

namespace BrewPoint.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AdminViewController : Controller
    {
        [Route("Admin/Orders")]
        public IActionResult Orders() => View("~/Views/Admin/Orders.cshtml");

        [Route("Admin/Coffees")]
        public IActionResult Coffees() => View("~/Views/Admin/Coffees.cshtml");

        [Route("Admin/Coffees/Add")]
        public IActionResult AddCoffee() => View("~/Views/Admin/AddCoffee.cshtml");

        [Route("Admin/Coffees/Edit/{id:int}")]
        public IActionResult EditCoffee(int id) => View("~/Views/Admin/EditCoffee.cshtml");
    }
}