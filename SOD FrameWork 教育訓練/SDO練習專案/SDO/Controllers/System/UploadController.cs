using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SDO.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class UploadController : _Controller
    {
        /// <summary>
        /// 
        /// </summary>
        [DataContract]
        protected class ChunkMetaData
        {
            /// <summary>
            /// 
            /// </summary>
            [DataMember(Name = "uploadUid")]
            public string UploadUid { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [DataMember(Name = "fileName")]
            public string FileName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [DataMember(Name = "contentType")]
            public string ContentType { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [DataMember(Name = "chunkIndex")]
            public long ChunkIndex { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [DataMember(Name = "totalChunks")]
            public long TotalChunks { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [DataMember(Name = "totalFileSize")]
            public long TotalFileSize { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected string ValidFilePath(string fileName)
        {
            return fileName.Replace(@"../", "").Replace(@"..\", "").Replace(@"..", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ChunkSave(IEnumerable<HttpPostedFileBase> uploader, string metaData)
        {
            if (metaData == null)
            {
                return Save(uploader);
            }
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ChunkMetaData));
            ChunkMetaData fileMeta = serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(metaData))) as ChunkMetaData;
            if (uploader != null)
            {
                foreach (var file in uploader)
                {

                    string tmpFilePath = Path.Combine(Server.MapPath(SysSet.GetSysParam("SystemConfig", "UploadFolder")), string.Format("uploader_{0}_{1}", GetLoginUser().USER_ID, Path.GetFileName(ValidFilePath(fileMeta.FileName))));
                    if (fileMeta.ChunkIndex == 0 && System.IO.File.Exists(tmpFilePath))
                    {
                        System.IO.File.Delete(tmpFilePath);
                    }
                    AppendToBlob(tmpFilePath, file.InputStream);
                }
            }
            return Json(new
            {
                uploaded = fileMeta.TotalChunks - 1 <= fileMeta.ChunkIndex,
                fileUid = fileMeta.UploadUid
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tmpFilePath"></param>
        /// <param name="content"></param>
        private void AppendToBlob(string tmpFilePath, Stream content)
        {
            using (FileStream stream = new FileStream(tmpFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (content)
                {
                    content.CopyTo(stream);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private JsonResult Save(IEnumerable<HttpPostedFileBase> uploader)
        {
            if (uploader != null)
            {
                foreach (var file in uploader)
                {
                    file.SaveAs(Path.Combine(Server.MapPath(SysSet.GetSysParam("SystemConfig", "UploadFolder")), Path.GetFileName(ValidFilePath(file.FileName))));
                }
            }
            return Json("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(string[] fileNames)
        {
            if (fileNames != null)
            {
                foreach (string fileName in fileNames)
                {
                    string tmpFilePath = Path.Combine(Server.MapPath(SysSet.GetSysParam("SystemConfig", "UploadFolder")), string.Format("uploader_{0}_{1}", GetLoginUser().USER_ID, Path.GetFileName(ValidFilePath(fileName))));
                    if (System.IO.File.Exists(tmpFilePath))
                    {
                        System.IO.File.Delete(tmpFilePath);
                    }
                }
            }
            return Content("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveUploads(string[] fileNames, string savePath = "")
        {
            ArrayList saveFiles = new ArrayList();
            if (fileNames != null)
            {
                savePath = string.IsNullOrWhiteSpace(savePath) ? Path.Combine(Server.MapPath(SysSet.GetSysParam("SystemConfig", "UploadFolder")), GetLoginUser().USER_ID) : ValidFilePath(savePath);
                Directory.CreateDirectory(savePath);
                foreach (string fileName in fileNames)
                {
                    string validFileName = ValidFilePath(fileName);
                    string tmpFilePath = Path.Combine(Server.MapPath(SysSet.GetSysParam("SystemConfig", "UploadFolder")), string.Format("uploader_{0}_{1}", GetLoginUser().USER_ID, Path.GetFileName(validFileName)));
                    if (System.IO.File.Exists(tmpFilePath))
                    {
                        string saveFileName = string.Format("{0}_{1}", DateTime.Now.ToFileTimeUtc(), Path.GetFileName(validFileName));
                        System.IO.File.Move(tmpFilePath, Path.Combine(savePath, saveFileName));
                        saveFiles.Add(HttpUtility.HtmlEncode(Path.Combine(GetLoginUser().USER_ID, saveFileName)));
                    }
                    else
                    {
                        FileInfo[] existFiles = new DirectoryInfo(savePath).GetFiles(Path.GetFileName(validFileName));
                        if (existFiles.Count() == 1)
                        {
                            saveFiles.Add(HttpUtility.HtmlEncode(Path.Combine(GetLoginUser().USER_ID, existFiles.First().Name)));
                        }
                    }
                }
            }
            foreach (FileInfo tmpFile in new DirectoryInfo(Server.MapPath(SysSet.GetSysParam("SystemConfig", "UploadFolder"))).GetFiles().Where(p => p.CreationTime < DateTime.Now.AddDays(-1)))
            {
                tmpFile.Delete();
            }
            return Json(saveFiles.ToArray(typeof(string)));
        }

        /// <summary>
        /// Get File Info List
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult GetUploads(string[] fileNames, string savePath = "")
        {
            ArrayList files = new ArrayList();
            savePath = string.IsNullOrWhiteSpace(savePath) ? Server.MapPath(SysSet.GetSysParam("SystemConfig", "UploadFolder")) : ValidFilePath(savePath);
            if (fileNames != null)
            {
                foreach (string attFile in fileNames)
                {
                    string validFileName = Path.Combine(savePath, ValidFilePath(attFile));
                    if (System.IO.File.Exists(validFileName))
                    {
                        byte[] attachBytes = System.IO.File.ReadAllBytes(validFileName);
                        files.Add(new
                        {
                            name = Path.GetFileName(validFileName),
                            size = attachBytes.Length,
                            extension = Path.GetExtension(validFileName)
                        });
                    }
                }
            }
            return Json(files);
        }
    }
}