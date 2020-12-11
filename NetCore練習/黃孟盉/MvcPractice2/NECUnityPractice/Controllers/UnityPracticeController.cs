using NECPractice.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Newtonsoft.Json;

namespace NECPractice.Controllers
{
    public class UnityPracticeController : Controller
    {
        public ActionResult UnityPractice()
        {
            return View();
        }

        [HttpPost]
        public string GetList(string text)
        {
            List<Interest> list = new List<Interest>();
            using (TestDBEntities1 db = new TestDBEntities1())
            {
                list = (from s in db.Interest select s ).Where(c => c.Food.Contains(text)).ToList();
            }
            return JsonConvert.SerializeObject(list);

        }

        [HttpPost]
        public string Add(Interest list)
        {
            using (TestDBEntities1 db = new TestDBEntities1())
            {
                db.Interest.Add(list);
                db.SaveChanges();
            }
            return JsonConvert.SerializeObject(list);

        }

        [HttpPost]
        public string Updata(Interest list)
        {
            List<Interest> result = new List<Interest>();
            using (TestDBEntities1 db = new TestDBEntities1())
            {
                db.Interest.Find(list.Id).Food = list.Food;
                db.Interest.Find(list.Id).Drink = list.Drink;
                db.Interest.Find(list.Id).Sport = list.Sport;
                db.SaveChanges();
                result = (from s in db.Interest select s).ToList();
            }
            return JsonConvert.SerializeObject(result);

        }

        [HttpPost]
        public string Delete(Interest list)
        {
            using (TestDBEntities1 db = new TestDBEntities1())
            {
                db.Interest.Remove(db.Interest.Find(list.Id));
                db.SaveChanges();
            }
            return JsonConvert.SerializeObject(list);

        }
    }
}
