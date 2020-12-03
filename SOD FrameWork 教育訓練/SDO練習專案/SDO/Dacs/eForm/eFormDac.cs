using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDO.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SDO.Dacs
{
    /// <summary>
    /// 
    /// </summary>
    public class eFormDac : _Dac
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public eFormDac(EmpUserModel user = null) : base(user)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<IDictionary<string, object>> Read(FormCollection form)
        {
            string strSql = @"select a.fill_id,a.form_id,b.form_name,b.form_type,dbo.fn_GetSetParam('eFormType',b.form_type) as form_typename,a.flow_code,dbo.fn_ChkFlowExecute(a.flow_code) as flow_execute,a.mdf_date 
                                from dbo.EFORM_DATAFILL a 
                                inner join dbo.EFORM_SET b on b.form_id=a.form_id 
                                where a.del_flg=0 and (a.crt_user = @user_id or a.crt_user like '%' + @user_id) and a.form_id=@form_id
                                order by a.mdf_date desc";
            objParam = new
            {
                user_id = HttpUtility.HtmlEncode(form.Get("qryUSER_ID")),
                form_id = HttpUtility.HtmlEncode(form.Get("qryEForm_ID"))
            };
            return ExecuteQuery(strSql, objParam);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<IDictionary<string, object>> ReadFlowList(FormCollection form)
        {
            IList<IDictionary<string, object>> lstFlow = new FlowSet.Log().GetUserList(HttpUtility.HtmlEncode(form.Get("qryUSER_ID")));
            string strSql = @"select a.fill_id,a.form_id,b.form_name,b.form_type,dbo.fn_GetSetParam('eFormType',b.form_type) as form_typename,a.flow_code,dbo.fn_ChkFlowExecute(a.flow_code) as flow_execute,a.crt_user,d.user_name as crt_name,a.crt_date,a.mdf_date,c.alert_day 
                                from dbo.EFORM_DATAFILL a 
                                inner join dbo.EFORM_SET b on b.form_id=a.form_id 
                                inner join dbo.MAP_EFORM_FLOW c on c.form_id=a.form_id
                                inner join dbo.EMP_USER d on d.user_id=a.crt_user
                                where a.del_flg=0 and a.flow_code in @flow_code
                                order by a.mdf_date desc";
            objParam = new
            {
                flow_code = lstFlow.Select(p => p["FLOW_CODE"].ToString()).ToArray()
            };
            IList<IDictionary<string, object>> result = ExecuteQuery(strSql, objParam);
            foreach (IDictionary<string, object> dic in result)
            {
                var flow = lstFlow.Where(p => p["FLOW_CODE"].ToString() == dic["FLOW_CODE"].ToString()).FirstOrDefault();
                dic.Add("SUB_FLOW_CODE", flow["SUB_FLOW_CODE"]);
                dic.Add("HAS_SUBFLOW", bool.Parse(flow["HAS_SUBFLOW"].ToString()));
                dic.Add("EXPIRE_DAY", new TimeSpan(DateTime.Parse(flow["MDF_DATE"].ToString()).AddDays(int.Parse(dic["ALERT_DAY"].ToString())).Ticks - DateTime.Now.Ticks).Days);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<eFormSetModel.DTO> ReadForms()
        {
            string strSql = @"select a.form_id,a.form_name from dbo.EFORM_SET a 
                                inner join dbo.MAP_EFORM_ORG b on b.org_id=@org_id";
            objParam = new
            {
                org_id = loggedUser.GetEmpOrg().ORG_ID
            };
            return ExecuteQuery<eFormSetModel.DTO>(strSql, objParam);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        public IList<IDictionary<string, object>> ReadFields(string formId)
        {
            string strSql = @"select c.field_id, c.field_name,c.field_type,c.field_exttype,c.field_size,c.field_option,c.memo,c.field_isfill,b.issplitter 
                                from dbo.EFORM_SET a inner join dbo.MAP_EFORM_EFIELD b on b.form_id = a.form_id inner join EFIELD_SET c on c.field_id = b.field_id
                                where a.form_id=@form_id order by b.sort_id";
            objParam = new
            {
                form_id = formId
            };
            return ExecuteQuery(strSql, objParam);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fillId"></param>
        /// <returns></returns>
        public string ReadDataFill(string fillId)
        {
            string strSql = @"select fill_data from dbo.EFORM_DATAFILL where del_flg=0 and fill_id=@fill_id";
            objParam = new
            {
                fill_id = fillId
            };
            return ExecuteQuery<string>(strSql, objParam).SingleOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel Create(FormCollection form)
        {
            string strSql = @"insert into dbo.EFORM_DATAFILL(fill_id,form_id,fill_data,crt_date,crt_user,mdf_date,mdf_user) 
                                values(@fill_id,@form_id,@fill_data,getdate(),@logged_user,getdate(),@logged_user)";
            objParam = new
            {
                fill_id = string.Format("FIL{0}{1}{2}{3}", loggedUser.GetEmpOrg().ORG_ID.Substring(0, 1).ToUpper(), loggedUser.USER_ID.Substring(0, 1).ToUpper(), DateTime.Now.ToString("yyMMdd"), DateTime.Now.Ticks.ToString().Substring(DateTime.Now.Ticks.ToString().Length - 7, 7)),
                form_id = form.Get("FORM_ID"),
                fill_data = form.Get("FILL_DATA"),
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            return new RtnResultModel(true, i18N.Message.R04);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel Update(FormCollection form)
        {
            Dictionary<string, string> dicForm = new Dictionary<string, string>();
            foreach (string key in form.Keys)
            {
                dicForm.Add(key, form[key]);
            }
            string strSql = @"update dbo.EFORM_DATAFILL set fill_data=@fill_data,mdf_date=getdate(),mdf_user=@logged_user where fill_id=@fill_id";
            objParam = new
            {
                fill_id = form.Get("FILL_ID"),
                fill_data = form.Get("FILL_DATA"),
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            return new RtnResultModel(true, i18N.Message.R05);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel UpdateFlow(FormCollection form)
        {
            string strSql = @"select isnull(flow_id,'') from dbo.MAP_EFORM_FLOW where form_id in (select form_id from dbo.EFORM_DATAFILL where fill_id=@fill_id)";
            objParam = new
            {
                fill_id = form.Get("FILL_ID")
            };
            string flowId = ExecuteQuery<string>(strSql, objParam).SingleOrDefault();
            if (!string.IsNullOrWhiteSpace(flowId))
            {
                var result = new FlowSet.Flow().SetActive(flowId, loggedUser.USER_ID);
                if (result.success)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        try
                        {
                            strSql = @"update dbo.EFORM_DATAFILL set flow_code=@flow_code,mdf_date=getdate(),mdf_user=@logged_user where fill_id=@fill_id;
                                insert into dbo.EFORM_FLOWLOG(flow_code,fill_id) values(@flow_code,@fill_id)";
                            objParam = new
                            {
                                flow_code = result.result[0]["flow_code"].ToString(),
                                fill_id = form.Get("FILL_ID"),
                                logged_user = loggedUser.USER_ID
                            };
                            ExecuteCommand(strSql, objParam);
                            ts.Complete();
                            return new RtnResultModel(result.success, result.message, result.result);
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
            }
            return new RtnResultModel(false, i18N.Message.R01);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel Delete(FormCollection form)
        {
            strSql = @"update dbo.EFORM_DATAFILL set del_flg=1,mdf_date=getdate(),mdf_user=@logged_user where fill_id=@fill_id";
            objParam = new
            {
                fill_id = form.Get("FILL_ID"),
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            return new RtnResultModel(true, i18N.Message.R06);
        }
    }
}


