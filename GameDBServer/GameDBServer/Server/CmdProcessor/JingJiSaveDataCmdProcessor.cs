using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using Server.Tools;
using GameDBServer.Logic;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 竞技场保存数据指令处理器
    /// </summary>
    public class JingJiSaveDataCmdProcessor : ICmdProcessor
    {
        private static JingJiSaveDataCmdProcessor instance = new JingJiSaveDataCmdProcessor();

        private JingJiSaveDataCmdProcessor() { }

        public static JingJiSaveDataCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            JingJiSaveData data = DataHelper.BytesToObject<JingJiSaveData>(cmdParams, 0, count);

            int winCount;

            JingJiChangManager.getInstance().saveData(data, out winCount);

            client.sendCmd<int>((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_SAVE_DATA, winCount);
        }
    }
}
