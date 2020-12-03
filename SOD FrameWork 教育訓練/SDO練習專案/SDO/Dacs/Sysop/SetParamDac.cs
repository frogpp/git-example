using SDO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SDO.Dacs
{
    public class SetParamDac : _Dac
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public SetParamDac(EmpUserModel user = null) : base(user)
        {
        }

        /// <summary>
        /// creae SET_PARAMITEM
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel CreateItem(FormCollection form)
        {
            #region check exists
            strSql = @"select set_item,set_item_name,memo,editable,del_flg from dbo.SET_PARAMITEM where 1=1 and set_item=@set_item";
            objParam = new
            {
                set_item = form.Get("SET_ITEM")
            };
            SetParamItemModel model = ExecuteQuery<SetParamItemModel>(strSql, objParam).SingleOrDefault();
            if (model != default(SetParamItemModel) && (!model.DEL_FLG || !model.EDITABLE))
            {
                return new RtnResultModel(false, string.Format(i18N.Message.R07, HttpUtility.HtmlEncode(form.Get("SET_ITEM"))));
            }
            #endregion
            strSql = (model != default(SetParamItemModel)) ?
                @"update dbo.SET_PARAMITEM set del_flg=0,set_item_name=@set_item_name,memo=@memo,mdf_date=getdate(),mdf_user=@logged_user where set_item=@set_item" :
                @"insert into dbo.SET_PARAMITEM(set_item,set_item_name,memo,crt_date,crt_user,mdf_date,mdf_user) values(@set_item,@set_item_name,@memo,getdate(),@logged_user,getdate(),@logged_user)";
            objParam = new
            {
                set_item = form.Get("SET_ITEM"),
                set_item_name = form.Get("SET_ITEM_NAME"),
                memo = form.Get("MEMO"),
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            return new RtnResultModel(true, i18N.Message.R04);
        }

        /// <summary>
        /// creae SET_PARAM
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel Create(FormCollection form)
        {
            #region check exists
            strSql = @"select set_item,set_type,set_value,memo,del_flg from dbo.SET_PARAM where set_item=@set_item and set_type=@set_type";
            objParam = new
            {
                set_item = form.Get("SET_ITEM"),
                set_type = form.Get("SET_TYPE")
            };
            SetParamModel model = ExecuteQuery<SetParamModel>(strSql, objParam).SingleOrDefault();
            if (model != default(SetParamModel) && !model.DEL_FLG)
            {
                return new RtnResultModel(false, string.Format(i18N.Message.R07, string.Format("{0}-{1}", HttpUtility.HtmlEncode(form.Get("SET_ITEM")), HttpUtility.HtmlEncode(form.Get("SET_TYPE")))));
            }
            #endregion
            strSql = (model != default(SetParamModel)) ?
                @"update dbo.SET_PARAM set del_flg=0,set_value=@set_value,memo=@memo,mdf_date=getdate(),mdf_user=@logged_user where set_item=@set_item and set_type=@set_type" :
                @"insert into dbo.SET_PARAM(set_item,set_type,set_value,memo,crt_date,crt_user,mdf_date,mdf_user) values(@set_item,@set_type,@set_value,@memo,getdate(),@logged_user,getdate(),@logged_user)";
            objParam = new
            {
                set_item = form.Get("SET_ITEM"),
                set_type = form.Get("SET_TYPE"),
                set_value = form.Get("SET_VALUE"),
                memo = form.Get("MEMO"),
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            return new RtnResultModel(true, i18N.Message.R04);
        }

        /// <summary>
        /// Get All SET_PARAMITEM
        /// </summary>
        /// <returns></returns>
        public IList<SetParamItemModel> ReadItem()
        {
            string strSql = @"select set_item,set_item_name,memo,editable from dbo.SET_PARAMITEM where del_flg=0";
            return ExecuteQuery<SetParamItemModel>(strSql);
        }

        /// <summary>
        /// Get SET_PARAMITEM
        /// </summary>
        /// <param name="setItem"></param>
        /// <returns></returns>
        public SetParamItemModel ReadItem(string setItem)
        {
            string strSql = @"select set_item,set_item_name,memo,editable from dbo.SET_PARAMITEM where del_flg=0 and set_item=@set_item";
            objParam = new
            {
                set_item = setItem
            };
            return ExecuteQuery<SetParamItemModel>(strSql, objParam).SingleOrDefault();
        }

        /// <summary>
        /// Get All SET_PARAM
        /// </summary>
        /// <returns></returns>
        public IList<SetParamModel> Read()
        {
            string strSql = @"select set_item,set_type,set_value,memo from dbo.SET_PARAM where del_flg=0";
            return ExecuteQuery<SetParamModel>(strSql);
        }

        /// <summary>
        /// Get SET_ITEM=?
        /// </summary>
        /// <param name="setItem"></param>
        /// <returns></returns>
        public IList<SetParamModel> Read(string setItem)
        {
            string strSql = @"select a.set_item,a.set_type,a.set_value,a.memo,b.editable 
                                from dbo.SET_PARAM a inner join dbo.SET_PARAMITEM b on b.set_item=a.set_item 
                                where a.del_flg=0 and a.set_item=@set_item";
            objParam = new
            {
                set_item = setItem
            };
            return ExecuteQuery<SetParamModel>(strSql, objParam);
        }

        /// <summary>
        /// Get SET_PARAM
        /// </summary>
        /// <param name="setItem"></param>
        /// <param name="set_type"></param>
        /// <returns></returns>
        public SetParamModel Read(string setItem, string set_type)
        {
            string strSql = @"select set_item,set_type,set_value,memo from dbo.SET_PARAM where del_flg=0 and set_item=@set_item and set_type=@set_type";
            objParam = new
            {
                set_item = setItem,
                set_type = set_type
            };
            return ExecuteQuery<SetParamModel>(strSql, objParam).SingleOrDefault();
        }

        /// <summary>
        /// update SET_PARAMITEM
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel UpdateItem(FormCollection form)
        {
            strSql = @"update dbo.SET_PARAMITEM set del_flg=0,set_item_name=@set_item_name,memo=@memo,mdf_date=getdate(),mdf_user=@logged_user where set_item=@set_item";
            objParam = new
            {
                set_item = form.Get("SET_ITEM"),
                set_item_name = form.Get("SET_ITEM_NAME"),
                memo = form.Get("MEMO"),
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            return new RtnResultModel(true, i18N.Message.R05);
        }

        /// <summary>
        /// Update SET_PARAM
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel Update(FormCollection form)
        {
            strSql = @"update dbo.SET_PARAM set del_flg=0,set_value=@set_value,memo=@memo,mdf_date=getdate(),mdf_user=@logged_user where set_item=@set_item and set_type=@set_type";
            objParam = new
            {
                set_item = form.Get("SET_ITEM"),
                set_type = form.Get("SET_TYPE"),
                set_value = form.Get("SET_VALUE"),
                memo = form.Get("MEMO"),
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            return new RtnResultModel(true, i18N.Message.R05);
        }

        /// <summary>
        /// Delete SET_PARAMITEM
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel DeleteItem(FormCollection form)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    strSql = @"update dbo.SET_PARAMITEM set del_flg=1,mdf_date=getdate(),mdf_user=@logged_user where set_item=@set_item;
                                update dbo.SET_PARAM set del_flg=1,mdf_date=getdate(),mdf_user=@logged_user where set_item=@set_item";
                    objParam = new
                    {
                        set_item = form.Get("SET_ITEM"),
                        logged_user = loggedUser.USER_ID
                    };
                    ExecuteCommand(strSql, objParam);
                    scope.Complete();
                    return new RtnResultModel(true, i18N.Message.R06);
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete SET_PARAM
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel Delete(FormCollection form)
        {
            strSql = @"update dbo.SET_PARAM set del_flg=1,mdf_date=getdate(),mdf_user=@logged_user where set_item=@set_item and set_type=@set_type";
            objParam = new
            {
                set_item = form.Get("SET_ITEM"),
                set_type = form.Get("SET_TYPE"),
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            return new RtnResultModel(true, i18N.Message.R06);
        }
    }
}