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
    public class DimRoleDac : _Dac
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public DimRoleDac(EmpUserModel user) : base(user)
        {
        }

        /// <summary>
        /// Create DIM_ROLE
        /// </summary>
        /// <param name="form"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public RtnResultModel Create(FormCollection form)
        {
            #region check DIM_ROLE exists
            strSql = @"select role_id,role_name,del_flg from dbo.DIM_ROLE where 1=1 and role_id=@role_id";
            objParam = new
            {
                role_id = form.Get("ROLE_ID")
            };
            DimRoleModel model = ExecuteQuery<DimRoleModel>(strSql, objParam).SingleOrDefault();
            if (model != default(DimRoleModel) && !model.DEL_FLG)
            {
                return new RtnResultModel(false, string.Format(i18N.Message.R07, HttpUtility.HtmlEncode(form.Get("ROLE_ID"))));
            }
            #endregion
            strSql = (model != default(DimRoleModel)) ?
                @"update dbo.DIM_ROLE set del_flg=0,role_name=@role_name,mdf_date=getdate(),mdf_user=@logged_user where role_id=@role_id" :
                @"insert into dbo.DIM_ROLE(role_id,role_name,crt_date,crt_user,mdf_date,mdf_user) values(@role_id,@role_name,getdate(),@logged_user,getdate(),@logged_user)";
            objParam = new
            {
                role_id = form.Get("ROLE_ID"),
                role_name = form.Get("ROLE_NAME"),
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            UpdateMapRightFunction(form);
            return new RtnResultModel(true, i18N.Message.R04);
        }

        /// <summary>
        /// Get all DIM_ROLEs
        /// </summary>
        /// <returns></returns>
        public IList<DimRoleModel> Read()
        {
            strSql = @"select role_id,role_name,del_flg from DIM_ROLE where del_flg=0";
            return ExecuteQuery<DimRoleModel>(strSql);
        }

        /// <summary>
        /// Get DIM_ROLE
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public DimRoleModel Read(string roleId)
        {
            strSql = @"select role_id,role_name,del_flg from dbo.DIM_ROLE where del_flg=0 and role_id=@role_id";
            objParam = new
            {
                role_id = roleId
            };
            return ExecuteQuery<DimRoleModel>(strSql, objParam).SingleOrDefault();
        }

        /// <summary>
        /// Get EMP_USER's DIM_ROLEs 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<DimRoleModel> ReadByUser(string userId = "")
        {
            strSql = @"select DIM_ROLE.role_id,role_name,del_flg from dbo.DIM_ROLE
                        inner join dbo.MAP_USER_ROLE on MAP_USER_ROLE.role_id=DIM_ROLE.role_id
                        where DIM_ROLE.del_flg=0 and MAP_USER_ROLE.user_id=@user_id";
            objParam = new
            {
                user_id = userId
            };
            return ExecuteQuery<DimRoleModel>(strSql, objParam);
        }

        /// <summary>
        /// Update DIM_ROLE
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
                    strSql = @"update dbo.DIM_ROLE set del_flg=0,role_name=@role_name,mdf_date=getdate(),mdf_user=@logged_user where role_id=@role_id";
                    objParam = new
                    {
                        role_id = form.Get("ROLE_ID"),
                        role_name = form.Get("ROLE_NAME"),
                        logged_user = loggedUser.USER_ID
                    };
                    ExecuteCommand(strSql, objParam);
                    UpdateMapRightFunction(form);
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
        /// update MAP_ROLE_RIGHT
        /// </summary>
        /// <param name="form"></param>
        private void UpdateMapRightFunction(FormCollection form)
        {
            strSql = @"delete from dbo.MAP_ROLE_RIGHT where role_id=@role_id";
            objParam = new
            {
                role_id = form.Get("ROLE_ID"),
            };
            ExecuteCommand(strSql, objParam);
            strSql = @"insert into dbo.MAP_ROLE_RIGHT(role_id,right_id,crt_date,crt_user) 
                        values(@role_id,@right_id,getdate(),@logged_user)";
            objParam = new ArrayList();
            foreach (string rightId in (form.Get("RIGHTS") ?? "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                ((ArrayList)objParam).Add(new
                {
                    role_id = form.Get("ROLE_ID"),
                    right_id = rightId,
                    logged_user = loggedUser.USER_ID
                });
            }
            ExecuteCommand(strSql, ((ArrayList)objParam).ToArray());
        }

        /// <summary>
        /// Delete DIM_ROLEs
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
                    strSql = @"update dbo.DIM_ROLE set del_flg=1,mdf_date=getdate(),mdf_user=@logged_user where role_id=@role_id;
                                delete from dbo.MAP_ROLE_RIGHT where role_id=@role_id";
                    objParam = new ArrayList();
                    foreach (string roleId in form.Get("IDS").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        ((ArrayList)objParam).Add(new
                        {
                            role_id = roleId,
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