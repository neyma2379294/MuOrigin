using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Logic;
using GameServer.Logic.BangHui.ZhanMengShiJian;
using GameServer.Logic.JingJiChang;
using Server.Tools;
using GameServer.Logic.Copy;

namespace GameServer.Server.CmdProcesser
{
    /// <summary>
    /// 副本组队指令处理
    /// </summary>
    public class CopyTeamCmdProcessor :ICmdProcessor
    {
        private TCPGameServerCmds Cmd;

        private CopyTeamCmdProcessor(TCPGameServerCmds cmd)
        {
            Cmd = cmd;
        }

        public static CopyTeamCmdProcessor getInstance(TCPGameServerCmds cmd)
        {
            return new CopyTeamCmdProcessor(cmd);
        }

        public bool processCmd(GameClient client, string[] cmdParams)
        {
            if (Cmd == TCPGameServerCmds.CMD_SPR_COPYTEAM)
            {
                int teamType = Convert.ToInt32(cmdParams[1]);
                int extTag1 = Convert.ToInt32(cmdParams[2]);
                int extTag2 = Convert.ToInt32(cmdParams[3]);
                int autoEnter = Convert.ToInt32(cmdParams[4]);

                if (teamType == (int)TeamCmds.Create) //创建队伍
                {
                    CopyTeamManager.getInstance().CreateCopyTeam(client, extTag1, extTag2, autoEnter);
                }
                else if (teamType == (int)TeamCmds.Apply) //申请组队
                {
                    CopyTeamManager.getInstance().ApplyCopyTeam(client, extTag1);
                }
                else if (teamType == (int)TeamCmds.Remove) //踢出队伍
                {
                    CopyTeamManager.getInstance().RemoveFromCopyTeam(client, extTag1);
                }
                else if (teamType == (int)TeamCmds.Quit) //离开组队
                {
                    CopyTeamManager.getInstance().QuitFromTeam(client);
                }
                else if (teamType == (int)TeamCmds.Ready) //准备状态变化
                {
                    CopyTeamManager.getInstance().Ready(client, extTag1);
                }
                else if (teamType == (int)TeamCmds.QuickJoinTeam) //快速加入
                {
                    CopyTeamManager.getInstance().QuickJoinTeam(client, extTag1);
                } 
            }
            else if (Cmd == TCPGameServerCmds.CMD_SPR_REGEVENTNOTIFY)
            {
                int sceneIndex = Convert.ToInt32(cmdParams[1]);
                int ready = Convert.ToInt32(cmdParams[2]);

                if (ready > 0)
                {
                    CopyTeamManager.getInstance().RegisterCopyTeamListNotify(client, sceneIndex);
                }
                else
                {
                    CopyTeamManager.getInstance().UnRegisterCopyTeamListNotify(client);
                }
            }
            else if (Cmd == TCPGameServerCmds.CMD_SPR_LISTCOPYTEAMS)
            {
                int startIndex = Convert.ToInt32(cmdParams[1]);
                int sceneIndex = Convert.ToInt32(cmdParams[2]);

                //列举队伍并返回列表
                CopyTeamManager.getInstance().ListAllTeams(client, startIndex, sceneIndex);
            }

            return true;
        }
    }
}
