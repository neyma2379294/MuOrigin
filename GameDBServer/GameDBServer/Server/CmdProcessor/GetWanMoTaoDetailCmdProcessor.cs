using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Logic;
using GameDBServer.Data;
using Server.Tools;
using Server.Data;
using GameDBServer.Logic.WanMoTa;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 获取万魔塔详细信息
    /// </summary>
    public class GetWanMoTaoDetailCmdProcessor : ICmdProcessor
    {
        private static GetWanMoTaoDetailCmdProcessor instance = new GetWanMoTaoDetailCmdProcessor();

        private GetWanMoTaoDetailCmdProcessor() { }

        public static GetWanMoTaoDetailCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);

            WanMotaInfo data = WanMoTaManager.getInstance().getWanMoTaData(roleId);

            client.sendCmd<WanMotaInfo>((int)TCPGameServerCmds.CMD_DB_GET_WANMOTA_DETAIL, data);
        }
    }
}
