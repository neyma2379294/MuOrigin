using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 用户重置管理类
    /// </summary>
    public class UserMoneyMgr
    {
        #region 获取用户充值，同步到角色表中

        /// <summary>
        /// 上次扫描更新用户重置的元宝数据的时间
        /// </summary>
        private static long LastUpdateUserMoneyTicks = 0;

        /// <summary>
        /// 更新用户充值的元宝数据
        /// </summary>
        public static void UpdateUsersMoney(DBManager dbMgr)
        {
            long nowTicks = DateTime.Now.Ticks / 10000;
            if (nowTicks - LastUpdateUserMoneyTicks < (3 * 1000))
            {
                return;
            }

            LastUpdateUserMoneyTicks = nowTicks;

            List<string> userIDList = new List<string>();
            List<int> addUserMoneyList = new List<int>();
            DBQuery.QueryTempMoney(dbMgr, userIDList, addUserMoneyList);
            if (userIDList.Count <= 0)
            {
                return;
            }

            int currentGiftID = GameDBManager.GameConfigMgr.GetGameConfigItemInt("big_award_id", 0);
            int moneyToYuanBao = GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10);
            int moneyToJiFen = GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-to-jifen", 1);

            for (int i = 0; i < userIDList.Count; i++)
            {
                string userID = userIDList[i];
                int addUserMoney = addUserMoneyList[i];

                DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
                if (null != dbUserInfo)
                {
                    int userMoney = 0;
                    int realMoney = 0;
                    int giftID = 0;
                    int giftJiFen = 0;

                    lock (dbUserInfo)
                    {
                        dbUserInfo.Money += (addUserMoney * moneyToYuanBao); //一分钱??元宝
                        userMoney = dbUserInfo.Money;

                        dbUserInfo.RealMoney += addUserMoney;
                        realMoney = dbUserInfo.RealMoney;

                        if (currentGiftID != dbUserInfo.GiftID)
                        {
                            dbUserInfo.GiftJiFen = 0;
                            dbUserInfo.GiftID = currentGiftID;
                        }

                        giftID = dbUserInfo.GiftID;

                        if (dbUserInfo.GiftID > 0)
                        {
                            dbUserInfo.GiftJiFen += (addUserMoney * moneyToJiFen); //一分钱??积分
                        }

                        giftJiFen = dbUserInfo.GiftJiFen;
                    }

                    //更新用户元宝表
                    DBWriter.UpdateUserMoney2(dbMgr, dbUserInfo.UserID, userMoney, realMoney, giftID, giftJiFen);
                    string resoult = "1";
                    int rid = DBQuery.LastLoginRole(dbMgr, dbUserInfo.UserID);
                    //送绑钻
                    CFirstChargeMgr.SendToRolebindgold(dbMgr, dbUserInfo.UserID, rid, addUserMoney);
                    //添加GM命令消息
                    string gmCmdData = string.Format("-updateyb {0} {1} {2} {3}", dbUserInfo.UserID,rid,addUserMoney,resoult);
                    ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
                }
            }
        }

        #endregion 获取用户充值，同步到角色表中

        #region 将充值的流水写入DB文件中

        /// <summary>
        /// 上次扫描充值流水的时间
        /// </summary>
        private static long LastScanInputLogTicks = DateTime.Now.Ticks / 10000;

        /// <summary>
        /// 上次扫描充值流水的ID
        /// </summary>
        private static int LastScanID = -1;

        /// <summary>
        /// 扫描充值流水生成二进制日志
        /// </summary>
        public static void ScanInputLogToDBLog(DBManager dbMgr)
        {
            //if (!GameDBManager.DBEventsWriter.Enable)
            //{
            //    return;
            //}

            long nowTicks = DateTime.Now.Ticks / 10000;
            if (nowTicks - LastScanInputLogTicks < (30 * 1000))
            {
                return;
            }

            LastScanInputLogTicks = nowTicks;

            //上次扫描充值流水的ID
            if (LastScanID < 0)
            {
                LastScanID = DBQuery.QueryLastScanInputLogID(dbMgr);
            }

            //查询充值记录，并且写入日志中
            int newLastScanID = DBQuery.ScanInputLogFromTable(dbMgr, LastScanID);
            if (newLastScanID != LastScanID)
            {
                LastScanID = newLastScanID;
                DBWriter.UpdateLastScanInputLogID(dbMgr, LastScanID);
            }
        }

        #endregion 将充值的流水写入DB文件中
    }
}
