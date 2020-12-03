using SDO.Dacs;
using SDO.Models;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Web;

namespace SDO.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class ActiveFlowController : _Controller
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
        /// get flow in active
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ContentResult Query(FormCollection form)
        {
            return Content(JsonConvert.SerializeObject(string.IsNullOrWhiteSpace(form.Get("qryFLOW_CODE")) ? new FlowSet.Log().GetFlowList() : new FlowSet.Log().GetFlowDetail(HttpUtility.HtmlEncode(form.Get("qryFLOW_CODE")), true)), "application/json");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryDetail(string ID)
        {
            ViewBag.isAdmin = (Url.Action("Query", "ActiveFlow") == Request.UrlReferrer.AbsolutePath) && GetLoginUser().GetEmpRoles().Count(p => p.ROLE_ID == "FlowSysRole") > 0;
            ViewBag.FLOW_CODE = ID;
            return View();
        }

        /// <summary>
        /// get flow execute log
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ContentResult QueryDetail(FormCollection form)
        {
            return Content(JsonConvert.SerializeObject(new FlowSet.Log().GetFlowDetail(HttpUtility.HtmlEncode(form.Get("FLOW_CODE")))), "application/json");
        }

        /// <summary>
        /// get subflow execute log
        /// </summary>
        /// <param name="subflowCode"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult QuerySubDetail(string flowCode, int signedOdr)
        {
            ArrayList subflowDetail = new ArrayList();
            foreach (string subflowCode in new FlowSet.Log().GetSubflowCodeList(HttpUtility.HtmlEncode(flowCode), signedOdr))
            {
                subflowDetail.Add(JsonConvert.SerializeObject(new FlowSet.Log().GetSubFlowDetail(subflowCode)));
            }
            return Json(subflowDetail);
        }

        /// <summary>
        /// set flow deactive
        /// </summary>
        /// <param name="flowCode"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CancelFlow(string flowCode)
        {
            return Json(new FlowSet.Flow().SetDeactive(HttpUtility.HtmlEncode(flowCode), GetLoginUser().USER_ID, "系統管理員停止流程"));
        }

        /// <summary>
        /// reset flow
        /// </summary>
        /// <param name="flowCode"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetFlow(string flowCode)
        {
            return Json(new FlowSet.Flow().SetReject(HttpUtility.HtmlEncode(flowCode), GetLoginUser().USER_ID, "系統管理員重啟流程", true));
        }

        /// <summary>
        /// set subflow deactive
        /// </summary>
        /// <param name="subflowCode"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CancelSubflow(string subflowCode)
        {
            return Json(new FlowSet.Flow().SetSubDeactive(HttpUtility.HtmlEncode(subflowCode), GetLoginUser().USER_ID, "系統管理員停止分會流程"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowCode"></param>
        /// <param name="isSubflow"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ContentResult QueryFlowLvInfo(FormCollection form)
        {
            return Content(JsonConvert.SerializeObject(bool.Parse(form.Get("IS_SUBFLOW")) ? new FlowSet.Log().GetSubFlowDetail(HttpUtility.HtmlEncode(form.Get("FLOW_CODE")), true) : new FlowSet.Log().GetFlowDetail(HttpUtility.HtmlEncode(form.Get("FLOW_CODE")), true)), "application/json");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowCode"></param>
        /// <param name="memo"></param>
        /// <param name="signedDecision"></param>
        /// <param name="isSubflow"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetFlowAccept(FormCollection form)
        {
            return Json(bool.Parse(form.Get("IS_SUBFLOW")) ? new FlowSet.Flow().SetSubAccept(HttpUtility.HtmlEncode(form.Get("FLOW_CODE")), GetLoginUser().SEND_USER_ID, HttpUtility.HtmlEncode(form.Get("MEMO"))) : new FlowSet.Flow().SetAccept(HttpUtility.HtmlEncode(form.Get("FLOW_CODE")), GetLoginUser().SEND_USER_ID, HttpUtility.HtmlEncode(form.Get("MEMO")), bool.Parse(form.Get("SIGNED_DECISION") ?? "false")));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowCode"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetFlowReject(FormCollection form)
        {
            return Json(new FlowSet.Flow().SetReject(HttpUtility.HtmlEncode(form.Get("FLOW_CODE")), GetLoginUser().SEND_USER_ID, HttpUtility.HtmlEncode(form.Get("MEMO"))));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetSubFlowActive(FormCollection form)
        {
            ArrayList result = new ArrayList();
            foreach (string subUser in form.Get("SUB_USERS").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                result.Add(new FlowSet.Flow().SetSubActive("flowSubTest", GetLoginUser().SEND_USER_ID, HttpUtility.HtmlEncode(subUser), HttpUtility.HtmlEncode(form.Get("FLOW_CODE"))));
            }
            return Json(new RtnResultModel(true, string.Format(i18N.Flow.F07, HttpUtility.HtmlEncode(form.Get("FLOW_CODE"))), result));
        }
    }
}