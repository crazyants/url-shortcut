using Microsoft.AspNetCore.Mvc;

namespace URL_Shortcut.Controllers
{
    [Route("")]
    [Route("Home")]
    public class HomeController : Controller
    {
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
