using System;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SDO
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// 
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //強制AntiForgery不檢查目前登入帳號
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            string myLang = CookieSet.GetCookie("i18N");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(string.IsNullOrWhiteSpace(myLang) ? "zh-TW" : myLang);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(string.IsNullOrWhiteSpace(myLang) ? "zh-TW" : myLang);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_EndRequest(Object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Application_PostAuthorizeRequest()
        {
            //web api can use session
            HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_Start(Object sender, EventArgs e)
        {
            LogSet.ResetLog(new DirectoryInfo(Server.MapPath("~/Logs")));
        }

        /// <summary>
        /// 
        /// </summary>
        protected void Application_End()
        {
        }
    }
}
