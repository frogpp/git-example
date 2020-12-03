using SDO.Dacs;
using SDO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SDO.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class DimRoleController : _Controller
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
            using (DimRoleDac dac = new DimRoleDac(GetLoginUser()))
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
            using (DimRightDac dac = new DimRightDac(GetLoginUser()))
            {
                ViewBag.DimRights = dac.Read().Select(p => new SelectListItem()
                {
                    Value = p.RIGHT_ID,
                    Text = p.RIGHT_DISPLAY
                }).ToList();
                ViewBag.DimRoleRights = dac.ReadByRole().Select(p => new SelectListItem()
                {
                    Value = p.RIGHT_ID,
                    Text = p.RIGHT_DISPLAY
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
            using (DimRoleDac dac = new DimRoleDac(GetLoginUser()))
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
                return RedirectToAction("Create", "DimRole");
            }
            using (DimRightDac dac = new DimRightDac(GetLoginUser()))
            {
                ViewBag.DimRights = dac.Read().Select(p => new SelectListItem()
                {
                    Value = p.RIGHT_ID,
                    Text = p.RIGHT_DISPLAY
                }).ToList();
                ViewBag.DimRoleRights = dac.ReadByRole(id).Select(p => new SelectListItem()
                {
                    Value = p.RIGHT_ID,
                    Text = p.RIGHT_DISPLAY
                }).ToList();
            }
            using (DimRoleDac dac = new DimRoleDac(GetLoginUser()))
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
            using (DimRoleDac dac = new DimRoleDac(GetLoginUser()))
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
            using (DimRoleDac dac = new DimRoleDac(GetLoginUser()))
            {
                return Json(dac.Delete(form));
            }
        }
    }
}