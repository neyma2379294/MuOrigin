using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Logic.MUWings;
using GameServer.Logic;

namespace GameServer.Server.CmdProcesser
{
    /// <summary>
    /// 翅膀进阶指令
    /// </summary>
    public class WingUpgradeCmdProcessor : ICmdProcessor
    {
        private static WingUpgradeCmdProcessor instance = new WingUpgradeCmdProcessor();

        private WingUpgradeCmdProcessor() 
        {
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_SPR_WINGUPGRADE, 2, this);
        }

        public static WingUpgradeCmdProcessor getInstance()
        {
            return instance;
        }

        public bool processCmd(Logic.GameClient client, string[] cmdParams)
        {
            int nID = (int)TCPGameServerCmds.CMD_SPR_WINGUPGRADE;
            int nRoleID = Global.SafeConvertToInt32(cmdParams[0]);
            int nUpWingMode = Global.SafeConvertToInt32(cmdParams[1]); //0: 道具进阶, 1: 钻石进阶

            string strCmd = "";
            if (null == client.ClientData.MyWingData)
            {
                strCmd = string.Format("{0}:{1}:{2}:{3}", -3, nRoleID, 0, 0);
                client.sendCmd(nID, strCmd);
                return true;
            }

            // 已到最高级
            if (client.ClientData.MyWingData.WingID >= MUWingsManager.MaxWingID)
            {
                strCmd = string.Format("{0}:{1}:{2}:{3}", -8, nRoleID, 0, 0);
                client.sendCmd(nID, strCmd);
                return true;
            }

            // 获取进阶信息表
            SystemXmlItem upStarXmlItem = MUWingsManager.GetWingUPCacheItem(client.ClientData.MyWingData.WingID + 1);
            if (null == upStarXmlItem)
            {
                strCmd = string.Format("{0}:{1}:{2}:{3}", -3, nRoleID, 0, 0);
                client.sendCmd(nID, strCmd);
                return true;
            }

            if (0 == nUpWingMode)
            {
                // 获取进阶需要的道具的物品信息
                string strReqItemID = upStarXmlItem.GetStringValue("NeedGoods");

                // 解析物品ID与数量
                string[] itemParams = strReqItemID.Split(',');
                if (null == itemParams || itemParams.Length != 2)
                {
                    strCmd = string.Format("{0}:{1}:{2}:{3}", -3, nRoleID, 0, 0);
                    client.sendCmd(nID, strCmd);
                    return true;
                }

                // 获取进阶需要的道具的物品ID
                int nEnchanceNeedGoodsID = Convert.ToInt32(itemParams[0]);
                // 获取进阶需要的道具的物品数量
                int nEnchanceNeedGoodsNum = Convert.ToInt32(itemParams[1]);

                if (nEnchanceNeedGoodsID <= 0 || nEnchanceNeedGoodsNum <= 0)
                {
                    strCmd = string.Format("{0}:{1}:{2}:{3}", -3, nRoleID, 0, 0);
                    client.sendCmd(nID, strCmd);
                    return true;
                }

                // 判断数量是否够
                if (Global.GetTotalGoodsCountByID(client, nEnchanceNeedGoodsID) < nEnchanceNeedGoodsNum)
                {
                    // 物品数量不够
                    strCmd = string.Format("{0}:{1}:{2}:{3}", -4, nRoleID, 0, 0);
                    client.sendCmd(nID, strCmd);
                    return true;
                }

                // 自动扣除物品
                if (nEnchanceNeedGoodsNum > 0)
                {
                    bool bUsedBinding = false;
                    bool bUsedTimeLimited = false;

                    //扣除物品
                    if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener,
                        Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nEnchanceNeedGoodsID, nEnchanceNeedGoodsNum, false, out bUsedBinding, out bUsedTimeLimited))
                    {
                        strCmd = string.Format("{0}:{1}:{2}:{3}", -5, nRoleID, 0, 0);
                        client.sendCmd(nID, strCmd);
                        return true;
                    }
                }
            }
            else
            {
                // 获取进阶需要的钻石数量
                int nReqZuanShi = upStarXmlItem.GetIntValue("NeedZuanShi");
                if (nReqZuanShi <= 0)
                {
                    strCmd = string.Format("{0}:{1}:{2}:{3}", -3, nRoleID, 0, 0);
                    client.sendCmd(nID, strCmd);
                    return true;
                }
                // 判断用户点卷额是否不足【钻石】
                if (client.ClientData.UserMoney < nReqZuanShi)
                {
                    //用户点卷额不足【钻石】
                    strCmd = string.Format("{0}:{1}:{2}:{3}", -6, nRoleID, 0, 0);
                    client.sendCmd(nID, strCmd);
                    return true;
                }

                //先DBServer请求扣费
                //扣除用户点卷
                if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nReqZuanShi, "翅膀进阶"))
                {
                    //扣除用户点卷失败
                    strCmd = string.Format("{0}:{1}:{2}:{3}", -7, nRoleID, 0, 0);
                    client.sendCmd(nID, strCmd);
                    return true;
                }
            }

            int nLuckOne = upStarXmlItem.GetIntValue("LuckyOne");
            int nLuckyTwo = upStarXmlItem.GetIntValue("LuckyTwo");
            int nLuckTwoRate = (int)(upStarXmlItem.GetDoubleValue("LuckyTwoRate") * 100.0);

            int nNextWingID = client.ClientData.MyWingData.WingID;
            int nNextJinJieFailedNum = client.ClientData.MyWingData.JinJieFailedNum;
            int nNextStarLevel = client.ClientData.MyWingData.ForgeLevel;
            int nNextStarExp = client.ClientData.MyWingData.StarExp;


            // LuckyOne+提升获得幸运点 < LuckyTwo;必定不会提升成功
            if (nLuckOne + client.ClientData.MyWingData.JinJieFailedNum < nLuckyTwo)
            {
                // 幸运点加1
                nNextJinJieFailedNum++;
            }
            // LuckyOne+提升获得幸运点>= LuckyTwo,则根据配置得到LuckyTwoRate概率判定是否能够完成进阶操作
            else if (nLuckOne + client.ClientData.MyWingData.JinJieFailedNum < 110000)
            {
                // 
                int nRandNum = Global.GetRandomNumber(0, 100);
                if (nRandNum < nLuckTwoRate)
                {
                    // 进阶成功
                    nNextWingID++;

                    // 幸运点清0
                    nNextJinJieFailedNum = 0;

                    // 星级清0
                    nNextStarLevel = 0;

                    // 星级经验清0
                    nNextStarExp = 0;
                }
                else
                {
                    // 幸运点加1
                    nNextJinJieFailedNum++;
                }
            }
            // LuckyOne+提升获得幸运点>=110000时，进阶必定成功
            else
            {
                // 进阶成功
                nNextWingID++;

                // 幸运点清0
                nNextJinJieFailedNum = 0;
              
                // 星级清0
                nNextStarLevel = 0;

                // 星级经验清0
                nNextStarExp = 0;
            }

            // 将改变保存到数据库
            int iRet = MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, nNextWingID, nNextJinJieFailedNum, nNextStarLevel, nNextStarExp);
            if (iRet < 0)
            {
                strCmd = string.Format("{0}:{1}:{2}:{3}", -3, nRoleID, 0, 0);
                client.sendCmd(nID, strCmd);
                return true;
            }
            else
            {
                strCmd = string.Format("{0}:{1}:{2}:{3}", 0, nRoleID, nNextWingID, nNextJinJieFailedNum);
                client.sendCmd(nID, strCmd);

                client.ClientData.MyWingData.JinJieFailedNum = nNextJinJieFailedNum;
                if (client.ClientData.MyWingData.WingID != nNextWingID)
                {
                    // 先移除原来的属性
                    if (1 == client.ClientData.MyWingData.Using)
                    {
                        MUWingsManager.UpdateWingDataProps(client, false);
                    }

                    client.ClientData.MyWingData.WingID = nNextWingID;
                    client.ClientData.MyWingData.ForgeLevel = 0;
                    client.ClientData.MyWingData.StarExp = 0;


                    // 按新等级增加属性
                    if (1 == client.ClientData.MyWingData.Using)
                    {
                        MUWingsManager.UpdateWingDataProps(client, true);

                        // 通知客户端属性变化
                        GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);

                        // 总生命值和魔法值变化通知(同一个地图才需要通知)
                        GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
                    }
                }
            }

            return true;
        }
    }
}
