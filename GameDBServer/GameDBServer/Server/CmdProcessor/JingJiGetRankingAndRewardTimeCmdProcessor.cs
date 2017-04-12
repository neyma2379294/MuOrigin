using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;
using GameDBServer.Logic;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 获取排名和上次领取奖励时间指令处理器
    /// </summary>
    public class JingJiGetRankingAndRewardTimeCmdProcessor : ICmdProcessor
    {
        private static JingJiGetRankingAndRewardTimeCmdProcessor instance = new JingJiGetRankingAndRewardTimeCmdProcessor();

        private JingJiGetRankingAndRewardTimeCmdProcessor() { }

        public static JingJiGetRankingAndRewardTimeCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);

            int ranking = -2;
            long nextRewardTime = 0;

            JingJiChangManager.getInstance().getRankingAndNextRewardTimeById(roleId, out ranking, out nextRewardTime);

            long[] resultParams = new long[]{ranking, nextRewardTime};

            client.sendCmd<long[]>((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_GET_RANKING_AND_NEXTREWARDTIME, resultParams);
        }
    }
}
