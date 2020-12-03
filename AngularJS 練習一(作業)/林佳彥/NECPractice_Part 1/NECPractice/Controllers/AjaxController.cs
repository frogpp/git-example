using System.Collections.Generic;
using System.Web.Mvc;
using NECPractice.Models;
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

        public JsonResult GetItemss() {
            //TestDBEntities1 testDBEntities1 = new TestDBEntities1();
            Vendor_D[] vens = {
                new Vendor_D { SupId="1", FirmName="item1", FirmNo="1"},
                new Vendor_D { SupId="2", FirmName="item2", FirmNo="2"},
                new Vendor_D { SupId="3", FirmName="item3", FirmNo="3"}
            };
            /*doSomethingModel doSomething = new doSomethingModel();
            List<Vendor_D> vens = new List<Vendor_D>();
            vens = doSomething.getData();*/
            return Json(vens, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult PostData(string SupId, string FirmNo, string FirmName) {
            Item2[] items = { new Item2 { FirmNo = FirmNo, FirmName = FirmName, SupId = SupId } };
            
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /*[HttpPost]
        public ActionResult PostData(Vendor_D ven) {
            var result = Request.Form;
            return Json(ven);
        }*/

        public string GetXMLItems()
        {
            Response.ContentType = "application/xml";
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?><items></items>";
        }

        class Item
        {
            public string id { get; set; }
            public string name { get; set; }
            public int amount { get; set; }
        }

        class Item2
        {
            public string SupId { get; set; }
            public string FirmNo { get; set; }
            public string FirmName { get; set; }
        }
    }
}
