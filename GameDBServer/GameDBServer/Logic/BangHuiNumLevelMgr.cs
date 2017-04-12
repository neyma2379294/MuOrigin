using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 帮会人数和级别项
    /// </summary>
    public class BangHuiNumLevelItem
    {
        /// <summary>
        /// 帮会ID
        /// </summary>
        public int BHID = 0;

        /// <summary>
        /// 帮会总的人数
        /// </summary>
        public int TotalNum = 0;

        /// <summary>
        /// 帮会总的级别
        /// </summary>
        public int TotalLevel = 0;

        // MU 新增 [12/28/2013 LiaoWei]
        /// <summary>
        /// 帮会成员总战斗力
        /// </summary>
        public int TotalCombatForce = 0;
    }

    /// <summary>
    /// 帮会的人数和总的级别管理
    /// </summary>
    public class BangHuiNumLevelMgr
    {
        /// <summary>
        /// 最小查询时间间隔
        /// </summary>
        private static long MaxQueryTimeSlotTicks = (5L * 60L * 1000L * 10000L);

        /// <summary>
        /// 最后一次查询的时间
        /// </summary>
        private static long LastQueryTimeTicks = 0;

        /// <summary>
        /// 重新计算帮会的人数
        /// </summary>
        /// <param name="dbMgr"></param>
        public static void RecalcBangHuiNumLevel(DBManager dbMgr)
        {
            long ticks = DateTime.Now.Ticks;
            if (ticks - LastQueryTimeTicks < MaxQueryTimeSlotTicks)
            {
                return; //禁止频繁操作
            }

            LastQueryTimeTicks = ticks;

            //获取帮会列表数据
            BangHuiListData bangHuiListData = DBQuery.GetBangHuiItemDataList(dbMgr, -1, 0, 10000);
            if (null != bangHuiListData && null != bangHuiListData.BangHuiItemDataList)
            {
                List<BangHuiNumLevelItem> bangHuiNumLevelItemList = new List<BangHuiNumLevelItem>();
                for (int i = 0; i < bangHuiListData.BangHuiItemDataList.Count; i++)
                {
                    bangHuiNumLevelItemList.Add(new BangHuiNumLevelItem()
                    {
                        BHID = bangHuiListData.BangHuiItemDataList[i].BHID,
                        TotalNum = DBQuery.QueryBHMemberNum(dbMgr, bangHuiListData.BangHuiItemDataList[i].BHID),
                        TotalLevel = DBQuery.QueryBHMemberLevel(dbMgr, bangHuiListData.BangHuiItemDataList[i].BHID),
                        TotalCombatForce = DBQuery.QueryBHMemberTotalCombatForce(dbMgr, bangHuiListData.BangHuiItemDataList[i].BHID),
                    });
                }

                for (int i = 0; i < bangHuiNumLevelItemList.Count; i++)
                {
                    //更新帮会的人员个数和级别总和
                    DBWriter.UpdateBangHuiNumLevel(dbMgr, bangHuiNumLevelItemList[i].BHID, bangHuiNumLevelItemList[i].TotalNum,
                        bangHuiNumLevelItemList[i].TotalLevel, bangHuiNumLevelItemList[i].TotalCombatForce);
                }
            }
        }
    }
}
