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
    /// 竞技场挑战结束消息
    /// </summary>
    public class JingJiChallengeEndCmdProcessor : ICmdProcessor
    {
        private static JingJiChallengeEndCmdProcessor instance = new JingJiChallengeEndCmdProcessor();

        private JingJiChallengeEndCmdProcessor() { }

        public static JingJiChallengeEndCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            JingJiChallengeResultData data = DataHelper.BytesToObject<JingJiChallengeResultData>(cmdParams, 0, count);

            int ranking = JingJiChangManager.getInstance().onChallengeEnd(data);

            client.sendCmd<int>((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_CHALLENGE_END, ranking);
        }
    }
}
