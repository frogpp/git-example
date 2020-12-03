using SDO.Dacs;
using SDO.Models;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Security.Application;

namespace SDO.Controllers
{

    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class MailLogController : _Controller
    {
        /// <summary>
        /// 
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
            using (MailLogDac dac = new MailLogDac(GetLoginUser()))
            {
                var result = dac.Read(DateTime.Parse(form.Get("log_date")), int.Parse(form.Get("skip")), int.Parse(form.Get("take")));
                return Json(new
                {
                    total = result.Count > 0 ? result[0]["DATACOUNT"] : 0,
                    gridData = result.Select(p => new
                    {
                        MAIL_SENDER = p["MAIL_SENDER"],
                        MAIL_RECEIVER = p["MAIL_RECEIVER"],
                        CC_RECEIVER = p["CC_RECEIVER"],
                        BCC_RECEIVER = p["BCC_RECEIVER"],
                        MAIL_SUBJECT = p["MAIL_SUBJECT"],
                        MAIL_CONTENT = p["MAIL_CONTENT"].ToString(),
                        SEND_FLG = p["SEND_FLG"],
                        LOG_DATE = string.Format("{0}/{1}", DateTime.Parse(p["CRT_DATE"].ToString()).Year - 1911, DateTime.Parse(p["CRT_DATE"].ToString()).ToString("MM/dd HH:mm"))
                    }).ToArray()
                });
            }
        }
    }



}