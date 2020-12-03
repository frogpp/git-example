using SDO.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace SDO.Dacs
{
    public class AnnouncementDac : _Dac
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public AnnouncementDac(EmpUserModel user) : base(user)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="attachFile"></param>
        /// <returns></returns>
        public RtnResultModel Create(FormCollection form)
        {
            strSql = @"insert into ANNOUNCEMENT(title,comment,effective_date,expire_date,attach_name,crt_date,crt_user,mdf_date,mdf_user)
                        values(@title,@comment,@effective_date,@expire_date,@attach_name,getdate(),@logged_user,getdate(),@logged_user)";
            objParam = new
            {
                title = form.Get("TITLE"),
                comment = form.Get("COMMENT"),
                effective_date = form.Get("EFFECTIVE_DATE"),
                expire_date = DateTime.Parse(form.Get("EXPIRE_DATE")),
                attach_name = form.Get("ATTACH_NAME"),
                logged_user = loggedUser.USER_ID
            };
            ExecuteCommand(strSql, objParam);
            return new RtnResultModel(true, i18N.Message.R04);
        }

        /// <summary>
        /// Get Announcement
        /// </summary>
        /// <returns></returns>
        public IList<AnnouncementModel> Read()
        {
            strSql = @"select sid,title,comment,effective_date,expire_date,attach_name from dbo.ANNOUNCEMENT where del_flg=0 order by effective_date desc";
            return ExecuteQuery<AnnouncementModel>(strSql, objParam);
        }

        /// <summary>
        /// Get Announcement by sid
        /// </summary>
        /// <returns></returns>
        public AnnouncementModel Read(int id)
        {
            strSql = @"select sid,title,comment,effective_date,expire_date,attach_name from dbo.ANNOUNCEMENT where sid=@sid and del_flg=0";
            objParam = new
            {
                sid = id,   
            };
            return ExecuteQuery<AnnouncementModel>(strSql, objParam).SingleOrDefault();
        }

        /// <summary>
        /// Get Announce to display
        /// </summary>
        /// <returns></returns>
        public IList<AnnouncementModel> ReadDisplay()
        {
            strSql = @"select sid,title,comment,effective_date,expire_date,attach_name from dbo.ANNOUNCEMENT where del_flg=0 and getdate() between effective_date and dateadd(day,1,expire_date) order by effective_date desc";
            return ExecuteQuery<AnnouncementModel>(strSql, objParam);
        }

        /// <summary>
        /// update Announcement
        /// </summary>
        /// <param name="form"></param>
        /// <param name="attachFile"></param>
        /// <returns></returns>
        public RtnResultModel Update(FormCollection form)
        {
            strSql = @"update dbo.ANNOUNCEMENT set title=@title,comment=@comment,effective_date=@effective_date,expire_date=@expire_date,attach_name=@attach_name,mdf_date=getdate(),mdf_user=@logged_user where sid=@sid";
            objParam = new
            {
                title = form.Get("TITLE"),
                comment = form.Get("COMMENT"),
                effective_date = form.Get("EFFECTIVE_DATE"),
                expire_date = DateTime.Parse(form.Get("EXPIRE_DATE")),
                attach_name = form.Get("ATTACH_NAME"),
                logged_user = loggedUser.USER_ID,
                sid = form.Get("SID")
            };
            ExecuteCommand(strSql, objParam);
            return new RtnResultModel(true, i18N.Message.R05);
        }

        /// <summary>
        /// delete Announcement
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public RtnResultModel Delete(FormCollection form)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    strSql = @"update dbo.ANNOUNCEMENT set del_flg=1,mdf_date=getdate(),mdf_user=@logged_user where sid=@sid";
                    objParam = new ArrayList();
                    foreach (string id in form.Get("IDS").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        ((ArrayList)objParam).Add(new
                        {
                            logged_user = loggedUser.USER_ID,
                            sid = id
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