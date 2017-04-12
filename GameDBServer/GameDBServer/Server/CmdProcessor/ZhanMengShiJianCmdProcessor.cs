using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Data;
using GameDBServer.Logic;
using Server.Tools;
using GameDBServer.DB;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 战盟事件指令处理器
    /// </summary>
    public class ZhanMengShiJianCmdProcessor : ICmdProcessor
    {
        private static ZhanMengShiJianCmdProcessor instance = new ZhanMengShiJianCmdProcessor();

        private ZhanMengShiJianCmdProcessor() { }

        public static ZhanMengShiJianCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            string cmd = new UTF8Encoding().GetString(cmdParams, 0, count);

            string[] param = cmd.Split(':');

            ZhanMengShiJianData data = new ZhanMengShiJianData();
            data.BHID = Convert.ToInt32(param[0]);
            data.RoleName = Convert.ToString(param[1]);
            data.ShiJianType = Convert.ToInt32(param[2]);
            data.SubValue1 = Convert.ToInt32(param[3]);
            data.SubValue2 = Convert.ToInt32(param[4]);
            data.SubValue3 = Convert.ToInt32(param[5]);
            data.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //需要特殊处理的战盟事件类型: 职务改变
            if (data.ShiJianType == ZhanMengShiJianConstants.ChangeZhiWu)
            {
                string otherRoleName;
                string otherUserID;
                Global.GetRoleNameAndUserID(DBManager.getInstance(), data.SubValue3, out otherRoleName, out otherUserID);
                data.RoleName = otherRoleName;
            }

            ZhanMengShiJianManager.getInstance().onAddZhanMengShiJian(data);

            byte[] arrSendData = DataHelper.ObjectToBytes<string>(string.Format("{0}", 1));
            client.sendCmd((int)TCPGameServerCmds.CMD_DB_ADD_ZHANMENGSHIJIAN, arrSendData);
        }
    }
}
