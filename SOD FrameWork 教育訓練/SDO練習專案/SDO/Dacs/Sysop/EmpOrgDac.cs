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
    public class EmpOrgDac : _Dac
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public EmpOrgDac(EmpUserModel user) : base(user)
        {
        }

        /// <summary>
        /// Create EMP_ORG
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel Create(FormCollection form)
        {
            #region check EMP_ORG exists
            strSql = @"select org_id,org_name,parent_id,del_flg from dbo.EMP_ORG where 1=1 and org_id=@org_id";
            objParam = new
            {
                org_id = form.Get("ORG_ID")
            };
            EmpOrgModel model = ExecuteQuery<EmpOrgModel>(strSql, objParam).SingleOrDefault();
            if (model != default(EmpOrgModel) && !model.DEL_FLG)
            {
                return new RtnResultModel(false, string.Format(i18N.Message.R07, HttpUtility.HtmlEncode(form.Get("ORG_ID"))));
            }
            #endregion
            strSql = (model != default(EmpOrgModel)) ?
                @"update dbo.EMP_ORG set del_flg=0,org_name=@org_name,paren_id=@paren_id,mdf_date=getdate(),mdf_user=@logged_user where org_id=@org_id" :
                @"insert into dbo.EMP_ORG(org_id,org_name,parent_id,crt_date,crt_user,mdf_date,mdf_user) values(@org_id,@org_name,@paren_id,getdate(),@logged_user,getdate(),@logged_user)";
            objParam = new
            {
                org_id = form.Get("ORG_ID"),
                org_name = form.Get("ORG_NAME"),
                paren_id = form.Get("PARENT_ID"),
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            UpdateMapOrgUser(form);
            return new RtnResultModel(true, i18N.Message.R04);
        }

        /// <summary>
        /// Get all EMP_ORGs
        /// </summary>
        /// <returns></returns>
        public IList<EmpOrgModel> Read()
        {
            strSql = @"with cteOrg(org_id,org_name,parent_id,del_flg) as
                        (select org_id,org_name,parent_id,del_flg from dbo.EMP_ORG where del_flg=0 and parent_id=''
                        union all
                        select sub.org_id,sub.org_name,sub.parent_id,sub.del_flg from dbo.EMP_ORG sub
                            inner join cteOrg on sub.parent_id=cteOrg.org_id where sub.del_flg=0)
                        select org_id,org_name,parent_id,del_flg from cteOrg";
            return ExecuteQuery<EmpOrgModel>(strSql);
        }

        /// <summary>
        ///  Get EMP_ORG's child EMP_ORGs
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public IList<EmpOrgModel> ReadByOrg(string orgId)
        {
            strSql = @"with cteOrg(org_id,org_name,parent_id,del_flg) as
                        (select org_id,org_name,parent_id,del_flg from dbo.EMP_ORG where del_flg=0 and org_id=@org_id
                        union all
                        select sub.org_id,sub.org_name,sub.parent_id,sub.del_flg from dbo.EMP_ORG sub
                            inner join cteOrg on sub.parent_id=cteOrg.org_id where sub.del_flg=0)
                        select org_id,org_name,parent_id,del_flg from cteOrg";
            objParam = new
            {
                org_id = orgId
            };
            return ExecuteQuery<EmpOrgModel>(strSql, objParam);
        }

        /// <summary>
        /// Get EMP_ORG by parent org id
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public IList<EmpOrgModel> ReadByParentOrg(string orgId)
        {
            strSql = @"select org_id,org_name,parent_id,del_flg,cast((select case count(0) when 0 then 0 else 1 end from dbo.EMP_ORG where parent_id=a.org_id) as bit) as hasChildren 
                        from dbo.EMP_ORG a where del_flg=0 and parent_id=@parent_id";
            objParam = new
            {
                parent_id = orgId
            };
            return ExecuteQuery<EmpOrgModel>(strSql, objParam);
        }

        /// <summary>
        /// Get EMP_ORG
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public EmpOrgModel Read(string orgId)
        {
            strSql = @"select org_id,org_name,parent_id,del_flg from dbo.EMP_ORG where del_flg=0 and org_id=@org_id";
            objParam = new
            {
                org_id = orgId
            };
            return ExecuteQuery<EmpOrgModel>(strSql, objParam).SingleOrDefault();
        }

        /// <summary>
        /// Get multiple EMP_ORG
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<EmpOrgModel> Read(string[] orgIds)
        {
            strSql = @"select org_id,org_name,parent_id,del_flg from dbo.EMP_ORG where del_flg=0 and org_id in @org_id";
            objParam = new
            {
                org_id = orgIds
            };
            return ExecuteQuery<EmpOrgModel>(strSql, objParam);
        }

        /// <summary>
        /// Get EMP_User's EMP_ORG
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public EmpOrgModel ReadByUser(string userId)
        {
            strSql = @"select org_id,org_name,parent_id,del_flg from dbo.V_USER_ORG where del_flg=0 and user_id=@user_id";
            objParam = new
            {
                user_id = userId
            };
            return ExecuteQuery<EmpOrgModel>(strSql, objParam).SingleOrDefault();
        }

        /// <summary>
        /// Update EMP_ORG
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel Update(FormCollection form)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    strSql = @"update dbo.EMP_ORG set del_flg=0,org_name=@org_name,parent_id=@parent_id,mdf_date=getdate(),mdf_user=@logged_user where org_id=@org_id";
                    objParam = new
                    {
                        org_id = form.Get("ORG_ID"),
                        org_name = form.Get("ORG_NAME"),
                        parent_id = form.Get("PARENT_ID"),
                        logged_user = loggedUser.USER_ID
                    };
                    ExecuteCommand(strSql, objParam);
                    UpdateMapOrgUser(form);
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
        /// update MAP_ORG_USER
        /// </summary>
        /// <param name="form"></param>
        private void UpdateMapOrgUser(FormCollection form)
        {
            strSql = @"delete from dbo.MAP_ORG_USER where org_id=@org_id";
            objParam = new
            {
                org_id = form.Get("ORG_ID"),
            };
            ExecuteCommand(strSql, objParam);
            strSql = @"insert into dbo.MAP_ORG_USER(org_id,user_id,crt_date,crt_user)
                        values(@org_id,@user_id,getdate(),@logged_user)";
            objParam = new ArrayList();
            foreach (string userId in (form.Get("USERS") ?? "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                ((ArrayList)objParam).Add(new
                {
                    org_id = form.Get("ORG_ID"),
                    user_id = userId,
                    logged_user = loggedUser.USER_ID
                });
            }
            ExecuteCommand(strSql, ((ArrayList)objParam).ToArray());
        }

        /// <summary>
        /// Delete EMP_ORGs
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel Delete(FormCollection form)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    strSql = @"update dbo.EMP_ORG set del_flg=1,mdf_date=getdate(),mdf_user=@logged_user where org_id=@org_id;
                                delete from dbo.MAP_ORG_USER where org_id=@org_id";
                    objParam = new ArrayList();
                    foreach (string orgId in form.Get("IDS").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        ((ArrayList)objParam).Add(new
                        {
                            org_id = orgId,
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