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
    /// <summary>
    /// 流程設定控制器
    /// </summary>
    /// <remarks>
    ///     Created by Steven Tsai at 2017/06/15
    /// </remarks>
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class FlowSetController : _Controller
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
            using (FlowSetDac dac = new FlowSetDac(GetLoginUser()))
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

            using (FlowSetDac dac = new FlowSetDac(GetLoginUser()))
            {
                // 流程設定資料清單（必要使用到的選單之一）
                ViewBag.flowSetList = new SelectList(dac.Read().Where(m => m.ENABLE_FLG).ToList(), "FLOW_ID", "FLOW_NAME");
            }

            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">流程設定資料編輯模型</param>
        /// <returns>新增結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Create(FlowSetModel.EditView model)
        {
            using (FlowSetDac dac = new FlowSetDac(GetLoginUser()))
            {
                return Json(dac.Create(model));
            }
        }

        /// <summary>
        /// 顯示修改畫面
        /// </summary>
        /// <param name="id">流程代碼</param>
        /// <returns>修改畫面</returns>
        public ActionResult Update(string id)
        {
            // 如果沒帶入流程代碼, 即跳轉至新增畫面
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Create", "FlowSet");
            }

            // 修改時, 必要使用到的選單初始化
            __InitSelectList();

            using (FlowSetDac dac = new FlowSetDac(GetLoginUser()))
            {
                // 流程設定資料清單（必要使用到的選單之一）
                ViewBag.flowSetList = new SelectList(dac.Read().Where(m => m.ENABLE_FLG).ToList(), "FLOW_ID", "FLOW_NAME");

                // 流程設定資料
                FlowSetModel.EditView model = dac.Read(id).ToEditView();

                // 流程設定明細資料（關卡設定資訊）
                foreach (FlowSetDetailModel.DTO stage in dac.ReadDetail(model.FLOW_ID))
                {
                    model.SET_STAGE.Add(stage.ToEditView());
                }

                return View(model);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">流程設定資料編輯模型</param>
        /// <returns>修改結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Update(FlowSetModel.EditView model)
        {
            using (FlowSetDac dac = new FlowSetDac(GetLoginUser()))
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
            using (EmpOrgDac dac = new EmpOrgDac(GetLoginUser()))
            {
                // 單位資料清單
                ViewBag.orgList = new SelectList(dac.Read(), "ORG_ID", "ORG_NAME");
            }

            using (FlowRoleDac dac = new FlowRoleDac(GetLoginUser()))
            {
                // 角色資料清單
                ViewBag.roleList = new SelectList(dac.Read(), "ROLE_ID", "ROLE_NAME");
            }

            using (EmpUserDac dac = new EmpUserDac(GetLoginUser()))
            {
                // 使用者資料清單
                ViewBag.userList = new SelectList(dac.Read(), "USER_ID", "USER_NAME");
            }
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="flowID">流程代碼</param>
        /// <returns>刪除結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Delete(string flowID)
        {
            using (FlowSetDac dac = new FlowSetDac(GetLoginUser()))
            {
                return Json(dac.Delete(flowID));
            }
        }

    }
}