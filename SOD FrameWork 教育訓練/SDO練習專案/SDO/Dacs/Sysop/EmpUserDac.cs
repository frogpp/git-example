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
    public class EmpUserDac : _Dac
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public EmpUserDac(EmpUserModel user) : base(user)
        {
        }

        /// <summary>
        /// Get all EMP_USERs
        /// </summary>
        /// <returns></returns>
        public IList<EmpUserModel> Read()
        {
            strSql = @"select user_id,user_name,user_email,del_flg from dbo.EMP_USER where del_flg=0";
            return ExecuteQuery<EmpUserModel>(strSql);
        }

        /// <summary>
        ///  Get EMP_USERs by query
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public IList<EmpUserModel> Read(FormCollection form)
        {
            strSql = @"select user_id,user_name,user_email,del_flg from dbo.EMP_USER where del_flg=0";
            if (!string.IsNullOrWhiteSpace(form.Get("qryUSER_ID")) || !string.IsNullOrWhiteSpace(form.Get("qryUSER_NAME")))
            {
                if (!string.IsNullOrWhiteSpace(form.Get("qryUSER_ID")))
                {
                    strSql += @" and (user_id like @user_id";
                }
                if (!string.IsNullOrWhiteSpace(form.Get("qryUSER_NAME")))
                {
                    if (!string.IsNullOrWhiteSpace(form.Get("qryUSER_ID")))
                    {
                        strSql += @" or user_name like @user_name";
                    }
                    else
                    {
                        strSql += @" and (user_name like @user_name";
                    }
                }
                strSql += ")";
            }
            objParam = new
            {
                user_id = string.Format("{0}%", HttpUtility.HtmlEncode(form.Get("qryUSER_ID"))),
                user_name = string.Format("{0}%", HttpUtility.HtmlEncode(form.Get("qryUSER_NAME")))
            };
            return ExecuteQuery<EmpUserModel>(strSql, objParam);
        }

        /// <summary>
        /// Get EMP_USER
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public EmpUserModel Read(string userId, string userPwd = "")
        {
            strSql = @"select user_id,user_name,user_email,del_flg from dbo.EMP_USER where del_flg=0 and user_id=@user_id";
            if (!string.IsNullOrWhiteSpace(userPwd)) {
                strSql += @" and user_pwd=@user_pwd"; 
            }
            objParam = new
            {
                user_id = userId,
                user_pwd = userPwd
            };
            return ExecuteQuery<EmpUserModel>(strSql, objParam).SingleOrDefault();
        }

        /// <summary>
        /// Get multiple EMP_USER
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public IList<EmpUserModel> Read(string[] userIds)
        {
            strSql = @"select user_id,user_name,user_email,del_flg from dbo.EMP_USER where del_flg=0 and user_id in @user_id";
            objParam = new
            {
                user_id = userIds
            };
            return ExecuteQuery<EmpUserModel>(strSql, objParam);
        }

        /// <summary>
        /// Get EMP_AGENT's EMP_USERs
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<EmpUserModel> ReadByAgent(string agentId)
        {
            strSql = @"select EMP_USER.user_id,user_name,user_email,EMP_USER.del_flg from dbo.EMP_USER
                        inner join dbo.EMP_AGENT on EMP_AGENT.user_id=EMP_USER.user_id
                        where EMP_USER.del_flg=0 and EMP_AGENT.del_flg=0 and EMP_AGENT.agent_id=@agent_id and getdate() between EMP_AGENT.agent_from and dateadd(day,1,EMP_AGENT.agent_to)";
            objParam = new
            {
                agent_id = agentId
            };
            return ExecuteQuery<EmpUserModel>(strSql, objParam);
        }

        /// <summary>
        /// Get EMP_ORG's EMP_USERs
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public IList<EmpUserModel> ReadByOrg(string orgId = "")
        {
            strSql = @"select EMP_USER.user_id,user_name,user_email,del_flg from dbo.EMP_USER
                        inner join dbo.MAP_ORG_USER on MAP_ORG_USER.user_id=EMP_USER.user_id
                        where EMP_USER.del_flg=0 and MAP_ORG_USER.org_id=@org_id";
            objParam = new
            {
                org_id = orgId
            };
            return ExecuteQuery<EmpUserModel>(strSql, objParam);
        }

        /// <summary>
        /// Update EMP_USER
        /// </summary>
        /// <param name="form"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public RtnResultModel Update(FormCollection form)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    strSql = @"update dbo.EMP_USER set del_flg=0,user_name=@user_name,user_email=@user_email,mdf_date=getdate(),mdf_user=@logged_user where user_id=@user_id";
                    objParam = new
                    {
                        user_id = form.Get("USER_ID"),
                        user_name = form.Get("USER_NAME"),
                        user_email = form.Get("USER_EMAIL"),
                        logged_user = loggedUser.USER_ID
                    };
                    ExecuteCommand(strSql, objParam);
                    if (!string.IsNullOrWhiteSpace(form.Get("USER_PWD")))
                    {
                        strSql = @"update dbo.EMP_USER set user_pwd=@user_pwd where user_id=@user_id";
                        objParam = new
                        {
                            user_pwd = form.Get("USER_PWD"),
                            user_id = form.Get("USER_ID")
                        };
                        ExecuteCommand(strSql, objParam);
                    }
                    UpdateMapUserRole(form);
                    scope.Complete();
                    return new RtnResultModel(true, i18N.Message.R05);
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// update MAP_USER_ROLE
        /// </summary>
        /// <param name="form"></param>
        private void UpdateMapUserRole(FormCollection form)
        {
            strSql = @"delete from dbo.MAP_USER_ROLE where user_id=@user_id";
            objParam = new
            {
                user_id = form.Get("USER_ID"),
            };
            ExecuteCommand(strSql, objParam);
            strSql = @"insert into dbo.MAP_USER_ROLE(user_id,role_id,crt_date,crt_user)
                        values(@user_id,@role_id,getdate(),@logged_user)";
            objParam = new ArrayList();
            foreach (string roleId in (form.Get("ROLES") ?? "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                ((ArrayList)objParam).Add(new
                {
                    user_id = form.Get("USER_ID"),
                    role_id = roleId,
                    logged_user = loggedUser.USER_ID
                });
            }
            ExecuteCommand(strSql, ((ArrayList)objParam).ToArray());
        }

        /// <summary>
        /// Delete EMP_USERs
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
                    strSql = @"update dbo.EMP_USER set del_flg=1,mdf_date=getdate(),mdf_user=@logged_user where user_id=@user_id;
                                delete from dbo.MAP_USER_ROLE where user_id=@user_id";
                    objParam = new ArrayList();
                    foreach (string userId in form.Get("IDS").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        ((ArrayList)objParam).Add(new
                        {
                            user_id = userId,
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