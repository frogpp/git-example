using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDO.Models
{
    /// <summary>
    /// 郵件範本設定資料模型
    /// </summary>
    public class MailSetModel
    {
        /// <summary>
        /// 編輯用檢視物件
        /// </summary>
        public class EditView
        {
            /// <summary>
            /// 郵件範本代碼
            /// </summary>
            [Key, Required]
            [Display(Name = "MailSet_MAIL_ID", ResourceType = typeof(i18N.Label))]
            public string MAIL_ID { get; set; }

            /// <summary>
            /// 郵件範本名稱
            /// </summary>
            [Required]
            [Display(Name = "MailSet_MAIL_NAME", ResourceType = typeof(i18N.Label))]
            public string MAIL_NAME { get; set; }

            /// <summary>
            /// 郵件範本主旨
            /// </summary>
            [Required]
            [Display(Name = "MailSet_MAIL_SUBJECT", ResourceType = typeof(i18N.Label))]
            public string MAIL_SUBJECT { get; set; }

            /// <summary>
            /// 郵件範本內文
            /// </summary>
            [Required]
            [Display(Name = "MailSet_MAIL_CONTENT", ResourceType = typeof(i18N.Label))]
            public string MAIL_CONTENT { get; set; }

            /// <summary>
            /// 郵件設定
            /// </summary>
            [Display(Name = "MailSet_MAIL_SETTING", ResourceType = typeof(i18N.Label))]
            public List<MailSetDetailModel.EditView> MAIL_SETTING { get; set; }
        }

        /// <summary>
        /// 資料傳輸物件
        /// </summary>
        public class DTO
        {
            /// <summary>
            /// 郵件範本代碼
            /// </summary>
            public string MAIL_ID { get; set; }

            /// <summary>
            /// 郵件範本名稱
            /// </summary>
            public string MAIL_NAME { get; set; }

            /// <summary>
            /// 郵件範本主旨
            /// </summary>
            public string MAIL_SUBJECT { get; set; }

            /// <summary>
            /// 郵件範本內文
            /// </summary>
            public string MAIL_CONTENT { get; set; }

            /// <summary>
            /// 是否已刪除
            /// </summary>
            public bool DEL_FLG { get; set; }

            /// <summary>
            /// 轉換為前端編輯用檢視物件
            /// </summary>
            /// <remarks>
            ///     由於 MAIL_CONTENT 為 HTML 格式資料, 
            ///     於「前端 Editor 取值送出存檔」與「底層取資料出來會經過 ParseXSS 機制」皆會針對特殊字元進行編碼, 
            ///     所以從取值完畢到送至畫面顯示前, 
            ///     需經過兩次解碼才得以在 Editor 上正常顯示（Editor 預設顯示是吃未編碼的 HTML 內文）
            /// </remarks>
            /// <returns>前端編輯用檢視物件</returns>
            public MailSetModel.EditView ToEditView()
            {
                return new MailSetModel.EditView()
                {
                    MAIL_ID = this.MAIL_ID,
                    MAIL_NAME = this.MAIL_NAME,
                    MAIL_SUBJECT = this.MAIL_SUBJECT,
                    MAIL_CONTENT = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(this.MAIL_CONTENT)),
                    MAIL_SETTING = new List<MailSetDetailModel.EditView>()
                };
            }
        }
    }

    /// <summary>
    /// 郵件範本設定明細資料模型
    /// </summary>
    /// <remarks>郵件設定資訊</remarks>
    public class MailSetDetailModel
    {
        /// <summary>
        /// 編輯用檢視物件
        /// </summary>
        public class EditView
        {
            /// <summary>
            /// 郵寄類型
            /// </summary>
            public string MAIL_TYPE { get; set; }

            /// <summary>
            /// 郵件角色代碼
            /// </summary>
            public string MAIL_ROLE { get; set; }

            /// <summary>
            /// 自訂使用者信箱
            /// </summary>
            public string MAIL_ADDRESS { get; set; }

            /// <summary>
            /// 自訂使用者名稱
            /// </summary>
            public string MAIL_TITLE { get; set; }
        }

        /// <summary>
        /// 資料傳輸物件
        /// </summary>
        public class DTO
        {
            /// <summary>
            /// 郵寄類型
            /// </summary>
            public string MAIL_TYPE { get; set; }

            /// <summary>
            /// 郵件角色代碼
            /// </summary>
            public string MAIL_ROLE { get; set; }

            /// <summary>
            /// 自訂使用者信箱
            /// </summary>
            public string MAIL_ADDRESS { get; set; }

            /// <summary>
            /// 自訂使用者名稱
            /// </summary>
            public string MAIL_TITLE { get; set; }

            /// <summary>
            /// 轉換為前端編輯用檢視物件
            /// </summary>
            /// <returns>前端編輯用檢視物件</returns>
            public MailSetDetailModel.EditView ToEditView()
            {
                return new MailSetDetailModel.EditView()
                {
                    MAIL_TYPE = this.MAIL_TYPE,
                    MAIL_ROLE = this.MAIL_ROLE,
                    MAIL_ADDRESS = this.MAIL_ADDRESS,
                    MAIL_TITLE = this.MAIL_TITLE
                };
            }
        }
    }
}