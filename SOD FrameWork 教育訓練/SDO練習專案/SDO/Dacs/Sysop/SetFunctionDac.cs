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
    public class SetFunctionDac : _Dac
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public SetFunctionDac(EmpUserModel user) : base(user)
        {
        }

        /// <summary>
        /// Create SET_FUNCTION
        /// </summary>
        /// <param name="form"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public RtnResultModel Create(FormCollection form)
        {
            #region check EMP_USER exists
            strSql = @"select function_id,function_name,function_url,parent_id,sort_id,del_flg from dbo.SET_FUNCTION where function_id=@function_id";
            objParam = new
            {
                function_id = form.Get("FUNCTION_ID"),
            };
            SetFunctionModel model = ExecuteQuery<SetFunctionModel>(strSql, objParam).SingleOrDefault();
            if (model != default(SetFunctionModel) && !model.DEL_FLG)
            {
                return new RtnResultModel(false, string.Format(i18N.Message.R07, HttpUtility.HtmlEncode(form.Get("FUNCTION_ID"))));
            }
            #endregion
            strSql = (model != default(SetFunctionModel)) ?
                @"update dbo.SET_FUNCTION set del_flg=0,function_name=@function_name,function_url=@function_url,parent_id=@parent_id,mdf_date=getdate(),mdf_user=@logged_user where function_id=@function_id" :
                @"insert into dbo.SET_FUNCTION(function_id,function_name,function_url,parent_id,crt_date,crt_user,mdf_date,mdf_user) values(@function_id,@function_name,@function_url,@parent_id,getdate(),@logged_user,getdate(),@logged_user)";
            objParam = new
            {
                function_id = form.Get("FUNCTION_ID"),
                function_name = form.Get("FUNCTION_NAME"),
                function_url = form.Get("FUNCTION_URL") ?? "",
                parent_id = form.Get("PARENT_ID") ?? "",
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            return new RtnResultModel(true, i18N.Message.R04);
        }

        /// <summary>
        /// Get all SET_FUNCTIONs
        /// </summary>
        /// <returns></returns>
        public IList<SetFunctionModel> Read()
        {
            strSql = @"select function_id,function_name,function_url,parent_id,sort_id,del_flg from dbo.SET_FUNCTION where del_flg=0 order by sort_id";
            return ExecuteQuery<SetFunctionModel>(strSql);
        }

        /// <summary>
        /// Get SET_FUNCTION
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        public SetFunctionModel Read(string functionId)
        {
            strSql = @"select function_id,function_name,function_url,parent_id,sort_id,del_flg from dbo.SET_FUNCTION where del_flg=0 and function_id=@function_id order by sort_id";
            objParam = new
            {
                function_id = functionId
            };
            return ExecuteQuery<SetFunctionModel>(strSql, objParam).SingleOrDefault();
        }

        /// <summary>
        /// Get group's SET_FUNCTIONs
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public IList<SetFunctionModel> ReadByGroup(string parentId = "")
        {
            strSql = @"select function_id,function_name,function_url,parent_id,sort_id,del_flg from dbo.SET_FUNCTION where del_flg=0 and parent_id=@parent_id order by sort_id";
            objParam = new
            {
                parent_id = parentId
            };
            return ExecuteQuery<SetFunctionModel>(strSql, objParam);
        }

        /// <summary>
        /// Get EMP_USER's SET_FUNCTIONs
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IList<SetFunctionModel> ReadByUser(string userId)
        {
            strSql = @"select function_id,function_name,function_url,parent_id,sort_id,del_flg from dbo.SET_FUNCTION where isnull(parent_id,'')='' and del_flg=0
                        and function_id in (select parent_id from dbo.V_USER_FUNCTION where user_id=@user_id)
                        union 
                        select function_id,function_name,function_url,parent_id,sort_id,del_flg from dbo.V_USER_FUNCTION where user_id=@user_id";
            objParam = new
            {
                user_id = userId
            };
            return ExecuteQuery<SetFunctionModel>(strSql, objParam);
        }

        /// <summary>
        /// Get DIM_RIGHT's SET_FUNCTIONs
        /// </summary>
        /// <param name="rightId"></param>
        /// <returns></returns>
        public IList<SetFunctionModel> ReadByRight(string rightId = "")
        {
            strSql = @"select SET_FUNCTION.function_id,function_name,function_url,parent_id,sort_id,del_flg from dbo.SET_FUNCTION
                        inner join dbo.MAP_RIGHT_FUNCTION on MAP_RIGHT_FUNCTION.function_id=SET_FUNCTION.function_id
                        where dbo.SET_FUNCTION.del_flg=0 and MAP_RIGHT_FUNCTION.right_id=@right_id order by sort_id";
            objParam = new
            {
                right_id = rightId
            };
            return ExecuteQuery<SetFunctionModel>(strSql, objParam);
        }

        /// <summary>
        /// Update SET_FUNCTION
        /// </summary>
        /// <param name="form"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public RtnResultModel Update(FormCollection form)
        {
            strSql = @"update dbo.SET_FUNCTION set del_flg=0,function_name=@function_name,function_url=@function_url,parent_id=@parent_id,mdf_date=getdate(),mdf_user=@logged_user 
                        where function_id=@function_id";
            objParam = new
            {
                function_id = form.Get("FUNCTION_ID"),
                function_name = form.Get("FUNCTION_NAME"),
                function_url = form.Get("FUNCTION_URL") ?? "",
                parent_id = form.Get("PARENT_ID") ?? "",
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            return new RtnResultModel(true, i18N.Message.R05);
        }

        /// <summary>
        /// Update SET_FUNCTION's sortorder
        /// </summary>
        /// <param name="sortorder"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public RtnResultModel UpdateSortorder(string[] sortData)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    int sortId = 1;
                    strSql = @"update dbo.SET_FUNCTION set sort_id=@sort_id,mdf_date=getdate(),mdf_user=@logged_user where function_id=@function_id";
                    objParam = new ArrayList();
                    foreach (string functionId in sortData)
                    {
                        ((ArrayList)objParam).Add(new
                        {
                            sort_id = sortId,
                            function_id = functionId,
                            logged_user = loggedUser.USER_ID
                        });
                        sortId++;
                    }
                    ExecuteCommand(strSql, ((ArrayList)objParam).ToArray());
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
        /// Delete SET_FUNCTIONs
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
                    strSql = @"update dbo.SET_FUNCTION set del_flg=1,mdf_date=getdate(),mdf_user=@logged_user where function_id=@function_id";
                    objParam = new ArrayList();
                    foreach (string functionId in form.Get("IDS").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        ((ArrayList)objParam).Add(new
                        {
                            function_id = functionId,
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