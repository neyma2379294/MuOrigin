using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using MySQLDriverCS;
using GameDBServer.Logic;
using System.Data;
using Server.Tools;
using GameDBServer.Logic.WanMoTa;

namespace GameDBServer.DB.DBController
{
    public class WanMoTaDBController : DBController<WanMotaInfo>
    {
        private static WanMoTaDBController instance = new WanMoTaDBController();

        private WanMoTaDBController() : base() { }

        public static WanMoTaDBController getInstance()
        {
            return instance;
        }

        /// <summary>
        /// 根据ID获取万魔塔数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public WanMotaInfo getPlayerWanMoTaDataById(int Id)
        {
            string sql = string.Format("select * from t_wanmota where roleID = {0};", Id);

            return this.queryForObject(sql);
        }

        /// <summary>
        /// 获取排名前500的万魔塔数据
        /// </summary>
        /// <returns></returns>
        public List<WanMotaInfo> getPlayerWanMotaDataList()
        {
            // string sql = string.Format("select * from t_wanmota where passLayerCount > 0 order by passLayerCount desc, flushTime asc limit {0};", WanMoTaManager.RankingList_Max_Num);
            string sql = string.Format("select * from t_wanmota order by passLayerCount desc, flushTime asc limit {0};", WanMoTaManager.RankingList_Max_Num);

            return this.queryForList(sql);
        }

        /// <summary>
        /// 更新万魔塔场数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int updateWanMoTaData(DBManager dbMgr, int nRoleID, string[] fields, int startIndex)
        {
            int ret = -1;
            MySQLConnection conn = null;
            string[] fieldNames = { "flushTime", "passLayerCount", "sweepLayer", "sweepReward", "sweepBeginTime" };
            byte[] fieldTypes = { 0, 0, 0, 1, 0 };

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = DBWriter.FormatUpdateSQL(nRoleID, fields, startIndex, fieldNames, "t_wanmota", fieldTypes, "roleID");

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
        /// 插入万魔塔场数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int insertWanMoTaData(DBManager dbMgr, WanMotaInfo data)
        {
            int ret = -1;
            MySQLConnection conn = null;

            try
            {
                bool error = false;
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = string.Format("INSERT INTO t_wanmota (roleID, roleName, flushTime, passLayerCount, sweepLayer, sweepReward, sweepBeginTime) VALUES({0}, '{1}', {2}, {3}, {4}, '{5}', {6})",
                                                data.nRoleID, data.strRoleName, data.lFlushTime, data.nPassLayerCount,
                                                data.nSweepLayer, data.strSweepReward, data.lSweepBeginTime);

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
    }
}
