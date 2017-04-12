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
    /// 获取竞技场战报指令处理器
    /// </summary>
    public class JingJiGetChallengeInfoDataCmdProcessor : ICmdProcessor
    {
        private static JingJiGetChallengeInfoDataCmdProcessor instance = new JingJiGetChallengeInfoDataCmdProcessor();

        private JingJiGetChallengeInfoDataCmdProcessor() { }

        public static JingJiGetChallengeInfoDataCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            int[] _cmdParams = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);

            int roleId = _cmdParams[0];
            int pageIndex = _cmdParams[1];

            List<JingJiChallengeInfoData> rankingListData = JingJiChangManager.getInstance().getChallengeInfoDataList(roleId, pageIndex);

            client.sendCmd<List<JingJiChallengeInfoData>>((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_ZHANBAO_DATA, rankingListData);
        }
    }
}
