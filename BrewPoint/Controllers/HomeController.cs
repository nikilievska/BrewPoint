using Microsoft.AspNetCore.Mvc;

namespace BrewPoint.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}