using System.Web.Mvc;

namespace NECPractice.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.IsHome = true;
            return View();
        }
    }
}
