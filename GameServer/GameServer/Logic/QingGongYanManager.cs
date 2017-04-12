using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Server.Tools;
using Server.TCP;
using Server.Protocol;
using GameServer.Server;
using GameServer.Logic;

namespace GameServer.Logic
{
    public class QingGongYanInfo
    {
        public int Index;               // 流水号
        public int NpcID;               // NPC编号
        public int MapCode;             // 地图编号
        public int X;                   // NPC坐标
        public int Y;                   // NPC坐标
        public int Direction;           // NPC方向
        // 可用天
        public List<int> DayOfWeek = new List<int>();
        public string BeginTime;        // 开启时间
        public string OverTime;         // 结束时间
        public int FunctionID;          // 功能编号 SystemOperations.xml中对应的编号
        public int HoldBindJinBi;    // 举办者要花费的金币
        public int TotalNum;            // 能够参加的总数
        public int SingleNum;           // 个人能够参加的总数
        public int JoinBindJinBi;       // 参加所需的金币
        public int ExpAward;            // 参加能够获得的经验
        public int XingHunAward;        // 参加能够获得的星魂
        public int ZhanGongAward;       // 参加能够获得的战功
        public int ZuanShiCoe;          // 结束金币转换成钻石的比例 / ZuanShiCoe

        /// <summary>
        /// 判断某个时间，是否在允许开放的日子
        /// </summary>
        public bool IfDayOfWeek(DateTime time)
        {
            int dayofweek = (int)time.DayOfWeek;

            if (dayofweek == 0)
                dayofweek = 7;

            return DayOfWeek.IndexOf(dayofweek) >= 0;
        }
    }

    public enum QingGongYanResult
    {
        Success,        // 成功
        ErrorParam,     // 参数错误
        Holding,        // 庆功宴正在举办
        NotKing,        // 不是王城占领者
        OutTime,        // 允许的时间外
        RepeatHold,     // 重复申请
        CountNotEnough, // 参加次数不足
        TotalNotEnough, // 总参加次数不足
        MoneyNotEnough, // 金钱不足
    }

    public class QingGongYanManager
    {
        #region 成员

        /// <summary>
        /// 庆功宴配置字典线程锁
        /// </summary>
        private object _QingGongYanMutex = new object();

        /// <summary>
        /// 配置字典
        /// </summary>
        private Dictionary<int, QingGongYanInfo> QingGongYanDict = new Dictionary<int, QingGongYanInfo>();

        /// <summary>
        /// 庆功宴的NPC是否刷新的标记
        /// 因为刷怪是异步操作，所以用这个做标记，防止生成多个Monster
        /// </summary>
        private bool QingGongYanOpenFlag = false;

        /// <summary>
        /// 庆功宴的NPC数据
        /// </summary>
        private Monster QingGongYanMonster = null;

        #endregion

        #region 配置文件

        /// <summary>
        /// 加载配置文件 GleeFeastAward.xml
        /// </summary>
        public void LoadQingGongYanConfig()
        {
            lock (_QingGongYanMutex)
            {
                QingGongYanDict.Clear();

                string fileName = "Config/GleeFeastAward.xml";
                XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
                if (null == xml) return;

                IEnumerable<XElement> xmlItems = xml.Elements();
                foreach (var xmlItem in xmlItems)
                {
                    QingGongYanInfo InfoData = new QingGongYanInfo();
                    InfoData.Index = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
                    InfoData.NpcID = (int)Global.GetSafeAttributeLong(xmlItem, "NPCID");
                    InfoData.MapCode = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode");
                    InfoData.X = (int)Global.GetSafeAttributeLong(xmlItem, "X");
                    InfoData.Y = (int)Global.GetSafeAttributeLong(xmlItem, "Y");
                    InfoData.Direction = (int)Global.GetSafeAttributeLong(xmlItem, "Direction");

                    // Week
                    string[] strWeek = Global.GetSafeAttributeStr(xmlItem, "Week").Split(',');
                    if (null != strWeek)
                    {
                        for (int i = 0; i < strWeek.Length; i++)
                        {
                            InfoData.DayOfWeek.Add(Convert.ToInt32(strWeek[i]));
                        }
                    }

                    InfoData.BeginTime = Global.GetSafeAttributeStr(xmlItem, "BeginTime");
                    InfoData.OverTime = Global.GetSafeAttributeStr(xmlItem, "OverTime");
                    InfoData.FunctionID = (int)Global.GetSafeAttributeLong(xmlItem, "FunctionID");

                    InfoData.HoldBindJinBi = (int)Global.GetSafeAttributeLong(xmlItem, "ConductBindJinBi");
                    InfoData.TotalNum = (int)Global.GetSafeAttributeLong(xmlItem, "SumNum");
                    InfoData.SingleNum = (int)Global.GetSafeAttributeLong(xmlItem, "UseNum");
                    InfoData.JoinBindJinBi = (int)Global.GetSafeAttributeLong(xmlItem, "BindJinBi");
                    InfoData.ExpAward = (int)Global.GetSafeAttributeLong(xmlItem, "EXPAward");
                    InfoData.XingHunAward = (int)Global.GetSafeAttributeLong(xmlItem, "XingHunAward");
                    InfoData.ZhanGongAward = (int)Global.GetSafeAttributeLong(xmlItem, "ZhanGongAward");
                    InfoData.ZuanShiCoe = (int)Global.GetSafeAttributeLong(xmlItem, "ZuanShiRatio");

                    QingGongYanDict[InfoData.Index] = InfoData;
                }

            }
        }

