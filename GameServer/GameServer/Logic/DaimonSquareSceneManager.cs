using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Server;
using System.Xml.Linq;
using Server.Data;
using System.Windows;
using Server.Tools;
using Server.Protocol;
using System.Threading;

namespace GameServer.Logic
{
    // 恶魔广场管理类 [11/5/2013 LiaoWei]
    class DaimonSquareSceneManager
    {
        /// <summary>
        /// 恶魔广场场景
        /// </summary>
        public static Dictionary<int, DaimonSquareScene> m_DaimonSquareListScenes = new Dictionary<int, DaimonSquareScene>();

        /// <summary>
        /// 最高积分 -- 分数
        /// </summary>
        public static int m_nDaimonSquareMaxPoint = -1;

        /// <summary>
        /// 最高积分 -- 人名
        /// /// </summary>
        public static string m_sDaimonSquareMaxPointName = "";

        /// <summary>
        /// 推送日期ID
        /// /// </summary>
        public static int m_nPushMsgDayID = -1;

        /// <summary>
        /// 加载恶魔广场场景到管理器
        /// </summary>
        static public void InitDaimonSquare()
        {
            //LoadDaimonSquareListScenes();
        }

        /// <summary>
        /// 加载恶魔广场场景到管理器
        /// </summary>
        static public void LoadDaimonSquareListScenes()
        {
            int[] nArray = GameManager.systemParamsList.GetParamValueIntArrayByName("DemonMapCodeIds");

            for (int i = 0; i < nArray.Length; ++i)
            {
                DaimonSquareScene tmpMap = new DaimonSquareScene();
                tmpMap.CleanAllInfo();

                tmpMap.m_nMapCode = nArray[i];

                AddDaimonSquareListScenes(nArray[i], tmpMap);
            }

            // 向DB请求最高积分信息
            Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.DemoSque);

            m_nPushMsgDayID = Global.SafeConvertToInt32(GameManager.GameConfigMgr.GetGameConifgItem(GameConfigNames.DemoSquarePushMsgDayID));
        }

        /// <summary>
        /// 添加一个场景
        /// </summary>
        static public void AddDaimonSquareListScenes(int nMap, DaimonSquareScene bcScene)
        {
            lock (m_DaimonSquareListScenes)
            {
                m_DaimonSquareListScenes.Add(nMap, bcScene);
            }
        }

        /// <summary>
        /// 删除一个场景
        /// </summary>
        public void RemoveDaimonSquareListScenes(int nMap)
        {
            lock (m_DaimonSquareListScenes)
            {
                m_DaimonSquareListScenes.Remove(nMap);
            }
        }

        /// <summary>
        /// 取得基本信息
        /// </summary>
        static public DaimonSquareScene GetDaimonSquareListScenes(int nMap)
        {
            return m_DaimonSquareListScenes[nMap];
        }

        /// <summary>
        /// 管理动态刷怪列表--增加
        /// </summary>
        static public void AddDaimonSquareDynamicMonster(int nMap, Monster monster)
        {
            m_DaimonSquareListScenes[nMap].m_nDynamicMonsterList.Add(monster);
            return;
        }

        /// <summary>
        /// 管理动态刷怪列表--移除
        /// </summary>
        public void RemoveDaimonSquareDynamicMonster(int nMap, Monster monster)
        {
            m_DaimonSquareListScenes[nMap].m_nDynamicMonsterList.Remove(monster);
            return;
        }

        /// <summary>
        /// 设置最高积分信息
        /// </summary>
        public static void SetTotalPointInfo(string sName, int nPoint)
        {
            m_sDaimonSquareMaxPointName = sName;
            m_nDaimonSquareMaxPoint     = nPoint;
        }

