using SDO.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDO.Dacs;
using System;

namespace SDO
{
    public class SysSet
    {
        /// <summary>
        /// Get system paramater
        /// </summary>
        /// <param name="strItem"></param>
        /// <param name="strType"></param>
        /// <returns></returns>
        public static string GetSysParam(string strItem, string strType)
        {
            try
            {
                if (HttpContext.Current.Session["sysParams"] == null || string.IsNullOrWhiteSpace(HttpContext.Current.Session["sysParams"].ToString()))
                {
                    using (SetParamDac dac = new SetParamDac())
                    {
                        HttpContext.Current.Session["sysParams"] = JsonConvert.SerializeObject(dac.Read());
                    }
                }
                return JsonConvert.DeserializeObject<IList<SetParamModel>>(HttpContext.Current.Session["sysParams"].ToString()).Where(p => p.SET_ITEM == HttpUtility.HtmlEncode(strItem) && p.SET_TYPE == HttpUtility.HtmlEncode(strType)).Select(p => p.SET_VALUE).SingleOrDefault();
            }
            catch (Exception ex)
            {
                LogSet.LogError(ex.ToString());
                return i18N.Message.R03;
            }
        }

        /// <summary>
        ///  Get system paramaters
        /// </summary>
        /// <param name="strItem"></param>
        /// <returns></returns>
        public static IList<SetParamModel> GetSysParams(string strItem)
        {
            try
            {
                if (HttpContext.Current.Session["sysParams"] != null && !string.IsNullOrWhiteSpace(HttpContext.Current.Session["sysParams"].ToString()))
                {
                    return JsonConvert.DeserializeObject<IList<SetParamModel>>(HttpContext.Current.Session["sysParams"].ToString()).Where(p => p.SET_ITEM == strItem).ToList();
                }
                else
                {
                    using (SetParamDac dac = new SetParamDac())
                    {
                        IList<SetParamModel> result = dac.Read();
                        HttpContext.Current.Session["sysParams"] = JsonConvert.SerializeObject(result);
                        return result.Where(p => p.SET_ITEM == strItem).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSet.LogError(ex.ToString());
                return new List<SetParamModel>();
            }
        }
    }
}