        /// <summary>
        /// 根据流水号取得庆功宴相关配置
        /// </summary>
        public QingGongYanInfo GetQingGongYanConfig(int index)
        {
            QingGongYanInfo InfoData = null;
            lock (_QingGongYanMutex)
            {
                if (QingGongYanDict.ContainsKey(index))
                    InfoData = QingGongYanDict[index];
            }
            return InfoData;
        }

        #endregion

        #region 逻辑相关

        /// <summary>
        /// 举办庆功宴
        /// </summary>
        public QingGongYanResult HoldQingGongYan(GameClient client, int index)
        {
            // 是不是王城占领者

            QingGongYanInfo InfoData = GetQingGongYanConfig(index);
            if (null != InfoData)
            {
                return QingGongYanResult.ErrorParam;
            }

            /// 此时是否能够开启庆功宴
            if (!InfoData.IfDayOfWeek(DateTime.Now))
            {
                return QingGongYanResult.OutTime;
            }

            int DBStartDay = Convert.ToInt32( GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_startday", "0") );
            int currDay = Global.GetOffsetDay(DateTime.Now);

            //  如果今天有庆功宴 并且庆功宴结束时间还没到 提示已经申请
            if (DBStartDay == currDay && DateTime.Now <= DateTime.Parse(InfoData.OverTime))
            {
                return QingGongYanResult.RepeatHold;
                
            }

            // 计算申请之后，庆功宴的举办时间
            int startDay = 0;
            // 在庆功宴开始时间之前，就在今天开启
            if (DateTime.Now < DateTime.Parse(InfoData.BeginTime))
            {
                startDay = currDay;
            }
            // 否则在明天开启
            else
            {
                startDay = currDay + 1;
            }

            // 如果计算出来的举办时间和数据库的举办时间相同，则返回已经申请
            if (startDay == DBStartDay)
            {
                return QingGongYanResult.RepeatHold;
            }

            // 检查举办所需金币是否足够
            if (InfoData.HoldBindJinBi > 0)
            {
                if (InfoData.HoldBindJinBi > client.ClientData.YinLiang)
                {
                    return QingGongYanResult.MoneyNotEnough;
                }
            
            }

            // 扣除举办所需金币
            if (InfoData.HoldBindJinBi > 0)
            {
                if (!GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool,
                        client, InfoData.HoldBindJinBi, "举办庆功宴"))
                {
                    return QingGongYanResult.MoneyNotEnough;
                }
            }

            GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_roleid", client.ClientData.RoleID.ToString());
            GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_rolename", client.ClientData.RoleName);
            GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_startday", startDay.ToString());
            GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_grade", index.ToString());
            GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joincount", "0");
            GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joinmoney", "0");
            // log it...
            GameManager.logDBCmdMgr.AddDBLogInfo(-1, "参加庆功宴", startDay.ToString(), "", client.ClientData.RoleName, "", index, client.ClientData.ZoneID, client.strUserID);

            return QingGongYanResult.Success;
        }

