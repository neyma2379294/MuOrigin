using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Interface;
using GameServer.Server;
using System.Windows;
using Server.Tools;
using ProtoBuf;
namespace GameServer.Logic
{
    /// <summary>
    /// 昨天之前（包括昨天）的采集数据
    /// </summary>
    [ProtoContract]
    public class OldCaiJiData
    {
        /// <summary>
        /// 环的ID
        /// </summary>
        [ProtoMember(1)]
        public int OldDay = -1;

        /// <summary>
        /// 跑环的日子
        /// </summary>
        [ProtoMember(2)]
        public int OldNum = -1;
    }

    public class CaiJiLogic
    {
        public static DateTimeRange DoubleAwardTimeRange = new DateTimeRange(); //双倍时间段
        public static DateTimeRange[] dateTimeRangeArray = new DateTimeRange[1];
        public static float DoubleAwardRate = 1.0f;     //双倍奖励倍率
        public static int DailyNum = 0;                //每天能做多少次
        public static int DeadReliveTime = 0;           //复活等待多少秒
        public static bool LoadConfig()
        {
            DailyNum = (int)GameManager.systemParamsList.GetParamValueIntByName("MuKuangNum");
            DeadReliveTime = (int)GameManager.systemParamsList.GetParamValueIntByName("CrystalDeadTime");
            List<string> doubleAwardParams = GameManager.systemParamsList.GetParamValueStringListByName("MuKuangDoubleAward");
            if (3 != doubleAwardParams.Count)
                return false;

            string startTime = doubleAwardParams[0];
            string[] temp = startTime.Split(':');
            DoubleAwardTimeRange.FromHour = int.Parse(temp[0]);
            DoubleAwardTimeRange.FromMinute = int.Parse(temp[1]);

            string endTime = doubleAwardParams[1];
            temp = endTime.Split(':');
            DoubleAwardTimeRange.EndHour = int.Parse(temp[0]);
            DoubleAwardTimeRange.EndMinute = int.Parse(temp[1]);

            DoubleAwardRate = float.Parse(doubleAwardParams[2]);

            dateTimeRangeArray[0] = DoubleAwardTimeRange;
            return true;
        }

        /// <summary>
        /// 请求开始采集
        /// </summary>
        public static int ReqStartCaiJi(GameClient client, int monsterId, out int GatherTime)
        {
            GatherTime = 0;

            CaiJiLogic.CancelCaiJiState(client);

            //判断玩家当前状态能否采集
            //死亡时不能采集
            if (client.ClientData.CurrentLifeV <= 0)
            {
                //取消采集状态
                CaiJiLogic.CancelCaiJiState(client);
                return -3;
            }

            //判断采集物是否存在
            Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, monsterId);
            if (null == monster)    //找不到采集物
            {
                return -1;
            }
            
            //判断是不是采集物
            if (monster.MonsterType != (int)MonsterTypes.CaiJi)    //不是采集物
            {
                return -4;
            }

            SystemXmlItem CaiJiMonsterXmlItem = null;
            if (!GameManager.systemCaiJiMonsterMgr.SystemXmlItemDict.TryGetValue(monster.MonsterInfo.ExtensionID, out CaiJiMonsterXmlItem) || null == CaiJiMonsterXmlItem)
            {
                return -4;  //传来的monsterID不对
            }

            GatherTime = CaiJiMonsterXmlItem.GetIntValue("GatherTime");

            //判断玩家与采集物的距离
            if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 400)     //判断距离
            {
                return -2;
            }

            if (monster.IsCollected)
            {
                return -4;  //已经被采集过了
            }

            //刷新活动中与采集相关的信息
            if (client.ClientData.DailyCrystalCollectNum >= DailyNum)
            {
                return -5;  //已经达到次数上限
            }

            //结束冥想
            Global.EndMeditate(client);

            //设置玩家的采集状态
            SetCaiJiState(client, monsterId);

