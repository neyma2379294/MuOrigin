using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using Server.TCP;
using Server.Tools;
using Server.Protocol;
using GameServer.Server;

namespace GameServer.Logic
{
    /// <summary>
    /// 数据库命令队列管理
    /// </summary>
    public class LogDBCmdManager
    {
        /// <summary>
        /// 数据库命令池
        /// </summary>
        private DBCmdPool _DBCmdPool = new DBCmdPool(2000);

        /// <summary>
        /// 等待处理的数据库命令队列
        /// </summary>
        private Queue<DBCommand> _DBCmdQueue = new Queue<DBCommand>(2000);

        /// <summary>
        /// 添加一个新的数据库命令到队列中
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="cmdText"></param>
        public void AddDBLogInfo(int nGoodDBID, string strObjName, string strFrom, string strCurrEnvName, string strTarEnvName, string strOptType, int nAmount, int nZoneID, string userid)
        {
            if ("" == strObjName)
            {
                return;
            }

            AddGameDBLogInfo(nGoodDBID, strObjName, strFrom, strCurrEnvName, strTarEnvName, strOptType, nAmount, nZoneID, userid);

            //是否禁用交易市场购买功能
            int disableDBLog = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-dblog", 0);
            if (disableDBLog > 0)
            {
                return;
            }

            strFrom = strFrom.Replace(':', '-');
            String strLogInfo = String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", nGoodDBID, strObjName, strFrom, strCurrEnvName, strTarEnvName, strOptType, nAmount, nZoneID);
            AddDBCmd((int)TCPGameServerCmds.CMD_LOGDB_ADD_ITEM_LOG, strLogInfo, null);
        }

        /// <summary>
        /// 游戏服务器记录的消费日志
        /// </summary>
        /// <param name="nGoodDBID"></param>
        /// <param name="strObjName"></param>
        /// <param name="strFrom"></param>
        /// <param name="strCurrEnvName"></param>
        /// <param name="strTarEnvName"></param>
        /// <param name="strOptType"></param>
        /// <param name="nAmount"></param>
        /// <param name="nZoneID"></param>
        /// <param name="userid"></param>
        public void AddGameDBLogInfo(int nGoodDBID, string strObjName, string strFrom, string strCurrEnvName, string strTarEnvName, string strOptType, int nAmount, int nZoneID, string userid)
        {
            if ("钻石" != strObjName)
            {
                return;
            }

            strFrom = strFrom.Replace(':', '-');
            string strLogInfo = String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", nGoodDBID, strObjName, strFrom, strCurrEnvName, strTarEnvName, strOptType, nAmount, nZoneID, userid);
            Global.ExecuteDBCmd((int)TCPGameServerCmds.CMD_LOGDB_ADD_ITEM_LOG, strLogInfo);
        }

        /// <summary>
        /// 添加一个新的数据库命令到队列中
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="cmdText"></param>
        private void AddDBCmd(int cmdID, string cmdText, DBCommandEventHandler dbCommandEvent)
        {
            DBCommand dbCmd = _DBCmdPool.Pop();
            if (null == dbCmd)
            {
                dbCmd = new DBCommand();
            }

            dbCmd.DBCommandID = cmdID;
            dbCmd.DBCommandText = cmdText;
            if (null != dbCommandEvent)
            {
                dbCmd.DBCommandEvent += dbCommandEvent;
            }

            lock (_DBCmdQueue)
            {
                _DBCmdQueue.Enqueue(dbCmd);
            }
        }

        /// <summary>
        /// 获取等待处理的DBCmd数量个数
        /// </summary>
        /// <returns></returns>
        public int GetDBCmdCount()
        {
            lock (_DBCmdQueue)
            {
                return _DBCmdQueue.Count;
            }
        }

        /// <summary>
        /// 执行数据库命令
        /// </summary>
        /// <param name="tcpClientPool"></param>
        /// <param name="pool"></param>
        /// <param name="dbCmd"></param>
        /// <returns></returns>
        private TCPProcessCmdResults DoDBCmd(TCPClientPool tcpClientPool, TCPOutPacketPool pool, DBCommand dbCmd, out byte[] bytesData)
        {
            bytesData = Global.SendAndRecvData(tcpClientPool, pool, dbCmd.DBCommandText, dbCmd.DBCommandID);
            if (null == bytesData || bytesData.Length <= 0)
            {
                return TCPProcessCmdResults.RESULT_FAILED;
            }

            return TCPProcessCmdResults.RESULT_OK;
        }

        /// <summary>
        /// 执行数据库命令
        /// </summary>
        public void ExecuteDBCmd(TCPClientPool tcpClientPool, TCPOutPacketPool pool)
        {
            //int nTestCount = 2000;
            //for (int i = 0; i < nTestCount; i++)
            //{
            //    AddDBLogInfo(123, "0000000", "111111111", "22222222222222", "增加", 1, 1);
            //}

            lock (_DBCmdQueue)
            {
                if (_DBCmdQueue.Count <= 0) return;
            }

            List<DBCommand> dbCmdList = new List<DBCommand>();
            lock (_DBCmdQueue)
            {
                while (_DBCmdQueue.Count > 0)
                {
                    dbCmdList.Add(_DBCmdQueue.Dequeue());
                }
            }
           
            byte[] bytesData = null;
            TCPProcessCmdResults result;
            //long ticks = DateTime.Now.Ticks / 10000;
            for (int i = 0; i < dbCmdList.Count; i++)
            {
                result = DoDBCmd(tcpClientPool, pool, dbCmdList[i], out bytesData);
                if (result == TCPProcessCmdResults.RESULT_FAILED)
                {
                    //写日志
                    LogManager.WriteLog(LogTypes.Error, string.Format("向LogDBServer请求执行命令失败, CMD={0}", (TCPGameServerCmds)dbCmdList[i].DBCommandID));
                }               

                //还回队列
                _DBCmdPool.Push(dbCmdList[i]);
            }

            //System.Console.WriteLine(string.Format("发送{0}条日志到数据库耗时 {1}", nTestCount, DateTime.Now.Ticks / 10000 - ticks));
        }
    }
}
