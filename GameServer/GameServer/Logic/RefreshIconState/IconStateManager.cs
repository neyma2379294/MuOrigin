using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Data;
using GameServer.Server;
using GameServer.Logic.JingJiChang;
using Server.Tools;
using Server.Protocol;

namespace GameServer.Logic.RefreshIconState
{
    /// summary
	/// 玩家各图标状态管理类
	/// summary
    public class IconStateManager
    {
        /// <summary>
        /// ICON状态项缓存，用于生成状态信息网络传输数据
        /// </summary>
        private Dictionary<ushort, ushort> m_StateIconsDict = new Dictionary<ushort, ushort>();

        /// <summary>
        /// ICON状态项缓存，用于减少网络传输次数
        /// </summary>
        private Dictionary<ushort, ushort> m_StateCacheIconsDict = new Dictionary<ushort, ushort>();

        /// <summary>
        /// ICON状态发送结构缓存，用于减少new次数
        /// </summary>
        private ActivityIconStateData m_ActivityIconStateData = new ActivityIconStateData();
                
        /// <summary>
        /// 添加刷新项到状态字典
        /// </summary>
        public bool AddFlushIconState(ushort nIconOrder, bool bIconState)
        {
            ushort iState = (ushort)(bIconState ? 1 : 0);
            return AddFlushIconState(nIconOrder, iState);
        }

        /// <summary>
        /// 添加刷新项到状态字典
        /// </summary>
        public bool AddFlushIconState(ushort nIconOrder, ushort iState)
        {
            ushort nIconInfo = (ushort)((nIconOrder << 1) + iState);            
            
            ushort nOldState = 0;
            lock (m_StateIconsDict)
            {
                // 原来缓存中没有值，需要刷新图标状态
                if (!m_StateCacheIconsDict.TryGetValue(nIconOrder, out nOldState))
                {
                    m_StateCacheIconsDict[nIconOrder] = nIconInfo;
                    m_StateIconsDict[nIconOrder] = nIconInfo;
                    // LogManager.WriteLog(LogTypes.Info, "!m_StateCacheIconsDict nIconOrder:" + nIconOrder + ", nIconInfo:" + nIconInfo + ", state:" + iState);
                    return true;
                }
                else
                {                    
                    // 如果设置的值和原来的相等，则不刷新
                    if ((nOldState & 0x1) == iState)
                    {
                        // LogManager.WriteLog(LogTypes.Info, "(nOldState % 2) == iState nIconOrder:" + nIconOrder + ", nOldState:" + nOldState + ", state:" + iState);
                        return false;
                    }
                    // 不相等，则更新
                    else
                    {
                        // LogManager.WriteLog(LogTypes.Info, "(nOldState % 2) == iState false nIconOrder:" + nIconOrder + ", nIconInfo:" + nIconInfo + ", nOldState:" + nOldState + ", state:" + iState);
                        m_StateCacheIconsDict[nIconOrder] = nIconInfo;
                        m_StateIconsDict[nIconOrder] = nIconInfo;
                        return true;
                    }
                }
            }
        }

        /// <summary>
        /// 重置ICON状态项缓存
        /// </summary>
        public void ResetIconStateDict(bool bIsLogin)
        {
            lock (m_StateIconsDict)
            {
                if (true == bIsLogin)
                {
                    // LogManager.WriteLog(LogTypes.Info, "m_StateCacheIconsDict.Clear()");
                    m_StateCacheIconsDict.Clear();
                }

                // LogManager.WriteLog(LogTypes.Info, "m_StateIconsDict.Clear()");
                m_StateIconsDict.Clear();
            }
        }

        /// <summary>
        /// 发送ICON消息到客户端
        /// </summary>
        public void SendIconStateToClient(GameClient client)
        {
            ushort[] arrState = null ;
            int nIconStateCount;

            lock (m_StateIconsDict)
            {
                nIconStateCount = m_StateIconsDict.Count();
                if (nIconStateCount > 0)
                {
                    arrState = new ushort[nIconStateCount];
                    nIconStateCount = 0;

                    foreach (KeyValuePair<ushort, ushort> kvp in m_StateIconsDict)
                    {
                        arrState[nIconStateCount++] = kvp.Value;
                    }
                }

                if (null != arrState && arrState.Length > 0)
                {
                    m_ActivityIconStateData.arrIconState = arrState;
                    client.sendCmd<ActivityIconStateData>((int)TCPGameServerCmds.CMD_SPR_REFRESH_ICON_STATE, m_ActivityIconStateData);

                    // 发送自动重置状态缓存列表
                    ResetIconStateDict(false);
                }
            }
        }

        /// <summary>
        /// 用户登录时刷新图标状态
        /// </summary>
        public void LoginGameFlushIconState(GameClient client)
        {
            ResetIconStateDict(true);

            // 日常活动及子项图标状态刷新
            CheckHuangJinBoss(client);
            CheckShiJieBoss(client);
            CheckHuoDongState(client);

            // 福利及子项图标状态刷新
            CheckFuLiMeiRiHuoYue(client);
            CheckFuLiLianXuDengLu(client);
            CheckFuLiLeiJiDengLu(client);
            CheckFuMeiRiZaiXian(client);
            CheckFuUpLevelGift(client);

            // 充值相关刷新
            FlushChongZhiIconState(client);
            FlushUsedMoneyconState(client);

            // 竞技场及子项图标状态刷新
            CheckJingJiChangLeftTimes(client);
            CheckJingJiChangJiangLi(client);
            CheckJingJiChangJunXian(client);

            // 每日必做图标刷新
            CheckZiYuanZhaoHui(client);

            //邮件状态
            CheckEmailCount(client);

            //免费祈福
            CheckFreeImpetrateState(client);

            //成就称号升级状态
            CheckChengJiuUpLevelState(client);

            //VIP奖励领取状态
            CheckVIPLevelAwardState(client);

            // 合服活动检测
            CheckHeFuActivity(client);

            // 节日活动检测
            CheckJieRiActivity(client, true);

            CheckGuildIcon(client, true);

            // 发送刷新项到客户端
            SendIconStateToClient(client);

            // 补偿状态检测
            CheckBuChangState(client);

            CheckCaiJiState(client);
        }

