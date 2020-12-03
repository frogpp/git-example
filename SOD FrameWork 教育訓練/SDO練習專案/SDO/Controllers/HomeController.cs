using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SDO.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class HomeController : _Controller
    {
        /// <summary>
        /// Page of System hoe index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            LogSet.LogInfo("登入");
            return View();
        }

        /// <summary>
        /// Page of system infomation
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Partical for TWDatetime
        /// </summary>
        /// <returns></returns>
        public ActionResult _TWDatetime(string ID, bool DISPLAYTIME, string CASCADEFROM = "")
        {
            ViewData["ID"] = ID;
            ViewData["DISPLAYTIME"] = DISPLAYTIME;
            ViewData["CASCADEFROM"] = CASCADEFROM;
            return PartialView();
        }

        /// <summary>
        /// Partical for OrgSelector
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="PARENT_NODEID"></param>
        /// <param name="MULTISELECT"></param>
        /// <returns></returns>
        public ActionResult _OrgSelector(string ID, string PARENT_NODEID, bool MULTISELECT)
        {
            ViewData["ID"] = ID;
            ViewData["PARENT_NODEID"] = PARENT_NODEID;
            ViewData["MULTISELECT"] = MULTISELECT;
            return PartialView();
        }

        /// <summary>
        /// Partical for OrgUserSelector
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult _OrgUserSelector(string ID)
        {
            ViewData["ID"] = ID;
            return PartialView();
        }

        /// <summary>
        /// Partical for FileUploader
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="FILES"></param>
        /// <returns></returns>
        public ActionResult _FileUploader(string ID, bool MULTIPLE, string FILES="")
        {
            ViewData["ID"] = ID;
            ViewData["MULTIPLE"] = MULTIPLE;
            ViewData["FILES"] = FILES;
            return PartialView();
        }

        /// <summary>
        /// Partical for Checkbox List
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="VALUE"></param>
        /// <returns></returns>
        public ActionResult _CheckboxList(string ID, string VALUE)
        {
            ViewData["ID"] = ID;
            ViewData["VALUE"] = JsonConvert.DeserializeObject<IList<SelectListItem>>(VALUE);
            return PartialView();
        }

        /// <summary>
        /// Partical for Radio List
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="VALUE"></param>
        /// <returns></returns>
        public ActionResult _RadioList(string ID, string VALUE)
        {
            ViewData["ID"] = ID;
            ViewData["VALUE"] = JsonConvert.DeserializeObject<IList<SelectListItem>>(VALUE);
            return PartialView();
        }
    }
}