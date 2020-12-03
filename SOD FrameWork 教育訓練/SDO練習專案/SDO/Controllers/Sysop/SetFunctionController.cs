using SDO.Dacs;
using SDO.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDO.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class SetFunctionController : _Controller
    {
        /// <summary>
        /// Query page
        /// </summary>
        /// <returns></returns>
        public ActionResult Query()
        {
            using (SetFunctionDac dac = new SetFunctionDac(GetLoginUser()))
            {
                ViewBag.GroupFunctions = new SelectList(dac.ReadByGroup(), "FUNCTION_ID", "FUNCTION_DISPLAY");
                return View();
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
            using (SetFunctionDac dac = new SetFunctionDac(GetLoginUser()))
            {
                IList<SetFunctionModel> result = dac.ReadByGroup(form.Get("qryPARENT_ID"));
                if (!string.IsNullOrWhiteSpace(form.Get("qryFUNCTION_ID")))
                {
                    result = result.Where(p => p.FUNCTION_ID.ToLower().Contains(HttpUtility.HtmlEncode(form.Get("qryFUNCTION_ID").Trim().ToLower()))).ToList();
                }
                if (!string.IsNullOrWhiteSpace(form.Get("qryFUNCTION_NAME")))
                {
                    result = result.Where(p => p.FUNCTION_NAME.Contains(HttpUtility.HtmlEncode(form.Get("qryFUNCTION_NAME").Trim()))).ToList();
                }
                return Json(result);
            }
        }

        /// <summary>
        /// Update SET_FUNCTION's sortorder by POST
        /// </summary>
        /// <param name="sortData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetSortorder(string[] sortData)
        {
            using (SetFunctionDac dac = new SetFunctionDac(GetLoginUser()))
            {
                return Json(dac.UpdateSortorder(sortData));
            }
        }


        /// <summary>
        /// Create page
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(string ID)
        {
            using (SetFunctionDac dac = new SetFunctionDac(GetLoginUser()))
            {
                ViewBag.GroupFunctions = new SelectList(string.IsNullOrWhiteSpace(ID) ? dac.ReadByGroup() : new List<SetFunctionModel>(), "FUNCTION_ID", "FUNCTION_DISPLAY");
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
            using (SetFunctionDac dac = new SetFunctionDac(GetLoginUser()))
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
                return RedirectToAction("Create", "SetFunction");
            }
            using (SetFunctionDac dac = new SetFunctionDac(GetLoginUser()))
            {
                SetFunctionModel model = dac.Read(id);
                ViewBag.GroupFunctions = new SelectList(!string.IsNullOrWhiteSpace(model.PARENT_ID) ? dac.ReadByGroup() : new List<SetFunctionModel>(), "FUNCTION_ID", "FUNCTION_DISPLAY", model.PARENT_ID);
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
            using (SetFunctionDac dac = new SetFunctionDac(GetLoginUser()))
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
            using (SetFunctionDac dac = new SetFunctionDac(GetLoginUser()))
            {
                return Json(dac.Delete(form));
            }
        }

        /// <summary>
        /// Query By USER_ID
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult QueryUser(FormCollection form)
        {
            using (SetFunctionDac dac = new SetFunctionDac(GetLoginUser()))
            {
                return Json(dac.ReadByUser(form.Get("USER_ID").ToString()).Select(p =>
                {
                    p.FUNCTION_URL = p.FUNCTION_URL.Trim().Length > 0 ? Url.Content(p.FUNCTION_URL) : "";
                    return p;
                }));
            }
        }
    }
}