        /// <summary>
        /// 充值后刷新与充值相关的ICON状态
        /// </summary>
        public bool FlushChongZhiIconState(GameClient client)
        {
            CheckShouCiChongZhi(client);
            CheckMeiRiChongZhi(client);
            CheckLeiJiChongZhi(client);

            CheckXinFuChongZhiMoney(client);
            CheckXinFuFreeGetMoney(client);
            return false;
        }

        /// <summary>
        /// 消费后刷新与充值相关的ICON状态
        /// </summary>
        public bool FlushUsedMoneyconState(GameClient client)
        {
            CheckLeiJiXiaoFei(client);
            CheckXinFuUseMoney(client);

            return false;
        }

        /// <summary>
        /// 检查“每日活跃”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckFuLiMeiRiHuoYue(GameClient client)
        {
            // 判断目前的活跃值是否够领奖
            foreach (KeyValuePair<int, SystemXmlItem> kvp in GameManager.systemDailyActiveAward.SystemXmlItemDict)
            {
                int nAwardDailyActiveValue = Math.Max(0, kvp.Value.GetIntValue("NeedhuoYue"));
                int nID = kvp.Value.GetIntValue("ID");

                // 活跃值够
                if (nAwardDailyActiveValue <= client.ClientData.DailyActiveValues)
                {
                     // 还没领过奖
                    if (DailyActiveManager.IsDailyActiveAwardFetched(client, nID) <= 0)
                    {
                        return AddFlushIconState((ushort)ActivityTipTypes.FuLiMeiRiHuoYue, true);
                    }
                }
            }

            // 活跃值不够或已经领过奖了
            return AddFlushIconState((ushort)ActivityTipTypes.FuLiMeiRiHuoYue, false);           
        }

        /// <summary>
        /// 检查“连续登录”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckFuLiLianXuDengLuReward(GameClient client)
        {
            int nDay = DateTime.Now.DayOfYear;
            bool bFulsh = true;
            if (client.ClientData.MyHuodongData.SeriesLoginAwardDayID == nDay && client.ClientData.MyHuodongData.SeriesLoginGetAwardStep <= client.ClientData.SeriesLoginNum)
            {
                // 今天没有抽奖次数了
                bFulsh = false;
            }

            return bFulsh;
        }

        /// <summary>
        /// 检查“连续登录”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckFuLiLianXuDengLu(GameClient client)
        {
            bool bFulsh = CheckFuLiLianXuDengLuReward(client);

            return AddFlushIconState((ushort)ActivityTipTypes.FuLiLianXuDengLu, bFulsh);
        }

        /// <summary>
        /// 检查“累计登陆”项是否有奖励未领取
        /// </summary>
        public bool CheckFuLiLeiJiDengLuReward(GameClient client)
        {
            int nFlag = Global.GetRoleParamsInt32FromDB(client, RoleParamName.TotalLoginAwardFlag);

            int nLoginNum = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalDayLogin);
            int nMaxLoginNum = Data.TotalLoginDataInfoList.Count();
            bool bFulsh = false;

            for (int i = 0; i < 7 && i < nLoginNum && i < nMaxLoginNum; i++)
            {
                // 还有未领取的累计登陆奖励
                if ((nFlag & (0x1 << (i + 1))) == 0)
                {
                    bFulsh = true;
                    break;
                }
            }

            if (nLoginNum == 30)
            {
                if ((nFlag & (0x1 << (10))) == 0)
                {
                    bFulsh = true;
                }
            }

            if (nLoginNum == 21)
            {
                if ((nFlag & (0x1 << 9)) == 0)
                {
                    bFulsh = true;
                }
            }

            if (nLoginNum == 14)
            {
                if ((nFlag & (0x1 << (8))) == 0)
                {
                    bFulsh = true;
                }
            }

            return bFulsh;
        }

        /// <summary>
        /// 检查“累计登陆”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckFuLiLeiJiDengLu(GameClient client)
        {
            bool bFulsh = CheckFuLiLeiJiDengLuReward(client);            

            return AddFlushIconState((ushort)ActivityTipTypes.FuLiLeiJiDengLu, bFulsh);
        }

