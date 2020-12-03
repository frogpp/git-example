using SDO.Dacs;
using SDO.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Web;

namespace SDO.Controllers
{
    [AllowAnonymous]
    public class SystemController : _Controller
    {
        /// <summary>
        /// Login page
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(FormsAuthentication.DefaultUrl);
            }
            return View();
        }

        /// <summary>
        /// Login by POST
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0)]
        public JsonResult Login(FormCollection form)
        {
            RtnResultModel loginResult = new RtnResultModel(false, i18N.Message.R02);
            using (EmpUserDac dac = new EmpUserDac(GetLoginUser()))
            {
                EmpUserModel objUser = dac.Read(form.Get("LOGIN_ID"));
                if (objUser != default(EmpUserModel))
                {
                    switch (int.Parse(SysSet.GetSysParam("SystemConfig", "AuthType")))
                    {
                        case 1:
                            EmpUserModel logonUser = dac.Read(form.Get("LOGIN_ID"), form.Get("LOGIN_PWD"));
                            if (logonUser != default(EmpUserModel) && objUser.USER_ID == logonUser.USER_ID)
                            {
                                SetLoginUser(objUser);
                                loginResult = new RtnResultModel(true, "", FormsAuthentication.GetRedirectUrl(objUser.USER_ID, false));
                            }
                            break;
                        case 2:
                            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, SysSet.GetSysParam("SystemConfig", "ADServer")))
                            {
                                if (pc.ValidateCredentials(form.Get("LOGIN_ID"), form.Get("LOGIN_PWD")))
                                {
                                    SetLoginUser(objUser);
                                    loginResult = new RtnResultModel(true, "", FormsAuthentication.GetRedirectUrl(objUser.USER_ID, false));
                                }
                            }
                            break;
                        case 3:
                            SetLoginUser(objUser);
                            loginResult = new RtnResultModel(true, "", FormsAuthentication.GetRedirectUrl(objUser.USER_ID, false));
                            break;
                        default:
                            break;
                    }
                }
            }
            return Json(loginResult);
        }

        /// <summary>
        /// Set login user info to session
        /// </summary>
        /// <param name="objUser"></param>
        private void SetLoginUser(EmpUserModel objUser)
        {
            objUser.USER_IP = Request.UserHostAddress;
            Session["loginUser"] = objUser;
            //FormsAuthentication.SetAuthCookie(GetLoginUser().USER_ID, true);
            Response.AppendCookie(new HttpCookie(FormsAuthentication.FormsCookieName)
            {
                Value = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(1, GetLoginUser().USER_ID, DateTime.Now, DateTime.Now.AddMinutes(30), false, "", FormsAuthentication.FormsCookiePath))
            });
        }

        /// <summary>
        /// Logout system
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Logout()
        {
            LogSet.LogInfo("登出");
            EmpUserModel liveUser = GetLoginUser();
            Session.Clear();
            foreach (string cookieName in Response.Cookies)
            {
                Response.Cookies[cookieName].Expires = DateTime.Now.AddDays(-1);
            }
            if (!string.IsNullOrWhiteSpace(liveUser.AGENT_ID))
            {
                using (EmpUserDac dac = new EmpUserDac(liveUser))
                {
                    EmpUserModel objUser = dac.Read(liveUser.AGENT_ID);
                    if (objUser != default(EmpUserModel))
                    {
                        SetLoginUser(objUser);
                        FormsAuthentication.RedirectFromLoginPage(objUser.USER_ID, false);
                    }
                }
            }
            else
            {
                Session.Abandon();
                FormsAuthentication.SignOut();
            }
            return Redirect(FormsAuthentication.LoginUrl);
        }

        /// <summary>
        /// Redirect to agent page
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult SetAgent(FormCollection form)
        {
            EmpUserModel liveUser = GetLoginUser();
            using (EmpUserDac dac = new EmpUserDac(liveUser))
            {
                EmpUserModel objUser = dac.Read(form.Get("USER_ID"));
                Session.Clear();
                Session.Abandon();
                Response.Cookies.Clear();
                if (objUser != default(EmpUserModel))
                {
                    objUser.AGENT_ID = liveUser.USER_ID;
                    objUser.USER_NAME = string.Format("{0}({1})", objUser.USER_NAME, liveUser.USER_NAME);
                    SetLoginUser(objUser);
                    FormsAuthentication.RedirectFromLoginPage(objUser.USER_ID, false);
                }
            }
            return Redirect(FormsAuthentication.DefaultUrl);
        }

        /// <summary>
        /// keep session alive
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0)]
        public ContentResult KeepSession()
        {
            Response.AppendCookie(new HttpCookie(FormsAuthentication.FormsCookieName)
            {
                Value = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(1, GetLoginUser().USER_ID, DateTime.Now, DateTime.Now.AddMinutes(30), false, "", FormsAuthentication.FormsCookiePath))
            });
            return Content(DateTime.Now.Ticks.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ContentResult Cookie(string key, string value = "", DateTime? expires = null)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    var valObj = JsonConvert.DeserializeObject(value);
                    value = JsonConvert.SerializeObject(valObj);
                }
                catch
                {
                    value = HttpUtility.HtmlEncode(value);
                }
                HttpCookie cookie = CookieSet.SetCookie(HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));
                if (expires != null)
                {
                    cookie.Expires = (DateTime)expires;
                }
            }
            return Content(HttpUtility.UrlDecode(CookieSet.GetCookie(HttpUtility.UrlEncode(key))));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ContentResult RemoveCookie(string key)
        {
            foreach (string cookieName in Request.Cookies.Cast<string>().ToArray())
            {
                if (cookieName.StartsWith(HttpUtility.UrlEncode(key)))
                {
                    Response.Cookies[cookieName].Expires = DateTime.Now.AddDays(-1);
                }
            }
            return Content(Response.Cookies.Count.ToString());
        }
    }
}