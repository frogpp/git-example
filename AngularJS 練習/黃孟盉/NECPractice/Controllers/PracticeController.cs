using System.Collections.Generic;
using System.Web.Mvc;

namespace NECPractice.Controllers
{
    public class PracticeController : Controller
    {
        public ActionResult Practice1()
        {
            return View();
        }

        public ActionResult Practice2()
        {
            return View();
        }

        public ActionResult Practice3()
        {
            return View();
        }

        public ActionResult Practice4()
        {
            return View();
        }

        public JsonResult GetItems()
        {
            List<string> items = new List<string> { "a", "b", "c" };
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostItems(int number)
        {
            List<int> items = new List<int> { 1 * number, 2 * number, 3 * number };
            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }
}
