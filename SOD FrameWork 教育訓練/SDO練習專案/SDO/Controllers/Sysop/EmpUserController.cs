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
    public class EmpUserController : _Controller
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
        /// Query EMP_USER
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Query(FormCollection form)
        {
            using (EmpUserDac dac = new EmpUserDac(GetLoginUser()))
            {
                return Json(dac.Read(form));
            }
        }

        /// <summary>
        /// Query multiple EMP_USER
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult QueryUsers(string[] userIds)
        {
            using (EmpUserDac dac = new EmpUserDac(GetLoginUser()))
            {
                return Json(dac.Read(userIds.Select(p=> HttpUtility.HtmlEncode(p)).ToArray()));
            }
        }

        /// <summary>
        /// Create Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            using (DimRoleDac dac = new DimRoleDac(GetLoginUser()))
            {
                ViewBag.DimRoles = dac.Read().Select(p => new SelectListItem() {
                    Value=p.ROLE_ID,
                    Text = p.ROLE_DISPLAY
                }).ToList();
                ViewBag.EmpUserRoles = dac.ReadByUser().Select(p => new SelectListItem()
                {
                    Value = p.ROLE_ID,
                    Text = p.ROLE_DISPLAY
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
        public JsonResult Create(FormCollection form, IEnumerable<HttpPostedFileBase> uploader)
        {
            using (EmpUserDac dac = new EmpUserDac(GetLoginUser()))
            {
                return Json("");
            }
        }
        
    }
}