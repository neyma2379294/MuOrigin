using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Logic;
using GameDBServer.Data;
using Server.Tools;
using Server.Protocol;
using GameDBServer.DB;
using Server.Data;
using ProtoBuf;
using GameDBServer.Logic.WanMoTa;
using GameDBServer.Core.Executor;

namespace GameDBServer.Server.CmdProcessor
{
    /// <summary>
    /// 修改万魔塔数据信息
    /// </summary>
    public class ModifyWanMoTaCmdProcessor : ICmdProcessor
    {
        private static ModifyWanMoTaCmdProcessor instance = new ModifyWanMoTaCmdProcessor();

        private ModifyWanMoTaCmdProcessor() { }

        public static ModifyWanMoTaCmdProcessor getInstance()
        {
            return instance;
        }

        public void processCmd(GameServerClient client, byte[] cmdParams,int count)
        {
            try
            {
                ModifyWanMotaData modifyData = DataHelper.BytesToObject<ModifyWanMotaData>(cmdParams, 0, count);
                if (null == modifyData)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数传输错误, CMD={0}, CmdData={2}",
                        TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, cmdParams));

                    client.sendCmd((int)TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, string.Format("{0}:{1}", 0, -1));
                    return;
                }

                string cmd = modifyData.strParams;

                string[] fields = cmd.Split(':');
                if (fields.Length != 6)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}",
                        TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, fields.Length, cmdParams));

                    client.sendCmd((int)TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, string.Format("{0}:{1}", 0, -1));
                    return ;
                }

                // 扫荡奖励单独传过来
                fields[4] = modifyData.strSweepReward;

                int roleID = Convert.ToInt32(fields[0]);
                WanMotaInfo dataWanMota = WanMoTaManager.getInstance().getWanMoTaData(roleID);
                if (null == dataWanMota)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("没有找到相应的万魔塔信息，CMD={0}, RoleID={1}",
                        TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, roleID));

                    client.sendCmd((int)TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, string.Format("{0}:{1}", 0, -1));
                    return;
                }

                // 将用户的请求发起写数据库的操作
                int ret = WanMoTaManager.getInstance().updateWanMoTaData(roleID, fields, 1);
                if (ret < 0)
                {
                    // 数据库更新万魔塔数据失败
                    LogManager.WriteLog(LogTypes.Error, string.Format("数据库更新万魔塔数据失败，CMD={0}, RoleID={1}",
                        TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, roleID));
                }
                else
                {
                    bool bPassLayerCountChange = false;
                    int nPassLayerCount = 0;

                    // 将用户的请求更新内存缓存
                    lock (dataWanMota)
                    {
                        dataWanMota.lFlushTime = DataHelper.ConvertToInt64(fields[1], dataWanMota.lFlushTime);
                        nPassLayerCount = DataHelper.ConvertToInt32(fields[2], dataWanMota.nPassLayerCount);
                        dataWanMota.nSweepLayer = DataHelper.ConvertToInt32(fields[3], dataWanMota.nSweepLayer);
                        dataWanMota.strSweepReward = DataHelper.ConvertToStr(fields[4], dataWanMota.strSweepReward);
                        dataWanMota.lSweepBeginTime = DataHelper.ConvertToInt64(fields[5], dataWanMota.lSweepBeginTime);

                        if (nPassLayerCount != dataWanMota.nPassLayerCount)
                        {
                            dataWanMota.nPassLayerCount = nPassLayerCount;
                            bPassLayerCountChange = true;
                        }
                    }

                    // 通关层数改变
                    if (bPassLayerCountChange)
                    {
                        // 刷新万魔塔排行榜
                        WanMoTaManager.getInstance().ModifyWanMoTaPaihangData(dataWanMota, false);
                    }
                }

                string strcmd = string.Format("{0}:{1}", roleID, ret);
                client.sendCmd((int)TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, strcmd);
                return;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "", false);

                string strcmd = string.Format("{0}:{1}", 0, -1);
            }
        }
    }
}