        /// <summary>
        // 心跳处理
        /// </summary>
        static public void HeartBeatDaimonSquareScene()
        {
            foreach (var DaimonSquareScenes in m_DaimonSquareListScenes)
            {
                DaimonSquareDataInfo bcDataTmp = Data.DaimonSquareDataInfoList[DaimonSquareScenes.Key];
                DaimonSquareScene bcTmp = GetDaimonSquareListScenes(DaimonSquareScenes.Key);
                if (bcTmp == null || bcDataTmp == null)
                    continue;

                int nRoleNum = 0;
                nRoleNum = GameManager.ClientMgr.GetMapClientsCount(bcTmp.m_nMapCode);
                if (nRoleNum <= 0)
                {
                    if (bcTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_BEGIN)
                    {
                        // 做清空处理  比如 所有动态刷出的怪 都delete掉
                        CleanDaimonSquareScene(bcTmp.m_nMapCode);
                        bcTmp.CleanAllInfo();
                        bcTmp.m_nMapCode = DaimonSquareScenes.Key;
                    }
                    //continue;
                }

                // 当前tick
                long ticks = DateTime.Now.Ticks / 10000;

                if (bcTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_NULL)
                {
                    bool bPushMsg = false;

                    if (Global.CanEnterDaimonSquareOnTime(bcDataTmp.BeginTime, 0))
                    {   
                        // 场景开启
                        bcTmp.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_PREPARE;
                        bcTmp.m_lPrepareTime = DateTime.Now.Ticks / 10000;
                        bcTmp.m_nMonsterTotalWave = bcDataTmp.MonsterID.Length;

                        // 消息推送
                        if (bPushMsg)
                        {
                            int nNow = DateTime.Now.DayOfYear;

                            if (bPushMsg && m_nPushMsgDayID != nNow)
                            {
                                //Global.DayActivityTiggerPushMessage((int)SpecialActivityTypes.DemoSque);

                                Global.UpdateDBGameConfigg(GameConfigNames.DemoSquarePushMsgDayID, nNow.ToString());

                                m_nPushMsgDayID = nNow;
                            }
                        }
                    }
                }
                else if (bcTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_PREPARE)
                {
                    if (ticks >= (bcTmp.m_lPrepareTime + (bcDataTmp.PrepareTime * 1000)))
                    {
                        // 准备战斗
                        bcTmp.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_BEGIN;

                        bcTmp.m_lBeginTime = DateTime.Now.Ticks / 10000;
                        int nTimer = (int)((bcDataTmp.DurationTime * 1000 - (ticks - bcTmp.m_lBeginTime)) / 1000);

                        GameManager.ClientMgr.NotifyDaimonSquareMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,DaimonSquareScenes.Key,
                                                                    (int)TCPGameServerCmds.CMD_SPR_QUERYDAIMONSQUARETIMERINFO, (int)DaimonSquareStatus.FIGHT_STATUS_BEGIN, nTimer, 0, 0, 0); // 战斗结束倒计时
                    }
                }
                else if (bcTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_BEGIN)
                {
                    // 开始战斗 -- 刷怪
                    if (bcTmp.m_nCreateMonsterFlag == 0 && bcTmp.m_nMonsterWave < bcTmp.m_nMonsterTotalWave)
                        DaimonSquareSceneCreateMonster(bcTmp, bcDataTmp);
                    
                    if (ticks >= (bcTmp.m_lBeginTime + (bcDataTmp.DurationTime * 1000)) || bcTmp.m_nKillMonsterTotalNum == bcDataTmp.MonsterSum)
                    {
                        bcTmp.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_END;
                        bcTmp.m_lEndTime = DateTime.Now.Ticks / 10000;
                    }
                }
                else if (bcTmp.m_eStatus == DaimonSquareStatus.FIGHT_STATUS_END)
                {
                    // 战斗结束

                    int nTimer = (int)((bcDataTmp.LeaveTime * 1000 - (ticks - bcTmp.m_lEndTime)) / 1000);

                    if (bcTmp.m_bEndFlag == false)
                    {
                        GameManager.ClientMgr.NotifyDaimonSquareMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, DaimonSquareScenes.Key,
                                                                    (int)TCPGameServerCmds.CMD_SPR_QUERYDAIMONSQUARETIMERINFO, (int)DaimonSquareStatus.FIGHT_STATUS_END, nTimer, 0, 0, 0);

                        // 剩余时间奖励
                        long nTimeInfo = 0;
                        nTimeInfo = bcTmp.m_lEndTime - bcTmp.m_lBeginTime;
                        
                        long nRemain = 0;
                        nRemain = ((bcDataTmp.DurationTime * 1000) - nTimeInfo) / 1000;

                        if (nRemain >= bcDataTmp.DurationTime)
                            nRemain = bcDataTmp.DurationTime / 2;
                        
                        int nTimeAward = 0;
                        nTimeAward = (int)(bcDataTmp.TimeModulus * nRemain);

                        if (nTimeAward < 0)
                            nTimeAward = 0;

                        GameManager.ClientMgr.NotifyDaimonSquareMsgEndFight(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, DaimonSquareScenes.Key,
                                                                            (int)TCPGameServerCmds.CMD_SPR_DAIMONSQUAREENDFIGHT, nTimeAward);

                        bcTmp.m_bEndFlag = true;
                    } 

                    if (ticks >= (bcTmp.m_lEndTime + (bcDataTmp.LeaveTime * 1000)))
                    {
                        // 清场
                        List<Object> objsList = GameManager.ClientMgr.GetMapClients(DaimonSquareScenes.Key);
                        if (objsList != null)
                        {
                            for (int n = 0; n < objsList.Count; ++n)
                            {
                                GameClient c = objsList[n] as GameClient;
                                if (c == null)
                                    continue;

                                if(c.ClientData.MapCode != DaimonSquareScenes.Key)
                                    continue;

                                //CompleteDaimonSquareScene(c, bcTmp, bcDataTmp);

                                // 根据公式和积分奖励经验
                                //GiveAwardDaimonSquareScene(c);

                                // 退出场景
                                int toMapCode = GameManager.MainMapCode;    //主城ID 防止意外
                                int toPosX = -1;
                                int toPosY = -1;
                                if (MapTypes.Normal == Global.GetMapType(c.ClientData.LastMapCode))
                                {
                                    if (GameManager.BattleMgr.BattleMapCode != c.ClientData.LastMapCode|| GameManager.ArenaBattleMgr.BattleMapCode != c.ClientData.LastMapCode)
                                    {
                                        toMapCode = c.ClientData.LastMapCode;
                                        toPosX = c.ClientData.LastPosX;
                                        toPosY = c.ClientData.LastPosY;
                                    }
                                }

                                GameMap gameMap = null;
                                if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
                                {
                                    c.ClientData.bIsInDaimonSquareMap = false;
                                    GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, c, toMapCode, toPosX, toPosY, -1);
                                }
                            }
                        }

                        CleanDaimonSquareScene(DaimonSquareScenes.Key);
                        bcTmp.CleanAllInfo();
                        bcTmp.m_nMapCode = DaimonSquareScenes.Key;
                    }
                }
            }

        }

        /// <summary>
        // 给奖励
        /// </summary>
        static public void GiveAwardDaimonSquareScene(GameClient client)
        {
            DaimonSquareDataInfo bcDataTmp = Data.DaimonSquareDataInfoList[client.ClientData.MapCode];
            DaimonSquareScene bcTmp = GetDaimonSquareListScenes(client.ClientData.MapCode);

            if (bcTmp == null || bcDataTmp == null)
                return;

            if (bcTmp.m_bIsFinishTask == true)
            {
                string[] sItem = bcDataTmp.AwardItem;

                if (null != sItem && sItem.Length > 0)
                {
                    for (int i = 0; i < sItem.Length; i++)
                    {
                        if (string.IsNullOrEmpty(sItem[i].Trim()))
                            continue;

                        string[] sFields = sItem[i].Split(',');
                        if (string.IsNullOrEmpty(sFields[i].Trim()))
                            continue;

                        int nGoodsID = Convert.ToInt32(sFields[0].Trim());
                        int nGoodsNum = Convert.ToInt32(sFields[1].Trim());

                        GoodsData goodsData = new GoodsData()
                        {
                            Id = -1,
                            GoodsID = nGoodsID,
                            Using = 0,
                            Forge_level = 0,
                            Starttime = "1900-01-01 12:00:00",
                            Endtime = Global.ConstGoodsEndTime,
                            Site = 0,
                            Quality = (int)GoodsQuality.White,
                            Props = "",
                            GCount = nGoodsNum,
                            Binding = 0,
                            Jewellist = "",
                            BagIndex = 0,
                            AddPropIndex = 0,
                            BornIndex = 0,
                            Lucky = 0,
                            Strong = 0,
                            ExcellenceInfo = 0,
                            AppendPropLev = 0,
                            ChangeLifeLevForEquip = 0,
                        };

                        string sMsg = "恶魔广场--统一奖励";

                        if (!Global.CanAddGoodsNum(client, nGoodsNum))
                        {
                            for (int j = 0; j < nGoodsNum; ++j)
                                Global.UseMailGivePlayerAward(client, goodsData, "恶魔广场", sMsg);
                        }
                        else
                            Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nGoodsID, nGoodsNum, goodsData.Quality, "", goodsData.Forge_level,
                                                        goodsData.Binding, 0, "", true, 1, sMsg, goodsData.Endtime);
                    }
                }
            }

            // 根据积分以及公式给奖励(经验)
            if (client.ClientData.DaimonSquarePoint > 0)
            {
                // 公式
                int nExp = client.ClientData.DaimonSquarePoint * bcDataTmp.ExpModulus;
                int nMoney = client.ClientData.DaimonSquarePoint * bcDataTmp.MoneyModulus;

                if (nExp > 0)
                    GameManager.ClientMgr.ProcessRoleExperience(client, nExp, false);

                if (nMoney > 0)
                    GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nMoney, "恶魔广场奖励一", false);
                    //GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nMoney);

                // 存盘
                if (client.ClientData.DaimonSquarePoint > client.ClientData.DaimonSquarePointTotalPoint)
                    client.ClientData.DaimonSquarePointTotalPoint = client.ClientData.DaimonSquarePoint;

                if (client.ClientData.DaimonSquarePoint > m_nDaimonSquareMaxPoint)
                    SetTotalPointInfo(client.ClientData.RoleName, client.ClientData.DaimonSquarePoint);

                // 清空
                client.ClientData.DaimonSquarePoint = 0;
            }

            return;
        }

        /// <summary>
        // 给奖励 -- 第二个版本
        /// </summary>
        static public void GiveAwardDaimonSquareScene2(GameClient client, int nMultiple)
        {   
            int nSceneID = Global.GetRoleParamsInt32FromDB(client, RoleParamName.DaimonSquareSceneid);
            int nFlag = Global.GetRoleParamsInt32FromDB(client, RoleParamName.DaimonSquareSceneFinishFlag);
            int nTimer = Global.GetRoleParamsInt32FromDB(client, RoleParamName.DaimonSquareSceneTimer);
            
            DaimonSquareDataInfo bcDataTmp = null;
            if (!Data.DaimonSquareDataInfoList.TryGetValue(nSceneID, out bcDataTmp))
                return;

            if (bcDataTmp == null)
                return;

            if (nFlag == 1)
            {
                string[] sItem = bcDataTmp.AwardItem;

                if (null != sItem && sItem.Length > 0)
                {
                    for (int i = 0; i < sItem.Length; i++)
                    {
                        if (string.IsNullOrEmpty(sItem[i].Trim()))
                            continue;

                        string[] sFields = sItem[i].Split(',');
                        if (string.IsNullOrEmpty(sFields[i].Trim()))
                            continue;

                        int nGoodsID = Convert.ToInt32(sFields[0].Trim());
                        int nGoodsNum = Convert.ToInt32(sFields[1].Trim());

                        GoodsData goodsData = new GoodsData(){Id = -1,GoodsID = nGoodsID,Using = 0,Forge_level = 0,Starttime = "1900-01-01 12:00:00",Endtime = Global.ConstGoodsEndTime,
                                                                Site = 0,Quality = (int)GoodsQuality.White,Props = "",GCount = nGoodsNum,Binding = 0,Jewellist = "",BagIndex = 0,
                                                                AddPropIndex = 0,BornIndex = 0,Lucky = 0,Strong = 0,ExcellenceInfo = 0,AppendPropLev = 0,ChangeLifeLevForEquip = 0,};

                        string sMsg = "恶魔广场--统一奖励";

                        if (!Global.CanAddGoodsNum(client, nGoodsNum))
                        {
                            for (int j = 0; j < nGoodsNum; ++j)
                                Global.UseMailGivePlayerAward(client, goodsData, "恶魔广场", sMsg);
                        }
                        else
                            Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nGoodsID, nGoodsNum, goodsData.Quality, "", goodsData.Forge_level,
                                                        goodsData.Binding, 0, "", true, 1, sMsg, goodsData.Endtime);
                    }
                }
            }

            // 根据积分以及公式给奖励(经验)
            int nTimeAward = 0;
            nTimeAward = (int)(bcDataTmp.TimeModulus * nTimer);
            if (client.ClientData.DaimonSquarePoint > 0)
            {
                // 公式
                int nExp = nMultiple * client.ClientData.DaimonSquarePoint * bcDataTmp.ExpModulus;
                int nMoney = client.ClientData.DaimonSquarePoint * bcDataTmp.MoneyModulus;

                if (nExp > 0)
                    GameManager.ClientMgr.ProcessRoleExperience(client, nExp, false);

                if (nMoney > 0)
                    GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nMoney, "恶魔广场奖励二", false);
                //GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nMoney);

                // 存盘
                if (client.ClientData.DaimonSquarePoint > client.ClientData.DaimonSquarePointTotalPoint)
                    client.ClientData.DaimonSquarePointTotalPoint = client.ClientData.DaimonSquarePoint;

                if (client.ClientData.DaimonSquarePoint > m_nDaimonSquareMaxPoint)
                    SetTotalPointInfo(client.ClientData.RoleName, client.ClientData.DaimonSquarePoint);

                // 清空
                client.ClientData.DaimonSquarePoint = 0;
                Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.DaimonSquarePlayerPoint, client.ClientData.DaimonSquarePoint, true);
            }

            return;
        }
        
        /// <summary>
        // 刷怪接口
        /// </summary>
        static public void DaimonSquareSceneCreateMonster(DaimonSquareScene bcTmp, DaimonSquareDataInfo bcDataTmp)
        {
            if (bcTmp.m_nMonsterWave >= bcTmp.m_nMonsterTotalWave)
                return;

            // 置刷怪标记
            bcTmp.m_nCreateMonsterFlag = 1;

            string sMonsterNum = null;
            string sMonsterID   = null;
            string sNeedSkillMonster = null;

            sMonsterNum = bcDataTmp.MonsterNum[bcTmp.m_nMonsterWave];
            sMonsterID  = bcDataTmp.MonsterID[bcTmp.m_nMonsterWave];
            sNeedSkillMonster = bcDataTmp.CreateNextWaveMonsterCondition[bcTmp.m_nMonsterWave];

            if (sMonsterID == null || sMonsterNum == null || sNeedSkillMonster == null)
                return;

            string[] sNum   = null;
            string[] sID    = null;
            string[] sRate  = null;

            sNum    = sMonsterNum.Split(',');
            sID     = sMonsterID.Split(',');
            sRate   = sNeedSkillMonster.Split(',');

            if (sNum.Length != sID.Length)
                return;

            GameMap gameMap = null;
            if (!GameManager.MapMgr.DictMaps.TryGetValue(bcDataTmp.MapCode, out gameMap))
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("恶魔广场报错 地图配置 ID = {0}", bcDataTmp.MapCode));
                return;
            }

            int gridX = gameMap.CorrectWidthPointToGridPoint(bcDataTmp.posX) / gameMap.MapGridWidth;
            int gridY = gameMap.CorrectHeightPointToGridPoint(bcDataTmp.posZ) / gameMap.MapGridHeight;

            int gridNum = gameMap.CorrectWidthPointToGridPoint(bcDataTmp.Radius);

            for (int i = 0; i < sNum.Length; ++i)
            {
                int nNum    = Global.SafeConvertToInt32(sNum[i]);
                int nID     = Global.SafeConvertToInt32(sID[i]);
                //System.Console.WriteLine("liaowei是帅哥  恶魔广场 i = {0}",i);
                for (int j = 0; j < nNum; ++j)
                {
                    GameManager.MonsterZoneMgr.AddDynamicMonsters(bcTmp.m_nMapCode, nID, -1, 1, gridX, gridY, gridNum);

                    //System.Console.WriteLine("liaowei是帅哥  恶魔广场 j = {0}", j);

                    ++bcTmp.m_nCreateMonsterCount;
                }
            }

            // 计数要杀死怪的数量
            bcTmp.m_nNeedKillMonsterNum = bcTmp.m_nCreateMonsterCount * Global.SafeConvertToInt32(sRate[0]) / 100;

            // 递增刷怪波数
            ++bcTmp.m_nMonsterWave;

            //System.Console.WriteLine("liaowei是帅哥  恶魔广场第{0}波 {1}只！！！", bcTmp.m_nMonsterWave, bcTmp.m_nCreateMonsterCount);

            // 恶魔广场怪物波和人物得分信息
            GameManager.ClientMgr.NotifyDaimonSquareMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, bcDataTmp.MapCode,(int)TCPGameServerCmds.CMD_SPR_QUERYDAIMONSQUAREMONSTERWAVEANDPOINTRINFO, 
                                                        0, 0, bcDataTmp.MonsterID.Length - bcTmp.m_nMonsterWave, -100, 0);  // 只更新波数

            //System.Console.WriteLine("liaowei是帅哥  恶魔广场{0}里 刷第{1}波怪了 一共{3}只！！！", bcTmp.m_nMapCode, bcTmp.m_nMonsterWave, bcTmp.m_nCreateMonsterCount);

            return;
        }

        /// <summary>
        // 杀怪接口
        /// </summary>
        static public void DaimonSquareSceneKillMonster(GameClient client, Monster monster)
        {
            DaimonSquareDataInfo bcDataTmp = null;
            if (!Data.DaimonSquareDataInfoList.TryGetValue(client.ClientData.MapCode, out bcDataTmp))  //bcDataTmp = Data.DaimonSquareDataInfoList[client.ClientData.MapCode];
                return;

            DaimonSquareScene bcTmp = null;
            bcTmp = m_DaimonSquareListScenes[client.ClientData.MapCode];

            if (bcTmp == null || bcDataTmp == null)
                return;

            if (bcTmp.m_bEndFlag == true)
                return;

            Interlocked.Increment(ref bcTmp.m_nKillMonsterNum);

            //System.Console.WriteLine("liaowei是帅哥  恶魔广场{0}里 杀了{1}只怪了！！！", client.ClientData.MapCode, bcTmp.m_nKillMonsterNum);

            int nKillMonsterTotalNum = Interlocked.Increment(ref bcTmp.m_nKillMonsterTotalNum);

            client.ClientData.DaimonSquarePoint += monster.MonsterInfo.DaimonSquareJiFen;

            if (bcTmp.m_nCreateMonsterFlag == 1 && bcTmp.m_nKillMonsterNum == bcTmp.m_nNeedKillMonsterNum)
            {
                bcTmp.m_nCreateMonsterFlag = 0;

                // 重新计数
                bcTmp.m_nNeedKillMonsterNum = 0;
                bcTmp.m_nKillMonsterNum = 0;

                bcTmp.m_nCreateMonsterCount = 0;
            }

            // 如果杀光了所有的怪 就胜利了
            if (nKillMonsterTotalNum == bcDataTmp.MonsterSum)
            {
                bcTmp.m_eStatus = DaimonSquareStatus.FIGHT_STATUS_END;
                
                bcTmp.m_lEndTime = DateTime.Now.Ticks / 10000;

                CompleteDaimonSquareScene(client, bcTmp, bcDataTmp);

                bcTmp.m_bIsFinishTask = true;
            }

            // 恶魔广场怪物波和人物得分信息
            //GameManager.ClientMgr.NotifyDaimonSquareMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, bcDataTmp.MapCode, (int)TCPGameServerCmds.CMD_SPR_QUERYDAIMONSQUAREMONSTERWAVEANDPOINTRINFO,
            //                                            0, 0, bcDataTmp.MonsterID.Length - bcTmp.m_nMonsterWave, client.ClientData.DaimonSquarePoint, 0);

            string strcmd = string.Format("{0}:{1}", bcDataTmp.MonsterID.Length - bcTmp.m_nMonsterWave, client.ClientData.DaimonSquarePoint);
            GameManager.ClientMgr.SendToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strcmd, (int)TCPGameServerCmds.CMD_SPR_QUERYDAIMONSQUAREMONSTERWAVEANDPOINTRINFO);

            //string strcmd = string.Format("{0}:{1}", bcDataTmp.MonsterID.Length - bcTmp.m_nMonsterWave, client.ClientData.DaimonSquarePoint);

            //GameManager.ClientMgr.SendToClient(client, strcmd, (int)TCPGameServerCmds.CMD_SPR_QUERYDAIMONSQUAREMONSTERWAVEANDPOINTRINFO);

        }

        /// <summary>
        // 玩家离开恶魔堡垒
        /// </summary>
        static public void LeaveDaimonSquareScene(GameClient client)
        {
            if (!Global.IsDaimonSquareSceneID(client.ClientData.MapCode))
                return;

            DaimonSquareScene bcTmp = GetDaimonSquareListScenes(client.ClientData.MapCode);
            if (bcTmp == null)
                return;

            --bcTmp.m_nPlarerCount;

            // 离开时 保存积分
            Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.DaimonSquarePlayerPoint, client.ClientData.DaimonSquarePoint, true);

            // 看看要不要给奖励
            //GiveAwardDaimonSquareScene(client);

            client.ClientData.bIsInDaimonSquareMap = false;

            return;
        }

        /// <summary>
        // 完成恶魔广场
        /// </summary>
        static public void CompleteDaimonSquareScene(GameClient client, DaimonSquareScene bsInfo, DaimonSquareDataInfo dsData)
        {
            int nFlag = Global.GetRoleParamsInt32FromDB(client, RoleParamName.DaimonSquareSceneFinishFlag);

            if (nFlag != 1)
            {
                // 保存完成状态
                Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.DaimonSquareSceneFinishFlag, 1, true);

                // 剩余时间奖励
                long nTimer = bsInfo.m_lEndTime - bsInfo.m_lBeginTime;
                long nRemain = ((dsData.DurationTime * 1000) - nTimer) / 1000;

                Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.DaimonSquareSceneTimer, (int)nRemain, true);
            }
        }

        /// <summary>
        // 清除
        /// </summary>
        static public void CleanDaimonSquareScene(int mapid)
        {
            // 首先是动态刷出的怪
            for (int i = 0; i < m_DaimonSquareListScenes[mapid].m_nDynamicMonsterList.Count; i++)
            {
                Monster monsterInfo = m_DaimonSquareListScenes[mapid].m_nDynamicMonsterList[i];
                if (monsterInfo != null)
                {
                    GameManager.MonsterMgr.AddDelayDeadMonster(monsterInfo);
                }
            }

            return;
        }

    }
}
