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
    /// 表單設定資料存取
    /// </summary>
    /// <remarks>
    ///     Created by Steven Tsai at 2017/06/15
    /// </remarks>
    public class eFormSetDac : _Dac
    {
        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="loginUser">登入使用者資訊</param>
        public eFormSetDac(EmpUserModel loginUser) : base(loginUser) { }

        /// <summary>
        /// 取得所有表單設定資訊
        /// </summary>
        /// <param name="model">查詢條件</param>
        /// <returns>所有表單設定資訊</returns>
        public IList<eFormSetModel.DTO> Read(eFormSetModel.QueryView model)
        {
            strSql = @"
                SELECT		FS.form_id, 
			                FS.form_type,
			                FMT.set_value AS form_type_name,
			                FS.form_name, 
			                FS.effective_date, 
			                FS.expire_date, 
			                FS.memo
                FROM		dbo.EFORM_SET AS FS
                LEFT JOIN	SET_PARAM AS FMT 
				                ON	FMT.set_item = 'eFormType' 
				                AND	FS.form_type = FMT.set_type
                WHERE       ( 
                                @form_type = '' OR FS.form_type = @form_type 
                            )
                AND		    (   @effective_date = '' 
                                OR CAST( @effective_date AS DATE )  
                                    BETWEEN	CONVERT(VARCHAR(10), FS.effective_date, 111) 
			                        AND		CONVERT(VARCHAR(10), FS.expire_date, 111) 
                            )
                AND		    ( 
                                @org_id = '' 
                                OR FS.form_id IN 
                                (
                                    SELECT	DISTINCT form_id
                                    FROM	dbo.MAP_EFORM_ORG
                                    WHERE	org_id IN (
                                                SELECT	value
	                                            FROM	dbo.fn_Split(@org_id, ',')
                                            )
                                )
                            )
            ";
            return ExecuteQuery<eFormSetModel.DTO>(strSql, new
            {
                form_type = model.FORM_TYPE ?? string.Empty,
                effective_date = model.EFFECTIVE_DATE == null ? string.Empty : ((DateTime)model.EFFECTIVE_DATE).ToString("yyyy/MM/dd"),
                org_id = model.MAP_ORG == null || model.MAP_ORG.Length == 0 ? string.Empty : string.Join(",", model.MAP_ORG)
            });
        }

        /// <summary>
        /// 取得表單設定資訊
        /// </summary>
        /// <param name="formID">表單代碼</param>
        /// <returns>特定表單設定資訊</returns>
        public eFormSetModel.DTO Read(string formID)
        {
            strSql = @"
                SELECT	form_id, 
                        form_type,
                        form_name, 
                        effective_date, 
                        expire_date, 
                        memo 
                FROM	dbo.EFORM_SET 
                WHERE	form_id = @form_id
            ";
            return ExecuteQuery<eFormSetModel.DTO>(strSql, new { form_id = formID }).SingleOrDefault();
        }

        /// <summary>
        /// 取得表單的對應流程
        /// </summary>
        /// <param name="formID">表單代碼</param>
        /// <remarks>流程為單選</remarks>
        /// <returns>表單的對應流程</returns>
        public string ReadMapFlows(string formID)
        {
            strSql = @"
                SELECT	[flow_id]
                FROM	dbo.MAP_EFORM_FLOW
                WHERE	form_id = @form_id
            ";
            return ExecuteQuery<string>(strSql, new { form_id = formID }).SingleOrDefault();
        }

        /// <summary>
        /// 取得表單的使用單位
        /// </summary>
        /// <param name="formID">表單代碼</param>
        /// <returns>表單的使用單位</returns>
        public List<SelectListItem> ReadMapOrgs(string formID)
        {
            strSql = @"
                SELECT		MEO.[org_id] AS [Value],
                            MEO.[org_id] + '-' + EO.org_name AS [Text]
                FROM		dbo.MAP_EFORM_ORG AS MEO
                LEFT JOIN	dbo.EMP_ORG AS EO ON MEO.org_id = EO.org_id 
                WHERE	    form_id = @form_id
            ";
            return ExecuteQuery<SelectListItem>(strSql, new { form_id = formID }).ToList();
        }

        /// <summary>
        /// 新增表單設定
        /// </summary>
        /// <param name="editModel">表單設定資料編輯模型</param>
        /// <returns>新增表單設定結果</returns>
        public RtnResultModel Create(eFormSetModel.EditView editModel)
        {
            #region 檢查表單設定是否已存在

            // 查詢指定表單設定資料
            eFormSetModel.DTO model = Read(editModel.FORM_ID);

            if (model != default(eFormSetModel.DTO))
            {
                return new RtnResultModel(false, string.Format(i18N.Message.R07, editModel.FORM_ID));
            }

            #endregion

            #region 執行新增

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    strSql = @"
                        INSERT INTO	dbo.EFORM_SET
                        (
	                        form_id, form_type, form_name, effective_date, expire_date, memo, crt_date, crt_user, mdf_date, mdf_user
                        ) VALUES (
	                        @form_id, @form_type, @form_name, @effective_date, @expire_date, @memo, GETDATE(), @logged_user, GETDATE(), @logged_user
                        )
                    ";

                    ExecuteCommand(strSql, new
                    {
                        form_id = editModel.FORM_ID,
                        form_type = editModel.FORM_TYPE,
                        form_name = editModel.FORM_NAME,
                        effective_date = editModel.EFFECTIVE_DATE,
                        expire_date = editModel.EXPIRE_DATE,
                        memo = editModel.MEMO ?? string.Empty,
                        logged_user = loggedUser.USER_ID
                    });

                    // 更新表單設定的對應申請流程
                    __UpdateMapFlow(editModel);

                    // 更新表單設定的對應使用單位
                    __UpdateMapOrg(editModel);

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
        /// 修改表單設定
        /// </summary>
        /// <param name="editModel">表單設定資料編輯模型</param>
        /// <returns>修改表單設定結果</returns>
        public RtnResultModel Update(eFormSetModel.EditView editModel)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    // 更新流程角色設定
                    strSql = @"
                        UPDATE	dbo.EFORM_SET 
                        SET		form_type = @form_type, 
                                form_name = @form_name, 
                                effective_date = @effective_date, 
                                expire_date = @expire_date, 
                                memo = @memo,
		                        mdf_date = GETDATE(),
		                        mdf_user = @logged_user 
                        WHERE	[form_id] = @form_id
                    ";

                    ExecuteCommand(strSql, new
                    {
                        form_id = editModel.FORM_ID,
                        form_type = editModel.FORM_TYPE,
                        form_name = editModel.FORM_NAME,
                        effective_date = editModel.EFFECTIVE_DATE,
                        expire_date = editModel.EXPIRE_DATE,
                        memo = editModel.MEMO ?? string.Empty,
                        logged_user = loggedUser.USER_ID
                    });

                    // 更新表單設定的對應申請流程
                    __UpdateMapFlow(editModel);

                    // 更新表單設定的對應使用單位
                    __UpdateMapOrg(editModel);

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
        /// 更新表單設定的對應申請流程
        /// </summary>
        /// <param name="editModel">表單設定資料編輯模型</param>
        /// <remarks>流程為單選</remarks>
        private void __UpdateMapFlow(eFormSetModel.EditView editModel)
        {
            #region 清除原先的對應申請流程

            ExecuteCommand(@"
                DELETE 
                FROM    dbo.MAP_EFORM_FLOW 
                WHERE   form_id = @form_id
            ",
                new
                {
                    form_id = editModel.FORM_ID
                }
            );

            #endregion

            #region 更新對應申請流程設定

            if (!string.IsNullOrEmpty(editModel.MAP_FLOW))
            {
                ExecuteCommand(@"
                    INSERT INTO	dbo.MAP_EFORM_FLOW
                    (	
	                    form_id, flow_id, crt_date, crt_user
                    ) VALUES (
	                    @form_id, @flow_id, GETDATE(), @logged_user
                    )
                ",
                    new
                    {
                        form_id = editModel.FORM_ID,
                        flow_id = editModel.MAP_FLOW,
                        logged_user = loggedUser.USER_ID
                    }
                );
            }

            #endregion
        }

        /// <summary>
        /// 更新表單設定的對應使用單位
        /// </summary>
        /// <param name="editModel">表單設定資料編輯模型</param>
        /// <remarks>單位可多選</remarks>
        private void __UpdateMapOrg(eFormSetModel.EditView editModel)
        {
            #region 清除原先的對應使用單位

            ExecuteCommand(@"
                DELETE 
                FROM    dbo.MAP_EFORM_ORG 
                WHERE   form_id = @form_id
            ",
                new
                {
                    form_id = editModel.FORM_ID
                }
            );

            #endregion

            #region 更新對應使用單位設定

            if (editModel.MAP_ORG != null && editModel.MAP_ORG.Count() > 0)
            {
                objParam = new ArrayList();

                foreach (string orgID in editModel.MAP_ORG)
                {
                    ((ArrayList)objParam).Add(new
                    {
                        form_id = editModel.FORM_ID,
                        org_id = orgID,
                        logged_user = loggedUser.USER_ID
                    });
                }

                ExecuteCommand(@"
                    INSERT INTO	dbo.MAP_EFORM_ORG
                    (	
	                    form_id, org_id, crt_date, crt_user
                    ) VALUES (
	                    @form_id, @org_id, GETDATE(), @logged_user
                    )
                ",
                    ((ArrayList)objParam).ToArray()
                );
            }

            #endregion
        }

        /// <summary>
        /// 刪除表單設定
        /// </summary>
        /// <param name="formID">表單代碼</param>
        /// <returns>刪除表單設定結果</returns>
        public RtnResultModel Delete(string formID)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    strSql = @"
                        -- 刪除對應申請流程設定
                        DELETE 
                        FROM    dbo.MAP_EFORM_FLOW 
                        WHERE   form_id = @form_id

                        -- 刪除對應使用單位設定
                        DELETE 
                        FROM    dbo.MAP_EFORM_ORG 
                        WHERE   form_id = @form_id

                        -- 刪除表單設定
                        DELETE 
                        FROM    dbo.EFORM_SET 
                        WHERE   form_id = @form_id
                    ";
                    ExecuteCommand(strSql, new { form_id = formID, logged_user = loggedUser.USER_ID });
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