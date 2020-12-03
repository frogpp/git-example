using Ionic.Zip;
using MiniProfiler.Integrations;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Transactions;
using Dapper;
using SDO.Models;
using System.Collections;
using System.Web;

namespace SDO
{
    /// <summary>
    /// 
    /// </summary>
    public class LogSet
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Log debug info.
        /// </summary>
        /// <param name="message"></param>
        public static void LogDebug(string message)
        {
            logger.Debug(message);
        }

        /// <summary>
        /// Log error & xception info.
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(string message)
        {
            logger.Error(message);
        }

        /// <summary>
        /// Log datacontext tracking info.
        /// </summary>
        /// <param name="message"></param>
        public static void LogInfo(string message)
        {
            logger.Info(message);
        }

        /// <summary>
        /// Log data operate info.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="conditions"></param>
        /// <param name="action"></param>
        public static void LogSqlTrace(object objParam)
        {
            SqlServerDbConnectionFactory mainConn = new SqlServerDbConnectionFactory(ConfigurationManager.ConnectionStrings["MainDBConnection"].ConnectionString);
            using (var conn = DbConnectionFactoryHelper.New(mainConn, CustomDbProfiler.Current))
            {
                string strSql = @"insert into dbo.SQL_TRACE(user_id,user_ip,commandtext,parameters,request_url) 
                                        values(@user_id,@user_ip,@commandtext,@parameters,@request_url)";
                conn.Execute(strSql, objParam);
            }
        }

        /// <summary>
        /// Reset&Backup logs 
        /// </summary>
        /// <param name="logDir"></param>
        /// <param name="months"></param>
        public static void ResetLog(DirectoryInfo logDir)
        {
            string fileName = DateTime.Now.AddMonths(-1).ToString(@"yyyy-MM");
            FileInfo[] logFiles = logDir.GetFiles(string.Format(@"{0}*.log", fileName), SearchOption.AllDirectories);
            if (logFiles.Length > 0)
            {
                string zipFile = Path.Combine(logDir.FullName, string.Format(@"{0}.zip", fileName));
                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }
                using (ZipFile zip = new ZipFile(Encoding.UTF8))
                {
                    foreach (FileInfo logFile in logFiles)
                    {
                        zip.AddFile(logFile.FullName, logFile.DirectoryName.Replace(logDir.FullName, @""));
                    }
                    zip.Save(zipFile);
                    foreach (FileInfo logFile in logFiles)
                    {
                        logFile.Delete();
                    }
                }
            }
            #region BackupDatabaseLog
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MainDBConnection"].ConnectionString))
            {
                conn.Execute("exec dbo.usp_ResetSqlLog");
            }
            #endregion
        }
    }
}