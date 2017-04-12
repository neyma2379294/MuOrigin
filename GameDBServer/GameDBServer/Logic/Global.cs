using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
//using System.Windows.Resources;
using System.Windows;
//using System.Windows.Media.Animation;
using System.Threading;
using Server.Data;
using Server.TCP;
using Server.Protocol;
using Server.Tools;
using System.Net;
using System.Net.Sockets;
using GameDBServer.DB;
using System.Text.RegularExpressions;
using GameDBServer.Server;
using System.Diagnostics;
using System.IO;

namespace GameDBServer.Logic
{
    class Global
    {
        static Global()
        {
        }

        #region XML操作相关

        /// <summary>
        /// 获取指定的xml节点的节点路径
        /// </summary>
        /// <param name="element"></param>
        public static string GetXElementNodePath(XElement element)
        {
            try
            {
                string path = element.Name.ToString();
                element = element.Parent;
                while (null != element)
                {
                    path = element.Name.ToString() + "/" + path;
                    element = element.Parent;
                }

                return path;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取XML文件树节点段XElement
        /// </summary>
        /// <param name="XML">XML文件载体</param>
        /// <param name="newroot">要查找的独立节点</param>
        /// <returns>独立节点XElement</returns>
        public static XElement GetXElement(XElement XML, string newroot)
        {
            return XML.DescendantsAndSelf(newroot).Single();
        }

        /// <summary>
        /// 获取XML文件树节点段XElement
        /// </summary>
        /// <param name="XML">XML文件载体</param>
        /// <param name="newroot">要查找的独立节点</param>
        /// <returns>独立节点XElement</returns>
        public static XElement GetSafeXElement(XElement XML, string newroot)
        {
            try
            {
                return XML.DescendantsAndSelf(newroot).Single();
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取: {0} 失败, xml节点名: {1}", newroot, GetXElementNodePath(XML)));
            }
        }

        /// <summary>
        /// 获取XML文件树节点段XElement
        /// </summary>
        /// <param name="xml">XML文件载体</param>
        /// <param name="mainnode">要查找的主节点</param>
        /// <param name="attribute">主节点条件属性名</param>
        /// <param name="value">主节点条件属性值</param>
        /// <returns>以该主节点为根的XElement</returns>
        public static XElement GetXElement(XElement XML, string newroot, string attribute, string value)
        {
            return XML.DescendantsAndSelf(newroot).Single(X => X.Attribute(attribute).Value == value);
        }

        /// <summary>
        /// 获取XML文件树节点段XElement
        /// </summary>
        /// <param name="xml">XML文件载体</param>
        /// <param name="mainnode">要查找的主节点</param>
        /// <param name="attribute">主节点条件属性名</param>
        /// <param name="value">主节点条件属性值</param>
        /// <returns>以该主节点为根的XElement</returns>
        public static XElement GetSafeXElement(XElement XML, string newroot, string attribute, string value)
        {
            try
            {
                return XML.DescendantsAndSelf(newroot).Single(X => X.Attribute(attribute).Value == value);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取: {0}/{1}={2} 失败, xml节点名: {3}", newroot, attribute, value, GetXElementNodePath(XML)));
            }
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="XML"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static XAttribute GetSafeAttribute(XElement XML, string attribute)
        {
            try
            {
                XAttribute attrib = XML.Attribute(attribute);
                if (null == attrib)
                {
                    throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, GetXElementNodePath(XML)));
                }

                return attrib;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, GetXElementNodePath(XML)));
            }
        }

        /// <summary>
        /// 获取属性值(字符串)
        /// </summary>
        /// <param name="XML"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string GetSafeAttributeStr(XElement XML, string attribute)
        {
            XAttribute attrib = GetSafeAttribute(XML, attribute);
            return (string)attrib;
        }

