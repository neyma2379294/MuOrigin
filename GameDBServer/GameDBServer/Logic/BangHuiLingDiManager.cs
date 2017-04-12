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
    /// 帮会领地管理
    /// </summary>
    public class BangHuiLingDiManager
    {
        #region 基础数据

        /// <summary>
        /// 帮旗领地占领信息字典
        /// </summary>
        private Dictionary<int, BangHuiLingDiInfoData> _BangHuiLingDiItemsDict = new Dictionary<int, BangHuiLingDiInfoData>();

        #endregion 基础数据

        #region 基础方法

        /// <summary>
        /// 从数据库中获取领地占领列表
        /// </summary>
        public void LoadBangHuiLingDiItemsDictFromDB(DBManager dbMgr)
        {
            //获取领地势力分布字典
            DBQuery.QueryBHLingDiInfoDict(dbMgr, _BangHuiLingDiItemsDict);

            for (int i = (int)LingDiIDs.YanZhou; i < (int)LingDiIDs.MaxVal; i++)
            {
                if (!_BangHuiLingDiItemsDict.ContainsKey(i))
                {
                    BangHuiLingDiInfoData BangHuiLingDiInfoData = new BangHuiLingDiInfoData()
                    {
                        LingDiID = i,
                        //BHID = 0,
                        ZoneID = 0,
                        BHName = "",
                        LingDiTax = 0,
                        TakeDayID = 0,
                        TakeDayNum = 0,
                        YestodayTax = 0,
                        TaxDayID = 0,
                        TodayTax = 0,
                        TotalTax = 0,
                        //WarRequest = "",
                        //AwardFetchDay = 0,
                    };

                    _BangHuiLingDiItemsDict[BangHuiLingDiInfoData.LingDiID] = BangHuiLingDiInfoData;
                }
            }
        }

        /// <summary>
        /// 根据领地ID查找领地信息
        /// </summary>
        public BangHuiLingDiInfoData FindBangHuiLingDiByID(int lingDiID)
        {
            BangHuiLingDiInfoData BangHuiLingDiInfoData = null;
            lock (_BangHuiLingDiItemsDict)
            {
                if (!_BangHuiLingDiItemsDict.TryGetValue(lingDiID, out BangHuiLingDiInfoData))
                {
                    return null;
                }
            }

            return BangHuiLingDiInfoData;
        }

        /// <summary>
        /// 清空指定帮会的领地---->注 这儿不能情况warrequest字段，领地争夺战请求字段不能随便改
        /// </summary>
        /// <param name="bhid"></param>
        public void ClearBangHuiLingDi(int bhid)
        {
            lock (_BangHuiLingDiItemsDict)
            {
                foreach (var val in _BangHuiLingDiItemsDict.Values)
                {
                    if (val.BHID == bhid)
                    {
                        val.BHID = 0;
                        val.ZoneID = 0;
                        val.BHName = "";
                        val.LingDiTax = 0;
                        val.TakeDayID = 0;
                        val.TakeDayNum = 0;
                        val.YestodayTax = 0;
                        val.TaxDayID = 0;
                        val.TodayTax = 0;
                        val.TotalTax = 0;
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定帮会的领地的税收
        /// </summary>
        /// <param name="bhid"></param>
        public void ClearBangHuiLingDiByID(int lingDiID)
        {
            lock (_BangHuiLingDiItemsDict)
            {
                foreach (var val in _BangHuiLingDiItemsDict.Values)
                {
                    if (val.LingDiID == lingDiID)
                    {
                        val.TotalTax = 0;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定领地的所属帮会信息---->注 这儿不能情况warrequest字段，领地争夺战请求字段不能随便改
        /// </summary>
        /// <param name="bhid"></param>
        public BangHuiLingDiInfoData ClearLingDiBangHuiInfo(int lingDiID)
        {
            BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
            lock (_BangHuiLingDiItemsDict)
            {
                if (!_BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
                {
                    return null;
                }

                bangHuiLingDiInfoData.BHID = 0;
                bangHuiLingDiInfoData.ZoneID = 0;
                bangHuiLingDiInfoData.BHName = "";
                bangHuiLingDiInfoData.LingDiTax = 0;
                bangHuiLingDiInfoData.TakeDayID = 0;
                bangHuiLingDiInfoData.TakeDayNum = 0;
                bangHuiLingDiInfoData.YestodayTax = 0;
                bangHuiLingDiInfoData.TaxDayID = 0;
                bangHuiLingDiInfoData.TodayTax = 0;
                bangHuiLingDiInfoData.TotalTax = 0;
            }

            return bangHuiLingDiInfoData;
        }

        /// <summary>
        /// 添加帮会占领领地
        /// </summary>
        public BangHuiLingDiInfoData AddBangHuiLingDi(int bhid, int zoneID, string bhName, int lingDiID)
        {
            BangHuiLingDiInfoData BangHuiLingDiInfoData = null;
            lock (_BangHuiLingDiItemsDict)
            {
                if (!_BangHuiLingDiItemsDict.ContainsKey(lingDiID))
                {
                    BangHuiLingDiInfoData = new BangHuiLingDiInfoData()
                    {
                        LingDiID = lingDiID,
                        BHID = bhid,
                        ZoneID = zoneID,
                        BHName = bhName,
                        LingDiTax = 0,
                        TakeDayID = 0,
                        TakeDayNum = 0,
                        YestodayTax = 0,
                        TaxDayID = 0,
                        TodayTax = 0,
                        TotalTax = 0,
                    };

                    _BangHuiLingDiItemsDict[BangHuiLingDiInfoData.LingDiID] = BangHuiLingDiInfoData;
                }
                else
                {
                    BangHuiLingDiInfoData = _BangHuiLingDiItemsDict[lingDiID];

                    if (BangHuiLingDiInfoData.BHID != bhid) //如果是新的帮会，则清空原有的税收设置
                    {
                        BangHuiLingDiInfoData.LingDiTax = 0;
                        BangHuiLingDiInfoData.TakeDayID = 0;
                        BangHuiLingDiInfoData.TakeDayNum = 0;
                        BangHuiLingDiInfoData.YestodayTax = 0;
                        BangHuiLingDiInfoData.TaxDayID = 0;
                        BangHuiLingDiInfoData.TodayTax = 0;
                        BangHuiLingDiInfoData.TotalTax = 0;
                    }

                    BangHuiLingDiInfoData.BHID = bhid;
                    BangHuiLingDiInfoData.ZoneID = zoneID;
                    BangHuiLingDiInfoData.BHName = bhName;
                }
            }

            return BangHuiLingDiInfoData;
        }

        /// <summary>
        /// 更新领地的税率
        /// </summary>
        public BangHuiLingDiInfoData UpdateBangHuiLingDiTax(int bhid, int lingDiID, int tax)
        {
            BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
            lock (_BangHuiLingDiItemsDict)
            {
                if (!_BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
                {
                    return null;
                }

                if (bangHuiLingDiInfoData.BHID != bhid)
                {
                    return null;
                }

                bangHuiLingDiInfoData.LingDiTax = tax;
            }

            return bangHuiLingDiInfoData;
        }

        /// <summary>
        /// 更新王城领地战申请列表
        /// </summary>
        public BangHuiLingDiInfoData UpdateBangHuiLingDiWarRequest(int lingDiID, String warRequest)
        {
            BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
            lock (_BangHuiLingDiItemsDict)
            {
                if (!_BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
                {
                    return null;
                }

                bangHuiLingDiInfoData.WarRequest = warRequest;
            }

            return bangHuiLingDiInfoData;
        }

        /// <summary>
        /// 添加税收
        /// </summary>
        public BangHuiLingDiInfoData AddLingDiTaxMoney(int bhid, int lingDiID, int addMoney)
        {
            int dayID = DateTime.Now.DayOfYear;
            BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
            lock (_BangHuiLingDiItemsDict)
            {
                if (!_BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
                {
                    return null;
                }

                if (bangHuiLingDiInfoData.BHID != bhid)
                {
                    return null;
                }

                bangHuiLingDiInfoData.TotalTax += addMoney;
                if (bangHuiLingDiInfoData.TaxDayID == dayID)
                {
                    bangHuiLingDiInfoData.TodayTax += addMoney;
                }
                else
                {
                    bangHuiLingDiInfoData.YestodayTax = bangHuiLingDiInfoData.TodayTax;
                    bangHuiLingDiInfoData.TaxDayID = dayID;
                    bangHuiLingDiInfoData.TodayTax = addMoney;
                }
            }

            return bangHuiLingDiInfoData;
        }

        /// <summary>
        /// 提取税收
        /// </summary>
        public BangHuiLingDiInfoData TakeLingDiTaxMoney(int bhid, int lingDiID, int takeMoney)
        {
            int dayID = DateTime.Now.DayOfYear;
            BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
            lock (_BangHuiLingDiItemsDict)
            {
                if (!_BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
                {
                    return null;
                }

                if (bangHuiLingDiInfoData.BHID != bhid)
                {
                    return null;
                }

                if (dayID == bangHuiLingDiInfoData.TakeDayID)
                {
                    if (bangHuiLingDiInfoData.TakeDayNum >= 1)
                    {
                        return null;
                    }
                }

                if (takeMoney > bangHuiLingDiInfoData.TotalTax * 0.25)
                {
                    return null;
                }

                bangHuiLingDiInfoData.TakeDayID = dayID;
                bangHuiLingDiInfoData.TakeDayNum = 1;
                bangHuiLingDiInfoData.TotalTax = Math.Max(bangHuiLingDiInfoData.TotalTax - takeMoney, 0);
            }

            return bangHuiLingDiInfoData;
        }

        /// <summary>
        /// 提取每日奖励，这儿主要设置日标志位
        /// </summary>
        public BangHuiLingDiInfoData TakeLingDiDailyAward(int bhid, int lingDiID)
        {
            int dayID = DateTime.Now.DayOfYear;
            BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
            lock (_BangHuiLingDiItemsDict)
            {
                if (!_BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
                {
                    return null;
                }

                if (bangHuiLingDiInfoData.BHID != bhid)
                {
                    return null;
                }

                if (dayID == bangHuiLingDiInfoData.AwardFetchDay)
                {
                    return null;
                }

                bangHuiLingDiInfoData.AwardFetchDay = dayID;
            }

            return bangHuiLingDiInfoData;
        }

        /// <summary>
        /// 获取领地信息字典项的发送tcp对象
        /// </summary>
        public TCPOutPacket GetBangHuiLingDiItemsDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
        {
            Dictionary<int, BangHuiLingDiItemData> bangHuiLingDiItemDataDict = new Dictionary<int, BangHuiLingDiItemData>();
            BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
            lock (_BangHuiLingDiItemsDict)
            {
                foreach (var key in _BangHuiLingDiItemsDict.Keys)
                {
                    bangHuiLingDiInfoData = _BangHuiLingDiItemsDict[key];
                    bangHuiLingDiItemDataDict[key] = new BangHuiLingDiItemData()
                    {
                        LingDiID = bangHuiLingDiInfoData.LingDiID,
                        BHID = bangHuiLingDiInfoData.BHID,
                        ZoneID = bangHuiLingDiInfoData.ZoneID,
                        BHName = bangHuiLingDiInfoData.BHName,
                        LingDiTax = bangHuiLingDiInfoData.LingDiTax,
                        WarRequest = bangHuiLingDiInfoData.WarRequest,
                        AwardFetchDay = bangHuiLingDiInfoData.AwardFetchDay,
                    };
                }
            }

            TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiLingDiItemData>>(bangHuiLingDiItemDataDict, pool, cmdID);
            return tcpOutPacket;
        }

        /// <summary>
        /// 获取领地信息字典项的发送tcp对象
        /// </summary>
        public TCPOutPacket GetBangHuiLingDiInfosDictTCPOutPacket(TCPOutPacketPool pool, int bhid, int cmdID)
        {
            Dictionary<int, BangHuiLingDiInfoData> bangHuiLingDiInfoDataDict = new Dictionary<int, BangHuiLingDiInfoData>();
            BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
            lock (_BangHuiLingDiItemsDict)
            {
                foreach (var key in _BangHuiLingDiItemsDict.Keys)
                {
                    bangHuiLingDiInfoData = _BangHuiLingDiItemsDict[key];
                    if (bhid != bangHuiLingDiInfoData.BHID)
                    {
                        continue;
                    }

                    bangHuiLingDiInfoDataDict[key] = new BangHuiLingDiInfoData()
                    {
                        LingDiID = bangHuiLingDiInfoData.LingDiID,
                        BHID = bangHuiLingDiInfoData.BHID,
                        ZoneID = bangHuiLingDiInfoData.ZoneID,
                        BHName = bangHuiLingDiInfoData.BHName,
                        LingDiTax = bangHuiLingDiInfoData.LingDiTax,
                        TakeDayID = bangHuiLingDiInfoData.TakeDayID,
                        TakeDayNum = bangHuiLingDiInfoData.TakeDayNum,
                        YestodayTax = bangHuiLingDiInfoData.YestodayTax,
                        TaxDayID = bangHuiLingDiInfoData.TaxDayID,
                        TodayTax = bangHuiLingDiInfoData.TodayTax,
                        TotalTax = bangHuiLingDiInfoData.TotalTax,
                    };
                }
            }

            TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiLingDiInfoData>>(bangHuiLingDiInfoDataDict, pool, cmdID);
            return tcpOutPacket;
        }

        #endregion 基础方法

        #region 隔周清空扬州城的税收

        private static int WeekOfYear()
        {
            System.Globalization.GregorianCalendar gc = new System.Globalization.GregorianCalendar();
            int weekOfYear = gc.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }

        /// <summary>
        /// 周ID
        /// </summary>
        private int ThisWeekID = WeekOfYear();

        /// <summary>
        /// 最后一次判断的时间间隔
        /// </summary>
        private long LastClearYangZhouTotalTaxTicks = DateTime.Now.Ticks;

        /// <summary>
        /// 每周日凌晨清空扬州城中的税收
        /// </summary>
        public void ProcessClearYangZhouTotalTax(DBManager dbMgr)
        {
            long nowTicks = DateTime.Now.Ticks;
            if (nowTicks - LastClearYangZhouTotalTaxTicks < (60L * 1000L * 10000L))
            {
                return;
            }

            LastClearYangZhouTotalTaxTicks = nowTicks;
            int thisWeekID = WeekOfYear();
            if (thisWeekID == ThisWeekID)
            {
                return;
            }

            ThisWeekID = thisWeekID;

            //清空指定帮会的领地的税收
            ClearBangHuiLingDiByID((int)LingDiIDs.YouZhou);

            //清空某个帮会占领的领地税收
            DBWriter.ClearBHLingDiTotalTaxByID(dbMgr, (int)LingDiIDs.YouZhou);
        }

        #endregion 隔周清空扬州城的税收
    }
}