        /// <summary>
        /// 此时是否需要开启庆功宴
        /// </summary>
        public bool IfNeedOpenQingGongYan()
        {
            // 没有档次
            QingGongYanInfo InfoData = GetInfoData();
            if (null != InfoData)
            {
                return false;
            }

            int DBStartDay = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_startday", "0"));
            int currDay = Global.GetOffsetDay(DateTime.Now);
            // 今天没有庆功宴
            if (DBStartDay != currDay)
            {
                return false;
            }

            // 在举办时间外
            if (DateTime.Now < DateTime.Parse(InfoData.BeginTime) || DateTime.Now > DateTime.Parse(InfoData.OverTime))
            {
                return false;
            }

            // 如果已经开启，刷过怪
            if (true == QingGongYanOpenFlag)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 此时是否需要关闭庆功宴
        /// </summary>
        public bool IfNeedCloseQingGongYan()
        {
            // 没有档次
            QingGongYanInfo InfoData = GetInfoData();
            if (null != InfoData)
            {
                return false;
            }

            int DBStartDay = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_startday", "0"));
            int currDay = Global.GetOffsetDay(DateTime.Now);
            // 今天没有庆功宴
            if (DBStartDay != currDay)
            {
                return false;
            }

            // 在结束时间之前
            if ( DateTime.Now <= DateTime.Parse(InfoData.OverTime))
            {
                return false;
            }

            // 没有开启的标志
            if (false == QingGongYanOpenFlag)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 定时记录标志
        /// </summary>
        private long lastProcessTicks = 0;

        /// <summary>
        /// 定时检测庆功宴是否要开启
        /// </summary>
        public void CheckQingGongYan(long ticks)
        {
            // 10秒检测一次
            if (ticks - lastProcessTicks < 1000 * 10)
            {
                return;
            }
            lastProcessTicks = ticks;

            if (IfNeedOpenQingGongYan())
            {
                OpenQingGongYan();
            }
            if (IfNeedCloseQingGongYan())
            {
                CloseQingGongYan();
            }
        }

        /// <summary>
        /// 开启庆功宴
        /// </summary>
        public void OpenQingGongYan()
        {
            QingGongYanOpenFlag = true;

            QingGongYanInfo InfoData = GetInfoData();
            if (null != InfoData)
            {
                return;
            }

            // 生成怪物
            GameManager.MonsterZoneMgr.AddDynamicMonsters(InfoData.MapCode, InfoData.NpcID, -1, 1, InfoData.X, InfoData.Y, 1);
        }

        /// <summary>
        /// 保存Monster
        /// </summary>
        public void OnLoadDynamicMonsters(Monster monster)
        {
            QingGongYanInfo InfoData = GetInfoData();
            if (null != InfoData)
            {
                return;
            }

            // 是不是庆功宴的Monster
            if (InfoData.NpcID == monster.MonsterInfo.ExtensionID)
            {
                return;
            }

            // 怪物方向
            monster.Direction = (double)InfoData.Direction;
            // 保存起来便于释放
            QingGongYanMonster = monster;

            string roleName = GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_rolename", "神秘人");
            string broadCastMsg = StringUtil.substitute(Global.GetLang("{0}举办的庆功宴刚刚开启了，大家快去参加呀"), roleName);

            //播放用户行为消息
            Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, broadCastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.SysHintAndChatBox);
        }

        /// <summary>
        /// 取得当前庆功宴的配置
        /// </summary>
        public QingGongYanInfo GetInfoData()
        {
            // 没有档次
            int DBGrade = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_grade", "0"));
            if (DBGrade <= 0)
            {
                return null;
            }

            // 档次没有配置
            return GetQingGongYanConfig(DBGrade);
        }

        /// <summary>
        /// 关闭庆功宴
        /// </summary>
        public void CloseQingGongYan()
        {
            // 销毁怪物
            if (null != QingGongYanMonster)
            { 
                GameManager.MonsterMgr.DeadMonsterImmediately(QingGongYanMonster);
                QingGongYanMonster = null;                
            }
            QingGongYanOpenFlag = false;

            // log it...

            // 档次没有配置
            QingGongYanInfo InfoData = GetInfoData();
            if (null != InfoData)
            {
                return;
            }

            if (InfoData.ZuanShiCoe <= 0)
            {
                return;
            }

            int JoinMoney = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_joinmoney", "0"));
            if (JoinMoney <= 0)
            {
                return;
            }

            int ZuanShiAward = JoinMoney / InfoData.ZuanShiCoe;

            if (ZuanShiAward <= 0)
            {
                return;
            }

            int DBRoleID = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_roleid", "0"));
            if (DBRoleID <= 0)
            {
                return;
            }

            //string sContent = "您在2015年02月02日 20:00举办的宴会已成功结束，共获得收益200钻石。";
            string sContent = string.Format("您在{0:0000}年{1:00}月{2:00}日 {3}举办的宴会已成功结束，共获得收益{4}钻石。", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, InfoData.BeginTime);

            Global.UseMailGivePlayerAward3(DBRoleID, null, sContent, "庆功宴", ZuanShiAward);

            // 清空记录
            GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_roleid", "0");
            GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_startday", "0");
            GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_grade", "0");
            GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joincount", "0");
            GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joinmoney", "0");

            string roleName = GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_rolename", "神秘人");
            string broadCastMsg = StringUtil.substitute(Global.GetLang("{0}举办的庆功宴刚刚结束了，大家快去参加呀"), roleName);

            //播放用户行为消息
            Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, broadCastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.SysHintAndChatBox);
        }

