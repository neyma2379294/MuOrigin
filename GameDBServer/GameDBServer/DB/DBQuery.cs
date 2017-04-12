using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySQLDriverCS;
using System.Data;
using System.Windows;
using Server.Tools;
using GameDBServer.Logic;
using Server.Data;

namespace GameDBServer.DB
{
    /// <summary>
    /// 数据库查询(要有控制，禁止频繁查询)
    /// </summary>
    public class DBQuery
    {
        #region 查询操作

        /// <summary>
        /// 查询点将积分数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void QueryDJPointData(DBManager dbMgr, DBRoleInfo dbRoleInfo)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT Id, rid, djpoint, total, wincnt, yestoday, lastweek, lastmonth, dayupdown, weekupdown, monthupdown FROM t_djpoints WHERE rid={0}", dbRoleInfo.RoleID);
                MySQLSelectCommand cmd = new MySQLSelectCommand(conn,
                    new string[] { "Id", "djpoint", "total", "wincnt", "yestoday", "lastweek", "lastmonth", "dayupdown", "weekupdown", "monthupdown" },
                    new string[] { "t_djpoints" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);
                if (cmd.Table.Rows.Count > 0)
                {
                    lock (dbRoleInfo)
                    {
                        dbRoleInfo.LastDJPointDataTikcs = DateTime.Now.Ticks / 10000;
                        dbRoleInfo.RoleDJPointData = new DJPointData()
                        {
                            DbID = Convert.ToInt32(cmd.Table.Rows[0]["Id"].ToString()),
                            RoleID = dbRoleInfo.RoleID,
                            DJPoint = Convert.ToInt32(cmd.Table.Rows[0]["djpoint"].ToString()),
                            Total = Convert.ToInt32(cmd.Table.Rows[0]["total"].ToString()),
                            Wincnt = Convert.ToInt32(cmd.Table.Rows[0]["wincnt"].ToString()),
                            Yestoday = Convert.ToInt32(cmd.Table.Rows[0]["yestoday"].ToString()),
                            Lastweek = Convert.ToInt32(cmd.Table.Rows[0]["lastweek"].ToString()),
                            Lastmonth = Convert.ToInt32(cmd.Table.Rows[0]["lastmonth"].ToString()),
                            Dayupdown = Convert.ToInt32(cmd.Table.Rows[0]["dayupdown"].ToString()),
                            Weekupdown = Convert.ToInt32(cmd.Table.Rows[0]["weekupdown"].ToString()),
                            Monthupdown = Convert.ToInt32(cmd.Table.Rows[0]["monthupdown"].ToString()),
                        };
                    }                    
                }
                else
                {
                    lock (dbRoleInfo)
                    {
                        dbRoleInfo.LastDJPointDataTikcs = DateTime.Now.Ticks / 10000;
                        dbRoleInfo.RoleDJPointData = new DJPointData()
                        {
                            DbID = -1,
                            RoleID = dbRoleInfo.RoleID,
                            DJPoint = 0,
                            Total = 0,
                            Wincnt = 0,
                            Yestoday = 0,
                            Lastweek = 0,
                            Lastmonth = 0,
                            Dayupdown = 0,
                            Weekupdown = 0,
                            Monthupdown = 0,
                        };
                    }
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        /// <summary>
        /// 查询点将积分数据热门排行榜
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static List<DJPointData> QueryDJPointData(DBManager dbMgr)
        {
            List<DJPointData> djPointsHostList = new List<DJPointData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT Id, rid, djpoint, total, wincnt, yestoday, lastweek, lastmonth, dayupdown, weekupdown, monthupdown FROM t_djpoints ORDER BY djpoint DESC LIMIT 100";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                int count = 0;
                while (reader.Read() && count < 100)
                {
                    djPointsHostList.Add(new DJPointData()
                    {
                        DbID = Convert.ToInt32(reader["Id"].ToString()),
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        DJPoint = Convert.ToInt32(reader["djpoint"].ToString()),
                        Total = Convert.ToInt32(reader["total"].ToString()),
                        Wincnt = Convert.ToInt32(reader["wincnt"].ToString()),
                        Yestoday = Convert.ToInt32(reader["yestoday"].ToString()),
                        Lastweek = Convert.ToInt32(reader["lastweek"].ToString()),
                        Lastmonth = Convert.ToInt32(reader["lastmonth"].ToString()),
                        Dayupdown = Convert.ToInt32(reader["dayupdown"].ToString()),
                        Weekupdown = Convert.ToInt32(reader["weekupdown"].ToString()),
                        Monthupdown = Convert.ToInt32(reader["monthupdown"].ToString()),
                    });
                    
                    count++;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();               
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return djPointsHostList;
        }

        /// <summary>
        /// 查询系统公告列表
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static Dictionary<string, BulletinMsgData> QueryBulletinMsgDict(DBManager dbMgr)
        {
            Dictionary<string, BulletinMsgData> dict = new Dictionary<string, BulletinMsgData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT msgid, toplaynum, bulletintext, bulletintime FROM t_bulletin";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    BulletinMsgData bulletinMsgData = new BulletinMsgData()
                    {
                        MsgID = reader["msgid"].ToString(),
                        PlayMinutes = -1,
                        ToPlayNum = Convert.ToInt32(reader["toplaynum"].ToString()),
                        BulletinText = reader["bulletintext"].ToString(),
                        BulletinTicks = DataHelper.ConvertToTicks(reader["bulletintime"].ToString()),
                    };

                    dict[bulletinMsgData.MsgID] = bulletinMsgData;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return dict;
        }

        /// <summary>
        /// 查询游戏配置参数
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static Dictionary<string, string> QueryGameConfigDict(DBManager dbMgr)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT paramname, paramvalue FROM t_config";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    string paramName = reader["paramname"].ToString();
                    string paramVal = reader["paramvalue"].ToString();
                    dict[paramName] = paramVal;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return dict;
        }

        /// <summary>
        /// 查询元宝充值临时记录表中，新充值的记录的用户ID列表
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void QueryTempMoney(DBManager dbMgr, List<string> userIDList, List<int> userMoneyList)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = "SELECT uid, addmoney FROM t_tempmoney";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    userIDList.Add(reader["uid"].ToString());
                    userMoneyList.Add(Convert.ToInt32(reader["addmoney"].ToString()));
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;

                //清空表
                if (userIDList.Count > 0)
                {
                    cmdText = "DELETE FROM t_tempmoney";
                    cmd = new MySQLCommand(cmdText, conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;

                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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

        /// <summary>
        /// 查询礼品码
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static Dictionary<string, LiPinMaItem> QueryLiPinMaDict(DBManager dbMgr)
        {
            Dictionary<string, LiPinMaItem> dict = new Dictionary<string, LiPinMaItem>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT lipinma, huodongid, maxnum, usednum, ptid, ptrepeat FROM t_linpinma";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    LiPinMaItem liPinMaItem = new LiPinMaItem()
                    {
                        LiPinMa = reader["lipinma"].ToString(),
                        HuodongID = Convert.ToInt32(reader["huodongid"].ToString()),
                        MaxNum = Convert.ToInt32(reader["maxnum"].ToString()),
                        UsedNum = Convert.ToInt32(reader["usednum"].ToString()),
                        PingTaiID = Convert.ToInt32(reader["ptid"].ToString()),
                        PingTaiRepeat = Convert.ToInt32(reader["ptrepeat"].ToString()),
                    };

                    dict[liPinMaItem.LiPinMa] = liPinMaItem;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return dict;
        }

        /// <summary>
        /// 查询预先分配的名字
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void QueryPreNames(DBManager dbMgr, Dictionary<string, PreNameItem> preNamesDict, List<PreNameItem> malePreNamesList, List<PreNameItem> femalePreNamesList)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT name, sex FROM t_prenames WHERE used=0";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    PreNameItem preNameItem = new PreNameItem()
                    {
                        Name = reader["name"].ToString(),
                        Sex = Convert.ToInt32(reader["sex"].ToString()),
                        Used = 0,
                    };

                    preNamesDict[preNameItem.Name] = preNameItem;
                    if (0 == preNameItem.Sex) //男性
                    {
                        malePreNamesList.Add(preNameItem);
                    }
                    else //女性
                    {
                        femalePreNamesList.Add(preNameItem);
                    }
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        /// <summary>
        /// 查询副本历史记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static Dictionary<int, FuBenHistData> QueryFuBenHistDict(DBManager dbMgr)
        {
            Dictionary<int, FuBenHistData> dict = new Dictionary<int, FuBenHistData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "SELECT fubenid, rid, rname, usedsecs FROM t_fubenhist";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    FuBenHistData fuBenHistData = new FuBenHistData()
                    {
                        FuBenID = Convert.ToInt32(reader["fubenid"].ToString()),
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = reader["rname"].ToString(),
                        UsedSecs = Convert.ToInt32(reader["usedsecs"].ToString()),
                    };

                    dict[fuBenHistData.FuBenID] = fuBenHistData;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return dict;
        }

        /// <summary>
        /// 根据角色名称查询平台用户ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static string QueryUserIDByRoleName(DBManager dbMgr, string otherRoleName, int zoneID)
        {
            string ret = "";
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT userid FROM t_roles WHERE rname='{0}' AND zoneid={1}", otherRoleName, zoneID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    ret = reader["userid"].ToString();
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return ret;
        }

        /// <summary>
        /// 根据平台用户ID查询元宝和真实的充值钱数
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void QueryUserMoneyByUserID(DBManager dbMgr, string userID, out int userMoney, out int realMoney)
        {
            userMoney = 0;
            realMoney = 0;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT money, realmoney FROM t_money WHERE userid='{0}'", userID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    userMoney = Convert.ToInt32(reader["money"].ToString());
                    realMoney = Convert.ToInt32(reader["realmoney"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        /// <summary>
        /// 根据平台用户ID查询今日元宝和真实的充值钱数
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void QueryTodayUserMoneyByUserID(DBManager dbMgr, string userID, int zoneID, out int userMoney, out int realMoney)
        {
            userMoney = 0;
            realMoney = 0;

            DateTime now = DateTime.Now;
            string todayStart = string.Format("{0:0000}-{1:00}-{2:00} 00:00:00", now.Year, now.Month, now.Day);
            string todayEnd = string.Format("{0:0000}-{1:00}-{2:00} 23:59:59", now.Year, now.Month, now.Day);          

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT SUM(amount) AS totalmoney FROM t_inputlog WHERE u='{0}' AND zoneID={1} AND inputtime>='{2}' AND inputtime<='{3}'", userID, zoneID, todayStart, todayEnd);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    try
                    {
                        userMoney = Convert.ToInt32(reader["totalmoney"].ToString());
                        realMoney = userMoney;
                    }
                    catch(Exception)
                    {
                    }
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        /// <summary>
        /// 根据平台用户ID查询今日元宝和真实的充值钱数
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void QueryTodayUserMoneyByUserID2(DBManager dbMgr, string userID, int zoneID, out int userMoney, out int realMoney)
        {
            userMoney = 0;
            realMoney = 0;

            DateTime now = DateTime.Now;
            string todayStart = string.Format("{0:0000}-{1:00}-{2:00} 00:01:01", now.Year, now.Month, now.Day);
            string todayEnd = string.Format("{0:0000}-{1:00}-{2:00} 23:59:59", now.Year, now.Month, now.Day); 

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT SUM(amount) AS totalmoney FROM t_inputlog2 WHERE u='{0}' AND zoneID={1} AND inputtime>='{2}' AND inputtime<='{3}'", userID, zoneID, todayStart, todayEnd);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    try
                    {
                        userMoney = Convert.ToInt32(reader["totalmoney"].ToString());
                        realMoney = userMoney;
                    }
                    catch (Exception)
                    {
                    }
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        /// <summary>
        /// 根据字符串搜索在线的角色
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static List<SearchRoleData> SearchOnlineRoleByName(DBManager dbMgr, string searchText, int startIndex, int limitNum)
        {
            List<SearchRoleData> searchRoleDataList = new List<SearchRoleData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT rid, rname, sex, occupation, level, zoneid, faction, bhname, changelifecount FROM t_roles WHERE rname LIKE '%{0}%' AND rid>{1} AND lasttime>logofftime AND isdel=0 LIMIT {2}", searchText, startIndex, limitNum);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    int nChangLifeCount = Convert.ToInt32(reader["changelifecount"].ToString());
                    int nLevel = Convert.ToInt32(reader["level"].ToString());
                    
                    // 低于50级不能加入战盟
                    if (100 * nChangLifeCount + nLevel < 50)
                    {
                        continue;
                    }

                    SearchRoleData searchRoleData = new SearchRoleData()
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(Convert.ToInt32(reader["zoneid"].ToString()), reader["rname"].ToString()),
                        RoleSex = Convert.ToInt32(reader["sex"].ToString()),
                        Level = nLevel,
                        Occupation = Convert.ToInt32(reader["occupation"].ToString()),
                        Faction = Convert.ToInt32(reader["faction"].ToString()),
                        BHName = reader["bhname"].ToString()
                    };

                    searchRoleDataList.Add(searchRoleData);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return searchRoleDataList;
        }

        /// <summary>
        /// 获取角色参数表
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static String GetRoleParamByName(DBManager dbMgr, int roleID, string paramName)
        {
            List<PaiHangItemData> list = new List<PaiHangItemData>();
            MySQLConnection conn = null;

            String sValue = "";

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT p.rid, p.pvalue FROM t_roleparams as p "
                    + " where p.pname='{0}' and p.rid={1}", paramName, roleID);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

        
                while (reader.Read())
                {
                    sValue = reader["pvalue"].ToString();
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return sValue;
        }

        #endregion 查询操作

        #region 排行榜查询相关

        /// <summary>
        /// 获取角色参数表中的排行数据
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleParamsTablePaiHang(DBManager dbMgr, string paramName)
        {
            List<PaiHangItemData> list = new List<PaiHangItemData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT p.rid, pvalue, rname, zoneid FROM t_roleparams as p, t_roles as r "
                    + " where p.pname='{0}' and p.rid=r.rid ORDER BY p.pvalue*1 DESC LIMIT 100", paramName);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                int count = 0;
                while (reader.Read() && count < 100)
                {
                    PaiHangItemData paiHangItemData = new PaiHangItemData()
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        Val1 = Convert.ToInt32(reader["pvalue"].ToString()),
                    };

                    list.Add(paiHangItemData);
                    count++;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取角色表中的排行数据
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        private static List<PaiHangItemData> GetRoleTablePaiHang(DBManager dbMgr, string fieldVal1, string otherCondition)
        {
            List<PaiHangItemData> list = new List<PaiHangItemData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT rid, rname, zoneid, admiredcount, {0} FROM t_roles{1} ORDER BY {0} DESC LIMIT 100", fieldVal1, otherCondition);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                int count = 0;
                while (reader.Read() && count < 100)
                {
                    PaiHangItemData paiHangItemData = new PaiHangItemData()
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        Val1 = Convert.ToInt32(reader[fieldVal1].ToString()),
                        Val2 = Convert.ToInt32(reader["admiredcount"].ToString()),
                    };

                    list.Add(paiHangItemData);
                    count++;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取角色的装备排行榜
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleEquipPaiHang(DBManager dbMgr)
        {
            //获取角色表中的排行数据
            return GetRoleTablePaiHang(dbMgr, "equipjifen", " WHERE equipjifen>0 AND isdel=0 AND isflashplayer=0");
        }

        /// <summary>
        /// 获取角色的冲穴个数排行榜
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleXueWeiNumPaiHang(DBManager dbMgr)
        {
            //获取角色表中的排行数据
            return GetRoleTablePaiHang(dbMgr, "xueweinum", " WHERE xueweinum>=20 AND isdel=0");
        }

        /// <summary>
        /// 获取角色的技能层数排行榜
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleSkillLevelPaiHang(DBManager dbMgr)
        {
            //获取角色表中的排行数据
            return GetRoleTablePaiHang(dbMgr, "skilllearnednum", " WHERE skilllearnednum>=60 AND isdel=0");
        }

        /// <summary>
        /// 获取角色的坐骑积分排行榜
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleHorseJiFenPaiHang(DBManager dbMgr)
        {
            //获取角色表中的排行数据
            return GetRoleTablePaiHang(dbMgr, "horsejifen", " WHERE horsejifen>=54 AND isdel=0 AND isflashplayer=0");
        }

        /// <summary>
        /// 获取角色的级别排行榜
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleLevelPaiHang(DBManager dbMgr)
        {
            List<PaiHangItemData> list = new List<PaiHangItemData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT rid, rname, zoneid, level, changelifecount, admiredcount FROM t_roles WHERE level>0 AND isdel=0 AND isflashplayer=0 ORDER BY changelifecount DESC, level DESC, experience DESC LIMIT 100");

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                int count = 0;
                while (reader.Read() && count < 100)
                {
                    PaiHangItemData paiHangItemData = new PaiHangItemData()
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        Val1 = Convert.ToInt32(reader["level"].ToString()),
                        Val2 = Convert.ToInt32(reader["changelifecount"].ToString()),
                        Val3 = Convert.ToInt32(reader["admiredcount"].ToString()),
                    };

                    list.Add(paiHangItemData);
                    count++;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取角色的银两排行榜
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleYinLiangPaiHang(DBManager dbMgr)
        {
            //获取角色表中的排行数据
            return GetRoleTablePaiHang(dbMgr, "yinliang", " WHERE yinliang>0 AND isdel=0 AND isflashplayer=0");
        }

        /// <summary>
        /// 获取角色的金币排行榜
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleGoldPaiHang(DBManager dbMgr)
        {
            //获取角色表中的排行数据
            return GetRoleTablePaiHang(dbMgr, "money2", " WHERE money2>0 AND isdel=0 AND isflashplayer=0");
        }

        /// <summary>
        /// 获取角色的连斩排行榜
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleLianZhanPaiHang(DBManager dbMgr)
        {
            //获取角色表中的排行数据
            return GetRoleTablePaiHang(dbMgr, "lianzhan", " WHERE lianzhan>=100 AND isdel=0 AND isflashplayer=0");
        }

        /// <summary>
        /// 获取角色的杀BOSS排行榜
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleKillBossPaiHang(DBManager dbMgr)
        {
            //获取角色表中的排行数据
            return GetRoleTablePaiHang(dbMgr, "killboss", " WHERE killboss>=5 AND isdel=0 AND isflashplayer=0");
        }

        /// <summary>
        /// 角斗场称号次数排行数据
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleBattleNumPaiHang(DBManager dbMgr)
        {
            //角斗场称号次数排行数据
            return GetRoleTablePaiHang(dbMgr, "battlenum", " WHERE battlenum>=1 AND isdel=0 AND isflashplayer=0");
        }

        /// <summary>
        /// 英雄逐擂到达层数排行数据
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleHeroIndexPaiHang(DBManager dbMgr)
        {
            //英雄逐擂到达层数排行数据
            return GetRoleTablePaiHang(dbMgr, "heroindex", " WHERE heroindex>=1 AND isdel=0 AND isflashplayer=0");
        }


        // 战斗力 [12/17/2013 LiaoWei]
        /// <summary>
        ///战斗力排行数据
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<PaiHangItemData> GetRoleCombatForcePaiHang(DBManager dbMgr)
        {
            //英雄逐擂到达层数排行数据
            return GetRoleTablePaiHang(dbMgr, "combatforce", " WHERE combatforce>=1 AND isdel=0 AND isflashplayer=0");
        }

        #endregion 排行榜查询相关

        #region 帮会相关

        /// <summary>
        /// 获取帮会列表数据
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static BangHuiListData GetBangHuiItemDataList(DBManager dbMgr, int isVerify, int startIndex, int endIndex)
        {
            BangHuiListData bangHuiListData = new BangHuiListData();
            bangHuiListData.TotalBangHuiItemNum = 0;
            bangHuiListData.BangHuiItemDataList = new List<BangHuiItemData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                // string cmdText = string.Format("SELECT b.bhid, b.bhname, b.zoneid, b.rid, r.rname, r.occupation, b.totalnum, b.totallevel, b.qilevel, b.isverfiy, b.totalcombatforce FROM t_banghui AS b, t_roles AS r WHERE b.isdel=0 AND b.rid=r.rid ORDER BY b.totallevel DESC");
                // 按总战斗力排行
                string cmdText = string.Format("SELECT b.bhid, b.bhname, b.zoneid, b.rid, r.rname, r.occupation, b.totalnum, b.totallevel, b.qilevel, b.isverfiy, b.totalcombatforce FROM t_banghui AS b, t_roles AS r WHERE b.isdel=0 AND b.rid=r.rid ORDER BY b.totalcombatforce DESC");

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                //int index = 0;
                while (reader.Read())
                {
                    if (isVerify >= 0 && Convert.ToInt32(reader["isverfiy"].ToString()) != isVerify)
                    {
                        continue;
                    }

                    //if (index >= startIndex && index < endIndex)
                    {
                        BangHuiItemData bangHuiItemData = new BangHuiItemData()
                        {
                            BHID = Convert.ToInt32(reader["bhid"].ToString()),
                            BHName = reader["bhname"].ToString(),
                            ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                            BZRoleID = Convert.ToInt32(reader["rid"].ToString()),
                            BZRoleName = reader["rname"].ToString(),
                            BZOccupation = Convert.ToInt32(reader["occupation"].ToString()),
                            TotalNum = Convert.ToInt32(reader["totalnum"].ToString()),
                            TotalLevel = Convert.ToInt32(reader["totallevel"].ToString()),
                            QiLevel = Convert.ToInt32(reader["qilevel"].ToString()),
                            IsVerfiy = Convert.ToInt32(reader["isverfiy"].ToString()),
                            TotalCombatForce = Convert.ToInt32(reader["totalcombatforce"].ToString()),
                        };

                        bangHuiListData.BangHuiItemDataList.Add(bangHuiItemData);
                    }

                    bangHuiListData.TotalBangHuiItemNum++;
                    //index++;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bangHuiListData;
        }

        /// <summary>
        /// 根据角色ID获取帮会ID
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int FindBangHuiByRoleID(DBManager dbMgr, int roleID)
        {
            int bhid = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT bhid FROM t_banghui WHERE isdel=0 AND rid={0}", roleID);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    bhid = Convert.ToInt32(reader["bhid"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bhid;
        }

        /// <summary>
        /// 根据角色ID获取参见的帮会ID
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int FindJoinBangHuiByRoleID(DBManager dbMgr, int roleID)
        {
            int bhid = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT faction FROM t_roles WHERE rid={0}", roleID);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    bhid = Convert.ToInt32(reader["faction"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bhid;
        }

        /// <summary>
        /// 查询一个帮会的总的人数
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int QueryBHMemberNum(DBManager dbMgr, int bhid)
        {
            int totalNum = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT COUNT(rid) AS totalnum FROM t_roles WHERE isdel=0 AND faction={0}", bhid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    totalNum =Convert.ToInt32(reader["totalnum"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return totalNum;
        }

        /// <summary>
        /// 查询一个帮会的总的级别
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int QueryBHMemberLevel(DBManager dbMgr, int bhid)
        {
            int totalLevel = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT SUM(level) AS totallevel FROM t_roles WHERE isdel=0 AND faction={0}", bhid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {                    
                    try
                    {
                        totalLevel = Convert.ToInt32(reader["totallevel"].ToString());
                    }
                    catch (Exception)
                    {
                        totalLevel = 0;
                    }
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return totalLevel;
        }

        // MU 新增 战斗力总值 [12/28/2013 LiaoWei]
        /// <summary>
        /// 查询一个帮会会员的总战斗力
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int QueryBHMemberTotalCombatForce(DBManager dbMgr, int bhid)
        {
            int totalcombatforce = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT SUM(combatforce) AS totalcombatforce FROM t_roles WHERE isdel=0 AND faction={0}", bhid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    try
                    {
                        totalcombatforce = Convert.ToInt32(reader["totalcombatforce"].ToString());
                    }
                    catch (Exception)
                    {
                        totalcombatforce = 0;
                    }
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return totalcombatforce;
        }

        /// <summary>
        /// 根据帮会ID获取帮会详细信息
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static BangHuiDetailData QueryBangHuiInfoByID(DBManager dbMgr, int bhid)
        {
            BangHuiDetailData bangHuiDetailData = null;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT b.bhid, b.bhname, b.zoneid, b.rid, r.rname, r.occupation, b.totalnum, b.totallevel, b.bhbulletin, b.buildtime, b.qiname, b.qilevel, b.isverfiy, b.tongqian, b.jitan, b.junxie, b.guanghuan FROM t_banghui AS b, t_roles AS r WHERE b.isdel=0 AND b.rid=r.rid AND b.bhid={0}", bhid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    bangHuiDetailData = new BangHuiDetailData();

                    bangHuiDetailData.BHID = Convert.ToInt32(reader["bhid"].ToString());
                    bangHuiDetailData.BHName = reader["bhname"].ToString();
                    bangHuiDetailData.ZoneID = Convert.ToInt32(reader["zoneid"].ToString());
                    bangHuiDetailData.BZRoleID = Convert.ToInt32(reader["rid"].ToString());
                    bangHuiDetailData.BZRoleName = reader["rname"].ToString();
                    bangHuiDetailData.BZOccupation = Convert.ToInt32(reader["occupation"].ToString());
                    bangHuiDetailData.TotalNum = Convert.ToInt32(reader["totalnum"].ToString());
                    bangHuiDetailData.TotalLevel = Convert.ToInt32(reader["totallevel"].ToString());
                    bangHuiDetailData.BHBulletin = reader["bhbulletin"].ToString();
                    bangHuiDetailData.BuildTime = reader["buildtime"].ToString();
                    bangHuiDetailData.QiName = reader["qiname"].ToString();
                    bangHuiDetailData.QiLevel = Convert.ToInt32(reader["qilevel"].ToString());
                    bangHuiDetailData.IsVerify = Convert.ToInt32(reader["isverfiy"].ToString());
                    bangHuiDetailData.TotalMoney = Convert.ToInt32(reader["tongqian"].ToString());
                    bangHuiDetailData.JiTan = Convert.ToInt32(reader["jitan"].ToString());
                    bangHuiDetailData.JunXie = Convert.ToInt32(reader["junxie"].ToString());
                    bangHuiDetailData.GuangHuan = Convert.ToInt32(reader["guanghuan"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bangHuiDetailData;
        }

        /// <summary>
        /// 获取帮会管理成员列表数据
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<BangHuiMgrItemData> GetBangHuiMgrItemItemDataList(DBManager dbMgr, int bhid)
        {
            List<BangHuiMgrItemData> list = new List<BangHuiMgrItemData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.bhzhiwu, r.chenghao, r.banggong, r.level FROM t_roles AS r WHERE r.bhzhiwu>0 AND r.faction={0}", bhid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    BangHuiMgrItemData bangHuiMgrItemData = new BangHuiMgrItemData()
                    {
                        ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = reader["rname"].ToString(),
                        Occupation = Convert.ToInt32(reader["occupation"].ToString()),
                        BHZhiwu = Convert.ToInt32(reader["bhzhiwu"].ToString()),
                        ChengHao = reader["chenghao"].ToString(),
                        BangGong = Convert.ToInt32(reader["banggong"].ToString()),
                        Level = Convert.ToInt32(reader["level"].ToString()),
                    };

                    list.Add(bangHuiMgrItemData);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取帮会管理成员数据
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static BangHuiMgrItemData GetBangHuiMgrItemItemDataByID(DBManager dbMgr, int bhid, int roleID)
        {
            BangHuiMgrItemData bangHuiMgrItemData = null;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.bhzhiwu, r.chenghao, r.banggong, r.level FROM t_roles AS r WHERE r.bhzhiwu>0 AND r.faction={0} AND r.rid={1}", bhid, roleID);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    bangHuiMgrItemData = new BangHuiMgrItemData()
                    {
                        ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = reader["rname"].ToString(),
                        Occupation = Convert.ToInt32(reader["occupation"].ToString()),
                        BHZhiwu = Convert.ToInt32(reader["bhzhiwu"].ToString()),
                        ChengHao = reader["chenghao"].ToString(),
                        BangGong = Convert.ToInt32(reader["banggong"].ToString()),
                        Level = Convert.ToInt32(reader["level"].ToString()),
                    };
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bangHuiMgrItemData;
        }

        /// <summary>
        /// 获取帮会成员列表数据
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<BangHuiMemberData> GetBangHuiMemberDataList(DBManager dbMgr, int bhid)
        {
            List<BangHuiMemberData> list = new List<BangHuiMemberData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.bhzhiwu, r.chenghao, r.banggong, r.level, r.xueweinum, r.skilllearnednum, r.combatforce, r.changelifecount FROM t_roles AS r WHERE r.faction={0} AND r.isdel=0", bhid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    BangHuiMemberData bangHuiMemberData = new BangHuiMemberData()
                    {
                        ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = reader["rname"].ToString(),
                        Occupation = Convert.ToInt32(reader["occupation"].ToString()),
                        BHZhiwu = Convert.ToInt32(reader["bhzhiwu"].ToString()),
                        ChengHao = reader["chenghao"].ToString(),
                        BangGong = Convert.ToInt32(reader["banggong"].ToString()),
                        Level = Convert.ToInt32(reader["level"].ToString()),
                        XueWeiNum = Convert.ToInt32(reader["xueweinum"].ToString()),
                        SkillLearnedNum = Convert.ToInt32(reader["skilllearnednum"].ToString()),
                        BangHuiMemberCombatForce = Convert.ToInt32(reader["combatforce"].ToString()),
                        BangHuiMemberChangeLifeLev = Convert.ToInt32(reader["changelifecount"].ToString()),
                    };

                    list.Add(bangHuiMemberData);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取帮会管理成员字符串列表
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static string GetBangHuiMgrItemItemStringList(DBManager dbMgr, int bhid)
        {
            StringBuilder sb = new StringBuilder();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.bhzhiwu, r.chenghao, r.banggong FROM t_roles AS r WHERE r.bhzhiwu>0 AND r.faction={0}", bhid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    sb.AppendFormat("{0},", reader["rid"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 根据帮会ID获取帮会库存信息
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static BangHuiBagData QueryBangHuiBagDataByID(DBManager dbMgr, int bhid)
        {
            BangHuiBagData bangHuiBagData = new BangHuiBagData();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT goods1num, goods2num, goods3num, goods4num, goods5num, tongqian FROM t_banghui WHERE isdel=0 AND bhid={0}", bhid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    bangHuiBagData.Goods1Num = Convert.ToInt32(reader["goods1num"].ToString());
                    bangHuiBagData.Goods2Num = Convert.ToInt32(reader["goods2num"].ToString());
                    bangHuiBagData.Goods3Num = Convert.ToInt32(reader["goods3num"].ToString());
                    bangHuiBagData.Goods4Num = Convert.ToInt32(reader["goods4num"].ToString());
                    bangHuiBagData.Goods5Num = Convert.ToInt32(reader["goods5num"].ToString());
                    bangHuiBagData.TongQian = Convert.ToInt32(reader["tongqian"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bangHuiBagData;
        }

        /// <summary>
        /// 根据帮会ID获取帮会库存历史贡献信息
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<BangGongHistData> GetBangHuiBagHistList(DBManager dbMgr, int bhid)
        {
            List<BangGongHistData> bangGongHistDataList = new List<BangGongHistData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.level, r.bhzhiwu, r.chenghao, b.goods1num, b.goods2num, b.goods3num, b.goods4num, b.goods5num, b.tongqian, b.banggong FROM t_banggonghist AS b, t_roles AS r WHERE b.bhid={0} AND b.rid=r.rid ORDER BY b.banggong DESC", bhid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    BangGongHistData bangGongHistData = new BangGongHistData()
                    {
                        ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = reader["rname"].ToString(),
                        Occupation = Convert.ToInt32(reader["occupation"].ToString()),
                        RoleLevel = Convert.ToInt32(reader["level"].ToString()),
                        BHZhiWu = Convert.ToInt32(reader["bhzhiwu"].ToString()),
                        BHChengHao = reader["chenghao"].ToString(),
                        Goods1Num = Convert.ToInt32(reader["goods1num"].ToString()),
                        Goods2Num = Convert.ToInt32(reader["goods2num"].ToString()),
                        Goods3Num = Convert.ToInt32(reader["goods3num"].ToString()),
                        Goods4Num = Convert.ToInt32(reader["goods4num"].ToString()),
                        Goods5Num = Convert.ToInt32(reader["goods5num"].ToString()),
                        TongQian = Convert.ToInt32(reader["tongqian"].ToString()),
                        BangGong = Convert.ToInt32(reader["banggong"].ToString()),
                    };

                    bangGongHistDataList.Add(bangGongHistData);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bangGongHistDataList;
        }

        /// <summary>
        /// 根据帮会ID获取帮旗详细信息
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static BangQiInfoData QueryBangQiInfoByID(DBManager dbMgr, int bhid)
        {
            BangQiInfoData bangQiInfoData = new BangQiInfoData();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT b.qiname, b.qilevel FROM t_banghui AS b WHERE b.isdel=0 AND b.bhid={0}", bhid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    bangQiInfoData.BangQiName = reader["qiname"].ToString();
                    bangQiInfoData.BangQiLevel = Convert.ToInt32(reader["qilevel"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bangQiInfoData;
        }

        /// <summary>
        /// 根据帮会ID获取领地势力分布信息
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static Dictionary<int, BHLingDiOwnData> GetBHLingDiOwnDataDict(DBManager dbMgr)
        {
            Dictionary<int, BHLingDiOwnData> bhLingDiOwnDataDict = new Dictionary<int, BHLingDiOwnData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT l.lingdi, b.zoneid, b.bhid, b.bhname, b.qiname, b.qilevel FROM t_banghui AS b, t_lingdi AS l WHERE b.bhid=l.bhid AND b.isdel=0");

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    BHLingDiOwnData bhLingDiOwnData = new BHLingDiOwnData()
                    {
                        LingDiID = Convert.ToInt32(reader["lingdi"].ToString()),
                        ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                        BHID = Convert.ToInt32(reader["bhid"].ToString()),
                        BHName = reader["bhname"].ToString(),
                        BangQiName = reader["qiname"].ToString(),
                        BangQiLevel = Convert.ToInt32(reader["qilevel"].ToString()),
                    };

                    bhLingDiOwnDataDict[bhLingDiOwnData.LingDiID] = bhLingDiOwnData;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bhLingDiOwnDataDict;
        }

        /// <summary>
        /// 获取帮会的军旗数据并填充字典
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static void QueryBangQiDict(DBManager dbMgr, Dictionary<int, BangHuiJunQiItemData> bangHuiJunQiItemDcit)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT b.bhid, b.qiname, b.qilevel FROM t_banghui AS b WHERE b.isdel=0");

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    int bhid = Convert.ToInt32(reader["bhid"].ToString());
                    bangHuiJunQiItemDcit[bhid] = new BangHuiJunQiItemData()
                    {
                        BHID = bhid,
                        QiName = reader["qiname"].ToString(),
                        QiLevel = Convert.ToInt32(reader["qilevel"].ToString()),
                    };
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        /// <summary>
        /// 获取领地势力分布字典
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static void QueryBHLingDiInfoDict(DBManager dbMgr, Dictionary<int, BangHuiLingDiInfoData> bangHuiLingDiItemsDict)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT l.lingdi, l.bhid, 1 as zoneid, \"\" as bhname, l.tax, l.takedayid, l.takedaynum, l.yestodaytax, l.taxdayid, l.todaytax, l.totaltax, l.warrequest, l.awardfetchday FROM t_lingdi AS l");
                //string cmdText = string.Format("SELECT l.lingdi, b.bhid, b.zoneid, b.bhname, l.tax, l.takedayid, l.takedaynum, l.yestodaytax, l.taxdayid, l.todaytax, l.totaltax, l.warrequest, l.awardfetchday FROM t_lingdi AS l, t_banghui AS b WHERE l.bhid=b.bhid");

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    BangHuiLingDiInfoData bangHuiLingDiInfoData = new BangHuiLingDiInfoData()
                    {
                        LingDiID = Convert.ToInt32(reader["lingdi"].ToString()),
                        BHID = Convert.ToInt32(reader["bhid"].ToString()),
                        ZoneID = Convert.ToInt32(reader["zoneid"].ToString()),
                        BHName = reader["bhname"].ToString(),
                        LingDiTax = Convert.ToInt32(reader["tax"].ToString()),
                        TakeDayID = Convert.ToInt32(reader["takedayid"].ToString()),
                        TakeDayNum = Convert.ToInt32(reader["takedaynum"].ToString()),
                        YestodayTax = Convert.ToInt32(reader["yestodaytax"].ToString()),
                        TaxDayID = Convert.ToInt32(reader["taxdayid"].ToString()),
                        TodayTax = Convert.ToInt32(reader["todaytax"].ToString()),
                        TotalTax = Convert.ToInt32(reader["TotalTax"].ToString()),
                        AwardFetchDay = Convert.ToInt32(reader["awardfetchday"].ToString()),
                    };

                    byte[] warReqArr = reader["warrequest"] as byte[];
                    if (null == warReqArr)
                    {
                        bangHuiLingDiInfoData.WarRequest = "";
                    }
                    else
                    {
                        bangHuiLingDiInfoData.WarRequest = Encoding.Default.GetString(warReqArr);
                    }

                    bangHuiLingDiItemsDict[bangHuiLingDiInfoData.LingDiID] = bangHuiLingDiInfoData;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }
        }

        /// <summary>
        /// 根据帮会ID获取帮会副本详细信息
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static string QueryBangFuBenByID(DBManager dbMgr, int bhid)
        {
            string result = "";
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT b.fubenid, b.fubenstate, b.openday, b.killers FROM t_banghui AS b WHERE b.isdel=0 AND b.bhid={0}", bhid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    result = string.Format("{0}:{1}:{2}:{3}:{4}", bhid,
                        Convert.ToInt32(reader["fubenid"].ToString()),
                        Convert.ToInt32(reader["fubenstate"].ToString()),
                        Convert.ToInt32(reader["openday"].ToString()),
                        reader["killers"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return result;
        }

        /// <summary>
        /// 查询皇妃的个数
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int QueryHuangFeiCount(DBManager dbMgr)
        {
            int ret = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT COUNT(rid) AS huanghounum FROM t_roles WHERE isdel=0 AND huanghou=1");
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    ret = Convert.ToInt32(reader["huanghounum"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return ret;
        }

        /// <summary>
        /// 查询皇妃列表
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<SearchRoleData> QueryHuangFeiDataList(DBManager dbMgr)
        {
            List<SearchRoleData> huangFeiDataList = new List<SearchRoleData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT rid, rname, sex, occupation, level, zoneid, faction, bhname FROM t_roles WHERE isdel=0 AND huanghou=1");
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    SearchRoleData searchRoleData = new SearchRoleData()
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(Convert.ToInt32(reader["zoneid"].ToString()), reader["rname"].ToString()),
                        RoleSex = Convert.ToInt32(reader["sex"].ToString()),
                        Level = Convert.ToInt32(reader["level"].ToString()),
                        Occupation = Convert.ToInt32(reader["occupation"].ToString()),
                        Faction = Convert.ToInt32(reader["faction"].ToString()),
                        BHName = reader["bhname"].ToString()
                    };

                    huangFeiDataList.Add(searchRoleData);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return huangFeiDataList;
        }

        /// <summary>
        /// 加载皇帝特权数据
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static HuangDiTeQuanItem LoadHuangDiTeQuan(DBManager dbMgr)
        {
            HuangDiTeQuanItem huangDiTeQuanItem = null;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT Id, tolaofangdayid, tolaofangnum, offlaofangdayid, offlaofangnum, bancatdayid, bancatnum FROM t_hdtequan WHERE Id=1");
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    huangDiTeQuanItem = new HuangDiTeQuanItem()
                    {
                        ID = Convert.ToInt32(reader["Id"].ToString()),
                        ToLaoFangDayID = Convert.ToInt32(reader["tolaofangdayid"].ToString()),
                        ToLaoFangNum = Convert.ToInt32(reader["tolaofangnum"].ToString()),
                        OffLaoFangDayID = Convert.ToInt32(reader["offlaofangdayid"].ToString()),
                        OffLaoFangNum = Convert.ToInt32(reader["offlaofangnum"].ToString()),
                        BanCatDayID = Convert.ToInt32(reader["bancatdayid"].ToString()),
                        BanCatNum = Convert.ToInt32(reader["bancatnum"].ToString()),
                    };
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return huangDiTeQuanItem;
        }

        /// <summary>
        /// 根据帮会ID获取帮会库存历史贡献信息
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<int> GetNoMoneyBangHuiList(DBManager dbMgr)
        {
            List<int> noMoneyBangHuiList = new List<int>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT bhid FROM t_banghui WHERE tongqian<0");

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    noMoneyBangHuiList.Add(Convert.ToInt32(reader["bhid"].ToString()));
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return noMoneyBangHuiList;
        }

        #endregion 帮会相关

        #region 奇珍阁相关

        /// <summary>
        /// 获取奇珍阁购买数据记录
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<QizhenGeBuItemData> QueryQizhenGeBuItemDataList(DBManager dbMgr)
        {
            List<QizhenGeBuItemData> list = new List<QizhenGeBuItemData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT r.rid, r.rname, r.zoneid, q.goodsid, q.goodsnum FROM t_roles AS r, t_qizhengebuy AS q WHERE q.rid=r.rid ORDER BY buytime DESC LIMIT 10");

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                int count = 0;
                while (reader.Read() && count < 100)
                {
                    QizhenGeBuItemData qizhenGeBuItemData = new QizhenGeBuItemData()
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        GoodsID = Convert.ToInt32(reader["goodsid"].ToString()),
                        GoodsNum = Convert.ToInt32(reader["goodsnum"].ToString()),
                    };

                    list.Add(qizhenGeBuItemData);
                    count++;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return list;
        }

        #endregion 奇珍阁相关

        #region 限时抢购相关

        /// <summary>
        /// 获取某角色某项抢购购买数据记录
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int QueryQiangGouBuyItemNumByRoleID(DBManager dbMgr, int roleID, int goodsID, int qiangGouID, int random)
        {
            int count = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = "";
                if (random <= 0)
                {
                    cmdText = string.Format("SELECT SUM(goodsnum) AS totalgoodsnum FROM t_qianggoubuy WHERE rid={0} and goodsid={1} and qianggouid={2}", roleID, goodsID, qiangGouID);
                }
                else
                {
                    DateTime now = DateTime.Now;
                    string todayStart = string.Format("{0:0000}-{1:00}-{2:00} 00:01:01", now.Year, now.Month, now.Day);
                    string todayEnd = string.Format("{0:0000}-{1:00}-{2:00} 23:59:59", now.Year, now.Month, now.Day);
                    cmdText = string.Format("SELECT SUM(goodsnum) AS totalgoodsnum FROM t_qianggoubuy WHERE rid={0} and goodsid={1} and qianggouid={2} and buytime>='{3}' and buytime<='{4}'",
                        roleID, goodsID, qiangGouID, todayStart, todayEnd);
                }

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    try
                    {
                        count = Convert.ToInt32(reader["totalgoodsnum"].ToString());
                    }
                    catch (Exception)
                    {
                        count = 0;
                    }
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return count;
        }

        /// <summary>
        /// 获取某项抢购购买数据记录
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int QueryQiangGouBuyItemNum(DBManager dbMgr, int goodsID, int qiangGouID, int random)
        {
            int count = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = "";
                if (random <= 0)
                {
                    cmdText = string.Format("SELECT SUM(goodsnum) AS totalgoodsnum FROM t_qianggoubuy WHERE goodsid={0} and qianggouid={1}", goodsID, qiangGouID);
                }
                else
                {
                    DateTime now = DateTime.Now;
                    string todayStart = string.Format("{0:0000}-{1:00}-{2:00} 00:01:01", now.Year, now.Month, now.Day);
                    string todayEnd = string.Format("{0:0000}-{1:00}-{2:00} 23:59:59", now.Year, now.Month, now.Day);
                    cmdText = string.Format("SELECT SUM(goodsnum) AS totalgoodsnum FROM t_qianggoubuy WHERE goodsid={0} and qianggouid={1} and buytime>='{2}' and buytime<='{3}'",
                        goodsID, qiangGouID, todayStart, todayEnd);
                }

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    try
                    {
                        count = Convert.ToInt32(reader["totalgoodsnum"].ToString());
                    }
                    catch (Exception)
                    {
                        count = 0;
                    }
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return count;
        }

        #endregion 限时抢购相关

        #region 生肖运程竞猜相关

        /// <summary>
        /// 获取奇珍阁购买数据记录
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<ShengXiaoGuessHistory> QueryShengXiaoGuessHistoryDataList(DBManager dbMgr, int roleID = -1)
        {
            List<ShengXiaoGuessHistory> list = new List<ShengXiaoGuessHistory>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string sWhere = " Where gainnum>0 ";
                if (roleID > 0)
                {
                    sWhere += string.Format(" and rid={0} ", roleID);
                }

                string cmdText = string.Format("SELECT * FROM t_shengxiaoguesshist {0} ORDER BY guesstime DESC LIMIT 15", sWhere);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                int count = 0;
                while (reader.Read() && count < 100)
                {
                    ShengXiaoGuessHistory historyData = new ShengXiaoGuessHistory()
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        GuessKey = Convert.ToInt32(reader["guesskey"].ToString()),
                        Mortgage = Convert.ToInt32(reader["mortgage"].ToString()),
                        ResultKey = Convert.ToInt32(reader["resultkey"].ToString()),
                        GainNum = Convert.ToInt32(reader["gainnum"].ToString()),
                        LeftMortgage = Convert.ToInt32(reader["leftmortgage"].ToString()),
                        GuessTime = reader["guesstime"].ToString(),
                    };

                    list.Add(historyData);
                    count++;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return list;
        }

        #endregion 生肖运程竞猜相关

        #region 礼品码相关

        /// <summary>
        /// 通过活动ID查询平台ID
        /// </summary>
        /// <param name="huodongID"></param>
        /// <returns></returns>
        public static int QueryPingTaiIDByHuoDongID(DBManager dbMgr, int huodongID, int rid, int pingTaiID)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT ptid FROM t_usedlipinma WHERE huodongid={0} AND rid={1} AND ptid={2}", huodongID, rid, pingTaiID);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    ret = Convert.ToInt32(reader["ptid"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return ret;
        }

        #endregion 礼品码相关

        #region 充值相关

        /// <summary>
        /// 通过用户ID查询充值记录
        /// </summary>
        /// <param name="huodongID"></param>
        /// <returns></returns>
        public static int QueryTotalChongZhiMoney(DBManager dbMgr, string userID, int zoneID)
        {
            int ret = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT SUM(amount) AS totalmoney FROM t_inputlog WHERE u='{0}' AND zoneid={1}", userID, zoneID);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    try
                    {
                        ret = Convert.ToInt32(reader["totalmoney"].ToString());
                    }
                    catch (Exception)
                    {
                        ret = 0;
                    }
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return ret;
        }


        /// <summary>
        /// 通过用户ID和充值钱数查指定的充值记录
        /// </summary>
        /// <param name="huodongID"></param>
        /// <returns></returns>
        public static int QueryChargeMoney(DBManager dbMgr, string userID, int zoneID,int addmoney)
        {
            int ret = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT count(amount) as num FROM t_inputlog WHERE u='{0}' AND zoneid={1} AND amount={2}", userID, zoneID,addmoney);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    try
                    {
                        ret = Convert.ToInt32(reader["num"].ToString());
                    }
                    catch (Exception)
                    {
                        ret = 0;
                    }
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return ret;
        }
        /// <summary>
        /// 获取上次扫描的充值日志的ID
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int QueryLastScanInputLogID(DBManager dbMgr)
        {
            int ret = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT lastid FROM t_inputhist WHERE Id=1");

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    try
                    {
                        ret = Convert.ToInt32(reader["lastid"].ToString());
                    }
                    catch (Exception)
                    {
                        ret = 0;
                    }
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return ret;
        }

        /// <summary>
        /// 查询充值记录，并且写入日志中
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int ScanInputLogFromTable(DBManager dbMgr, int lastScanID)
        {
            int ret = lastScanID;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT Id, amount, u, inputtime, zoneid FROM t_inputlog WHERE Id>{0} and result='success'", lastScanID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    /*GameDBManager.DBEventsWriter.CacheInputlog(-1,
                        Convert.ToInt32(reader["zoneid"].ToString()),
                        Convert.ToInt32(reader["amount"].ToString()),
                        reader["u"].ToString(),
                        reader["inputtime"].ToString());*/

                    ret = Math.Max(ret, Convert.ToInt32(reader["Id"].ToString()));
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return ret;
        }

        #endregion 礼品码相关

        #region 活动奖励相关

        /// <summary>
        /// 返回充值排行信息 key:userid, value:totalmoney 的list,排行小于等于maxPaiHang的被返回,如果两个玩家充值额相等，先达到值的排名靠前
        /// dictionay顺序未知，直接返回排好序的list,这个函数返回的排行值是具体的真实货币值，外部使用时需要自行转换为元宝数，不在这转换
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="startTime">字符串时间</param>
        /// <param name="endTime">字符串时间</param>
        /// <param name="maxPaiHang">必须大于等于1,默认返回排名前三的信息</param>
        /// <returns></returns>
        public static List<InputKingPaiHangData> GetUserInputPaiHang(DBManager dbMgr, string startTime, string endTime, int maxPaiHang = 3)
        {
            List<InputKingPaiHangData> lsPaiHang = new List<InputKingPaiHangData>();
 
            if (maxPaiHang < 1)
            {
                return lsPaiHang;
            }

            string userid = ""; int totalmoney = 0;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                //这个位置需要采用unix时间戳进行定位,普通情况下，unix时间戳和inputtime是同一个值，单inputtime时间戳比unix要大一点
                //对用户充值而言，采用unix时间戳比较准确,此外，默认任务玩家充值信息只在t_inputlog表中有，而gm充值信息只在t_inputlog2表中，
                //如果存在某个玩家在两个表中都有充值，该玩家可能有两条数据位于取出的数据中，这时，需要做过滤操作，所以，返回数据最大行数乘以2,
                //然后再对返回的数据进行重复玩家过滤
                string cmdText = string.Format("SELECT u, sum(amount) as totalmoney, max(time) as time from t_inputlog where t_inputlog.u IN (select DISTINCT  userid from t_roles where t_roles.isdel=0) and inputtime>='{0}' and inputtime<='{1}' and result='success' GROUP by u " +
                                 " union " +
                                 " SELECT u, sum(amount) as totalmoney, max(time) as time from t_inputlog2 where t_inputlog2.u IN (select DISTINCT userid from t_roles where t_roles.isdel=0) and  inputtime>='{0}' and inputtime<='{1}' and result='success' GROUP by u order by totalmoney desc,time asc " +
                                 " limit 0, {2} ", startTime, endTime, maxPaiHang * 2);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                List<string> tmp = new List<string>();
                int count = 0;
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //最多返回maxPaiHang 条数据
                while (reader.Read())
                {
                    count++;

                    userid = reader["u"].ToString();
                    totalmoney = Convert.ToInt32(reader["totalmoney"].ToString());
                    if( totalmoney > 0 )
                    {
                        if ( !tmp.Contains(userid) )
                        {
                            tmp.Add(userid);

                            InputKingPaiHangData phData = new InputKingPaiHangData
                            {
                                UserID = userid,
                                PaiHang = count,
                                PaiHangTime = now,
                                PaiHangValue = totalmoney
                            };

                            lsPaiHang.Add(phData);
                        }
                        else
                        {
                            if ( tmp.Count > 0 && lsPaiHang.Count > 0 )
                            {
                                //对在两个表中都有充值[一般是gm]特殊处理
                                InputKingPaiHangData phData = lsPaiHang[tmp.IndexOf(userid)];
                                phData.PaiHangValue += totalmoney;
                            }
                        }
                    }
                    //最多取回 maxPaiHang条
                    if (lsPaiHang.Count >= maxPaiHang)
                    {
                        break;
                    }
                    
                }

                //对返回结果进行排序，按照PaiHangValue 由大到小排序，然后更新PaiHang值

                Comparison<InputKingPaiHangData> com = new Comparison<InputKingPaiHangData>(InputKingPaiHangDataCompare);
                lsPaiHang.Sort(com);

                for (int n = 0; n < lsPaiHang.Count; n++)
                {
                    lsPaiHang[n].PaiHang = n + 1;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return lsPaiHang;
        }

        /// <summary>
        /// 这样写导致排序时降序排列
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static int InputKingPaiHangDataCompare(InputKingPaiHangData left, InputKingPaiHangData right)
        {
            return right.PaiHangValue - left.PaiHangValue;
        }

        /// <summary>
        /// 返回玩家某时间内在某区的消费总额【这儿返回的是真实货币单位，要转换为元宝，必须调用相应的转换函数】
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetUserInputMoney(DBManager dbMgr, string userid, int zoneid, string startTime, string endTime)
        {
            int totalmoney = 0;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                //这个位置需要采用unix时间戳进行定位,普通情况下，unix时间戳和inputtime是同一个值，单inputtime时间戳比unix要大一点
                //对用户充值而言，采用unix时间戳比较准确,此外，默认任务玩家充值信息只在t_inputlog表中有，而gm充值信息只在t_inputlog2表中，
                //如果存在某个玩家在两个表中都有充值，该玩家可能有两条数据位于取出的数据中，这时，需要做求和操作
                string cmdText = string.Format("SELECT u, sum(amount) as totalmoney, max(time) as time from t_inputlog where inputtime>='{0}' and inputtime<='{1}' and u='{2}' and zoneid={3} and result='success' GROUP by u " +
                                               " union " +
                                               " SELECT u, sum(amount) as totalmoney, max(time) as time from t_inputlog2 where inputtime>='{0}' and inputtime<='{1}' and u='{2}' and zoneid={3} and result='success' GROUP by u ",
                                               startTime, endTime, userid, zoneid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    totalmoney += Convert.ToInt32(reader["totalmoney"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return totalmoney;
        }

        /// <summary>
        /// 返回角色奖励领取记录,主要用于针对角色的活动奖励
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetAwardHistoryForRole(DBManager dbMgr, int rid, int zoneid, int activitytype, string keystr, out int hasgettimes, out string lastgettime)
        {
            hasgettimes = 0;
            lastgettime = "";
            int ret = -1;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT hasgettimes, lastgettime from t_huodongawardrolehist where rid={0} and zoneid={1} and activitytype={2} and keystr='{3}' ",
                    rid, zoneid, activitytype, keystr);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                //这儿应该最多只有一条数据
                if (reader.Read())
                {
                    hasgettimes = Convert.ToInt32(reader["hasgettimes"].ToString());
                    lastgettime = reader["lastgettime"].ToString();
                    ret = 0;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return ret;
        }

        /// <summary>
        /// 返回角色奖励领取记录,主要用于针对用户账号【一个账号会有多个角色】的活动奖励
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetAwardHistoryForUser(DBManager dbMgr, string userid, int activitytype, string keystr, out int hasgettimes, out string lastgettime)
        {
            hasgettimes = 0;
            lastgettime = "";
            int ret = -1;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT hasgettimes, lastgettime from t_huodongawarduserhist where userid='{0}' and activitytype={1} and keystr='{2}' ",
                    userid, activitytype, keystr);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                //这儿应该最多只有一条数据
                if (reader.Read())
                {
                    hasgettimes = Convert.ToInt32(reader["hasgettimes"].ToString());
                    lastgettime = reader["lastgettime"].ToString();
                    ret = 0;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return ret;
        }

        /// <summary>
        /// 返回角色活动排行列表数据,最靠近且MidTime的排行信息,最多maxPaiHang条数据,即排行最大值maxPaiHang
        /// 如果两个排行时间离MidTime一样进，则取比midtime大一点的那个时间的排行信息
        /// 没有对活动的特殊限制条件进行过滤，过滤条件可能变化，所以由外部临时处理
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static List<HuoDongPaiHangData> GetActivityPaiHangListNearMidTime(DBManager dbMgr, int huoDongType, string midTime, int maxPaiHang = 10)
        {
            List<HuoDongPaiHangData> lsPaiHang = new List<HuoDongPaiHangData>();

            MySQLConnection conn = null;
            try
            {
                string minTime = DateTime.Parse(midTime).AddHours(-36).ToString();
                string maxTime = DateTime.Parse(midTime).AddHours(36).ToString();

                conn = dbMgr.DBConns.PopDBConnection();

                //时间差值diff升序,时间降序,paihang升序，保证取到的数据是最接近midTime从小到大的排行信息，不需要用paihang<=maxPaiHang，采用排行递增排序和条数限制就行
                string cmdText = string.Format("SELECT rid, rname, zoneid, type, paihang, phvalue, paihangtime, ABS(datediff(paihangtime, '{0}')) as diff " +
                    " from t_huodongpaihang where type={1} and paihangtime<='{2}' and paihangtime>='{3}' ORDER by diff ASC, paihangtime desc, paihang ASC LIMIT 0, {4}",
                    midTime, huoDongType, maxTime, minTime, maxPaiHang);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                string sPaiHangTime = "";

                //查询得到的数据以最靠近midTime的排行为准，写t_huodongpaihang时采用同一个paihangtime，这儿使用paihangtime进行不一致的排行时间过滤
                while (reader.Read())
                {
                    HuoDongPaiHangData ph = new HuoDongPaiHangData();
                    ph.RoleID = Convert.ToInt32(reader["rid"].ToString());
                    ph.RoleName = reader["rname"].ToString();
                    ph.ZoneID = Convert.ToInt32(reader["zoneid"].ToString());
                    ph.Type = Convert.ToInt32(reader["type"].ToString());
                    ph.PaiHang = Convert.ToInt32(reader["paihang"].ToString());
                    ph.PaiHangValue = Convert.ToInt32(reader["phvalue"].ToString());
                    ph.PaiHangTime = reader["paihangtime"].ToString();

                    //确保返回的数据是同一时间点排行，尽管数量可能比maxPaiHang小
                    if (string.IsNullOrEmpty(sPaiHangTime))
                    {
                        sPaiHangTime = ph.PaiHangTime;
                    }
                    else if (string.Compare(sPaiHangTime, ph.PaiHangTime) != 0)
                    {
                        break;
                    }

                    lsPaiHang.Add(ph);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return lsPaiHang;
        }

        #endregion 活动奖励相关

        #region 物品限购

        /// <summary>
        /// 通过角色ID和物品ID查询物品每日的已经购买数量
        /// </summary>
        /// <param name="huodongID"></param>
        /// <returns></returns>
        public static int QueryLimitGoodsUsedNumByRoleID(DBManager dbMgr, int roleID, int goodsID, out int dayID, out int usedNum)
        {
            dayID = 0;
            usedNum = 0;

            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT dayid, usednum FROM t_limitgoodsbuy WHERE rid={0} AND goodsid={1}", roleID, goodsID);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    dayID = Convert.ToInt32(reader["dayid"].ToString());
                    usedNum = Convert.ToInt32(reader["usednum"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;

                ret = 0;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return ret;
        }

        #endregion 物品限购

        #region 邮件相关

        /// <summary>
        /// 获取邮件列表数据,邮件内容和物品列表省略
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<MailData> GetMailItemDataList(DBManager dbMgr, int rid)
        {
            List<MailData> list = new List<MailData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT mailid,senderrid,senderrname,sendtime,receiverrid,reveiverrname,readtime,isread," +
                                                " mailtype,hasfetchattachment,subject,content,yinliang,tongqian,yuanbao" +
                                                " from t_mail where receiverrid={0} ORDER by sendtime desc", rid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    MailData mailItemData = new MailData()
                    {
                        MailID = Convert.ToInt32(reader["mailid"].ToString()),
                        SenderRID = Convert.ToInt32(reader["senderrid"].ToString()),
                        SenderRName = reader["senderrname"].ToString(),
                        SendTime = reader["sendtime"].ToString(),
                        ReceiverRID = Convert.ToInt32(reader["receiverrid"].ToString()),
                        ReveiverRName = reader["reveiverrname"].ToString(),
                        ReadTime = reader["readtime"].ToString(),
                        IsRead = Convert.ToInt32(reader["isread"].ToString()),
                        MailType = Convert.ToInt32(reader["mailtype"].ToString()),
                        Hasfetchattachment = Convert.ToInt32(reader["hasfetchattachment"].ToString()),
                        Subject = reader["subject"].ToString(),
                        Content = "",
                        Yinliang = Convert.ToInt32(reader["yinliang"].ToString()),
                        Tongqian = Convert.ToInt32(reader["tongqian"].ToString()),
                        YuanBao = Convert.ToInt32(reader["yuanbao"].ToString()),
                    };

                    list.Add(mailItemData);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取邮件的数量
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="rid"></param>
        /// <param name="excludeIsRead">排除邮件读取状态(默认排除已读的)</param>
        /// <returns></returns>
        public static int GetMailItemDataCount(DBManager dbMgr, int rid, int excludeReadState = 0, int limitCount = 1)
        {
            MySQLConnection conn = null;
            int count = 0;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT mailid from t_mail where receiverrid={0} and isread<>{1} LIMIT 0,{2}", rid, excludeReadState, limitCount);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    count++;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return count;
        }

        /// <summary>
        /// 获取邮件具体数据[包括附件物品数据和邮件正文]
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static MailData GetMailItemData(DBManager dbMgr, int rid, int mailID)
        {
            MailData mailItemData = null;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT mailid,senderrid,senderrname,sendtime,receiverrid,reveiverrname,readtime,isread," +
                                                " mailtype,hasfetchattachment,subject,content,yinliang,tongqian,yuanbao" +
                                                " from t_mail where receiverrid={0} and mailid={1} ORDER by sendtime desc", rid, mailID);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                //有且仅有一封
                if (reader.Read())
                {
                    mailItemData = new MailData()
                    {
                        MailID = Convert.ToInt32(reader["mailid"].ToString()),
                        SenderRID = Convert.ToInt32(reader["senderrid"].ToString()),
                        SenderRName = reader["senderrname"].ToString(),
                        SendTime = reader["sendtime"].ToString(),
                        ReceiverRID = Convert.ToInt32(reader["receiverrid"].ToString()),
                        ReveiverRName = reader["reveiverrname"].ToString(),
                        ReadTime = reader["readtime"].ToString(),
                        IsRead = Convert.ToInt32(reader["isread"].ToString()),
                        MailType = Convert.ToInt32(reader["mailtype"].ToString()),
                        Hasfetchattachment = Convert.ToInt32(reader["hasfetchattachment"].ToString()),
                        Subject = reader["subject"].ToString(),
                        Content = Encoding.Default.GetString((byte[])reader["content"]),
                        Yinliang = Convert.ToInt32(reader["yinliang"].ToString()),
                        Tongqian = Convert.ToInt32(reader["tongqian"].ToString()),
                        YuanBao = Convert.ToInt32(reader["yuanbao"].ToString()),
                    };
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            //在这儿调用,保证conn 被还回连接池之后再调用，避免某个链接被过长时间占用
            if (null != mailItemData)
            {
                mailItemData.GoodsList = GetMailGoodsDataList(dbMgr, mailID);
            }

            return mailItemData;
        }

        /// <summary>
        /// 获取邮件附件数据
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<MailGoodsData> GetMailGoodsDataList(DBManager dbMgr, int mailID)
        {
            List<MailGoodsData> list = new List<MailGoodsData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT id,mailid,goodsid,forge_level,quality,Props,gcount,binding,origholenum,rmbholenum,jewellist,addpropindex,bornindex,lucky,strong,excellenceinfo,appendproplev, equipchangelife from t_mailgoods where mailid={0}", mailID);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    MailGoodsData mailItemData = new MailGoodsData()
                    {
                        Id = Convert.ToInt32(reader["id"].ToString()),
                        MailID = Convert.ToInt32(reader["mailid"].ToString()),
                        GoodsID = Convert.ToInt32(reader["goodsid"].ToString()),
                        Forge_level = Convert.ToInt32(reader["forge_level"].ToString()),
                        Quality = Convert.ToInt32(reader["quality"].ToString()),
                        Props = reader["Props"].ToString(),
                        GCount = Convert.ToInt32(reader["gcount"].ToString()),
                        Binding = Convert.ToInt32(reader["binding"].ToString()),
                        OrigHoleNum = Convert.ToInt32(reader["origholenum"].ToString()),
                        RMBHoleNum = Convert.ToInt32(reader["rmbholenum"].ToString()),
                        Jewellist = reader["jewellist"].ToString(),
                        AddPropIndex = Convert.ToInt32(reader["addpropindex"].ToString()),
                        BornIndex = Convert.ToInt32(reader["bornindex"].ToString()),
                        Lucky = Convert.ToInt32(reader["lucky"].ToString()),
                        Strong = Convert.ToInt32(reader["strong"].ToString()),
                        ExcellenceInfo = Convert.ToInt32(reader["excellenceinfo"].ToString()),
                        AppendPropLev = Convert.ToInt32(reader["appendproplev"].ToString()),
                        EquipChangeLifeLev = Convert.ToInt32(reader["equipchangelife"].ToString()),
                    };

                    list.Add(mailItemData);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return list;
        }

        /// <summary>
        /// 扫描新邮件信息，并且写入日志中 返回roleid, mailid 对应的列表
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static Dictionary<int, int> ScanLastMailIDListFromTable(DBManager dbMgr)
        {
            Dictionary<int, int> lastMailDct = new Dictionary<int, int>();

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                //每次最多处理20条,同一用户取mailid最大的那一条
                string cmdText = string.Format("SELECT MAX(mailid) as mailid, receiverrid from t_mailtemp  GROUP by mailid,receiverrid ORDER by receiverrid asc limit 0, 20");

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    int receiverrid = Convert.ToInt32(reader["receiverrid"].ToString());
                    if (!lastMailDct.ContainsKey(receiverrid))
                    {
                        lastMailDct.Add(receiverrid, Convert.ToInt32(reader["mailid"].ToString()));
                    }
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return lastMailDct;
        }

        #endregion 邮件相关

        #region 角色ID查询

        /// <summary>
        /// 通过角色名称在数据库查询角色ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int GetRoleIDByRoleName(DBManager dbMgr, string roleName, int zoneid)
        {
            int rid = -1;

            //传入非法的角色名称直接返回负数
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return rid;
            }

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT rid from t_roles WHERE rname='{0}' and zoneid={1}", roleName, zoneid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    rid = Convert.ToInt32(reader["rid"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return rid;
        }

        #endregion 角色ID查询

        #region 数据库ID字段自增长相关

        /// <summary>
        /// 返回最大的mailID
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int GetMaxMailID(DBManager dbMgr)
        {
            int maxValue = -1;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT MAX(mailid) as mymaxvalue from t_mail");
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    maxValue = Global.SafeConvertToInt32(reader["mymaxvalue"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return maxValue;
        }

        /// <summary>
        /// 返回最大的角色ID
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int GetMaxRoleID(DBManager dbMgr)
        {
            int maxValue = -1;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT MAX(rid) as mymaxvalue from t_roles");
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    maxValue = Global.SafeConvertToInt32(reader["mymaxvalue"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return maxValue;
        }

        /// <summary>
        /// 返回最大的帮会ID
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int GetMaxBangHuiID(DBManager dbMgr)
        {
            int maxValue = -1;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT MAX(bhid) as mymaxvalue from t_banghui");
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    maxValue = Global.SafeConvertToInt32(reader["mymaxvalue"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return maxValue;
        }

        /// <summary>
        /// 返回最大的抢购项ID
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int GetMaxQiangGouItemID(DBManager dbMgr)
        {
            int maxValue = -1;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT MAX(Id) as mymaxvalue from t_qianggouitem");
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    maxValue = Global.SafeConvertToInt32(reader["mymaxvalue"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return maxValue;
        }

        #endregion 数据库ID字段自增长相关

        #region 砸金蛋相关

        /// <summary>
        /// 获取砸金蛋数据记录
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<ZaJinDanHistory> QueryZaJinDanHistoryDataList(DBManager dbMgr, int roleID = -1)
        {
            List<ZaJinDanHistory> list = new List<ZaJinDanHistory>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string sWhere = "";
                if (roleID > 0)
                {
                    sWhere += string.Format(" Where rid={0} ", roleID);
                }
                else
                {
                    sWhere += string.Format(" Where gaingoodsnum>0 ");
                }

                string cmdText = string.Format("SELECT * FROM t_zajindanhist {0} ORDER BY operationtime DESC LIMIT 50", sWhere);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                int count = 0;
                while (reader.Read() && count < 100)
                {
                    ZaJinDanHistory historyData = new ZaJinDanHistory()
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        TimesSelected = Convert.ToInt32(reader["timesselected"].ToString()),
                        UsedYuanBao = Convert.ToInt32(reader["usedyuanbao"].ToString()),
                        UsedJinDan = Convert.ToInt32(reader["usedjindan"].ToString()),
                        GainGoodsId = Convert.ToInt32(reader["gaingoodsid"].ToString()),
                        GainGoodsNum = Convert.ToInt32(reader["gaingoodsnum"].ToString()),
                        GainGold = Convert.ToInt32(reader["gaingold"].ToString()),
                        GainYinLiang = Convert.ToInt32(reader["gainyinliang"].ToString()),
                        GainExp = Convert.ToInt32(reader["gainexp"].ToString()),
                        GoodPorp = reader["strprop"].ToString(),
                        OperationTime = reader["operationtime"].ToString(),
                    };

                    list.Add(historyData);
                    count++;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return list;
        }

        #endregion 砸金蛋相关

        #region 首充大礼/每日充值大礼

        /// <summary>
        /// 通过用户ID，查询是否已经领取过首充大礼
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int GetFirstChongZhiDaLiNum(DBManager dbMgr, string userID)
        {
            int totalNum = 0;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT COUNT(rid) AS totalnum from t_roles WHERE userid='{0}' and cztaskid>0", userID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                try
                {
                    if (reader.Read())
                    {
                        totalNum = Convert.ToInt32(reader["totalnum"].ToString());
                    }
                }
                catch (Exception)
                {
                    totalNum = 0;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return totalNum;
        }

        #endregion 首充大礼/每日充值大礼

        #region 开服在线大礼

        /// <summary>
        /// 通过天数，查询那个角色能够获取开服在线大礼
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int GetKaiFuOnlineAwardRoleID(DBManager dbMgr, int dayID, out int totalRoleNum)
        {
            totalRoleNum = 0;
            int roleID = -1;
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT rid, pvalue FROM t_roleparams WHERE pname='{0}'", RoleParamName.KaiFuOnlineDayID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                List<int> roleIDs = new List<int>();

                try
                {
                    while (reader.Read())
                    {
                        int pvalue = Global.SafeConvertToInt32(reader["pvalue"].ToString());
                        if (pvalue < dayID)
                        {
                            continue;
                        }

                        roleIDs.Add(Global.SafeConvertToInt32(reader["rid"].ToString()));
                    }
                }
                catch (Exception)
                {
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;

                if (roleIDs.Count > 0)
                {
                    Random rand = new Random();
                    roleID = roleIDs[rand.Next(0, roleIDs.Count)];
                    totalRoleNum = roleIDs.Count;
                }
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return roleID;
        }

        /// <summary>
        /// 查询开服在线大礼的列表
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static List<KaiFuOnlineAwardData> GetKaiFuOnlineAwardDataList(DBManager dbMgr, int zoneID)
        {
            List<KaiFuOnlineAwardData> kaiFuOnlineAwardDataList = new List<KaiFuOnlineAwardData>();
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT K.rid, R.zoneid, R.rname, K.dayid, K.totalrolenum FROM t_kfonlineawards AS K, t_roles AS R WHERE K.rid=R.rid AND K.zoneid={0}", zoneID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();                

                try
                {
                    while (reader.Read())
                    {
                        kaiFuOnlineAwardDataList.Add(new KaiFuOnlineAwardData()
                        {
                            RoleID = Global.SafeConvertToInt32(reader["rid"].ToString()),
                            ZoneID = Global.SafeConvertToInt32(reader["zoneid"].ToString()),
                            RoleName = reader["rname"].ToString(),
                            DayID = Global.SafeConvertToInt32(reader["dayid"].ToString()),
                            TotalRoleNum = Global.SafeConvertToInt32(reader["totalrolenum"].ToString()),
                        });
                    }
                }
                catch (Exception)
                {
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return kaiFuOnlineAwardDataList;
        }

        #endregion 开服在线大礼

        #region GM命令相关

        /// <summary>
        /// 查询GM命令的，并且写入日志中
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void ScanGMMsgFromTable(DBManager dbMgr, List<string> msgList)
        {
            MySQLConnection conn = null;

            try
            {
                int Id = 0;
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = "SELECT id, msg FROM t_gmmsg";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    Id = Math.Max(Convert.ToInt32(reader["id"].ToString()), Id);
                    string msg = System.Text.Encoding.Default.GetString((byte[])reader["msg"]);
                    msgList.Add(msg);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;

                //清空表
                if (Id > 0)
                {
                    cmdText = string.Format("DELETE FROM t_gmmsg WHERE id<={0}", Id);
                    cmd = new MySQLCommand(cmdText, conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;

                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
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

        #endregion GM命令相关

        #region 角色消费活动相关

        /// <summary>
        /// 返回充值排行信息 key:userid, value:totalmoney 的list,排行小于等于maxPaiHang的被返回,如果两个玩家消费额相等，先达到值的排名靠前
        /// dictionay顺序未知，直接返回排好序的list,这个函数返回的排行值是具体的真实货币值，外部使用时需要自行转换为元宝数，不在这转换
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="startTime">字符串时间</param>
        /// <param name="endTime">字符串时间</param>
        /// <param name="maxPaiHang">必须大于等于1,默认返回排名前三的信息</param>
        /// <returns></returns>
        public static List<InputKingPaiHangData> GetUserUsedMoneyPaiHang1(DBManager dbMgr, string startTime, string endTime, int maxPaiHang = 3)
        {
            List<InputKingPaiHangData> lsPaiHang = new List<InputKingPaiHangData>();

            if (maxPaiHang < 1)
            {
                return lsPaiHang;
            }

            int rid = -1, totalmoney = 0;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                //这个位置需要采用unix时间戳进行定位,普通情况下，unix时间戳和inputtime是同一个值，单inputtime时间戳比unix要大一点
                //对用户充值而言，采用unix时间戳比较准确,此外，默认任务玩家充值信息只在t_mallbuy表中有，而gm充值信息只在t_qizhengebuy表中，t_zajindanhist
                //如果存在某个玩家在两个表中都有充值，该玩家可能有3条数据位于取出的数据中，这时，需要做过滤操作，所以，返回数据最大行数乘以3,
                //然后再对返回的数据进行重复玩家过滤
                string cmdText = string.Format("SELECT t_mallbuy.rid, sum(t_mallbuy.totalprice) as totalmoney, max(t_mallbuy.buytime) as time from t_mallbuy,t_roles  where t_mallbuy.rid=t_roles.rid and buytime>='{0}' and buytime<='{1}' and t_roles.isdel=0 GROUP by rid " +
                                 " union " +
                                 " SELECT t_zajindanhist.rid, sum(usedyuanbao/timesselected) as totalmoney, max(operationtime) as time from t_zajindanhist,t_roles where t_zajindanhist.rid=t_roles.rid and t_roles.isdel=0 and operationtime>='{0}' and operationtime<='{1}' GROUP by rid " +
                                 " union " +
                                 " SELECT t_qizhengebuy.rid, sum(totalprice) as totalmoney, max(buytime) as time from t_qizhengebuy,t_roles where buytime>='{0}' and buytime<='{1}' and t_qizhengebuy.rid=t_roles.rid and t_roles.isdel=0  GROUP by rid order by totalmoney desc,time asc " +
                                 " limit 0, {2} ", startTime, endTime, maxPaiHang * 3);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                List<int> tmp = new List<int>();
                int count = 0;
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //最多返回maxPaiHang 条数据
                while (reader.Read())
                {
                    count++;

                    rid = Convert.ToInt32(reader["rid"].ToString());
                    totalmoney = (int)Convert.ToDouble(reader["totalmoney"].ToString());

                    if (totalmoney > 0)
                    {
                        if (!tmp.Contains(rid))
                        {
                            tmp.Add(rid);

                            InputKingPaiHangData phData = new InputKingPaiHangData
                            {
                                UserID = rid.ToString(),
                                PaiHang = count,
                                PaiHangTime = now,
                                PaiHangValue = totalmoney
                            };

                            lsPaiHang.Add(phData);
                        }
                        else
                        {
                            //对在两个表中都有充值[一般是gm]特殊处理
                            InputKingPaiHangData phData = lsPaiHang[tmp.IndexOf(rid)];
                            phData.PaiHangValue += totalmoney;
                        }
                    }

                    //最多取回 maxPaiHang条
                    if (lsPaiHang.Count >= maxPaiHang)
                    {
                        break;
                    }
                }

                //对返回结果进行排序，按照PaiHangValue 由大到小排序，然后更新PaiHang值

                Comparison<InputKingPaiHangData> com = new Comparison<InputKingPaiHangData>(InputKingPaiHangDataCompare);
                lsPaiHang.Sort(com);

                for (int n = 0; n < lsPaiHang.Count; n++)
                {
                    lsPaiHang[n].PaiHang = n + 1;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return lsPaiHang;
        }

        /// <summary>
        /// 返回玩家某时间内在某区的充值总额【这儿返回的是真实货币单位，要转换为元宝，必须调用相应的转换函数】
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetUserUsedMoney1(DBManager dbMgr, int rid, string startTime, string endTime)
        {
            int totalmoney = 0;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                //这个位置需要采用unix时间戳进行定位,普通情况下，unix时间戳和inputtime是同一个值，单inputtime时间戳比unix要大一点
                //对用户充值而言，采用unix时间戳比较准确,此外，默认任务玩家充值信息只在t_mallbuy表中有，而gm充值信息只在t_qizhengebuy表中，t_zajindanhist
                //如果存在某个玩家在两个表中都有充值，该玩家可能有3条数据位于取出的数据中，这时，需要做过滤操作，所以，返回数据最大行数乘以3,
                //然后再对返回的数据进行重复玩家过滤
                string cmdText = string.Format("SELECT rid, sum(totalprice) as totalmoney, max(buytime) as time from t_mallbuy where buytime>='{0}' and buytime<='{1}' and rid={2} GROUP by rid " +
                                 " union " +
                                 " SELECT rid, sum(usedyuanbao/timesselected) as totalmoney, max(operationtime) as time from t_zajindanhist where operationtime>='{0}' and operationtime<='{1}' and rid={2} GROUP by rid " +
                                 " union " +
                                 " SELECT rid, sum(totalprice) as totalmoney, max(buytime) as time from t_qizhengebuy where buytime>='{0}' and buytime<='{1}' and rid={2} GROUP by rid"
                                 , startTime, endTime, rid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    totalmoney += (int)Convert.ToDouble(reader["totalmoney"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return totalmoney;
        }

        #endregion 角色消费活动相关

        #region 月度抽奖

        /// <summary>
        /// 获取月度抽奖数据记录
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static List<YueDuChouJiangData> QueryYueDuChouJiangHistoryDataList(DBManager dbMgr, int roleID = -1)
        {
            List<YueDuChouJiangData> list = new List<YueDuChouJiangData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string sWhere = "";
                if (roleID > 0)
                {
                    sWhere += string.Format(" Where rid={0}", roleID);
                }
                else
                {
                    sWhere += string.Format(" Where gaingoodsnum>0 ");
                }

                string cmdText = string.Format("SELECT * FROM t_yueduchoujianghist {0} ORDER BY operationtime DESC LIMIT 50", sWhere);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                int count = 0;
                while (reader.Read() && count < 100)
                {
                    YueDuChouJiangData historyData = new YueDuChouJiangData()
                    {
                        RoleID = Convert.ToInt32(reader["rid"].ToString()),
                        RoleName = Global.FormatRoleName(reader["zoneid"].ToString(), reader["rname"].ToString()),
                        GainGoodsId = Convert.ToInt32(reader["gaingoodsid"].ToString()),
                        GainGoodsNum = Convert.ToInt32(reader["gaingoodsnum"].ToString()),
                        GainGold = Convert.ToInt32(reader["gaingold"].ToString()),
                        GainYinLiang = Convert.ToInt32(reader["gainyinliang"].ToString()),
                        GainExp = Convert.ToInt32(reader["gainexp"].ToString()),
                        OperationTime = reader["operationtime"].ToString(),
                    };

                    list.Add(historyData);
                    count++;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return list;
        }

        #endregion 月度抽奖

        #region 血色堡垒
        /// <summary>
        /// 返回角色当天进入血色堡垒的次数
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetBloodCastleEnterCount(DBManager dbMgr, int roleid, int nDate, int activityid)
        {
            int ret = 0;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT triggercount from t_dayactivityinfo where roleid={0} and activityid={1} and timeinfo={2} ",
                    roleid, activityid, nDate);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                    ret = Convert.ToInt32(reader["triggercount"].ToString());
                
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return ret;
        }
        #endregion 血色堡垒

        #region 日常活动最高积分

        /// <summary>
        /// 返回日常活动最高积分
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static List<int> GetDayActivityTotlePoint(DBManager dbMgr, int activityid)
        {
            List<int> lData = new List<int>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT roleid, totalpoint FROM t_dayactivityinfo WHERE totalpoint>0 AND activityid = {0} ORDER BY totalpoint DESC LIMIT 1", activityid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                int nRoleid = -1;
                int nPoint  = -1;

                if (reader.Read())
                {
                    nRoleid = Convert.ToInt32(reader["roleid"].ToString());
                    nPoint = Convert.ToInt32(reader["totalpoint"].ToString());
                }

                if (nRoleid != -1 && nPoint != -1)
                {
                    lData.Add(nRoleid);
                    lData.Add(nPoint);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return lData;
        }

        /// <summary>
        /// 返回玩家日常活动最高积分
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetRoleDayActivityPoint(DBManager dbMgr, int nRole, int activityid)
        {
            int nPoint = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT totalpoint FROM t_dayactivityinfo WHERE roleid = {0} AND activityid = {1}", nRole, activityid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                    nPoint = Convert.ToInt32(reader["totalpoint"].ToString());

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return nPoint;
        }

        #endregion 日常活动最高积分

        #region 崇拜
        
        /// <summary>
        /// 查询是否崇拜了某人
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int QueryPlayerAdmiredAnother(DBManager dbMgr, int roleAID, int roleBID, int nDate)
        {
            int nID = -1;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT adorationroleid from t_adorationinfo where roleid={0} and adorationroleid={1} and dayid={2}", roleAID, roleBID, nDate);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                    nID = Convert.ToInt32(reader["adorationroleid"].ToString());

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return nID;
        }

        #endregion 崇拜

        #region 每日在线奖励相关
        
        /// <summary>
        /// 查询每日在线奖励数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static List<int> QueryPlayerEveryDayOnLineAwardGiftInfo(DBManager dbMgr, int roleID)
        {
            List<int> lData = new List<int>();

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT everydayonlineawardstep, geteverydayonlineawarddayid from t_huodong where roleid={0}", roleID);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                if (reader.Read())
                {
                    int nValue = 0;

                    nValue = Convert.ToInt32(reader["everydayonlineawardstep"].ToString());
                    lData.Add(nValue);
                    
                    nValue = Convert.ToInt32(reader["geteverydayonlineawarddayid"].ToString());
                    
                    lData.Add(nValue);
                }
                else
                    lData = null;

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return lData;
        }

        #endregion 每日在线奖励相关

        #region 推送相关

        /// <summary>
        /// 查询在条件范围内的玩家 返回列表
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static List<PushMessageData> QueryPushMsgUerList(DBManager dbMgr, int nCondition)
        {
            List<PushMessageData> list = new List<PushMessageData>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                DateTime time = DateTime.Now;

                string cmdText = string.Format("SELECT userid, pushid, lastlogintime from t_pushmessageinfo where NOW() <= ADDDATE(lastlogintime, {0})", nCondition+1);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {   
                    PushMessageData PushMsgData = new PushMessageData()
                    {
                        UserID = reader["userid"].ToString(),
                        PushID = reader["pushid"].ToString(),
                        LastLoginTime = reader["lastlogintime"].ToString()
                    };

                    list.Add(PushMsgData);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return list;
        }

        #endregion 推送相关

        #region 魔晶兑换相关

        /// <summary>
        /// 查询绑定钻石兑换信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static Dictionary<int, int> QueryMoJingExchangeDict(DBManager dbMgr, int nRoleid, int nDayID)
        {
            Dictionary<int, int> TmpDict = new Dictionary<int, int>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("SELECT exchangeid, exchangenum FROM t_mojingexchangeinfo WHERE roleid = {0} AND dayid = {1}", nRoleid, nDayID);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    int nExchangeid = Convert.ToInt32(reader["exchangeid"].ToString());
                    int nNum        = Convert.ToInt32(reader["exchangenum"].ToString());

                    TmpDict.Add(nExchangeid, nNum);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return TmpDict;
        }

        #endregion 魔晶兑换相关

        #region 查询资源找回数据

       
        /// <summary>
        /// 查询资源找回数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static Dictionary<int,OldResourceInfo> QueryResourceGetInfo(DBManager dbMgr, int nRoleid)
        {
            Dictionary<int, OldResourceInfo> datadict = new Dictionary<int, OldResourceInfo>();
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                DateTime time = DateTime.Now;

                string cmdText = string.Format("SELECT type, exp, leftCount,mojing,bandmoney,zhangong,chengjiu,shengwang,bangzuan,xinghun from t_resourcegetinfo where roleid = {0} AND hasget = {1}", nRoleid, 0);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    OldResourceInfo data = new OldResourceInfo()
                    {
                        type =Global.SafeConvertToInt32( reader["type"].ToString()),
                        exp = Global.SafeConvertToInt32(reader["exp"].ToString())>0?Global.SafeConvertToInt32(reader["exp"].ToString()):0,
                        leftCount = Global.SafeConvertToInt32(reader["leftCount"].ToString())>0?Global.SafeConvertToInt32(reader["leftCount"].ToString()):0,
                        mojing = Global.SafeConvertToInt32(reader["mojing"].ToString())>0?Global.SafeConvertToInt32(reader["mojing"].ToString()):0,
                        bandmoney = Global.SafeConvertToInt32(reader["bandmoney"].ToString())>0?Global.SafeConvertToInt32(reader["bandmoney"].ToString()):0,
                        zhangong = Global.SafeConvertToInt32(reader["zhangong"].ToString())>0?Global.SafeConvertToInt32(reader["zhangong"].ToString()):0,
                        chengjiu = Global.SafeConvertToInt32(reader["chengjiu"].ToString())>0?Global.SafeConvertToInt32(reader["chengjiu"].ToString()):0,
                        shengwang = Global.SafeConvertToInt32(reader["shengwang"].ToString())>0?Global.SafeConvertToInt32(reader["shengwang"].ToString()):0,
                        bandDiamond = Global.SafeConvertToInt32(reader["bangzuan"].ToString()) > 0 ? Global.SafeConvertToInt32(reader["bangzuan"].ToString()) : 0,
                        xinghun = Global.SafeConvertToInt32(reader["xinghun"].ToString()) > 0 ? Global.SafeConvertToInt32(reader["xinghun"].ToString()) : 0,
                        roleId = nRoleid
                    };

                    datadict[data.type] = data;  
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return datadict;
        }
        #endregion

        /// <summary>
        /// 新查找消费数额
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="rid"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static int GetUserUsedMoney(DBManager dbMgr, int rid, string startTime, string endTime)
        {
            int totalmoney = 0;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT rid, sum(amount) as totalmoney  from t_consumelog where cdate>='{0}' and cdate<='{1}' and rid={2} GROUP by rid "
                                 , startTime, endTime, rid);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                while (reader.Read())
                {
                    totalmoney = (int)Convert.ToDouble(reader["totalmoney"].ToString());
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return totalmoney;
        }

        /// <summary>
        /// 新消费排行查询
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="maxPaiHang"></param>
        /// <returns></returns>
        public static List<InputKingPaiHangData> GetUserUsedMoneyPaiHang(DBManager dbMgr, string startTime, string endTime, int maxPaiHang = 3)
        {
            List<InputKingPaiHangData> lsPaiHang = new List<InputKingPaiHangData>();

            if (maxPaiHang < 1)
            {
                return lsPaiHang;
            }

            int rid = -1, totalmoney = 0;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT t_consumelog.rid, sum(t_consumelog.amount) as totalmoney, max(cdate) as time from t_consumelog,t_roles  where t_consumelog.rid=t_roles.rid and cdate>='{0}' and cdate<='{1}' and t_roles.isdel=0 GROUP by rid "
                                                +" order by totalmoney desc,time asc " +
                                                 " limit 0, {2} "
                                 , startTime, endTime, maxPaiHang * 2);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                List<int> tmp = new List<int>();
                int count = 0;
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //最多返回maxPaiHang 条数据
                while (reader.Read())
                {
                    count++;

                    rid = Convert.ToInt32(reader["rid"].ToString());
                    totalmoney = (int)Convert.ToDouble(reader["totalmoney"].ToString());

                    if (totalmoney > 0)
                    {
                        if (!tmp.Contains(rid))
                        {
                            tmp.Add(rid);

                            InputKingPaiHangData phData = new InputKingPaiHangData
                            {
                                UserID = rid.ToString(),
                                PaiHang = count,
                                PaiHangTime = now,
                                PaiHangValue = totalmoney
                            };

                            lsPaiHang.Add(phData);
                        }
                        else
                        {
                            //对在两个表中都有充值[一般是gm]特殊处理
                            InputKingPaiHangData phData = lsPaiHang[tmp.IndexOf(rid)];
                            phData.PaiHangValue += totalmoney;
                        }
                    }

                    //最多取回 maxPaiHang条
                    if (lsPaiHang.Count >= maxPaiHang)
                    {
                        break;
                    }
                }

                //对返回结果进行排序，按照PaiHangValue 由大到小排序，然后更新PaiHang值

                Comparison<InputKingPaiHangData> com = new Comparison<InputKingPaiHangData>(InputKingPaiHangDataCompare);
                lsPaiHang.Sort(com);

                for (int n = 0; n < lsPaiHang.Count; n++)
                {
                    lsPaiHang[n].PaiHang = n + 1;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return lsPaiHang;
        }

        #region MU VIP相关
        
        /// <summary>
        /// 查询VIP等级奖励标记信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int QueryVipLevelAwardFlagInfo(DBManager dbMgr, int nId)
        {
            int nFlag = 0;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT vipawardflag from t_roles WHERE rid = '{0}' ", nId);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                try
                {
                    if (reader.Read())
                    {
                        nFlag = Convert.ToInt32(reader["vipawardflag"].ToString());
                    }
                }
                catch (Exception)
                {
                    nFlag = 0;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return nFlag;
        }

        /// <summary>
        /// 根据账号查询VIP等级奖励标记信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int QueryVipLevelAwardFlagInfoByUserID(DBManager dbMgr, string struseid)
        {
            int nFlag = 0;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT vipawardflag FROM t_roles WHERE userid = '{0}' ", struseid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                try
                {
                    if (reader.Read())
                    {   
                        if (Convert.ToInt32(reader["vipawardflag"].ToString()) > 0)
                        {
                            nFlag = Convert.ToInt32(reader["vipawardflag"].ToString());
                        }
                        
                    }
                }
                catch (Exception)
                {
                    nFlag = 0;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return nFlag;
        }

        #endregion MU VIP相关

       

        #region 最后一次登陆的角色
        public static int LastLoginRole(DBManager dbMgr,string uid)
        {
            int rid = 0;

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT rid FROM t_roles WHERE userid = '{0}' AND isdel=0   ORDER BY lasttime DESC LIMIT 1 ", uid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                try
                {
                    if (reader.Read())
                    {
                        if (Convert.ToInt32(reader["rid"].ToString()) > 0)
                        {
                            rid = Convert.ToInt32(reader["rid"].ToString());
                        }

                    }
                }
                catch (Exception)
                {
                    rid = 0;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return rid;
        }
        #endregion

        #region 检查用户有没有这个角色
        public static bool GetUserRole(DBManager dbMgr, string userID, int roleID)
        {
            int rid = 0;

            MySQLConnection conn = null;
            bool result = false;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT rid FROM t_roles WHERE userid = '{0}' AND rid={1} AND isdel=0", userID, roleID);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                try
                {
                    if (reader.Read())
                    {
                        if (Convert.ToInt32(reader["rid"].ToString()) == roleID)
                        {
                            result = true;
                        }

                    }
                }
                catch (Exception)
                {
                    result = false;
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);

                cmd.Dispose();
                cmd = null;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return result;
        }
        #endregion
    }
}
