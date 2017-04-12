using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using GameServer.Logic;
using Server.Protocol;
using Server.Tools;
using GameServer.Server.CmdProcesser;
using Server.TCP;

namespace GameServer.Server
{
    public class TCPCmdDispatcher
    {
        private static readonly TCPCmdDispatcher instance = new TCPCmdDispatcher();

        /// <summary>
        /// 指令参数个数映射<指令ID， 参数个数>
        /// </summary>
        private Dictionary<int, short> cmdParamNumMapping = new Dictionary<int, short>();
        private Dictionary<int, ICmdProcessor> cmdProcesserMapping = new Dictionary<int, ICmdProcessor>();

        private TCPCmdDispatcher(){}

        public static TCPCmdDispatcher getInstance()
        {
            return instance;
        }

        public void initialize()
        {
            
        }

        public void registerProcessor(int cmdId, short paramNum, ICmdProcessor processor) 
        {
            cmdParamNumMapping.Add(cmdId, paramNum);
            cmdProcesserMapping.Add(cmdId, processor);
        }

        public ICmdProcessor GetProcesser(int cmdID)
        {
            ICmdProcessor processor;
            if (cmdProcesserMapping.TryGetValue(cmdID, out processor))
            {
                return processor;
            }
            return null;
        }

        /// <summary>
        /// 透传到DBServer处理
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="nID"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public TCPProcessCmdResults transmission(TMSKSocket socket, int nID, byte[] data, int count)
        {
            string cmdData = null;

            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception) //解析错误
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket)));
                return TCPProcessCmdResults.RESULT_FAILED;
            }

//             GameClient client = GameManager.ClientMgr.FindClient(socket);
//             if (null == client)
//             {
//                 LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket)));
//                 return TCPProcessCmdResults.RESULT_FAILED;
//             }

            try
            {
                byte[] bytesData = Global.SendAndRecvData(TCPClientPool.getInstance(), TCPOutPacketPool.getInstance(), data, count, nID);
                if (null == bytesData)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("与DBServer通讯失败, CMD={0}", (TCPGameServerCmds)nID));
                    return TCPProcessCmdResults.RESULT_FAILED;
                }

                Int32 length = BitConverter.ToInt32(bytesData, 0);
                Int16 cmd = BitConverter.ToInt16(bytesData, 4);

                TCPOutPacket tcpOutPacket = TCPOutPacketPool.getInstance().Pop();
                tcpOutPacket.PacketCmdID = (Int16)cmd;
                tcpOutPacket.FinalWriteData(bytesData, 6, length - 2);

                //client.sendCmd(tcpOutPacket);
                TCPManager.getInstance().MySocketListener.SendData(socket, tcpOutPacket);
                return TCPProcessCmdResults.RESULT_OK;
            }
            catch (Exception ex)
            {
                // 格式化异常错误信息
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false);
            }


            return TCPProcessCmdResults.RESULT_FAILED;
        }

        /// <summary>
        /// 本地处理
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="nID"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public TCPProcessCmdResults dispathProcessor(TMSKSocket socket, int nID, byte[] data, int count) 
        {
            string cmdData = null;

            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception) //解析错误
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket)));
                return TCPProcessCmdResults.RESULT_FAILED;
            }

            try
            {
                //获取指令参数数量
                short cmdParamNum = -1;
                if (!cmdParamNumMapping.TryGetValue(nID, out cmdParamNum))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("未注册指令, CMD={0}, Client={1}",
                        (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket)));
                    return TCPProcessCmdResults.RESULT_FAILED;
                };
                //解析用户名称和用户密码
                string[] cmdParams = cmdData.Split(':');
                if (cmdParams.Length != cmdParamNum)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}",
                        (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket), cmdParams.Length));
                    return TCPProcessCmdResults.RESULT_FAILED;
                }

                //根据socket获取GameClient
                GameClient client = GameManager.ClientMgr.FindClient(socket);
                if (null == client)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket)));
                    return TCPProcessCmdResults.RESULT_FAILED;
                }

                //获取相对应的指令处理器
                ICmdProcessor cmdProcessor = null;
                if (!cmdProcesserMapping.TryGetValue(nID, out cmdProcessor))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("未注册指令, CMD={0}, Client={1}",
                        (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket)));
                    return TCPProcessCmdResults.RESULT_FAILED;
                }

                if (!cmdProcessor.processCmd(client, cmdParams))
                {
                    return TCPProcessCmdResults.RESULT_FAILED;
                }

                return TCPProcessCmdResults.RESULT_OK;

            }
            catch (Exception ex)
            {
                // 格式化异常错误信息
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false);
            }

            return TCPProcessCmdResults.RESULT_FAILED;
        }

    }
}
