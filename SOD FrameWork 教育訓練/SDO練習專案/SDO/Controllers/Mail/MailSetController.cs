using SDO.Dacs;
using SDO.Models;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;

namespace SDO.Controllers
{
    /// <summary>
    /// 郵件範本設定控制器
    /// </summary>
    /// <remarks>
    ///     Created by Steven Tsai at 2017/07/10
    /// </remarks>
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class MailSetController : _Controller
    {
        /// <summary>
        /// 顯示功能主畫面
        /// </summary>
        /// <returns>功能主畫面</returns>
        public ActionResult Query()
        {
            return View();
        }

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="form">查詢表單資訊</param>
        /// <returns>查詢結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Query(FormCollection form)
        {
            using (MailSetDac dac = new MailSetDac(GetLoginUser()))
            {
                return Json(dac.Read());
            }
        }

        /// <summary>
        /// 顯示新增畫面
        /// </summary>
        /// <returns>新增畫面</returns>
        public ActionResult Create()
        {
            // 新增時, 必要使用到的選單初始化
            __InitSelectList();

            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">郵件範本設定資料編輯模型</param>
        /// <returns>新增結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Create(MailSetModel.EditView model)
        {
            using (MailSetDac dac = new MailSetDac(GetLoginUser()))
            {
                return Json(dac.Create(model));
            }
        }

        /// <summary>
        /// 顯示修改畫面
        /// </summary>
        /// <param name="id">郵件範本代碼</param>
        /// <returns>修改畫面</returns>
        public ActionResult Update(string id)
        {
            // 如果沒帶入郵件範本代碼, 即跳轉至新增畫面
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Create", "MailSet");
            }

            // 修改時, 必要使用到的選單初始化
            __InitSelectList();

            using (MailSetDac dac = new MailSetDac(GetLoginUser()))
            {
                // 郵件範本設定資料
                MailSetModel.EditView model = dac.Read(id).ToEditView();

                // 郵件範本設定明細資料（郵件設定）
                foreach (MailSetDetailModel.DTO mailSetting in dac.ReadDetail(model.MAIL_ID))
                {
                    model.MAIL_SETTING.Add(mailSetting.ToEditView());
                }

                return View(model);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">郵件範本設定資料編輯模型</param>
        /// <returns>修改結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Update(MailSetModel.EditView model)
        {
            using (MailSetDac dac = new MailSetDac(GetLoginUser()))
            {
                return Json(dac.Update(model));
            }
        }

        /// <summary>
        /// 初始化畫面必要選單
        /// </summary>
        /// <remarks>新增、修改時共同會使用到的下拉式選單資料</remarks>
        private void __InitSelectList()
        {
            using (SetParamDac dac = new SetParamDac(GetLoginUser()))
            {
                // 類型清單
                ViewBag.mailTypeList = new SelectList(dac.Read("MailType"), "SET_TYPE", "SET_VALUE");
            }

            using (MailRoleDac dac = new MailRoleDac(GetLoginUser()))
            {
                // 角色資料清單
                ViewBag.roleList = new SelectList(dac.Read(), "ROLE_ID", "ROLE_NAME");
            }
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="mailID">郵件範本代碼</param>
        /// <returns>刪除結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Delete(string mailID)
        {
            using (MailSetDac dac = new MailSetDac(GetLoginUser()))
            {
                return Json(dac.Delete(mailID));
            }
        }
    }
}