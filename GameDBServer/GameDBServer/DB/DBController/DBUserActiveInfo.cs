using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using Server.Data;
using MySQLDriverCS;
using GameDBServer.Logic;
using GameDBServer.Data;
using System.Data;
using Server.Tools;
using System.Windows;
using Server.Data;


namespace GameDBServer.DB.DBController
{
    /// <summary>
    /// 用户活跃数据处理类
    /// </summary>
    class DBUserActiveInfo : DBController<AccountActiveData>
    {
        private static DBUserActiveInfo instance = new DBUserActiveInfo();

        private DBUserActiveInfo() : base() { }

        public static DBUserActiveInfo getInstance()
        {
            return instance;
        }

        /// <summary>
        /// 更新一个用户角色的最后登录时间和登录次数
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public AccountActiveData GetAccountActiveInfo(DBManager dbMgr, string strAccountID)
        {
            string sql = string.Format("select * from t_user_active_info where Account = '{0}';", strAccountID);

            return this.queryForObject(sql);
        }

        /// <summary>
        /// 更新用户活跃数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public bool UpdateAccountActiveInfo(DBManager dbMgr, string strAccountID)
        {   
            bool ret = false;
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            MySQLConnection conn = null;

            AccountActiveData dataActive = GetAccountActiveInfo(dbMgr, strAccountID);
            if (null == dataActive)
            {
                try
                {
                    conn = dbMgr.DBConns.PopDBConnection();
                    string cmdText = string.Format("INSERT INTO t_user_active_info(Account, createTime, seriesLoginCount, lastSeriesLoginTime) VALUES('{0}', '{1}', {2}, '{3}')",
                                                    strAccountID, today, 1, today);

                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                    MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                    }

                    cmd.Dispose();
                    cmd = null;

                    ret = true;
                }
                finally
                {
                    if (null != conn)
                    {
                        dbMgr.DBConns.PushDBConnection(conn);
                    }
                }
            }
            else
            {
                try
                {
                    DateTime datePreDay = DateTime.Now.AddDays(-1);
                    DateTime dateLastLogin = DateTime.Parse(dataActive.strLastSeriesLoginTime + " 00:00:00");

                    // 如果是前一天登录过
                    if (datePreDay.DayOfYear == dateLastLogin.DayOfYear)
                    {                    
                        conn = dbMgr.DBConns.PopDBConnection();
                        string cmdText = string.Format("UPDATE t_user_active_info SET seriesLoginCount={0}, lastSeriesLoginTime='{1}' WHERE Account='{2}'",
                                                        dataActive.nSeriesLoginCount + 1, today, dataActive.strAccount);

                        GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                        MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                        }

                        cmd.Dispose();
                        cmd = null;

                        ret = true;
                    }
                }
                finally
                {
                    if (null != conn)
                    {
                        dbMgr.DBConns.PushDBConnection(conn);
                    }
                }
            }

            return ret;
        }       
    }
}
