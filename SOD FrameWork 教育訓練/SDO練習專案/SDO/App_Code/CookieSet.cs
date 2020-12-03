using System;
using System.Web;

namespace SDO
{
    public class CookieSet
    {
        /// <summary>
        /// Set cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static HttpCookie SetCookie(string key, string value)
        {
            HttpContext.Current.Response.AppendCookie(new HttpCookie(key)
            {
                Value = value,
                HttpOnly = true
            });
            return HttpContext.Current.Response.Cookies[key];
        }

        /// <summary>
        /// Get cookie
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetCookie(string key)
        {
            return HttpContext.Current.Request.Cookies[key] == null ? "" : HttpContext.Current.Request.Cookies[key].Value;
        }
    }
}