        /// <summary>
        /// 检查“每日在线”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckFuMeiRiZaiXian(GameClient client)
        {
            int nDate = DateTime.Now.DayOfYear;

            if (client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID != nDate)
            {
                client.ClientData.MyHuodongData.EveryDayOnLineAwardStep = 0;
                client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID = nDate;
            }

            int nSetp = client.ClientData.MyHuodongData.EveryDayOnLineAwardStep;

            // 一共能领几次
            int nTotal = HuodongCachingMgr.GetEveryDayOnLineItemCount();
            if (nTotal == client.ClientData.MyHuodongData.EveryDayOnLineAwardStep)
            {
                // 今天已经全部领完了
                return AddFlushIconState((ushort)ActivityTipTypes.FuLiMeiRiZaiXian, false);
            }

            int nIndex1 = nTotal - client.ClientData.MyHuodongData.EveryDayOnLineAwardStep;
            EveryDayOnLineAward EveryDayOnLineAwardItem = null;
            for (int n = client.ClientData.MyHuodongData.EveryDayOnLineAwardStep + 1; n <= nTotal; ++n)
            {
                EveryDayOnLineAwardItem = HuodongCachingMgr.GetEveryDayOnLineItem(n);
                if (null == EveryDayOnLineAwardItem)
                {
                    return false;
                }

                // 如果已到领取的时间
                if (client.ClientData.DayOnlineSecond >= EveryDayOnLineAwardItem.TimeSecs)
                {
                    return AddFlushIconState((ushort)ActivityTipTypes.FuLiMeiRiZaiXian, true);
                }
            }

            return AddFlushIconState((ushort)ActivityTipTypes.FuLiMeiRiZaiXian, false);
        }

        /// <summary>
        /// 检查“等级奖励”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckFuUpLevelGift(GameClient client)
        {
            List<int> flagList = Global.GetRoleParamsIntListFromDB(client, RoleParamName.UpLevelGiftFlags);
            bool exist = false;
            for (int i = 0; i < flagList.Count * 16; i++ )
            {
                if (Global.GetBitValue(flagList, i * 2) == 1 && Global.GetBitValue(flagList, i * 2 + 1) == 0)
                {
                    exist = true;
                    break;
                }
            }

            return AddFlushIconState((ushort)ActivityTipTypes.FuLiUpLevelGift, exist);
        }

        /// <summary>
        /// 检查“充值回馈”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckFuLiChongZhiHuiKui(GameClient client)
        {
            bool bShouCiChongZhi = CheckShouCiChongZhi(client);
            bool bMeiRiChongZhi = CheckMeiRiChongZhi(client);
            bool bLeiJiChongZhi = CheckLeiJiChongZhi(client);
            bool bLeiJiXiaoFei = CheckLeiJiXiaoFei(client);

            // 有任意子项要更新，主图标更新
            if (bShouCiChongZhi || bMeiRiChongZhi || bLeiJiChongZhi || bLeiJiXiaoFei)
            {
                return AddFlushIconState((ushort)ActivityTipTypes.FuLiChongZhiHuiKui, true);
            }

            return AddFlushIconState((ushort)ActivityTipTypes.FuLiChongZhiHuiKui, false);
        }

        /// <summary>
        /// 检查“充值回馈-首次充值”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckShouCiChongZhi(GameClient client)
        {
            int totalChongZhiMoney = GameManager.ClientMgr.QueryTotaoChongZhiMoney(client);
            if (totalChongZhiMoney > 0)
            {
                if (Global.CanGetFirstChongZhiDaLiByUserID(client))
                {
                    AddFlushIconState((ushort)ActivityTipTypes.ShouCiChongZhi_YiLingQu, 0);
                    return AddFlushIconState((ushort)ActivityTipTypes.ShouCiChongZhi, 1);
                }
                else
                {
                    AddFlushIconState((ushort)ActivityTipTypes.ShouCiChongZhi_YiLingQu, 1);
                }
            }
            else
            {
                AddFlushIconState((ushort)ActivityTipTypes.ShouCiChongZhi_YiLingQu, 0);
            }

            return AddFlushIconState((ushort)ActivityTipTypes.ShouCiChongZhi, 0);
        }

