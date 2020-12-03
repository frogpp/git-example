using SDO.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace SDO.Dacs
{
    /// <summary>
    /// 郵件範本設定資料存取
    /// </summary>
    /// <remarks>
    ///     Created by Steven Tsai at 2017/07/10
    /// </remarks>
    public class MailSetDac: _Dac
    {
        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="loginUser">登入使用者資訊</param>
        public MailSetDac(EmpUserModel loginUser) : base(loginUser) { }

        /// <summary>
        /// 取得特定郵件範本設定資訊
        /// </summary>
        /// <param name="mailID">郵件範本代碼</param>
        /// <returns>特定郵件範本設定資訊</returns>
        public MailSetModel.DTO Read(string mailID)
        {
            strSql = @"
                SELECT  mail_id,
		                mail_name,
		                mail_subject,
                        mail_content,
		                del_flg
                FROM	MAIL_TEMPLATE
                WHERE	mail_id = @mail_id
            ";
            return ExecuteQuery<MailSetModel.DTO>(strSql, new { mail_id = mailID }).SingleOrDefault();
        }

        /// <summary>
        /// 取得所有郵件範本設定資訊
        /// </summary>
        /// <returns>所有郵件範本設定資訊</returns>
        public IList<MailSetModel.DTO> Read()
        {
            strSql = @"
                SELECT  mail_id,
		                mail_name,
		                mail_subject,
		                del_flg
                FROM	MAIL_TEMPLATE
                WHERE	del_flg=0
            ";
            return ExecuteQuery<MailSetModel.DTO>(strSql);
        }

        /// <summary>
        /// 新增郵件範本設定
        /// </summary>
        /// <param name="editModel">郵件範本設定資料編輯模型</param>
        /// <returns>新增郵件範本設定結果</returns>
        public RtnResultModel Create(MailSetModel.EditView editModel)
        {
            #region 檢查郵件範本設定是否已存在

            // 查詢指定郵件範本設定資料
            MailSetModel.DTO model = Read(editModel.MAIL_ID);

            if (model != default(MailSetModel.DTO))
            {
                return new RtnResultModel(false, string.Format(i18N.Message.R07, editModel.MAIL_ID));
            }

            #endregion

            #region 執行新增

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    strSql = @"
                        INSERT INTO	dbo.MAIL_TEMPLATE
                        (
	                        mail_id, mail_name, mail_subject, mail_content, crt_date, crt_user, mdf_date, mdf_user
                        ) VALUES (
	                        @mail_id, @mail_name, @mail_subject, @mail_content, GETDATE(), @logged_user, GETDATE(), @logged_user
                        )
                    ";

                    ExecuteCommand(strSql, new
                    {
                        mail_id = editModel.MAIL_ID,
                        mail_name = editModel.MAIL_NAME,
                        mail_subject = editModel.MAIL_SUBJECT,
                        mail_content = editModel.MAIL_CONTENT,
                        logged_user = loggedUser.USER_ID
                    });

                    // 更新範本設定的郵件設定明細
                    __UpdateMailSetDetail(editModel);

                    scope.Complete();

                    return new RtnResultModel(true, i18N.Message.R04);
                }
            }
            catch
            {
                throw;
            }

            #endregion
        }

        /// <summary>
        /// 修改郵件範本設定
        /// </summary>
        /// <param name="editModel">郵件範本設定資料編輯模型</param>
        /// <returns>修改郵件範本設定結果</returns>
        public RtnResultModel Update(MailSetModel.EditView editModel)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    // 更新郵件範本設定資料
                    strSql = @"
                        UPDATE	dbo.MAIL_TEMPLATE
                        SET		mail_name = @mail_name, 
                                mail_subject = @mail_subject, 
                                mail_content = @mail_content, 
		                        mdf_date = GETDATE(),
		                        mdf_user = @logged_user 
                        WHERE	[mail_id] = @mail_id
                    ";

                    ExecuteCommand(strSql, new
                    {
                        mail_id = editModel.MAIL_ID,
                        mail_name = editModel.MAIL_NAME,
                        mail_subject = editModel.MAIL_SUBJECT,
                        mail_content = editModel.MAIL_CONTENT,
                        logged_user = loggedUser.USER_ID
                    });

                    // 更新範本設定的郵件設定明細
                    __UpdateMailSetDetail(editModel);

                    scope.Complete();

                    return new RtnResultModel(true, i18N.Message.R05);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 刪除郵件範本設定
        /// </summary>
        /// <param name="mailID">郵件範本代碼</param>
        /// <returns>刪除郵件範本設定結果</returns>
        public RtnResultModel Delete(string mailID)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    strSql = @"
                        -- 刪除範本的郵件設定明細
                        DELETE 
                        FROM    dbo.MAIL_RECIPIENT
                        WHERE   mail_id = @mail_id

                        -- 刪除郵件範本設定資料
                        DELETE 
                        FROM    dbo.MAIL_TEMPLATE 
                        WHERE   mail_id = @mail_id
                    ";
                    ExecuteCommand(strSql, new { mail_id = mailID });
                    scope.Complete();
                    return new RtnResultModel(true, i18N.Message.R06);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 取得特定範本的郵件設定明細
        /// </summary>
        /// <param name="mailID">郵件範本代碼</param>
        /// <returns>特定範本的郵件設定明細</returns>
        public IList<MailSetDetailModel.DTO> ReadDetail(string mailID)
        {
            strSql = @"
                SELECT	mail_id, 
                        mail_type, 
                        mail_role, 
                        mail_address, 
                        mail_title
                FROM	MAIL_RECIPIENT
                WHERE	mail_id = @mail_id
            ";
            return ExecuteQuery<MailSetDetailModel.DTO>(strSql, new { mail_id = mailID });
        }

        /// <summary>
        /// 更新範本設定的郵件設定明細
        /// </summary>
        /// <param name="editModel">郵件範本設定資料編輯模型</param>
        private void __UpdateMailSetDetail(MailSetModel.EditView editModel)
        {
            #region 清除原先的郵件設定明細

            ExecuteCommand(@"
                DELETE 
                FROM    dbo.MAIL_RECIPIENT
                WHERE   mail_id = @mail_id
            ",
                new
                {
                    mail_id = editModel.MAIL_ID
                }
            );

            #endregion

            #region 更新郵件設定明細

            if (editModel.MAIL_SETTING != null && editModel.MAIL_SETTING.Count() > 0)
            {
                objParam = new ArrayList();

                foreach (MailSetDetailModel.EditView detailViewModel in editModel.MAIL_SETTING)
                {
                    ((ArrayList)objParam).Add(new
                    {
                        mail_id = editModel.MAIL_ID,
                        mail_type = detailViewModel.MAIL_TYPE,
                        mail_role = detailViewModel.MAIL_ROLE ?? string.Empty,
                        mail_address = detailViewModel.MAIL_ADDRESS ?? string.Empty,
                        mail_title = detailViewModel.MAIL_TITLE ?? string.Empty
                    });

                }

                ExecuteCommand(@"
                    INSERT INTO	dbo.MAIL_RECIPIENT
                    (	
                        mail_id, mail_type, mail_role, mail_address, mail_title
                    ) VALUES (
	                    @mail_id, @mail_type, @mail_role, @mail_address, @mail_title
                    )
                ",
                    ((ArrayList)objParam).ToArray()
                );

            }

            #endregion
        }

    }
}