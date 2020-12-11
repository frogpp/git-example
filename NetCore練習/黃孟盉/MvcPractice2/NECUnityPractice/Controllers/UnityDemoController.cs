using NECPractice.Models;
using System.Web.Mvc;

namespace NECPractice.Controllers
{
    public class UnityDemoController : Controller
    {
        private readonly TestDBEntities1 dbContext;

        public UnityDemoController()
        {
            dbContext = new TestDBEntities1();
        }

        public ActionResult UnityDemo()
        {
            return View();
        }

        public ActionResult GetInterests()
        {
            return Json(dbContext.Interest);
        }

        public ActionResult AddInterest(Interest interest)
        {
            dbContext.Interest.Add(interest);
            dbContext.SaveChanges();

            return Json(interest);
        }
    }
}
