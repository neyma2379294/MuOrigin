using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Logic;
using GameDBServer.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 战盟事件详情指令处理器
    /// </summary>
    public class ZhanMengShiJianDetailCmdProcessor : ICmdProcessor
    {
        private static ZhanMengShiJianDetailCmdProcessor instance = new ZhanMengShiJianDetailCmdProcessor();

        private ZhanMengShiJianDetailCmdProcessor() { }

        public static ZhanMengShiJianDetailCmdProcessor getInstance()
        {
            return instance;
        }


        public void processCmd(GameServerClient client, byte[] cmdParams,int count)
        {
            int[] param = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
            int bhId = param[0];
            int pageIndex = param[1];

            client.sendCmd<List<ZhanMengShiJianData>>((int)TCPGameServerCmds.CMD_DB_ZHANMENGSHIJIAN_DETAIL, ZhanMengShiJianManager.getInstance().getDetailByPageIndex(bhId, pageIndex));
        }
    }
}
