using NECPractice.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace NECPractice.Controllers
{
    public class AjaxController : Controller
    {
        public JsonResult GetItems()
        {
            Item[] items = {
                               new Item{id="1", name="item1", amount=12345},
                               new Item{id="2", name="item2", amount=1234567},
                               new Item{id="3", name="item3", amount=123},
                           };


            return Json(items, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItems2()
        {
            Item[] items = {
                               new Item{id="1", name="Jani", amount=54321},
                               new Item{id="2", name="Hege", amount=7654321},
                               new Item{id="3", name="Kai", amount=321},
                           };



            return Json(items, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetItems3(Item data)
        {
            Item[] items = {
                               new Item{id=data.id, name=data.name, amount=data.amount},
                               new Item{id="2", name="Hege", amount=7654321},
                               new Item{id="3", name="Kai", amount=321},
                           };



            return Json(items, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetItems3()
        //{
        //    List<Vendor_D> vendor_Ds = new List<Vendor_D>();
        //    using (TestDBEntities1 context = new TestDBEntities1())
        //    {
        //        context.Database.Connection.Open();
        //        vendor_Ds = context.Vendor_D.ToList() ;
        //        // the rest
        //    }
        //    return Json(vendor_Ds, JsonRequestBehavior.AllowGet);
        //}

        public string GetXMLItems()
        {
            Response.ContentType = "application/xml";
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?><items></items>";
        }

        public class Item
        {
            public string id { get; set; }
            public string name { get; set; }
            public int amount { get; set; }
        }
    }
}
