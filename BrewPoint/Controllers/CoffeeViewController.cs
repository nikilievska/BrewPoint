using Microsoft.AspNetCore.Mvc;

namespace BrewPoint.Controllers
{
    [Route("Coffee")]
    public class CoffeeViewController : Controller
    {
        [Route("")]
        public IActionResult Index() => View("~/Views/Coffee/Index.cshtml");

        [Route("Detail/{id}")]
        public IActionResult Detail(int id) => View("~/Views/Coffee/Detail.cshtml");
    }
}