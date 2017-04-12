using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using GameDBServer.DB;
using GameDBServer.Server;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 皇帝特权数据项
    /// </summary>
    public class HuangDiTeQuanItem
    {
        /// <summary>
        /// ID号
        /// </summary>
        public int ID = 0;

        /// <summary>
        /// 关入牢房的日ID
        /// </summary>
        public int ToLaoFangDayID = 0;

        /// <summary>
        /// 关入牢房的次数
        /// </summary>
        public int ToLaoFangNum = 0;

        /// <summary>
        /// 放出牢房的日ID
        /// </summary>
        public int OffLaoFangDayID = 0;

        /// <summary>
        /// 放出牢房的次数
        /// </summary>
        public int OffLaoFangNum = 0;

        /// <summary>
        /// 世界频道禁言的日ID
        /// </summary>
        public int BanCatDayID = 0;

        /// <summary>
        /// 世界频道禁言的次数
        /// </summary>
        public int BanCatNum = 0;

        /// <summary>
        /// 上次禁言的操作时间
        /// </summary>
        public long LastBanChatTicks = 0;

        /// <summary>
        /// 上次关入牢房的操作时间
        /// </summary>
        public long LastSendToLaoFangTicks = 0;

        /// <summary>
        /// 上次从牢房放出的操作时间
        /// </summary>
        public long LastTakeOffLaoFangTicks = 0;
    }

    /// <summary>
    /// 皇帝特权管理
    /// </summary>
    public class HuangDiTeQuanMgr
    {
        #region 基础数据

        //皇帝特权数据项
        private static HuangDiTeQuanItem MyHuangDiTeQuanItem = null;

        /// <summary>
        /// 限制对于某个角色的次数
        /// </summary>
        private static Dictionary<string, int> HuangDiToOtherRoleDict = new Dictionary<string, int>();

        #endregion 基础数据

        #region 访问接口

        /// <summary>
        /// 讲对于某角色的操作加入字典中
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="otherRoleID"></param>
        private static void AddToHuangDiToOtherRoleDict(int cmdID, int otherRoleID)
        {
            string key = string.Format("{0}_{1}", cmdID, otherRoleID);
            HuangDiToOtherRoleDict[key] = DateTime.Now.DayOfYear;
        }

        /// <summary>
        /// 查找是否已经对于某个人执行过指定的命令
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="otherRoleID"></param>
        /// <returns></returns>
        public static bool FindHuangDiToOtherRoleDict(int cmdID, int otherRoleID)
        {
            string key = string.Format("{0}_{1}", cmdID, otherRoleID);
            lock (MyHuangDiTeQuanItem)
            {
                int dayID = -1;
                if (!HuangDiToOtherRoleDict.TryGetValue(key, out dayID))
                {
                    return false;
                }

                return (dayID == DateTime.Now.DayOfYear);
            }
        }

        /// <summary>
        /// 皇帝特权数据项
        /// </summary>
        /// <param name="dbMgr"></param>
        public static void LoadHuangDiTeQuan(DBManager dbMgr)
        {
            //加载皇帝特权数据
            MyHuangDiTeQuanItem = DBQuery.LoadHuangDiTeQuan(dbMgr);
            if (null == MyHuangDiTeQuanItem)
            {
                MyHuangDiTeQuanItem = new HuangDiTeQuanItem()
                {
                    ID = 1,
                    ToLaoFangDayID = 0,
                    ToLaoFangNum = 0,
                    OffLaoFangDayID = 0,
                    OffLaoFangNum = 0,
                    BanCatDayID = 0,
                    BanCatNum = 0,
                    LastBanChatTicks = 0,
                    LastSendToLaoFangTicks = 0,
                    LastTakeOffLaoFangTicks = 0,
                };
            }
        }

        /// <summary>
        /// 获取皇帝特权数据项
        /// </summary>
        /// <returns></returns>
        public static HuangDiTeQuanItem GetHuangDiTeQuanItem()
        {
            return MyHuangDiTeQuanItem;
        }

        /// <summary>
        /// 清空现有皇帝的特权
        /// </summary>
        public static void ClearHuangDiTeQuan()
        {
            MyHuangDiTeQuanItem = new HuangDiTeQuanItem()
            {
                ID = 1,
                ToLaoFangDayID = 0,
                ToLaoFangNum = 0,
                OffLaoFangDayID = 0,
                OffLaoFangNum = 0,
                BanCatDayID = 0,
                BanCatNum = 0,
                LastBanChatTicks = 0,
                LastSendToLaoFangTicks = 0,
                LastTakeOffLaoFangTicks = 0,
            };
        }

        /// <summary>
        /// 判断现在能否执行皇帝特权
        /// </summary>
        /// <param name="cmdID"></param>
        /// <returns></returns>
        public static bool CanExecuteHuangDiTeQuanNow(int cmdID)
        {
            long nowTicks = DateTime.Now.Ticks;
            if ((int)TCPGameServerCmds.CMD_SPR_SENDTOLAOFANG == cmdID)
            {
                lock (MyHuangDiTeQuanItem)
                {
                    if (nowTicks - MyHuangDiTeQuanItem.LastSendToLaoFangTicks > (30L * 60L * 1000L * 10000L))
                    {
                        return true;
                    }
                }
            }
            else if ((int)TCPGameServerCmds.CMD_SPR_TAKEOUTLAOFANG == cmdID)
            {
                lock (MyHuangDiTeQuanItem)
                {
                    if (nowTicks - MyHuangDiTeQuanItem.LastTakeOffLaoFangTicks > (30L * 60L * 1000L * 10000L))
                    {
                        return true;
                    }
                }
            }
            else if ((int)TCPGameServerCmds.CMD_SPR_BANCHAT == cmdID)
            {
                lock (MyHuangDiTeQuanItem)
                {
                    if (nowTicks - MyHuangDiTeQuanItem.LastBanChatTicks > (30L * 60L * 1000L * 10000L))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 增加皇帝特权项
        /// </summary>
        /// <param name="cmdID"></param>
        /// <param name="addNum"></param>
        /// <returns></returns>
        public static bool AddHuanDiTeQuan(int cmdID, int otherRoleID)
        {
            long nowTicks = DateTime.Now.Ticks;
            int dayID = DateTime.Now.DayOfYear;
            if ((int)TCPGameServerCmds.CMD_SPR_SENDTOLAOFANG == cmdID)
            {
                lock (MyHuangDiTeQuanItem)
                {
                    if (MyHuangDiTeQuanItem.ToLaoFangDayID == dayID)
                    {
                        if (MyHuangDiTeQuanItem.ToLaoFangNum >= (int)HuanDiTeQuanNum.Max)
                        {
                            return false;
                        }

                        MyHuangDiTeQuanItem.ToLaoFangNum += 1;
                    }
                    else
                    {
                        MyHuangDiTeQuanItem.ToLaoFangDayID = dayID;
                        MyHuangDiTeQuanItem.ToLaoFangNum = 1;
                    }

                    MyHuangDiTeQuanItem.LastSendToLaoFangTicks = nowTicks;

                    //讲对于某角色的操作加入字典中
                    AddToHuangDiToOtherRoleDict(cmdID, otherRoleID);

                    return true;
                }
            }
            else if ((int)TCPGameServerCmds.CMD_SPR_TAKEOUTLAOFANG == cmdID)
            {
                lock (MyHuangDiTeQuanItem)
                {
                    if (MyHuangDiTeQuanItem.OffLaoFangDayID == dayID)
                    {
                        if (MyHuangDiTeQuanItem.OffLaoFangNum >= (int)HuanDiTeQuanNum.Max)
                        {
                            return false;
                        }

                        MyHuangDiTeQuanItem.OffLaoFangNum += 1;
                    }
                    else
                    {
                        MyHuangDiTeQuanItem.OffLaoFangDayID = dayID;
                        MyHuangDiTeQuanItem.OffLaoFangNum = 1;
                    }

                    MyHuangDiTeQuanItem.LastTakeOffLaoFangTicks = nowTicks;

                    //讲对于某角色的操作加入字典中
                    AddToHuangDiToOtherRoleDict(cmdID, otherRoleID);

                    return true;
                }
            }
            else if ((int)TCPGameServerCmds.CMD_SPR_BANCHAT == cmdID)
            {
                lock (MyHuangDiTeQuanItem)
                {
                    if (MyHuangDiTeQuanItem.BanCatDayID == dayID)
                    {
                        if (MyHuangDiTeQuanItem.BanCatNum >= (int)HuanDiTeQuanNum.Max)
                        {
                            return false;
                        }

                        MyHuangDiTeQuanItem.BanCatNum += 1;
                    }
                    else
                    {
                        MyHuangDiTeQuanItem.BanCatDayID = dayID;
                        MyHuangDiTeQuanItem.BanCatNum = 1;
                    }

                    MyHuangDiTeQuanItem.LastBanChatTicks = nowTicks;

                    //讲对于某角色的操作加入字典中
                    AddToHuangDiToOtherRoleDict(cmdID, otherRoleID);

                    return true;
                }
            }

            return false;
        }

        #endregion 访问接口
    }
}
