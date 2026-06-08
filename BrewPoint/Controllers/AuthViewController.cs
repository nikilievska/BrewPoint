using Microsoft.AspNetCore.Mvc;

namespace BrewPoint.Controllers
{
    [Route("Auth")]
    public class AuthViewController : Controller
    {
        [Route("Login")]
        public IActionResult Login() => View("~/Views/Auth/Login.cshtml");

        [Route("Register")]
        public IActionResult Register() => View("~/Views/Auth/Register.cshtml");
    }
}