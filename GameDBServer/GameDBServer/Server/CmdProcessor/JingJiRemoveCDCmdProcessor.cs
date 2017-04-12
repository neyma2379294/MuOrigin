using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;
using GameDBServer.Logic;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 消除CD指令处理器
    /// </summary>
    public class JingJiRemoveCDCmdProcessor : ICmdProcessor
    {
        private static JingJiRemoveCDCmdProcessor instance = new JingJiRemoveCDCmdProcessor();

        private JingJiRemoveCDCmdProcessor() { }

        public static JingJiRemoveCDCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);

            bool result = JingJiChangManager.getInstance().removeCD(roleId);

            client.sendCmd<bool>((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_REMOVE_CD, result);
        }
    }
}
