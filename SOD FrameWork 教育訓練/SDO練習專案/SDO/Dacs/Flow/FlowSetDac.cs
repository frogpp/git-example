using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SDO.Models;
using System.Collections;
using System.Transactions;

namespace SDO.Dacs
{
    /// <summary>
    /// 流程設定資料存取
    /// </summary>
    /// <remarks>
    ///     Created by Steven Tsai at 2017/06/19
    /// </remarks>
    public class FlowSetDac : _Dac
    {
        /// <summary>
        /// 建構函式
        /// </summary>
        /// <param name="loginUser">登入使用者資訊</param>
        public FlowSetDac(EmpUserModel loginUser) : base(loginUser) { }

        /// <summary>
        /// 取得所有流程設定資訊
        /// </summary>
        /// <returns>所有流程設定資訊</returns>
        public IList<FlowSetModel.DTO> Read()
        {
            strSql = @"
                SELECT	flow_id, 
		                flow_name,
                        memo,
			            FS.effective_date, 
			            FS.expire_date, 
		                CASE 
			                WHEN CONVERT(VARCHAR(10), GETDATE(), 111) 
					                BETWEEN	CONVERT(VARCHAR(10), FS.effective_date, 111) 
					                AND		CONVERT(VARCHAR(10), FS.expire_date, 111) 
			                THEN CAST(1 AS BIT)
			                ELSE CAST(0 AS BIT)
		                END AS enable_flg,
		                (
			                SELECT	COUNT(form_id)
			                FROM	MAP_EFORM_FLOW
			                WHERE	flow_id = FS.flow_id
		                ) AS form_count
                FROM	FLOW_SET AS FS
            ";
            return ExecuteQuery<FlowSetModel.DTO>(strSql);
        }

        /// <summary>
        /// 取得流程設定資訊
        /// </summary>
        /// <param name="flowID">流程代碼</param>
        /// <returns>特定流程設定資訊</returns>
        public FlowSetModel.DTO Read(string flowID)
        {
            strSql = @"
                SELECT	flow_id, 
		                flow_name,
                        memo,
			            effective_date, 
			            expire_date
                FROM	FLOW_SET
                WHERE	flow_id = @flow_id
            ";
            return ExecuteQuery<FlowSetModel.DTO>(strSql, new { flow_id = flowID }).SingleOrDefault();
        }

        /// <summary>
        /// 取得流程設定資訊
        /// </summary>
        /// <param name="flowID">流程代碼</param>
        /// <returns>特定流程設定資訊</returns>
        public IList<FlowSetDetailModel.DTO> ReadDetail(string flowID)
        {
            strSql = @"
                SELECT	flow_id, 
                        set_odr, 
                        set_org_id, 
                        set_role_id, 
                        set_user_id, 
                        set_flow_id, 
                        set_decision, 
                        set_option
                FROM	FLOW_SET_DETAIL
                WHERE	flow_id = @flow_id
            ";
            return ExecuteQuery<FlowSetDetailModel.DTO>(strSql, new { flow_id = flowID });
        }

