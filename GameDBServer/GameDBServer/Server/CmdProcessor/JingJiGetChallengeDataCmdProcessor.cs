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
    /// 获取挑战者数据
    /// </summary>
    public class JingJiGetChallengeDataCmdProcessor : ICmdProcessor
    {
        private static JingJiGetChallengeDataCmdProcessor instance = new JingJiGetChallengeDataCmdProcessor();

        private JingJiGetChallengeDataCmdProcessor() { }

        public static JingJiGetChallengeDataCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            int[] challengeRankings = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);

            List<PlayerJingJiMiniData> miniDatas = JingJiChangManager.getInstance().getChallengeData(challengeRankings);

            client.sendCmd<List<PlayerJingJiMiniData>>((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_GET_CHALLENGE_DATA, miniDatas);
        }
    }
}
