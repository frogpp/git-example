using SDO.Dacs;
using SDO.Models;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;

namespace SDO.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class DemoController : _Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SampleCSS()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SampleJS()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SampleUsercontrol()
        {
            using (SetFunctionDac dac = new SetFunctionDac(GetLoginUser()))
            {
                ViewBag.demoListAll = dac.Read().Where(p => !string.IsNullOrWhiteSpace(p.PARENT_ID)).OrderBy(p => p.PARENT_ID + p.SORT_ID).Select(p => new SelectListItem()
                {
                    Value = p.FUNCTION_ID,
                    Text = p.FUNCTION_DISPLAY
                }).ToList();
                ViewBag.demoListSel = dac.ReadByRight("AgentRight").OrderBy(p => p.PARENT_ID + p.SORT_ID).Select(p => new SelectListItem()
                {
                    Value = p.FUNCTION_ID,
                    Text = p.FUNCTION_DISPLAY
                }).ToList();
                ViewBag.demoList = dac.Read().OrderBy(p => p.FUNCTION_ID).ThenBy(p => p.SORT_ID).Select(p => new SelectListItem()
                {
                    Value = string.IsNullOrWhiteSpace(p.PARENT_ID) ? "_GROUP" : p.FUNCTION_ID,
                    Text = p.FUNCTION_DISPLAY,
                }).ToList();
            }
            using (EmpOrgDac dac=new EmpOrgDac(GetLoginUser()))
            {
                ViewBag.demoOrg = dac.ReadByOrg("LSBU3").Select(p => new SelectListItem()
                {
                    Value = p.ORG_ID,
                    Text = p.ORG_DISPLAY,
                }).ToList();
            }
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SampleGrid()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SampleDAC()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SampleAppCode()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SampleFlowApi()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SampleApi()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SampleEform()
        {
            return View();
        }
    }
}