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
    /// 创建竞技场数据
    /// </summary>
    public class JingJiCreateDataCmdProcessor : ICmdProcessor
    {
        private static JingJiCreateDataCmdProcessor instance = new JingJiCreateDataCmdProcessor();

        private JingJiCreateDataCmdProcessor(){}

        public static JingJiCreateDataCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            PlayerJingJiData data = DataHelper.BytesToObject<PlayerJingJiData>(cmdParams, 0, count);

            if(null != data)
            {
                if(!JingJiChangManager.getInstance().createRobotData(data))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("JingJiChangCreateDataCmdProcessor.processCmd， 创建竞技场数据失败, roleId={0}", data.roleId));
                }  
            }
            else
            {
                LogManager.WriteLog(LogTypes.Error, "JingJiChangCreateDataCmdProcessor.processCmd， 竞技场数据解析失败");
            }

            client.sendCmd<byte>((int)TCPGameServerCmds.CMD_DB_JINGJICHANG_CREATE_DATA, (byte)0);
        }
    }
}
