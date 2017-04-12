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
    /// 获取竞技场排行榜数据指令处理器
    /// </summary>
    public class JingJiGetRankingListDataCmdProcessor : ICmdProcessor
    {
        private static JingJiGetRankingListDataCmdProcessor instance = new JingJiGetRankingListDataCmdProcessor();

        private JingJiGetRankingListDataCmdProcessor() { }

        public static JingJiGetRankingListDataCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            int pageIndex = DataHelper.BytesToObject<int>(cmdParams, 0, count);

            List<PlayerJingJiRankingData> rankingDatas = JingJiChangManager.getInstance().getRankingList(pageIndex);

            client.sendCmd<List<PlayerJingJiRankingData>>((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_GET_RANKINGLIST_DATA, rankingDatas);
        }
    }
}