        /// <summary>
        /// 参加庆功宴
        /// </summary>
        public QingGongYanResult JoinQingGongYan(GameClient client)
        {
            if (null == QingGongYanMonster)
            {
                return QingGongYanResult.OutTime;
            }

            QingGongYanInfo InfoData = GetInfoData();
            if (null != InfoData)
            {
                return QingGongYanResult.OutTime;
            }

            int JoinCount = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_joincount", "0"));
            if (JoinCount > 0)
            {
                if (JoinCount >= InfoData.TotalNum)
                {
                    return QingGongYanResult.TotalNotEnough;
                }
            }

            // 检查举办所需金币是否足够
            if (InfoData.JoinBindJinBi > 0)
            {
                if (InfoData.JoinBindJinBi > client.ClientData.YinLiang)
                {
                    return QingGongYanResult.MoneyNotEnough;
                }
            }

            String QingGongYanJoinFlag = Global.GetRoleParamByName(client, RoleParamName.QingGongYanJoinFlag);
            int currDay = Global.GetOffsetDay(DateTime.Now);
            int lastJoinDay = 0;
            int joinCount = 0;
            // day:count
            if (null != QingGongYanJoinFlag)
            {
                string[] fields = QingGongYanJoinFlag.Split(',');
                if (2 == fields.Length)
                {
                    lastJoinDay = Convert.ToInt32(fields[0]);
                    joinCount = Convert.ToInt32(fields[1]);
                }
            }

            if (currDay != lastJoinDay)
            {
                joinCount = 0;
            }


            if (InfoData.SingleNum > 0)
            {
                if (joinCount >= InfoData.SingleNum)
                {
                    return QingGongYanResult.CountNotEnough;
                }
            }

            // 扣除举办所需金币
            if (InfoData.JoinBindJinBi > 0)
            {
                if (!GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool,
                        client, InfoData.JoinBindJinBi, "举办庆功宴"))
                {
                    return QingGongYanResult.MoneyNotEnough;
                }
            }

            // 计数
            string roleParam = currDay.ToString() + "," + (joinCount + 1).ToString();
            Global.UpdateRoleParamByName(client, RoleParamName.QingGongYanJoinFlag, roleParam, true);
            // 参加计数叠加
            GameManager.GameConfigMgr.ModifyGameConfigItem("qinggongyan_joincount", 1);
            // 记录缴纳的
            GameManager.GameConfigMgr.ModifyGameConfigItem("qinggongyan_joinmoney", InfoData.JoinBindJinBi);

            // 发奖
            if (InfoData.ExpAward > 0)
            {
                GameManager.ClientMgr.ProcessRoleExperience(client, InfoData.ExpAward);
            }

            if (InfoData.XingHunAward > 0)
            {
                GameManager.ClientMgr.ModifyStarSoulValue(client, InfoData.XingHunAward, "庆功宴", true, true);
            }

