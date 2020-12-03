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
    public class DimRightDac : _Dac
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public DimRightDac(EmpUserModel user) : base(user)
        {
        }

        /// <summary>
        /// Create DIM_RIGHT
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel Create(FormCollection form)
        {
            #region check DIM_RIGHT exists
            strSql = @"select right_id,right_name,del_flg from dbo.DIM_RIGHT where 1=1 and right_id=@right_id";
            objParam = new
            {
                right_id = form.Get("RIGHT_ID")
            };
            DimRightModel model = ExecuteQuery<DimRightModel>(strSql, objParam).SingleOrDefault();
            if (model != default(DimRightModel) && !model.DEL_FLG)
            {
                return new RtnResultModel(false, string.Format(i18N.Message.R07, HttpUtility.HtmlEncode(form.Get("RIGHT_ID"))));
            }
            #endregion
            strSql = (model != default(DimRightModel)) ?
                @"update dbo.DIM_RIGHT set del_flg=0,right_name=@right_name,mdf_date=getdate(),mdf_user=@logged_user where right_id=@right_id" :
                @"insert into dbo.DIM_RIGHT(right_id,right_name,crt_date,crt_user,mdf_date,mdf_user) values(@right_id,@right_name,getdate(),@logged_user,getdate(),@logged_user)";
            objParam = new
            {
                right_id = form.Get("RIGHT_ID"),
                right_name = form.Get("RIGHT_NAME"),
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            UpdateMapRightFunction(form);
            return new RtnResultModel(true, i18N.Message.R04);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<DimRightModel> Read()
        {
            strSql = @"select right_id,right_name,del_flg from dbo.DIM_RIGHT where del_flg=0";
            return ExecuteQuery<DimRightModel>(strSql);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rightId"></param>
        /// <returns></returns>
        public DimRightModel Read(string rightId)
        {
            strSql = @"select right_id,right_name,del_flg from dbo.DIM_RIGHT where del_flg=0 and right_id=@right_id";
            objParam = new
            {
                right_id = rightId
            };
            return ExecuteQuery<DimRightModel>(strSql, objParam).SingleOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IList<DimRightModel> ReadByRole(string roleId = "")
        {
            strSql = @"select DIM_RIGHT.right_id,right_name,del_flg from dbo.DIM_RIGHT 
                        inner join dbo.MAP_ROLE_RIGHT on MAP_ROLE_RIGHT.right_id=DIM_RIGHT.right_id
                        where DIM_RIGHT.del_flg=0 and MAP_ROLE_RIGHT.role_id=@role_id";
            objParam = new
            {
                role_id = roleId
            };
            return ExecuteQuery<DimRightModel>(strSql, objParam);
        }

        /// <summary>
        /// Update DIM_RIGHT
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
                    strSql = @"update dbo.DIM_RIGHT set del_flg=0,right_name=@right_name,mdf_date=getdate(),mdf_user=@logged_user where right_id=@right_id";
                    objParam = new
                    {
                        right_id = form.Get("RIGHT_ID"),
                        right_name = form.Get("RIGHT_NAME"),
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
        /// update MAP_RIGHT_FUNCTION 
        /// </summary>
        /// <param name="form"></param>
        private void UpdateMapRightFunction(FormCollection form)
        {
            strSql = @"delete from dbo.MAP_RIGHT_FUNCTION where right_id=@right_id";
            objParam = new
            {
                right_id = form.Get("RIGHT_ID"),
            };
            ExecuteCommand(strSql, objParam);
            strSql = @"insert into dbo.MAP_RIGHT_FUNCTION(right_id,function_id,crt_date,crt_user) 
                        values(@right_id,@function_id,getdate(),@logged_user)";
            objParam = new ArrayList();
            foreach (string functionId in (form.Get("FUNCTIONS") ?? "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                ((ArrayList)objParam).Add(new
                {
                    right_id = form.Get("RIGHT_ID"),
                    function_id = functionId,
                    logged_user = loggedUser.USER_ID
                });
            }
            ExecuteCommand(strSql, ((ArrayList)objParam).ToArray());
        }

        /// <summary>
        /// Delete DIM_RIGHTs
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
                    strSql = @"update dbo.DIM_RIGHT set del_flg=1,mdf_date=getdate(),mdf_user=@logged_user where right_id=@right_id;
                                delete from dbo.MAP_RIGHT_FUNCTION where right_id=@right_id";
                    objParam = new ArrayList();
                    foreach (string rightId in form.Get("IDS").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        ((ArrayList)objParam).Add(new
                        {
                            right_id = rightId,
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