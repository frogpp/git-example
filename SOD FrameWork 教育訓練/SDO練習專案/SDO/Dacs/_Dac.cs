using Dapper;
using MiniProfiler.Integrations;
using SDO.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;

namespace SDO.Dacs
{
    /// <summary>
    /// 
    /// </summary>
    public class _Dac : IDisposable
    {
        protected string strConnMain;
        protected string strSql;
        protected object objParam;
        protected EmpUserModel loggedUser;
        private bool disposed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginUser"></param>
        public _Dac(EmpUserModel loginUser)
        {
            CustomDbProfiler.Current.ProfilerContext.Reset();
            strConnMain = ConfigurationManager.ConnectionStrings["MainDBConnection"].ConnectionString;
            strSql = "";
            objParam = null;
            loggedUser = loginUser ?? new EmpUserModel();
            loggedUser.USER_ID = string.IsNullOrWhiteSpace(loggedUser.AGENT_ID) ? loggedUser.USER_ID : string.Format("{0}*{1}", loggedUser.AGENT_ID, loggedUser.USER_ID);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        ~_Dac()
        {
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    SetProfilerLog();
                }
                disposed = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected IList<IDictionary<string, object>> ExecuteQuery(string sql, object param = null)
        {
            using (var conn = DbConnectionFactoryHelper.New(new SqlServerDbConnectionFactory(strConnMain), CustomDbProfiler.Current))
            {
                return ParseXSSResult(conn.Query(sql, param) as IEnumerable<IDictionary<string, object>>);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected IList<T> ExecuteQuery<T>(string sql, object param = null)
        {
            using (var conn = DbConnectionFactoryHelper.New(new SqlServerDbConnectionFactory(strConnMain), CustomDbProfiler.Current))
            {
                return ParseXSSResult(conn.Query<T>(sql, param));
            }
        }

        /// <summary>
        /// 防止 Stored XSS 攻擊
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        private IList<T> ParseXSSResult<T>(IEnumerable<T> result)
        {
            if (typeof(T) == typeof(string))
            {
                return result.Select(p => (T)Convert.ChangeType(HttpUtility.HtmlEncode(p.ToString().Trim()), typeof(T))).ToList();
            }
            else if (typeof(T) == typeof(IDictionary<string, object>))
            {
                return result.Select(p =>
                {
                    foreach (KeyValuePair<string, object> keyValue in p as IDictionary<string, object>)
                    {
                        if (keyValue.Key.ToUpper() != keyValue.Key)
                        {
                            (p as IDictionary<string, object>).Add(keyValue.Key.ToUpper(), (keyValue.Value != null && keyValue.Value is string) ? HttpUtility.HtmlEncode(keyValue.Value.ToString().Trim()) : keyValue.Value);
                            (p as IDictionary<string, object>).Remove(keyValue.Key);
                        }
                        else
                        {
                            if (keyValue.Value != null && keyValue.Value is string)
                            {
                                (p as IDictionary<string, object>)[keyValue.Key] = HttpUtility.HtmlEncode(keyValue.Value.ToString().Trim());
                            }
                        }
                    }
                    return (T)p;
                }).ToList();
            }
            else
            {
                return result.Select(p =>
                {
                    if (p != null)
                    {
                        foreach (var prop in p.GetType().GetProperties())
                        {
                            if (prop.PropertyType == typeof(string) && prop.GetValue(p) != null)
                            {
                                prop.SetValue(p, HttpUtility.HtmlEncode(prop.GetValue(p).ToString().Trim()));
                            }
                        }
                    }
                    return p;
                }).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected bool ExecuteCommand(string sql, object param = null)
        {
            using (var conn = DbConnectionFactoryHelper.New(new SqlServerDbConnectionFactory(strConnMain), CustomDbProfiler.Current))
            {
                return conn.Execute(sql, param) > 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetProfilerLog()
        {
            string[] ignoreList = new string[] { @"/System/Login", @"/SqlLog/QueryPageable", @"/MailLog/QueryPageable" };
            if (!ignoreList.Any(p => HttpContext.Current.Request.Url.AbsoluteUri.IndexOf(p, StringComparison.InvariantCulture) >= 0) &&
                bool.Parse(SysSet.GetSysParam("SystemConfig", "SqlLogTrace")) &&
                HttpContext.Current != null && HttpContext.Current.Session.Count > 0 && HttpContext.Current.User.Identity.IsAuthenticated &&
                CustomDbProfiler.Current != null && CustomDbProfiler.Current.ProfilerContext.ExecutedCommands != null)
            {
                lock (CustomDbProfiler.Current.ProfilerContext.ExecutedCommands)
                {
                    object objParam = new ArrayList();
                    foreach (DbCommandInfo command in CustomDbProfiler.Current.ProfilerContext.ExecutedCommands.ToArray())
                    {
                        ((ArrayList)objParam).Add(new
                        {
                            user_id = loggedUser.USER_ID ?? "",
                            user_ip = loggedUser.USER_IP ?? "",
                            commandtext = command.CommandText,
                            parameters = command.Parameters.Aggregate(new StringBuilder(),
                                     (sb, p) => sb.AppendLine(string.Format("@{0}='{1}'", p.Key, p.Value)),
                                     (sb) => sb.ToString()),
                            request_url = HttpContext.Current.Request.Url.AbsoluteUri
                        });
                    }
                    LogSet.LogSqlTrace(((ArrayList)objParam).ToArray());
                    CustomDbProfiler.Current.ProfilerContext.Reset();
                }
            }
        }
    }
}

