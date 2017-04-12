using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySQLDriverCS;
using System.Data;
using System.Windows;
using Server.Tools;
using Server.Data;
using GameDBServer.Logic;

namespace GameDBServer.DB
{
    /// <summary>
    /// 数据库写操作类
    /// </summary>
    public class DBWriter
    {
        /// <summary>
        /// 添加一个新用户角色
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int CreateRole(DBManager dbMgr, string userID, string userName, int sex, int occup, string roleName, int zoneID, int bagnum, int isflashplayer)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string mapPos = "-1:0:-1:-1";
            int roleID = -1;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = "SELECT count(*) as RoleCount FROM t_roles";
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                int nCount = 0;
                while (reader.Read())
                {
                    nCount = int.Parse(reader["RoleCount"].ToString());
                }

                if (null != cmd)
                {
                    cmd.Dispose();
                    cmd = null;
                }

                if (nCount >= GameDBManager.DBAutoIncreaseStepValue)
                {
                    roleID = -2;
                    return roleID;
                }
            }
            catch (MySQLException)
            {
                roleID = -2; //创建失败(角色名称重复)
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            try
            {
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_roles (userid, rname, sex, occupation, position, regtime, lasttime, biguantime, zoneid, bhname, chenghao, username, bagnum, isflashplayer) VALUES('{0}', '{1}', {2}, {3}, '{4}', '{5}', '{6}', '{7}', {8}, '', '', '{9}', {10}, {11})",
                    userID, roleName, sex, occup, mapPos, today, today, today, zoneID, userName, bagnum, isflashplayer);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    error = true;
                    roleID = -1; //创建失败(角色名称重复)

                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                try
                {
                    if (!error)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            roleID = Convert.ToInt32(reader[0].ToString());
                        }

                        cmd.Dispose();
                        cmd = null;
                    }
                }
                catch (MySQLException)
                {
                    if (null != cmd)
                    {
                        cmd.Dispose();
                        cmd = null;
                    }

                    roleID = -2; //创建失败(角色名称重复)
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
        /// 删除一个用户角色
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool RemoveRole(DBManager dbMgr, int roleID)
        {
            bool ret = false;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET isdel=1, deltime='{0}' WHERE rid={1}", today, roleID);

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

            return ret;
        }

        /// <summary>
        /// 删除一个用户角色
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool RemoveRoleByName(DBManager dbMgr, string roleName)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET isdel=1 WHERE rname='{0}'", roleName);

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

            return ret;
        }

        /// <summary>
        /// 恢复删除一个用户角色
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UnRemoveRole(DBManager dbMgr, string roleName)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET isdel=0 WHERE rname='{0}'", roleName);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的最后登录时间和登录次数
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleLoginInfo(DBManager dbMgr, int roleID, int loginNum, int loginDayID, int loginDayNum, string userid, int zoneid, string ip)
        {
            bool ret = false;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET lasttime='{0}', loginnum={1}, logindayid={2}, logindaynum={3} WHERE rid={4}", today, loginNum, loginDayID, loginDayNum, roleID);

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

                try
                {
                    DateTime now = DateTime.Now;
                    cmdText = string.Format("INSERT INTO t_login (userid,dayid,rid,logintime,logouttime,ip,mac,zoneid,onlinesecs,loginnum) " +
                                                "VALUES('{0}',{1},{2},'{3}','{4}','{5}','{6}',{7},0,1) ON DUPLICATE KEY UPDATE loginnum=loginnum+1,rid={2}"
                                                , userid, Global.GetOffsetDay(now), roleID, today, Global.GetDayEndTime(now), ip, null, zoneid);
                    cmd = new MySQLCommand(cmdText, conn);
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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的离线时间
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleLogOff(DBManager dbMgr, int roleID, string userid, int zoneid, string ip, int onlineSecs)
        {
            bool ret = false;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET logofftime='{0}' WHERE rid={1}", today, roleID);

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

                try
                {
                    DateTime now = DateTime.Now;
                    cmdText = string.Format("INSERT INTO t_login (userid,dayid,rid,logintime,logouttime,ip,mac,zoneid,onlinesecs,loginnum) " +
                                                "VALUES('{0}',{1},{2},'{3}','{4}','{5}','{6}',{7},{8},1) ON DUPLICATE KEY UPDATE logouttime='{4}',onlinesecs=onlineSecs+{8}"
                                                , userid, Global.GetOffsetDay(now), roleID, Global.GetDayStartTime(now), today, ip, null, zoneid, onlineSecs);
                    cmd = new MySQLCommand(cmdText, conn);
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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的相关在线时长
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleOnlineSecs(DBManager dbMgr, int roleID, int totalOnlineSecs, int antiAddictionSecs)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET totalonlinesecs={0}, antiaddictionsecs={1} WHERE rid={2}", totalOnlineSecs, antiAddictionSecs, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的闭关开始时间
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleBiGuanTime(DBManager dbMgr, int roleID, long biguanTime)
        {
            bool ret = false;
            string today = (new DateTime(biguanTime * 10000)).ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET biguantime='{0}' WHERE rid={1}", today, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的角斗场称号信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleBattleNameInfo(DBManager dbMgr, int roleID, long startTime, int nameIndex)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET battlenamestart={0}, battlenameindex={1} WHERE rid={2}", startTime, nameIndex, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的充值TaskID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleCZTaskID(DBManager dbMgr, int roleID, int czTaskID)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET cztaskid={0} WHERE rid={1}", czTaskID, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的单次奖励历史标志位
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleOnceAwardFlag(DBManager dbMgr, int roleID, long onceawardflag)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET onceawardflag={0} WHERE rid={1}", onceawardflag, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的永久禁止聊天标志
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleBanChat(DBManager dbMgr, int roleID, int banChat)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET banchat={0} WHERE rid={1}", banChat, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的永久禁止登陆标志
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleBanLogin(DBManager dbMgr, int roleID, int banLogin)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET banlogin={0} WHERE rid={1}", banLogin, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的帮会信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleBangHuiInfo(DBManager dbMgr, int roleID, int faction, string bhName, int bhZhiWu)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET faction={0}, bhname='{1}', bhzhiwu={2} WHERE rid={3}", faction, bhName, bhZhiWu, roleID);

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

            return ret;
        }

        /// <summary>
        /// 清空所有指定帮会用户的帮会信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool ClearAllRoleBangHuiInfo(DBManager dbMgr, int bhid)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET faction=0, bhname='', bhzhiwu=0, chenghao='', banggong=0 WHERE faction={0}", bhid);

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

            return ret;
        }

        /// <summary>
        /// 清空上次加入帮会的遗留信息(根据角色ID)
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool ClearLastBangHuiInfoByRoleID(DBManager dbMgr, int roleID)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET chenghao='', banggong=0 WHERE rid={0}", roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的帮会被邀请验证信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleBangHuiVerify(DBManager dbMgr, int roleID, int toVerify)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET bhverify={0} WHERE rid={1}", toVerify, roleID);

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

            return ret;
        }

        /// <summary>
        /// 添加一个新任务
        /// </summary>
        public static int NewTask(DBManager dbMgr, int roleID, int npcID, int taskID, string addtime, int focus, int nStarLevel)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_tasks (taskid, rid, value1, value2, focus, isdel, addtime, starlevel) VALUES({0}, {1}, {2}, {3}, {4}, {5}, '{6}', {7})", taskID, roleID, 0, 0, focus, 0, addtime, nStarLevel);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    error = true;
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                try
                {
                    if (!error)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            ret = Convert.ToInt32(reader[0].ToString());
                        }

                        cmd.Dispose();
                        cmd = null;
                    }
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
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
        /// 更新一个用户角色的位置信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRolePosition(DBManager dbMgr, int roleID, string position)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET position='{0}' WHERE rid={1}", position, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的级别和经验
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleExpLevel(DBManager dbMgr, int roleID, int level, long experience)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET level={0}, experience={1} WHERE rid={2}", level, experience, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的内力
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleInterPower(DBManager dbMgr, int roleID, int interPower)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET interpower={0} WHERE rid={1}", interPower, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的游戏币1
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleMoney1(DBManager dbMgr, int roleID, int money)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET money1={0} WHERE rid={1}", money, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的银两
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleYinLiang(DBManager dbMgr, int roleID, int yinLiang)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET yinliang={0} WHERE rid={1}", yinLiang, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的金币
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleGold(DBManager dbMgr, int roleID, int gold)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET money2={0} WHERE rid={1}", gold, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的帮贡
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleBangGong(DBManager dbMgr, int roleID, int bgDayID1, int bgMoney, int bgDayID2, int bgGoods, int bangGong)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET bgdayid1={0}, bgmoney={1}, bgdayid2={2}, bggoods={3}, banggong={4} WHERE rid={5}",
                    bgDayID1, bgMoney, bgDayID2, bgGoods, bangGong, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户的点卷
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateUserMoney(DBManager dbMgr, string userID, int userMoney)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "";
                cmdText = string.Format("INSERT INTO t_money (userid, money) VALUES('{0}', {1}) ON DUPLICATE KEY UPDATE money={2}", userID, userMoney, userMoney);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户的元宝表的信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateUserMoney2(DBManager dbMgr, string userID, int userMoney, int realMoney, int giftID, int giftJiFen)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "";
                cmdText = string.Format("INSERT INTO t_money (userid, money, realmoney, giftid, giftjifen) VALUES('{0}', {1}, {2}, {3}, {4}) ON DUPLICATE KEY UPDATE money={5}, realmoney={6}, giftid={7}, giftjifen={8}",
                    userID, userMoney, realMoney, giftID, giftJiFen, userMoney, realMoney, giftID, giftJiFen);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户的充值积分
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateUserGiftJiFen(DBManager dbMgr, string userID, int giftJiFen)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "";
                cmdText = string.Format("INSERT INTO t_money (userid, giftjifen) VALUES('{0}', {1}) ON DUPLICATE KEY UPDATE giftjifen={2}", userID, giftJiFen, giftJiFen);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的缺省技能ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleDefSkillID(DBManager dbMgr, int roleID, int defSkillID)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET skillid={0} WHERE rid={1}", defSkillID, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的劫镖信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleJieBiaoInfo(DBManager dbMgr, int roleID, int jieBiaoDayID, int jieBiaoDayNum)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET jiebiaodayid={0}, jiebiaonum={1} WHERE rid={2}", jieBiaoDayID, jieBiaoDayNum, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的自动喝药的设置
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleAutoDrink(DBManager dbMgr, int roleID, int autoLifeV, int autoMagicV)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET autolife={0}, automagic={1} WHERE rid={2}", autoLifeV, autoMagicV, roleID);

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

            return ret;
        }

        /// <summary>
        /// 移动一个物品
        /// </summary>
        public static int MoveGoods(DBManager dbMgr, int roleID, int goodsDbID)
        {
            int ret = -10;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_goods SET rid={0}, site=0 WHERE Id={1}", roleID, goodsDbID);

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

                ret = 0;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            if (ret >= 0)
            {
                // 设置物品扩展属性表的某物品记录角色ID
                DBWriter.UpdateGoodsPropsRoleID(dbMgr, roleID, goodsDbID);
            }

            return ret;
        }

        /// <summary>
        /// 添加一个新的物品
        /// </summary>
        public static int NewGoods(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int quality, string props, int forgeLevel, int binding, int site, string jewelList, int bagindex, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_goods (rid, goodsid, quality, Props, gcount, forge_level, binding, site, jewellist, bagindex, endtime, addpropindex, bornindex, lucky, strong, excellenceinfo, appendproplev, equipchangelife, ehinfo) VALUES({0}, {1}, {2}, '{3}', {4}, {5}, {6}, {7}, '{8}', {9}, '{10}', {11}, {12}, {13}, {14}, {15}, {16}, {17}, '')",
                    roleID, goodsID, quality, props, goodsNum, forgeLevel, binding, site, jewelList, bagindex, endTime, addPropIndex, bornIndex, lucky, strong, ExcellenceProperty, nAppendPropLev, nEquipChangeLife);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    error = true;
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0} {1}", cmdText, ex.ToString()));
                }

                cmd.Dispose();
                cmd = null;

                try
                {
                    if (!error)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            ret = Convert.ToInt32(reader[0].ToString());
                        }
                    }
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
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
        /// 格式化修改数据库的字符串
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="startIndex"></param>
        /// <param name="fieldNames"></param>
        /// <param name="tableName"></param>
        /// <param name="fieldTypes"></param>
        /// <returns></returns>
        public static string FormatUpdateSQL(int id, string[] fields, int startIndex, string[] fieldNames, string tableName, byte[] fieldTypes, string idName = "Id")
        {
            bool first = true;
            string sql = "UPDATE " + tableName + " SET ";
            for (int i = 0; i < fieldNames.Count(); i++)
            {
                if (fields[startIndex + i] == "*")
                {
                    continue;
                }

                if (!first)
                {
                    sql += ", ";
                }

                if (fieldTypes[i] == 0) //数字
                {
                    sql += string.Format("{0}={1}", fieldNames[i], fields[startIndex + i]);
                }
                else if (fieldTypes[i] == 1)//字符串
                {
                    sql += string.Format("{0}='{1}'", fieldNames[i], fields[startIndex + i]);
                }
                else if (fieldTypes[i] == 2)//数字叠加操作
                {
                    sql += string.Format("{0}={1}+{2}", fieldNames[i], fieldNames[i], fields[startIndex + i]);
                }
                else if (fieldTypes[i] == 3)//用$替代:的时间格式
                {
                    sql += string.Format("{0}='{1}'", fieldNames[i], fields[startIndex + i].Replace('$', ':'));
                }

                first = false;
            }

            sql += string.Format(" WHERE {0}={1}", idName, id);
            return sql;
        }

        /// <summary>
        /// 修改物品
        /// </summary>
        public static int UpdateGoods(DBManager dbMgr, int id, string[] fields, int startIndex)
        {
            int ret = -1;
            MySQLConnection conn = null;
            string[] fieldNames = { "isusing", "forge_level", "starttime", "endtime", "site", "quality", "Props", "gcount", "jewellist", "bagindex", "salemoney1", "saleyuanbao", "saleyinpiao", "binding", "addpropindex", "bornindex", "lucky", "strong", "excellenceinfo", "appendproplev", "equipchangelife" };
            byte[] fieldTypes = { 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = FormatUpdateSQL(id, fields, startIndex, fieldNames, "t_goods", fieldTypes);

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

        /// <summary>
        /// 删除物品
        /// </summary>
        public static int RemoveGoods(DBManager dbMgr, int id)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_goods SET gcount=0 WHERE Id={0}", id);

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

        /// <summary>
        /// 删除物品
        /// </summary>
        public static int MoveGoodsDataToBackupTable(DBManager dbMgr, int id)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_goods_bak SELECT *,0,NOW(),0 FROM t_goods WHERE Id={0}", id);
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                    return ret;
                }

                cmd.Dispose();

                cmdText = string.Format("DELETE FROM t_goods WHERE Id={0}", id);
                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                    return ret;
                }

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

        /// <summary>
        /// 修改任务
        /// </summary>
        public static int UpdateTask(DBManager dbMgr, int dbID, string[] fields, int startIndex)
        {
            int ret = -1;
            MySQLConnection conn = null;
            string[] fieldNames = { "focus", "value1", "value2" };
            byte[] fieldTypes = { 0, 0, 0 };

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = FormatUpdateSQL(dbID, fields, startIndex, fieldNames, "t_tasks", fieldTypes);

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

        /// <summary>
        /// 完成一个任务
        /// </summary>
        public static bool CompleteTask(DBManager dbMgr, int roleID, int npcID, int taskID, int dbID)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_taskslog (taskid, rid, count) VALUES({0}, {1}, 1) ON DUPLICATE KEY UPDATE count=count+1", taskID, roleID);

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

                cmdText = string.Format("UPDATE t_tasks SET isdel=1 WHERE Id={0}", dbID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
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

            return ret;
        }

        /// <summary>
        /// 完成一个任务
        /// </summary>
        public static bool DeleteTask(DBManager dbMgr, int roleID, int taskID, int dbID)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_tasks SET isdel=1 WHERE Id={0}", dbID);

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

            return ret;
        }

        /// <summary>
        /// 添加一个朋友
        /// </summary>
        public static int AddFriend(DBManager dbMgr, int dbID, int roleID, int otherID, int friendType)
        {
            int ret = dbID;
            MySQLConnection conn = null;

            try
            {
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_friends (myid, otherid, friendType) VALUES({0}, {1}, {2}) ON DUPLICATE KEY UPDATE friendType={3}", roleID, otherID, friendType, friendType);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    error = true;
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                try
                {
                    if (!error && dbID < 0)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            ret = Convert.ToInt32(reader[0].ToString());
                        }
                    }
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
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
        /// 删除一个朋友
        /// </summary>
        public static bool RemoveFriend(DBManager dbMgr, int dbID, int roleID)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("DELETE FROM t_friends WHERE Id={0}", dbID);

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

            return ret;
        }

        /// <summary>
        /// 修改角色的PK模式
        /// </summary>
        public static bool UpdatePKMode(DBManager dbMgr, int roleID, int pkMode)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET pkmode={0} WHERE rid={1}", pkMode, roleID);

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

            return ret;
        }

        /// <summary>
        /// 修改角色的PK值和PKPoint
        /// </summary>
        public static bool UpdatePKValues(DBManager dbMgr, int roleID, int pkValue, int pkPoint)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET pkvalue={0}, pkpoint={1} WHERE rid={2}", pkValue, pkPoint, roleID);

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

            return ret;
        }

        /// <summary>
        /// 修改角色的连斩数
        /// </summary>
        public static bool UpdateLianZhan(DBManager dbMgr, int roleID, int lianzhan)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET lianzhan={0} WHERE rid={1}", lianzhan, roleID);

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

            return ret;
        }

        /// <summary>
        /// 修改角色的杀BOSS的个数
        /// </summary>
        public static bool UpdateKillBoss(DBManager dbMgr, int roleID, int killBoss)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET killboss={0} WHERE rid={1}", killBoss, roleID);

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

            return ret;
        }

        /// <summary>
        /// 修改角色的角斗场称号的个数
        /// </summary>
        public static bool UpdateBattleNum(DBManager dbMgr, int roleID, int battleNum)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET battlenum={0} WHERE rid={1}", battleNum, roleID);

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

            return ret;
        }

        /// <summary>
        /// 修改角色的英雄逐擂的到达层数
        /// </summary>
        public static bool UpdateHeroIndex(DBManager dbMgr, int roleID, int heroIndex)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET heroindex={0} WHERE rid={1}", heroIndex, roleID);

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

            return ret;
        }

        /// <summary>
        /// 修改角色的统计数据
        /// </summary>
        public static bool UpdateRoleStat(DBManager dbMgr, int roleID, int equipJiFen, int xueWeiNum, int skillLearnedNum, int horseJiFen)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET equipjifen={0}, xueweinum={1}, skilllearnednum={2}, horsejifen={3} WHERE rid={4}", equipJiFen, xueWeiNum, skillLearnedNum, horseJiFen, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的快捷键映射
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleKeys(DBManager dbMgr, int roleID, int type, string keys)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = null;
                if (0 == type) //主快捷键映射
                {
                    cmdText = string.Format("UPDATE t_roles SET main_quick_keys='{0}' WHERE rid={1}", keys, roleID);
                }
                else //辅助快捷键
                {
                    cmdText = string.Format("UPDATE t_roles SET other_quick_keys='{0}' WHERE rid={1}", keys, roleID);
                }

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的剩余自动战斗时间
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleLeftFightSecs(DBManager dbMgr, int roleID, int leftFightSecs)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET leftfightsecs={0} WHERE rid={1}", leftFightSecs, roleID);

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

            return ret;
        }

        /// <summary>
        /// 添加一个新的坐骑
        /// </summary>
        public static int NewHorse(DBManager dbMgr, int roleID, int horseID, int bodyID, string addtime)
        {
            int ret = -1;
            MySQLConnection conn = null;
            string defVal = "0,0,0,0,0,0,0,0,0";

            try
            {
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_horses (rid, horseid, bodyid, propsNum, PropsVal, addtime, isdel, failednum) VALUES({0}, {1}, {2}, '{3}', '{4}', '{5}', {6}, {7})",
                    roleID, horseID, bodyID, defVal, defVal, addtime, 0, 0);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    error = true;
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                try
                {
                    if (!error)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            ret = Convert.ToInt32(reader[0].ToString());
                        }
                    }
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
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
        /// 添加一个新的宠物
        /// </summary>
        public static int NewPet(DBManager dbMgr, int roleID, int petID, string petName, int petType, string props, string addtime)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_pets (rid, petid, petname, pettype, feednum, realivenum, addtime, props, isdel) VALUES({0}, {1}, '{2}', {3}, {4}, {5}, '{6}', '{7}', {8})",
                    roleID, petID, petName, petType, 0, 0, addtime, props, 0);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    error = true;
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                try
                {
                    if (!error)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            ret = Convert.ToInt32(reader[0].ToString());
                        }
                    }
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
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
        /// 修改坐骑
        /// </summary>
        public static int UpdateHorse(DBManager dbMgr, int id, string[] fields, int startIndex)
        {
            int ret = -1;
            MySQLConnection conn = null;
            string[] fieldNames = { "isdel", "horseid", "bodyid", "propsNum", "PropsVal", "failednum", "temptime", "tempnum", "faileddayid" };
            byte[] fieldTypes = { 0, 0, 0, 1, 1, 0, 3, 0, 0 };

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = FormatUpdateSQL(id, fields, startIndex, fieldNames, "t_horses", fieldTypes);

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

        /// <summary>
        /// 更新一个用户角色的坐骑ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleHorse(DBManager dbMgr, int roleID, int horseDbID)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = null;
                if (horseDbID > 0)
                {
                    cmdText = string.Format("UPDATE t_roles SET horseid={0}, lasthorseid={1} WHERE rid={2}", horseDbID, horseDbID, roleID);
                }
                else
                {
                    cmdText = string.Format("UPDATE t_roles SET horseid={0} WHERE rid={1}", horseDbID, roleID);
                }

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

            return ret;
        }

        /// <summary>
        /// 修改宠物
        /// </summary>
        public static int UpdatePet(DBManager dbMgr, int id, string[] fields, int startIndex)
        {
            int ret = -1;
            MySQLConnection conn = null;
            string[] fieldNames = { "petname", "pettype", "feednum", "realivenum", "props", "isdel", "addtime", "level" };
            byte[] fieldTypes = { 1, 0, 0, 0, 1, 0, 3, 0 };

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = FormatUpdateSQL(id, fields, startIndex, fieldNames, "t_pets", fieldTypes);

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

        /// <summary>
        /// 更新一个用户角色的宠物ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRolePet(DBManager dbMgr, int roleID, int petDbID)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET petid={0} WHERE rid={1}", petDbID, roleID);

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

            return ret;
        }

        /// <summary>
        /// 添加一个用户的点将积分
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddRoleDJPoint(DBManager dbMgr, int dbID, int roleID, int djPoint)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                bool insert = false;
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "";
                if (dbID <= 0)
                {
                    insert = true;
                    cmdText = string.Format("INSERT INTO t_djpoints (rid, djpoint, total, wincnt) VALUES({0}, {1}, {2}, {3})", roleID, djPoint, 1, djPoint > 0 ? 1 : 0);
                }
                else
                {
                    cmdText = string.Format("UPDATE t_djpoints SET djpoint=djpoint+{0}, total=total+1, wincnt=wincnt+{1} WHERE Id={2}", djPoint, djPoint > 0 ? 1 : 0, dbID);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    error = true;
                    ret = -1;

                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                if (error)
                {
                    return ret;
                }

                try
                {
                    if (insert)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            ret = Convert.ToInt32(reader[0].ToString());
                        }

                        cmd.Dispose();
                        cmd = null;
                    }
                    else
                    {
                        ret = dbID;
                    }
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
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
        /// 添加一个用户的经脉级别
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpRoleJingMai(DBManager dbMgr, int roleID, int dbID, int jingMaiBodyLevel, int jingMaiID, int jingMaiLevel)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                bool insert = false;
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "";
                if (dbID <= 0)
                {
                    insert = true;
                    cmdText = string.Format("INSERT INTO t_jingmai (rid, jmid, jmlevel, bodylevel) VALUES({0}, {1}, {2}, {3})",
                        roleID, jingMaiID, jingMaiLevel, jingMaiBodyLevel);
                }
                else
                {
                    cmdText = string.Format("UPDATE t_jingmai SET jmlevel={0} WHERE Id={1}", jingMaiLevel, dbID);
                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    error = true;
                    ret = -1;

                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                if (error)
                {
                    return ret;
                }

                try
                {
                    if (insert)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            ret = Convert.ToInt32(reader[0].ToString());
                        }

                        cmd.Dispose();
                        cmd = null;
                    }
                    else
                    {
                        ret = dbID;
                    }
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
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
        /// 添加一个新公告
        /// </summary>
        public static int NewBulletinText(DBManager dbMgr, string msgID, int toPlayNum, string bulletinText)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_bulletin (msgid, toplaynum, bulletintext, bulletintime) VALUES('{0}', {1}, '{2}', '{3}') ON DUPLICATE KEY UPDATE toplaynum={4}, bulletintext='{5}', bulletintime='{6}'", msgID, toPlayNum, bulletinText, today, toPlayNum, bulletinText, today);

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

        /// <summary>
        /// 删除一个旧公告
        /// </summary>
        public static int RemoveBulletinText(DBManager dbMgr, string msgID)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("DELETE FROM t_bulletin WHERE msgid='{0}'", msgID);

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

        /// <summary>
        /// 添加一个游戏配置参数
        /// </summary>
        public static int UpdateGameConfig(DBManager dbMgr, string paramName, string paramValue)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_config (paramname, paramvalue) VALUES('{0}', '{1}') ON DUPLICATE KEY UPDATE paramvalue='{2}'", paramName, paramValue, paramValue);

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

        /// <summary>
        /// 添加一个新的技能
        /// </summary>
        public static int AddSkill(DBManager dbMgr, int roleID, int skillID, int skillLevel)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_skills (rid, skillid, skilllevel, usednum) VALUES({0}, {1}, {2}, 0)", roleID, skillID, skillLevel);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    error = true;
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                try
                {
                    if (!error)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            ret = Convert.ToInt32(reader[0].ToString());
                        }
                    }
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
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
        /// 更新一个技能的数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateSkillInfo(DBManager dbMgr, int skillDbID, int skillLevel, int usedNum)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_skills SET skilllevel={0}, usednum={1} WHERE Id={2}", skillLevel, usedNum, skillDbID);

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

            return ret;
        }

        /// <summary>
        /// 更新角色的从其他人冲脉获取的经验和次数
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateJingMaiExp(DBManager dbMgr, int roleID, int jingMaiExpNum, int totalJingMaiExp)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET jingmai_exp_num={0}, total_jingmai_exp={1} WHERE rid={2}", jingMaiExpNum, totalJingMaiExp, roleID);

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

            return ret;
        }

        /// <summary>
        /// 添加一个用户的buffer项
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateRoleBufferItem(DBManager dbMgr, int roleID, int bufferID, long startTime, int bufferSecs, long bufferVal)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_buffer (rid, bufferid, starttime, buffersecs, bufferval) VALUES({0}, {1}, {2}, {3}, {4}) ON DUPLICATE KEY UPDATE starttime={5}, buffersecs={6}, bufferval={7}",
                        roleID, bufferID, startTime, bufferSecs, bufferVal, startTime, bufferSecs, bufferVal, roleID, bufferID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        /// <summary>
        /// 更新一个用户角色的日跑环任务数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateRoleDailyTaskData(DBManager dbMgr, int roleID, int huanID, string rectime, int recnum, int taskClass, int extDayID, int extNum)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_dailytasks (rid, huanid, rectime, recnum, taskClass, extdayid, extnum) VALUES({0}, {1}, '{2}', {3}, {4}, {5}, {6}) ON DUPLICATE KEY UPDATE huanid={1}, rectime='{2}', recnum={3}, taskClass={4}, extdayid={5}, extnum={6}",
                        roleID, huanID, rectime, recnum, taskClass, extDayID, extNum);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        /// <summary>
        /// 更新一个用户角色的每日冲穴次数数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateRoleDailyJingMaiData(DBManager dbMgr, int roleID, string jmTime, int jmNum)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_dailyjingmai (rid, jmtime, jmnum) VALUES({0}, '{1}', {2}) ON DUPLICATE KEY UPDATE jmtime='{3}', jmnum={4}",
                        roleID, jmTime, jmNum, jmTime, jmNum);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        /// <summary>
        /// 更新一个用户角色的打坐增加熟练度的被动技能的ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleNumSkillID(DBManager dbMgr, int roleID, int numSkillID)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET numskillid={0} WHERE rid={1}", numSkillID, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的已经完成的主线任务的ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleMainTaskID(DBManager dbMgr, int roleID, int mainTaskID)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET maintaskid={0} WHERE rid={1}", mainTaskID, roleID);

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

            return ret;
        }

        /// <summary>
        /// 更新一个用户角色的随身仓库信息的操作
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateRolePBInfo(DBManager dbMgr, int roleID, int extGridNum)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_ptbag (rid, extgridnum) VALUES({0}, {1}) ON DUPLICATE KEY UPDATE extgridnum={2}",
                        roleID, extGridNum, extGridNum);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        /// <summary>
        /// 更新一个用户角色背包格子数
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateRoleBagNum(DBManager dbMgr, int roleID, int bagNum)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles set bagnum={0} where rid={1}", bagNum, roleID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        /// <summary>
        /// 创建角色的活动数据
        /// </summary>
        public static void CreateHuoDong(DBManager dbMgr, int roleID)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_huodong (rid, loginweekid, logindayid, loginnum, newstep, steptime, lastmtime, curmid, curmtime, songliid, logingiftstate, onlinegiftstate) VALUES({0}, '{1}', '{2}', {3}, {4}, '{5}', {6}, '{7}', {8}, {9}, {10}, {11})",
                    roleID, "", "", 0, 0, today, 0, "", 0, 0, 0, 0);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 修改活动数据
        /// </summary>
        public static int UpdateHuoDong(DBManager dbMgr, int id, string[] fields, int startIndex)
        {
            int ret = -1;
            MySQLConnection conn = null;
            string[] fieldNames = { "loginweekid", "logindayid", "loginnum", "newstep", "steptime", "lastmtime", "curmid", "curmtime", "songliid", "logingiftstate", "onlinegiftstate", "lastlimittimehuodongid", "lastlimittimedayid", "limittimeloginnum", "limittimegiftstate", "everydayonlineawardstep", "geteverydayonlineawarddayid", "serieslogingetawardstep", "seriesloginawarddayid", "seriesloginawardgoodsid", "everydayonlineawardgoodsid" };
            byte[] fieldTypes = { 1, 1, 0, 0, 3, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 };

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = FormatUpdateSQL(id, fields, startIndex, fieldNames, "t_huodong", fieldTypes, "rid");

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

        /// <summary>
        /// 清空礼品码的数据
        /// </summary>
        public static int ClearAllLiPinMa(DBManager dbMgr)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("DELETE FROM t_linpinma");

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

        /// <summary>
        /// 插入一个新的礼品码的数据
        /// </summary>
        public static int InsertNewLiPinMa(DBManager dbMgr, string liPinMa, string songLiID, string maxNum, string ptid, string ptRepeat, string usedNum = "0")
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_linpinma (lipinma, huodongid, maxnum, usednum, ptid, ptrepeat) VALUES('{0}', {1}, {2}, {3}, {4}, {5})",
                    liPinMa, songLiID, maxNum, usedNum, ptid, ptRepeat);

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

        /// <summary>
        /// 修改一个礼品码的使用次数
        /// </summary>
        public static int UpdateLiPinMaUsedNum(DBManager dbMgr, string liPinMa, int usedNum)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_linpinma SET usednum={0} WHERE lipinma='{1}'",
                    usedNum, liPinMa);

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

        /// <summary>
        /// 更新一个用户角色的副本数据 -- 副本改造 增加最快通关时间  add by liaowei
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateFuBenData(DBManager dbMgr, int roleID, int fuBenID, int dayID, int enterNum, int nQuickPassTimeSec, int nFinishNum)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_fuben (rid, fubenid, dayid, enternum, quickpasstimer, finishnum) VALUES({0}, {1}, {2}, {3}, {4}, {5}) ON DUPLICATE KEY UPDATE fubenid={1}, dayid={2}, enternum={3}, quickpasstimer={4}, finishnum={5}",
                        roleID, fuBenID, dayID, enterNum, nQuickPassTimeSec, nFinishNum);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        /// <summary>
        /// 插入一个新的预先分配的名字
        /// </summary>
        public static int InsertNewPreName(DBManager dbMgr, string preName, int sex)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_prenames (name, sex, used) VALUES('{0}', {1}, {2})",
                    preName, sex, 0);

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

        /// <summary>
        /// 修改一个预先分配的名字的使用
        /// </summary>
        public static int UpdatePreNameUsedState(DBManager dbMgr, string preName, int used)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_prenames SET used={0} WHERE name='{1}'",
                    used, preName);

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

        /// <summary>
        /// 插入一个新的副本通关历史记录
        /// </summary>
        public static int InsertNewFuBenHist(DBManager dbMgr, int fuBenID, int roleID, string roleName, int usedSecs)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_fubenhist (fubenid, rid, rname, usedsecs) VALUES({0}, {1}, '{2}', {3}) ON DUPLICATE KEY UPDATE rid={4}, rname='{5}', usedsecs={6}",
                    fuBenID, roleID, roleName, usedSecs, roleID, roleName, usedSecs);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (Exception)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 更新一个用户角色的每日数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateRoleDailyData(DBManager dbMgr, int roleID, int expDayID, int todayExp, int lingLiDayID, int todayLingLi, int killBossDayID, int todayKillBoss, int fuBenDayID, int todayFuBenNum, int wuXingDayID, int wuXingNum)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_dailydata (rid, expdayid, todayexp, linglidayid, todaylingli, killbossdayid, todaykillboss, fubendayid, todayfubennum, wuxingdayid, wuxingnum) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}) ON DUPLICATE KEY UPDATE expdayid={1}, todayexp={2}, linglidayid={3}, todaylingli={4}, killbossdayid={5}, todaykillboss={6}, fubendayid={7}, todayfubennum={8}, wuxingdayid={9}, wuxingnum={10}",
                        roleID, expDayID, todayExp, lingLiDayID, todayLingLi, killBossDayID, todayKillBoss, fuBenDayID, todayFuBenNum, wuXingDayID, wuXingNum);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        /// <summary>
        /// 更新一个用户角色的押镖数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateYaBiaoData(DBManager dbMgr, int roleID, int yaBiaoID, long startTime, int state, int lineID, int touBao, int yaBiaoDayID, int yaBiaoNum, int takeGoods)
        {
            int ret = -1;
            MySQLConnection conn = null;

            string today = "1900-01-01 12:00:00";
            if (startTime > 0)
            {
                today = (new DateTime(startTime * 10000)).ToString("yyyy-MM-dd HH:mm:ss");
            }

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_yabiao (rid, yabiaoid, starttime, state, lineid, toubao, yabiaodayid, yabiaonum, takegoods) VALUES({0}, {1}, '{2}', {3}, {4}, {5}, {6}, {7}, {8}) ON DUPLICATE KEY UPDATE yabiaoid={1}, starttime='{2}', state={3}, lineid={4}, toubao={5}, yabiaodayid={6}, yabiaonum={7}, takegoods={8}",
                        roleID, yaBiaoID, today, state, lineID, touBao, yaBiaoDayID, yaBiaoNum, takeGoods);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        /// <summary>
        /// 更新一个用户角色的押镖数据的状态
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateYaBiaoDataState(DBManager dbMgr, int roleID, int state)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_yabiao SET state={0} WHERE rid={1}",
                        state, roleID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        /// <summary>
        /// 添加一个新的商城购买记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewMallBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_mallbuy (rid, goodsid, goodsnum, totalprice, leftmoney, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')",
                    roleID, goodsID, goodsNum, totalPrice, leftMoney, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一个新的奇珍阁购买记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewQiZhenGeBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_qizhengebuy (rid, goodsid, goodsnum, totalprice, leftmoney, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')",
                    roleID, goodsID, goodsNum, totalPrice, leftMoney, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一个新的竞猜记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewShengXiaoGuessHistory(DBManager dbMgr, int roleID, string roleName, int zoneID, int guessKey, int mortgage, int resultKey, int gainNum, int leftMortgage)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_shengxiaoguesshist (rid, rname, guesskey, mortgage, resultkey, gainnum, leftmortgage, zoneid, guesstime) VALUES({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, '{8}')",
                    roleID, roleName, guessKey, mortgage, resultKey, gainNum, leftMortgage, zoneID, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一个新的银票购买记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewYinPiaoBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftYinPiaoNum)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_yinpiaobuy (rid, goodsid, goodsnum, totalprice, leftyinpiao, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')",
                    roleID, goodsID, goodsNum, totalPrice, leftYinPiaoNum, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一个新的在线人数记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewOnlineNumItem(DBManager dbMgr, int totalNum, DateTime dateTime, String strMapOnlineInfo)
        {
            int ret = -1;
            string today = dateTime.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_onlinenum (num, rectime, mapnum) VALUES({0}, '{1}', '{2}')",
                    totalNum, today, strMapOnlineInfo);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一个新帮会
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int CreateBangHui(DBManager dbMgr, int roleID, int zoneID, int totalLevel, string bhName, string bhBulletin, int nMoney = 0)
        {
            int bhid = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_banghui (bhname, zoneid, rid, totalnum, totallevel, bhbulletin, buildtime, qiname, tongqian) VALUES('{0}', {1}, {2}, {3}, {4}, '{5}', '{6}', '{7}', {8})",
                    bhName, zoneID, roleID, 1, totalLevel, bhBulletin, today, bhName, nMoney);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    error = true;
                    bhid = -1; //创建失败(角色名称重复)

                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                try
                {
                    if (!error)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            bhid = Convert.ToInt32(reader[0].ToString());
                        }

                        cmd.Dispose();
                        cmd = null;
                    }
                }
                catch (MySQLException)
                {
                    bhid = -2; //创建失败(角色名称重复)
                }
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
        /// 根据帮会ID删除帮会
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void DeleteBangHui(DBManager dbMgr, int bhid)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("UPDATE t_banghui SET isdel=1 WHERE bhid={0}",
                        bhid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 修改帮会介绍
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateBangHuiBulletin(DBManager dbMgr, int bhid, string bhBulletinMsg)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_banghui SET bhbulletin='{0}' WHERE bhid={1} AND isdel=0",
                    bhBulletinMsg, bhid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 修改帮会是否验证
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateBangHuiVerify(DBManager dbMgr, int roleID, int bhid, int isVerify)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_banghui SET isverfiy={0} WHERE bhid={1} AND rid={2}",
                    isVerify, bhid, roleID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 修改帮会的首领角色ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateBangHuiRoleID(DBManager dbMgr, int roleID, int bhid)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_banghui SET rid={0} WHERE bhid={1}",
                    roleID, bhid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 清空某个帮会成员的职务
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void ClearBangHuiMemberZhiWu(DBManager dbMgr, int bhid, int zhiWu)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET bhzhiwu=0 WHERE faction={0} AND bhzhiwu={1}",
                    bhid, zhiWu);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 修改帮会成员的职务
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateBangHuiMemberZhiWu(DBManager dbMgr, int bhid, int otherRoleID, int zhiWu)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET bhzhiwu={0} WHERE faction={1} AND rid={2}",
                    zhiWu, bhid, otherRoleID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 修改帮会成员的称号
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateBangHuiMemberChengHao(DBManager dbMgr, int bhid, int otherRoleID, string chengHao)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET chenghao='{0}' WHERE faction={1} AND rid={2}",
                    chengHao, bhid, otherRoleID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 增加帮贡库存
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateBangHuiBangGong(DBManager dbMgr, int bhid, int goods1Num, int goods2Num, int goods3Num, int goods4Num, int goods5Num, int tongQian)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_banghui SET goods1num=goods1num+{0}, goods2num=goods2num+{1}, goods3num=goods3num+{2}, goods4num=goods4num+{3}, goods5num=goods5num+{4}, tongqian=tongqian+{5} WHERE bhid={6}",
                     goods1Num, goods2Num, goods3Num, goods4Num, goods5Num, tongQian, bhid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加帮贡历史记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void AddBangGongHistItem(DBManager dbMgr, int roleID, int bhid, int goods1Num, int goods2Num, int goods3Num, int goods4Num, int goods5Num, int tongQian, int banggong)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_banggonghist (bhid, rid, goods1num, goods2num, goods3num, goods4num, goods5num, tongqian, banggong, addtime) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}') ON DUPLICATE KEY UPDATE goods1num=goods1num+{2}, goods2num=goods2num+{3}, goods3num=goods3num+{4}, goods4num=goods4num+{5}, goods5num=goods5num+{6}, tongqian=tongqian+{7}, banggong=banggong+{8}, addtime='{9}'",
                    roleID, bhid, goods1Num, goods2Num, goods3Num, goods4Num, goods5Num, tongQian, banggong, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 修改帮旗名称，同时扣除铜钱库存
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateBangHuiQiName(DBManager dbMgr, int bhid, string qiName, int needMoney)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_banghui SET qiname='{0}', tongqian=tongqian-{1} WHERE bhid={2}",
                     qiName, needMoney, bhid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 升级帮旗级别，同时扣除库存
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateBangHuiQiLevel(DBManager dbMgr, int bhid, int toLevel, int goods1Num, int goods2Num, int goods3Num, int goods4Num, int goods5Num, int needMoney)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_banghui SET qilevel={0}, goods1num=goods1num-{1}, goods2num=goods2num-{2}, goods3num=goods3num-{3}, goods4num=goods4num-{4}, goods5num=goods5num-{5}, tongqian=tongqian-{6} WHERE bhid={7}",
                     toLevel, goods1Num, goods2Num, goods3Num, goods4Num, goods5Num, needMoney, bhid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 升级战盟建筑的级别，同时扣除库存
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateZhanMengBuildLevel(DBManager dbMgr, int bhid, int toLevel, int needMoney, string fieldName)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_banghui SET {0}={1}, tongqian=tongqian-{2} WHERE bhid={3}",
                     fieldName, toLevel, needMoney, bhid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加库存的铜钱
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void AddBangHuiTongQian(DBManager dbMgr, int bhid, int addMoney)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_banghui SET tongqian=tongqian+{0} WHERE bhid={1}",
                     addMoney, bhid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 扣除库存的铜钱
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void SubBangHuiTongQian(DBManager dbMgr, int bhid, int subMoney)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_banghui SET tongqian=tongqian-{0} WHERE bhid={1}",
                     subMoney, bhid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 更新帮会的人员个数和级别总和
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateBangHuiNumLevel(DBManager dbMgr, int bhid, int totalNum, int totalLevel, int TotalCombatForce)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_banghui SET totalnum={0}, totallevel={1}, totalcombatforce={3} WHERE bhid={2}",
                     totalNum, totalLevel, bhid, TotalCombatForce);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 更新领地占领表
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateBHLingDi(DBManager dbMgr, BangHuiLingDiInfoData bangHuiLingDiInfoData)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_lingdi (lingdi, bhid, tax, takedayid, takedaynum, yestodaytax, taxdayid, todaytax, totaltax, warrequest, awardfetchday) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}', {10}) ON DUPLICATE KEY UPDATE bhid={1}, tax={2}, takedayid={3}, takedaynum={4}, yestodaytax={5}, taxdayid={6}, todaytax={7}, totaltax={8}, warrequest='{9}', awardfetchday={10}",
                    bangHuiLingDiInfoData.LingDiID, bangHuiLingDiInfoData.BHID, bangHuiLingDiInfoData.LingDiTax,
                    bangHuiLingDiInfoData.TakeDayID, bangHuiLingDiInfoData.TakeDayNum,
                    bangHuiLingDiInfoData.YestodayTax,
                    bangHuiLingDiInfoData.TaxDayID, bangHuiLingDiInfoData.TodayTax,
                    bangHuiLingDiInfoData.TotalTax, bangHuiLingDiInfoData.WarRequest, bangHuiLingDiInfoData.AwardFetchDay);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 清空某个帮会占领的领地列表
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void ClearBHLingDiByID(DBManager dbMgr, int bhid)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_lingdi SET bhid=0, tax=0, takedayid=0, takedaynum=0, yestodaytax=0, taxdayid=0, todaytax=0, totaltax=0 WHERE bhid={0}",
                    bhid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 清空某个帮会占领的领地税收
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void ClearBHLingDiTotalTaxByID(DBManager dbMgr, int lingDiID)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_lingdi SET totaltax=0 WHERE lingdi={0}",
                    lingDiID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 执行扣除战盟维护资金
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void SubBangHuiTongQianByQiLevel(DBManager dbMgr, int moneyPerLevel)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                // 维护资金跟建筑等级无关
                // string cmdText = string.Format("UPDATE t_banghui SET tongqian=tongqian-{0}*qilevel", moneyPerLevel);
                string cmdText = string.Format("UPDATE t_banghui SET tongqian=tongqian-{0}", moneyPerLevel);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 修改帮会副本信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateBangHuiFuBen(DBManager dbMgr, int bhid, int fubenid, int state, int openday, string killers)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_banghui SET fubenid={0}, fubenstate={1}, openday={2}, killers='{3}' WHERE bhid={4} AND isdel=0",
                    fubenid, state, openday, killers, bhid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 设置某个角色为皇妃
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateRoleToHuangFei(DBManager dbMgr, int roleID, int huangHou)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET huanghou={0} WHERE rid={1}",
                     huangHou, roleID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 更新皇帝特权数据项
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateHuangDiTeQuan(DBManager dbMgr, HuangDiTeQuanItem hangDiTeQuanItem)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_hdtequan (Id, tolaofangdayid, tolaofangnum, offlaofangdayid, offlaofangnum, bancatdayid, bancatnum) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}) ON DUPLICATE KEY UPDATE Id={0}, tolaofangdayid={1}, tolaofangnum={2}, offlaofangdayid={3}, offlaofangnum={4}, bancatdayid={5}, bancatnum={6}",
                     hangDiTeQuanItem.ID, hangDiTeQuanItem.ToLaoFangDayID, hangDiTeQuanItem.ToLaoFangNum, hangDiTeQuanItem.OffLaoFangDayID, hangDiTeQuanItem.OffLaoFangNum, hangDiTeQuanItem.BanCatDayID, hangDiTeQuanItem.BanCatNum);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加刷新奇珍阁的记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void AddRefreshQiZhenGeRec(DBManager dbMgr, int roleID, int oldUserMoney, int leftUserMoney)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_refreshqizhen (rid, oldusermoney, leftusermoney, refreshtime) VALUES({0}, {1}, {2}, '{3}')",
                     roleID, oldUserMoney, leftUserMoney, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加使用礼品码的平台记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void AddUsedLiPinMa(DBManager dbMgr, int huodongID, string lipinMa, int pingTaiID, int roleID)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_usedlipinma (lipinma, huodongid, ptid, rid) VALUES('{0}', {1}, {2}, {3})",
                     lipinMa, huodongID, pingTaiID, roleID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加元宝消费记录告警记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void AddMoneyWarning(DBManager dbMgr, int roleID, int usedMoney, int goodsMoney)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_warning (rid, usedmoney, goodsmoney, warningtime) VALUES({0}, {1}, {2}, '{3}')",
                     roleID, usedMoney, goodsMoney, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一个新的物品购买记录【不管用什么购买的，都通过购买类型识别】,从npc购买
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewBuyItemFromNpc(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney, int moneyType)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_npcbuy (rid, goodsid, goodsnum, totalprice, leftmoney, buytime, moneytype) VALUES({0}, {1}, {2}, {3}, {4}, '{5}', {6})",
                    roleID, goodsID, goodsNum, totalPrice, leftMoney, today, moneyType);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一个新的银两购买记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewYinLiangBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftYinLiang)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_yinliangbuy (rid, goodsid, goodsnum, totalprice, leftyinliang, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')",
                    roleID, goodsID, goodsNum, totalPrice, leftYinLiang, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一个新的金币购买记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewGoldBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftGold)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_goldbuy (rid, goodsid, goodsnum, totalprice, leftgold, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')",
                    roleID, goodsID, goodsNum, totalPrice, leftGold, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一个新的帮贡购买记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewBangGongBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftBangGong)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_banggongbuy (rid, goodsid, goodsnum, totalprice, leftbanggong, buytime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')",
                    roleID, goodsID, goodsNum, totalPrice, leftBangGong, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 更新上次扫描的充值日志的ID
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="id"></param>
        public static void UpdateLastScanInputLogID(DBManager dbMgr, int lastid)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_inputhist (Id, lastid) VALUES(1, {0}) ON DUPLICATE KEY UPDATE lastid={0}", lastid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        #region 邮件相关
        /// <summary>
        /// 更新邮件已读标志[包括标志位和读取时间]
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateMailHasReadFlag(DBManager dbMgr, int mailID, int rid)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_mail SET isread=1, readtime=now() where mailid={0} and receiverrid={1}", mailID, rid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 更新邮件已提取物品标志位标志
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateMailHasFetchGoodsFlag(DBManager dbMgr, int mailID, int rid)
        {
            bool ret = true;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_mail SET hasfetchattachment=1 where mailid={0} and receiverrid={1}", mailID, rid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    ret = false;
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 删除邮件实体数据，不包括附件列表
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool DeleteMailDataItemExcludeGoodsList(DBManager dbMgr, int mailID, int rid)
        {
            bool ret = true;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("DELETE from t_mail where mailid={0} and receiverrid={1}", mailID, rid);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    ret = false;
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 删除邮件附件物品列表
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void DeleteMailGoodsList(DBManager dbMgr, int mailID)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("DELETE from t_mailgoods where mailid={0}", mailID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一条新的邮件实体数据[不包括附件数据],返回邮件ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddMailBody(DBManager dbMgr, int senderrid, string senderrname, int receiverrid, string reveiverrname, string subject, string content, int yinliang, int tongqian, int yuanbao)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int mailID = -1;
            MySQLConnection conn = null;

            try
            {
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();

                //添加邮件主体数据
                string cmdText = string.Format("INSERT INTO t_mail (senderrid, senderrname, sendtime, receiverrid, reveiverrname, readtime, " +
                    "isread, mailtype, hasfetchattachment, subject,content, yinliang, tongqian, yuanbao) VALUES (" +
                    "{0},'{1}','{2}', {3}, '{4}','{5}',{6},{7},{8},'{9}','{10}',{11},{12}, {13})",
                    senderrid, senderrname, today, receiverrid, reveiverrname, "2000-11-11 11:11:11",
                    0, 1, 0, subject, content, yinliang, tongqian, yuanbao);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    error = true;
                    mailID = -2; //添加新邮件失败

                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                try
                {
                    if (!error)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            mailID = Convert.ToInt32(reader[0].ToString());
                        }

                        cmd.Dispose();
                        cmd = null;
                    }
                }
                catch (MySQLException)
                {
                    error = true;
                    mailID = -3; //添加新邮件失败
                }

                //在邮件扫描表中加入新邮件信息
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return mailID;
        }

        /// <summary>
        /// 添加邮件附件数据,如果都成功，返回成功添加的物品数量，如果存在失败，返回值小于0
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool AddMailGoodsDataItem(DBManager dbMgr, int mailID, int goodsid, int forge_level, int quality, string Props, int gcount, int origholenum, int rmbholenum, string jewellist, int addpropindex, int binding, int bornindex, int lucky, int strong, int ExcellenceInfo, int nAppendPropLev, int nEquipChangeLife)
        {
            bool ret = true;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "";
                MySQLCommand cmd = null;

                //添加邮件附件数据
                try
                {
                    cmdText = string.Format("INSERT INTO t_mailgoods (mailid,goodsid,forge_level, quality,Props,gcount,origholenum,rmbholenum," +
                                            "jewellist,addpropindex,binding,bornindex,lucky,strong, excellenceinfo, appendproplev, equipchangelife) VALUES ({0}, {1}, {2}, {3}, '{4}', {5}, {6}, {7}, '{8}', {9},{10},{11},{12},{13},{14},{15},{16})",
                                            mailID, goodsid, forge_level, quality, Props, gcount, origholenum, rmbholenum, jewellist, addpropindex, binding, bornindex, lucky, strong, ExcellenceInfo, nAppendPropLev, nEquipChangeLife);

                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                    cmd = new MySQLCommand(cmdText, conn);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySQLException)
                    {
                        ret = false;//添加新邮件附件数据失败

                        LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                    }

                    cmd.Dispose();
                    cmd = null;
                }
                catch (MySQLException)
                {
                    ret = false;//添加新邮件附件数据失败
                }

                //在邮件扫描表中加入新邮件信息
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
        /// 在临时扫描表中添加一条新邮件ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateLastScanMailID(DBManager dbMgr, int roleID, int mailID)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_mailtemp (receiverrid, mailid) " +
                                                "VALUES ({0}, {1})", roleID, mailID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 删除临时扫描表中上次扫描到的新邮件ID[roleid,mailid 的辞典列表，mailid是扫描到的最大mailid]
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void DeleteLastScanMailIDs(DBManager dbMgr, Dictionary<int, int> lastMailDict)
        {
            MySQLConnection conn = null;

            try
            {

                string sWhere = "";
                //生成where语句
                foreach (var item in lastMailDict)
                {
                    if (sWhere.Length > 0)
                    {
                        sWhere += " or ";
                    }
                    else
                    {
                        sWhere = " where ";
                    }

                    sWhere += string.Format(" (mailid<={0} and receiverrid={1}) ", item.Value, item.Key);
                }

                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("DELETE from t_mailtemp {0}", sWhere);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 删除邮件临时表中邮件ID【删除邮件时调用】
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void DeleteMailIDInMailTemp(DBManager dbMgr, int mailID)
        {
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("DELETE from t_mailtemp where mailid={0}", mailID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 更新一个离线用户角色新邮件字段
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleLastMail(DBManager dbMgr, int roleID, int mailID)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET lastmailid={0} WHERE rid={1}", mailID, roleID);

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

            return ret;
        }
        #endregion 邮件相关

        /// <summary>
        /// 添加一个新的活动排行记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddHongDongPaiHangRecord(DBManager dbMgr, int rid, string rname, int zoneid, int huoDongType, int paihang, string paihangtime, int phvalue)
        {
            int ret = -1;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_huodongpaihang (rid, rname, zoneid, type, paihang, paihangtime, phvalue) VALUES({0}, '{1}', {2}, {3}, {4}, '{5}', {6})",
                    rid, rname, zoneid, huoDongType, paihang, paihangtime, phvalue);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一个新的活动奖励记录,针对角色
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddHongDongAwardRecordForRole(DBManager dbMgr, int rid, int zoneid, int activitytype, string keystr, int hasgettimes, string lastgettime)
        {
            int ret = -1;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_huodongawardrolehist (rid, zoneid, activitytype, keystr, hasgettimes,lastgettime) VALUES({0}, {1}, {2}, '{3}', {4}, '{5}')",
                    rid, zoneid, activitytype, keystr, hasgettimes, lastgettime);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        ret = 0;
                    }
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一个新的活动奖励记录，针对用户
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddHongDongAwardRecordForUser(DBManager dbMgr, string userid, int activitytype, string keystr, int hasgettimes, string lastgettime)
        {
            int ret = -1;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_huodongawarduserhist (userid, activitytype, keystr, hasgettimes,lastgettime) VALUES('{0}', {1}, '{2}', {3}, '{4}')",
                    userid, activitytype, keystr, hasgettimes, lastgettime);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        ret = 0;
                    }
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 更新活动奖励记录【根据角色id，用户id，活动类型，活动关键字作为条件更新活动奖励记录，主要用于针对角色的奖励记录】
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateHongDongAwardRecordForRole(DBManager dbMgr, int rid, int zoneid, int activitytype, string keystr, int hasgettimes, string lastgettime)
        {
            int ret = -1;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_huodongawardrolehist SET hasgettimes={0}, lastgettime='{1}' WHERE rid={2} AND zoneid={3} AND activitytype={4} AND keystr='{5}' AND hasgettimes!={0}",
                    hasgettimes, lastgettime, rid, zoneid, activitytype, keystr, hasgettimes);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    //返回影响的行数，返回 0 表示没有任何影响的行数，对于执行表结构更改等，返回-1也是成功，失败抛异常
                    //这儿采用返回影响行数来判断本次操作是否真的成功,对于奖励历史记录的更新，如果hasgettimes已经被设置了
                    //本次要设置的值，表示本次设置无效，这样可以避免多个指令同时采用同样的值更改这个字段
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        ret = 0;
                    }
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 更新活动奖励记录【根据用户id，活动类型，活动关键字作为条件更新活动奖励记录，主要用于针对用户的奖励记录,比如充值奖励】
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateHongDongAwardRecordForUser(DBManager dbMgr, string userid, int activitytype, string keystr, int hasgettimes, string lastgettime)
        {
            int ret = -1;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("update t_huodongawarduserhist set hasgettimes={0}, lastgettime='{1}' where userid='{2}' and activitytype={3} and keystr='{4}' and hasgettimes!={5}",
                    hasgettimes, lastgettime, userid, activitytype, keystr, hasgettimes);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    //返回影响的行数，返回 0 表示没有任何影响的行数，对于执行表结构更改等，返回-1也是成功，失败抛异常
                    //这儿采用返回影响行数来判断本次操作是否真的成功,对于奖励历史记录的更新，如果hasgettimes已经被设置了
                    //本次要设置的值，表示本次设置无效，这样可以避免多个指令同时采用同样的值更改这个字段
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        ret = 0;
                    }
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加限购物品的历史记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddLimitGoodsBuyItem(DBManager dbMgr, int roleID, int goodsID, int dayID, int usedNum)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_limitgoodsbuy (rid, goodsid, dayid, usednum) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE dayID={2}, usedNum={3}",
                    roleID, goodsID, dayID, usedNum);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加VIP日数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddVipDailyData(DBManager dbMgr, int roleID, int priorityType, int dayID, int usedTimes)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_vipdailydata (rid, prioritytype, dayid, usedtimes) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE dayid={2}, usedtimes={3}",
                    roleID, priorityType, dayID, usedTimes);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加杨公宝库每日积分奖励数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddYangGongBKDailyJiFenData(DBManager dbMgr, int roleID, int jifen, int dayID, long awardhistory)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_yangguangbkdailydata (rid, jifen, dayid, awardhistory) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE jifen={1}, dayid={2}, awardhistory={3}",
                    roleID, jifen, dayID, awardhistory);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        #region 数据库ID字段自增长相关

        /// <summary>
        /// 更改数据库相关表的自增长值
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="sTableName"></param>
        /// <param name="nAutoIncrementValue"></param>
        /// <returns></returns>
        public static int ChangeTablesAutoIncrementValue(DBManager dbMgr, string sTableName, int nAutoIncrementValue)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("alter table {0} auto_increment={1}", sTableName, nAutoIncrementValue);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    //打印出来，便于跟踪查看
                    System.Console.WriteLine(string.Format("正在执行 {0}", cmdText));
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        #endregion 数据库ID字段自增长相关

        /// <summary>
        /// 更新一个角色的物品使用次数限制
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateGoodsLimit(DBManager dbMgr, int roleID, int goodsID, int dayID, int usedNum)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "";
                cmdText = string.Format("INSERT INTO t_goodslimit (rid, goodsid, dayid, usednum) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE dayid={2}, usednum={3}", roleID, goodsID, dayID, usedNum);

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

            return ret;
        }

        /// <summary>
        /// 更新一个角色的参数表
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleParams(DBManager dbMgr, int roleID, string name, string value)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "";
                cmdText = string.Format("INSERT INTO t_roleparams (rid, pname, pvalue) VALUES({0}, '{1}', '{2}') ON DUPLICATE KEY UPDATE pvalue='{2}'", roleID, name, value);

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

            return ret;
        }

        #region 限时抢购相关

        /// <summary>
        /// 添加一个新的限时抢购购买记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewQiangGouBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney, int qiangGouId)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_qianggoubuy (rid, goodsid, goodsnum, totalprice, leftmoney, buytime, qianggouid) VALUES({0}, {1}, {2}, {3}, {4}, '{5}', {6})",
                    roleID, goodsID, goodsNum, totalPrice, leftMoney, today, qiangGouId);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加一个新的限时抢购项记录,返回插入ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewQiangGouItem(DBManager dbMgr, int group, int random, int itemid, int goodsid, int origprice, int price,
            int singlepurchase, int fullpurchase, int daystime)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_qianggouitem (itemgroup, random, itemid, goodsid, origprice, price, singlepurchase, fullpurchase, daystime," +
                    " starttime, endtime, istimeover) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}','{10}', {11})",
                    group, random, itemid, goodsid, origprice, price, singlepurchase, fullpurchase, daystime, today, today, 0);//反正没结束，开始时间和结束时间设置成同一个

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                try
                {
                    if (0 == ret)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            ret = Convert.ToInt32(reader[0].ToString());
                        }

                        cmd.Dispose();
                        cmd = null;
                    }
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
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
        /// 设置抢购项结束标志
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="qiangGouId"></param>
        /// <returns></returns>
        public static bool UpdateQiangGouItemTimeOverFlag(DBManager dbMgr, int qiangGouId)
        {
            bool ret = false;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_qianggouitem SET istimeover=1, endtime='{0}' WHERE Id={1}", today, qiangGouId);

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

            return ret;
        }

        #endregion 限时抢购相关

        #region 砸金蛋相关
        /// <summary>
        /// 添加一个新的砸金蛋记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewZaJinDanHistory(DBManager dbMgr, int roleID, string roleName, int zoneID, int timesselected, int usedyuanbao, int usedjindan, int gaingoodsid, int gaingoodsnum, int gaingold, int gainyinliang, int gainexp, string srtProp)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_zajindanhist (rid, rname, zoneid, timesselected, usedyuanbao, usedjindan, gaingoodsid, gaingoodsnum, gaingold, gainyinliang, gainexp, strprop, operationtime)" +
                    " VALUES ({0},'{1}',{2},{3},{4},{5},{6},{7},{8},{9},{10},'{11}','{12}')",
                    roleID, roleName, zoneID, timesselected, usedyuanbao, usedjindan, gaingoodsid, gaingoodsnum, gaingold, gainyinliang, gainexp, srtProp, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        #endregion 砸金蛋相关

        #region 开服在线大礼

        /// <summary>
        /// 添加开服在线奖励项
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="rid"></param>
        /// <param name="dayID"></param>
        /// <param name="yuanBao"></param>
        /// <param name="totalRoleNum"></param>
        /// <param name="zoneID"></param>
        public static int AddKaiFuOnlineAward(DBManager dbMgr, int rid, int dayID, int yuanBao, int totalRoleNum, int zoneID)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_kfonlineawards (rid, dayid, yuanbao, totalrolenum, zoneid)" +
                    " VALUES ({0},{1},{2},{3},{4}) ON DUPLICATE KEY UPDATE rid={0}, yuanbao={2}, totalrolenum={3}",
                    rid, dayID, yuanBao, totalRoleNum, zoneID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        #endregion 开服在线大礼

        #region 开服在线大礼

        /// <summary>
        /// 添加系统给予的元宝记录项
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="rid"></param>
        /// <param name="dayID"></param>
        /// <param name="yuanBao"></param>
        /// <param name="totalRoleNum"></param>
        /// <param name="zoneID"></param>
        public static int AddSystemGiveUserMoney(DBManager dbMgr, int rid, int yuanBao, string giveType)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_givemoney (rid, yuanbao, rectime, givetype)" +
                    " VALUES ({0},{1},'{2}','{3}')",
                    rid, yuanBao, today, giveType);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        #endregion 开服在线大礼

        #region 交易和掉落日志

        /// <summary>
        /// 添加物品交易日志
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="rid"></param>
        /// <param name="goodsid"></param>
        /// <param name="goodsnum"></param>
        /// <param name="leftgoodsnum"></param>
        /// <param name="otherroleid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static int AddExchange1Item(DBManager dbMgr, int rid, int goodsid, int goodsnum, int leftgoodsnum, int otherroleid, string result)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_exchange1 (rid, goodsid, goodsnum, leftgoodsnum, otherroleid, result, rectime)" +
                    " VALUES ({0},{1},{2},{3},{4},'{5}','{6}')",
                    rid, goodsid, goodsnum, leftgoodsnum, otherroleid, result, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加铜钱交易日志
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="rid"></param>
        /// <param name="goodsid"></param>
        /// <param name="goodsnum"></param>
        /// <param name="leftgoodsnum"></param>
        /// <param name="otherroleid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static int AddExchange2Item(DBManager dbMgr, int rid, int yinliang, int leftyinliang, int otherroleid)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_exchange2 (rid, yinliang, leftyinliang, otherroleid, rectime)" +
                    " VALUES ({0},{1},{2},{3},'{4}')",
                    rid, yinliang, leftyinliang, otherroleid, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加元宝交易日志
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="rid"></param>
        /// <param name="goodsid"></param>
        /// <param name="goodsnum"></param>
        /// <param name="leftgoodsnum"></param>
        /// <param name="otherroleid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static int AddExchange3Item(DBManager dbMgr, int rid, int yuanbao, int leftyuanbao, int otherroleid)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_exchange3 (rid, yuanbao, leftyuanbao, otherroleid, rectime)" +
                    " VALUES ({0},{1},{2},{3},'{4}')",
                    rid, yuanbao, leftyuanbao, otherroleid, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        /// 添加物品掉落日志
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="rid"></param>
        /// <param name="goodsid"></param>
        /// <param name="goodsnum"></param>
        /// <param name="leftgoodsnum"></param>
        /// <param name="otherroleid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static int AddFallGoodsItem(DBManager dbMgr, int rid, int autoid, int goodsdbid, int goodsid, int goodsnum, int binding, int quality, int forgelevel, string jewellist, string mapname, string goodsgrid, string fromname)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_fallgoods (rid, autoid, goodsdbid, goodsid, goodsnum, binding, quality, forgelevel, jewellist, mapname, goodsgrid, fromname, rectime)" +
                    " VALUES ({0},{1},{2},{3},{4},{5},{6},{7},'{8}','{9}','{10}','{11}','{12}')",
                    rid, autoid, goodsdbid, goodsid, goodsnum, binding, quality, forgelevel, jewellist, mapname, goodsgrid, fromname, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        #endregion 交易和掉落日志


        #region 月度抽奖
        /// <summary>
        /// 添加一个月度抽奖记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddNewYueDuChouJiangHistory(DBManager dbMgr, int roleID, string roleName, int zoneID, int gaingoodsid, int gaingoodsnum, int gaingold, int gainyinliang, int gainexp)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_yueduchoujianghist (rid, rname, zoneid, gaingoodsid, gaingoodsnum, gaingold, gainyinliang, gainexp, operationtime)" +
                    " VALUES ({0},'{1}',{2},{3},{4},{5},{6},{7},'{8}')",
                    roleID, roleName, zoneID, gaingoodsid, gaingoodsnum, gaingold, gainyinliang, gainexp, today);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (MySQLException)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        #endregion 月度抽奖

        #region 转职
        /// <summary>
        /// 更新一个用户角色的职业
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleOccupation(DBManager dbMgr, int roleID, int nOccu)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET occupation={0} WHERE rid={1}", nOccu, roleID);

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

            return ret;
        }

        #endregion 转职

        #region 血色堡垒

        /// <summary>
        /// 更新当天进入的次数
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateBloodCastleEnterCount(DBManager dbMgr, int nRoleID, int nDate, int nType, int nCount, string lastgettime)
        {
            bool bRet = false;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("REPLACE INTO t_dayactivityinfo(roleid, activityid, timeinfo, triggercount, lastgettime) VALUES({0}, {1}, {2}, {3}, '{4}')",
                                                nRoleID, nType, nDate, nCount, lastgettime);

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

                bRet = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bRet;
        }

        #endregion 血色堡垒

        #region 新手场景

        /// <summary>
        /// 完成新手场景
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleInfoForFlashPlayerFlag(DBManager dbMgr, int nRoleID, int isflashplayer)
        {
            bool bRet = false;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET isflashplayer={1} WHERE rid={0}", nRoleID, isflashplayer);

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

                bRet = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bRet;
        }

        /// <summary>
        /// 新手场景logout 经验处理
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleExpForFlashPlayerWhenLogOut(DBManager dbMgr, int nRoleID)
        {
            bool bRet = false;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string strQuickKey = "";

                string cmdText = string.Format("UPDATE t_roles SET experience={1}, maintaskid={1}, main_quick_keys='{2}' WHERE rid={0}", nRoleID, 0, strQuickKey);

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

                bRet = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bRet;
        }

        /// <summary>
        /// 新手场景logout 等级处理
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleLevForFlashPlayerWhenLogOut(DBManager dbMgr, int nRoleID)
        {
            bool bRet = false;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET level={1} WHERE rid={0}", nRoleID, 1);

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

                bRet = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bRet;
        }

        /// <summary>
        /// 新手场景logout 物品处理
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleGoodsForFlashPlayerWhenLogOut(DBManager dbMgr, int nRoleID)
        {
            bool bRet = false;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("DELETE FROM t_goods WHERE rid = {0}", nRoleID);

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

                bRet = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bRet;
        }

        /// <summary>
        /// 新手场景logout 任务处理
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleTasksForFlashPlayerWhenLogOut(DBManager dbMgr, int nRoleID)
        {
            bool bRet = false;

            MySQLConnection conn = null;

            try
            {
                // 1. t_tasks
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("DELETE FROM t_tasks WHERE rid = {0}", nRoleID);

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

                // 2. t_taskslog
                cmdText = string.Format("DELETE FROM t_taskslog WHERE rid = {0}", nRoleID);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                }

                cmd.Dispose();
                cmd = null;

                bRet = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bRet;
        }

        #endregion 新手场景


        #region 任务刷星

        /// <summary>
        /// 任务星级更新
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleTasksStarLevel(DBManager dbMgr, int nRoleID, int taskid, int StarLevel)
        {
            bool bRet = false;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_tasks SET starlevel={2} WHERE rid = {0} AND Id = {1}", nRoleID, taskid, StarLevel);

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

                bRet = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bRet;
        }
        #endregion 任务刷星


        #region 转生

        /// <summary>
        /// 转生信息更新
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleChangeLifeInfo(DBManager dbMgr, int nRoleID, int ChangeCount)
        {
            bool bRet = false;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET changelifecount={1} WHERE rid={0}", nRoleID, ChangeCount);

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

                bRet = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bRet;
        }
        #endregion 转生

        #region 崇拜

        /// <summary>
        /// 崇拜信息更新
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleAdmiredInfo1(DBManager dbMgr, int nRoleID, int nCount)
        {
            bool bRet = false;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET admiredcount={1} WHERE rid={0}", nRoleID, nCount);

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

                bRet = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bRet;
        }

        /// <summary>
        /// 崇拜信息更新--崇拜者数据t_adorationinfo表
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleAdmiredInfo2(DBManager dbMgr, int nRoleID1, int nRoleID2, int nDate)
        {
            bool bRet = false;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("REPLACE INTO t_adorationinfo(roleid, adorationroleid, dayid) VALUES({0}, {1}, {2})", nRoleID1, nRoleID2, nDate);

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

                bRet = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bRet;
        }

        #endregion 崇拜

        #region 战斗力
        /// <summary>
        /// 更新一个用户角色的战斗力
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleCombatForce(DBManager dbMgr, int roleID, int CombatForce)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET combatforce={0} WHERE rid={1}", CombatForce, roleID);

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

            return ret;
        }

        #endregion 战斗力


        #region 等级
        /// <summary>
        /// 等级
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleLevel(DBManager dbMgr, int nRoleID, int lev)
        {
            bool bRet = false;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET level={1} WHERE rid={0}", nRoleID, lev);

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

                bRet = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bRet;
        }

        #endregion 等级

        #region 日常活动

        /// <summary>
        /// 更新日常活动数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleDayActivityPoint(DBManager dbMgr, int nRoleID, int nDate, int nType, int nCount, long nValue)
        {
            bool bRet = false;

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("REPLACE INTO t_dayactivityinfo(roleid, activityid, timeinfo, triggercount, totalpoint, lastgettime) VALUES({0}, {1}, {2}, {3}, {4}, '{5}')", nRoleID, nType, nDate, nCount, nValue, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

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

                bRet = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bRet;
        }

        /// <summary>
        /// 删除玩家日常活动数据
        /// </summary>
        public static bool DeleteRoleDayActivityInfo(DBManager dbMgr, int roleID, int activityid)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("DELETE FROM t_dayactivityinfo WHERE roleid = {0} AND activityid = {1}", roleID, activityid);

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

            return ret;
        }

        #endregion 日常活动

        #region 自动分配属性点

        /// <summary>
        /// 玩家自动分配属性点
        /// </summary>
        public static int SetRoleAutoAssignPropertyPoint(DBManager dbMgr, int roleID, int nFlag)
        {
            int ret = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET autoassignpropertypoint = {0} WHERE rid = {1}", nFlag, roleID);

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

                ret = 1;
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

        #endregion 自动分配属性点

        #region 消息推送相关

        /// <summary>
        /// 更新推送ID
        /// </summary>
        public static int SetUserPushMessageID(DBManager dbMgr, string strUser, string strPushMegID)
        {
            int ret = 0;
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("REPLACE INTO t_pushmessageinfo(userid, pushid, lastlogintime) VALUE('{0}', '{1}', '{2}')", strUser, strPushMegID, today);

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

                ret = 1;
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

        #endregion 消息推送相关

        #region 翅膀相关

        /// <summary>
        /// 添加一个新的翅膀
        /// </summary>
        public static int NewWing(DBManager dbMgr, int roleID, int wingID, int forgeLevel, string addtime, string strRoleName, int nOccupation)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_wings (rid, rname, occupation, wingid, forgeLevel, addtime, isdel, failednum, equiped) VALUES({0}, '{1}', {2}, {3}, {4}, '{5}', {6}, {7}, {8})",
                    roleID, strRoleName, nOccupation, wingID, forgeLevel, addtime, 0, 0, 0);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    error = true;
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                try
                {
                    if (!error)
                    {
                        cmd = new MySQLCommand("SELECT LAST_INSERT_ID() AS LastID", conn);
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            ret = Convert.ToInt32(reader[0].ToString());
                        }
                    }
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
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
        /// 修改翅膀
        /// </summary>
        public static int UpdateWing(DBManager dbMgr, int id, string[] fields, int startIndex)
        {
            int ret = -1;
            MySQLConnection conn = null;
            string[] fieldNames = { "equiped", "wingid", "forgeLevel", "failednum", "starexp" };
            byte[] fieldTypes = { 0, 0, 0, 0, 0 };

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = FormatUpdateSQL(id, fields, startIndex, fieldNames, "t_wings", fieldTypes);

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

        #endregion 翅膀相关

        #region 图鉴系统

        /// <summary>
        /// 更新图鉴提交信息
        /// </summary>
        public static int UpdateRoleReferPictureJudgeInfo(DBManager dbMgr, int roleid, int nPictureJudgeID, int nNum)
        {
            int ret = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("REPLACE INTO t_picturejudgeinfo(roleid, picturejudgeid, refercount) VALUES({0}, {1}, {2})", roleid, nPictureJudgeID, nNum);

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

                ret = 1;
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

        #endregion 图鉴系统

        #region 魔晶兑换相关

        /// <summary>
        /// 更新绑定钻石兑换信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateMoJingExchangeDict(DBManager dbMgr, int nRoleid, int nExchangeid, int nDayID, int nNum)
        {
            int ret = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("REPLACE INTO t_mojingexchangeinfo(roleid, exchangeid, exchangenum, dayid) VALUES({0}, {1}, {2}, {3})", nRoleid, nExchangeid, nNum, nDayID);

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

                ret = 1;
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

        #endregion 魔晶钻石兑换相关
        #region 资源找回
        /// <summary>
        /// 更新资源找回数据
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateResourceGetInfo(DBManager dbMgr, int nRoleid, int type, OldResourceInfo info)
        {
            int ret = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "";
                if (info == null)
                {
                    cmdText = string.Format("REPLACE INTO t_resourcegetinfo(roleid, type, exp, leftCount,mojing,bandmoney,zhangong,chengjiu,shengwang,bangzuan,xinghun,hasget) VALUES({0}, {1}, {2},{3},{4},{5},{6},{7},{8},{9},{10},{11})",
                    nRoleid, type, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
                }
                else
                {
                    cmdText = string.Format("REPLACE INTO t_resourcegetinfo(roleid, type, exp, leftCount,mojing,bandmoney,zhangong,chengjiu,shengwang,bangzuan,xinghun,hasget) VALUES({0}, {1}, {2},{3},{4},{5},{6},{7},{8},{9},{10},{11})",
                    nRoleid, info.type, info.exp, info.leftCount, info.mojing, info.bandmoney, info.zhangong, info.chengjiu, info.shengwang, info.bandDiamond, info.xinghun, 0);

                }

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    ret = 0;
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

                cmd.Dispose();
                cmd = null;

                ret = 1;
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
        #endregion 资源找回

        #region 更新物品扩展

        /// <summary>
        /// 修改物品
        /// </summary>
        public static int UpdateGoods2(DBManager dbMgr, int roleID, GoodsData gd, UpdateGoodsArgs args)
        {
            int ret = -1;
            MySQLConnection conn = null;
            string[] fieldNames = { "isusing", "forge_level", "starttime", "endtime", "site", "quality", "Props", "gcount", "jewellist", "bagindex", "salemoney1", "saleyuanbao", "saleyinpiao", "binding", "addpropindex", "bornindex", "lucky", "strong", "excellenceinfo", "appendproplev", "equipchangelife" };
            byte[] fieldTypes = { 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            try
            {
                bool update = false;
                StringBuilder sb = new StringBuilder("UPDATE t_goods SET ");

                conn = dbMgr.DBConns.PopDBConnection();
                foreach (var idx in args.ChangedIndexes)
                {
                    if (idx == UpdatePropIndexes.WashProps && null != args.WashProps)
                    {
                        string cmdText = string.Format("INSERT INTO t_goodsprops (id, rid, type, props) VALUES({0}, {1}, {2}, '{3}') ON DUPLICATE KEY UPDATE props='{3}'",
                                args.DbID, roleID, (int)UpdatePropIndexes.WashProps, Convert.ToBase64String(DataHelper.ObjectToBytes(args.WashProps)));

                        GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                        MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                        //MySQLParameter param = new MySQLParameter("props", DataHelper.ObjectToBytes(args.WashProps));
                        //cmd.Parameters.Add(param);
                        //MySQLParameter parameter = cmd.CreateMySQLParameter();
                        //parameter.DbType = DbType.Binary;
                        //parameter.ParameterName = "props";
                        //parameter.Value = DataHelper.ObjectToBytes(args.WashProps);
                        //parameter.Value = args.WashProps;

                        try
                        {
                            cmd.ExecuteNonQuery();
                            gd.WashProps = args.WashProps;
                        }
                        catch (Exception ex)
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0} \n{1}", cmdText, ex.ToString()));
                        }

                        cmd.Dispose();
                        cmd = null;
                        ret = 1;
                    }
                    else if (idx == UpdatePropIndexes.ElementhrtsProps && null != args.ElementhrtsProps)
                    {
                        //string cmdText = string.Format("INSERT INTO t_goodsprops (id, rid, type, props) VALUES({0}, {1}, {2}, '{3}') ON DUPLICATE KEY UPDATE props='{3}'",
                        //        args.DbID, roleID, (int)UpdatePropIndexes.ElementhrtsProps, Convert.ToBase64String(DataHelper.ObjectToBytes(args.ElementhrtsProps)));

                        //string cmdText = string.Format("INSERT INTO t_goods (id, rid, ehinfo) VALUES({0}, {1}, '{2}') ON DUPLICATE KEY UPDATE ehinfo='{2}'",
                        //        args.DbID, roleID, Convert.ToBase64String(DataHelper.ObjectToBytes(args.ElementhrtsProps)));

                        string cmdText = string.Format("UPDATE t_goods set ehinfo='{0}' where id={1} and rid={2}",
                            Convert.ToBase64String(DataHelper.ObjectToBytes(args.ElementhrtsProps)), args.DbID, roleID);

                        GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                        MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                        //MySQLParameter param = new MySQLParameter("props", DataHelper.ObjectToBytes(args.WashProps));
                        //cmd.Parameters.Add(param);
                        //MySQLParameter parameter = cmd.CreateMySQLParameter();
                        //parameter.DbType = DbType.Binary;
                        //parameter.ParameterName = "props";
                        //parameter.Value = DataHelper.ObjectToBytes(args.WashProps);
                        //parameter.Value = args.WashProps;

                        try
                        {
                            cmd.ExecuteNonQuery();
                            gd.ElementhrtsProps = args.ElementhrtsProps;
                        }
                        catch (Exception ex)
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0} \n{1}", cmdText, ex.ToString()));
                        }

                        cmd.Dispose();
                        cmd = null;
                        ret = 1;
                    }
                    else if (idx < UpdatePropIndexes.MaxBaseIndex)
                    {
                        AppendSQLForGoodsProps(sb, (int)idx, args.Binding, fieldNames, fieldTypes, update);
                        update = true;
                    }
                }

                if (update)
                {
                    int result = -1;
                    sb.AppendFormat(" WHERE {0}={1}", "Id", args.DbID);
                    string sql = sb.ToString();
                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
                    MySQLCommand cmd = new MySQLCommand(sql, conn);

                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql));
                    }
                    cmd.Dispose();
                    cmd = null;

                    if (result >= 0)
                    {
                        UpdateGoodsDataFromArgs(gd, args);
                    }
                }
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
        /// 设置物品扩展属性表的某物品记录为删除状态
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="roleID"></param>
        /// <param name="gd"></param>
        /// <param name="type"></param>
        public static void UpdateGoods2SetDelete(DBManager dbMgr, int roleID, GoodsData gd, int type = -1)
        {
            string sql = string.Format("UPDATE t_goodsprops SET isdel=1 where id={0} and rid={1}", gd.Id, roleID); ;
            if (type >= 0)
                sql += string.Format(" and type={0}", type);

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                MySQLCommand cmd = new MySQLCommand(sql, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql));
                }
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
        /// 设置物品扩展属性表的某物品记录角色ID
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="roleID"></param>
        /// <param name="gd"></param>
        /// <param name="type"></param>
        public static void UpdateGoodsPropsRoleID(DBManager dbMgr, int roleID, int goodsDbId)
        {
            string sql = string.Format("UPDATE t_goodsprops SET rid={0} where id={1}", roleID, goodsDbId);

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                MySQLCommand cmd = new MySQLCommand(sql, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql));
                }
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

        public static void AppendSQLForGoodsProps(StringBuilder sb, int index, object value, string[] fieldNames, byte[] fieldTypes, bool hasAppend)
        {
            if (hasAppend)
            {
                sb.Append(", ");
            }
            if (0 == fieldTypes[index])
            {
                sb.AppendFormat("{0}={1}", fieldNames[index], value);
            }
            else if (1 == fieldTypes[index])
            {
                sb.AppendFormat("{0}='{1}'", fieldNames[index], value);
            }
            else if (2 == fieldTypes[index])
            {
                sb.AppendFormat("{0}={0}+{1}", fieldNames[index], value);
            }
            else if (3 == fieldTypes[index])
            {
                sb.AppendFormat("{0}='{1}'", fieldNames[index], value.ToString().Replace('$', ':'));
            }
        }

        public static void UpdateGoodsDataFromArgs(GoodsData gd, UpdateGoodsArgs args)
        {
            foreach (var idx in args.ChangedIndexes)
            {
                if (idx == UpdatePropIndexes.binding)
                {
                    gd.Binding = args.Binding;
                }
            }
        }

        #endregion 更新物品扩展

        #region 星座系统

        /// <summary>
        /// 更新星座信息
        /// </summary>
        public static int UpdateRoleStarConstellationInfo(DBManager dbMgr, int roleid, int nStarSite, int nStarSlot)
        {
            int ret = 0;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("REPLACE INTO t_starconstellationinfo(roleid, starsiteid, starslotid) VALUES({0}, {1}, {2})", roleid, nStarSite, nStarSlot);

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

                ret = 1;
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

        #endregion 星座系统

        #region 保存钻石消费记录
        public static int SaveConsumeLog(DBManager dbMgr, int roleid, string cdate, int ctype, int amount)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_consumelog (rid, amount, cdate, ctype) VALUES({0}, {1}, '{2}', {3})",
                    roleid, amount, cdate, ctype);

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", cmdText), EventLevels.Important);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    cmd.ExecuteNonQuery();
                    ret = 0;
                }
                catch (Exception)
                {
                    ret = -1;
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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
        #endregion 保存钻石消费记录

        #region MU VIP相关

        /// <summary>
        /// 更新VIP等级奖励标记信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateVipLevelAwardFlagInfo(DBManager dbMgr, string strUserid, int nFlag)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET vipawardflag={0} WHERE userid='{1}'", nFlag, strUserid);

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

            return ret;
        }

        /// <summary>
        /// 根据roleid更新VIP等级奖励标记信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateVipLevelAwardFlagInfoByRoleID(DBManager dbMgr, int nRoleid, int nFlag)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("UPDATE t_roles SET vipawardflag={0} WHERE rid={1}", nFlag, nRoleid);

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

            return ret;
        }

        #endregion MU VIP相关

        #region 准备数据库表

        /// <summary>
        /// 执行一个sql语句，并返回读取的整数结果
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="sqlText"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int ExecuteSQLReadInt(DBManager dbMgr, string sqlText, MySQLConnection conn = null)
        {
            int result = 0;
            bool keepConn = true;

            MySQLCommand cmd = null;
            try
            {
                if (conn == null)
                {
                    keepConn = false;
                    conn = dbMgr.DBConns.PopDBConnection();
                }

                using (cmd = new MySQLCommand(sqlText, conn))
                {
                    try
                    {
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            result = Convert.ToInt32(reader[0].ToString());
                        }
                        reader.Close();
                    }
                    catch
                    {
                        return -1;
                    }
                }
            }
            catch (System.Exception ex)
            {
                return -2;
            }
            finally
            {
                if (!keepConn && null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return result;
        }

        /// <summary>
        /// 清理物品表的垃圾数据，转移到t_goods_bak和t_goodsprops_bak表
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int ValidateDatabase(DBManager dbMgr, string dbName)
        {
            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                int flag_t_roles_auto_increment = GameDBManager.GameConfigMgr.GetGameConfigItemInt("flag_t_roles_auto_increment", 0);
                //最低为200000，且必须是100000的整数倍
                if (flag_t_roles_auto_increment < 200000 || (flag_t_roles_auto_increment % 100000) != 0)
                {
                    Global.LogAndExitProcess("flag_t_roles_auto_increment 未设置");
                }

                GameDBManager.DBAutoIncreaseStepValue = flag_t_roles_auto_increment;

                string sqlText = string.Format("SELECT COUNT(*) FROM information_schema.columns WHERE table_schema='{0}' AND table_name = 't_login' AND column_name='userid';", dbName);
                int result = ExecuteSQLReadInt(dbMgr, sqlText, conn);
                if (result <= 0)
                {
                    Global.LogAndExitProcess("t_login 错误");
                }
                sqlText = string.Format("SELECT COUNT(*) FROM information_schema.columns WHERE table_schema='{0}' AND table_name = 't_usemoney_log' AND column_name='Id';", dbName);
                result = ExecuteSQLReadInt(dbMgr, sqlText, conn);
                if (result <= 0)
                {
                    Global.LogAndExitProcess("t_usemoney_log 错误");
                }
                sqlText = string.Format("SELECT COUNT(*) FROM information_schema.columns WHERE table_schema='{0}' AND table_name = 't_goods_bak' AND column_name='Id';", dbName);
                result = ExecuteSQLReadInt(dbMgr, sqlText, conn);
                if (result <= 0)
                {
                    Global.LogAndExitProcess("t_goods_bak 错误");
                }
            }
            catch (MySQLException ex)
            {
                LogManager.WriteException(string.Format("检查数据库是否正确时发生异常: {0}", ex.ToString()));
                throw new Exception(string.Format("检查数据库是否正确时发生异常: {0}", ex.ToString()));
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return 1;
        }

        #endregion 准备数据库表

        #region 转移和清理清理已标记删除的数据

        /// <summary>
        /// 清理物品表的垃圾数据，转移到t_goods_bak和t_goodsprops_bak表
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int ClearUnusedGoodsData(DBManager dbMgr, bool clearAll = false)
        {
            int maxGoodsDBID = -1;
            int minGoodsDBID = -1;
            MySQLConnection conn = null;
            try
            {
                int toClearDBID = 0;
                string cmdText;
                conn = dbMgr.DBConns.PopDBConnection();
                MySQLCommand cmd = null;
                try
                {
                    using (cmd = new MySQLCommand("SELECT MAX(id) FROM t_goods", conn))
                    {
                        try
                        {
                            MySQLDataReader reader = cmd.ExecuteReaderEx();
                            if (reader.Read())
                            {
                                maxGoodsDBID = Convert.ToInt32(reader[0].ToString());
                            }
                            reader.Close();
                        }
                        catch
                        {
                            return -1;
                        }
                    }
                    using (cmd = new MySQLCommand("SELECT MIN(id) FROM t_goods", conn))
                    {
                        try
                        {
                            MySQLDataReader reader = cmd.ExecuteReaderEx();
                            if (reader.Read())
                            {
                                minGoodsDBID = Convert.ToInt32(reader[0].ToString());
                            }
                            reader.Close();
                        }
                        catch
                        {
                            return -1;
                        }
                    }
                }
                catch (MySQLException ex)
                {
                    LogManager.WriteException(string.Format("清理t_goods表时异常: {0}", ex.ToString()));
                    return -1;
                }

                try
                {
                    int last_goods_dbid = GameDBManager.GameConfigMgr.GetGameConfigItemInt("last_clear_goods_dbid", 0);
                    if (last_goods_dbid > minGoodsDBID)
                    {
                        //最小值设置有效的最小值
                        minGoodsDBID = last_goods_dbid;
                    }

                    int max_clear_goods_count = GameDBManager.GameConfigMgr.GetGameConfigItemInt("max_clear_goods_count", 1);
                    toClearDBID = minGoodsDBID + max_clear_goods_count;
                    if (maxGoodsDBID < toClearDBID)
                    {
                        //数量太少，不值得清理
                        return 0;
                    }

                    if (clearAll)
                    {
                        //清理所有的
                        toClearDBID = maxGoodsDBID;
                    }

                    //更新清理到的goodsDBID参数
                    GameDBManager.GameConfigMgr.UpdateGameConfigItem("last_goods_dbid", toClearDBID.ToString());
                    DBWriter.UpdateGameConfig(dbMgr, "last_goods_dbid", toClearDBID.ToString());

                    cmdText = string.Format("INSERT INTO t_goods_bak SELECT *,0,NOW(),0 FROM t_goods WHERE gcount <= 0 AND id > {0} AND id <= {1}", minGoodsDBID, toClearDBID);
                    using (cmd = new MySQLCommand(cmdText, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    cmdText = string.Format("DELETE FROM t_goods WHERE gcount <= 0 AND id > {0} AND id <= {1}", minGoodsDBID, toClearDBID);
                    using (cmd = new MySQLCommand(cmdText, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (MySQLException ex)
                {
                    LogManager.WriteException(string.Format("清理t_goods表时异常: {0}", ex.ToString()));
                    return -1;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteException(string.Format("清理t_goods表时异常: {0}", ex.ToString()));
                return -1;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return 1;
        }

        #endregion 转移和清理清理已标记删除的数据

        #region 消费日志相关

        /// <summary>
        /// 插入万魔塔场数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int insertItemLog(DBManager dbMgr, String[] fields)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                //String strTableName = "t_log_" + DateTime.Now.ToString("yyyyMMdd");
                String strTableName = "t_usemoney_log";
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO {0} (DBId, userid, ObjName, optFrom, currEnvName, tarEnvName, optType, optTime, optAmount, zoneID) VALUES({1}, '{9}', '{2}', '{3}', '{4}', '{5}', '{6}', now(), {7}, {8})",
                                                strTableName, fields[0], fields[1], fields[2], fields[3], fields[4], fields[5], fields[6], fields[7], fields[8]);

                MySQLCommand cmd = new MySQLCommand(cmdText, conn);

                try
                {
                    ret = cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", cmdText));
                }

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

        #endregion 消费日志相关
    }
}
