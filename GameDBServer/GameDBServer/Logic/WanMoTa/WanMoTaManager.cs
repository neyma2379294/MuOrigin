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
using GameDBServer.Core;


namespace GameDBServer.Logic.WanMoTa
{
    public class WanMoTaManager : IManager
    {
        private static WanMoTaManager instance = new WanMoTaManager();

        /// <summary>
        /// 万魔塔排行榜最大缓存数量
        /// </summary>
        public static readonly int RankingList_Max_Num = 50;

        /// <summary>
        /// 万魔塔排行榜每页显示数量
        /// </summary>
        public static readonly int RankingList_PageShowNum = 30;//20;

        /// <summary>
        /// 万魔塔排行榜数据
        /// </summary>
        private List<PlayerWanMoTaRankingData> rankingDatas = new List<PlayerWanMoTaRankingData>();

        /// <summary>
        /// 万魔塔信息缓存
        /// </summary>
        private Dictionary<int, WanMotaInfo> playerWanMoTaDatas = new Dictionary<int, WanMotaInfo>();        

        public static WanMoTaManager getInstance()
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
            //修改万魔塔表数据
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_GET_WANMOTA_DETAIL, GetWanMoTaoDetailCmdProcessor.getInstance());
            //获取万魔塔信息
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, ModifyWanMoTaCmdProcessor.getInstance());
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void initData()
        {
            // 初始化万魔塔数据，排行榜数据
            List<WanMotaInfo> playerWanMoTaDataList = WanMoTaDBController.getInstance().getPlayerWanMotaDataList();

            if (null == playerWanMoTaDataList)
                return;

            foreach (WanMotaInfo data in playerWanMoTaDataList)
            {
                playerWanMoTaDatas.Add(data.nRoleID, data);
                rankingDatas.Add(data.getPlayerWanMoTaRankingData());
            }            
        }

        /// <summary>
        /// 初始化监听器
        /// </summary>
        private void initListener()
        {
            GlobalEventSource.getInstance().registerListener((int)EventTypes.PlayerLogin, WanMoTaPlayerLoginEventListener.getInstnace());
            GlobalEventSource.getInstance().registerListener((int)EventTypes.PlayerLogout, WanMoTaPlayerLogoutEventListener.getInstnace());
        }

        /// <summary>
        /// 删除监听器
        /// </summary>
        private void removeListener()
        {
            GlobalEventSource.getInstance().removeListener((int)EventTypes.PlayerLogin, WanMoTaPlayerLoginEventListener.getInstnace());
            GlobalEventSource.getInstance().removeListener((int)EventTypes.PlayerLogout, WanMoTaPlayerLogoutEventListener.getInstnace());
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
        /// 修改万魔塔排行榜数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void ModifyWanMoTaPaihangData(WanMotaInfo data, bool bIsLogin)
        {
            if (null == data)
                return;

            lock (rankingDatas)
            {
                PlayerWanMoTaRankingData result = rankingDatas.Find(
                    delegate(PlayerWanMoTaRankingData paiHang)
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
                        rankingDatas.Add(data.getPlayerWanMoTaRankingData());
                    }
                    else
                    {
                        // 如果数据好于排行榜最后一名，把排行榜最后移除掉，用其代替
                        if (data.nPassLayerCount > rankingDatas[rankingDatas.Count - 1].passLayerCount)
                        {
                            rankingDatas.RemoveAt(rankingDatas.Count - 1);
                            rankingDatas.Add(data.getPlayerWanMoTaRankingData());
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
        /// 创建万魔塔数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int createWanMoTaData(WanMotaInfo data)
        {
            lock (playerWanMoTaDatas)
            {
                if (playerWanMoTaDatas.ContainsKey(data.nRoleID))
                    return 0;                

                playerWanMoTaDatas.Add(data.nRoleID, data);
            }

            // 添加更新万魔塔排行榜数据
            ModifyWanMoTaPaihangData(data, true);

            return WanMoTaDBController.getInstance().insertWanMoTaData(TCPManager.getInstance().DBMgr, data);
        }

        /// <summary>
        /// 更新万魔塔数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int updateWanMoTaData(int nRoleID, string[] fields, int startIndex)
        {
            return WanMoTaDBController.updateWanMoTaData(TCPManager.getInstance().DBMgr, nRoleID, fields, 1);
        }

        /// <summary>
        /// 创建万魔塔数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public WanMotaInfo getWanMoTaData(int nRoleID)
        {
            WanMotaInfo data = null;
            lock (playerWanMoTaDatas)
            {
                if (playerWanMoTaDatas.TryGetValue(nRoleID, out data))
                {
                    // 获取最高通关层数
                    data.nTopPassLayerCount = rankingDatas[0].passLayerCount;
                    return data;
                }
            }

            // 没有找到的话直接返回空
            // 因为在登录时有处理，这里不找重复处理
            return data;
        }

        /// <summary>
        /// 玩家上线初始化时获取玩家万魔塔数据
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public void onPlayerLogin(int roleId, string strRoleName)
        {
            WanMotaInfo data = null;
            lock (playerWanMoTaDatas)
            {
                if (playerWanMoTaDatas.TryGetValue(roleId, out data))
                {
                    return;
                }
            }

            if (null == data)
            {
                data = WanMoTaDBController.getInstance().getPlayerWanMoTaDataById(roleId);
                if (null == data)
                {
                    // 数据库中没有万魔塔数据，插入相应的数据
                    WanMotaInfo newData = new WanMotaInfo();
                    newData.nRoleID = roleId;
                    newData.strRoleName = strRoleName;
                    newData.lFlushTime = TimeUtil.NOW();
                    newData.nSweepLayer = -1;

                    createWanMoTaData(newData);
                }
                else
                {
                    // 添加更新万魔塔排行榜数据
                    ModifyWanMoTaPaihangData(data, true);

                    lock (playerWanMoTaDatas)
                    {
                        playerWanMoTaDatas.Add(data.nRoleID, data);
                    }
                }
            }
        }

        /// <summary>
        /// 玩家下线时清除玩家万魔塔数据
        /// </summary>
        /// <param name="roleId"></param>
        public void onPlayerLogout(int roleId)
        {
            WanMotaInfo data = null;

            lock (playerWanMoTaDatas)
            {
                playerWanMoTaDatas.TryGetValue(roleId, out data);

                if (null != data)
                {
                    playerWanMoTaDatas.Remove(roleId);
                }
            }
        }
    }



    /// <summary>
    /// 万魔塔玩家登陆事件监听器
    /// </summary>
    public class WanMoTaPlayerLoginEventListener : IEventListener
    {
        private static WanMoTaPlayerLoginEventListener instance = new WanMoTaPlayerLoginEventListener();

        private WanMoTaPlayerLoginEventListener() { }

        public static WanMoTaPlayerLoginEventListener getInstnace()
        {
            return instance;
        }

        public void processEvent(EventObject eventObject)
        {
            if (eventObject.getEventType() != (int)EventTypes.PlayerLogin)
                return;

            PlayerLoginEventObject loginEvent = (PlayerLoginEventObject)eventObject;

            WanMoTaManager.getInstance().onPlayerLogin(loginEvent.RoleInfo.RoleID, loginEvent.RoleInfo.RoleName);
        }
    }

    /// <summary>
    /// 万魔塔玩家登出事件监听器
    /// </summary>
    public class WanMoTaPlayerLogoutEventListener : IEventListener
    {
        private static WanMoTaPlayerLogoutEventListener instance = new WanMoTaPlayerLogoutEventListener();

        private WanMoTaPlayerLogoutEventListener() { }

        public static WanMoTaPlayerLogoutEventListener getInstnace()
        {
            return instance;
        }

        public void processEvent(EventObject eventObject)
        {
            if (eventObject.getEventType() != (int)EventTypes.PlayerLogout)
                return;

            PlayerLogoutEventObject logoutEvent = (PlayerLogoutEventObject)eventObject;

            WanMoTaManager.getInstance().onPlayerLogout(logoutEvent.RoleInfo.RoleID);
        }
    }
}