            if (InfoData.ZhanGongAward > 0)
            {
                Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.GuildZhanGong, Global.GetRoleParamsInt32FromDB(client, RoleParamName.GuildZhanGong) + InfoData.ZhanGongAward, true);
                GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZhanGong, InfoData.ZhanGongAward);
            }

            // log it
            GameManager.logDBCmdMgr.AddDBLogInfo(-1, "参加庆功宴", "", "", client.ClientData.RoleName, "", 1, client.ClientData.ZoneID, client.strUserID);

            return QingGongYanResult.Success;
        }

        #endregion

        #region 协议处理

        /// <summary>
        /// 申请举办
        /// </summary>
        public TCPProcessCmdResults ProcessHoldQingGongYanCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            string cmdData = null;

            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception) //解析错误
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket)));
                return TCPProcessCmdResults.RESULT_FAILED;
            }

            try
            { 
                string[] fields = cmdData.Split(':');
                if (fields.Length != 2)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}",
                        (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket), fields.Length));
                    return TCPProcessCmdResults.RESULT_FAILED;
                }

                int roleID = Convert.ToInt32(fields[0]);
                int Index = Convert.ToInt32(fields[1]);
                GameClient client = GameManager.ClientMgr.FindClient(socket);
                if (null == client || client.ClientData.RoleID != roleID)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket), roleID));
                    return TCPProcessCmdResults.RESULT_FAILED;
                }

                QingGongYanResult result = HoldQingGongYan(client, Index);
                string strcmd = "";

                if (result != QingGongYanResult.Success)
                {
                    strcmd = string.Format("{0}:{1}", (int)result, roleID);
                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                    return TCPProcessCmdResults.RESULT_DATA;
                }

                strcmd = string.Format("{0}:{1}", (int)QingGongYanResult.Success, roleID);
                tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                return TCPProcessCmdResults.RESULT_DATA;
            }

            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "ProcessHoldQingGongYanCMD", false);
            }


            return TCPProcessCmdResults.RESULT_FAILED;
        }
        /// <summary>
        /// 申请信息
        /// </summary>
        public TCPProcessCmdResults ProcessQueryQingGongYanCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            string cmdData = null;

            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception) //解析错误
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket)));
                return TCPProcessCmdResults.RESULT_FAILED;
            }

            try
            {
                string[] fields = cmdData.Split(':');
                if (fields.Length != 1)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}",
                        (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket), fields.Length));
                    return TCPProcessCmdResults.RESULT_FAILED;
                }

                int roleID = Convert.ToInt32(fields[0]);
                GameClient client = GameManager.ClientMgr.FindClient(socket);
                if (null == client || client.ClientData.RoleID != roleID)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket), roleID));
                    return TCPProcessCmdResults.RESULT_FAILED;
                }

                int DBGrade = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_grade", "0"));
                int TotalCount = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_joincount", "0"));
                int JoinMoney = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_joinmoney", "0"));

                String QingGongYanJoinFlag = Global.GetRoleParamByName(client, RoleParamName.QingGongYanJoinFlag);
                int currDay = Global.GetOffsetDay(DateTime.Now);
                int lastJoinDay = 0;
                int joinCount = 0;
                // day:count
                if (null != QingGongYanJoinFlag)
                {
                    string[] strTemp = QingGongYanJoinFlag.Split(',');
                    if (2 == strTemp.Length)
                    {
                        lastJoinDay = Convert.ToInt32(strTemp[0]);
                        joinCount = Convert.ToInt32(strTemp[1]);
                    }
                }

                if (currDay != lastJoinDay)
                {
                    joinCount = 0;
                }

                string strcmd = "";

                strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", roleID, DBGrade, joinCount, TotalCount, JoinMoney);
                tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                return TCPProcessCmdResults.RESULT_DATA;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "ProcessHoldQingGongYanCMD", false);
            }

            return TCPProcessCmdResults.RESULT_FAILED;
        }

        /// <summary>
        /// 申请参加
        /// </summary>
        public TCPProcessCmdResults ProcessJoinQingGongYanCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            string cmdData = null;

            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception) //解析错误
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket)));
                return TCPProcessCmdResults.RESULT_FAILED;
            }

            try
            {
                string[] fields = cmdData.Split(':');
                if (fields.Length != 1)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}",
                        (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket), fields.Length));
                    return TCPProcessCmdResults.RESULT_FAILED;
                }

                int roleID = Convert.ToInt32(fields[0]);
                GameClient client = GameManager.ClientMgr.FindClient(socket);
                if (null == client || client.ClientData.RoleID != roleID)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket), roleID));
                    return TCPProcessCmdResults.RESULT_FAILED;
                }

                QingGongYanResult result = JoinQingGongYan(client);
                string strcmd = "";

                if (result != QingGongYanResult.Success)
                {
                    strcmd = string.Format("{0}:{1}", (int)result, roleID);
                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                    return TCPProcessCmdResults.RESULT_DATA;
                }

                strcmd = string.Format("{0}:{1}", (int)QingGongYanResult.Success, roleID);
                tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
                return TCPProcessCmdResults.RESULT_DATA;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "ProcessJoinQingGongYanCMD", false);
            }

            return TCPProcessCmdResults.RESULT_FAILED;
        }

        #endregion

    }
}
