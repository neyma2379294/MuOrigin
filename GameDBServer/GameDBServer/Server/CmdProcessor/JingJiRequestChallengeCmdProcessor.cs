using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;
using GameDBServer.Logic;
using Server.Data;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 竞技场请求挑战指令处理器
    /// </summary>
    public class JingJiRequestChallengeCmdProcessor : ICmdProcessor
    {
        private static JingJiRequestChallengeCmdProcessor instance = new JingJiRequestChallengeCmdProcessor();

        private JingJiRequestChallengeCmdProcessor() { }

        public static JingJiRequestChallengeCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            int[] _cmdParams = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);

            //挑战者ID
            int challengerId = _cmdParams[0];

            //被挑战者ID
            int beChallengerId = _cmdParams[1];

            //被挑战者排名
            int beChallengerRanking = _cmdParams[2];

            JingJiBeChallengeData result = JingJiChangManager.getInstance().requestChallenge(challengerId, beChallengerId, beChallengerRanking);

            client.sendCmd<JingJiBeChallengeData>((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_REQUEST_CHALLENGE, result);
        }
    }
}