            return 0;   //采集开始
        }

        /// <summary>
        /// 请求完成采集
        /// </summary>
        public static int ReqFinishCaiJi(GameClient client, int monsterId)
        {
            //判断玩家当前状态能否采集
            if (monsterId != client.ClientData.CaijTargetId || client.ClientData.CaiJiStartTick == 0 || client.ClientData.CaijTargetId == 0)
            {
                //取消采集状态
                CaiJiLogic.CancelCaiJiState(client);
                return -3;
            }

            //死亡时不能采集
            if (client.ClientData.CurrentLifeV <= 0)
            {
                //取消采集状态
                CaiJiLogic.CancelCaiJiState(client);
                return -3;
            }

            //刷新活动中与采集相关的信息
            if (client.ClientData.DailyCrystalCollectNum >= DailyNum)
            {
                CaiJiLogic.CancelCaiJiState(client);
                return -6;  //已经达到次数上限
            }

            //判断采集物是否存在
            Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, monsterId);
            if (null == monster)    //找不到采集物
            {
                //取消采集状态
                CaiJiLogic.CancelCaiJiState(client);
                return -1;
            }

            //判断是不是采集物
            if (monster.MonsterType != (int)MonsterTypes.CaiJi)    //不是采集物
            {
                //取消采集状态
                CaiJiLogic.CancelCaiJiState(client);
                return -4;
            }

            SystemXmlItem CaiJiMonsterXmlItem = null;
            if (!GameManager.systemCaiJiMonsterMgr.SystemXmlItemDict.TryGetValue(monster.MonsterInfo.ExtensionID, out CaiJiMonsterXmlItem) || null == CaiJiMonsterXmlItem)
            {
                //取消采集状态
                CaiJiLogic.CancelCaiJiState(client);
                return -4;  //传来的monsterID不对
            }

             //读条时间
            long GatherTime = (long)CaiJiMonsterXmlItem.GetIntValue("GatherTime");

            //读条时间不对不能采集
            if (DateTime.Now.Ticks - client.ClientData.CaiJiStartTick < GatherTime * 1000 * 10000)
            {
                //取消采集状态
                CaiJiLogic.CancelCaiJiState(client);
                return -5;
            }

            CaiJiLogic.CancelCaiJiState(client);

            //判断玩家与采集物的距离
            if (Global.GetTwoPointDistance(client.CurrentPos, monster.CurrentPos) > 400)    //判断距离
            {
                return -2;  //太远了
            }

            lock (monster.CaiJiStateLock)
            {
                if (monster.IsCollected)
                {
                    return -4;  //已经被别人采了
                }
                else
                    monster.IsCollected = true;
            }

            //增加采集次数
            UpdateCaiJiData(client);
            //通知客户端采集次数
            NotifyCollectLastNum(client, 0, DailyNum - client.ClientData.DailyCrystalCollectNum);

            //清除采集物
            GameManager.MonsterMgr.DeadMonsterImmediately(monster);

            //给予采集奖励
            float AwardRate = 1.0f;
            //判断是否有双倍
            int endMinute = 0;
            if (Global.JugeDateTimeInTimeRange(DateTime.Now, dateTimeRangeArray, out endMinute))
            {
                AwardRate = DoubleAwardRate;
            }

            int ExpAward = (int)(AwardRate * CaiJiMonsterXmlItem.GetIntValue("ExpAward"));
            int XingHunAward = (int)(AwardRate * CaiJiMonsterXmlItem.GetIntValue("XingHunAward"));
            int BindZuanShiAward = (int)(AwardRate * CaiJiMonsterXmlItem.GetIntValue("BindZuanShiAward"));
            int BindJinBiAward = (int)(AwardRate * CaiJiMonsterXmlItem.GetIntValue("BindJinBiAward"));
            int MoJingAward = (int)(AwardRate * CaiJiMonsterXmlItem.GetIntValue("MoJingAward"));

            if (ExpAward > 0)
            {
                //处理角色经验
                GameManager.ClientMgr.ProcessRoleExperience(client, ExpAward, true, true);
            }

            if (XingHunAward > 0)
            {
                GameManager.ClientMgr.ModifyStarSoulValue(client, XingHunAward, "采集获得星魂", true, true);
            }

            if (BindZuanShiAward > 0)
            {
                GameManager.ClientMgr.AddUserGold(client, BindZuanShiAward, "采集获得绑钻");
            }

            if (BindJinBiAward > 0)
            {
                GameManager.ClientMgr.AddMoney1(client, BindJinBiAward, "采集获得绑金", true);
            }

            if (MoJingAward > 0)
            {
                GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, MoJingAward, "采集获得魔晶", true);
            }

            /*
            Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, caiJiRoleID);
            if (null != monster && monster.MonsterType == (int)MonsterTypes.CaiJi)
            {
                //首先判断背包是否已经满了，如果是则提示用户采集失败
                if (monster.MonsterInfo.FallGoodsPackID <= 0)
                {
                    if (!Global.CanAddGoodsNum(client, 1))
                    {
                        GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, StringUtil.substitute(Global.GetLang("背包已满，无法将进行采集")),
                            GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);
                    }
                }

                //杀死怪物并处理任务
                Global.SystemKillMonster(client, monster);
            }
            */

            return 0;   //采集完成
        }

        /// <summary>
        /// 取消玩家的采集状态
        /// </summary>
        public static int CancelCaiJiState(GameClient client)
        {
            if (null != client)
            {
                client.ClientData.CaiJiStartTick = 0;
                client.ClientData.CaijTargetId = 0;
            }
            return 0;
        }

        /// <summary>
        /// 设置玩家的采集状态
        /// </summary>
        public static int SetCaiJiState(GameClient client, int monsterId)
        {
            if (null != client)
            {
                client.ClientData.CaiJiStartTick = DateTime.Now.Ticks;
                client.ClientData.CaijTargetId = monsterId;
            }
            return 0;
        }

        /// <summary>
        /// 通知采集次数
        /// </summary>
        public static int NotifyCollectLastNum(GameClient client, int HuodongType, int lastnum)
        {
            string strcmd = string.Format("{0}:{1}:{2}", 0, HuodongType, lastnum);
            client.sendCmd((int)TCPGameServerCmds.CMD_SPR_CAIJI_LASTNUM, strcmd);
            return 0;
        }

        /// <summary>
        /// 请求采集剩余次数
        /// </summary>
        public static int ReqCaiJiLastNum(GameClient client, int huodongType, out int lastnum)
        {
            lastnum = 0;
            if (0 == huodongType)
            {
                lastnum = DailyNum - client.ClientData.DailyCrystalCollectNum;
                return 0;
            }
            else
                return -1;
        }

        /// <summary>
        /// 处理每日采集水晶
        /// </summary>
        /// <returns></returns>
        public static void UpdateCaiJiData(GameClient client)
        {
	        client.ClientData.DailyCrystalCollectNum++;
            client._IconStateMgr.CheckCaiJiState(client);
            Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.CaiJiCrystalNum, client.ClientData.DailyCrystalCollectNum, true);
            if (0 == client.ClientData.CrystalCollectDayID) //此角色第一次采集
            {
                client.ClientData.CrystalCollectDayID = (int)DateTime.Now.DayOfYear;
                Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.CaiJiCrystalDayID, client.ClientData.CrystalCollectDayID, true);
            }
        }

        //登录时处理采集数据
        public static void InitRoleDailyCaiJiData(GameClient client, bool isLogin, bool isNewday)
        {
            //是否开启了水晶幻境采集
            if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.CrystalCollect))
                return;

            //登录时需要取得上次采集的日期和次数，在线跨天就不需要取
            if (isLogin)
            {
                client.ClientData.DailyCrystalCollectNum = Global.GetRoleParamsInt32FromDB(client, RoleParamName.CaiJiCrystalNum);
                client.ClientData.CrystalCollectDayID = Global.GetRoleParamsInt32FromDB(client, RoleParamName.CaiJiCrystalDayID);
            }

            bool bClear = false;
            //是否跨天了（今天第一次登录或者在线跨天）
            if (isNewday)
            {
                //昨天或昨天之前是否开启了水晶幻境采集
                if (client.ClientData.DailyCrystalCollectNum >= 0 && client.ClientData.CrystalCollectDayID > 0)
                {
                    //这个数据是水晶幻境资源找回用的
                    client.ClientData.OldCrystalCollectData = new OldCaiJiData();
                    client.ClientData.OldCrystalCollectData.OldDay = client.ClientData.CrystalCollectDayID;
                    client.ClientData.OldCrystalCollectData.OldNum = client.ClientData.DailyCrystalCollectNum;
                }

                //跨天清零次数
                bClear = true;
            }
            else if (0 == client.ClientData.CrystalCollectDayID)   
            {
                //之前没保存过采集次数，现在需要保存一下
                bClear = true;
            }

            if (bClear)
            {
                //清次数，重设日期
                client.ClientData.DailyCrystalCollectNum = 0;
                client.ClientData.CrystalCollectDayID = (int)DateTime.Now.DayOfYear;

                Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.CaiJiCrystalNum, 0, true);
                Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.CaiJiCrystalDayID, client.ClientData.CrystalCollectDayID, true);

                //通知玩家水晶幻境的采集次数清零
                if (Global.GetMapSceneType(client.ClientData.MapCode) == SceneUIClasses.ShuiJingHuanJing)
                    NotifyCollectLastNum(client, 0, DailyNum);
            }

            client._IconStateMgr.CheckCaiJiState(client);
        }

        public static bool HasLeftnum(GameClient client)
        {
            if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.CrystalCollect))
                return false;
            else
                return client.ClientData.DailyCrystalCollectNum < DailyNum;
        }
    }
}
