using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.DB
{
    /// <summary>
    /// 用户信息管理(使用弱引用，当系统内存不足时，让系统自动释放内存)
    /// </summary>
    public class DBUserMgr
    {
        /// <summary>
        /// 记录用户信息的词典对象
        /// </summary>
        private Dictionary<string, MyWeakReference> DictUserInfos = new Dictionary<string, MyWeakReference>(10000);

        /// <summary>
        /// 获取个数
        /// </summary>
        /// <returns></returns>
        public int GetUserInfoCount()
        {
            lock (DictUserInfos)
            {
                return DictUserInfos.Count;
            }
        }

        /// <summary>
        /// 定位用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DBUserInfo FindDBUserInfo(string userID)
        {
            DBUserInfo dbUserInfo = null;
            MyWeakReference weakRef = null;
            lock (DictUserInfos)
            {
                if (DictUserInfos.Count > 0)
                {
                    if (DictUserInfos.TryGetValue(userID, out weakRef))
                    {
                        if (weakRef.IsAlive) //判断是否仍在存活
                        {
                            dbUserInfo = weakRef.Target as DBUserInfo;
                        }
                    }
                }
            }

            if (null != dbUserInfo)
            {
                lock (dbUserInfo)
                {
                    dbUserInfo.LastReferenceTicks = DateTime.Now.Ticks / 10000;
                }
            }

            return dbUserInfo;
        }

        /// <summary>
        /// 添加用户信息到字典中
        /// </summary>
        /// <param name="dbUserInfo"></param>
        public void AddDBUserInfo(DBUserInfo dbUserInfo)
        {
            MyWeakReference weakRef = null;
            lock (DictUserInfos)
            {
                if (DictUserInfos.TryGetValue(dbUserInfo.UserID, out weakRef))
                {
                    weakRef.Target = dbUserInfo;
                }
                else
                {
                    DictUserInfos.Add(dbUserInfo.UserID, new MyWeakReference(dbUserInfo));
                }
            }
        }

        /// <summary>
        /// 从字典中删除用户信息
        /// </summary>
        /// <param name="dbUserInfo"></param>
        public void RemoveDBUserInfo(string userID)
        {
            MyWeakReference weakRef = null;
            lock (DictUserInfos)
            {
                if (DictUserInfos.TryGetValue(userID, out weakRef))
                {
                    weakRef.Target = null; //释放内存
                }
            }
        }

        /// <summary>
        /// 释放空闲的用户信息
        /// </summary>
        public void ReleaseIdleDBUserInfos(int ticksSlot)
        {
            long nowTicks = DateTime.Now.Ticks / 10000;
            List<string> idleUserIDList = new List<string>();
            lock (DictUserInfos)
            {
                foreach (var weakRef in DictUserInfos.Values)
                {
                    if (weakRef.IsAlive) //判断是否仍在存活
                    {
                        DBUserInfo dbUserInfo = (weakRef.Target as DBUserInfo);
                        if (null != dbUserInfo)
                        {
                            if (nowTicks - dbUserInfo.LastReferenceTicks >= ticksSlot)
                            {
                                idleUserIDList.Add(dbUserInfo.UserID);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < idleUserIDList.Count; i++)
            {
                RemoveDBUserInfo(idleUserIDList[i]);
                LogManager.WriteLog(LogTypes.Info, string.Format("释放空闲的用户数据: {0}", idleUserIDList[i]));
            }
        }
    }
}
