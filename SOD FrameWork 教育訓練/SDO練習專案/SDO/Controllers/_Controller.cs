using SDO.Models;
using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using SDO.Dacs;
using System.Net;
using System.Web.Helpers;
using System.IO;

namespace SDO.Controllers
{
    public class _Controller : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestContext"></param>
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ViewBag.GetCookie = new Func<string, string>(CookieSet.GetCookie);
            ViewBag.GetSysParam = new Func<string, string, string>(SysSet.GetSysParam);
            ViewBag.GetLoginUser = new Func<EmpUserModel>(GetLoginUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (User == null || (User.Identity.IsAuthenticated && Session["loginUser"] == null))
            {
                FormsAuthentication.SignOut();
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Logout", controller = "System" }));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            LogSet.LogError(filterContext.Exception.ToString());
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }
            if (IsAjax(filterContext))
            {
                filterContext.Result = new JsonResult()
                {
                    Data = new RtnResultModel(false, filterContext.Exception.Message),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
            }
            else
            {
                base.OnException(filterContext);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private bool IsAjax(ExceptionContext filterContext)
        {
            return filterContext.HttpContext.Request.Headers["X-Requested-With"].ToUpper() == "XMLHTTPREQUEST";
        }

        /// <summary>
        /// Get login user info from session
        /// </summary>
        /// <returns></returns>
        protected EmpUserModel GetLoginUser()
        {
            return Session == null || Session["loginUser"] == null ? new EmpUserModel() : (EmpUserModel)((EmpUserModel)Session["loginUser"]).Clone();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        protected DateTime ParseTWDate(DateTime dt)
        {
            return DateTime.Parse(string.Format("{0}/{1}", dt.AddYears(-1911).Year, dt.ToString("MM/dd HH:mm:ss")));
        }
    }
}