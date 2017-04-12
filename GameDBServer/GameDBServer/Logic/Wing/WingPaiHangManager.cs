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
using Server.Tools;


namespace GameDBServer.Logic.Wing
{
    public class WingPaiHangManager : IManager
    {
        private static WingPaiHangManager instance = new WingPaiHangManager();

        /// <summary>
        /// 翅膀排行榜最大缓存数量
        /// </summary>
        public static readonly int RankingList_Max_Num =100;

        /// <summary>
        /// 翅膀排行榜每页显示数量
        /// </summary>
        public static readonly int RankingList_PageShowNum = 30;//20;

        /// <summary>
        /// 翅膀排行榜数据
        /// </summary>
        private List<PlayerWingRankingData> rankingDatas = new List<PlayerWingRankingData>();

        /// <summary>
        /// 翅膀信息缓存
        /// </summary>
        private Dictionary<int, WingRankingInfo> playerWingDatas = new Dictionary<int, WingRankingInfo>();        

        public static WingPaiHangManager getInstance()
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
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void initData()
        {
            // 初始化翅膀数据，排行榜数据
            List<WingRankingInfo> playerWingDataList = WingPaiHangDBController.getInstance().getPlayerWingDataList();

            if (null == playerWingDataList)
                return;

            foreach (WingRankingInfo data in playerWingDataList)
            {
                playerWingDatas.Add(data.nRoleID, data);
                rankingDatas.Add(data.getPlayerWingRankingData());
            }            
        }

        /// <summary>
        /// 初始化监听器
        /// </summary>
        private void initListener()
        {
            GlobalEventSource.getInstance().registerListener((int)EventTypes.PlayerLogin, WingPlayerLoginEventListener.getInstnace());
            GlobalEventSource.getInstance().registerListener((int)EventTypes.PlayerLogout, WingPlayerLogoutEventListener.getInstnace());
        }