        /// <summary>
        /// 新增流程設定
        /// </summary>
        /// <param name="editModel">流程設定資料編輯模型</param>
        /// <returns>新增流程設定結果</returns>
        public RtnResultModel Create(FlowSetModel.EditView editModel)
        {
            #region 檢查流程設定是否已存在

            // 查詢指定流程設定資料
            FlowSetModel.DTO model = Read(editModel.FLOW_ID);

            if (model != default(FlowSetModel.DTO))
            {
                return new RtnResultModel(false, string.Format(i18N.Message.R07, editModel.FLOW_ID));
            }

            #endregion

            #region 執行新增

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    strSql = @"
                        INSERT INTO	dbo.FLOW_SET
                        (
	                        flow_id, flow_name, effective_date, expire_date, memo, crt_date, crt_user, mdf_date, mdf_user
                        ) VALUES (
	                        @flow_id, @flow_name, @effective_date, @expire_date, @memo, GETDATE(), @logged_user, GETDATE(), @logged_user
                        )
                    ";

                    ExecuteCommand(strSql, new
                    {
                        flow_id = editModel.FLOW_ID,
                        flow_name = editModel.FLOW_NAME,
                        effective_date = editModel.EFFECTIVE_DATE,
                        expire_date = editModel.EXPIRE_DATE,
                        memo = editModel.MEMO ?? string.Empty,
                        logged_user = loggedUser.USER_ID
                    });

                    // 更新流程設定的關卡明細
                    __UpdateFlowSetStage(editModel);

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
        /// 修改流程設定
        /// </summary>
        /// <param name="editModel">流程設定資料編輯模型</param>
        /// <returns>修改流程設定結果</returns>
        public RtnResultModel Update(FlowSetModel.EditView editModel)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    // 更新流程角色設定
                    strSql = @"
                        UPDATE	dbo.FLOW_SET
                        SET		flow_name = @flow_name, 
                                effective_date = @effective_date, 
                                expire_date = @expire_date, 
                                memo = @memo,
		                        mdf_date = GETDATE(),
		                        mdf_user = @logged_user 
                        WHERE	[flow_id] = @flow_id
                    ";

                    ExecuteCommand(strSql, new
                    {
                        flow_id = editModel.FLOW_ID,
                        flow_name = editModel.FLOW_NAME,
                        effective_date = editModel.EFFECTIVE_DATE,
                        expire_date = editModel.EXPIRE_DATE,
                        memo = editModel.MEMO ?? string.Empty,
                        logged_user = loggedUser.USER_ID
                    });

                    // 更新流程設定的關卡明細
                    __UpdateFlowSetStage(editModel);

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
        /// 更新流程設定的關卡明細
        /// </summary>
        /// <param name="editModel">表單設定資料編輯模型</param>
        private void __UpdateFlowSetStage(FlowSetModel.EditView editModel)
        {
            #region 清除原先的關卡明細

            ExecuteCommand(@"
                DELETE 
                FROM    dbo.FLOW_SET_DETAIL
                WHERE   flow_id = @flow_id
            ",
                new
                {
                    flow_id = editModel.FLOW_ID
                }
            );

            #endregion

            #region 更新關卡明細設定

            if (editModel.SET_STAGE != null && editModel.SET_STAGE.Count() > 0)
            {
                objParam = new ArrayList();

                foreach (FlowSetDetailModel.EditView detailViewModel in editModel.SET_STAGE)
                {

                    // 將檢視物件轉資料傳輸物件再處理進資料庫的資料
                    FlowSetDetailModel.DTO detailDTOModel = detailViewModel.ToDTO();

                    ((ArrayList)objParam).Add(new
                    {
                        flow_id = editModel.FLOW_ID,
                        set_odr = detailDTOModel.SET_ODR,
                        set_org_id = detailDTOModel.SET_ORG_ID ?? string.Empty,
                        set_role_id = detailDTOModel.SET_ROLE_ID ?? string.Empty,
                        set_user_id = detailDTOModel.SET_USER_ID ?? string.Empty,
                        set_flow_id = detailDTOModel.SET_FLOW_ID ?? string.Empty,
                        set_decision = detailDTOModel.SET_DECISION,
                        set_option = detailDTOModel.SET_OPTION,
                        logged_user = loggedUser.USER_ID
                    });

                }

                ExecuteCommand(@"
                    INSERT INTO	dbo.FLOW_SET_DETAIL
                    (	
                        flow_id, set_odr, set_org_id, set_role_id, set_user_id, set_flow_id, set_decision, set_option, crt_date, crt_user
                    ) VALUES (
	                    @flow_id, @set_odr, @set_org_id, @set_role_id, @set_user_id, @set_flow_id, @set_decision, @set_option, GETDATE(), @logged_user
                    )
                ",
                    ((ArrayList)objParam).ToArray()
                );

            }

            #endregion
        }

        /// <summary>
        /// 刪除流程設定
        /// </summary>
        /// <param name="flowID">流程代碼</param>
        /// <returns>刪除流程設定結果</returns>
        public RtnResultModel Delete(string flowID)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    strSql = @"
                        -- 刪除流程的關卡明細
                        DELETE 
                        FROM    dbo.FLOW_SET_DETAIL
                        WHERE   flow_id = @flow_id

                        -- 刪除流程設定
                        DELETE 
                        FROM    dbo.FLOW_SET 
                        WHERE   flow_id = @flow_id
                    ";
                    ExecuteCommand(strSql, new { flow_id = flowID });
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