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
    /// 郵件角色設定控制器
    /// </summary>
    /// <remarks>
    ///     Created by Steven Tsai at 2017/07/10
    /// </remarks>
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class MailRoleController : _Controller
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
            using (MailRoleDac dac = new MailRoleDac(GetLoginUser()))
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
            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">郵件角色資料編輯模型</param>
        /// <returns>新增結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Create(MailRoleModel.EditView model)
        {
            using (MailRoleDac dac = new MailRoleDac(GetLoginUser()))
            {
                return Json(dac.Create(model));
            }
        }

        /// <summary>
        /// 顯示修改畫面
        /// </summary>
        /// <param name="id">角色代碼</param>
        /// <returns>修改畫面</returns>
        public ActionResult Update(string id)
        {
            // 如果沒帶入角色代碼, 即跳轉至新增畫面
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Create", "MailRole");
            }

            using (MailRoleDac dac = new MailRoleDac(GetLoginUser()))
            {
                // 查詢指定角色代碼資料
                MailRoleModel.DTO model = dac.Read(id);

                // 如果角色不存在或已刪除, 一併跳轉至新增畫面
                if (model == default(MailRoleModel.DTO) || model.DEL_FLG)
                {
                    return RedirectToAction("Create", "MailRole");
                }

                // 將 DTO 模型轉成具有行為的 View Model, 並取得擁有指定身分的使用者清單
                MailRoleModel.EditView viewModel = model.ToEditView();
                viewModel.DEFAULT_MAP_USER = dac.ReadUsers(model.ROLE_ID);

                return View(viewModel);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">流程角色資料編輯模型</param>
        /// <returns>修改結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Update(MailRoleModel.EditView model)
        {
            using (MailRoleDac dac = new MailRoleDac(GetLoginUser()))
            {
                return Json(dac.Update(model));
            }
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="roleID">角色代碼</param>
        /// <returns>刪除結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Delete(string roleID)
        {
            using (MailRoleDac dac = new MailRoleDac(GetLoginUser()))
            {
                return Json(dac.Delete(roleID));
            }
        }
    }
}