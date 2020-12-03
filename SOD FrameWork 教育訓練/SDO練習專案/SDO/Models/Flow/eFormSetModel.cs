using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDO.Models
{
    /// <summary>
    /// 表單設定資料模型
    /// </summary>
    public class eFormSetModel
    {
        /// <summary>
        /// 查詢用檢視物件
        /// </summary>
        public class QueryView
        {
            /// <summary>
            /// 表單類型
            /// </summary>
            [Display(Name = "eForm_FLOW_TYPE", ResourceType = typeof(i18N.Label))]
            public string FORM_TYPE { get; set; }

            /// <summary>
            /// 有效期限
            /// </summary>
            [Display(Name = "eForm_EFFECTIVE_DATE", ResourceType = typeof(i18N.Label))]
            public DateTime? EFFECTIVE_DATE { get; set; }

            /// <summary>
            /// 使用單位
            /// </summary>
            [Display(Name = "eForm_ORG_ID", ResourceType = typeof(i18N.Label))]
            public string[] MAP_ORG { get; set; }
        }

        /// <summary>
        /// 編輯用檢視物件
        /// </summary>
        public class EditView
        {
            /// <summary>
            /// 表單代碼
            /// </summary>
            [Key, Required]
            [Display(Name = "eForm_FORM_ID", ResourceType = typeof(i18N.Label))]
            public string FORM_ID { get; set; }

            /// <summary>
            /// 表單類型
            /// </summary>
            [Required]
            [Display(Name = "eForm_FLOW_TYPE", ResourceType = typeof(i18N.Label))]
            public string FORM_TYPE { get; set; }

            /// <summary>
            /// 表單名稱
            /// </summary>
            [Required]
            [Display(Name = "eForm_FORM_NAME", ResourceType = typeof(i18N.Label))]
            public string FORM_NAME { get; set; }

            /// <summary>
            /// 表單說明
            /// </summary>
            [Display(Name = "eForm_MEMO", ResourceType = typeof(i18N.Label))]
            public string MEMO { get; set; }

            /// <summary>
            /// 有效期限
            /// </summary>
            [Required]
            [Display(Name = "eForm_EFFECTIVE_DATE", ResourceType = typeof(i18N.Label))]
            public DateTime EFFECTIVE_DATE { get; set; }

            /// <summary>
            /// 到期日期
            /// </summary>
            [Required]
            [Display(Name = "eForm_EXPIRE_DATE", ResourceType = typeof(i18N.Label))]
            public DateTime EXPIRE_DATE { get; set; }

            /// <summary>
            /// 申請流程
            /// </summary>
            [Required]
            [Display(Name = "eForm_FOW_ID", ResourceType = typeof(i18N.Label))]
            public string MAP_FLOW { get; set; }

            /// <summary>
            /// 使用單位
            /// </summary>
            /// <remarks>
            ///     存檔時, 前端選取的單位資料, 以字串陣列傳送
            /// </remarks>
            [Required]
            [Display(Name = "eForm_ORG_ID", ResourceType = typeof(i18N.Label))]
            public string[] MAP_ORG { get; set; }

            /// <summary>
            /// 使用單位
            /// </summary>
            /// <remarks>
            ///     修改時, 預設已選擇的單位, 以選項清單設定值
            /// </remarks>
            public List<SelectListItem> DEFAULT_MAP_ORG { get; set; }
        }

        /// <summary>
        /// 資料傳輸物件
        /// </summary>
        public class DTO
        {
            /// <summary>
            /// 表單代碼
            /// </summary>
            public string FORM_ID { get; set; }

            /// <summary>
            /// 表單類型
            /// </summary>
            public string FORM_TYPE { get; set; }

            /// <summary>
            /// 表單類型名稱
            /// </summary>
            public string FORM_TYPE_NAME { get; set; }

            /// <summary>
            /// 表單名稱
            /// </summary>
            public string FORM_NAME { get; set; }

            /// <summary>
            /// 有效期限
            /// </summary>
            public DateTime EFFECTIVE_DATE { get; set; }

            /// <summary>
            /// 到期日期
            /// </summary>
            public DateTime EXPIRE_DATE { get; set; }

            /// <summary>
            /// 表單說明
            /// </summary>
            public string MEMO { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string FORM_DISPLAY {
                get {
                    return string.Format("{0}-{1}", this.FORM_ID, this.FORM_NAME);
                }
                private set { }
            }

            /// <summary>
            /// 轉換為前端編輯用檢視物件
            /// </summary>
            /// <returns>前端編輯用檢視物件</returns>
            public eFormSetModel.EditView ToEditView()
            {
                return new eFormSetModel.EditView()
                {
                    FORM_ID = this.FORM_ID,
                    FORM_TYPE = this.FORM_TYPE,
                    FORM_NAME = this.FORM_NAME,
                    EFFECTIVE_DATE = this.EFFECTIVE_DATE,
                    EXPIRE_DATE = this.EXPIRE_DATE,
                    MEMO = this.MEMO
                };
            }
        }
    }
}