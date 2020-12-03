using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDO.Models;
using System.Transactions;
using System.Collections;
using System.Web.Mvc;

namespace SDO.Dacs
{
    /// <summary>
    /// 郵件角色設定資料存取
    /// </summary>
    /// <remarks>
    ///     Created by Steven Tsai at 2017/07/10
    /// </remarks>
    public class MailRoleDac : _Dac
    {
        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="loginUser">登入使用者資訊</param>
        public MailRoleDac(EmpUserModel loginUser) : base(loginUser) { }

        /// <summary>
        /// 取得特定郵件角色設定資訊
        /// </summary>
        /// <param name="roleID">角色代碼</param>
        /// <returns>特定郵件角色設定資訊</returns>
        public MailRoleModel.DTO Read(string roleID)
        {
            strSql = @"
                SELECT	role_id, role_name, del_flg 
                FROM	dbo.MAIL_ROLE 
                WHERE	role_id = @role_id
            ";
            return ExecuteQuery<MailRoleModel.DTO>(strSql, new { role_id = roleID }).SingleOrDefault();
        }

        /// <summary>
        /// 取得所有郵件角色設定資訊
        /// </summary>
        /// <returns>所有郵件角色設定資訊</returns>
        public IList<MailRoleModel.DTO> Read()
        {
            strSql = @"
                SELECT	role_id, 
		                role_name, 
		                del_flg, 
		                (
			                SELECT	COUNT([user_id])
			                FROM	MAP_USER_MAILROLE
			                WHERE	role_id = MR.role_id
		                ) AS user_count
                FROM	MAIL_ROLE AS MR
                WHERE	del_flg=0
            ";
            return ExecuteQuery<MailRoleModel.DTO>(strSql);
        }

        /// <summary>
        /// 取得擁有特定郵件角色的使用者清單
        /// </summary>
        /// <param name="roleID">角色代碼</param>
        /// <returns>擁有特定郵件角色的使用者清單</returns>
        public List<SelectListItem> ReadUsers(string roleID)
        {
            strSql = @"
                SELECT	    MUM.[user_id] AS [Value], 
                            MUM.[user_id] + '-' + EU.[user_name] AS [Text]
                FROM	    dbo.MAP_USER_MAILROLE AS MUM
                LEFT JOIN   EMP_USER AS EU 
                                ON  MUM.[user_id] = EU.[user_id]
                WHERE	    MUM.role_id = @role_id
            ";
            return ExecuteQuery<SelectListItem>(strSql, new { role_id = roleID }).ToList();
        }

        /// <summary>
        /// 新增郵件角色設定
        /// </summary>
        /// <param name="editModel">郵件角色資料編輯模型</param>
        /// <returns>新增郵件角色設定結果</returns>
        public RtnResultModel Create(MailRoleModel.EditView editModel)
        {
            #region 檢查郵件角色設定是否已存在

            // 查詢指定角色代碼資料
            MailRoleModel.DTO model = Read(editModel.ROLE_ID);

            if (model != default(MailRoleModel.DTO) && !(model.DEL_FLG))
            {
                return new RtnResultModel(false, string.Format(i18N.Message.R07, editModel.ROLE_ID));
            }

            #endregion

            #region 執行修改或新增

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    strSql = model != default(MailRoleModel.DTO)
                        ? @"
                            UPDATE	dbo.MAIL_ROLE 
                            SET		del_flg = 0,
		                            role_name = @role_name,
		                            mdf_date = GETDATE(),
		                            mdf_user = @logged_user
                            where	role_id = @role_id
                        "
                        : @"
                            INSERT INTO	dbo.MAIL_ROLE
                            (
	                            role_id, role_name, crt_date, crt_user, mdf_date, mdf_user
                            ) VALUES (
	                            @role_id, @role_name, GETDATE(), @logged_user, GETDATE(), @logged_user
                            )
                        ";

                    ExecuteCommand(strSql, new
                    {
                        role_id = editModel.ROLE_ID,
                        role_name = editModel.ROLE_NAME,
                        logged_user = loggedUser.USER_ID
                    });

                    // 更新郵件角色設定的對應使用者
                    __UpdateMapUser(editModel);

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
        /// 修改郵件角色設定
        /// </summary>
        /// <param name="editModel">郵件角色資料編輯模型</param>
        /// <returns>修改郵件角色設定結果</returns>
        public RtnResultModel Update(MailRoleModel.EditView editModel)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    // 更新郵件角色設定
                    strSql = @"
                        UPDATE	dbo.MAIL_ROLE 
                        SET		del_flg = 0,
		                        role_name = @role_name,
		                        mdf_date = GETDATE(),
		                        mdf_user = @logged_user 
                        WHERE	[role_id] = @role_id
                    ";

                    ExecuteCommand(strSql, new
                    {
                        role_id = editModel.ROLE_ID,
                        role_name = editModel.ROLE_NAME,
                        logged_user = loggedUser.USER_ID
                    });

                    // 更新郵件角色設定的對應使用者
                    __UpdateMapUser(editModel);

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
        /// 更新郵件角色設定的對應使用者
        /// </summary>
        /// <param name="editModel">郵件角色資料編輯模型</param>
        private void __UpdateMapUser(MailRoleModel.EditView editModel)
        {
            #region 清除原先的對應使用者設定

            ExecuteCommand(@"
                DELETE 
                FROM    dbo.MAP_USER_MAILROLE 
                WHERE   role_id = @role_id
            ",
                new
                {
                    role_id = editModel.ROLE_ID
                }
            );

            #endregion

            #region 更新對應使用者設定

            if (editModel.MAP_USER != null && editModel.MAP_USER.Count() > 0)
            {
                objParam = new ArrayList();

                foreach (string userID in editModel.MAP_USER)
                {
                    ((ArrayList)objParam).Add(new
                    {
                        role_id = editModel.ROLE_ID,
                        user_id = userID,
                        logged_user = loggedUser.USER_ID
                    });
                }

                ExecuteCommand(@"
                    INSERT INTO	dbo.MAP_USER_MAILROLE
                    (	
	                    role_id,user_id, crt_date, crt_user
                    ) VALUES (
	                    @role_id, @user_id, GETDATE(), @logged_user
                    )
                ",
                    ((ArrayList)objParam).ToArray()
                );
            }

            #endregion
        }

        /// <summary>
        /// 刪除郵件角色設定
        /// </summary>
        /// <param name="roleID">角色代碼</param>
        /// <returns>刪除郵件角色設定結果</returns>
        public RtnResultModel Delete(string roleID)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    strSql = @"
                        -- 刪除對應使用者設定
                        DELETE 
                        FROM    dbo.MAP_USER_MAILROLE 
                        WHERE   role_id = @role_id

                        -- 刪除郵件角色設定
                        UPDATE  dbo.MAIL_ROLE 
                        SET     del_flg = 1,
                                mdf_date = GETDATE(),
                                mdf_user = @logged_user 
                        WHERE   role_id = @role_id
                    ";
                    ExecuteCommand(strSql, new { role_id = roleID, logged_user = loggedUser.USER_ID });
                    scope.Complete();
                    return new RtnResultModel(true, i18N.Message.R06);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}