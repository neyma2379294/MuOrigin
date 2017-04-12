using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;
using GameDBServer.Logic;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 更新下次领取竞技场排行榜奖励时间
    /// </summary>
    public class JingJiUpdateNextRewardTimeCmdProcessor : ICmdProcessor
    {
        private static JingJiUpdateNextRewardTimeCmdProcessor instance = new JingJiUpdateNextRewardTimeCmdProcessor();

        private JingJiUpdateNextRewardTimeCmdProcessor() { }

        public static JingJiUpdateNextRewardTimeCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            long[] _cmdParams = DataHelper.BytesToObject<long[]>(cmdParams, 0, count);

            int roleId = (int)_cmdParams[0];
            long nextRewardTime = _cmdParams[1];

            bool result = JingJiChangManager.getInstance().updateNextRewardTime(roleId, nextRewardTime);

            client.sendCmd<int>((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_UPDATE_NEXTREWARDTIME, result ? 1 : 0);
        }
    }
}
