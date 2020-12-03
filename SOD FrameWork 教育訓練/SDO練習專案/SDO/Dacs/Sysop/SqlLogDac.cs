using SDO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace SDO.Dacs
{
    public class SqlLogDac : _Dac
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public SqlLogDac(EmpUserModel user = null) : base(user)
        {
        }

        /// <summary>
        ///  Get SQL_LOG by log_date pageable
        /// </summary>
        /// <param name="logdate"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IList<IDictionary<string, object>> Read(DateTime logdate, int skip, int take)
        {
            string strSql = @"select count(*) over() as datacount,a.user_id,b.user_name,a.user_ip,commandtext,parameters,request_url,log_date from dbo.SQL_TRACE a
                                left join dbo.EMP_USER b on b.user_id=a.user_id where log_date between @logdate and dateadd(day,1,@logdate)
                                order by a.log_date desc offset @skip rows fetch next @take rows only";
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