        /// <summary>
        /// 检查“充值回馈-每日充值”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckMeiRiChongZhi(GameClient client)
        {
            bool hasGet;
            bool ret = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.MeiRiChongZhiHaoLi, out hasGet);
            AddFlushIconState((ushort)ActivityTipTypes.MeiRiChongZhi_YiLingQu, hasGet);
            return AddFlushIconState((ushort)ActivityTipTypes.MeiRiChongZhi, ret);
        }

        /// <summary>
        /// 检查“充值回馈-累积充值”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckLeiJiChongZhi(GameClient client)
        {
            bool hasGet;
            bool ret = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.TotalCharge, out hasGet);
             return AddFlushIconState((ushort)ActivityTipTypes.LeiJiChongZhi, ret);
        }

        /// <summary>
        /// 检查“充值回馈-累计消费”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckLeiJiXiaoFei(GameClient client)
        {
            bool hasGet;
            bool ret = RechargeRepayActiveMgr.CheckRechargeReplay(client, ActivityTypes.TotalConsume, out hasGet);
            return AddFlushIconState((ushort)ActivityTipTypes.LeiJiXiaoFei, ret);
        }

        /// <summary>
        /// 检查“主新服图标”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckMainXinFuIcon(GameClient client)
        {
            return false;
            // 此功能没有，暂不添加
            // return AddFlushIconState((ushort)ActivityTipTypes.FuLiLeiJiDengLu, false);
        }

        /// <summary>
        /// 检查“屠魔勇士”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckXinFuKillBoss(GameClient client)
        {
            return false;
            // 此功能没有，暂不添加
            // return AddFlushIconState((ushort)ActivityTipTypes.FuLiLeiJiDengLu, false);
        }

        /// <summary>
        /// 检查“充值达人”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckXinFuChongZhiMoney(GameClient client)
        {
            return false;
            // 此功能没有，暂不添加
            // return AddFlushIconState((ushort)ActivityTipTypes.FuLiLeiJiDengLu, false);
        }

        /// <summary>
        /// 检查“消费达人”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckXinFuUseMoney(GameClient client)
        {
            return false;
            // 此功能没有，暂不添加
            // return AddFlushIconState((ushort)ActivityTipTypes.FuLiLeiJiDengLu, false);
        }

        /// <summary>
        /// 检查“劲爆返利”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckXinFuFreeGetMoney(GameClient client)
        {
            return false;
            // 此功能没有，暂不添加
            // return AddFlushIconState((ushort)ActivityTipTypes.FuLiLeiJiDengLu, false);
        }


        /// <summary>
        /// 检查“剩余挑战次数”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckJingJiChangLeftTimes(GameClient client)
        {
            if (JingJiChangManager.getInstance().checkEnterNum(client, (int)JingJiChangManager.Enter_Type_Free) == ResultCode.Success)
            {
                return AddFlushIconState((ushort)ActivityTipTypes.JingJiChangLeftTimes, true);
            }

            return AddFlushIconState((ushort)ActivityTipTypes.JingJiChangLeftTimes, false);
        }

        /// <summary>
        /// 检查“奖励预览”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckJingJiChangJiangLi(GameClient client)
        {
            if (JingJiChangManager.getInstance().CanGetrankingReward(client))
            {
                return AddFlushIconState((ushort)ActivityTipTypes.JingJiChangJiangLi, true);
            }

            return AddFlushIconState((ushort)ActivityTipTypes.JingJiChangJiangLi, false);
        }

        /// <summary>
        /// 检查“军衔提升”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckJingJiChangJunXian(GameClient client)
        {
            if (JingJiChangManager.getInstance().CanGradeJunXian(client))
            {
                return AddFlushIconState((ushort)ActivityTipTypes.JingJiChangJunXian, true);
            }

            return AddFlushIconState((ushort)ActivityTipTypes.JingJiChangJunXian, false);
        }

        /// <summary>
        /// 检查“世界BOSS”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckShiJieBoss(GameClient client)
        {
            if (TimerBossManager.getInstance().HaveWorldBoss(client))
            {
                return AddFlushIconState((ushort)ActivityTipTypes.ShiJieBoss, true);
            }

            return AddFlushIconState((ushort)ActivityTipTypes.ShiJieBoss, false);
        }

        /// <summary>
        /// 活动项是否要显示要更新图标状态
        /// </summary>
        public bool CheckHuoDongState(GameClient client)
        {
            if (GameManager.AngelTempleMgr.CanEnterAngelTempleOnTime())
            {
                return AddFlushIconState((ushort)ActivityTipTypes.AngelTemple, true);
            }
            AddFlushIconState((ushort)ActivityTipTypes.AngelTemple, false);

            return true;
        }

        /// <summary>
        /// 检查“黄金部队”项是否要显示要更新图标状态
        /// </summary>
        public bool CheckHuangJinBoss(GameClient client)
        {
            if (TimerBossManager.getInstance().HaveHuangJinBoss(client))
            {
                return AddFlushIconState((ushort)ActivityTipTypes.HuangJinBoss, true);
            }

            return AddFlushIconState((ushort)ActivityTipTypes.HuangJinBoss, false);
        }


        /// <summary>
        /// 检查“资源找回”图标状态
        /// </summary>
        public bool CheckZiYuanZhaoHui(GameClient client)
        {
            if (CGetOldResourceManager.HasOldResource(client))
            {
                return AddFlushIconState((ushort)ActivityTipTypes.ZiYuanZhaoHui, true);
            }

            return AddFlushIconState((ushort)ActivityTipTypes.ZiYuanZhaoHui, false);
        }

        /// <summary>
        /// 检查是否有未读邮件
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool CheckEmailCount(GameClient client)
        {
            string cmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 1, 1);
            int emailCount = Global.sendToDB<int>((int)TCPGameServerCmds.CMD_SPR_GETUSERMAILCOUNT, cmd);
            if (emailCount > 0)
            {
                return AddFlushIconState((ushort)ActivityTipTypes.MainEmailIcon, true);
            }

            return AddFlushIconState((ushort)ActivityTipTypes.MainEmailIcon, false);
        }

        /// <summary>
        /// 检查是否可提升成就称号
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool CheckChengJiuUpLevelState(GameClient client)
        {
            bool result = AddFlushIconState((ushort)ActivityTipTypes.MainChengJiuIcon, ChengJiuManager.CanActiveNextChengHao(client));
            SendIconStateToClient(client);
            return result;
        }

        /// <summary>
        /// 检查是否可领取VIP奖励
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool CheckVIPLevelAwardState(GameClient client)
        {
            for (int nIndex = 1; nIndex <= client.ClientData.VipLevel; nIndex++)
            {
                int nFlag = client.ClientData.VipAwardFlag & Global.GetBitValue(nIndex + 1);
                if (nFlag < 1)
                {
                    return AddFlushIconState((ushort)ActivityTipTypes.VIPGifts, true);
                }
            }

            return AddFlushIconState((ushort)ActivityTipTypes.VIPGifts, false);
        }

        /// <summary>
        /// 检查免费祈福状态
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool CheckFreeImpetrateState(GameClient client)
        {
            bool bFlush = false;

            string strTime = null;
            strTime = Global.GetRoleParamsStringWithNullFromDB(client, RoleParamName.ImpetrateTime);

            if (string.IsNullOrEmpty(strTime))
            {
                bFlush = true;
            }
            else
            {
                DateTime dTime1 = DateTime.Now;
                DateTime dTime2 = DateTime.Parse(strTime);

                TimeSpan dTimeSpan = dTime1 - dTime2;

                double dSecond = 0.0;
                dSecond = dTimeSpan.TotalSeconds;

                double dRet = 0.0;
                dRet = Global.GMax(0, Data.FreeImpetrateIntervalTime - dSecond);
                if (dRet <= 0)
                {
                    bFlush = true;
                }
            }

            return AddFlushIconState((ushort)ActivityTipTypes.QiFuIcon, bFlush);
        }

        /// <summary>
        /// 检查是否可领取VIP奖励
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool CheckBuChangState(GameClient client)
        {
            bool bFlush = BuChangManager.CheckGiveBuChang(client);

            return AddFlushIconState((ushort)ActivityTipTypes.BuChangIcon, bFlush);
        }

        /// <summary>
        /// 检查合服活动领取标记
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool CheckHeFuActivity(GameClient client)
        {
            AddFlushIconState((ushort)ActivityTipTypes.HeFuLogin, false);
            AddFlushIconState((ushort)ActivityTipTypes.HeFuTotalLogin, false);
            AddFlushIconState((ushort)ActivityTipTypes.HeFuRecharge, false);
            AddFlushIconState((ushort)ActivityTipTypes.HeFuPKKing, false);

            bool bFlush = false;
            // 登录好礼
            bFlush = CheckHeFuLogin(client) | bFlush;
            // 累计登陆
            bFlush = CheckHeFuTotalLogin(client) | bFlush;
            // 充值返利
            bFlush = CheckHeFuRecharge(client) | bFlush;
            // 战场之神
            bFlush = CheckHeFuPKKing(client) | bFlush;
            return AddFlushIconState((ushort)ActivityTipTypes.HeFuActivity, bFlush);
        }

        /// <summary>
        /// 检查合服登陆豪礼领取标记
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool CheckHeFuLogin(GameClient client)
        {
            HeFuLoginActivity activity = HuodongCachingMgr.GetHeFuLoginActivity();
            if (null == activity)
            {
                return false;
            }

            // 检查是否在允许领取的时间内
            if (!activity.InAwardTime())
            {
                return false;
            }

            bool bFlush = false;
            while (true)
            {
                int nFlag = Global.GetRoleParamsInt32FromDB(client, RoleParamName.HeFuLoginFlag);
                int nValue = Global.GetIntSomeBit(nFlag, (int)HeFuLoginFlagTypes.HeFuLogin_Login);
                // 是否在活动期间登陆过
                if (nValue == 0)
                {
                    break;
                }

                // 检查是否已经领取普通奖励
                nValue = Global.GetIntSomeBit(nFlag, (int)HeFuLoginFlagTypes.HeFuLogin_NormalAward);
                if (nValue == 0)
                {
                    bFlush = true;
                    break;
                }

                // 如果普通奖励已经领取则检测是否领取vip奖励
                nValue = Global.GetIntSomeBit(nFlag, (int)HeFuLoginFlagTypes.HeFuLogin_VIPAward);
                // 没领取判断是不是vip
                if (nValue == 0)
                {
                    // 判断玩家是不是VIP
                    if (Global.IsVip(client))
                    {
                        bFlush = true;
                        break;
                    }
                }
                break;
            }

            return AddFlushIconState((ushort)ActivityTipTypes.HeFuLogin, bFlush); ;
        }

        /// <summary>
        /// 检查合服累计登陆领取标记
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool CheckHeFuTotalLogin(GameClient client)
        {
            HeFuTotalLoginActivity activity = HuodongCachingMgr.GetHeFuTotalLoginActivity();
            if (null == activity)
            {
                return false;
            }
            // 检查是否在允许领取的时间内
            if (!activity.InAwardTime())
            {
                return false;
            }

            bool bFlush = false;
            while (true)
            { 
                

                // 玩家登陆的总数
                int totalloginnum = Global.GetRoleParamsInt32FromDB(client, RoleParamName.HeFuTotalLoginNum);
                // 依次检查是否有满足条件的没领取的奖励
                for (int i = 1; i <= totalloginnum; i++)
                {
                    if (activity.GetAward(i) == null)
                        continue;

                    int nFlag = Global.GetRoleParamsInt32FromDB(client, RoleParamName.HeFuTotalLoginFlag);
                    //int nValue = nFlag & Global.GetBitValue(i);
                    int nValue = Global.GetIntSomeBit(nFlag, i);
                    // 发现有一天没领取
                    if (nValue == 0)
                    {
                        bFlush = true;
                        break;
                    }
                }
                break;
            }
            
            return AddFlushIconState((ushort)ActivityTipTypes.HeFuTotalLogin, bFlush); ;
        }

        /// <summary>
        /// 检查用户合服充值豪礼状态
        /// 会向GameDBServer申请数据库查询，请避免在同一时间点一起申请
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool CheckHeFuRecharge(GameClient client)
        {
            int currday = Global.GetOffsetDay(DateTime.Now);
            int hefuday = Global.GetOffsetDay(Global.GetHefuStartDay());
            // 活动开始的第一天没有数据
            if (currday == hefuday)
            {
                return false;
            }
            HeFuRechargeActivity activity = HuodongCachingMgr.GetHeFuRechargeActivity();
            if (null == activity)
            {
                return false;
            }
            if (!activity.InActivityTime() && !activity.InAwardTime())
            {
                return false;
            }

            bool bFlush = false;
            while (true)
            { 
                string[] dbFields = null;
                // 向DB申请数据
                TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, (int)TCPGameServerCmds.CMD_DB_QUERY_REPAYACTIVEINFO, string.Format("{0}:{1}:{2}:{3}", client.ClientData.RoleID, (int)ActivityTypes.HeFuRecharge, hefuday, activity.strcoe), out dbFields);
                if (null == dbFields)
                    break;

                if (null == dbFields || 1 != dbFields.Length)
                    break;

                string[] strrebate = dbFields[0].Split('|');
                if (1 > dbFields.Length)
                    break;

                bFlush = Convert.ToInt32(strrebate[0]) > 0;
                break;
            }
            
            return AddFlushIconState((ushort)ActivityTipTypes.HeFuRecharge, bFlush);
        }

        /// <summary>
        /// 检查用户PK之王领奖状态
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool CheckHeFuPKKing(GameClient client)
        {
            HeFuPKKingActivity activity = HuodongCachingMgr.GetHeFuPKKingActivity();
            if (null == activity)
            {
                return false;
            }
            // 检查是否在允许领取的时间内
            if (!activity.InAwardTime())
            {
                return false;
            }

            bool bFlush = false;
            while (true)
            { 
                // 判断玩家是不是战场之神
                if (client.ClientData.RoleID != HuodongCachingMgr.GetHeFuPKKingRoleID())
                {
                    break;
                }

                // 判断是否已经领取
                int nFlag = Global.GetRoleParamsInt32FromDB(client, RoleParamName.HeFuPKKingFlag);
                if (nFlag == 0)
                {
                    bFlush = true;
                    break;
                }
                break;
            }
            return AddFlushIconState((ushort)ActivityTipTypes.HeFuPKKing, bFlush); ;
        }

        public bool CheckCaiJiState(GameClient client)
        {
            //水晶幻境采集状态检测
            return AddFlushIconState((ushort)ActivityTipTypes.ShuiJingHuangJin, CaiJiLogic.HasLeftnum(client));
        }

        /// <summary>
        /// 检查用户节日活动总叹号
        /// </summary>
        public bool CheckJieRiActivity(GameClient client, bool isLogin)
        {
            if (isLogin)
            { 
                AddFlushIconState((ushort)ActivityTipTypes.JieRiLogin, false);
                AddFlushIconState((ushort)ActivityTipTypes.JieRiTotalLogin, false);
                AddFlushIconState((ushort)ActivityTipTypes.JieRiDayCZ, false);
                AddFlushIconState((ushort)ActivityTipTypes.JieRiLeiJiXF, false);
                AddFlushIconState((ushort)ActivityTipTypes.JieRiLeiJiCZ, false);
                AddFlushIconState((ushort)ActivityTipTypes.JieRiCZKING, false);
                AddFlushIconState((ushort)ActivityTipTypes.JieRiXFKING, false);            
            }

            bool bFlush = false;
            // 节日登陆
            bFlush = bFlush | CheckJieRiLogin(client);
            // 节日累计登陆
            bFlush = bFlush | CheckJieRiTotalLogin(client);
            // 节日每日充值
            bFlush = bFlush | CheckJieRiDayCZ(client);
            // 节日累计消费
            bFlush = bFlush | CheckJieRiLeiJiXF(client);
            // 节日累计充值
            bFlush = bFlush | CheckJieRiLeiJiCZ(client);
            // 节日充值王
            bFlush = bFlush | CheckJieRiCZKING(client);
            // 节日消费王
            bFlush = bFlush | CheckJieRiXFKING(client);

            return AddFlushIconState((ushort)ActivityTipTypes.JieRiActivity, bFlush); 
        }

        /// <summary>
        /// 节日登陆
        /// </summary>
        public bool CheckJieRiLogin(GameClient client)
        {
            JieriDaLiBaoActivity activity = HuodongCachingMgr.GetJieriDaLiBaoActivity();
            if (null == activity)
            {
                return false;
            }
            if (!activity.InActivityTime() && !activity.InAwardTime())
            {
                return false;
            }

            bool bFlush = false;
            while (true)
            { 
                string[] dbFields = null;
                // 向DB申请数据
                TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 
                    (int)TCPGameServerCmds.CMD_SPR_QUERYJIERIDALIBAO, Global.GetActivityRequestCmdString(ActivityTypes.JieriDaLiBao, client), out dbFields);
                if (null == dbFields)
                    break;

                // strcmd = string.Format("{0}:{1}:{2}", 1, roleID, hasgettimes);
                if (null == dbFields || 3 != dbFields.Length)
                    break;
                
                int hasgettimes = Convert.ToInt32(dbFields[2]);

                if (hasgettimes == 0)
                    bFlush = true;

                break;
            }

            return AddFlushIconState((ushort)ActivityTipTypes.JieRiLogin, bFlush);
        }

        /// <summary>
        /// 节日累计登陆
        /// </summary>
        public bool CheckJieRiTotalLogin(GameClient client)
        {
            JieRiDengLuActivity activity = HuodongCachingMgr.GetJieRiDengLuActivity();
            if (null == activity)
            {
                return false;
            }
            if (!activity.InAwardTime())
            {
                return false;
            }

            bool bFlush = false;
            while (true)
            {
                string[] dbFields = null;
                // 向DB申请数据
                TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 
                    (int)TCPGameServerCmds.CMD_SPR_QUERYJIERIDENGLU, Global.GetActivityRequestCmdString(ActivityTypes.JieriDengLuHaoLi, client), out dbFields);
                if (null == dbFields)
                    break;

                // hasgettimes |= (1 << (extTag - 1));
                // strcmd = string.Format("{0}:{1}:{2}:{3}", 1, roleID, hasgettimes, dengLuTimes);
                if (null == dbFields || 4 != dbFields.Length)
                    break;
                
                int hasgettimes = Convert.ToInt32(dbFields[2]);
                int dengLuTimes = Convert.ToInt32(dbFields[3]);

                // 依次检查是否有满足条件的没领取的奖励
                for (int i = 0; i < dengLuTimes; i++)
                {
                    if (activity.GetAward(client, i + 1) == null)
                        continue;

                    int nValue = Global.GetIntSomeBit(hasgettimes, i);
                    // 发现有一天没领取
                    if (nValue == 0)
                    {
                        bFlush = true;
                        break;
                    }
                }
                break;
            }

            return AddFlushIconState((ushort)ActivityTipTypes.JieRiTotalLogin, bFlush);
        }

        /// <summary>
        /// 节日每日充值
        /// </summary>
        public bool CheckJieRiDayCZ(GameClient client)
        {
            JieriCZSongActivity activity = HuodongCachingMgr.GetJieriCZSongActivity();
            if (null == activity)
            {
                return false;
            }
            if (!activity.InAwardTime())
            {
                return false;
            }

            bool bFlush = false;
            while (true)
            {
                string[] dbFields = null;
                // 向DB申请数据
                TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool,
                    (int)TCPGameServerCmds.CMD_SPR_QUERYJIERICZSONG, Global.GetActivityRequestCmdString(ActivityTypes.JieriCZSong, client), out dbFields);
                if (null == dbFields)
                    break;

                // hasgettimes |= (1 << (extTag - 1));
                // strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", 1, roleID, minYuanBao, roleYuanBaoInPeriod, hasgettimes);
                if (null == dbFields || 5 != dbFields.Length)
                    break;

                int roleYuanBaoInPeriod = Convert.ToInt32(dbFields[3]);
                int hasgettimes = Convert.ToInt32(dbFields[4]);

                foreach (KeyValuePair<int, AwardItem> item in activity.AwardItemDict)
                { 
                    // 满足领取条件
                    if (roleYuanBaoInPeriod >= item.Value.MinAwardCondionValue)
                    {
                        // 判断是否领取
                        int nValue = Global.GetIntSomeBit(hasgettimes, item.Key - 1);
                        // 发现有一天没领取
                        if (nValue == 0)
                        {
                            bFlush = true;
                            break;
                        }
                    }
                }
                break;
            }

            return AddFlushIconState((ushort)ActivityTipTypes.JieRiDayCZ, bFlush);
        }

        /// <summary>
        /// 节日累计消费
        /// <summary>
        public bool CheckJieRiLeiJiXF(GameClient client)
        {
            JieRiTotalConsumeActivity activity = HuodongCachingMgr.GetJieRiTotalConsumeActivity();
            if (null == activity)
            {
                return false;
            }
            if (!activity.InAwardTime())
            {
                return false;
            }

            bool bFlush = false;
            while (true)
            {
                string[] dbFields = null;
                // 向DB申请数据
                TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool,
                    (int)TCPGameServerCmds.CMD_SPR_QUERYJIERITOTALCONSUME, Global.GetActivityRequestCmdString(ActivityTypes.JieriTotalConsume, client), out dbFields);
                if (null == dbFields)
                    break;

                // hasgettimes |= (1 << (extTag - 1));
                // strcmd = string.Format("{0}:{1}:{2}:{3}", 1, roleID, roleYuanBaoInPeriod, hasgettimes);
                if (null == dbFields || 4 != dbFields.Length)
                    break;

                int roleYuanBaoInPeriod = Convert.ToInt32(dbFields[2]);
                int hasgettimes = Convert.ToInt32(dbFields[3]);

                foreach (KeyValuePair<int, AwardItem> item in activity.AwardItemDict)
                { 
                    // 满足领取条件
                    if (roleYuanBaoInPeriod >= item.Value.MinAwardCondionValue)
                    {
                        // 判断是否领取
                        int nValue = Global.GetIntSomeBit(hasgettimes, item.Key - 1);
                        // 发现有一天没领取
                        if (nValue == 0)
                        {
                            bFlush = true;
                            break;
                        }
                    }
                }

                break;
            }

            return AddFlushIconState((ushort)ActivityTipTypes.JieRiLeiJiXF, bFlush);
        }

        /// <summary>
        /// 节日累计充值 
        /// </summary>
        public bool CheckJieRiLeiJiCZ(GameClient client)
        {
            JieRiLeiJiCZActivity activity = HuodongCachingMgr.GetJieRiLeiJiCZActivity();
            if (null == activity)
            {
                return false;
            }
            if (!activity.InAwardTime())
            {
                return false;
            }

            bool bFlush = false;
            while (true)
            {
                string[] dbFields = null;
                // 向DB申请数据
                TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool,
                    (int)TCPGameServerCmds.CMD_SPR_QUERYJIERICZLEIJI, Global.GetActivityRequestCmdString(ActivityTypes.JieriLeiJiCZ, client), out dbFields);
                if (null == dbFields)
                    break;

                // hasgettimes |= (1 << (extTag - 1));
                // strcmd = string.Format("{0}:{1}:{2}:{3}", 1, roleID, roleYuanBaoInPeriod, hasgettimes);
                if (null == dbFields || 4 != dbFields.Length)
                    break;

                int roleYuanBaoInPeriod = Convert.ToInt32(dbFields[2]);
                int hasgettimes = Convert.ToInt32(dbFields[3]);

                foreach (KeyValuePair<int, AwardItem> item in activity.AwardItemDict)
                { 
                    // 满足领取条件
                    if (roleYuanBaoInPeriod >= item.Value.MinAwardCondionValue)
                    {
                        // 判断是否领取
                        int nValue = Global.GetIntSomeBit(hasgettimes, item.Key - 1);
                        // 发现有一天没领取
                        if (nValue == 0)
                        {
                            bFlush = true;
                            break;
                        }
                    }
                }
                break;
            }

            return AddFlushIconState((ushort)ActivityTipTypes.JieRiLeiJiCZ, bFlush);
        }

        /// <summary>
        /// 节日充值王
        /// </summary>
        public bool CheckJieRiCZKING(GameClient client)
        {
            KingActivity activity = HuodongCachingMgr.GetJieRiCZKingActivity();
            if (null == activity)
            {
                return false;
            }
            if (!activity.InAwardTime())
            {
                return false;
            }
            bool bFlush = false;
            while (true)
            {
                string[] dbFields = null;
                // 向DB申请数据
                TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool,
                    (int)TCPGameServerCmds.CMD_SPR_QUERYJIERICZKING, Global.GetActivityRequestCmdString(ActivityTypes.JieriPTCZKing, client, 1/*查询是否有奖励的标记*/), out dbFields);

                // string strCmd = string.Format("{0}:{1}:{2}", 1, roleID, (paiHang > 0 && hasgettimes > 0) ? "1" : "0");
                if (null == dbFields || 3 != dbFields.Length)
                    break;

                int result = Convert.ToInt32(dbFields[0]);
                int roleid = Convert.ToInt32(dbFields[1]);
                int hasgettimes = Convert.ToInt32(dbFields[2]);

                if (1 != result)
                    break;
 
                if (roleid != client.ClientData.RoleID)
                    break;

                bFlush = (hasgettimes == 1);
                break;
            }
            return AddFlushIconState((ushort)ActivityTipTypes.JieRiCZKING, bFlush);
        }

        /// <summary>
        /// 节日消费王
        /// </summary>
        public bool CheckJieRiXFKING(GameClient client)
        {
            KingActivity activity = HuodongCachingMgr.GetJieriXiaoFeiKingActivity();
            if (null == activity)
            {
                return false;
            }
            if (!activity.InAwardTime())
            {
                return false;
            }

            bool bFlush = false;
            while (true)
            {
                //CMD_SPR_QUERYJIERIXIAOFEIKING
                string[] dbFields = null;
                // 向DB申请数据
                TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool,
                    (int)TCPGameServerCmds.CMD_SPR_QUERYJIERIXIAOFEIKING, Global.GetActivityRequestCmdString(ActivityTypes.JieriPTXiaoFeiKing, client, 1/*查询是否有奖励的标记*/), out dbFields);
                // string strCmd = string.Format("{0}:{1}:{2}", 1, roleID, (paiHang > 0 && hasgettimes == 0) ? "1" : "0");
                if (null == dbFields || 3 != dbFields.Length)
                    break;

                int result = Convert.ToInt32(dbFields[0]);
                int roleid = Convert.ToInt32(dbFields[1]);
                int hasgettimes = Convert.ToInt32(dbFields[2]);

                if (1 != result)
                    break;

                if (roleid != client.ClientData.RoleID)
                    break;

                bFlush = (hasgettimes == 1);
                break;
            }
            return AddFlushIconState((ushort)ActivityTipTypes.JieRiXFKING, bFlush);
        }

        public bool CheckGuildIcon(GameClient client, bool isLogin)
        {
            if (isLogin)
            {
                AddFlushIconState((ushort)ActivityTipTypes.GuildIcon, false);
                AddFlushIconState((ushort)ActivityTipTypes.GuildCopyMap, false);
            }

            bool bFlush = false;
            // 战盟副本
            bFlush = bFlush | CheckGuildCopyMap(client);

            return AddFlushIconState((ushort)ActivityTipTypes.GuildIcon, bFlush); 
        }

        /// <summary>
        /// 战盟副本
        /// </summary>
        public bool CheckGuildCopyMap(GameClient client)
        {
            bool bFlush = false;
            int mapid = -1;
            int seqid = -1;
            int mapcode = -1;
            // 查找本周副本通关情况，mapid返回打到第几个副本了
            GameManager.GuildCopyMapMgr.CheckCurrGuildCopyMap(client, out mapid, out seqid, mapcode);
            if (mapid < 0)
            {
                return false;
            }

            int nGuildCopyMapAwardFlag = Global.GetRoleParamsInt32FromDB(client, RoleParamName.GuildCopyMapAwardFlag);
            for (int i = 0; i < GameManager.GuildCopyMapMgr.GuildCopyMapOrderList.Count; i++)
            {
                int fubenID = GameManager.GuildCopyMapMgr.GuildCopyMapOrderList[i];
                // 如果没通关 就提示叹号
                if (mapid != 0)
                {
                    bFlush = true;
                    break;
                }
                // 不符合领取条件
                if (mapid > 0 && fubenID >= mapid)
                {
                    break;
                }
                bool flag = GameManager.GuildCopyMapMgr.GetGuildCopyMapAwardDayFlag(nGuildCopyMapAwardFlag, i, 2);
                if (flag == false)
                {
                    bFlush = true;
                    break;
                }
            }
            return AddFlushIconState((ushort)ActivityTipTypes.GuildCopyMap, bFlush);
        }
    }
}
