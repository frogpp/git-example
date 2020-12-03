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
    public class EmpAgentController : _Controller
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
            using (EmpAgentDac dac = new EmpAgentDac(GetLoginUser()))
            {
                return Json(dac.Read());
            }
        }

        /// <summary>
        /// Create Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            using (EmpUserDac dac = new EmpUserDac(GetLoginUser()))
            {
                ViewBag.OrgEmpUsers = new SelectList(dac.ReadByOrg(GetLoginUser().GetEmpOrg().ORG_ID).Where(p => p.USER_ID != GetLoginUser().USER_ID), "USER_ID", "USER_DISPLAY");
            }
            return View();
        }

        /// <summary>
        /// Create by POST
        /// </summary>
        /// <param name="form"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(FormCollection form, EmpAgentModel model)
        {
            using (EmpAgentDac dac = new EmpAgentDac(GetLoginUser()))
            {
                if (ModelState.IsValid)
                {
                    return Json(dac.Create(form));
                }
                else
                {
                    return Json(new RtnResultModel(false));
                }
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
            using (EmpAgentDac dac = new EmpAgentDac(GetLoginUser()))
            {
                return Json(dac.Delete(form));
            }
        }
    }
}