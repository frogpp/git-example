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
    /// 表單設定控制器
    /// </summary>
    /// <remarks>
    ///     Created by Steven Tsai at 2017/06/15
    /// </remarks>
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [Authorize]
    public class eFormSetController : _Controller
    {
        /// <summary>
        /// 顯示功能主畫面
        /// </summary>
        /// <returns>功能主畫面</returns>
        public ActionResult Query()
        {
            // 表單類型資料清單
            ViewBag.eFormTypeList = new SelectList(SysSet.GetSysParams("eFormType"), "SET_TYPE", "SET_VALUE");
            return View();
        }

        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="model">查詢條件</param>
        /// <returns>查詢結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Query(eFormSetModel.QueryView model)
        {
            using (eFormSetDac dac = new eFormSetDac(GetLoginUser()))
            {
                return Json(dac.Read(model));
            }
        }

        /// <summary>
        /// 顯示新增畫面
        /// </summary>
        /// <returns>新增畫面</returns>
        public ActionResult Create()
        {
            // 表單類型資料清單
            ViewBag.eFormTypeList = new SelectList(SysSet.GetSysParams("eFormType"), "SET_TYPE", "SET_VALUE");

            using (FlowSetDac dac = new FlowSetDac(GetLoginUser()))
            {
                // 流程設定資料清單
                ViewBag.flowSetList = new SelectList(dac.Read().Where(m => m.ENABLE_FLG).ToList(), "FLOW_ID", "FLOW_NAME");
            }

            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">表單設定資料編輯模型</param>
        /// <returns>新增結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Create(eFormSetModel.EditView model)
        {
            using (eFormSetDac dac = new eFormSetDac(GetLoginUser()))
            {
                return Json(dac.Create(model));
            }
        }

        /// <summary>
        /// 顯示修改畫面
        /// </summary>
        /// <param name="id">表單代碼</param>
        /// <returns>修改畫面</returns>
        public ActionResult Update(string id)
        {
            // 如果沒帶入表單代碼, 即跳轉至新增畫面
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Create", "eFormSet");
            }

            // 表單類型資料清單
            ViewBag.eFormTypeList = new SelectList(SysSet.GetSysParams("eFormType"), "SET_TYPE", "SET_VALUE");

            using (FlowSetDac dac = new FlowSetDac(GetLoginUser()))
            {
                // 流程設定資料清單
                ViewBag.flowSetList = new SelectList(dac.Read().Where(m => m.ENABLE_FLG).ToList(), "FLOW_ID", "FLOW_NAME");
            }

            using (eFormSetDac dac = new eFormSetDac(GetLoginUser()))
            {
                // 查詢指定角色代碼資料
                eFormSetModel.DTO model = dac.Read(id);

                // 如果角色不存在或已刪除, 一併跳轉至新增畫面
                if (model == default(eFormSetModel.DTO))
                {
                    return RedirectToAction("Create", "eFormSet");
                }

                // 將 DTO 模型轉成具有行為的 View Model, 並取得該表單的對應流程與使用單位
                eFormSetModel.EditView viewModel = model.ToEditView();
                viewModel.MAP_FLOW = dac.ReadMapFlows(model.FORM_ID);
                viewModel.DEFAULT_MAP_ORG = dac.ReadMapOrgs(model.FORM_ID);

                return View(viewModel);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">表單設定資料編輯模型</param>
        /// <returns>修改結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Update(eFormSetModel.EditView model)
        {
            using (eFormSetDac dac = new eFormSetDac(GetLoginUser()))
            {
                return Json(dac.Update(model));
            }
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="formID">表單代碼</param>
        /// <returns>刪除結果</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Delete(string formID)
        {
            using (eFormSetDac dac = new eFormSetDac(GetLoginUser()))
            {
                return Json(dac.Delete(formID));
            }
        }
    }
}