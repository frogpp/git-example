using SDO.Dacs;
using SDO.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using System.Collections;
using Ionic.Zip;
using System.Text;

namespace SDO.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class AnnouncementController : _Controller
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
        public JsonResult Query(FormCollection form)
        {
            using (AnnouncementDac dac = new AnnouncementDac(GetLoginUser()))
            {
                return Json(dac.Read());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult QueryDisplay()
        {
            using (AnnouncementDac dac = new AnnouncementDac(GetLoginUser()))
            {
                return Json(dac.ReadDisplay());
            }
        }

        /// <summary>
        /// Create page
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create by POST
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(FormCollection form)
        {
            using (AnnouncementDac dac = new AnnouncementDac(GetLoginUser()))
            {
                return Json(dac.Create(form));
            }
        }

        /// <summary>
        /// Update page
        /// </summary>
        /// <returns></returns>
        public ActionResult Update(int id)
        {
            using (AnnouncementDac dac = new AnnouncementDac(GetLoginUser()))
            {
                AnnouncementModel model = dac.Read(id);
                return View(model);
            }
        }

        /// <summary>
        /// Update by POST
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update(FormCollection form)
        {
            using (AnnouncementDac dac = new AnnouncementDac(GetLoginUser()))
            {
                return Json(dac.Update(form));
            }
        }

        /// <summary>
        /// Delete by POST
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(FormCollection form)
        {
            using (AnnouncementDac dac = new AnnouncementDac(GetLoginUser()))
            {
                return Json(dac.Delete(form));
            }
        }

        /// <summary>
        /// Download Attachment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public FileResult GetAttachment(int id)
        {
            using (AnnouncementDac dac = new AnnouncementDac(GetLoginUser()))
            {
                AnnouncementModel model = dac.Read(id);
                MemoryStream ms = new MemoryStream();
                if (model != default(AnnouncementModel) && model.ATTACH_NAME.Trim().Length > 0)
                {
                    using (ZipFile zip = new ZipFile(Encoding.UTF8))
                    {
                        string savePath = Server.MapPath(SysSet.GetSysParam("SystemConfig", "UploadFolder"));
                        foreach (string att in model.ATTACH_NAME.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            FileInfo attInfo = new FileInfo(Path.Combine(savePath, att));
                            if (attInfo.Exists)
                            {
                                zip.AddEntry(attInfo.Name.Substring(attInfo.Name.IndexOf("_") + 1), System.IO.File.ReadAllBytes(attInfo.FullName));
                            }
                        }
                        if (zip.Count == 0)
                        {
                            zip.AddEntry(i18N.Message.R09, "");
                        }
                        zip.Save(ms);
                    }
                }
                return File(ms.ToArray(), "application/octet-stream", string.Format("{0}.zip", model.TITLE));
            }
        }
    }
}