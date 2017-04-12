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
    public class ZhanMengBuildGetBufferCmdProcessor : ICmdProcessor
    {
        private static ZhanMengBuildGetBufferCmdProcessor instance = new ZhanMengBuildGetBufferCmdProcessor();

        private ZhanMengBuildGetBufferCmdProcessor()  //此函数默认不会出发，必须在CmdReigsterTriggerManager中注册下
        {
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_SPR_ZHANMENGBUILDGETBUFFER, this);
        }

        public static ZhanMengBuildGetBufferCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            string cmdData = null;
            int nID = (int)TCPGameServerCmds.CMD_SPR_ZHANMENGBUILDGETBUFFER;

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
            if (fields.Length != 5)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}",
                    (TCPGameServerCmds)nID, fields.Length, cmdData));

                client.sendCmd((int)TCPGameServerCmds.CMD_DB_ERR_RETURN, "0");
                return;
            }

            int roleID = Convert.ToInt32(fields[0]);
            int bhid = Convert.ToInt32(fields[1]);
            int buildType = Convert.ToInt32(fields[2]);
            int convertCost = Convert.ToInt32(fields[3]);
            int toLevel = Convert.ToInt32(fields[4]);

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

            if (dbRoleInfo.BangGong < Math.Abs(convertCost))
            {
                strcmd = string.Format("{0}", -1110);
                client.sendCmd(nID, strcmd);
                return;
            }

            if (!DBWriter.UpdateRoleBangGong(dbMgr, roleID, dbRoleInfo.BGDayID1, dbRoleInfo.BGMoney, dbRoleInfo.BGDayID2, dbRoleInfo.BGGoods, dbRoleInfo.BangGong - convertCost))
            {
                //添加任务失败
                strcmd = string.Format("{0}", -1110);
                client.sendCmd(nID, strcmd);
                return;
            }

            dbRoleInfo.BangGong -= Math.Abs(convertCost);
            strcmd = string.Format("{0}", 1);
            client.sendCmd(nID, strcmd);
        }
    }
}

