using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySQLDriverCS;
using System.Data;
using System.Windows;
using Server.Tools;
using Server.Data;
using ProtoBuf;
using Server.Protocol;
using GameDBServer.Logic;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 副本历史记录管理
    /// </summary>
    public class FuBenHistManager
    {
        #region 数据定义

        /// <summary>
        /// 线程间的互斥锁
        /// </summary>
        private static object _Mutex = new object();

        /// <summary>
        /// 数据列表
        /// </summary>
        private static Dictionary<int, FuBenHistData> _FuBenHistDict = null;

        #endregion 数据定义

        #region 方法定义

        /// <summary>
        /// 从数据库总加载副本通关记录排行榜
        /// </summary>
        /// <param name="conn"></param>
        public static void LoadFuBenHist(DBManager dbMgr)
        {
            lock (_Mutex)
            {
                /// 查询副本通关记录排行榜
                _FuBenHistDict = DBQuery.QueryFuBenHistDict(dbMgr);
            }
        }

        /// <summary>
        /// 根据副本ID查找副本的历史记录
        /// </summary>
        /// <param name="fuBenID"></param>
        /// <returns></returns>
        public static FuBenHistData FindFuBenHistDataByID(int fuBenID)
        {
            lock (_Mutex)
            {
                FuBenHistData fuBenHistData = null;
                if (!_FuBenHistDict.TryGetValue(fuBenID, out fuBenHistData))
                {
                    return null;
                }

                return fuBenHistData;
            }
        }

        /// <summary>
        /// 添加新的副本通关历史记录
        /// </summary>
        /// <param name="fuBenID"></param>
        /// <returns></returns>
        public static void AddFuBenHistData(int fuBenID, int roleID, string roleName, int usedSecs)
        {
            FuBenHistData fuBenHistData = new FuBenHistData()
            {
                FuBenID = fuBenID,
                RoleID = roleID,
                RoleName = roleName,
                UsedSecs = usedSecs,
            };

            lock (_Mutex)
            {
                _FuBenHistDict[fuBenID] = fuBenHistData;
            }
        }

        /// <summary>
        /// 获取副本列表数据
        /// </summary>
        /// <returns></returns>
        public static TCPOutPacket GetFuBenHistListData(TCPOutPacketPool pool, int cmdID)
        {
            lock (_Mutex)
            {
                return DataHelper.ObjectToTCPOutPacket<Dictionary<int, FuBenHistData>>(_FuBenHistDict, pool, cmdID);
            }
        }

        #endregion 方法定义
    }
}
