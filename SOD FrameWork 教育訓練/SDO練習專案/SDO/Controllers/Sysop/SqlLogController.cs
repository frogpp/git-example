using Newtonsoft.Json;
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
    public class SqlLogController : _Controller
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
        public JsonResult QueryPageable(FormCollection form)
        {
            using (SqlLogDac dac = new SqlLogDac(GetLoginUser()))
            {
                var result = dac.Read(DateTime.Parse(form.Get("log_date")), int.Parse(form.Get("skip")), int.Parse(form.Get("take")));
                return Json(new
                {
                    total = result.Count > 0 ? result[0]["DATACOUNT"] : 0,
                    gridData = result.Select(p=> new {
                        USER_IP = p["USER_IP"],
                        USER_ID = p["USER_ID"],
                        REQUEST_URL = p["REQUEST_URL"],
                        LOG_DATE = DateTime.Parse(p["LOG_DATE"].ToString()).ToString("yyyy/MM/dd HH:mm:ss"),
                        COMMANDTEXT = p["COMMANDTEXT"],
                        PARAMETERS = p["PARAMETERS"]
                    })
                });
            }
        }
    }
}