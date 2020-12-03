using SDO.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace SDO.Dacs
{
    public class EmpAgentDac : _Dac
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public EmpAgentDac(EmpUserModel user) : base(user)
        {
        }

        /// <summary>
        /// Create EMP_USER
        /// </summary>
        /// <param name="form"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public RtnResultModel Create(FormCollection form)
        {
            strSql = @"select sid,user_id,agent_id,agent_from,agent_to,del_flg from dbo.EMP_AGENT where del_flg=0 and user_id=@user_id 
                        and ((agent_from between @agent_from and dateadd(day,1,@agent_to)) or (agent_to between @agent_from and dateadd(day,1,@agent_to)))";
            objParam = new
            {
                user_id = form.Get("USER_ID"),
                agent_from = DateTime.Parse(form.Get("AGENT_FROM")),
                agent_to = DateTime.Parse(form.Get("AGENT_TO"))
            };
            IList<EmpAgentModel> models = ExecuteQuery<EmpAgentModel>(strSql, objParam);
            if (models.Count > 0)
            {
                return new RtnResultModel(false, string.Format(i18N.Message.R10, models.Min(p => p.AGENT_FROM_TWDATE), models.Max(p => p.AGENT_TO_TWDATE)));
            }
            else
            {
                strSql = @"insert into dbo.EMP_AGENT(user_id,agent_id,agent_from,agent_to,crt_date,crt_user,mdf_date,mdf_user)
                            values(@user_id,@agent_id,@agent_from,@agent_to,getdate(),@logged_user,getdate(),@logged_user)";
                objParam = new
                {
                    user_id = form.Get("USER_ID"),
                    agent_id = form.Get("AGENT_ID"),
                    agent_from = DateTime.Parse(form.Get("AGENT_FROM")),
                    agent_to = DateTime.Parse(form.Get("AGENT_TO")),
                    logged_user = loggedUser.USER_ID
                };
                ExecuteCommand(strSql, objParam);
            }
            return new RtnResultModel(true, i18N.Message.R04);
        }

        /// <summary>
        /// Get all EMP_AGENTs
        /// </summary>
        /// <returns></returns>
        public IList<EmpAgentModel> Read()
        {
            strSql = @"select sid,EMP_AGENT.user_id,agent_id,agent_from,agent_to,EMP_AGENT.del_flg, user_name as AGENT_NAME from dbo.EMP_AGENT
                        inner join dbo.EMP_USER on EMP_USER.user_id=EMP_AGENT.agent_id
                        where getdate() between agent_from and dateadd(day,1,agent_to) and EMP_AGENT.del_flg=0";
            return ExecuteQuery<EmpAgentModel>(strSql);
        }

        /// <summary>
        /// Delete EMP_AGENTs
        /// </summary>
        /// <param name="form"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public RtnResultModel Delete(FormCollection form)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    strSql = @"update dbo.EMP_AGENT set del_flg=1,mdf_date=getdate(),mdf_user=@logged_user where sid=@sid";
                    objParam = new ArrayList();
                    foreach (string sId in form.Get("IDS").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        ((ArrayList)objParam).Add(new
                        {
                            sid = sId,
                            logged_user = loggedUser.USER_ID
                        });
                    }
                    ExecuteCommand(strSql, ((ArrayList)objParam).ToArray());
                    scope.Complete();
                    return new RtnResultModel(true, i18N.Message.R06);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}