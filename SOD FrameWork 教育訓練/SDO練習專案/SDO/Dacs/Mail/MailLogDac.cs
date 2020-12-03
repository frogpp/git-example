using SDO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace SDO.Dacs
{
    public class MailLogDac : _Dac
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public MailLogDac(EmpUserModel user = null) : base(user)
        {
        }

        /// <summary>
        /// Get MAIL_LOG by crt_date pageable
        /// </summary>
        /// <param name="logdate"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IList<IDictionary<string, object>> Read(DateTime logdate, int skip, int take)
        {
            string strSql = @"select count(*) over() as datacount,mail_sender,mail_receiver,cc_receiver,bcc_receiver,mail_subject,mail_content,send_flg,crt_date 
                                from dbo.MAIL_LOG where crt_date between @logdate and dateadd(day,1,@logdate)
                                order by crt_date desc offset @skip rows fetch next @take rows only";
            objParam = new
            {
                logdate = logdate,
                skip = skip,
                take = take
            };
            return ExecuteQuery(strSql, objParam);
        }
    }
}