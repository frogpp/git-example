using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDO.Models
{
    /// <summary>
    /// 郵件角色設定資料模型
    /// </summary>
    public class MailRoleModel
    {
        /// <summary>
        /// 編輯用檢視物件
        /// </summary>
        public class EditView
        {
            /// <summary>
            /// 角色代碼
            /// </summary>
            [Key, Required]
            [Display(Name = "MailRole_ROLE_ID", ResourceType = typeof(i18N.Label))]
            public string ROLE_ID { get; set; }

            /// <summary>
            /// 角色名稱
            /// </summary>
            [Required]
            [Display(Name = "MailRole_ROLE_NAME", ResourceType = typeof(i18N.Label))]
            public string ROLE_NAME { get; set; }

            /// <summary>
            /// 帳號數
            /// </summary>
            [Display(Name = "MailRole_MAP_USER", ResourceType = typeof(i18N.Label))]
            public string[] MAP_USER { get; set; }

            /// <summary>
            /// 帳號數
            /// </summary>
            /// <remarks>
            ///     修改時, 預設已選擇的帳號, 以選項清單設定值
            /// </remarks>
            public List<SelectListItem> DEFAULT_MAP_USER { get; set; }
        }

        /// <summary>
        /// 資料傳輸物件
        /// </summary>
        public class DTO
        {
            /// <summary>
            /// 角色代碼
            /// </summary>
            public string ROLE_ID { get; set; }

            /// <summary>
            /// 角色名稱
            /// </summary>
            public string ROLE_NAME { get; set; }

            /// <summary>
            /// 是否已刪除
            /// </summary>
            public bool DEL_FLG { get; set; }

            /// <summary>
            /// 帳號數
            /// </summary>
            public int USER_COUNT { get; set; }

            /// <summary>
            /// 轉換為前端編輯用檢視物件
            /// </summary>
            /// <returns>前端編輯用檢視物件</returns>
            public MailRoleModel.EditView ToEditView()
            {
                return new MailRoleModel.EditView()
                {
                    ROLE_ID = this.ROLE_ID,
                    ROLE_NAME = this.ROLE_NAME
                };
            }
        }
    }
}