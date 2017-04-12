using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;
using GameDBServer.DB;
using Server.Data;
using GameDBServer.Logic;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 战盟建筑升级事件处理
    /// </summary>
    public class ZhanMengBuildUpLevelCmdProcessor : ICmdProcessor
    {
        private static ZhanMengBuildUpLevelCmdProcessor instance = new ZhanMengBuildUpLevelCmdProcessor();

        private ZhanMengBuildUpLevelCmdProcessor()  //此函数默认不会出发，必须在CmdReigsterTriggerManager中注册下
        {
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_SPR_ZHANMENGBUILDUPLEVEL, this);
        }

        public static ZhanMengBuildUpLevelCmdProcessor getInstance()
        {
            return instance;
        }

        private bool CheckHaveUpGradeItem(String strReqItem, DBManager dbMgr, int nBangHuiID, int nRoleID, int nToLevel)
        {
            BangHuiBagData dataBangHuiBag = DBQuery.QueryBangHuiBagDataByID(dbMgr, nBangHuiID);

            String[] arrReqItems = strReqItem.Split('|');
            int[] arrItemNums = new int[5];
            for (int i = 0; i < arrItemNums.Length; i++)
            {
                arrItemNums[i] = 0;
            }

            for (int i = 0; i < arrReqItems.Length; i++)
            {
                String[] arrItemInfo = arrReqItems[i].Split(',');
                if (2 != arrItemInfo.Length)
                {
                    continue;
                }

                arrItemNums[i] = int.Parse(arrItemInfo[1]);
            }

            if (dataBangHuiBag.Goods1Num < arrItemNums[0])
            {
                return false;
            }

            if (dataBangHuiBag.Goods2Num < arrItemNums[1])
            {
                return false;
            }

            if (dataBangHuiBag.Goods3Num < arrItemNums[2])
            {
                return false;
            }

            if (dataBangHuiBag.Goods4Num < arrItemNums[3])
            {
                return false;
            }

            if (dataBangHuiBag.Goods5Num < arrItemNums[4])
            {
                return false;
            }

            DBWriter.UpdateBangHuiQiLevel(dbMgr, nBangHuiID, nToLevel, arrItemNums[0], arrItemNums[1], arrItemNums[2], arrItemNums[3], arrItemNums[4], 0);
            return true;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            string cmdData = null;
            int nID = (int)TCPGameServerCmds.CMD_SPR_ZHANMENGBUILDUPLEVEL;

            try
            {
                cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
            }
            catch (Exception) //解析错误
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID));
                client.sendCmd((int)TCPGameServerCmds.CMD_DB_ERR_RETURN, "0");
                return;
            }

            string[] fields = cmdData.Split(':');
            if (fields.Length != 7)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}",
                    (TCPGameServerCmds)nID, fields.Length, cmdData));

                client.sendCmd((int)TCPGameServerCmds.CMD_DB_ERR_RETURN, "0");
                return;
            }

            int roleID = Convert.ToInt32(fields[0]);
            int bhid = Convert.ToInt32(fields[1]);
            int buildType = Convert.ToInt32(fields[2]);
            int levelupCost = Convert.ToInt32(fields[3]);
            int toLevel = Convert.ToInt32(fields[4]);
            int initCoin = Convert.ToInt32(fields[5]);
            String strReqGoods = fields[6];

            DBManager dbMgr = DBManager.getInstance();

            string strcmd = "";
            DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(roleID);
            if (null == dbRoleInfo)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}",
                    (TCPGameServerCmds)nID, roleID));

                client.sendCmd((int)TCPGameServerCmds.CMD_DB_ERR_RETURN, "0");
                return;
            }

            BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
            if (null == bangHuiDetailData)
            {
                //添加任务失败
                strcmd = string.Format("{0}", -1000);
                client.sendCmd(nID, strcmd);
                return;
            }

            if (roleID != bangHuiDetailData.BZRoleID)
            {
                //只有帮主才能升级帮会旗帜，客户端不用处理这个返回结果，用于避免外挂指令
                strcmd = string.Format("{0}", -9368);
                client.sendCmd(nID, strcmd);
                return;
            }

            int currentLevel = bangHuiDetailData.QiLevel;
            if (buildType == (int)ZhanMengBuilds.JiTan)
            {
                currentLevel = bangHuiDetailData.JiTan;
            }
            else if (buildType == (int)ZhanMengBuilds.JunXie)
            {
                currentLevel = bangHuiDetailData.JunXie;
            }
            else if (buildType == (int)ZhanMengBuilds.GuangHuan)
            {
                currentLevel = bangHuiDetailData.GuangHuan;
            }

            if (currentLevel + 1 != toLevel)
            {
                //添加任务失败
                strcmd = string.Format("{0}", -1005);
                client.sendCmd(nID, strcmd);
                return;
            }

            // 战盟的初始资金是不能被消耗的 ChenXiaojun
            if (bangHuiDetailData.TotalMoney < (levelupCost + initCoin))
            {
                //添加任务失败
                strcmd = string.Format("{0}", -1120);
                client.sendCmd(nID, strcmd);
                return;
            }

            if (bangHuiDetailData.TotalMoney < levelupCost)
            {
                //添加任务失败
                strcmd = string.Format("{0}", -1110);
                client.sendCmd(nID, strcmd);
                return;
            }

            if (!CheckHaveUpGradeItem(strReqGoods, dbMgr, bhid, roleID, toLevel))
            {
                strcmd = string.Format("{0}", -1210);
                client.sendCmd(nID, strcmd);
                return;
            }

            string fieldName = "qilevel";
            if (buildType == (int)ZhanMengBuilds.JiTan)
            {
                fieldName = "jitan";
            }
            else if (buildType == (int)ZhanMengBuilds.JunXie)
            {
                fieldName = "junxie";
            }
            else if (buildType == (int)ZhanMengBuilds.GuangHuan)
            {
                fieldName = "guanghuan";
            }

            //升级帮旗级别，同时扣除库存
            DBWriter.UpdateZhanMengBuildLevel(dbMgr, bhid, toLevel, levelupCost, fieldName);

            if (buildType == (int)ZhanMengBuilds.ZhanQi)
            {
                //修改内存字典中的军旗的级别
                GameDBManager.BangHuiJunQiMgr.UpdateBangHuiQiLevel(bhid, toLevel);
            }

            strcmd = string.Format("{0}", 0);
            client.sendCmd(nID, strcmd);
        }
    }
}