        /// <summary>
        /// 删除监听器
        /// </summary>
        private void removeListener()
        {
            GlobalEventSource.getInstance().removeListener((int)EventTypes.PlayerLogin, WingPlayerLoginEventListener.getInstnace());
            GlobalEventSource.getInstance().removeListener((int)EventTypes.PlayerLogout, WingPlayerLogoutEventListener.getInstnace());
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        private void removeData()
        {            
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
        /// 获取排行榜数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<PaiHangItemData> getRankingList(int pageIndex)
        {
            int maxIndex = RankingList_PageShowNum;

            if (maxIndex > rankingDatas.Count)
            {
                maxIndex = rankingDatas.Count;
            }

            List<PaiHangItemData> _rankingDatas = new List<PaiHangItemData>();

            for (int i = 0; i < maxIndex; i++)
            {
                _rankingDatas.Add(rankingDatas[i].getPaiHangItemData());
            }

            return _rankingDatas;

        }

        /// <summary>
        /// 修改翅膀排行榜数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void ModifyWingPaihangData(WingRankingInfo data, bool bIsLogin)
        {
            if (null == data)
                return;

            lock (rankingDatas)
            {
                PlayerWingRankingData result = rankingDatas.Find(
                    delegate(PlayerWingRankingData paiHang)
                    {
                        return paiHang.roleId == data.nRoleID;
                    }
                );

                // 如果没有在排行榜中，看看能不能添加
                if (null == result)
                {
                    // 排行榜数据量是有限制的
                    if (rankingDatas.Count < RankingList_Max_Num)
                    {
                        rankingDatas.Add(data.getPlayerWingRankingData());
                    }
                    else
                    {
                        // 如果数据好于排行榜最后一名，把排行榜最后移除掉，用其代替
                        if (data.nWingID * 20 + data.nStarNum > rankingDatas[rankingDatas.Count - 1].WingID * 20 + rankingDatas[rankingDatas.Count - 1].WingStarNum)
                        {
                            rankingDatas.RemoveAt(rankingDatas.Count - 1);
                            rankingDatas.Add(data.getPlayerWingRankingData());
                        }
                    }
                }
                // 在排行榜中，用新数据重新排序
                else
                {
                    // 由参数控制，登录时数据没有改变，不需要重新排序
                    if (!bIsLogin)
                    {
                        try
                        {
                            rankingDatas.Sort();
                        }
                        catch (Exception ex)
                        {
                            DataHelper.WriteFormatExceptionLog(ex, "", false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建翅膀数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int createWingData(int nRoleID)
        {
            WingRankingInfo data = null;
            lock (playerWingDatas)
            {
                if (playerWingDatas.ContainsKey(nRoleID))
                    return 0;

                data = getWingData(nRoleID);
                if (null != data)
                {
                    playerWingDatas.Add(data.nRoleID, data);
                }
            }

            if (null != data)
            {
                // 添加更新翅膀排行榜数据
                ModifyWingPaihangData(data, false);
            }

            return 1;
        }

        /// <summary>
        /// 创建翅膀数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public WingRankingInfo getWingData(int nRoleID)
        {
            WingRankingInfo data = null;
            lock (playerWingDatas)
            {
                if (playerWingDatas.TryGetValue(nRoleID, out data))
                {
                    return data;
                }
            }

            // 没有找到的话直接返回空
            // 因为在登录时有处理，这里不找重复处理
            return WingPaiHangDBController.getInstance().getWingDataById(nRoleID);
        }

        /// <summary>
        /// 玩家上线初始化时获取玩家翅膀数据
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public void onPlayerLogin(int roleId, string strRoleName)
        {
            WingRankingInfo data = null;
            lock (playerWingDatas)
            {
                if (playerWingDatas.TryGetValue(roleId, out data))
                {
                    return;
                }
            }

            if (null == data)
            {
                data = WingPaiHangDBController.getInstance().getWingDataById(roleId);
                if (null != data)
                {
                    // 添加更新翅膀排行榜数据
                    ModifyWingPaihangData(data, true);

                    lock (playerWingDatas)
                    {
                        playerWingDatas.Add(data.nRoleID, data);
                    }
                }
            }
        }

        /// <summary>
        /// 玩家下线时清除玩家翅膀数据
        /// </summary>
        /// <param name="roleId"></param>
        public void onPlayerLogout(int roleId)
        {
            WingRankingInfo data = null;

            lock (playerWingDatas)
            {
                playerWingDatas.TryGetValue(roleId, out data);

                if (null != data)
                {
                    playerWingDatas.Remove(roleId);
                }
            }
        }
    }



    /// <summary>
    /// 翅膀玩家登陆事件监听器
    /// </summary>
    public class WingPlayerLoginEventListener : IEventListener
    {
        private static WingPlayerLoginEventListener instance = new WingPlayerLoginEventListener();

        private WingPlayerLoginEventListener() { }

        public static WingPlayerLoginEventListener getInstnace()
        {
            return instance;
        }

        public void processEvent(EventObject eventObject)
        {
            if (eventObject.getEventType() != (int)EventTypes.PlayerLogin)
                return;

            PlayerLoginEventObject loginEvent = (PlayerLoginEventObject)eventObject;

            WingPaiHangManager.getInstance().onPlayerLogin(loginEvent.RoleInfo.RoleID, loginEvent.RoleInfo.RoleName);
        }
    }

    /// <summary>
    /// 翅膀玩家登出事件监听器
    /// </summary>
    public class WingPlayerLogoutEventListener : IEventListener
    {
        private static WingPlayerLogoutEventListener instance = new WingPlayerLogoutEventListener();

        private WingPlayerLogoutEventListener() { }

        public static WingPlayerLogoutEventListener getInstnace()
        {
            return instance;
        }

        public void processEvent(EventObject eventObject)
        {
            if (eventObject.getEventType() != (int)EventTypes.PlayerLogout)
                return;

            PlayerLogoutEventObject logoutEvent = (PlayerLogoutEventObject)eventObject;

            WingPaiHangManager.getInstance().onPlayerLogout(logoutEvent.RoleInfo.RoleID);
        }
    }
}
