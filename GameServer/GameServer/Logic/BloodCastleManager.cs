using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Server;
using System.Xml.Linq;
using Server.Data;
using System.Windows;
using Server.Tools;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using Server.Protocol;
using System.Threading;

namespace GameServer.Logic
{
    // 血色城堡管理类 [11/5/2013 LiaoWei]   说明 - 给客户端显示的刷怪状态消息里 1击杀吊桥怪物 2击杀吊桥 3击杀巫师 4击碎灵棺 5天使武器
    public class BloodCastleManager
    {
        /// <summary>
        /// 血色城堡场景
        /// </summary>
        public static Dictionary<int, BloodCastleScene> m_BloodCastleListScenes = new Dictionary<int, BloodCastleScene>();

        /// <summary>
        /// 最高积分 -- 分数
        /// </summary>
        public static int m_nTotalPointValue = -1;

        /// <summary>
        /// 最高积分 -- 人名
        /// /// </summary>
        public static string m_sTotalPointName = "";

        /// <summary>
        /// 推送日期ID
        /// /// </summary>
        public static int m_nPushMsgDayID = -1;

        /// <summary>
        /// 加载血色城堡场景到管理器
        /// </summary>
        public void InitBloodCastle()
        {
            //LoadBloodCastleListScenes();
        }

        /// <summary>
        /// 加载血色城堡场景到管理器
        /// </summary>
        public void LoadBloodCastleListScenes()
        {
            int[] nArray = GameManager.systemParamsList.GetParamValueIntArrayByName("BloodCastleMapCodeID");

            for (int i = 0; i < nArray.Length; ++i)
            {
                BloodCastleScene tmpMap = new BloodCastleScene();
                tmpMap.CleanAllInfo();

                tmpMap.m_nMapCode = nArray[i];

                AddBloodCastleListScenes(nArray[i], tmpMap);
            }

            // 向DB请求最高积分信息
            Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.BloodCastle);

            m_nPushMsgDayID = Global.SafeConvertToInt32(GameManager.GameConfigMgr.GetGameConifgItem(GameConfigNames.BloodCastlePushMsgDayID));

        }

        /// <summary>
        /// 添加一个场景
        /// </summary>
        public void AddBloodCastleListScenes(int nMap, BloodCastleScene bcScene)
        {
            lock (m_BloodCastleListScenes)
            {
                m_BloodCastleListScenes.Add(nMap, bcScene);
            }
        }

        /// <summary>
        /// 删除一个场景
        /// </summary>
        public void RemoveBloodCastleListScenes(int nMap)
        {
            lock (m_BloodCastleListScenes)
            {
                m_BloodCastleListScenes.Remove(nMap);
            }
        }

        /// <summary>
        /// 取得基本信息
        /// </summary>
        static public BloodCastleScene GetBloodCastleListScenes(int nMap)
        {
            return m_BloodCastleListScenes[nMap];
        }

        /// <summary>
        /// 管理动态刷怪列表--增加
        /// </summary>
        public void AddBloodCastleDynamicMonster(int nMap, Monster monster)
        {
            m_BloodCastleListScenes[nMap].m_nDynamicMonsterList.Add(monster);
            return ;
        }

        /// <summary>
        /// 管理动态刷怪列表--移除
        /// </summary>
        public void RemoveBloodCastleDynamicMonster(int nMap, Monster monster)
        {
            m_BloodCastleListScenes[nMap].m_nDynamicMonsterList.Remove(monster);
            return;
        }

        /// <summary>
        /// 设置最高积分信息
        /// </summary>
        public static void SetTotalPointInfo(string sName, int nPoint)
        {
            m_sTotalPointName = sName;
            m_nTotalPointValue = nPoint;
        }

