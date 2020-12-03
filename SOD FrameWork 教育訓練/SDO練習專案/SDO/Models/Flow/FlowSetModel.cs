using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace SDO.Models
{
    /// <summary>
    /// 流程設定資料模型
    /// </summary>
    public class FlowSetModel
    {
        /// <summary>
        /// 編輯用檢視物件
        /// </summary>
        public class EditView
        {
            /// <summary>
            /// 流程代碼
            /// </summary>
            [Key, Required]
            [Display(Name = "FlowSet_FLOW_ID", ResourceType = typeof(i18N.Label))]
            public string FLOW_ID { get; set; }

            /// <summary>
            /// 流程名稱
            /// </summary>
            [Required]
            [Display(Name = "FlowSet_FLOW_NAME", ResourceType = typeof(i18N.Label))]
            public string FLOW_NAME { get; set; }

            /// <summary>
            /// 流程說明
            /// </summary>
            [Display(Name = "FlowSet_MEMO", ResourceType = typeof(i18N.Label))]
            public string MEMO { get; set; }

            /// <summary>
            /// 有效期限
            /// </summary>
            [Required]
            [Display(Name = "FlowSet_EFFECTIVE_DATE", ResourceType = typeof(i18N.Label))]
            public DateTime EFFECTIVE_DATE { get; set; }

            /// <summary>
            /// 到期日期
            /// </summary>
            [Required]
            [Display(Name = "FlowSet_EXPIRE_DATE", ResourceType = typeof(i18N.Label))]
            public DateTime EXPIRE_DATE { get; set; }

            /// <summary>
            /// 關卡設定
            /// </summary>
            public List<FlowSetDetailModel.EditView> SET_STAGE { get; set; }
        }

        /// <summary>
        /// 資料傳輸物件
        /// </summary>
        public class DTO
        {
            /// <summary>
            /// 流程代碼
            /// </summary>
            public string FLOW_ID { get; set; }

            /// <summary>
            /// 流程名稱
            /// </summary>
            public string FLOW_NAME { get; set; }

            /// <summary>
            /// 流程說明
            /// </summary>
            public string MEMO { get; set; }

            /// <summary>
            /// 有效期限
            /// </summary>
            public DateTime EFFECTIVE_DATE { get; set; }

            /// <summary>
            /// 到期日期
            /// </summary>
            public DateTime EXPIRE_DATE { get; set; }

            /// <summary>
            /// 是否啟用
            /// </summary>
            public bool ENABLE_FLG { get; set; }

            /// <summary>
            /// 使用表單數
            /// </summary>
            public int FORM_COUNT { get; set; }

            /// <summary>
            /// 轉換為前端編輯用檢視物件
            /// </summary>
            /// <returns>前端編輯用檢視物件</returns>
            public FlowSetModel.EditView ToEditView()
            {
                return new FlowSetModel.EditView()
                {
                    FLOW_ID = this.FLOW_ID,
                    FLOW_NAME = this.FLOW_NAME,
                    MEMO = this.MEMO,
                    EFFECTIVE_DATE = this.EFFECTIVE_DATE,
                    EXPIRE_DATE = this.EXPIRE_DATE,
                    SET_STAGE = new List<FlowSetDetailModel.EditView>()
                };
            }
        }
    }

    /// <summary>
    /// 流程設定明細資料模型
    /// </summary>
    /// <remarks>關卡資訊</remarks>
    public class FlowSetDetailModel
    {
        /// <summary>
        /// 編輯用檢視物件
        /// </summary>
        public class EditView
        {
            /// <summary>
            /// 流程代碼
            /// </summary>
            public string FLOW_ID { get; set; }

            /// <summary>
            /// 關卡順序
            /// </summary>
            public Int16 SET_ODR { get; set; }

            /// <summary>
            /// 單位
            /// </summary>
            public string SET_ORG_ID { get; set; }

            /// <summary>
            /// 角色
            /// </summary>
            public string SET_ROLE_ID { get; set; }

            /// <summary>
            /// 使用者
            /// </summary>
            public string SET_USER_ID { get; set; }

            /// <summary>
            /// 流程
            /// </summary>
            public string SET_FLOW_ID { get; set; }

            /// <summary>
            /// 是否決行
            /// </summary>
            public bool SET_DECISION { get; set; }

            /// <summary>
            /// 其他設定項
            /// </summary>
            public SET_OPTIONS SET_OPTION { get; set; }

            /// <summary>
            /// 轉換資料傳輸物件
            /// </summary>
            /// <returns>資料傳輸物件</returns>
            public FlowSetDetailModel.DTO ToDTO()
            {
                return new FlowSetDetailModel.DTO()
                {
                    FLOW_ID = this.FLOW_ID,
                    SET_ODR = this.SET_ODR,
                    SET_ORG_ID = this.SET_ORG_ID,
                    SET_ROLE_ID = this.SET_ROLE_ID,
                    SET_USER_ID = this.SET_USER_ID,
                    SET_FLOW_ID = this.SET_FLOW_ID,
                    SET_DECISION = this.SET_DECISION,
                    SET_OPTION = new JavaScriptSerializer().Serialize(this.SET_OPTION)
                };
            }
        }

        /// <summary>
        /// 設定項目
        /// </summary>
        public class SET_OPTIONS
        {
            /// <summary>
            /// 是否 MAIL 通知
            /// </summary>
            public bool MAIL { get; set; }

            /// <summary>
            /// 是否簽章
            /// </summary>
            public bool SIGNATURE { get; set; }

            /// <summary>
            /// 是否檢查憑證
            /// </summary>
            public bool CERTIFICATE { get; set; }

            /// <summary>
            /// 是否封存
            /// </summary>
            public bool SEALED { get; set; }
        }

        /// <summary>
        /// 資料傳輸物件
        /// </summary>
        public class DTO
        {
            /// <summary>
            /// 流程代碼
            /// </summary>
            public string FLOW_ID { get; set; }

            /// <summary>
            /// 關卡順序
            /// </summary>
            public Int16 SET_ODR { get; set; }

            /// <summary>
            /// 單位
            /// </summary>
            public string SET_ORG_ID { get; set; }

            /// <summary>
            /// 角色
            /// </summary>
            public string SET_ROLE_ID { get; set; }

            /// <summary>
            /// 使用者
            /// </summary>
            public string SET_USER_ID { get; set; }

            /// <summary>
            /// 流程
            /// </summary>
            public string SET_FLOW_ID { get; set; }

            /// <summary>
            /// 是否決行
            /// </summary>
            public bool SET_DECISION { get; set; }

            /// <summary>
            /// 其他設定項
            /// </summary>
            public string SET_OPTION { get; set; }

            /// <summary>
            /// 轉換為前端編輯用檢視物件
            /// </summary>
            /// <remarks>由於 SET_OPTION 為 Json 格式資料, 於底層會被 ParseXSS 針對特殊字元編碼, 故於此再次解析</remarks>
            /// <returns>前端編輯用檢視物件</returns>
            public FlowSetDetailModel.EditView ToEditView()
            {
                return new FlowSetDetailModel.EditView()
                {
                    FLOW_ID = this.FLOW_ID,
                    SET_ODR = this.SET_ODR,
                    SET_ORG_ID = this.SET_ORG_ID,
                    SET_ROLE_ID = this.SET_ROLE_ID,
                    SET_USER_ID = this.SET_USER_ID,
                    SET_FLOW_ID = this.SET_FLOW_ID,
                    SET_DECISION = this.SET_DECISION,
                    SET_OPTION = new JavaScriptSerializer().Deserialize<FlowSetDetailModel.SET_OPTIONS>(HttpUtility.HtmlDecode(this.SET_OPTION))
                };
            }
        }
    }
}