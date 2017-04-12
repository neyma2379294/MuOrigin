using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
//using System.Windows.Forms;
using GameDBServer.DB;
using Server.Data;
using ProtoBuf;
using GameDBServer.Logic;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 聊天消息的管理
    /// </summary>
    public class ChatMsgManager
    {
        #region 数据定义

        /// <summary>
        /// 存储分线的聊天消息的队列字典
        /// </summary>
        private static Dictionary<int, Queue<string>> ChatMsgDict = new Dictionary<int, Queue<string>>();

        #endregion 数据定义

        #region 存储消息

        /// <summary>
        /// 根据线路ID获取聊天的消息
        /// </summary>
        /// <param name="serverLineID"></param>
        /// <returns></returns>
        private static Queue<string> GetChatMsgQueue(int serverLineID)
        {
            Queue<string> msgQueue = null;
            lock (ChatMsgDict)
            {
                if (!ChatMsgDict.TryGetValue(serverLineID, out msgQueue))
                {
                    msgQueue = new Queue<string>();
                    ChatMsgDict[serverLineID] = msgQueue;
                }
            }

            return msgQueue;
        }

        /// <summary>
        /// 添加GM命令消息
        /// </summary>
        /// <param name="serverLineID"></param>
        /// <param name="gmCmd"></param>
        public static void AddGMCmdChatMsg(int serverLineID, string gmCmd)
        {
            string chatMsg = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", 0, "", 0, "", 0, gmCmd, 0, 0, serverLineID);            
            List<LineItem> itemList = LineManager.GetLineItemList();
            if (null != itemList)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (itemList[i].LineID == serverLineID)
                    {
                        continue;
                    }

                    ChatMsgManager.AddChatMsg(itemList[i].LineID, chatMsg);
                }
            }
        }

        /// <summary>
        /// 存储聊天消息
        /// </summary>
        /// <param name="serverLineID"></param>
        /// <param name="chatMsg"></param>
        public static void AddChatMsg(int serverLineID, string chatMsg)
        {
            Queue<string> msgQueue = GetChatMsgQueue(serverLineID);
            lock (msgQueue)
            {
                if (msgQueue.Count > 3000) //大于这个临界数字，表示服务器端没有工作, 防止浪费太多内存
                {
                    msgQueue.Clear();
                }

                msgQueue.Enqueue(chatMsg);
            }
        }

        #endregion 存储消息

        #region 获取消息

        /// <summary>
        /// 获取指定线路上的所有正在等待的消息
        /// </summary>
        /// <param name="serverLineID"></param>
        /// <returns></returns>
        public static TCPOutPacket GetWaitingChatMsg(TCPOutPacketPool pool, int cmdID, int serverLineID)
        {
            TCPOutPacket tcpOutPacket = null;

            List<string> msgList = new List<string>();
            Queue<string> msgQueue = GetChatMsgQueue(serverLineID);
            lock (msgQueue)
            {
                while (msgQueue.Count > 0 && msgList.Count < 100)
                {
                    msgList.Add(msgQueue.Dequeue());
                }
            }

            tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<string>>(msgList, pool, cmdID);
            return tcpOutPacket;
        }

        #endregion 获取消息

        #region 将GM的消息的流水发送到客户端

        /// <summary>
        /// 上次扫描流水的时间
        /// </summary>
        private static long LastScanInputGMMsgTicks = DateTime.Now.Ticks / 10000;

        /// <summary>
        /// 扫描GM命令流水发送到客户端
        /// </summary>
        public static void ScanGMMsgToGameServer(DBManager dbMgr)
        {
            try
            {
                long nowTicks = DateTime.Now.Ticks / 10000;
                if (nowTicks - LastScanInputGMMsgTicks < (10 * 1000))
                {
                    return;
                }

                LastScanInputGMMsgTicks = nowTicks;

                List<string> msgList = new List<string>();

                /// 查询GM命令的，并且写入日志中
                DBQuery.ScanGMMsgFromTable(dbMgr, msgList);

                // 如果是-config -- 更新缓存  [9/22/2013 LiaoWei]
                bool reloadConfig = false;
                for (int i = 0; i < msgList.Count; i++)
                {
                    string msg = msgList[i].Replace(":", "：");
                    ChatMsgManager.AddGMCmdChatMsg(-1, msg);

                    if (msg.IndexOf("-config ") >= 0)
                    {
                        reloadConfig = true;
                    }
                }

                if (reloadConfig)
                {
                    // 注意！！ [9/22/2013 LiaoWei]
                    GameDBManager.GameConfigMgr.LoadGameConfigFromDB(dbMgr);
                }
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("扫描GM命令表时发生了错误"));
            }
        }

        #endregion 将GM的消息的流水发送到客户端
    }
}
