using SDO.Dacs;
using SDO.Models;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using System.Web;

namespace SDO.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///  
    /// </remarks>
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class eFormController : _Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Query()
        {
            using (eFormDac dac = new eFormDac(GetLoginUser()))
            {
                ViewBag.eFormList = new SelectList(dac.ReadForms(), "FORM_ID", "FORM_DISPLAY");
            }
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ContentResult Query(FormCollection form)
        {
            using (eFormDac dac = new eFormDac(GetLoginUser()))
            {
                return Content(JsonConvert.SerializeObject(dac.Read(form)), "application/json");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SendFlow(FormCollection form)
        {
            using (eFormDac dac = new eFormDac(GetLoginUser()))
            {
                return Json(dac.UpdateFlow(form));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryFlow()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ContentResult QueryFlow(FormCollection form)
        {
            using (eFormDac dac = new eFormDac(GetLoginUser()))
            {
                return Content(JsonConvert.SerializeObject(dac.ReadFlowList(form)), "application/json");
            }
        }

        /// <summary>
        /// Create Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(string ID)
        {
            ViewBag.eFormId = ID;
            ViewBag.fillId = "";
            return View();
        }

        /// <summary>
        /// Get eFields by eForm id
        /// </summary>
        /// <param name="eFormId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ContentResult QueryFields(string eFormId)
        {
            using (eFormDac dac = new eFormDac(GetLoginUser()))
            {
                return Content(JsonConvert.SerializeObject(dac.ReadFields(HttpUtility.HtmlEncode(eFormId))), "application/json");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(FormCollection form)
        {
            using (eFormDac dac = new eFormDac(GetLoginUser()))
            {
                return Json(dac.Create(form));
            }
        }

        /// <summary>
        /// Update page
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Update(string ID)
        {
            ViewBag.fillId = ID;
            return View();
        }

        /// <summary>
        /// Get DataFill by fill id
        /// </summary>
        /// <param name="fillId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ContentResult QueryDataFills(string fillId)
        {
            using (eFormDac dac = new eFormDac(GetLoginUser()))
            {
                return Content(dac.ReadDataFill(HttpUtility.HtmlEncode(fillId)) ?? "");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update(FormCollection form)
        {
            using (eFormDac dac = new eFormDac(GetLoginUser()))
            {
                return Json(dac.Update(form));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(FormCollection form)
        {
            using (eFormDac dac = new eFormDac(GetLoginUser()))
            {
                return Json(dac.Delete(form));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Audit(string ID)
        {
            ViewBag.fillId = ID;
            return View();
        }
    }
}