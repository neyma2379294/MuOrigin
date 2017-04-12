using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;
using GameDBServer.DB;
using Server.Data;
using GameDBServer.Logic;
using GameDBServer.Logic.BaiTan;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 获取摆摊log命令处理
    /// </summary>
    public class BaiTanLogDetailCmdProcessor : ICmdProcessor
    {
        private static BaiTanLogDetailCmdProcessor instance = new BaiTanLogDetailCmdProcessor();

        private BaiTanLogDetailCmdProcessor()
        {
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_SPR_GETBAITANLOG, this);
        }

        public static BaiTanLogDetailCmdProcessor getInstance()
        {
            return instance;
        }


        public void processCmd(GameServerClient client, byte[] cmdParams,int count)
        {
            string cmdData = null;
            int nID = (int)TCPGameServerCmds.CMD_SPR_GETBAITANLOG;

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

            List<BaiTanLogItemData> list = new List<BaiTanLogItemData>();

            string[] fields = cmdData.Split(':');
            if (fields.Length != 2)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}",
                    (TCPGameServerCmds)nID, fields.Length, cmdData));

                client.sendCmd((int)TCPGameServerCmds.CMD_DB_ERR_RETURN, list);
                return;
            }

            int rid = Convert.ToInt32(fields[0]);
            int pageIndex = Convert.ToInt32(fields[1]);

            client.sendCmd<List<BaiTanLogItemData>>((int)TCPGameServerCmds.CMD_SPR_GETBAITANLOG, BaiTanManager.getInstance().getDetailByPageIndex(rid, pageIndex));
        }
    }
}
