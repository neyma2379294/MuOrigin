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
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Protocol;

namespace GameDBServer.Logic
{
    class CFirstChargeMgr
    {
        #region 成员变量
        public static Dictionary<int, int> _FirstChargeConfig = null;
        #endregion
        /// <summary>
        /// 从gameserver传过来的配置表
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="pool"></param>
        /// <param name="nID"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="tcpOutPacket"></param>
        /// <returns></returns>
        public static TCPProcessCmdResults FirstChargeConfig(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            try
            {
                Dictionary<int, int> tmpdict = null;
                tmpdict = DataHelper.BytesToObject<Dictionary<int, int>>(data, 0, count);

                _FirstChargeConfig = tmpdict;
                byte[] retBytes = DataHelper.ObjectToBytes<int>(1);
                tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, retBytes, 0, retBytes.Length, nID);

            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "", false);
            }
            return TCPProcessCmdResults.RESULT_DATA;


        }
        /// <summary>
        /// 发送给gameserver 增加角色绑钻
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="uid"></param>
        /// <param name="rid"></param>
        /// <param name="addMoney"></param>
        public static void SendToRolebindgold(DBManager dbMgr, string uid,int rid,int addMoney)
        {
            string data = GetFirstChargeInfo(dbMgr, uid);
            if (_FirstChargeConfig == null)
            {
                LogManager.WriteException("送绑钻失败，配置表信息为空 uid="+uid+" money="+addMoney);
                return;
            }
            if (!_FirstChargeConfig.ContainsKey(addMoney))
                return;
            if (!string.IsNullOrEmpty(data))
            {
                string[] datalist = data.Split(',');

                foreach (string item in datalist)
                {
                    //已经充过了
                    if (item == addMoney.ToString())
                    {
                        return;
                    }

                }
                data += "," + addMoney;
            }
            else
            {
                data = "" + addMoney;
            }
            int bindMoney = _FirstChargeConfig[addMoney];
            //更新数据库首次记录
            bool svResoult=UpdateFirstCharge(dbMgr, uid, data);
            if (!svResoult)
            {
                LogManager.WriteException("送绑钻失败，保存数据库失败 uid=" + uid + " money=" + addMoney);
                return;
            }
                
            //添加GM命令消息
            string gmCmdData = string.Format("-updateBindgold {0} {1} {2} {3}", uid, rid, bindMoney, data);
            ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);

        }
        #region 查询首次充值
        public static string GetFirstChargeInfo(DBManager dbMgr, string uid)
        {
            string resoult = "-1";

            MySQLConnection conn = null;
            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                string cmdText = string.Format("SELECT  charge_info FROM t_firstcharge WHERE uid = '{0}'", uid);
                MySQLCommand cmd = new MySQLCommand(cmdText, conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                try
                {
                    if (reader.Read())
                    {
                        resoult = reader["charge_info"].ToString();
                        if (string.IsNullOrEmpty(resoult))
                        {
                            resoult = "-1";
                        }
                        // resoult +="|"+ reader["notget"].ToString();

                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteException("GetFirstChargeInfo excepton=" + ex.ToString());
                    resoult = "-2";
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

            return resoult;
        }
        #endregion

        #region 更新首次充值信息
        public static bool UpdateFirstCharge(DBManager dbMgr, string userId, string chargeinfo, int notget = 0)
        {
            bool ret = false;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                string cmdText = "";
                cmdText = string.Format("REPLACE  INTO t_firstcharge (uid, charge_info, notget) VALUES('{0}', '{1}', '{2}')", userId, chargeinfo, notget);

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
        #endregion


        #region 每档首次充值领取绑钻

       

        #region 废弃代码
        //public static TCPProcessCmdResults ProcessSaveUserFirstCharge(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        //{
        //    tcpOutPacket = null;
        //    string cmdData = null;

        //    try
        //    {
        //        cmdData = new UTF8Encoding().GetString(data, 0, count);
        //    }
        //    catch (Exception) //解析错误
        //    {
        //        LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID));

        //        tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", (int)TCPGameServerCmds.CMD_DB_ERR_RETURN);
        //        return TCPProcessCmdResults.RESULT_DATA;
        //    }

        //    try
        //    {

        //        string[] fields = cmdData.Split(':');
        //        if (fields.Length != 3)
        //        {
        //            LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}",
        //                (TCPGameServerCmds)nID, fields.Length, cmdData));

        //            tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", (int)TCPGameServerCmds.CMD_DB_ERR_RETURN);
        //            return TCPProcessCmdResults.RESULT_DATA;
        //        }

        //        string uid = fields[0];
        //        string firstdata = fields[1];
        //        int notget = Global.SafeConvertToInt32(fields[2]);
        //        DBUserInfo info = dbMgr.GetDBUserInfo(uid);

        //        if (null == info)
        //        {
        //            LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的账号不存在，CMD={0}, RoleID={1}",
        //                (TCPGameServerCmds)nID, uid));

        //            tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", (int)TCPGameServerCmds.CMD_DB_ERR_RETURN);
        //            return TCPProcessCmdResults.RESULT_DATA;
        //        }
        //        if (notget > 0)
        //        {

        //        }
        //        DBUserInfo userInfo = dbMgr.GetDBUserInfo(uid);
        //        bool ret = UpdateFirstCharge(dbMgr, uid, firstdata);

        //        if (ret)
        //            tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
        //        else
        //            tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", (int)TCPGameServerCmds.CMD_DB_ERR_RETURN);

        //        return TCPProcessCmdResults.RESULT_DATA;
        //    }
        //    catch (Exception e)
        //    {
        //        LogManager.WriteException("ProcessSaveUserFirstCharge:" + e.ToString());
        //    }
        //    return TCPProcessCmdResults.RESULT_DATA;
        //}
        #endregion 

        /// <summary>
        /// 查询每档首次充值信息
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="pool"></param>
        /// <param name="nID"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="tcpOutPacket"></param>
        /// <returns></returns>
        public static TCPProcessCmdResults ProcessQueryUserFirstCharge(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            string cmdData = null;

            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception) //解析错误
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID));

                tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", (int)TCPGameServerCmds.CMD_DB_ERR_RETURN);
                return TCPProcessCmdResults.RESULT_DATA;
            }

            try
            {

                string[] fields = cmdData.Split(':');
                if (fields.Length != 1)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}",
                        (TCPGameServerCmds)nID, fields.Length, cmdData));

                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", (int)TCPGameServerCmds.CMD_DB_ERR_RETURN);
                    return TCPProcessCmdResults.RESULT_DATA;
                }

                string uid = fields[0];

                DBUserInfo info = dbMgr.GetDBUserInfo(uid);

                if (null == info)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的账号不存在，CMD={0}, RoleID={1}",
                        (TCPGameServerCmds)nID, uid));

                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", (int)TCPGameServerCmds.CMD_DB_ERR_RETURN);
                    return TCPProcessCmdResults.RESULT_DATA;
                }

                DBUserInfo userInfo = dbMgr.GetDBUserInfo(uid);
                string ret = GetFirstChargeInfo(dbMgr, uid);

                if (ret != "-2")
                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, ret, nID);
                else
                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", (int)TCPGameServerCmds.CMD_DB_ERR_RETURN);

                return TCPProcessCmdResults.RESULT_DATA;
            }
            catch (Exception e)
            {
                LogManager.WriteException("ProcessSaveUserFirstCharge:" + e.ToString());
            }
            return TCPProcessCmdResults.RESULT_DATA;
        }
        #endregion
    }
}
