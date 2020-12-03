using SDO.Dacs;
using SDO.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SDO.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class DimRightController : _Controller
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
        public JsonResult Query(FormCollection form)
        {
            using (DimRightDac dac = new DimRightDac(GetLoginUser()))
            {
                return Json(dac.Read());
            }
        }

        /// <summary>
        /// Create page
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            using (SetFunctionDac dac = new SetFunctionDac(GetLoginUser()))
            {
                ViewBag.SetFunctions = dac.Read().Where(p => !string.IsNullOrWhiteSpace(p.PARENT_ID)).OrderBy(p => p.PARENT_ID + p.SORT_ID).Select(p => new SelectListItem()
                {
                    Value = p.FUNCTION_ID,
                    Text = p.FUNCTION_DISPLAY
                }).ToList();
                ViewBag.DimRightFunctions = dac.ReadByRight().OrderBy(p => p.PARENT_ID + p.SORT_ID).Select(p => new SelectListItem()
                {
                    Value = p.FUNCTION_ID,
                    Text = p.FUNCTION_DISPLAY
                }).ToList();
            }
            return View();
        }

        /// <summary>
        /// Create by POST
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(FormCollection form)
        {
            using (DimRightDac dac = new DimRightDac(GetLoginUser()))
            {
                return Json(dac.Create(form));
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
                return RedirectToAction("Create", "DimRight");
            }
            using (SetFunctionDac dac = new SetFunctionDac(GetLoginUser()))
            {
                ViewBag.SetFunctions = dac.Read().Where(p => !string.IsNullOrWhiteSpace(p.PARENT_ID)).OrderBy(p => p.PARENT_ID + p.SORT_ID).Select(p => new SelectListItem()
                {
                    Value = p.FUNCTION_ID,
                    Text = p.FUNCTION_DISPLAY
                }).ToList();
                ViewBag.DimRightFunctions = dac.ReadByRight(id).OrderBy(p => p.PARENT_ID + p.SORT_ID).Select(p => new SelectListItem()
                {
                    Value = p.FUNCTION_ID,
                    Text = p.FUNCTION_DISPLAY
                }).ToList();
            }
            using (DimRightDac dac = new DimRightDac(GetLoginUser()))
            {
                return View(dac.Read(id));
            }
        }

        /// <summary>
        /// Update by POST
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update(FormCollection form)
        {
            using (DimRightDac dac = new DimRightDac(GetLoginUser()))
            {
                return Json(dac.Update(form));
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
            using (DimRightDac dac = new DimRightDac(GetLoginUser()))
            {
                return Json(dac.Delete(form));
            }
        }
    }
}