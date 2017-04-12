using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Logic;
using GameDBServer.Server;
using Server.Protocol;
using Server.Tools;
using Server.Data;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 公告消息管理
    /// </summary>
    public class BulletinMsgManager
    {
        #region 基础数据

        /// <summary>
        /// 公告字典
        /// </summary>
        private Dictionary<string, BulletinMsgData> _BulletinMsgDict = new Dictionary<string, BulletinMsgData>();

        #endregion 基础数据

        #region 基础方法

        /// <summary>
        /// 从数据库中获取永久的公告数据
        /// </summary>
        public void LoadBulletinMsgFromDB(DBManager dbMgr)
        {
            //查询系统公告列表
            _BulletinMsgDict = DBQuery.QueryBulletinMsgDict(dbMgr);
            if (null == _BulletinMsgDict)
            {
                _BulletinMsgDict = new Dictionary<string, BulletinMsgData>();
            }
        }

        /// <summary>
        /// 发布公告消息
        /// </summary>
        /// <param name="msgID"></param>
        /// <param name="playMinutes"></param>
        /// <param name="playNum"></param>
        /// <param name="bulletinText"></param>
        public BulletinMsgData AddBulletinMsg(string msgID, int playNum, string bulletinText)
        {
            BulletinMsgData bulletinMsgData = new BulletinMsgData()
            {
                MsgID = msgID,
                PlayMinutes = -1,
                ToPlayNum = playNum,
                BulletinText = bulletinText,
                BulletinTicks = DateTime.Now.Ticks / 10000,
            };

            lock (_BulletinMsgDict)
            {
                _BulletinMsgDict[msgID] = bulletinMsgData;
            }

            return bulletinMsgData;
        }

        /// <summary>
        /// 删除公告消息
        /// </summary>
        /// <param name="msgID"></param>
        public void RemoveBulletinMsg(string msgID)
        {
            lock (_BulletinMsgDict)
            {
                _BulletinMsgDict.Remove(msgID);
            }
        }

        /// <summary>
        /// 获取公告消息词典的发送tcp对象
        /// </summary>
        public TCPOutPacket GetBulletinMsgDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
        {
            TCPOutPacket tcpOutPacket = null;
            lock (_BulletinMsgDict)
            {
                tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, BulletinMsgData>>(_BulletinMsgDict, pool, cmdID);
            }

            return tcpOutPacket;
        }

        #endregion 基础方法
    }
}