        /// <summary>
        // 心跳处理
        /// </summary>
        public void HeartBeatBloodCastScene()
        {
            //int[] nArray = GameManager.systemParamsList.GetParamValueIntArrayByName("BloodCastleMapCodeID");

            foreach (var BloodCastleScenes in m_BloodCastleListScenes)
            {
                //for (int i = 0; i < nArray.Length; ++i)
                //{
                BloodCastleDataInfo bcDataTmp = Data.BloodCastleDataInfoList[BloodCastleScenes.Key];
                BloodCastleScene bcTmp = GetBloodCastleListScenes(BloodCastleScenes.Key);
                if (bcTmp == null || bcDataTmp == null)
                    continue;

                int nRoleNum = 0;
                nRoleNum = GameManager.ClientMgr.GetMapClientsCount(bcTmp.m_nMapCode);
                if (nRoleNum <= 0)
                {
                    if (bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_BEGIN)
                    {
                        // 做清空处理  比如 所有动态刷出的怪 都delete掉
                        CleanBloodCastScene(bcTmp.m_nMapCode);
                        bcTmp.CleanAllInfo();
                        bcTmp.m_nMapCode = BloodCastleScenes.Key;
                    }
                    //continue;
                }

                // 区分时段 注意 每个时段都要计时

                // 当前tick
                long ticks = DateTime.Now.Ticks / 10000;

                if (bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_NULL)             // 如果处于空状态 -- 是否要切换到准备状态
                {
                    bool bPushMsg = false;

                    if (Global.CanEnterBloodCastleOnTime(bcDataTmp.BeginTime, 0/*bcDataTmp.PrepareTime*/))
                    {
                        bcTmp.m_eStatus = BloodCastleStatus.FIGHT_STATUS_PREPARE;
                        bcTmp.m_lPrepareTime = DateTime.Now.Ticks / 10000;

                        //触发血色城堡事件
                        GlobalEventSource.getInstance().fireEvent(XueSeChengBaoBaseEventObject.CreateStatusEvent((int)bcTmp.m_eStatus));

                        // 消息推送
                        int nNow = DateTime.Now.DayOfYear;
                        
                        if (bPushMsg && m_nPushMsgDayID != nNow)
                        {
                            //Global.DayActivityTiggerPushMessage((int)SpecialActivityTypes.BloodCastle);

                            Global.UpdateDBGameConfigg(GameConfigNames.BloodCastlePushMsgDayID, nNow.ToString());

                            m_nPushMsgDayID = nNow;
                        }
                        
                    }
                }
                else if (bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_PREPARE)     // 场景战斗状态切换
                {
                    if (ticks >= (bcTmp.m_lPrepareTime + (bcDataTmp.PrepareTime * 1000)))
                    {
                        // 准备战斗 通知客户端 面前的阻挡消失 玩家可以上桥上去杀怪了
                        bcTmp.m_eStatus = BloodCastleStatus.FIGHT_STATUS_BEGIN;

                        bcTmp.m_lBeginTime = DateTime.Now.Ticks / 10000;
                        int nTimer = (int)((bcDataTmp.DurationTime*1000 - (ticks - bcTmp.m_lBeginTime)) / 1000);

                        GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                                        BloodCastleScenes.Key, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEBEGINFIGHT, nTimer); // 战斗结束倒计时

                        // 把城门刷出来
                        int monsterID = bcDataTmp.GateID;
                        string[] sfields = bcDataTmp.GatePos.Split(',');

                        int nPosX = Global.SafeConvertToInt32(sfields[0]);
                        int nPosY = Global.SafeConvertToInt32(sfields[1]);

                        GameMap gameMap = null;
                        if (!GameManager.MapMgr.DictMaps.TryGetValue(bcDataTmp.MapCode, out gameMap))
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("血色城堡报错 地图配置 ID = {0}", bcDataTmp.MapCode));
                            return;
                        }
                        int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
                        int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;

                        //System.Console.WriteLine("liaowei是帅哥  血色堡垒{0}里 刷城门怪了--1！！！", BloodCastleScenes.Key);
                        GameManager.MonsterZoneMgr.AddDynamicMonsters(BloodCastleScenes.Key, monsterID, -1, 1, gridX, gridY, 0);

                        GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                                    bcTmp.m_nMapCode, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEKILLMONSTERSTATUS, 0, 0, 1);   // 杀死桥上的怪 0/50 -- 显示杀怪状态

