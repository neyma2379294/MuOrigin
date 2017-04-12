using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using GameDBServer.DB.DBController;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;
using GameDBServer.Server.CmdProcessor;
using GameDBServer.Server;
using GameDBServer.Core.Executor;
using GameDBServer.Core;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 竞技场静态常量配置
    /// </summary>
    public class JingJiChangConstants
    {
        /// <summary>
        /// 当前数据版本号
        /// 此版本号改变时，一定要修改RobotData.convertBinary和RobotData.convertObject
        /// </summary>
        public static readonly int Current_Data_Version = 0;

        /// <summary>
        /// 战报类型：挑战成功
        /// </summary>
        public static readonly int ChallengeInfoType_Challenge_Win = 0;

        /// <summary>
        /// 战报类型：挑战失败
        /// </summary>
        public static readonly int ChallengeInfoType_Challenge_Failed = 1;

        /// <summary>
        /// 战报类型：被挑战成功
        /// </summary>
        public static readonly int ChallengeInfoType_Be_Challenge_Win = 2;

        /// <summary>
        /// 战报类型：被挑战失败
        /// </summary>
        public static readonly int ChallengeInfoType_Be_Challenge_Failed = 3;

        /// <summary>
        /// 战报类型：连胜
        /// </summary>
        public static readonly int ChallengeInfoType_LianSheng = 4;

        /// <summary>
        /// 战报每页显示数量
        /// </summary>
        public static readonly int ChallengeInfo_PageShowNum = 20;

        /// <summary>
        /// 战报最大页数
        /// </summary>
        public static readonly int ChallengeInfo_Max_PageIndex = ChallengeInfo_Max_Num % ChallengeInfo_PageShowNum == 0 ? ChallengeInfo_Max_Num / ChallengeInfo_PageShowNum : (ChallengeInfo_Max_Num / ChallengeInfo_PageShowNum) + 1;

        /// <summary>
        /// 战报最大缓存数量
        /// </summary>
        public static readonly int ChallengeInfo_Max_Num = 50;

        /// <summary>
        /// 排行榜最大缓存数量
        /// </summary>
        public static readonly int RankingList_Max_Num = 500;

        /// <summary>
        /// 排行榜每页显示数量
        /// </summary>
        public static readonly int RankingList_PageShowNum = 100;//20;

        /// <summary>
        /// 排行榜自大页数
        /// </summary>
        public static readonly int RankingList_Max_PageIndex = RankingList_Max_Num % RankingList_PageShowNum == 0 ? RankingList_Max_Num / RankingList_PageShowNum : (RankingList_Max_Num % RankingList_PageShowNum) + 1;
    }

    /// <summary>
    /// 补挑战次数
    /// </summary>
    public class BeChallengerCount
    {
        public int nBeChallengerCount;
    }

    /// <summary>
    /// 竞技场全局管理器
    /// </summary>
    public class JingJiChangManager : JingJiChangConstants, IManager
    {
        private static JingJiChangManager instance = new JingJiChangManager();

        /// <summary>
        /// 排行榜
        /// </summary>
        private List<PlayerJingJiRankingData> rankingDatas = new List<PlayerJingJiRankingData>();

        /// <summary>
        /// 被挑战者缓存<roleId，机器人数据>
        /// </summary>
        private Dictionary<int, PlayerJingJiData> playerJingJiDatas = new Dictionary<int, PlayerJingJiData>();

        /// <summary>
        /// 锁定数据，当机器人被挑战时，放入此集合锁定 <roleId，机器人数据>
        /// </summary>
        private Dictionary<int, BeChallengerCount> lockPlayerJingJiDatas = new Dictionary<int, BeChallengerCount>();

        /// <summary>
        /// 修改排行榜数据锁
        /// </summary>
        private Object changeRankingLock = new Object();

        /// <summary>
        /// 竞技场战报数据缓存<roleId，个人战报数据>
        /// </summary>
        private Dictionary<int, List<JingJiChallengeInfoData>> challengeInfos = new Dictionary<int, List<JingJiChallengeInfoData>>();

        private JingJiChangManager() { }

        public static JingJiChangManager getInstance()
        {
            return instance;
        }

        public bool initialize()
        {
            initCmdProcessor();
            initData();
            initListener();

            return true;
        }

        /// <summary>
        /// 初始化指令
        /// </summary>
        private void initCmdProcessor()
        {
            //获取竞技场数据
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_GET_DATA, JingJiGetDataCmdProcessor.getInstance());
            //获取竞技场挑战数据
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_GET_CHALLENGE_DATA, JingJiGetChallengeDataCmdProcessor.getInstance());
            //创建竞技场数据
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_CREATE_DATA, JingJiCreateDataCmdProcessor.getInstance());
            //请求挑战
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_REQUEST_CHALLENGE, JingJiRequestChallengeCmdProcessor.getInstance());
            //挑战结束
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_CHALLENGE_END, JingJiChallengeEndCmdProcessor.getInstance());
            //保存数据
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_SAVE_DATA, JingJiSaveDataCmdProcessor.getInstance());
            //获取竞技场战报数据
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_ZHANBAO_DATA, JingJiGetChallengeInfoDataCmdProcessor.getInstance());
            //消除挑战CD
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_REMOVE_CD, JingJiRemoveCDCmdProcessor.getInstance());
            //获取竞技场排名和上次领取奖励时间指令
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_GET_RANKING_AND_NEXTREWARDTIME, JingJiGetRankingAndRewardTimeCmdProcessor.getInstance());
            //更新下次领取竞技场排行榜奖励时间
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_UPDATE_NEXTREWARDTIME, JingJiUpdateNextRewardTimeCmdProcessor.getInstance());
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void initData()
        {
            //初始化竞技场机器人数据，排行榜数据
            List<PlayerJingJiData> playerJingJiDataList = JingJiChangDBController.getInstance().getPlayerJingJiDataList();

            if (null == playerJingJiDataList)
                return;

            foreach (PlayerJingJiData data in playerJingJiDataList)
            {
                data.convertObject();
                playerJingJiDatas.Add(data.roleId, data);
                rankingDatas.Add(data.getPlayerJingJiRankingData());
            }

        }

        /// <summary>
        /// 初始化监听器
        /// </summary>
        private void initListener()
        {
            GlobalEventSource.getInstance().registerListener((int)EventTypes.PlayerLogin,JingJiChangPlayerLoginEventListener.getInstnace());
            GlobalEventSource.getInstance().registerListener((int)EventTypes.PlayerLogout,JingJiChangPlayerLogoutEventListener.getInstnace());
        }

        /// <summary>
        /// 删除监听器
        /// </summary>
        private void removeListener()
        {
            GlobalEventSource.getInstance().removeListener((int)EventTypes.PlayerLogin,JingJiChangPlayerLoginEventListener.getInstnace());
            GlobalEventSource.getInstance().removeListener((int)EventTypes.PlayerLogout,JingJiChangPlayerLogoutEventListener.getInstnace());
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        private void removeData()
        {
            if (null != playerJingJiDatas)
            {
                playerJingJiDatas.Clear();
            }

            playerJingJiDatas = null;

            if (null != rankingDatas)
            {
                rankingDatas.Clear();
            }

            rankingDatas = null;

            if (null != lockPlayerJingJiDatas)
            {
                lockPlayerJingJiDatas.Clear();
            }

            lockPlayerJingJiDatas = null;

            if (null != challengeInfos)
            {
                challengeInfos.Clear();
            }

            challengeInfos = null;
        }

        public bool startup()
        {
            return true;
        }

        public bool showdown()
        {
            return true;
        }

        public bool destroy()
        {
            removeListener();
            removeData();
            return true;
        }

        /// <summary>
        /// 创建竞技场数据
        /// 用于玩家到达一定等级开启竞技场功能时，初始化竞技场数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool createRobotData(PlayerJingJiData data)
        {
            lock (changeRankingLock)
            {
                if (playerJingJiDatas.ContainsKey(data.roleId))
                    return false;

                data.isOnline = true;
                playerJingJiDatas.Add(data.roleId, data);
                challengeInfos.Add(data.roleId, new List<JingJiChallengeInfoData>());
            
                if (rankingDatas.Count >= RankingList_Max_Num)
                {
                    data.ranking = -1;
                }
                else
                {
                    data.ranking = rankingDatas.Count + 1;
                    rankingDatas.Add(data.getPlayerJingJiRankingData());
                }
            }

            return JingJiChangDBController.getInstance().insertJingJiData(data);
        }

        /// <summary>
        /// 玩家上线初始化时获取玩家竞技场数据、战报数据
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public void onPlayerLogin(int roleId)
        {
            PlayerJingJiData data = null;

            lock (changeRankingLock)
            {
                if (playerJingJiDatas.TryGetValue(roleId, out data))
                {
                    data.isOnline = true;
                }
                else
                {
                    data = JingJiChangDBController.getInstance().getPlayerJingJiDataById(roleId);

                    if (null != data)
                    {
                        data.convertObject();

                        data.isOnline = true;
                        playerJingJiDatas.Add(data.roleId, data);
                    }
                }

                if (null != data)
                {
                    List<JingJiChallengeInfoData> zhanBaoList = null;
                    if (!challengeInfos.TryGetValue(roleId, out zhanBaoList))
                    {
                        zhanBaoList = JingJiChangZhaoBaoDBController.getInstnace().getChallengeInfoListByRoleId(roleId);

                        if (null == zhanBaoList)
                        {
                            zhanBaoList = new List<JingJiChallengeInfoData>();
                        }

                        challengeInfos.Add(roleId, zhanBaoList);
                    }
                }
            }            
        }

        /// <summary>
        /// 玩家下线时清除玩家竞技场数据、战报数据
        /// </summary>
        /// <param name="roleId"></param>
        public void onPlayerLogout(int roleId)
        {
            PlayerJingJiData data = null;

            lock (changeRankingLock)
            {
                playerJingJiDatas.TryGetValue(roleId, out data);

                if (null != data)
                {
                    data.isOnline = false;

                    //大于500名 并且没有被别的玩家挑战
                    if (data.ranking == -1 && !lockPlayerJingJiDatas.ContainsKey(data.roleId))
                    {
                        playerJingJiDatas.Remove(data.roleId);
                    }

                }

                //清除战报数据
                challengeInfos.Remove(roleId);
            }

            
        }

        /// <summary>
        /// 获取玩家排名
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public void getRankingAndNextRewardTimeById(int roleId, out int ranking, out long nextRewardTime)
        {
            ranking = -2;
            nextRewardTime = 0;

            PlayerJingJiData data = getPlayerJingJiDataById(roleId);
            
            if (null != data)
            {
                ranking = data.ranking;
                nextRewardTime = data.nextRewardTime;
            }
        }

        /// <summary>
        /// 更新下次领取奖励时间
        /// </summary>
        public bool updateNextRewardTime(int roleId, long nextRewardTime)
        {
            PlayerJingJiData data = null;

            lock (changeRankingLock)
            {
                playerJingJiDatas.TryGetValue(roleId, out data);
            }

            if (null != data)
            {
                data.nextRewardTime = nextRewardTime;
                return JingJiChangDBController.getInstance().updateNextRewardTime(roleId, nextRewardTime);
            }

            return false;
        }

        /// <summary>
        /// 获取玩家竞技场数据
        /// </summary>
        /// <returns></returns>
        public PlayerJingJiData getPlayerJingJiDataById(int roleId)
        {
            PlayerJingJiData robotData = null;

            lock (changeRankingLock)
            {
                playerJingJiDatas.TryGetValue(roleId, out robotData);
            }

            return robotData;
        }

        /// <summary>
        /// 消除挑战CD
        /// </summary>
        public bool removeCD(int roleId)
        {
            PlayerJingJiData data = null;

            lock (changeRankingLock)
            {
                playerJingJiDatas.TryGetValue(roleId, out data);
            }

            if (null != data)
            {
                //更新内存
                data.nextChallengeTime = 0;
            }

            //更新数据库
            return JingJiChangDBController.getInstance().updateNextChallengeTime(roleId, 0);
        }

        /// <summary>
        /// 请求挑战
        /// </summary>
        /// <param name="beChallengeId">被挑战者ID</param>
        /// <param name="beChallengeRanking">被挑战者排名</param>
        /// <returns>1:请求成功，0：非法参数,-1:冷却时间未到，-2：被挑战机器人不存在,-3:被挑战机器人排名已更改,-4:正在被其他玩家挑战</returns>
        public JingJiBeChallengeData requestChallenge(int challengerId, int beChallengerId, int beChallengerRanking)
        {
            JingJiBeChallengeData data = new JingJiBeChallengeData();

            PlayerJingJiData challengerData = null;

            lock (changeRankingLock)
            {
                if (!playerJingJiDatas.TryGetValue(challengerId, out challengerData))
                {
                    data.state = 0;
                    return data;
                }

                //冷却时间未到
                if (TimeUtil.NOW() < challengerData.nextChallengeTime)
                {
                    data.state = -1;
                    return data;
                }

                PlayerJingJiRankingData rankingData = null;
                int bakBeChallengerRanking = beChallengerRanking;
                // int bakBeChallengerRanking = beChallengerData.ranking;
                if (bakBeChallengerRanking > rankingDatas.Count || bakBeChallengerRanking < 1)
                {
                    //被挑战机器人不存在
                    data.state = -2;
                    return data;
                }

                //lock (rankingDatas)
                {
                    rankingData = rankingDatas[bakBeChallengerRanking - 1];
                }

                PlayerJingJiData beChallengerData = null;
                if (!playerJingJiDatas.TryGetValue(rankingData.roleId, out beChallengerData))
                {
                    data.state = 0;
                    return data;
                }

                if (challengerId == rankingData.roleId)
                {
                    //不能挑战自己
                    data.state = -3;
                    return data;
                }

                //if (beChallengerId != rankingData.roleId)
                //{
                //    //被挑战机器人排名已更改
                //    data.state = -3;
                //    return data;
                //}
        

                //lock (lockPlayerJingJiDatas)
                //{
                //    if (lockPlayerJingJiDatas.TryGetValue(beChallengerId, out beChallengerData))
                //    {
                //        //正在被其他玩家挑战
                //        data.state = -4;
                //        return data;
                //    }

                //    //获取被挑战者数据
                //    playerJingJiDatas.TryGetValue(beChallengerId, out beChallengerData);

                //    //锁定
                //    lockPlayerJingJiDatas.Add(beChallengerData.roleId, beChallengerData);
                //}

                BeChallengerCount beChallengerCount = null;
                lockPlayerJingJiDatas.TryGetValue(beChallengerData.roleId, out beChallengerCount);
                if (null == beChallengerCount)
                {
                    beChallengerCount = new BeChallengerCount();
                    beChallengerCount.nBeChallengerCount = 1;
                    lockPlayerJingJiDatas.Add(beChallengerData.roleId, beChallengerCount);
                }
                else
                {
                    beChallengerCount.nBeChallengerCount += 1;
                }

                data.state = 1;
                data.beChallengerData = beChallengerData;

                return data;
            }
        }

        /// <summary>
        /// 挑战结束处理
        /// </summary>
        public int onChallengeEnd(JingJiChallengeResultData result)
        {
            PlayerJingJiData challenger = null;
            PlayerJingJiData beChallenger = null;

            //获取数据
            lock (changeRankingLock)
            {
                playerJingJiDatas.TryGetValue(result.playerId, out challenger);
                playerJingJiDatas.TryGetValue(result.robotId, out beChallenger);

                BeChallengerCount beChallengerCount = null;
                lockPlayerJingJiDatas.TryGetValue(result.robotId, out beChallengerCount);
                if (null != beChallengerCount)
                {
                    beChallengerCount.nBeChallengerCount -= 1;
                }

                //胜利
                if (result.isWin)
                {
                    // lock (rankingDatas)
                    {
                        //记录挑战前排行
                        int playerRanking = challenger.ranking;
                        int robotRanking = beChallenger.ranking;

                        // 被挑战者500名以后或排名相等，直接返回
                        if (robotRanking < 1 || playerRanking == robotRanking)
                        {
                            return challenger.ranking;
                        }
                        //挑战者500名以后
                        else if (playerRanking == -1)
                        {
                            //排行互换
                            challenger.ranking = robotRanking;
                            beChallenger.ranking = playerRanking;

                            //被挑战者排名500名以后，被踢出排行榜
                            rankingDatas.Remove(beChallenger.getPlayerJingJiRankingData());

                            //将挑战者加入排行榜
                            rankingDatas.Add(challenger.getPlayerJingJiRankingData());

                            rankingDatas.Sort();

                            JingJiChangDBController.getInstance().updateJingJiRanking(challenger.roleId, challenger.ranking);
                            JingJiChangDBController.getInstance().updateJingJiRanking(beChallenger.roleId, beChallenger.ranking);
                        }
                        //双方都在榜上
                        else if (playerRanking > robotRanking)
                        {
                            //排行互换
                            challenger.ranking = robotRanking;
                            beChallenger.ranking = playerRanking;

                            beChallenger.getPlayerJingJiRankingData();
                            challenger.getPlayerJingJiRankingData();

                            rankingDatas.Sort();

                            JingJiChangDBController.getInstance().updateJingJiRanking(challenger.roleId, challenger.ranking);
                            JingJiChangDBController.getInstance().updateJingJiRanking(beChallenger.roleId, beChallenger.ranking);
                        }

                    }

                }

                return challenger.ranking;
            }            
        }

        /// <summary>
        /// 创建挑战者胜利竞技场战报
        /// </summary>
        /// <param name="challengePlayer"></param>
        /// <param name="beChallengePlayer"></param>
        /// <param name="playerZhanBaoData"></param>
        /// <param name="robotZhanBaoData"></param>
        private void createChallengeWinChallengeInfoData(PlayerJingJiData challengePlayer, PlayerJingJiData beChallengePlayer, out JingJiChallengeInfoData playerZhanBaoData, out JingJiChallengeInfoData robotZhanBaoData)
        {
            //创建挑战者竞技场战报
            playerZhanBaoData = new JingJiChallengeInfoData();

            playerZhanBaoData.roleId = challengePlayer.roleId;
            playerZhanBaoData.challengeName = beChallengePlayer.roleName;
            playerZhanBaoData.zhanbaoType = ChallengeInfoType_Challenge_Win;
            playerZhanBaoData.value = challengePlayer.ranking;
            playerZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //创建被挑战者竞技场战报
            robotZhanBaoData = new JingJiChallengeInfoData();
            robotZhanBaoData.roleId = beChallengePlayer.roleId;
            robotZhanBaoData.challengeName = challengePlayer.roleName;
            robotZhanBaoData.zhanbaoType = ChallengeInfoType_Be_Challenge_Failed;
            robotZhanBaoData.value = beChallengePlayer.ranking;
            robotZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 创建挑战者失败竞技场战报
        /// </summary>
        /// <param name="challengePlayer"></param>
        /// <param name="beChallengePlayer"></param>
        /// <param name="playerZhanBaoData"></param>
        /// <param name="robotZhanBaoData"></param>
        private void createChallengeFailedChallengeInfoData(PlayerJingJiData challengePlayer, PlayerJingJiData beChallengePlayer, out JingJiChallengeInfoData playerZhanBaoData, out JingJiChallengeInfoData robotZhanBaoData)
        {
            //创建挑战者竞技场战报
            playerZhanBaoData = new JingJiChallengeInfoData();

            playerZhanBaoData.roleId = challengePlayer.roleId;
            playerZhanBaoData.challengeName = beChallengePlayer.roleName;
            playerZhanBaoData.zhanbaoType = ChallengeInfoType_Challenge_Failed;
            playerZhanBaoData.value = challengePlayer.ranking;
            playerZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //创建被挑战者竞技场战报
            robotZhanBaoData = new JingJiChallengeInfoData();
            robotZhanBaoData.roleId = beChallengePlayer.roleId;
            robotZhanBaoData.challengeName = challengePlayer.roleName;
            robotZhanBaoData.zhanbaoType = ChallengeInfoType_Be_Challenge_Win;
            robotZhanBaoData.value = beChallengePlayer.ranking;
            robotZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 创建竞技场连胜战报
        /// </summary>
        private void createLianShengChallengeInfo(PlayerJingJiData challengePlayer, out JingJiChallengeInfoData playerZhanBaoData)
        {
            //创建挑战者竞技场战报
            playerZhanBaoData = new JingJiChallengeInfoData();

            playerZhanBaoData.roleId = challengePlayer.roleId;
            playerZhanBaoData.challengeName = "";
            playerZhanBaoData.zhanbaoType = ChallengeInfoType_LianSheng;
            playerZhanBaoData.value = challengePlayer.winCount;
            playerZhanBaoData.createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// GS处理完成后保存数据
        /// </summary>
        public void saveData(JingJiSaveData data, out int winCount)
        {
            winCount = 0;

            PlayerJingJiData playerData = null;
            PlayerJingJiData robotData = null;

            lock (changeRankingLock)
            {
                playerJingJiDatas.TryGetValue(data.roleId, out playerData);
                playerJingJiDatas.TryGetValue(data.robotId, out robotData);

                if (data.isWin)
                {
                    //更新挑战者竞技场数据 begin

                    playerData.level = data.level;
                    playerData.changeLiveCount = data.changeLiveCount;
                    playerData.nextChallengeTime = data.nextChallengeTime;
                    playerData.baseProps = data.baseProps;
                    playerData.extProps = data.extProps;
                    playerData.equipDatas = data.equipDatas;
                    playerData.skillDatas = data.skillDatas;
                    playerData.combatForce = data.combatForce;
                    playerData.winCount += 1;

                    playerData.convertString();

                    JingJiChangDBController.getInstance().updateJingJiDataForWin(playerData);

                    //                 //更新被挑战者竞技场数据
                    //                 JingJiChangDBController.getInstance().updateJingJiRanking(robotData.roleId, robotData.ranking);

                    //创建竞技场战报
                    JingJiChallengeInfoData playerZhanBaoData;
                    JingJiChallengeInfoData robotZhanBaoData;

                    this.createChallengeWinChallengeInfoData(playerData, robotData, out playerZhanBaoData, out robotZhanBaoData);

                    //插入数据库
                    JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(playerZhanBaoData);
                    JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(robotZhanBaoData);

                    JingJiChallengeInfoData lianShengZhanBaoData = null;
                    //创建连胜战报
                    if (playerData.winCount >= 10 && playerData.winCount % 10 == 0)
                    {
                        winCount = playerData.winCount;

                        this.createLianShengChallengeInfo(playerData, out lianShengZhanBaoData);

                        JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(lianShengZhanBaoData);
                    }

                    //更新内存数据
                    List<JingJiChallengeInfoData> playerZhanbaoList = null;
                    challengeInfos.TryGetValue(playerData.roleId, out playerZhanbaoList);

                    if (null != lianShengZhanBaoData)
                    {
                        playerZhanbaoList.Insert(0, lianShengZhanBaoData);
                        //只缓存50条
                        if (playerZhanbaoList.Count > ChallengeInfo_Max_Num)
                        {
                            playerZhanbaoList.RemoveAt(playerZhanbaoList.Count - 1);
                        }
                    }

                    playerZhanbaoList.Insert(0, playerZhanBaoData);

                    //只缓存50条
                    if (playerZhanbaoList.Count > ChallengeInfo_Max_Num)
                    {
                        playerZhanbaoList.RemoveAt(playerZhanbaoList.Count - 1);
                    }

                    if (robotData.isOnline)
                    {
                        List<JingJiChallengeInfoData> robotZhanbaoList = null;
                        challengeInfos.TryGetValue(robotData.roleId, out robotZhanbaoList);

                        robotZhanbaoList.Insert(0, robotZhanBaoData);

                        //只缓存50条
                        if (robotZhanbaoList.Count > ChallengeInfo_Max_Num)
                        {
                            robotZhanbaoList.RemoveAt(robotZhanbaoList.Count - 1);
                        }
                    }
                }
                else
                {
                    //连胜清零，重置上次挑战时间，更新声望值

                    if (playerData.winCount >= 10)
                    {
                        winCount = playerData.winCount;
                    }

                    playerData.winCount = 0;
                    playerData.nextChallengeTime = data.nextChallengeTime;

                    JingJiChangDBController.getInstance().updateJingJiDataForFailed(playerData.roleId, playerData.nextChallengeTime);

                    //创建竞技场战报
                    JingJiChallengeInfoData playerZhanBaoData;
                    JingJiChallengeInfoData robotZhanBaoData;

                    this.createChallengeFailedChallengeInfoData(playerData, robotData, out playerZhanBaoData, out robotZhanBaoData);

                    JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(playerZhanBaoData);
                    JingJiChangZhaoBaoDBController.getInstnace().insertZhanBao(robotZhanBaoData);

                    //更新内存数据
                    List<JingJiChallengeInfoData> playerZhanbaoList = null;
                    challengeInfos.TryGetValue(playerData.roleId, out playerZhanbaoList);

                    playerZhanbaoList.Insert(0, playerZhanBaoData);

                    //只缓存50条
                    if (playerZhanbaoList.Count > ChallengeInfo_Max_Num)
                    {
                        playerZhanbaoList.RemoveAt(playerZhanbaoList.Count - 1);
                    }

                    if (robotData.isOnline)
                    {
                        List<JingJiChallengeInfoData> robotZhanbaoList = null;
                        challengeInfos.TryGetValue(robotData.roleId, out robotZhanbaoList);

                        robotZhanbaoList.Insert(0, robotZhanBaoData);

                        //只缓存50条
                        if (robotZhanbaoList.Count > ChallengeInfo_Max_Num)
                        {
                            robotZhanbaoList.RemoveAt(robotZhanbaoList.Count - 1);
                        }
                    }
                }

                //解除被挑战者锁定状态
                //lock (lockPlayerJingJiDatas)
                //{
                //    lockPlayerJingJiDatas.Remove(robotData.roleId);
                //}

                BeChallengerCount beChallengerCount = null;
                int nBeChallengerCount = 0;
                lockPlayerJingJiDatas.TryGetValue(robotData.roleId, out beChallengerCount);
                if (null != beChallengerCount)
                {
                    nBeChallengerCount = beChallengerCount.nBeChallengerCount;
                    if (nBeChallengerCount <= 0)
                    {
                        lockPlayerJingJiDatas.Remove(robotData.roleId);
                    }
                }

                //已经不再500名以内了、不在线、没人挑战，就没有必要缓存了
                if (robotData.ranking == -1 && nBeChallengerCount <= 0 && !robotData.isOnline)
                {
                    // lock (playerJingJiDatas)
                    {
                        playerJingJiDatas.Remove(robotData.roleId);
                    }
                }                
            }           
            
        }

        /// <summary>
        /// 获取被挑战者mini数据用于面板展示
        /// </summary>
        /// <param name="challengeIds"></param>
        /// <returns></returns>
        public List<PlayerJingJiMiniData> getChallengeData(int[] challengeRankings)
        {
            List<PlayerJingJiMiniData> miniDataList = new List<PlayerJingJiMiniData>();
            lock (changeRankingLock)
            {
                // 防止出现错误的排名
                int nCheckCount = 0;
                while (nCheckCount++ < 6)
                {
                    bool bErrorRank = false;
                    foreach (int challengeRanking in challengeRankings)
                    {
                        PlayerJingJiData robotData = null;

                        if (challengeRanking > rankingDatas.Count)
                            continue;

                        PlayerJingJiRankingData rankingData;
                        rankingData = rankingDatas[challengeRanking - 1];
                        if (rankingData.ranking < 0)
                        {
                            bErrorRank = true;
                            rankingDatas.Remove(rankingData);
                            break;
                        }
                    }

                    if (bErrorRank)
                    {
                        rankingDatas.Sort();
                    }
                    else
                    {
                        break;
                    }
                }

                foreach (int challengeRanking in challengeRankings)
                {
                    PlayerJingJiData robotData = null;

                    if (challengeRanking > rankingDatas.Count)
                        continue;

                    PlayerJingJiRankingData rankingData;
                    rankingData = rankingDatas[challengeRanking - 1];
                    
                    if (!playerJingJiDatas.TryGetValue(rankingData.roleId, out robotData))
                        continue;

                    miniDataList.Add(robotData.getPlayerJingJiMiniData());
                }
            }

            return miniDataList;
        }

        /// <summary>
        /// 获取战报数据
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<JingJiChallengeInfoData> getChallengeInfoDataList(int roleId, int pageIndex)
        {

            if (pageIndex >= ChallengeInfo_Max_Num)
                return null;

            List<JingJiChallengeInfoData> zhanbaoDataList = null;

            //理论上不可能
            if (!challengeInfos.TryGetValue(roleId, out zhanbaoDataList))
            {
                return null;
            }

            int minIndex = pageIndex * ChallengeInfo_PageShowNum;
            int getNum = ChallengeInfo_PageShowNum;

            if (minIndex >= zhanbaoDataList.Count)
            {
                return null;
            }

            if (minIndex + getNum >= zhanbaoDataList.Count)
            {
                getNum = zhanbaoDataList.Count - minIndex;
            }

            if (getNum == 0)
            {
                return null;
            }

            return zhanbaoDataList.GetRange(minIndex, getNum);

        }

        /// <summary>
        /// 获取排行榜数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<PaiHangItemData> getRankingList(int pageIndex)
        {
            //int minIndex = 0;
            int maxIndex = RankingList_PageShowNum;

            if (maxIndex > rankingDatas.Count)
            {
                maxIndex = rankingDatas.Count;
            }

            List<PaiHangItemData> _rankingDatas = new List<PaiHangItemData>();

            lock (changeRankingLock)
            {
                for (int i = 0; i < maxIndex; i++)
                {
                    _rankingDatas.Add(rankingDatas[i].getPaiHangItemData());
                }
            }

            return _rankingDatas;

        }
    }

    /// <summary>
    /// 竞技场玩家登陆事件监听器
    /// </summary>
    public class JingJiChangPlayerLoginEventListener : IEventListener
    {
        private static JingJiChangPlayerLoginEventListener instance = new JingJiChangPlayerLoginEventListener();

        private JingJiChangPlayerLoginEventListener() { }

        public static JingJiChangPlayerLoginEventListener getInstnace()
        {
            return instance;
        }

        public void processEvent(EventObject eventObject)
        {
            if (eventObject.getEventType() != (int)EventTypes.PlayerLogin)
                return;

            PlayerLoginEventObject loginEvent = (PlayerLoginEventObject)eventObject;

            JingJiChangManager.getInstance().onPlayerLogin(loginEvent.RoleInfo.RoleID);
        }
    }

    /// <summary>
    /// 竞技场玩家登出事件监听器
    /// </summary>
    public class JingJiChangPlayerLogoutEventListener : IEventListener
    {
        private static JingJiChangPlayerLogoutEventListener instance = new JingJiChangPlayerLogoutEventListener();

        private JingJiChangPlayerLogoutEventListener() { }

        public static JingJiChangPlayerLogoutEventListener getInstnace()
        {
            return instance;
        }

        public void processEvent(EventObject eventObject)
        {
            if (eventObject.getEventType() != (int)EventTypes.PlayerLogout)
                return;

            PlayerLogoutEventObject logoutEvent = (PlayerLogoutEventObject)eventObject;

            JingJiChangManager.getInstance().onPlayerLogin(logoutEvent.RoleInfo.RoleID);
        }
    }
}
