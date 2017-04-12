using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;
using Server.Data;
using GameDBServer.Logic;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 获取竞技场数据指令处理器
    /// </summary>
    public class JingJiGetDataCmdProcessor : ICmdProcessor
    {
        private static JingJiGetDataCmdProcessor instance = new JingJiGetDataCmdProcessor();

        private JingJiGetDataCmdProcessor() { }

        public static JingJiGetDataCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);

            PlayerJingJiData data = JingJiChangManager.getInstance().getPlayerJingJiDataById(roleId);

            client.sendCmd<PlayerJingJiData>((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_GET_DATA, data);
        }
    }
}