                        //触发血色城堡事件
                        GlobalEventSource.getInstance().fireEvent(XueSeChengBaoBaseEventObject.CreateStatusEvent((int)bcTmp.m_eStatus));

                    }
                }
                else if (bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_BEGIN)       // 战斗开始
                {
                    if (ticks >= (bcTmp.m_lBeginTime + (bcDataTmp.DurationTime * 1000)))
                    {
                        bcTmp.m_eStatus = BloodCastleStatus.FIGHT_STATUS_END;
                        bcTmp.m_lEndTime = DateTime.Now.Ticks / 10000;

                        //触发血色城堡事件
                        GlobalEventSource.getInstance().fireEvent(XueSeChengBaoBaseEventObject.CreateStatusEvent((int)bcTmp.m_eStatus));
                    }
                }
                else if (bcTmp.m_eStatus == BloodCastleStatus.FIGHT_STATUS_END)         // 战斗结束
                {
                    // 血色堡垒结束战斗  客户端显示倒计时界面
                    int nTimer = (int)((bcDataTmp.LeaveTime*1000 - (ticks - bcTmp.m_lEndTime)) / 1000);

                    if (bcTmp.m_bEndFlag == false)
                    {
                        // 剩余时间奖励
                        long nTimerInfo = 0;
                        nTimerInfo = bcTmp.m_lEndTime - bcTmp.m_lBeginTime;
                        long nRemain = 0;
                        nRemain = ((bcDataTmp.DurationTime * 1000) - nTimerInfo) / 1000;

                        if (nRemain >= bcDataTmp.DurationTime)
                            nRemain = bcDataTmp.DurationTime / 2;
                        
                        int nTimeAward = 0;
                        nTimeAward = (int)(bcDataTmp.TimeModulus * nRemain);

                        if (nTimeAward < 0)
                            nTimeAward = 0;

                        GameManager.ClientMgr.NotifyBloodCastleMsgEndFight(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                                            BloodCastleScenes.Key, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEENDFIGHT, nTimer, nTimeAward);
                    }

                    if (ticks >= (bcTmp.m_lEndTime + (bcDataTmp.LeaveTime * 1000)))
                    {
                        // 清场
                        List<Object> objsList = GameManager.ClientMgr.GetMapClients(BloodCastleScenes.Key);
                        if (objsList != null)
                        {
                            for (int n = 0; n < objsList.Count; ++n)
                            {
                                GameClient c = objsList[n] as GameClient;
                                if (c == null)
                                    continue;

                                if (c.ClientData.MapCode != BloodCastleScenes.Key)
                                    continue;

                                // 完成该血色堡垒
                                //CompleteBloodCastScene(c, bcTmp, bcDataTmp);

                                // 根据公式和积分奖励经验 注释掉
                                //GiveAwardBloodCastScene(c);

                                // 退出场景
                                int toMapCode = GameManager.MainMapCode;    //主城ID 防止意外
                                int toPosX = -1;
                                int toPosY = -1;
                                if (MapTypes.Normal == Global.GetMapType(c.ClientData.LastMapCode))
                                {
                                    if (GameManager.BattleMgr.BattleMapCode != c.ClientData.LastMapCode
                                        || GameManager.ArenaBattleMgr.BattleMapCode != c.ClientData.LastMapCode)
                                    {
                                        toMapCode = c.ClientData.LastMapCode;
                                        toPosX = c.ClientData.LastPosX;
                                        toPosY = c.ClientData.LastPosY;
                                    }
                                }

                                GameMap gameMap = null;
                                if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
                                {
                                    c.ClientData.bIsInBloodCastleMap = false;
                                    GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, c, toMapCode, toPosX, toPosY, -1);
                                }

                            }
                        }

                        CleanBloodCastScene(BloodCastleScenes.Key);
                        bcTmp.CleanAllInfo();
                        bcTmp.m_nMapCode = BloodCastleScenes.Key;
                    }
                }

                //}
            }

            return;
        }

        /// <summary>
        // 刷B怪
        /// </summary>
        public void CreateMonsterBBloodCastScene(int mapid, BloodCastleDataInfo bcDataTmp, BloodCastleScene bcTmp)
        {
            int monsterID = bcDataTmp.NeedKillMonster2ID;
            string[] sfields = bcDataTmp.NeedCreateMonster2Pos.Split(',');

            int nPosX = Global.SafeConvertToInt32(sfields[0]);
            int nPosY = Global.SafeConvertToInt32(sfields[1]);

            GameMap gameMap = null;
            if (!GameManager.MapMgr.DictMaps.TryGetValue(bcDataTmp.MapCode, out gameMap))
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("血色城堡报错 地图配置 ID = {0}", bcDataTmp.MapCode));
                return;
            }
            int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
            int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;

            int gridNum = gameMap.CorrectPointToGrid(bcDataTmp.NeedCreateMonster2Radius);

            for (int i = 0; i < bcDataTmp.NeedCreateMonster2Num; ++i)
                GameManager.MonsterZoneMgr.AddDynamicMonsters(mapid, monsterID, -1, 1, gridX, gridY, gridNum, bcDataTmp.NeedCreateMonster2PursuitRadius);
            
            return;
        }

        /// <summary>
        // 杀死了怪
        /// </summary>
        public void KillMonsterABloodCastScene(GameClient client, Monster monster)
        {
            BloodCastleDataInfo bcDataTmp = Data.BloodCastleDataInfoList[client.ClientData.MapCode];
            BloodCastleScene bcTmp = GetBloodCastleListScenes(client.ClientData.MapCode);
            if (bcTmp == null || bcDataTmp == null)
                return ;

            if (bcTmp.m_bEndFlag == true)
                return;

            // 给玩家积分
            client.ClientData.BloodCastleAwardPoint += monster.MonsterInfo.BloodCastJiFen;

            // 节省性能 当杀够1000分时 做存储
            if (client.ClientData.BloodCastleAwardPoint - client.ClientData.BloodCastleAwardPointTmp >= 1000)
            {
                client.ClientData.BloodCastleAwardPointTmp = client.ClientData.BloodCastleAwardPoint;
                Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.BloodCastlePlayerPoint, client.ClientData.BloodCastleAwardPoint, true);
            }

            string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.BloodCastleAwardPoint);

            GameManager.ClientMgr.SendToClient(client, strcmd, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLECOMBATPOINT);
            
            if (monster.MonsterInfo.VLevel >= bcDataTmp.NeedKillMonster1Level && bcTmp.m_bKillMonsterAStatus == 0)
            {
                int killedMonster = Interlocked.Increment(ref bcTmp.m_nKillMonsterACount);

                GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                               bcTmp.m_nMapCode, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEKILLMONSTERSTATUS, 0, bcTmp.m_nKillMonsterACount, 1);    // 杀死桥上的怪 数量/50 显示杀怪状态

                if (killedMonster == bcDataTmp.NeedKillMonster1Num)
                {
                    // 杀死A怪的数量已经达到限额 通知客户端 面前的阻挡消失 玩家可以离开桥 攻击城门了
                    GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                               bcTmp.m_nMapCode, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEKILLMONSTERAHASDONE);

                    GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                               bcTmp.m_nMapCode, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEKILLMONSTERSTATUS, 0, 0, 2);   // 杀死吊桥怪 0/1 -- 显示杀怪状态

                    bcTmp.m_bKillMonsterAStatus = 1;
                }
            }

            if (monster.MonsterInfo.ExtensionID == bcDataTmp.GateID)
            {
                CreateMonsterBBloodCastScene(bcTmp.m_nMapCode, bcDataTmp, bcTmp);       // 把B怪刷出来吧

                GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                               bcTmp.m_nMapCode, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEKILLMONSTERSTATUS, 0, 1, 2);   // 杀死吊桥怪 1/1 -- 显示杀怪状态

                
                GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                               bcTmp.m_nMapCode, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEKILLMONSTERSTATUS, 0, 0, 3);   // 杀死巫师怪 0/8 -- 显示杀怪状态

            }
            
            if (monster.MonsterInfo.ExtensionID == bcDataTmp.NeedKillMonster2ID && bcTmp.m_bKillMonsterBStatus == 0)
            {
                int killedMonster = Interlocked.Increment(ref bcTmp.m_nKillMonsterBCount);

                GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                               bcTmp.m_nMapCode, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEKILLMONSTERSTATUS, 0, bcTmp.m_nKillMonsterBCount, 3);   // 杀死巫师怪 数量/8 -- 显示杀怪状态

                if (killedMonster == bcDataTmp.NeedKillMonster2Num)
                {
                    // 把水晶棺刷出来
                    int monsterID = bcDataTmp.CrystalID;
                    string[] sfields = bcDataTmp.CrystalPos.Split(',');

                    int nPosX = Global.SafeConvertToInt32(sfields[0]);
                    int nPosY = Global.SafeConvertToInt32(sfields[1]);


                    GameMap gameMap = null;
                    if (!GameManager.MapMgr.DictMaps.TryGetValue(bcDataTmp.MapCode, out gameMap))
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("血色城堡报错 地图配置 ID = {0}", bcDataTmp.MapCode));
                        return;
                    }
                    int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
                    int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;

                    GameManager.MonsterZoneMgr.AddDynamicMonsters(bcTmp.m_nMapCode, monsterID, -1, 1, gridX, gridY, 0);

                    GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                               bcTmp.m_nMapCode, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEKILLMONSTERSTATUS, 0, 0, 4);   // 杀死水晶棺 0/1 -- 显示杀怪状态

                    bcTmp.m_bKillMonsterBStatus = 1;
                }
            }

            if (monster.MonsterInfo.ExtensionID == bcDataTmp.CrystalID)
            {
                GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                               bcTmp.m_nMapCode, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEKILLMONSTERSTATUS, 0, 1, 4);   // 杀死水晶棺 1/1 -- 显示杀怪状态


                GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                               bcTmp.m_nMapCode, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEKILLMONSTERSTATUS, 0, 0, 5);   // 采集雕像 0/1 -- 显示杀怪状态

            }

            if (monster.MonsterInfo.ExtensionID == bcDataTmp.DiaoXiangID)
            {
                GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                               bcTmp.m_nMapCode, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEKILLMONSTERSTATUS, 0, 1, 5);   // 采集雕像 1/1 -- 显示杀怪状态

                GameManager.ClientMgr.NotifyBloodCastleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                                               bcTmp.m_nMapCode, (int)TCPGameServerCmds.CMD_SPR_BLOODCASTLEKILLMONSTERSTATUS, 0, 1, 6);   // 提交大天使武器 1/1 -- 显示杀怪状态
            }

            // 刷雕像
            if (monster.MonsterInfo.ExtensionID == bcDataTmp.CrystalID)
            {
                int monsterID = bcDataTmp.DiaoXiangID;
                string[] sfields = bcDataTmp.DiaoXiangPos.Split(',');

                int nPosX = Global.SafeConvertToInt32(sfields[0]);
                int nPosY = Global.SafeConvertToInt32(sfields[1]);

                GameMap gameMap = null;
                if (!GameManager.MapMgr.DictMaps.TryGetValue(bcDataTmp.MapCode, out gameMap))
                {
                    return;
                }

                int gridX = gameMap.CorrectWidthPointToGridPoint(nPosX) / gameMap.MapGridWidth;
                int gridY = gameMap.CorrectHeightPointToGridPoint(nPosY) / gameMap.MapGridHeight;

                GameManager.MonsterZoneMgr.AddDynamicMonsters(bcDataTmp.MapCode, monsterID, -1, 1, gridX, gridY, 0);
            }

            return;
        }

        /// <summary>
        // 给奖励
        /// </summary>
        public void GiveAwardBloodCastScene(GameClient client)
        {
            BloodCastleDataInfo bcDataTmp = null;
            if (!Data.BloodCastleDataInfoList.TryGetValue(client.ClientData.MapCode, out bcDataTmp))  //bcDataTmp = Data.BloodCastleDataInfoList[client.ClientData.MapCode];
                return;
            
            BloodCastleScene bcTmp = null;
            bcTmp = GetBloodCastleListScenes(client.ClientData.MapCode);
            
            if (bcTmp == null || bcDataTmp == null)
                return;

            // 如果提交了任务(就是提交了水晶棺物品) 就给大家奖励
            if (bcTmp.m_bIsFinishTask == true)
            {
                string[] sItem = bcDataTmp.AwardItem2;

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
                        int nBinding = Convert.ToInt32(sFields[2].Trim());

                        GoodsData goodsData = new GoodsData(){Id = -1,GoodsID = nGoodsID,Using = 0,Forge_level = 0,Starttime = "1900-01-01 12:00:00",Endtime = Global.ConstGoodsEndTime,
                            Site = 0,Quality = (int)GoodsQuality.White,Props = "",GCount = nGoodsNum,Binding = nBinding,Jewellist = "",BagIndex = 0,AddPropIndex = 0,BornIndex = 0,
                            Lucky = 0,Strong = 0,ExcellenceInfo = 0,AppendPropLev = 0,ChangeLifeLevForEquip = 0,};
                        
                        string sMsg = "血色堡垒奖励--统一奖励";

                        if (!Global.CanAddGoodsNum(client, nGoodsNum))
                        {
                            for (int j = 0; j < nGoodsNum; ++j)
                                Global.UseMailGivePlayerAward(client, goodsData, "血色堡垒奖励", sMsg);
                        }
                        else
                            Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nGoodsID, nGoodsNum, goodsData.Quality, "", goodsData.Forge_level,
                                                        goodsData.Binding, 0, "", true, 1, sMsg, goodsData.Endtime);
                    }
                }
            }

            // 根据积分以及公式给奖励(经验)
            if (client.ClientData.BloodCastleAwardPoint > 0)
            {
                // 公式
                int nExp = client.ClientData.BloodCastleAwardPoint * bcDataTmp.ExpModulus;
                int nMoney = client.ClientData.BloodCastleAwardPoint * bcDataTmp.MoneyModulus;

                if (nExp > 0)
                    GameManager.ClientMgr.ProcessRoleExperience(client, nExp, false);

                if (nMoney > 0)
                    GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nMoney, "血色城堡奖励一", false);
                    //GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nMoney);

                // 存盘
                if (client.ClientData.BloodCastleAwardPoint > client.ClientData.BloodCastleAwardTotalPoint)
                    client.ClientData.BloodCastleAwardTotalPoint = client.ClientData.BloodCastleAwardPoint;

                if (client.ClientData.BloodCastleAwardPoint > m_nTotalPointValue)
                    SetTotalPointInfo(client.ClientData.RoleName, client.ClientData.BloodCastleAwardPoint);

                // 清空
                client.ClientData.BloodCastleAwardPoint = 0;
            }

            return;
        }

        /// <summary>
        // 给奖励 --  第二个版本
        /// </summary>
        static public void GiveAwardBloodCastScene2(GameClient client, int nMultiple)
        {
            int nSceneID = Global.GetRoleParamsInt32FromDB(client, RoleParamName.BloodCastleSceneid);
            int nFlag = Global.GetRoleParamsInt32FromDB(client, RoleParamName.BloodCastleSceneFinishFlag);

            BloodCastleDataInfo bcDataTmp = null;
            if (!Data.BloodCastleDataInfoList.TryGetValue(nSceneID, out bcDataTmp))
                return;

            // 如果提交了任务(就是提交了水晶棺物品) 就给大家奖励
            if (nFlag == 1)
            {
                string[] sItem = bcDataTmp.AwardItem2;

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
                        int nBinding = Convert.ToInt32(sFields[2].Trim());

                        GoodsData goodsData = new GoodsData() {Id = -1,GoodsID = nGoodsID,Using = 0,Forge_level = 0,Starttime = "1900-01-01 12:00:00",Endtime = Global.ConstGoodsEndTime,Site = 0,
                            Quality = (int)GoodsQuality.White,Props = "",GCount = nGoodsNum,Binding = nBinding,Jewellist = "",BagIndex = 0,AddPropIndex = 0,BornIndex = 0,Lucky = 0,Strong = 0,
                            ExcellenceInfo = 0,AppendPropLev = 0,ChangeLifeLevForEquip = 0,};

                        string sMsg = "血色堡垒奖励--统一奖励";

                        if (!Global.CanAddGoodsNum(client, nGoodsNum))
                        {
                            for (int j = 0; j < nGoodsNum; ++j)
                                Global.UseMailGivePlayerAward(client, goodsData, "血色堡垒奖励", sMsg);
                        }
                        else
                            Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nGoodsID, nGoodsNum, goodsData.Quality, "", goodsData.Forge_level,
                                                        goodsData.Binding, 0, "", true, 1, sMsg, goodsData.Endtime);
                    }
                }
            }

            // 根据积分以及公式给奖励(经验)
            if (client.ClientData.BloodCastleAwardPoint > 0)
            {
                // 公式
                int nExp = nMultiple * client.ClientData.BloodCastleAwardPoint * bcDataTmp.ExpModulus;
                int nMoney = client.ClientData.BloodCastleAwardPoint * bcDataTmp.MoneyModulus;

                if (nExp > 0)
                    GameManager.ClientMgr.ProcessRoleExperience(client, nExp, false);

                if (nMoney > 0)
                    GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nMoney, "血色城堡奖励二", false);
                //GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nMoney);

                // 存盘
                if (client.ClientData.BloodCastleAwardPoint > client.ClientData.BloodCastleAwardTotalPoint)
                    client.ClientData.BloodCastleAwardTotalPoint = client.ClientData.BloodCastleAwardPoint;

                if (client.ClientData.BloodCastleAwardPoint > m_nTotalPointValue)
                    SetTotalPointInfo(client.ClientData.RoleName, client.ClientData.BloodCastleAwardPoint);

                // 清空
                client.ClientData.BloodCastleAwardPoint = 0;
                Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.BloodCastlePlayerPoint, client.ClientData.BloodCastleAwardPoint, true);
            }

        }

        /// <summary>
        // 玩家离开血色堡垒
        /// </summary>
        public void LeaveBloodCastScene(GameClient client)
        {
            if (!Global.IsBloodCastleSceneID(client.ClientData.MapCode))
                return;

            BloodCastleScene bcTmp = GetBloodCastleListScenes(client.ClientData.MapCode);
            if (bcTmp == null)
                return;

            Interlocked.Decrement(ref bcTmp.m_nPlarerCount);

            // 离开时 保存积分
            Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.BloodCastlePlayerPoint, client.ClientData.BloodCastleAwardPoint, true);

            // 如果持有水晶棺宝物 则在离开场景时 掉落出来

            // 获取到没有装备的物品列表
            //List<GoodsData> fallGoodsList = Global.GetFallGoodsList(client);
            List<GoodsData> goodsDataList = new List<GoodsData>();
            
            GoodsData tmpFallGoods = null;
            tmpFallGoods = Global.GetGoodsByID(client, (int)BloodCastleCrystalItemID.BloodCastleCrystalItemID1);

            if (tmpFallGoods == null)
                tmpFallGoods = Global.GetGoodsByID(client, (int)BloodCastleCrystalItemID.BloodCastleCrystalItemID2);

            if (tmpFallGoods == null)
                tmpFallGoods = Global.GetGoodsByID(client, (int)BloodCastleCrystalItemID.BloodCastleCrystalItemID3);

            if (tmpFallGoods != null)
            {
                int oldGoodsNum = 1;
                if (Global.GetGoodsDefaultCount(tmpFallGoods.GoodsID) > 1)
                    oldGoodsNum = tmpFallGoods.GCount;

                if (GameManager.ClientMgr.FallRoleGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, tmpFallGoods))
                {
                    tmpFallGoods = Global.CopyGoodsData(tmpFallGoods);
                    tmpFallGoods.Id = GameManager.GoodsPackMgr.GetNextGoodsID();
                    tmpFallGoods.GCount = oldGoodsNum;
                    goodsDataList.Add(tmpFallGoods);
                }

                Point grid = client.CurrentGrid;

                List<GoodsPackItem> tempgoodsPackItem = GameManager.GoodsPackMgr.GetRoleGoodsPackItemList(client.ClientData.RoleID, Global.FormatRoleName(client, client.ClientData.RoleName),
                                                                                                            goodsDataList, client.ClientData.MapCode, client.ClientData.CopyMapID,
                                                                                                                (int)grid.X, (int)grid.Y, client.ClientData.RoleName);

                StringBuilder sInfor = new StringBuilder();

                for (int i = 0; i < tempgoodsPackItem.Count; i++)
                {
                    GameManager.GoodsPackMgr.ProcessGoodsPackItem(client, client, tempgoodsPackItem[i], 0);

                    sInfor.AppendFormat("{0}", Global.GetGoodsNameByID(tempgoodsPackItem[i].GoodsDataList[0].GoodsID));
                    if (i != tempgoodsPackItem.Count - 1)
                        sInfor.Append(" ");
                }

                GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,client,
                                                            StringUtil.substitute(Global.GetLang("很不幸，[{0}]离开血色堡垒 掉落物品{1}"), client.ClientData.RoleName, 
                                                            sInfor.ToString()),GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0);
            }

            // 看看要不要给奖励 注释掉
            //GiveAwardBloodCastScene(client);

            client.ClientData.bIsInBloodCastleMap = false;

            return;
        }

        /// <summary>
        // 完成血色堡垒
        /// </summary>
        public void CompleteBloodCastScene(GameClient client, BloodCastleScene bsInfo, BloodCastleDataInfo bsData)
        {
            int nFlag = Global.GetRoleParamsInt32FromDB(client, RoleParamName.BloodCastleSceneFinishFlag);

            if (nFlag != 1)
            {
                // 保存完成状态
                Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.BloodCastleSceneFinishFlag, 1, true);
            }
        }


        /// <summary>
        // 清除
        /// </summary>
        public void CleanBloodCastScene(int mapid)
        {
            // 首先是动态刷出的怪
            for (int i = 0; i < m_BloodCastleListScenes[mapid].m_nDynamicMonsterList.Count; i++)
            {
                Monster monsterInfo = m_BloodCastleListScenes[mapid].m_nDynamicMonsterList[i];
                if (monsterInfo != null)
                {
                    GameManager.MonsterMgr.AddDelayDeadMonster(monsterInfo);
                    //RemoveBloodCastleDynamicMonster(mapid, monsterInfo);
                }
            }

            return;
        }
    
    }

}
