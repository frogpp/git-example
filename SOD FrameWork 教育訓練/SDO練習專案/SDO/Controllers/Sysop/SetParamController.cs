using SDO.Dacs;
using SDO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDO.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class SetParamController : _Controller
    {
        /// <summary>
        /// Query page
        /// </summary>
        /// <returns></returns>
        public ActionResult Query()
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
        public JsonResult QueryItem(FormCollection form)
        {
            using (SetParamDac dac = new SetParamDac(GetLoginUser()))
            {
                return Json(dac.ReadItem());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Query(FormCollection form)
        {
            using (SetParamDac dac = new SetParamDac(GetLoginUser()))
            {
                return Json(dac.Read(HttpUtility.HtmlEncode(form.Get("qrySET_ITEM"))));
            }
        }

        /// <summary>
        /// Careate page paramitem 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateItem()
        {
            return View();
        }

        /// <summary>
        /// Careate page paramitem by post
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateItem(FormCollection form)
        {
            using (SetParamDac dac = new SetParamDac(GetLoginUser()))
            {
                return Json(dac.CreateItem(form));
            }
        }

        /// <summary>
        /// Create page
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            using (SetParamDac dac = new SetParamDac(GetLoginUser()))
            {
                ViewBag.SetParamItems = new SelectList(dac.ReadItem(), "SET_ITEM", "SET_ITEM_DISPLAY");
                return View();
            }
        }

        /// <summary>
        /// Create by POST
        /// </summary>
        /// <param name="form"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(FormCollection form)
        {
            using (SetParamDac dac = new SetParamDac(GetLoginUser()))
            {
                return Json(dac.Create(form));
            }
        }

        /// <summary>
        /// Update page paramitem 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UpdateItem(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("CreateItem", "SetParam");
            }
            using (SetParamDac dac = new SetParamDac(GetLoginUser()))
            {
                return View(dac.ReadItem(id));
            }
        }

        /// <summary>
        ///  Update page paramitem by POST
        /// </summary>
        /// <param name="form"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateItem(FormCollection form)
        {
            using (SetParamDac dac = new SetParamDac(GetLoginUser()))
            {
                return Json(dac.UpdateItem(form));
            }
        }

        /// <summary>
        /// Update page
        /// </summary>
        /// <returns></returns>
        public ActionResult Update(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Create", "SetParam");
            }
            string[] setParams = id.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            using (SetParamDac dac = new SetParamDac(GetLoginUser()))
            {
                SetParamModel model = dac.Read(setParams[0], setParams[1]);
                model.MEMO = HttpUtility.HtmlDecode(model.MEMO);
                ViewBag.SetParamItems = new SelectList(dac.ReadItem(), "SET_ITEM", "SET_ITEM_DISPLAY", model.SET_ITEM);
                return View(model);
            }
        }

        /// <summary>
        /// Update by POST
        /// </summary>
        /// <param name="form"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update(FormCollection form)
        {
            using (SetParamDac dac = new SetParamDac(GetLoginUser()))
            {
                return Json(dac.Update(form));
            }
        }

        /// <summary>
        /// Delete SetParamItem by post
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteItem(FormCollection form)
        {
            using (SetParamDac dac = new SetParamDac(GetLoginUser()))
            {
                return Json(dac.DeleteItem(form));
            }
        }

        /// <summary>
        /// Delete by POST
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(FormCollection form)
        {
            using (SetParamDac dac = new SetParamDac(GetLoginUser()))
            {
                return Json(dac.Delete(form));
            }
        }
    }
}