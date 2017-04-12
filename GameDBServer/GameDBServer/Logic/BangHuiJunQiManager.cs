using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using GameDBServer.DB;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 帮会军旗管理
    /// </summary>
    public class BangHuiJunQiManager
    {
        #region 基础数据

        /// <summary>
        /// 帮旗信息字典
        /// </summary>
        private Dictionary<int, BangHuiJunQiItemData> _BangHuiJunQiItemsDict = new Dictionary<int, BangHuiJunQiItemData>();

        #endregion 基础数据

        #region 基础方法

        /// <summary>
        /// 从数据库中获取帮旗列表
        /// </summary>
        public void LoadBangHuiJunQiItemFromDB(DBManager dbMgr)
        {
            //查询帮旗字典数据
            DBQuery.QueryBangQiDict(dbMgr, _BangHuiJunQiItemsDict);
        }

        /// <summary>
        /// 添加新的军旗
        /// </summary>
        public void AddBangHuiJunQi(int bhid, string qiName, int qiLevel)
        {
            lock (_BangHuiJunQiItemsDict)
            {
                _BangHuiJunQiItemsDict[bhid] = new BangHuiJunQiItemData()
                {
                    BHID = bhid,
                    QiName = qiName,
                    QiLevel = qiLevel,
                };
            }
        }

        /// <summary>
        /// 删除军旗
        /// </summary>
        public void RemoveBangHuiJunQi(int bhid)
        {
            lock (_BangHuiJunQiItemsDict)
            {
                _BangHuiJunQiItemsDict.Remove(bhid);
            }
        }

        /// <summary>
        /// 更新帮旗名称
        /// </summary>
        public void UpdateBangHuiQiName(int bhid, string qiName)
        {
            BangHuiJunQiItemData bangHuiJunQiItemData = null;
            lock (_BangHuiJunQiItemsDict)
            {
                if (!_BangHuiJunQiItemsDict.TryGetValue(bhid, out bangHuiJunQiItemData))
                {
                    return;
                }

                bangHuiJunQiItemData.QiName = qiName;
            }
        }

        /// <summary>
        /// 更新帮旗级别
        /// </summary>
        public void UpdateBangHuiQiLevel(int bhid, int qiLevel)
        {
            BangHuiJunQiItemData bangHuiJunQiItemData = null;
            lock (_BangHuiJunQiItemsDict)
            {
                if (!_BangHuiJunQiItemsDict.TryGetValue(bhid, out bangHuiJunQiItemData))
                {
                    return;
                }

                bangHuiJunQiItemData.QiLevel = qiLevel;
            }
        }

        /// <summary>
        /// 获取帮旗信息字典项的发送tcp对象
        /// </summary>
        public TCPOutPacket GetBangHuiJunQiItemsDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
        {
            TCPOutPacket tcpOutPacket = null;
            lock (_BangHuiJunQiItemsDict)
            {
                tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiJunQiItemData>>(_BangHuiJunQiItemsDict, pool, cmdID);
            }

            return tcpOutPacket;
        }

        #endregion 基础方法
    }
}
