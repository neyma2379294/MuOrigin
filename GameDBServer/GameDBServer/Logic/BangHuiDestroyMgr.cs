using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 解散帮会管理
    /// </summary>
    public class BangHuiDestroyMgr
    {
        #region 时间校验数据

        /// <summary>
        /// 上次检测解散帮会的日期
        /// </summary>
        private static int LastCheckDestroyDayID = DateTime.Now.DayOfYear;

        /// <summary>
        /// 上次检测解散的的时间
        /// </summary>
        private static int LastCheckDestroyTimer = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

        /// <summary>
        /// 进行解散的时间
        /// </summary>
        private static int DestroyTimer = 1;

        #endregion 时间校验数据

        /// <summary>
        /// 处理解散帮会
        /// </summary>
        /// <param name="dbMgr"></param>
        public static void ProcessDestroyBangHui(DBManager dbMgr)
        {
            DateTime dateTime = DateTime.Now;
            int dayID = dateTime.DayOfYear;
            DayOfWeek dayOfWeek = dateTime.DayOfWeek;
            int nowTimer = dateTime.Hour * 60 + dateTime.Minute;

            if (dayID != LastCheckDestroyDayID)
            {
                LastCheckDestroyDayID = dayID;
                LastCheckDestroyTimer = nowTimer;
                return;
            }

            // 星期天的0点1分进行战盟维护费用扣除
            if (nowTimer >= DestroyTimer && LastCheckDestroyTimer < DestroyTimer && dayOfWeek == DayOfWeek.Sunday)
            {
                LastCheckDestroyDayID = dayID;
                LastCheckDestroyTimer = nowTimer;

                //处理解散帮会
                HandleDestroyBangHuis(dbMgr);

                return;
            }
        }

        /// <summary>
        /// 处理解散帮会
        /// </summary>
        /// <param name="dbMgr"></param>
        private static void HandleDestroyBangHuis(DBManager dbMgr)
        {
            // 临时注释掉战盟解散代码 [5/11/2014 LiaoWei]
            int moneyPerLevel = GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-per-qilevel", 10000);

            //执行扣除战盟维护资金
            DBWriter.SubBangHuiTongQianByQiLevel(dbMgr, moneyPerLevel);

            //获取帮会列表数据
            List<int> noMoneyBangHuiList = DBQuery.GetNoMoneyBangHuiList(dbMgr);

            for (int i = 0; i < noMoneyBangHuiList.Count; i++)
            {
                int bhid = noMoneyBangHuiList[i];

                //执行删除帮会的操作
                BangHuiDestroyMgr.DoDestroyBangHui(dbMgr, bhid);

                //添加GM命令消息
                string gmCmdData = string.Format("-autodestroybh {0}", bhid);
                ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
            }
        }

        /// <summary>
        /// 执行删除帮会的操作
        /// </summary>
        /// <param name="dbMgr"></param>
        public static void DoDestroyBangHui(DBManager dbMgr, int bhid)
        {
            lock (Global.BangHuiMutex)
            {
                //删除帮会信息
                //根据帮会ID删除帮会
                DBWriter.DeleteBangHui(dbMgr, bhid);

                //删除内存字典中的军旗
                GameDBManager.BangHuiJunQiMgr.RemoveBangHuiJunQi(bhid);

                //清空所有指定帮会用户的帮会信息
                DBWriter.ClearAllRoleBangHuiInfo(dbMgr, bhid);

                List<DBRoleInfo> dbRoleInfoList = dbMgr.DBRoleMgr.GetCachingDBRoleInfoListByFaction(bhid);
                if (null != dbRoleInfoList)
                {
                    for (int i = 0; i < dbRoleInfoList.Count; i++)
                    {
                        dbRoleInfoList[i].Faction = 0;
                        dbRoleInfoList[i].BHName = "";
                        dbRoleInfoList[i].BHZhiWu = 0;
                        //dbRoleInfoList[i].BGDayID1 = 0;
                        //dbRoleInfoList[i].BGMoney = 0;
                        //dbRoleInfoList[i].BGDayID2 = 0;
                        //dbRoleInfoList[i].BGGoods = 0;
                        dbRoleInfoList[i].BangGong = 0;
                    }
                }
            }

            //清空某个帮会占领的领地列表
            DBWriter.ClearBHLingDiByID(dbMgr, bhid);

            //清空指定帮会的领地
            GameDBManager.BangHuiLingDiMgr.ClearBangHuiLingDi(bhid);

            //清空战盟事件
            ZhanMengShiJianManager.getInstance().onZhanMengJieSan(bhid);
        }
    }
}