        /// <summary>
        /// 获取属性值(整型)
        /// </summary>
        /// <param name="XML"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static long GetSafeAttributeLong(XElement XML, string attribute)
        {
            XAttribute attrib = GetSafeAttribute(XML, attribute);
            string str = (string)attrib;
            if (null == str || str == "") return -1;

            try
            {
                return (long)Convert.ToDouble(str);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, GetXElementNodePath(XML)));
            }
        }

        /// <summary>
        /// 获取属性值(浮点)
        /// </summary>
        /// <param name="XML"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static double GetSafeAttributeDouble(XElement XML, string attribute)
        {
            XAttribute attrib = GetSafeAttribute(XML, attribute);
            string str = (string)attrib;
            if (null == str || str == "") return 0.0;

            try
            {
                return Convert.ToDouble(str);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, GetXElementNodePath(XML)));
            }
        }

        /// <summary>
        /// 取得xml的属性值
        /// </summary>
        /// <param name="XML"></param>
        /// <param name="root"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static XAttribute GetSafeAttribute(XElement XML, string root, string attribute)
        {
            try
            {
                XAttribute attrib = XML.Element(root).Attribute(attribute);
                if (null == attrib)
                {
                    throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, GetXElementNodePath(XML)));
                }

                return attrib;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, GetXElementNodePath(XML)));
            }
        }

        /// <summary>
        /// 取得xml的属性值(字符串)
        /// </summary>
        /// <param name="XML"></param>
        /// <param name="root"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string GetSafeAttributeStr(XElement XML, string root, string attribute)
        {
            XAttribute attrib = GetSafeAttribute(XML, root, attribute);
            return (string)attrib;
        }

        /// <summary>
        /// 取得xml的属性值(整型值)
        /// </summary>
        /// <param name="XML"></param>
        /// <param name="root"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static long GetSafeAttributeLong(XElement XML, string root, string attribute)
        {
            XAttribute attrib = GetSafeAttribute(XML, root, attribute);
            string str = (string)attrib;
            if (null == str || str == "") return -1;

            try
            {
                return (long)Convert.ToDouble(str);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, GetXElementNodePath(XML)));
            }
        }

        /// <summary>
        /// 取得xml的属性值(浮点型)
        /// </summary>
        /// <param name="XML"></param>
        /// <param name="root"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static double GetSafeAttributeDouble(XElement XML, string root, string attribute)
        {
            XAttribute attrib = GetSafeAttribute(XML, root, attribute);
            string str = (string)attrib;
            if (null == str || str == "") return -1;

            try
            {
                return Convert.ToDouble(str);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, GetXElementNodePath(XML)));
            }
        }

        #endregion //XML操作相关

        #region 数据转换

        /// <summary>
        /// 数据库角色信息到用户数据的转换
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="roleDataEx"></param>
        public static void DBRoleInfo2RoleDataEx(DBRoleInfo dbRoleInfo, RoleDataEx roleDataEx)
        {
            //将用户的请求更新内存缓存
            lock (dbRoleInfo)
            {
                roleDataEx.RoleID = dbRoleInfo.RoleID;
                roleDataEx.RoleName = dbRoleInfo.RoleName;
                roleDataEx.RoleSex = dbRoleInfo.RoleSex;
                roleDataEx.Occupation = dbRoleInfo.Occupation;
                roleDataEx.Level = dbRoleInfo.Level;
                roleDataEx.Faction = dbRoleInfo.Faction;
                roleDataEx.Money1 = dbRoleInfo.Money1;
                roleDataEx.Money2 = dbRoleInfo.Money2;
                roleDataEx.Experience = dbRoleInfo.Experience;
                roleDataEx.PKMode = dbRoleInfo.PKMode;
                roleDataEx.PKValue = dbRoleInfo.PKValue;

                string[] fileds = dbRoleInfo.Position.Split(':');
                if (fileds.Length == 4)
                {
                    roleDataEx.MapCode = Convert.ToInt32(fileds[0]);
                    roleDataEx.RoleDirection = Convert.ToInt32(fileds[1]);
                    roleDataEx.PosX = Convert.ToInt32(fileds[2]);
                    roleDataEx.PosY = Convert.ToInt32(fileds[3]);
                }

                roleDataEx.LifeV = 0;
                roleDataEx.MagicV = 0;

                roleDataEx.OldTasks = dbRoleInfo.OldTasks;
                roleDataEx.TaskDataList = dbRoleInfo.DoingTaskList;
                roleDataEx.RolePic = dbRoleInfo.RolePic;
                roleDataEx.BagNum = dbRoleInfo.BagNum;
                roleDataEx.TaskDataList = dbRoleInfo.DoingTaskList;
                roleDataEx.GoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 0);
                roleDataEx.OtherName = dbRoleInfo.OtherName;
                roleDataEx.MainQuickBarKeys = dbRoleInfo.MainQuickBarKeys;
                roleDataEx.OtherQuickBarKeys = dbRoleInfo.OtherQuickBarKeys;
                roleDataEx.LoginNum = dbRoleInfo.LoginNum;
                roleDataEx.LeftFightSeconds = dbRoleInfo.LeftFightSeconds;
                roleDataEx.FriendDataList = dbRoleInfo.FriendDataList;
                roleDataEx.HorsesDataList = dbRoleInfo.HorsesDataList;
                roleDataEx.HorseDbID = dbRoleInfo.HorseDbID;
                roleDataEx.PetsDataList = dbRoleInfo.PetsDataList;
                roleDataEx.PetDbID = dbRoleInfo.PetDbID;
                roleDataEx.InterPower = dbRoleInfo.InterPower;
                roleDataEx.JingMaiDataList = dbRoleInfo.JingMaiDataList;
                roleDataEx.DJPoint = (null != dbRoleInfo.RoleDJPointData) ? dbRoleInfo.RoleDJPointData.DJPoint : 0;
                roleDataEx.DJTotal = (null != dbRoleInfo.RoleDJPointData) ? dbRoleInfo.RoleDJPointData.Total : 0;
                roleDataEx.DJWincnt = (null != dbRoleInfo.RoleDJPointData) ? dbRoleInfo.RoleDJPointData.Wincnt : 0;
                roleDataEx.TotalOnlineSecs = Math.Max(0, dbRoleInfo.TotalOnlineSecs); //2011-05-31, 以前的会出现负数？兼容错误
                roleDataEx.AntiAddictionSecs = Math.Max(0, dbRoleInfo.AntiAddictionSecs); //2011-05-31, 以前的会出现负数？兼容错误
                roleDataEx.BiGuanTime = dbRoleInfo.BiGuanTime;
                roleDataEx.YinLiang = dbRoleInfo.YinLiang;
                roleDataEx.SkillDataList = dbRoleInfo.SkillDataList;
                roleDataEx.TotalJingMaiExp = dbRoleInfo.TotalJingMaiExp;
                roleDataEx.JingMaiExpNum = dbRoleInfo.JingMaiExpNum;
                roleDataEx.RegTime = DataHelper.ConvertToTicks(dbRoleInfo.RegTime);
                roleDataEx.LastHorseID = dbRoleInfo.LastHorseID;
                roleDataEx.SaleGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, (int)SaleGoodsConsts.SaleGoodsID);
                roleDataEx.DefaultSkillID = dbRoleInfo.DefaultSkillID;
                roleDataEx.AutoLifeV = dbRoleInfo.AutoLifeV;
                roleDataEx.AutoMagicV = dbRoleInfo.AutoMagicV;
                roleDataEx.BufferDataList = dbRoleInfo.BufferDataList;
                roleDataEx.MyDailyTaskDataList = dbRoleInfo.MyDailyTaskDataList;
                roleDataEx.MyDailyJingMaiData = dbRoleInfo.MyDailyJingMaiData;
                roleDataEx.NumSkillID = dbRoleInfo.NumSkillID;
                roleDataEx.MyPortableBagData = dbRoleInfo.MyPortableBagData;
                roleDataEx.MyHuodongData = dbRoleInfo.MyHuodongData;
                roleDataEx.FuBenDataList = dbRoleInfo.FuBenDataList;
                roleDataEx.MainTaskID = dbRoleInfo.MainTaskID;
                roleDataEx.PKPoint = dbRoleInfo.PKPoint;
                roleDataEx.LianZhan = dbRoleInfo.LianZhan;
                roleDataEx.MyRoleDailyData = dbRoleInfo.MyRoleDailyData;
                roleDataEx.KillBoss = dbRoleInfo.KillBoss;
                roleDataEx.MyYaBiaoData = dbRoleInfo.MyYaBiaoData;
                roleDataEx.BattleNameStart = dbRoleInfo.BattleNameStart;
                roleDataEx.BattleNameIndex = dbRoleInfo.BattleNameIndex;
                roleDataEx.CZTaskID = dbRoleInfo.CZTaskID;
                roleDataEx.BattleNum = dbRoleInfo.BattleNum;
                roleDataEx.HeroIndex = dbRoleInfo.HeroIndex;
                roleDataEx.ZoneID = dbRoleInfo.ZoneID;
                roleDataEx.BHName = dbRoleInfo.BHName;
                roleDataEx.BHVerify = dbRoleInfo.BHVerify;
                roleDataEx.BHZhiWu = dbRoleInfo.BHZhiWu;
                roleDataEx.BGDayID1 = dbRoleInfo.BGDayID1;
                roleDataEx.BGMoney = dbRoleInfo.BGMoney;
                roleDataEx.BGDayID2 = dbRoleInfo.BGDayID2;
                roleDataEx.BGGoods = dbRoleInfo.BGGoods;
                roleDataEx.BangGong = dbRoleInfo.BangGong;
                roleDataEx.HuangHou = dbRoleInfo.HuangHou;
                roleDataEx.PaiHangPosDict = PaiHangManager.CalcPaiHangPosDictRoleID(dbRoleInfo.RoleID);
                roleDataEx.JieBiaoDayID = dbRoleInfo.JieBiaoDayID;
                roleDataEx.JieBiaoDayNum = dbRoleInfo.JieBiaoDayNum;
                roleDataEx.VipDailyDataList = dbRoleInfo.VipDailyDataList;
                roleDataEx.YangGongBKDailyJiFen = dbRoleInfo.YangGongBKDailyJiFen;
                roleDataEx.LastMailID = dbRoleInfo.LastMailID;
                roleDataEx.OnceAwardFlag = dbRoleInfo.OnceAwardFlag;
                roleDataEx.Gold = dbRoleInfo.Gold;
                roleDataEx.BanChat = dbRoleInfo.BanChat;
                roleDataEx.BanLogin = dbRoleInfo.BanLogin;
                roleDataEx.IsFlashPlayer = dbRoleInfo.IsFlashPlayer;
                roleDataEx.ChangeLifeCount = dbRoleInfo.ChangeLifeCount;
                roleDataEx.AdmiredCount = dbRoleInfo.AdmiredCount;
                roleDataEx.CombatForce = dbRoleInfo.CombatForce;
                roleDataEx.AutoAssignPropertyPoint = dbRoleInfo.AutoAssignPropertyPoint;

                roleDataEx.GoodsLimitDataList = dbRoleInfo.GoodsLimitDataList;
                roleDataEx.RoleParamsDict = dbRoleInfo.RoleParamsDict;

                //计算移动仓库中的物品个数
                roleDataEx.MyPortableBagData.GoodsUsedGridNum = Global.GetGoodsDataCountBySite(dbRoleInfo, (int)SaleGoodsConsts.PortableGoodsID);

                long ticks = DateTime.Now.Ticks / 10000;

                int restartSecs = GameDBManager.GameConfigMgr.GetGameConfigItemInt("anti-addiction-restart", 18000);
                if (((ticks - dbRoleInfo.LogOffTime) / 1000) >= restartSecs) //如果离线没有超过最大配置时间，则启用重新计算防止沉迷时间
                {
                    roleDataEx.AntiAddictionSecs = 0;
                }

                dbRoleInfo.UpdateDBPositionTicks = ticks; //强行设置为当前
                dbRoleInfo.UpdateDBTimeTicks = ticks;
                dbRoleInfo.UpdateDBInterPowerTimeTicks = ticks;

                roleDataEx.MyWingData = dbRoleInfo.MyWingData;

                roleDataEx.RolePictureJudgeReferInfo = dbRoleInfo.PictureJudgeReferInfo;
                roleDataEx.RoleStarConstellationInfo = dbRoleInfo.StarConstellationInfo;
                roleDataEx.VIPLevel = GetRoleParamsInt32(dbRoleInfo, "VIPExp");
                roleDataEx.ElementhrtsList = Global.GetGoodsDataListBySite(dbRoleInfo, (int)SaleGoodsConsts.ElementhrtsGoodsID);
                roleDataEx.UsingElementhrtsList = Global.GetGoodsDataListBySite(dbRoleInfo, (int)SaleGoodsConsts.UsingElementhrtsGoodsID);
            }
        }

        /// <summary>
        /// 数据库角色信息到选择用户数据的转换
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="roleDataEx"></param>
        public static void DBRoleInfo2RoleData4Selector(DBRoleInfo dbRoleInfo, RoleData4Selector roleData4Selector)
        {
            //将用户的请求更新内存缓存
            lock (dbRoleInfo)
            {
                roleData4Selector.RoleID = dbRoleInfo.RoleID;
                roleData4Selector.RoleName = dbRoleInfo.RoleName;
                roleData4Selector.RoleSex = dbRoleInfo.RoleSex;
                roleData4Selector.Occupation = dbRoleInfo.Occupation;
                roleData4Selector.Level = dbRoleInfo.Level;
                roleData4Selector.Faction = dbRoleInfo.Faction;
                roleData4Selector.MyWingData = dbRoleInfo.MyWingData;

                roleData4Selector.GoodsDataList = Global.GetUsingGoodsDataList(dbRoleInfo);
                roleData4Selector.OtherName = dbRoleInfo.OtherName;
                roleData4Selector.CombatForce = dbRoleInfo.CombatForce;
                roleData4Selector.AdmiredCount = dbRoleInfo.AdmiredCount;
            }
        }

        /// <summary>
        /// 将LineItem转换为LineData
        /// </summary>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        public static LineData LineItemToLineData(LineItem lineItem)
        {
            LineData lineData = new LineData()
            {
                LineID = lineItem.LineID,
                GameServerIP = lineItem.GameServerIP,
                GameServerPort = lineItem.GameServerPort,
                OnlineCount = lineItem.OnlineCount,
            };

            return lineData;
        }

        /// <summary>
        /// 安全的字符串到整型的转换
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int SafeConvertToInt32(string str)
        {
            str = str.Trim();
            if (string.IsNullOrEmpty(str)) return 0;

            try
            {
                return Convert.ToInt32(str);
            }
            catch (Exception)
            {
            }

            return 0;
        }

        /// <summary>
        /// 返回服务器时间相对于相对于"2011-11-11"经过了多少天
        /// 可以避免使用DayOfYear产生的跨年问题
        /// </summary>
        /// <returns></returns>
        public static int GetOffsetDay(DateTime now)
        {
            TimeSpan ts = now - DateTime.Parse("2011-11-11");
            // 经过的毫秒数
            double temp = ts.TotalMilliseconds;
            int day = (int)(temp / 1000 / 60 / 60 / 24);
            return day;
        }

        public static string GetDayStartTime(DateTime now)
        {
            return Global.GetDateTimeString(now.Date);
        }

        public static string GetDayEndTime(DateTime now)
        {
            return Global.GetDateTimeString(now.Date.AddDays(1));
        }

        /// <summary>
        /// 获取时间的标准字符串表示
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static string GetDateTimeString(DateTime now)
        {
            return now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 使用服务器时间相对于相对于"2011-11-11"经过了多少天 来返回具体的日期
        /// 可以避免使用DayOfYear产生的跨年问题
        /// </summary>
        /// <returns></returns>
        public static DateTime GetRealDate(int day)
        {
            DateTime startDay = DateTime.Parse("2011-11-11");
            return Global.GetAddDaysDataTime(startDay, day);
        }

        #endregion 数据转换

        #region 调试辅助

        /// <summary>
        /// 获取Socket的远端IP地址
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetSocketRemoteEndPoint(Socket s)
        {
            try
            {
                return string.Format("{0} ", s.RemoteEndPoint);
            }
            catch (Exception)
            {
            }

            return "";
        }

        public static string GetDebugHelperInfo(Socket socket)
        {
            if (null == socket)
            {
                return "socket为null, 无法打印错误信息";
            }

            string ret = "";
            try
            {
                ret += string.Format("IP={0} ", GetSocketRemoteEndPoint(socket));
            }
            catch (Exception)
            {
            }

            return ret;
        }

        #endregion 调试辅助

        #region 物品管理

        /// <summary>
        /// 根据位置获取物品列表
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public static List<GoodsData> GetGoodsDataListBySite(DBRoleInfo dbRoleInfo, int site)
        {
            List<GoodsData> goodsDataList = new List<GoodsData>();
            if (null == dbRoleInfo.GoodsDataList) return goodsDataList;

            for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
            {
                if (dbRoleInfo.GoodsDataList[i].Site == site)
                {
                    goodsDataList.Add(dbRoleInfo.GoodsDataList[i]);
                }
            }

            return goodsDataList;
        }

        /// <summary>
        /// 根据位置获取物品列表个数
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public static int GetGoodsDataCountBySite(DBRoleInfo dbRoleInfo, int site)
        {
            if (null == dbRoleInfo.GoodsDataList) return 0;

            int count = 0;
            for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
            {
                if (dbRoleInfo.GoodsDataList[i].Site == site)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 根据物品的DBID获取物品
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public static GoodsData GetGoodsDataByDbID(DBRoleInfo dbRoleInfo, int goodsDbID)
        {
            lock (dbRoleInfo)
            {
                if (null != dbRoleInfo.GoodsDataList)
                {
                    for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
                    {
                        if (dbRoleInfo.GoodsDataList[i].Id == goodsDbID)
                        {
                            return dbRoleInfo.GoodsDataList[i];
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 根据是否使用获取物品列表
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public static List<GoodsData> GetUsingGoodsDataList(DBRoleInfo dbRoleInfo)
        {
            List<GoodsData> usingGoodsDataList = new List<GoodsData>();

            if (null == dbRoleInfo.GoodsDataList) return usingGoodsDataList;

            for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
            {
                if (dbRoleInfo.GoodsDataList[i].Using > 0)
                {
                    usingGoodsDataList.Add(dbRoleInfo.GoodsDataList[i]);
                }
            }

            return usingGoodsDataList;
        }

        #endregion 物品管理

        #region 物品使用限制管理

        /// <summary>
        /// 物品限制列表中加入指定的物品
        /// </summary>
        /// <param name="client"></param>
        /// <param name="goodsID"></param>
        /// <returns></returns>
        public static void UpdateGoodsLimitByID(DBRoleInfo dbRoleInfo, int goodsID, int dayID, int usedNum)
        {
            lock (dbRoleInfo)
            {
                if (dbRoleInfo.GoodsLimitDataList == null)
                {
                    dbRoleInfo.GoodsLimitDataList = new List<GoodsLimitData>();
                }

                int findIndex = -1;
                for (int i = 0; i < dbRoleInfo.GoodsLimitDataList.Count; i++)
                {
                    if (dbRoleInfo.GoodsLimitDataList[i].GoodsID == goodsID)
                    {
                        findIndex = i;

                        dbRoleInfo.GoodsLimitDataList[i].DayID = dayID;
                        dbRoleInfo.GoodsLimitDataList[i].UsedNum = usedNum;

                        break;
                    }
                }

                if (-1 == findIndex)
                {
                    GoodsLimitData goodsLimitData  = new GoodsLimitData()
                    {
                        GoodsID = goodsID,
                        DayID = dayID,
                        UsedNum = usedNum,
                    };

                    dbRoleInfo.GoodsLimitDataList.Add(goodsLimitData);
                }
            }
        }

        #endregion 物品使用限制管理

        #region 坐骑和宠物

        /// <summary>
        /// 通过数据库ID定位坐骑数据
        /// </summary>
        /// <param name="dbID"></param>
        /// <returns></returns>
        public static HorseData GetHorseDataByDbID(DBRoleInfo dbRoleInfo, int dbID)
        {
            if (null == dbRoleInfo) return null;
            lock (dbRoleInfo)
            {
                if (null != dbRoleInfo.HorsesDataList)
                {
                    for (int i = 0; i < dbRoleInfo.HorsesDataList.Count; i++)
                    {
                        if (dbRoleInfo.HorsesDataList[i].DbID == dbID)
                        {
                            return dbRoleInfo.HorsesDataList[i];
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 通过数据库ID定位宠物数据
        /// </summary>
        /// <param name="dbID"></param>
        /// <returns></returns>
        public static PetData GetPetDataByDbID(DBRoleInfo dbRoleInfo, int dbID)
        {
            if (null == dbRoleInfo) return null;
            lock (dbRoleInfo)
            {
                if (null != dbRoleInfo.PetsDataList)
                {
                    for (int i = 0; i < dbRoleInfo.PetsDataList.Count; i++)
                    {
                        if (dbRoleInfo.PetsDataList[i].DbID == dbID)
                        {
                            return dbRoleInfo.PetsDataList[i];
                        }
                    }
                }
            }

            return null;
        }

        #endregion 坐骑和宠物

        #region 点将积分管理

        /// <summary>
        /// 获取角色积分的复制
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <returns></returns>
        public static DJPointData GetRoleDJPointData(DBRoleInfo dbRoleInfo)
        {
            DJPointData djPointData = null;
            lock (dbRoleInfo)
            {
                djPointData = new DJPointData()
                {
                    DbID = dbRoleInfo.RoleDJPointData.DbID,
                    RoleID = dbRoleInfo.RoleDJPointData.RoleID,
                    DJPoint = dbRoleInfo.RoleDJPointData.DJPoint,
                    Total = dbRoleInfo.RoleDJPointData.Total,
                    Wincnt = dbRoleInfo.RoleDJPointData.Wincnt,
                    Yestoday = dbRoleInfo.RoleDJPointData.Yestoday,
                    Lastweek = dbRoleInfo.RoleDJPointData.Lastweek,
                    Lastmonth = dbRoleInfo.RoleDJPointData.Lastmonth,
                    Dayupdown = dbRoleInfo.RoleDJPointData.Dayupdown,
                    Weekupdown = dbRoleInfo.RoleDJPointData.Weekupdown,
                    Monthupdown = dbRoleInfo.RoleDJPointData.Monthupdown,
                };
            }

            return djPointData;
        }

        #endregion 点将积分管理

        #region 在线状态管理

        /// <summary>
        /// 获取一个角色是否在线
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <returns></returns>
        public static int GetRoleOnlineState(DBRoleInfo dbRoleInfo)
        {
            if (null == dbRoleInfo) return 0;
            if (dbRoleInfo.ServerLineID <= 0) return 0;
            return 1;
        }

        /// <summary>
        /// 获取一个账号是否在线
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <returns></returns>
        public static int GetUserOnlineState(DBUserInfo dbUserInfo)
        {
            if (null == dbUserInfo) return 0;
            return UserOnlineManager.GetUserOnlineState(dbUserInfo.UserID);
        }

        #endregion 在线状态管理

        #region 构造角色名称

        /// <summary>
        /// 格式化角色名称
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string FormatRoleName(DBRoleInfo dbRoleInfo)
        {
            return FormatRoleName(dbRoleInfo.ZoneID, dbRoleInfo.RoleName);
        }

        /// <summary>
        /// 格式化角色名称
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string FormatRoleName(string zoneID, string roleName)
        {
            //return string.Format(Global.GetLang("[{0}区]{1}"), zoneID, roleName);
            return roleName;
        }

        /// <summary>
        /// 格式化角色名称
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string FormatRoleName(int zoneID, string roleName)
        {
            //return string.Format(Global.GetLang("[{0}区]{1}"), zoneID, roleName);
            return roleName;
        }

        #endregion 构造角色名称

        #region 格式化帮会名称

        /// <summary>
        /// 格式化帮会名称
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string FormatBangHuiName(int zoneID, string bhName)
        {
            //return string.Format(Global.GetLang("[{0}区]{1}"), zoneID, bhName);
            return bhName;
        }

        #endregion 格式化帮会名称

        #region 帮派管理

        /// <summary>
        /// 帮派逻辑锁
        /// </summary>
        public static object BangHuiMutex = new object();

        /// <summary>
        /// 从管理角色列表中根据职务查找
        /// </summary>
        /// <param name="dbRoleInfoList"></param>
        /// <param name="zhiWu"></param>
        /// <returns></returns>
        public static int GetDBRoleInfoByZhiWu(List<BangHuiMgrItemData> bangHuiMgrItemDataList, int zhiWu)
        {
            if (null == bangHuiMgrItemDataList || bangHuiMgrItemDataList.Count <= 0) return 0;
            for (int i = 0; i < bangHuiMgrItemDataList.Count; i++)
            {
                if (bangHuiMgrItemDataList[i].BHZhiwu == zhiWu)
                {
                    return bangHuiMgrItemDataList[i].RoleID;
                }
            }

            return 0;
        }

        #endregion 帮派管理

        #region 角色DB日志提交

        /// <summary>
        /// 将角色的信息写入二进制日志
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        public static void WriteRoleInfoLog(DBManager dbMgr, DBRoleInfo dbRoleInfo)
        {
            try
            {
                //if (!GameDBManager.DBEventsWriter.Enable)
                //{
                //    return;
                //}

                if (null == dbRoleInfo)
                {
                    return;
                }

                int userMoney = 0;
                string userID = dbRoleInfo.UserID;

                //将用户的请求更新内存缓存
                DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
                if (null != dbUserInfo)
                {
                    userMoney = dbUserInfo.Money;
                }

                int mapCode = 0;
                //int posX = 0;
                //int poxY = 0;
                if (!string.IsNullOrEmpty(dbRoleInfo.Position))
                {
                    string[] fileds = dbRoleInfo.Position.Split(':');
                    if (fileds.Length == 4)
                    {
                        mapCode = Convert.ToInt32(fileds[0]);
                        //posX = Convert.ToInt32(fileds[2]);
                        //poxY = Convert.ToInt32(fileds[3]);
                    }
                }

                string lastTime = (new DateTime(dbRoleInfo.LastTime * 10000)).ToString("yyyy-MM-dd HH:mm:ss");
                string logOffTime = (new DateTime(dbRoleInfo.LogOffTime * 10000)).ToString("yyyy-MM-dd HH:mm:ss");
                /*GameDBManager.DBEventsWriter.CacheRoles(-1,
                    dbRoleInfo.ZoneID,
                    dbRoleInfo.RoleID,
                    dbRoleInfo.UserID,
                    dbRoleInfo.RoleName,
                    dbRoleInfo.RoleSex,
                    dbRoleInfo.Occupation,
                    dbRoleInfo.Level,
                    dbRoleInfo.Money1,
                    dbRoleInfo.RegTime,
                    lastTime,
                    dbRoleInfo.LoginNum,
                    dbRoleInfo.TotalOnlineSecs,
                    logOffTime,
                    dbRoleInfo.YinLiang,
                    userMoney,
                    0,
                    dbRoleInfo.LoginDayNum,
                    mapCode,
                    "",
                    dbRoleInfo.UserName,
                    dbRoleInfo.MainTaskID);*/
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("将角色的数据写入二进制日志出错，RoleID={0}",
                    dbRoleInfo.RoleID));
            }
        }

        #endregion 角色DB日志提交

        #region 从语言包中获取语言

        /// <summary>
        /// 快速存取语言字符串的字典
        /// </summary>
        private static Dictionary<string, string> LangDict = null;

        /// <summary>
        /// 从程序资源中加载语言包字典
        /// </summary>
        /// <returns></returns>
        public static void LoadLangDict()
        {
            XElement xml = null;

            try
            {
                xml = XElement.Load("Language.xml");
            }
            catch (Exception)
            {
                return;
            }

            try
            {
                if (null == xml) return;
                Dictionary<string, string> langDict = new Dictionary<string, string>();
                IEnumerable<XElement> langItems = xml.Elements();
                foreach (var langItem in langItems)
                {
                    langDict[Global.GetSafeAttributeStr(langItem, "ChineseText")] = Global.GetSafeAttributeStr(langItem, "OtherLangText");
                }

                LangDict = langDict;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 将中文转成其他语言
        /// </summary>
        /// <param name="chineseText"></param>
        /// <returns></returns>
        public static string GetLang(string chineseText)
        {
            if (null == LangDict)
            {
                return chineseText;
            }

            string otherLangText = "";
            if (!LangDict.TryGetValue(chineseText, out otherLangText))
            {
                return chineseText;
            }

            if (string.IsNullOrEmpty(otherLangText))
            {
                return chineseText;
            }

            return otherLangText;
        }

        #endregion 从语言包中获取语言

        #region 邮件管理
        #region 提取邮件列表
        /// <summary>
        /// 提取用户的邮件列表【不包括附件数据和邮件内容数据,用于给客户端显示邮件列表用】
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="mailData"></param>
        /// <returns></returns>
        public static List<MailData> LoadUserMailItemDataList(DBManager dbMgr, int rid)
        {
            //先设置新邮件标志位0【没有新邮件】
            DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(rid);
            if (null != dbRoleInfo)
            {
                dbRoleInfo.LastMailID = 0;
            }

            return DBQuery.GetMailItemDataList(dbMgr, rid);
        }

        #endregion 返回邮件列表

        #region 获取邮件数量

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="rid"></param>
        /// <param name="excludeReadState">排除邮件读取状态(默认排除已读的)</param>
        /// <returns></returns>
        public static int LoadUserMailItemDataCount(DBManager dbMgr, int rid, int excludeReadState = 0, int limitCount = 1)
        {
            return DBQuery.GetMailItemDataCount(dbMgr, rid, excludeReadState, limitCount);
        }

        #endregion 获取邮件数量

        #region 提取单个邮件内容
        /// <summary>
        /// 提取用户的单个邮件数据【包括附件数据和邮件内容数据】
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="mailData"></param>
        /// <returns></returns>
        public static MailData LoadMailItemData(DBManager dbMgr, int rid, int mailID)
        {
            MailData mailData = DBQuery.GetMailItemData(dbMgr, rid, mailID);
            if (null != mailData)
            {
                if (mailData.IsRead != 1)
                {
                    //设置已读标志
                    DBWriter.UpdateMailHasReadFlag(dbMgr, mailID, rid);

                    mailData.IsRead = 1;
                    mailData.ReadTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            return mailData;
        }

        #endregion 提取单个邮件内容

        #region 设置邮件已读标志
        /// <summary>
        /// 设置邮件已读标志
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="mailData"></param>
        /// <returns></returns>
        public static bool UpdateHasReadMailFlag(DBManager dbMgr, int rid, int mailID)
        {
            bool ret = true;

            //设置邮件附件已提取标志
            DBWriter.UpdateMailHasReadFlag(dbMgr, mailID, rid);

            return ret;
        }

        #endregion 设置邮件已读标志

        #region 更新邮件附件提取标志
        /// <summary>
        /// 设置邮件附件已提取标志【设置为以提取】
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="mailData"></param>
        /// <returns></returns>
        public static bool UpdateHasFetchMailGoodsStat(DBManager dbMgr, int rid, int mailID)
        {
            bool ret = false;

            //设置邮件附件已提取标志
            ret = DBWriter.UpdateMailHasFetchGoodsFlag(dbMgr, mailID, rid);
            DBWriter.UpdateMailHasReadFlag(dbMgr, mailID, rid);
            //为了让用户下次能看到邮件附件信息，这些别删除
            //删除附件【没有附件的也可以调用】
            //DBWriter.DeleteMailGoodsList(dbMgr, mailData.MailID);

            //将邮件附件的银两和铜钱设置0

            return ret;
        }

        #endregion 更新邮件附件提取标志

        #region 删除邮件
        /// <summary>
        /// 删除邮件,包括附件
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="mailData"></param>
        /// <returns></returns>
        public static bool DeleteMail(DBManager dbMgr, int rid, string mailIDs)
        {
            bool ret = false, result = false;

            string[] mailidArr = mailIDs.Split('|');

            foreach (var strID in mailidArr)
            {
                try
                {
                    int mailID = int.Parse(strID);
                    //删除邮件实体【只要邮件实体被成功删除，就算删除成功】
                    ret = DBWriter.DeleteMailDataItemExcludeGoodsList(dbMgr, mailID, rid);
                    if (ret)
                    {
                        //删除邮件临时表中的相关项
                        DBWriter.DeleteMailIDInMailTemp(dbMgr, mailID);
                        //删除附件【没有附件的也可以调用】
                        DBWriter.DeleteMailGoodsList(dbMgr, mailID);

                        //有一个成功就都成功
                        result = ret;
                    }
                }
                catch
                {
                }
            }

            return result;
        }

        #endregion 删除邮件

        #region 发送新邮件
        /// <summary>
        /// 添加邮件,返回邮件ID
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="mailData"></param>
        /// <returns></returns>
        public static int AddMail(DBManager dbMgr, string[] fields, out int addGoodsCount)
        {
            //int senderrid, string senderrname, int receiverrid, string reveiverrname, string subject, string content, int yinliang, int tongqian, int yuanbao;
            //邮件主体部分
            int senderrid = Convert.ToInt32(fields[0]);
            string senderrname = fields[1];
            int receiverrid = Convert.ToInt32(fields[2]);
            string reveiverrname = fields[3];
            string subject = fields[4];
            string content = fields[5];
            int yinliang = Convert.ToInt32(fields[6]);
            int tongqian = Convert.ToInt32(fields[7]);
            int yuanbao = Convert.ToInt32(fields[8]);

            //邮件物品列表
            string goodslist = fields[9];

            if (reveiverrname == "")
            {
                string uid = "";
                Global.GetRoleNameAndUserID(dbMgr, receiverrid, out reveiverrname, out uid);
            }

            //传输协议中使用 $ 代替 :,避免协议出错，这儿还原
            senderrname = senderrname.Replace('$', ':');
            reveiverrname = reveiverrname.Replace('$', ':');
            subject = subject.Replace('$', ':');
            content = content.Replace('$', ':');

            int mailID = -1;

            addGoodsCount = 0;
            //根据接收角色名称定位接收角色ID【不用定位，gameserver已经进行了有效验证，并设置了正确的值】
            //receiverrid = Global.FindDBRoleID(dbMgr, reveiverrname);            
            //if (receiverrid < 0)
            //{
            //    mailID = -2000;
            //    return mailID;
            //}

            DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(senderrid);
            if (null != dbRoleInfo)
            {
                senderrname = Global.FormatRoleName(dbRoleInfo);
            }

            //添加邮件实体
            mailID = DBWriter.AddMailBody(dbMgr, senderrid, senderrname, receiverrid, reveiverrname, subject, content, yinliang, tongqian, yuanbao);

            //添加邮件成功
            if (mailID >= 0)
            {
                //每一个mail goods item 都是 goodsid_forge_level_quality_Props_gcount_origholenum_rmbholenum_jewellist_addpropindex_binding 多个用竖线隔开
                //添加邮件附件
                addGoodsCount = Global.AddMailGoods(dbMgr, mailID, goodslist.Split('|'));
                //更新邮件临时表，扫描用
                DBWriter.UpdateLastScanMailID(dbMgr, receiverrid, mailID);
            }

            return mailID;
        }

        /// <summary>
        /// 添加邮件附件物品部分,返回成功添加的物品数量
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="mailData"></param>
        /// <returns></returns>
        private static int AddMailGoods(DBManager dbMgr, int mailid, string[] goodsArr)
        {
            //每一个mail goods item 都是 goodsid_forge_level_quality_Props_gcount_origholenum_rmbholenum_jewellist_addpropindex_binding

            if (null == goodsArr || goodsArr.Length <= 0)
            {
                return 0;
            }

            int addCount = 0;
            string[] goods = null;

            for (int n = 0; n < goodsArr.Length; n++)
            {
                goods = goodsArr[n].Split('_');
                if (16 != goods.Length)
                {
                    continue;
                }

                if (DBWriter.AddMailGoodsDataItem(dbMgr, mailid, int.Parse(goods[0]), int.Parse(goods[1]), int.Parse(goods[2]), goods[3], int.Parse(goods[4]), int.Parse(goods[5]),
                    int.Parse(goods[6]), goods[7], int.Parse(goods[8]), int.Parse(goods[9]), int.Parse(goods[10]), int.Parse(goods[11]), int.Parse(goods[12]), int.Parse(goods[13]), int.Parse(goods[14]), int.Parse(goods[15]))) // 卓越信息 [12/14/2013 LiaoWei]
                {
                    addCount++;
                }
            }

            return addCount;
        }

        #endregion 发送新邮件
        #endregion 邮件管理

        #region 角色名称
        /// <summary>
        /// 通过角色名称定位用户ID【先从缓存中查询，如果不存在，直接查数据库】 失败返回负数
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static int FindDBRoleID(DBManager dbMgr, string roleName)
        {
            //先从缓存中查询
            int roleID = dbMgr.DBRoleMgr.FindDBRoleID(roleName);

            if (roleID < 0)
            {
                //直接从数据库查询时，必须解析角色名称，将roleName解析为区号+真的角色名称
                //没有查到，再从数据库直接查询
                try
                {
                    string roleRealName = "";
                    int zoneID = -1;

                    GetRoleRealNameAndZoneID(roleName, out roleRealName, out zoneID);

                    //0区存在吗？应该不存在，保留这样写，反正最多多查询一次
                    if (zoneID >= 0)
                    {
                        roleID = DBQuery.GetRoleIDByRoleName(dbMgr, roleRealName, zoneID);
                    }
                }
                catch
                {
                    //查询不到会返回负的roleID
                }
            }
            return roleID;
        }

        /// <summary>
        /// 从 [10区]宇文一闪 这样的名字中解析出区号 和 名字,主要解析格式 [{0}中文]{1} 中的 0 和 1 ,对所有语言适用
        /// </summary>
        /// <param name="inRoleName"></param>
        /// <param name="outRoleName"></param>
        /// <param name="zoneID"></param>
        public static void GetRoleRealNameAndZoneID(String inRoleName, out String outRoleName, out int zoneID)
        {
            outRoleName = "";
            zoneID = -1;

            //没有找到这两个字符中的任何一个就不是合法的名字,这两个[必须是第一个，]必须不是第一个字符
            if (inRoleName.IndexOf('[') != 0 || inRoleName.IndexOf(']') < 1)
            {
                return;
            }

            //去掉左边[再正则匹配
            String tmpStr = inRoleName.Substring(1);

            Regex r = new Regex("^[\\d]+"); //定义一个Regex对象实例,匹配刚刚开始的数字
            MatchCollection mc = r.Matches(tmpStr);
            if (mc.Count > 0) //在输入字符串中找到第一个匹配
            {
                zoneID = int.Parse(mc[0].Value); //将匹配的第一个字符串转换为zoneid
            }

            int pos = inRoleName.IndexOf(']') + 1;
            outRoleName = inRoleName.Substring(pos);
        }
        #endregion 角色名称

        #region 货币转换
        /// <summary>
        /// 将money转换为元宝，采用相应的转换比例
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static int TransMoneyToYuanBao(int money)
        {
            int moneyToYuanBao = GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10);

            int yuanBao = money * moneyToYuanBao;

            return yuanBao;
        }

        #endregion 货币转换

        #region 活动奖励
        /// <summary>
        /// 根据活动的开始时间和结束时间生成一个活动关键字
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static string GetHuoDongKeyString(string fromDate, string toDate)
        {
            return string.Format("{0}_{1}", fromDate, toDate);
        }

        /// <summary>
        /// 是否在活动期间内
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static bool IsInActivityPeriod(string fromDate, string toDate)
        {
            try
            {
                if (DateTime.Now >= DateTime.Parse(fromDate) &&
                    DateTime.Now <= DateTime.Parse(toDate))
                {
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// 返回玩家的最大等级角色的名称
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static bool GetUserMaxLevelRole(DBManager dbMgr, string userID, out string maxLevelRoleName, out int maxLevelRoleZoneID)
        {
            maxLevelRoleName = "";
            maxLevelRoleZoneID = 1;

            DBUserInfo userInfo = dbMgr.GetDBUserInfo(userID);

            if (null != userInfo)
            {
                int maxLevel = -1, pos = -1;
                int maxchangelife = -1;
                for (int i = 0; i < userInfo.ListRoleChangeLifeCount.Count; i++)
                {
                    if (userInfo.ListRoleChangeLifeCount[i] > maxchangelife)
                    {
                        maxchangelife = userInfo.ListRoleChangeLifeCount[i];
                        maxLevel = userInfo.ListRoleLevels[i];
                        pos=i;
                    }else  if (userInfo.ListRoleChangeLifeCount[i] == maxchangelife)
                    {
                        if(maxLevel<userInfo.ListRoleLevels[i])
                        {
                            maxLevel = userInfo.ListRoleLevels[i];
                            pos=i;
                        }
                            
                    }

                }
                
                if (pos >= 0 && pos < userInfo.ListRoleNames.Count)
                {
                    maxLevelRoleName = userInfo.ListRoleNames[pos];
                   
                }

                if (pos >= 0 && pos < userInfo.ListRoleZoneIDs.Count)
                {
                    maxLevelRoleZoneID = userInfo.ListRoleZoneIDs[pos];
                }
            }

            return true;
        }


        /// <summary>
        /// 返回玩家的角色的名称和UserID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static bool GetRoleNameAndUserID(DBManager dbMgr, int rid, out string maxLevelRoleName, out string userID)
        {
            maxLevelRoleName = "";
            userID = "";

            DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(rid);

            if (null != roleInfo)
            {
                maxLevelRoleName = Global.FormatRoleName(roleInfo);
                userID = roleInfo.UserID;
            }

            return true;
        }

        /// <summary>
        /// 根据活动限制条件返回活动排行信息,依次为第一名，第二名.... 可能没有第一名或者中间的某些名次
        ///minGateValueList存放排名最低数值要求列表，依次为第一名的最小值，第二名的最小值......
        /// </summary>
        /// <param name="minGateValueList"></param>
        /// <returns></returns>
        public static List<HuoDongPaiHangData> GetPaiHangItemListByHuoDongLimit(DBManager dbMgr, List<int> minGateValueList, int activityType, string midDate, int maxPaiHang = 10)
        {
            //返回排行信息【这个排行信息是真实的排行信息，没有处理排行值限制，活动奖励的时候，需要根据配置限制信息动态调整排行】
            List<HuoDongPaiHangData> listPaiHangReal = DBQuery.GetActivityPaiHangListNearMidTime(dbMgr, activityType, midDate, maxPaiHang);

            List<HuoDongPaiHangData> listPaiHang = new List<HuoDongPaiHangData>();

            //上一个玩家的排行 0 表示上个玩家没有排行
            int preUserPaiHang = 0;

            //重整排行表
            for (int n = 0; n < listPaiHangReal.Count; n++)
            {
                HuoDongPaiHangData phData = listPaiHangReal[n];
                phData.PaiHang = -1;

                //不满足该排行位置需要的最低数值要求，则依次降低排名
                for (int i = preUserPaiHang; i < minGateValueList.Count; i++)
                {
                    if (phData.PaiHangValue >= minGateValueList[i])
                    {
                        phData.PaiHang = i + 1;
                        listPaiHang.Add(phData);

                        preUserPaiHang = phData.PaiHang;

                        break;
                    }
                }

                //这个靠前的排行数据没有满足排行值限制的最小要求，则其后的排行数据也满足不了
                if (phData.PaiHang < 0 || phData.PaiHang >= minGateValueList.Count)
                {
                    break;
                }
            }

            return listPaiHang;
        }

        /// <summary>
        /// 返回充值王排行数据列表
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="minGateValueList"></param>
        /// <param name="maxPaiHang"></param>
        /// <returns></returns>
        public static List<InputKingPaiHangData> GetInputKingPaiHangListByHuoDongLimit(DBManager dbMgr, string fromDate, string toDate, List<int> minGateValueList, int maxPaiHang = 3)
        {
            //返回排行信息
            List<InputKingPaiHangData> listPaiHangReal = DBQuery.GetUserInputPaiHang(dbMgr, fromDate, toDate, maxPaiHang);

            List<InputKingPaiHangData> listPaiHang = new List<InputKingPaiHangData>();

            //上一个玩家的排行 0 表示上个玩家没有排行
            int preUserPaiHang = 0;

            //重整排行表
            for (int n = 0; n < listPaiHangReal.Count; n++)
            {
                InputKingPaiHangData phData = listPaiHangReal[n];
                phData.PaiHang = -1;

                //重置排行值，将其转换为元宝数量
                phData.PaiHangValue = Global.TransMoneyToYuanBao(phData.PaiHangValue);

                //不满足该排行位置需要的最低数值要求，则依次降低排名
                for (int i = preUserPaiHang; i < minGateValueList.Count; i++)
                {
                    if (phData.PaiHangValue >= minGateValueList[i])
                    {
                        phData.PaiHang = i + 1;
                        listPaiHang.Add(phData);

                        preUserPaiHang = phData.PaiHang;

                        break;
                    }
                }

                //这个靠前的排行数据没有满足排行值限制的最小要求，则其后的排行数据也满足不了
                if (phData.PaiHang < 0 || phData.PaiHang >= minGateValueList.Count)
                {
                    break;
                }
            }

            return listPaiHang;
        }

        /// <summary>
        /// 返回消费王排行数据列表
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="minGateValueList"></param>
        /// <param name="maxPaiHang"></param>
        /// <returns></returns>
        public static List<InputKingPaiHangData> GetUsedMoneyKingPaiHangListByHuoDongLimit(DBManager dbMgr, string fromDate, string toDate, List<int> minGateValueList, int maxPaiHang = 3)
        {
            //返回排行信息
            List<InputKingPaiHangData> listPaiHangReal = DBQuery.GetUserUsedMoneyPaiHang(dbMgr, fromDate, toDate, maxPaiHang);

            List<InputKingPaiHangData> listPaiHang = new List<InputKingPaiHangData>();

            //上一个玩家的排行 0 表示上个玩家没有排行
            int preUserPaiHang = 0;

            //重整排行表
            for (int n = 0; n < listPaiHangReal.Count; n++)
            {
                InputKingPaiHangData phData = listPaiHangReal[n];
                phData.PaiHang = -1;

                //不满足该排行位置需要的最低数值要求，则依次降低排名
                for (int i = preUserPaiHang; i < minGateValueList.Count; i++)
                {
                    if (phData.PaiHangValue >= minGateValueList[i])
                    {
                        phData.PaiHang = i + 1;
                        listPaiHang.Add(phData);

                        preUserPaiHang = phData.PaiHang;

                        break;
                    }
                }

                //这个靠前的排行数据没有满足排行值限制的最小要求，则其后的排行数据也满足不了
                if (phData.PaiHang < 0 || phData.PaiHang >= minGateValueList.Count)
                {
                    break;
                }
            }

            return listPaiHang;
        }

        /// <summary>
        /// 处理 除去 充值王 之外的 其它 王类活动，比如冲级王，装备王，坐骑王，经脉王等
        /// </summary>
        /// <param name="cmdData"></param>
        /// <param name="activityType"></param>
        /// <returns></returns>
        public static TCPProcessCmdResults ProcessHuoDongForKing(DBManager dbMgr, TCPOutPacketPool pool, int nID, string[] fields, int activityType, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;

            int roleID = Convert.ToInt32(fields[0]);
            string fromDate = fields[1].Replace('$', ':');
            string toDate = fields[2].Replace('$', ':');
            //排名最低级别要求列表，依次为第一名的最小级别，第二名的最小级别......
            string[] minGateValueArr = fields[3].Split('_');

            List<int> minGateValueList = new List<int>();
            foreach (var item in minGateValueArr)
            {
                minGateValueList.Add(Global.SafeConvertToInt32(item));
            }

            string strcmd = "";

            DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(roleID);
            if (null == roleInfo)
            {
                strcmd = string.Format("{0}:{1}:0", -1001, roleID);
                tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                return TCPProcessCmdResults.RESULT_DATA;
            }

            string paiHangDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //如果今天活动结束，就取活动结束时间作为排行读取时间点,活动介绍之前能否领取，由配置文件配置，gameserver在领取时会进行限制
            if (!Global.IsInActivityPeriod(fromDate, toDate))
            {
                paiHangDate = toDate;
            }

            //返回排行信息【这个排行信息了处理排行值限制，活动奖励的时候，需要根据配置限制信息动态调整排行】
            List<HuoDongPaiHangData> listPaiHang = Global.GetPaiHangItemListByHuoDongLimit(dbMgr, minGateValueList, activityType, paiHangDate);
            
            //排行值
            int paiHang = -1;

            for (int n = 0; n < listPaiHang.Count; n++)
            {
                if (null != listPaiHang[n] && roleInfo.RoleID == listPaiHang[n].RoleID)
                {
                    paiHang = listPaiHang[n].PaiHang;//这个值和 n+1 是不一样的，因为listPaiHang中的数据可能有排行空缺
                    break;
                }
            }

            //判断是否在排行内，不可能
            if (paiHang <= 0)
            {
                strcmd = string.Format("{0}:{1}:0", -10007, roleID);
                tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                return TCPProcessCmdResults.RESULT_DATA;
            }

            //活动关键字，能够唯一标志某内活动的一个实例【在某段时间内的活动】
            string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);

            int hasgettimes = 0;
            string lastgettime = "";

            //避免同一角色同时多次操作
            lock (roleInfo)
            {
                //判断是否领取过---活动关键字字符串唯一确定了每次活动，针对同类型不同时间的活动
                DBQuery.GetAwardHistoryForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, activityType, huoDongKeyStr, out hasgettimes, out lastgettime);

                //这个活动每次每个用户最多领取一次
                if (hasgettimes > 0)
                {
                    strcmd = string.Format("{0}:{1}:0", -10005, roleID);
                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                    return TCPProcessCmdResults.RESULT_DATA;
                }

                //更新已领取状态
                int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, activityType, huoDongKeyStr, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                if (ret < 0)
                {
                    strcmd = string.Format("{0}:{1}:0", -1008, roleID);
                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                    return TCPProcessCmdResults.RESULT_DATA;
                }
            }

            //发送命令给gameserver 由gameserver 统一奖励
            strcmd = string.Format("{0}:{1}:{2}", 1, roleID, paiHang);

            tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
            return TCPProcessCmdResults.RESULT_DATA;
        }

        /// <summary>
        /// 处理 除去 充值王 之外的 其它 王类活动，比如冲级王，装备王，坐骑王，经脉王等
        /// </summary>
        /// <param name="cmdData"></param>
        /// <param name="activityType"></param>
        /// <returns></returns>
        public static TCPProcessCmdResults GetHuoDongPaiHangForKing(DBManager dbMgr, TCPOutPacketPool pool, int nID, string[] fields, int activityType, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;

            int roleID = Convert.ToInt32(fields[0]);
            string fromDate = fields[1].Replace('$', ':');
            string toDate = fields[2].Replace('$', ':');
            //排名最低级别要求列表，依次为第一名的最小级别，第二名的最小级别......
            string[] minGateValueArr = fields[3].Split('_');

            List<int> minGateValueList = new List<int>();
            foreach (var item in minGateValueArr)
            {
                minGateValueList.Add(Global.SafeConvertToInt32(item));
            }

            string paiHangDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //如果今天活动结束，就取活动结束时间作为排行读取时间点
            if (!Global.IsInActivityPeriod(fromDate, toDate))
            {
                paiHangDate = toDate;
            }

            //返回排行信息【这个排行信息了处理排行值限制，在活动奖励的时候，需要根据配置限制信息动态调整排行】
            List<HuoDongPaiHangData> listPaiHang = Global.GetPaiHangItemListByHuoDongLimit(dbMgr, minGateValueList, activityType, paiHangDate);

            //生成排行信息的tcp对象流
            tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<HuoDongPaiHangData>>(listPaiHang, pool, nID);

            return TCPProcessCmdResults.RESULT_DATA;
        }

        #endregion 活动奖励

        #region 初始化数据库自增长字段

        public static void LogAndExitProcess(string error)
        {
            File.AppendAllText("error.log", "flag_t_roles_auto_increment 未设置\r\n");
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// 初始化数据库自增长字段
        /// </summary>
        /// <returns></returns>
        public static bool InitDBAutoIncrementValues(DBManager dbManger)
        {
            int baseValue = GameDBManager.ZoneID * GameDBManager.DBAutoIncreaseStepValue;

            if (baseValue < 0)
            {
                return false;
            }

            //更新角色表自增长值
            int dbMaxValue = DBQuery.GetMaxRoleID(dbManger) + 1;
            int ret1 = DBWriter.ChangeTablesAutoIncrementValue(dbManger, "t_roles", Math.Max(baseValue, dbMaxValue));

            //更新邮件表自增长值
            dbMaxValue = DBQuery.GetMaxMailID(dbManger) + 1;
            int ret2 = DBWriter.ChangeTablesAutoIncrementValue(dbManger, "t_mail", Math.Max(baseValue, dbMaxValue));

            //更新帮会表自增长值
            dbMaxValue = DBQuery.GetMaxBangHuiID(dbManger) + 1;
            int ret3 = DBWriter.ChangeTablesAutoIncrementValue(dbManger, "t_banghui", Math.Max(baseValue, dbMaxValue));

            if (0 != ret1)
            {
                System.Console.WriteLine("更新数据库表t_roles自增长字段出错");
            }

            if (0 != ret2)
            {
                System.Console.WriteLine("更新数据库表t_mail自增长字段出错");
            }

            if (0 != ret3)
            {
                System.Console.WriteLine("更新数据库表t_banghui自增长字段出错");
            }

            if (0 != ret1 || 0 != ret2 || 0 != ret3)
            {
                return false;
            }

            return true;

        }

        #endregion 初始化数据库自增长字段

        #region 角色参数管理

        /// <summary>
        /// 更新角色参数
        /// </summary>
        /// <param name="client"></param>
        /// <param name="goodsID"></param>
        /// <returns></returns>
        public static void UpdateRoleParamByName(DBRoleInfo dbRoleInfo, string name, string value)
        {
            lock (dbRoleInfo)
            {
                if (dbRoleInfo.RoleParamsDict == null)
                {
                    dbRoleInfo.RoleParamsDict = new Dictionary<string, RoleParamsData>();
                }

                RoleParamsData roleParamsData = null;
                if (!dbRoleInfo.RoleParamsDict.TryGetValue(name, out roleParamsData))
                {
                    roleParamsData = new RoleParamsData()
                    {
                        ParamName = name,
                        ParamValue = value,
                    };

                    dbRoleInfo.RoleParamsDict[name] = roleParamsData;
                }
                else
                {
                    roleParamsData.ParamValue = value;
                }
            }
        }

        /// <summary>
        /// 获取角色参数
        /// </summary>
        /// <param name="client"></param>
        /// <param name="goodsID"></param>
        /// <returns></returns>
        public static string GetRoleParamByName(DBRoleInfo dbRoleInfo, string name)
        {
            lock (dbRoleInfo)
            {
                if (dbRoleInfo.RoleParamsDict == null)
                {

                    dbRoleInfo.RoleParamsDict = new Dictionary<string, RoleParamsData>();
                }

                RoleParamsData roleParamsData = null;
                if (dbRoleInfo.RoleParamsDict.TryGetValue(name, out roleParamsData))
                {
                    return roleParamsData.ParamValue;
                }
            }

            return null;
        }

        /// <summary>
        /// 以 整数形式 返回角色的参数数据 没有配置则返回 0
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static int GetRoleParamsInt32(DBRoleInfo dbRoleInfo, string name)
        {
            String valueString = Global.GetRoleParamByName(dbRoleInfo, name);

            if (null == valueString || valueString.Length <= 0)
            {
                return 0;
            }

            return Global.SafeConvertToInt32(valueString);
        }

        #endregion 角色参数管理

        #region 限时抢购相关

        /// <summary>
        /// 限时抢购逻辑锁
        /// </summary>
        public static object QiangGouMutex = new object();

        /// <summary>
        /// 添加一个新的抢购记录
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="roleID"></param>
        /// <param name="goodsID"></param>
        /// <param name="goodsNum"></param>
        /// <param name="totalPrice"></param>
        /// <param name="leftMoney"></param>
        /// <param name="qiangGouId"></param>
        /// <returns></returns>
        public static int AddNewQiangGouBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney, int qiangGouId)
        {
            lock (QiangGouMutex)
            {
                //添加新的抢购记录到数据库
                int ret = DBWriter.AddNewQiangGouBuyItem(dbMgr, roleID, goodsID, goodsNum, totalPrice, leftMoney, qiangGouId);

                if (ret < 0)
                {
                    return -10001;//数据库执行出错
                }
            }

            return 1;//成功
        }

        /// <summary>
        /// 查询新的抢购记录信息
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="roleID"></param>
        /// <param name="goodsID"></param>
        /// <param name="qiangGouId"></param>
        /// <param name="roleBuyNum"></param>
        /// <param name="totalBuyNum"></param>
        public static void QueryQiangGouBuyItemInfo(DBManager dbMgr, int roleID, int goodsID, int qiangGouId, int random, out int roleBuyNum, out int totalBuyNum)
        {
            lock (QiangGouMutex)
            {
                ///查询个人购买个数
                roleBuyNum = DBQuery.QueryQiangGouBuyItemNumByRoleID(dbMgr, roleID, goodsID, qiangGouId, random); 

                ///查询总的购买个数
                totalBuyNum = DBQuery.QueryQiangGouBuyItemNum(dbMgr, goodsID, qiangGouId, random); 
            }
        }

        #endregion 限时抢购相关

        #region 位运算辅助

        /// <summary>
        /// 根据输入数值获取位的设置值
        /// </summary>
        /// <param name="whichOne"></param>
        /// <returns></returns>
        public static int GetBitValue(int whichOne)
        {
            int bitVal = 0;

            //标记已经领取过了指定的礼物
            /*if (1 == whichOne)
            {
                bitVal = 1;
            }
            else if (2 == whichOne)
            {
                bitVal = 2;
            }
            else if (3 == whichOne)
            {
                bitVal = 4;
            }
            else if (4 == whichOne)
            {
                bitVal = 8;
            }
            else if (5 == whichOne)
            {
                bitVal = 16;
            }
            else if (6 == whichOne)
            {
                bitVal = 32;
            }
            else if (7 == whichOne)
            {
                bitVal = 64;
            }*/

            bitVal = (int)Math.Pow(2, whichOne - 1);
            return bitVal;
        }

        #endregion 位运算辅助

        #region 时间装换

        /// <summary>
        /// 获取一个增加了几天时间的DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="addDays"></param>
        /// <returns></returns>
        public static DateTime GetAddDaysDataTime(DateTime dateTime, int addDays, bool roundDay = true)
        {
            if (roundDay)
            {
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
            }

            return new DateTime(dateTime.Ticks + ((long)addDays * 10000 * 1000 * 24 * 60 * 60));
        }

        #endregion 时间转换

        #region MU翅膀系统

        /// <summary>
        /// 通过数据库ID定位翅膀数据
        /// </summary>
        /// <param name="dbID"></param>
        /// <returns></returns>
        public static WingData GetWingData(DBRoleInfo dbRoleInfo)
        {
            if (null == dbRoleInfo) return null;
            lock (dbRoleInfo)
            {
                return dbRoleInfo.MyWingData;
            }
        }

        #endregion MU翅膀系统

        #region 处理钻石消费
        public static TCPProcessCmdResults SaveConsumeLog(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            string cmdData = null;

            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception) //解析错误
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID));

                tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", (int)TCPGameServerCmds.CMD_DB_ERR_RETURN);
                return TCPProcessCmdResults.RESULT_DATA;
            }

            try
            {

                string[] fields = cmdData.Split(':');
                if (fields.Length != 3)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}",
                        (TCPGameServerCmds)nID, fields.Length, cmdData));

                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", (int)TCPGameServerCmds.CMD_DB_ERR_RETURN);
                    return TCPProcessCmdResults.RESULT_DATA;
                }

                int roleID = Global.SafeConvertToInt32(fields[0]);
                int money = Global.SafeConvertToInt32(fields[1]);
                int type = Global.SafeConvertToInt32(fields[2]);
                string datestr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(roleID);
                if (null == dbRoleInfo)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}",
                        (TCPGameServerCmds)nID, roleID));

                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", (int)TCPGameServerCmds.CMD_DB_ERR_RETURN);
                    return TCPProcessCmdResults.RESULT_DATA;
                }
                int nRet = DBWriter.SaveConsumeLog(dbMgr, roleID, datestr, type, money);
                if (0 == nRet)
                {
                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
                }
                else
                {
                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", (int)TCPGameServerCmds.CMD_DB_ERR_RETURN);
                }

                return TCPProcessCmdResults.RESULT_DATA;
            }
            catch (Exception e)
            {
            }
            return TCPProcessCmdResults.RESULT_DATA;
        }
        #endregion

       
    }

    
}