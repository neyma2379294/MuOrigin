using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;
using GameDBServer.DB;
using Server.Data;
using GameDBServer.Logic;
using GameDBServer.Logic.BaiTan;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 添加摆摊日志
    /// </summary>
    public class BaiTanLogAddCmdProcessor : ICmdProcessor
    {
        private static BaiTanLogAddCmdProcessor instance = new BaiTanLogAddCmdProcessor();

        private BaiTanLogAddCmdProcessor()  //此函数默认不会出发，必须在CmdReigsterTriggerManager中注册下
        {
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_ADD_BAITANLOG, this);
        }

        public static BaiTanLogAddCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams, int count)
        {
            string cmdData = null;
            int nID = (int)TCPGameServerCmds.CMD_DB_ADD_BAITANLOG;

            try
            {
                cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
            }
            catch (Exception) //解析错误
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID));
                client.sendCmd((int)TCPGameServerCmds.CMD_DB_ERR_RETURN, "0");
                return;
            }

            string[] fields = cmdData.Split(':');
            if (fields.Length != 12)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}",
                    (TCPGameServerCmds)nID, fields.Length, cmdData));

                client.sendCmd((int)TCPGameServerCmds.CMD_DB_ERR_RETURN, "0");
                return;
            }

            int roleID = Convert.ToInt32(fields[0]);
            int otherroleid = Convert.ToInt32(fields[1]);
            string otherrname = fields[2];
            int goodsid = Convert.ToInt32(fields[3]);
            int goodsnum = Convert.ToInt32(fields[4]);
            int forgelevel = Convert.ToInt32(fields[5]);
            int totalprice = Convert.ToInt32(fields[6]);
            int leftyuanbao = Convert.ToInt32(fields[7]);
            int YinLiang = Convert.ToInt32(fields[8]);
            int LeftYinLiang = Convert.ToInt32(fields[9]);
            int tax = Convert.ToInt32(fields[10]);
            int excellenceinfo = Convert.ToInt32(fields[11]);

            BaiTanLogItemData data = new BaiTanLogItemData();
            data.rid = roleID;
            data.OtherRoleID = otherroleid;
            data.OtherRName = otherrname;
            data.GoodsID = goodsid;
            data.GoodsNum = goodsnum;
            data.ForgeLevel = forgelevel;
            data.TotalPrice = totalprice;
            data.LeftYuanBao = leftyuanbao;
            data.YinLiang = YinLiang;
            data.LeftYinLiang = LeftYinLiang;
            data.BuyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            data.Tax = tax;
            data.Excellenceinfo = excellenceinfo;

            BaiTanManager.getInstance().onAddBaiTanLog(data);

            client.sendCmd<string>((int)TCPGameServerCmds.CMD_DB_ADD_BAITANLOG, string.Format("{0}", 0));
        }
    }
}
