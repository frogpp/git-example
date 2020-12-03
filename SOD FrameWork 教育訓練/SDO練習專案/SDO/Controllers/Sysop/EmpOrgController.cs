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
    public class EmpOrgController : _Controller
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
        /// Get all EMP_ORGs
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryEmpOrgs(FormCollection form, string ORG_ID = "")
        {
            using (EmpOrgDac dac = new EmpOrgDac(GetLoginUser()))
            {
                return Json(dac.ReadByParentOrg(HttpUtility.HtmlEncode(ORG_ID)), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get all ORGs
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult QueryOrgs()
        {
            using (EmpOrgDac dac = new EmpOrgDac(GetLoginUser()))
            {
                IList<EmpOrgModel> results = dac.Read();
                return Json(dac.Read().OrderBy(p => p.PARENT_ID + p.ORG_ID).Select(p => new
                {
                    p.ORG_ID,
                    p.ORG_NAME,
                    p.PARENT_ID,
                }).ToArray());
            }
        }

        /// <summary>
        ///  Query multiple EMP_ORGs
        /// </summary>
        /// <param name="orgIds"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult QueryMultiOrgs(string[] orgIds)
        {
            using (EmpOrgDac dac = new EmpOrgDac(GetLoginUser()))
            {
                return Json(dac.Read(orgIds.Select(p => HttpUtility.HtmlEncode(p)).ToArray()));
            }
        }

        /// <summary>
        /// Get all org's users
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult QueryOrgUsers(string orgId = "")
        {
            using (EmpUserDac dac = new EmpUserDac(GetLoginUser()))
            {
                IList<EmpUserModel> orgUsers = dac.ReadByOrg(orgId);
                return Json(dac.ReadByOrg(orgId).Select(p => new
                {
                    p.USER_ID,
                    p.USER_DISPLAY
                }).ToArray());
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
                ViewBag.EmpUsers = dac.Read().Where(p => string.IsNullOrWhiteSpace(p.GetEmpOrg().ORG_ID)).OrderBy(p => p.USER_NAME).Select(p => new SelectListItem()
                {
                    Value = p.USER_ID,
                    Text = p.USER_DISPLAY
                }).ToList();
                ViewBag.EmpOrgUsers = dac.ReadByOrg().Select(p => new SelectListItem()
                {
                    Value = p.USER_ID,
                    Text = p.USER_DISPLAY
                }).ToList();
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
        public JsonResult Create(FormCollection form)
        {
            using (EmpOrgDac dac = new EmpOrgDac(GetLoginUser()))
            {
                return Json(dac.Create(form));
            }
        }

        /// <summary>
        /// Update page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Update(string id)
        {
            using (EmpUserDac dac = new EmpUserDac(GetLoginUser()))
            {
                ViewBag.EmpUsers = dac.Read().Where(p => string.IsNullOrWhiteSpace(p.GetEmpOrg().ORG_ID)).OrderBy(p => p.USER_NAME).Select(p => new SelectListItem()
                {
                    Value = p.USER_ID,
                    Text = p.USER_DISPLAY
                }).ToList();
                ViewBag.EmpOrgUsers = dac.ReadByOrg(id).Select(p => new SelectListItem()
                {
                    Value = p.USER_ID,
                    Text = p.USER_DISPLAY
                }).ToList();
            }
            using (EmpOrgDac dac = new EmpOrgDac(GetLoginUser()))
            {
                EmpOrgModel model = dac.Read(id);
                ViewBag.ParentOrg = dac.ReadByOrg(model.PARENT_ID).Select(p => new SelectListItem()
                {
                    Value = p.ORG_ID,
                    Text = p.ORG_DISPLAY
                }).ToList();
                return View(model);
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
            using (EmpOrgDac dac = new EmpOrgDac(GetLoginUser()))
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
            using (EmpOrgDac dac = new EmpOrgDac(GetLoginUser()))
            {
                return Json(dac.Delete(form));
            }
        }
    }
}