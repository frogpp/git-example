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
    }
}
