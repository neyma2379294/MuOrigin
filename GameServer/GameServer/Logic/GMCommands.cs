using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Net.Sockets;
using GameServer.Server;
using System.Windows;
using Server.Tools;
using Server.Data;
using Server.TCP;
using Server.Protocol;
using GameServer.Tools;
using GameServer.Core.Executor;
using System.Threading;

namespace GameServer.Logic
{
    public enum GMPrioritys
    {
        Guest = 1,
        ChatCensor = 2, //聊天监控权限
    }

    /// <summary>
    /// GM命令处理
    /// </summary>
    public class GMCommands
    {
        #region GM配置文件数据

        /// <summary>
        /// 超级GM的名称列表
        /// </summary>
        private string[] SuperGMUserNames = null;

        /// <summary>
        /// GM的名称列表
        /// </summary>
        private string[] GMUserNames = null;

        /// <summary>
        /// GM的操作IP列表
        /// </summary>
        private string[] GMIPs = null;

        /// <summary>
        /// 需要分配权限的其他玩家名称
        /// </summary>
        private Dictionary<string, int> OtherUserNamesDict = new Dictionary<string, int>();

        /// <summary>
        /// 需要分配权限的命令名称
        /// </summary>
        private Dictionary<int, string[]> GMCmdsDict = new Dictionary<int, string[]>();

        #endregion GM配置文件数据

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="userNames"></param>
        /// <param name="gmIPs"></param>
        public void InitGMCommands(XElement xml)
        {
            if (null == xml)
            {
                try
                {
                    xml = XElement.Load(@"GMConfig.xml");
                }
                catch (Exception)
                {
                    return;
                }
            }

            string superUserNames = Global.GetSafeAttributeStr(xml, "GameManager", "SuperUserNames");
            string userNames = Global.GetSafeAttributeStr(xml, "GameManager", "UserNames");
            string gmIPs = Global.GetSafeAttributeStr(xml, "GameManager", "IPs");

            if (!string.IsNullOrEmpty(superUserNames))
            {
                SuperGMUserNames = superUserNames.Trim().Split(',');
            }

            if (!string.IsNullOrEmpty(userNames))
            {
                GMUserNames = userNames.Trim().Split(',');
            }

            if (!string.IsNullOrEmpty(gmIPs))
            {
                GMIPs = gmIPs.Trim().Split(',');
            }

            Dictionary<string, int> dict1 = new Dictionary<string, int>();
            XElement otherNames = Global.GetXElement(xml, "OtherNames");
            if (null != otherNames)
            {
                IEnumerable<XElement> names = otherNames.Elements();
                foreach (var key in names)
                {
                    dict1[Global.GetSafeAttributeStr(key, "Name")] = (int)Global.GetSafeAttributeLong(key, "Priority");
                }
            }

            OtherUserNamesDict = dict1;

            Dictionary<int, string[]> dict2 = new Dictionary<int, string[]>();
            XElement prioritys = Global.GetXElement(xml, "Prioritys");
            if (null != prioritys)
            {
                IEnumerable<XElement> items = prioritys.Elements();
                foreach (var key in items)
                {
                    dict2[(int)Global.GetSafeAttributeLong(key, "ID")] = Global.GetSafeAttributeStr(key, "Cmds").Split(',');
                }
            }

            GMCmdsDict = dict2;
        }

        /// <summary>
        /// 重新加载GM命令的配置参数
        /// </summary>
        public int ReloadGMCommands()
        {
            XElement xml = null;

            try
            {
                xml = XElement.Load(@"GMConfig.xml");
            }
            catch (Exception)
            {
                return -1;
            }

            //初始化GM管理命令项
            GameManager.systemGMCommands.InitGMCommands(xml);
            return 0;
        }

        #endregion 初始化

        #region 合法性判断

        /// <summary>
        /// 是否是超级GM用户
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public bool IsSuperGMUser(TMSKSocket socket)
        {
            string userName = GameManager.OnlineUserSession.FindUserName(socket);
            return IsSuperGMUser(userName);
        }

        /// <summary>
        /// 是否是超级GM用户
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public bool IsSuperGMUser(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }

            string[] userNames = SuperGMUserNames;
            if (null == userNames || userNames.Length <= 0) return false;

            for (int i = 0; i < userNames.Length; i++)
            {
                if (userNames[i] == userName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否是GM用户
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public bool IsGMUser(TMSKSocket socket)
        {
            string userName = GameManager.OnlineUserSession.FindUserName(socket);
            return IsGMUser(userName);
        }

        /// <summary>
        /// 是否是GM用户
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public bool IsGMUser(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return false;
            }

            string[] userNames = GMUserNames;
            if (null == userNames || userNames.Length <= 0) return false;

            for (int i = 0; i < userNames.Length; i++)
            {
                if (userNames[i].IndexOf("*") >= 0)
                {
                    return true;
                }

                if (userNames[i] == userName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否是授权用户
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public int IsPriorityUser(TMSKSocket socket)
        {
            string userName = GameManager.OnlineUserSession.FindUserName(socket);
            return IsPriorityUser(userName);
        }

        /// <summary>
        /// 是否是授权用户
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public int IsPriorityUser(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return -1;
            }

            Dictionary<string, int> dict = OtherUserNamesDict;
            if (null == dict || dict.Count <= 0) return -1;

            int priority = -1;
            if (!dict.TryGetValue(userName, out priority))
            {
                priority = -1;
            }

            return priority;
        }

        private bool CanExecCmd(int priority, string cmd)
        {
            if (priority < 0)
            {
                return true;
            }

            if (string.IsNullOrEmpty(cmd))
            {
                return false;
            }

            Dictionary<int, string[]> dict = GMCmdsDict;
            if (null == dict || dict.Count <= 0) return false;

            string[] cmds = null;
            if (!dict.TryGetValue(priority, out cmds))
            {
                return false;
            }

            if (null == cmds || cmds.Length <= 0)
            {
                return false;
            }

            for (int i = 0; i < cmds.Length; i++)
            {
                if (cmds[i] == cmd)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否是合法的IP
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public bool IsValidIP(TMSKSocket socket)
        {
            string ip = Global.GetSocketRemoteEndPoint(socket);
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }

            string[] ipFields = ip.Split(':');
            if (ipFields.Length <= 0) return false;

            string[] IPs = GMIPs;
            if (null == IPs || IPs.Length <= 0) return false;

            for (int i = 0; i < IPs.Length; i++)
            {
                if (IPs[i].IndexOf("*") >= 0)
                {
                    return true;
                }

                if (IPs[i] == ipFields[0])
                {
                    return true;
                }
            }

            return false;
        }

        #endregion 合法性判断

        #region 聊天发送时的GM命令处理

        private List<GameClient> GMClientList = new List<GameClient>();

        /// <summary>
        /// GM角色登录
        /// </summary>
        /// <param name="client"></param>
        public void OnClientLogin(GameClient client)
        {
            if (IsPriorityUser(client.strUserName) != (int)GMPrioritys.ChatCensor)
            {
                return;
            }
            client.ClientData.HideGM = 1;
            client.ClientData.MapCode = (int)FRESHPLAYERSCENEINFO.FRESHPLAYERMAPCODEID;
            client.ClientData.PosX = 1000;
            client.ClientData.PosY = 1000;
            lock (GMClientList)
            {
                if (!GMClientList.Contains(client))
                {
                    GMClientList.Add(client);
                }
            }
        }

        /// <summary>
        /// GM角色离线
        /// </summary>
        /// <param name="client"></param>
        public void OnClientLogout(GameClient client)
        {
            if (IsPriorityUser(client.strUserName) != (int)GMPrioritys.ChatCensor)
            {
                return;
            }
            lock (GMClientList)
            {
                GMClientList.Remove(client);
            }
        }

        /// <summary>
        /// 向聊天监控GM角色广播聊天信息
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="client"></param>
        /// <param name="cmdText"></param>
        public void BroadcastChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
        {
            List<GameClient> objsList;
            lock (GMClientList)
            {
                int count = GMClientList.Count;
                if (count <= 0)
                {
                    return;
                }
                objsList = GMClientList.GetRange(0, count);
            }
            TCPOutPacket tcpOutPacket = null;
            try
            {
                for (int i = 0; i < objsList.Count; i++)
                {
                    //是否跳过自己
                    if (client == objsList[i])
                    {
                        continue;
                    }

                    if ((objsList[i] as GameClient).LogoutState) //如果已经退出了
                    {
                        continue;
                    }

                    if (null == tcpOutPacket) tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdText, (int)TCPGameServerCmds.CMD_SPR_CHAT);
                    if (!sl.SendData((objsList[i] as GameClient).ClientSocket, tcpOutPacket, false))
                    {
                    }
                }
            }
            finally
            {
                Global.PushBackTcpOutPacket(tcpOutPacket);
            }
        }

        /// <summary>
        /// 处理聊天文本判断是否是GM命令，并且是否是合法的发送者和IP
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool ProcessChatMessage(TMSKSocket socket, GameClient client, string text, bool transmit)
        {
            if (text.Length <= 0) return false;
            if (text[0] != '-') return false;

            //是否是超级GM账户
            bool isSuperGMUser = false;

            //授权的级别
            int priority = -1;

            //转发的不再检查
            if (!transmit)
            {
                //判断是否是超级GM
                if (!IsSuperGMUser(socket))
                {
                    if (!IsGMUser(socket)) //不是GM
                    {
                        //是否是授权用户
                        priority = IsPriorityUser(socket);
                        if (priority < 0)
                        {
                            return true;
                        }
                    }

                    if (!IsValidIP(socket)) //不是合法的IP
                    {
                        return true;
                    }
                }
                else //超级GM不检查IP限制
                {
                    ;//
                    //是否是超级GM账户
                    isSuperGMUser = true;
                }
            }

            string[] fields = text.Trim().Split(' ');
            if (fields.Length <= 0) //命令参数个数错误, 不处理
            {
                return true;
            }

            //如果是授权的用户
            if (!CanExecCmd(priority, fields[0]))
            {
                return true;
            }

            return ProcessGMCommands(client, text, fields, transmit, isSuperGMUser);
        }

        /// <summary>
        /// 安全转换字符串到整型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private int SafeConvertToInt32(string str)
        {
            int ret = -1;

            try
            {
                ret = Convert.ToInt32(str);
            }
            catch (Exception ex)
            {
                ret = -1;
                LogManager.WriteException(ex.ToString());
            }

            return ret;
        }

        /// <summary>
        /// 处理GM命令
        /// </summary>
        /// <param name="cmdFields"></param>
        /// <returns></returns>
        private bool ProcessGMCommands(GameClient client, string msgText, string[] cmdFields, bool transmit, bool isSuperGMUser)
        {
            //发送请求的命令回去
            if (!transmit)
            {
                GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                    client, msgText);
            }

            string strinfo = "";
            if ("-info" == cmdFields[0]) //显示服务器信息
            {
                if (!transmit) //非转发的命令
                {
                    strinfo = string.Format("当前线路{0}在线人数{1}", GameManager.ServerLineID, GameManager.ClientMgr.GetClientCount());

                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                        client, strinfo);
                }
            }
            else if ("-info2" == cmdFields[0]) //显示服务器信息2
            {
                if (!transmit) //非转发的命令
                {
                    //从DBServer获取副本顺序ID
                    string[] dbFields = Global.ExecuteDBCmd((int)TCPGameServerCmds.CMD_DB_GETTOTALONLINENUM, string.Format("{0}", client.ClientData.RoleID));
                    if (null == dbFields || dbFields.Length < 1)
                    {
                        strinfo = string.Format("获取所有线路在线人数时连接数据库失败");
                        return true;
                    }
                    else
                    {
                        int totalOnlineNum = Global.SafeConvertToInt32(dbFields[0]);
                        strinfo = string.Format("获取所有线路在线人数是{0}", totalOnlineNum);
                    }

                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                        client, strinfo); ;
                }
            }
            else if ("-version" == cmdFields[0]) //显示服务器信息2
            {
                if (!transmit) //非转发的命令
                {
                    string gameServerVer = GameManager.GameConfigMgr.GetGameConifgItem("gameserver_version");
                    string gameDBServerVer = GameManager.GameConfigMgr.GetGameConifgItem("gamedb_version");
                    strinfo = string.Format("gameserver_version：{0},gamedb_version：{1},client：mainver-{2},resver-{3},codereversion-{4}",
                        gameServerVer, gameDBServerVer, client.MainExeVer, client.ResVer, client.CodeRevision);

                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                        client, strinfo); ;
                }
            }
            else if ("-patch" == cmdFields[0]) //运行修补程序
            {
                if (false) //出于安全考虑,暂不开放此功能
                {
                    string arg = "";
                    for (int i = 1; i < cmdFields.Length; i++ )
                    {
                        arg += cmdFields[i];
                    }
                    Program.RunPatch(arg, false);
                }
            }
            else if ("-serverinfo" == cmdFields[0]) //显示服务器信息2
            {
                if (!transmit) //非转发的命令
                {
                    string gameDBServerVer = GameManager.GameConfigMgr.GetGameConifgItem("gamedb_version");
                    strinfo = string.Format("{0}_{1}", Global.GetLocalAddressIPs(), TCPManager.ServerPort);

                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                        client, strinfo); ;
                }
            }
            else if ("-hide" == cmdFields[0]) //自己隐身
            {
                if (!transmit) //非转发的命令
                {
                    if (client.ClientData.HideGM < 1)
                    {
                        client.ClientData.HideGM = 1;

                        List<Object> objsList = Global.GetAll9Clients(client);

                        //通知自己所在的地图，其他的所有用户，自己离开了
                        GameManager.ClientMgr.NotifyOthersLeave(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, objsList);

                        /// 退出时删除角色放出的宠物
                        GameManager.ClientMgr.RemoveRolePet(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, objsList, false);

                        strinfo = string.Format("进入隐身模式");
                    }
                    else
                    {
                        strinfo = string.Format("已经是隐身模式");
                    }

                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                        client, strinfo);
                }
            }
            else if ("-show" == cmdFields[0]) //自己现身
            {
                if (!transmit) //非转发的命令
                {
                    if (client.ClientData.HideGM > 0)
                    {
                        client.ClientData.HideGM = 0;

                        List<Object> objsList = Global.GetAll9Clients(client);

                        //再通知其他人自己上线了
                        GameManager.ClientMgr.NotifyOthersIamComing(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, objsList, (int)TCPGameServerCmds.CMD_OTHER_ROLE);

                        strinfo = string.Format("退出隐身模式");
                    }
                    else
                    {
                        strinfo = string.Format("已经退出隐身模式");
                    }

                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                        client, strinfo);
                }
            }
            else if ("-moveto" == cmdFields[0]) //传送到地点
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 4)
                    {
                        strinfo = string.Format("请输入： -moveto 地图编号 X坐标 Y坐标");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        int mapCode = SafeConvertToInt32(cmdFields[1]);
                        int toX = SafeConvertToInt32(cmdFields[2]);
                        int toY = SafeConvertToInt32(cmdFields[3]);

                        GameMap gameMap = null;
                        if (GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap) && 
                            MapTypes.Normal == Global.GetMapType(mapCode)) //确认地图编号是否有效, 并且是常规地图
                        {
                            Point newPos = Global.GetAGridPointIn4Direction(ObjectTypes.OT_CLIENT, new Point(toX / gameMap.MapGridWidth, toY / gameMap.MapGridHeight), mapCode);
                            newPos = new Point(newPos.X * gameMap.MapGridWidth, newPos.Y * gameMap.MapGridHeight);

                            if (!Global.InObs(ObjectTypes.OT_CLIENT, mapCode, (int)newPos.X, (int)newPos.Y))
                            {
                                if (mapCode != client.ClientData.MapCode)
                                {
                                    GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                        client, mapCode, (int)newPos.X, (int)newPos.Y, -1);
                                }
                                else
                                {
                                    GameManager.LuaMgr.GotoMap(client, mapCode, (int)newPos.X, (int)newPos.Y, -1);
                                }

                                strinfo = string.Format("执行移动到目标点({0},{1})的操作成功", newPos.X, newPos.Y);
                            }
                            else
                            {
                                strinfo = string.Format("目标点({0},{1})可能是障碍物", toX, toY);
                            }
                        }
                        else
                        {
                            strinfo = string.Format("请输入正确的地图编号, 副本地图无法直接进入");
                        }

                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-line" == cmdFields[0]) //-line 查看玩家所在的线路, 请输入： -line 角色名称
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -line 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];

                        //从DBServer获取角色的所在的线路
                        string[] dbFields = Global.ExecuteDBCmd((int)TCPGameServerCmds.CMD_SPR_QUERYIDBYNAME, string.Format("{0}:{1}:0", client.ClientData.RoleID, otherRoleName));
                        if (null == dbFields || dbFields.Length < 5)
                        {
                            strinfo = string.Format("获取{0}的线路状态时连接数据库失败", otherRoleName);
                        }
                        else
                        {
                            int onelineState = Global.SafeConvertToInt32(dbFields[4]);
                            strinfo = string.Format("{0}所在的线路是{1}", otherRoleName, onelineState);
                        }                

                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-viewum" == cmdFields[0]) //-viewum 查看玩家的元宝数和真实充值钱数, 请输入： -viewum 角色名称
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -viewum 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];

                        //从DBServer获取角色的所在的线路
                        string[] dbFields = Global.ExecuteDBCmd((int)TCPGameServerCmds.CMD_SPR_QUERYUMBYNAME, string.Format("{0}:{1}", client.ClientData.RoleID, otherRoleName));
                        if (null == dbFields || dbFields.Length < 4)
                        {
                            strinfo = string.Format("获取{0}的元宝数时连接数据库失败", otherRoleName);
                        }
                        else
                        {
                            string userID = dbFields[1];
                            int userMoney = Global.SafeConvertToInt32(dbFields[2]);
                            int realMoney = Global.SafeConvertToInt32(dbFields[3]);
                            strinfo = string.Format("{0}的平台ID是{1}，元宝是{2}，ＲＭＢ{3}", otherRoleName, userID, userMoney, realMoney);
                        }                

                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-follow" == cmdFields[0]) //传送到人物
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -follow 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        int mapCode = otherClient.ClientData.MapCode;
                        int toX = otherClient.ClientData.PosX;
                        int toY = otherClient.ClientData.PosY;

                        GameMap gameMap = null;
                        if (GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap) &&
                            MapTypes.Normal == Global.GetMapType(mapCode)) //确认地图编号是否有效, 并且是常规地图
                        {
                            Point pt = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, toX, toY, 60);
                            if (!Global.InObs(ObjectTypes.OT_CLIENT, mapCode, (int)pt.X, (int)pt.Y))
                            {
                                GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                    client, mapCode, (int)pt.X, (int)pt.Y, -1);

                                strinfo = string.Format("执行移动到{0}身边的操作成功", otherRoleName);
                            }
                            else
                            {
                                strinfo = string.Format("执行移动到{0}身边的操作失败", otherRoleName);
                            }
                        }
                        else
                        {
                            strinfo = string.Format("{0}目前在副本地图, 无法移动到其旁边", otherRoleName);
                        }

                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-substrong" == cmdFields[0]) //修改装备耐久值
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入：-substrong 角色名称 减少值");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int val = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        otherClient.UsingEquipMgr.GMAddEquipStrong(otherClient, val);

                        strinfo = string.Format("{0}佩戴的装备耐久减少({1})", otherRoleName, val);

                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-liequyuansu" == cmdFields[0])
            {
                if (cmdFields.Length == 3)
                {
                    bool val = SafeConvertToInt32(cmdFields[1]) > 0;
                    int count = SafeConvertToInt32(cmdFields[2]);

                    int GoodsID = 0;
                    for (int i = 0 ; i < count; i++)
                        ElementhrtsManager.GetSomeElementHrts(client, val, out GoodsID);
                }
            }
            else if ("-qingkongyuansu" == cmdFields[0])
            {
                List<GoodsData> goodslist = client.ClientData.ElementhrtsList;
                for (int i = goodslist.Count - 1; i >= 0; i--)
                {
                    GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodslist[i], 1, false);
                }
            }
            else if ("-setviplev" == cmdFields[0]) //修改VIP等级
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入：-setviplev 角色名称 VIP等级");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int val = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        Global.GMSetVipLevel(otherClient, val);

                        strinfo = string.Format("设置{0}的VIP等级为({1})", otherRoleName, val);

                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-kick" == cmdFields[0]) //踢掉玩家
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -kick 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];

                        strinfo = string.Format("将{0}踢出了当前线路", otherRoleName);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            string gmCmdData = string.Format("-kick {0}", cmdFields[1]);

                            //转发GM消息到DBServer
                            GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_SPR_CHAT,
                                string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", roleID, "", 0, "", 0, gmCmdData, 0, 0, GameManager.ServerLineID),
                                null);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            string gmCmdData = string.Format("-kick {0}", cmdFields[1]);

                            //转发GM消息到DBServer
                            GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_SPR_CHAT,
                                string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", roleID, "", 0, "", 0, gmCmdData, 0, 0, GameManager.ServerLineID),
                                null);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将{0}踢出了服务器...", otherRoleName));

                        //关闭角色连接
                        Global.ForceCloseClient(otherClient);
                    }
                }
                else //处理转发的踢人的GM命令
                {
                    if (cmdFields.Length >= 2)
                    {
                        string otherRoleName = cmdFields[1];

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将{0}踢出了服务器...", otherRoleName));

                        //关闭角色连接
                        Global.ForceCloseClient(otherClient);
                    }
                }
            }
            else if ("-kicku" == cmdFields[0]) //踢掉用户账号下的所有角色
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -kicku 账户名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string userName = cmdFields[1];

                        strinfo = string.Format("将账户{0}的角色踢出服务器", userName);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);

                        //根据ID查找敌人
                        TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserName(userName);
                        if (null == clientSocket)
                        {
                            string gmCmdData = string.Format("-kicku {0}", cmdFields[1]);

                            //转发GM消息到DBServer
                            GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_SPR_CHAT,
                                string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", 0, "", 0, "", 0, gmCmdData, 0, 0, GameManager.ServerLineID),
                                null);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(clientSocket);
                        if (null == otherClient)
                        {
                            LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将账户{0}的连接踢出了服务器...", userName));
                            Global.ForceCloseSocket(clientSocket);
                        }
                        else
                        {
                            LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将账户{0}的角色踢出了服务器...", userName));

                            //关闭角色连接
                            Global.ForceCloseClient(otherClient);
                        }
                    }
                }
                else //处理转发的踢人的GM命令
                {
                    if (cmdFields.Length >= 2)
                    {
                        string userName = cmdFields[1];

                        //根据ID查找敌人
                        TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserName(userName);
                        if (null == clientSocket)
                        {
                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将账户{0}的角色踢出了服务器...", userName));

                        GameClient otherClient = GameManager.ClientMgr.FindClient(clientSocket);
                        if (null == otherClient)
                        {
                            Global.ForceCloseSocket(clientSocket);
                        }
                        else
                        {
                            //关闭角色连接
                            Global.ForceCloseClient(otherClient);
                        }
                    }
                }
            }
            else if ("-ban" == cmdFields[0]) //禁止登陆
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -ban 角色名称 禁止的分钟(例如1表示禁止1分钟内登陆)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int minutes = Global.SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 != roleID)
                        {
                            GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                            if (null != otherClient)
                            {
                                LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将{0}踢出了服务器，并禁止从任何线路再登陆", otherRoleName));

                                //关闭角色连接
                                Global.ForceCloseClient(otherClient);
                            }
                            else
                            {
                                string gmCmdData = string.Format("-kick {0}", cmdFields[1]);

                                //转发GM消息到DBServer
                                GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_SPR_CHAT,
                                    string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", roleID, "", 0, "", 0, gmCmdData, 0, 0, GameManager.ServerLineID),
                                    null);
                            }
                        }

                        Global.BanRoleNameToDBServer(otherRoleName, minutes);

                        strinfo = string.Format("将{0}踢出了当前线路, 并禁止从任何线路再登陆", otherRoleName);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-unban" == cmdFields[0]) //解除禁止登陆
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -unban 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];

                        Global.BanRoleNameToDBServer(otherRoleName, 0);

                        strinfo = string.Format("解除对于{0}的禁止登陆限制", otherRoleName);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-banchat" == cmdFields[0]) //禁止聊天发言
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -banchat 角色名称 几个小时");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int banHours = SafeConvertToInt32(cmdFields[2]);
                        if (banHours > 0)
                        {
                            Global.BanRoleChatToDBServer(otherRoleName, banHours);

                            /// 聊天发言限制管理
                            BanChatManager.AddBanRoleName(otherRoleName, banHours);
                        }

                        strinfo = string.Format("将{0}列入黑名单，禁止聊天发言", otherRoleName);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-unbanchat" == cmdFields[0]) //解除禁止聊天发言
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -unbanchat 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];

                        Global.BanRoleChatToDBServer(otherRoleName, 0);

                        /// 聊天发言限制管理
                        BanChatManager.AddBanRoleName(otherRoleName, 0);

                        strinfo = string.Format("解除对于{0}的禁止聊天发言限制", otherRoleName);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-recover" == cmdFields[0]) //恢复指定角色的HP和MP
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -recover 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}恢复血和蓝...", otherRoleName));

                        //如果已经死亡，则不再恢复
                        if (otherClient.ClientData.CurrentLifeV > 0)
                        {
                            bool doRelife = false;

                            //判断如果血量少于最大血量
                            if (otherClient.ClientData.CurrentLifeV < otherClient.ClientData.LifeV)
                            {
                                doRelife = true;
                                int addLifeV = otherClient.ClientData.LifeV - otherClient.ClientData.CurrentLifeV;
                                otherClient.ClientData.CurrentLifeV = otherClient.ClientData.LifeV;

                                /*GameManager.SystemServerEvents.AddEvent(string.Format("角色加血, roleID={0}({1}), Add={2}, Life={3}",
                                    otherClient.ClientData.RoleID, otherClient.ClientData.RoleName,
                                    addLifeV, otherClient.ClientData.CurrentLifeV), EventLevels.Debug);*/
                            }

                            //判断如果魔量少于最大魔量
                            if (otherClient.ClientData.CurrentMagicV < otherClient.ClientData.MagicV)
                            {
                                doRelife = true;

                                int addMagicV = otherClient.ClientData.MagicV - otherClient.ClientData.CurrentMagicV;
                                otherClient.ClientData.CurrentMagicV = otherClient.ClientData.MagicV;

                                /*GameManager.SystemServerEvents.AddEvent(string.Format("角色加魔, roleID={0}({1}), Add={2}, Magic={3}",
                                    otherClient.ClientData.RoleID, otherClient.ClientData.RoleName,
                                    addMagicV, otherClient.ClientData.CurrentMagicV), EventLevels.Debug);*/
                            }

                            if (doRelife)
                            {
                                //通知客户端怪已经加血加魔      
                                List<Object> listObjs = Global.GetAll9Clients(otherClient);
                                GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                    otherClient, otherClient.ClientData.MapCode, otherClient.ClientData.CopyMapID, otherClient.ClientData.RoleID,
                                    (int)otherClient.ClientData.PosX, (int)otherClient.ClientData.PosY, (int)otherClient.ClientData.RoleDirection,
                                    otherClient.ClientData.CurrentLifeV, otherClient.ClientData.CurrentMagicV, (int)TCPGameServerCmds.CMD_SPR_RELIFE, listObjs);
                            }
                        }

                        strinfo = string.Format("为{0}恢复了HP和MP", otherRoleName);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-testmode" == cmdFields[0]) //设置压测模式
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -testmode 压测选项(1-15,1 锁生命值,2 不限测试号,4 全体PK) 地图模式(可选,0-3,0 指定地图, 1 新手场景, 2 多主线地图, 3 剧情副本地图) 指定地图编号(可选,默认1)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        GameManager.TestGamePerformanceMode = SafeConvertToInt32(cmdFields[1]) > 0;

                        //按要求设置PK模式
                        GameManager.TestGamePerformanceForAllUser = (SafeConvertToInt32(cmdFields[1]) & 2) > 0;
                        GameManager.TestGamePerformanceAllPK = (SafeConvertToInt32(cmdFields[1]) & 4) > 0;
                        GameManager.TestGamePerformanceLockLifeV = (SafeConvertToInt32(cmdFields[1]) & 8) > 0;
                        int pkmode = GameManager.TestGamePerformanceAllPK ? (int)GPKModes.Whole : (int)GPKModes.Normal;

                        List<GameClient> clientList = GameManager.ClientMgr.GetAllClients();
                        for (int i = 0; i < clientList.Count; i++)
                        {
                            GameClient c = clientList[i];
                            if (null != c && (GameManager.TestGamePerformanceForAllUser || c.strUserID == null || c.strUserID.StartsWith("mu")))
                            {
                                clientList[i].ClientData.PKMode = pkmode;
                            }
                        }
                        if (cmdFields.Length > 2)
                        {
                            GameManager.TestGamePerformanceMapMode = SafeConvertToInt32(cmdFields[2]);
                            if (cmdFields.Length > 3)
                            {
                                GameManager.TestGamePerformanceMapCode = SafeConvertToInt32(cmdFields[3]);
                            }
                        }

                        do
                        {
                            Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, 1, 10000, 10000, 8000);
                            if (GameManager.TestBirthPointList1.FindIndex((x) => { return (x.X / 100) == (newPos.X / 100) && (x.Y / 100 == newPos.Y / 100); }) >= 0)
                            {
                                continue;
                            }
                            GameManager.TestBirthPointList1.Add(newPos);
                        } while (GameManager.TestBirthPointList1.Count < 1000);
                        do
                        {
                            Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, 2, 5000, 5000, 4000);
                            if (GameManager.TestBirthPointList2.FindIndex((x) => { return (x.X / 100) == (newPos.X / 100) && (x.Y / 100 == newPos.Y / 100); }) >= 0)
                            {
                                continue;
                            }
                            GameManager.TestBirthPointList2.Add(newPos);
                        } while (GameManager.TestBirthPointList2.Count < 1000);

                        strinfo = string.Format("设置了压测模式 {0} {1}", GameManager.TestGamePerformanceMode, GameManager.TestGamePerformanceMapMode);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-useworkpool" == cmdFields[0]) //设置TCP模式
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -useworkpool 是否使用线程池(0/1)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        //TCPManager.UseWorkerPool = SafeConvertToInt32(cmdFields[1]) > 0;

                        strinfo = string.Format("设置了命令处理模式是否使用线程池 {0}", TCPManager.UseWorkerPool);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-manyattack" == cmdFields[0]) //设置TCP模式
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -manyattack 是否使用多段攻击(0/1)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        //GameManager.FlagManyAttack = SafeConvertToInt32(cmdFields[1]) > 0; //不测试时这个开关时,变量存储改为const,不再消耗性能

                        strinfo = string.Format("多段攻击开启状态： {0}", GameManager.FlagManyAttack);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-setmaxthread" == cmdFields[0]) //设置TCP模式
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -setmaxthread 后台线程数 完成端口线程数");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        int threadNum1 = Global.SafeConvertToInt32(cmdFields[1]);
                        int threadNum2 = Global.SafeConvertToInt32(cmdFields[2]);
                        ThreadPool.SetMaxThreads(threadNum1, threadNum2);

                        strinfo = string.Format("线程池最大线程数设置为：后台线程{0},完成端口线程{1}", threadNum1, threadNum2);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-setlev" == cmdFields[0]) //设置角色等级
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -setlev 角色名称 级别 转生次数");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int level = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        if (level < 0 || level > 400)
                        {
                            strinfo = string.Format("设置的级别{0}超出了最大限制400", level);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置级别{1}", otherRoleName, level));

                        otherClient.ClientData.Level = level;
                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_UPDATE_EXPLEVEL,
                            string.Format("{0}:{1}:{2}", otherClient.ClientData.RoleID, otherClient.ClientData.Level, otherClient.ClientData.Experience),
                            null);

                        otherClient.ClientData.Experience = 0;
                        GameManager.ClientMgr.NotifySelfExperience(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0);

                        if (cmdFields.Length >= 4)
                        {
                            int nNewChangeCount = SafeConvertToInt32(cmdFields[3]);

                            if (nNewChangeCount < 0 || nNewChangeCount > 100)
                            {
                                strinfo = string.Format("转生级别不在有效范围[0-100]!");
                                GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                    client, strinfo);

                                return true;
                            }

                            int nChangeCount = otherClient.ClientData.ChangeLifeCount;

                            lock (otherClient.ClientData.PropPointMutex)
                            {
                                if (cmdFields.Length > 5 && cmdFields[5] == "1")
                                {
                                    int countExt = SafeConvertToInt32(cmdFields[4]);

                                    otherClient.ClientData.PropStrength += countExt - Global.GetRoleParamsInt32FromDB(otherClient, RoleParamName.sPropStrengthChangeless);
                                    otherClient.ClientData.PropIntelligence += countExt - Global.GetRoleParamsInt32FromDB(otherClient, RoleParamName.sPropIntelligenceChangeless);
                                    otherClient.ClientData.PropDexterity += countExt - Global.GetRoleParamsInt32FromDB(otherClient, RoleParamName.sPropDexterityChangeless);
                                    otherClient.ClientData.PropConstitution += countExt - Global.GetRoleParamsInt32FromDB(otherClient, RoleParamName.sPropConstitutionChangeless);

                                    otherClient.ClientData.TotalPropPoint += countExt - Global.GetRoleParamsInt32FromDB(otherClient, RoleParamName.sPropStrengthChangeless);
                                    otherClient.ClientData.TotalPropPoint += countExt - Global.GetRoleParamsInt32FromDB(otherClient, RoleParamName.sPropIntelligenceChangeless);
                                    otherClient.ClientData.TotalPropPoint += countExt - Global.GetRoleParamsInt32FromDB(otherClient, RoleParamName.sPropDexterityChangeless);
                                    otherClient.ClientData.TotalPropPoint += countExt - Global.GetRoleParamsInt32FromDB(otherClient, RoleParamName.sPropConstitutionChangeless);

                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropStrength, otherClient.ClientData.PropStrength, true);
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropIntelligence, otherClient.ClientData.PropIntelligence, true);
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropDexterity, otherClient.ClientData.PropDexterity, true);
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropConstitution, otherClient.ClientData.PropConstitution, true);

                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.TotalPropPoint, otherClient.ClientData.TotalPropPoint, true);
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropStrengthChangeless, countExt, true);
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropIntelligenceChangeless, countExt, true);
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropDexterityChangeless, countExt, true);
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropConstitutionChangeless, countExt, true);
                                }

                                if (cmdFields.Length > 5 && cmdFields[5] == "0")
                                {
                                    //首先进行洗点操作
                                    otherClient.ClientData.PropStrength = 0;
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropStrength, otherClient.ClientData.PropStrength, true);

                                    otherClient.ClientData.PropIntelligence = 0;
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropIntelligence, otherClient.ClientData.PropIntelligence, true);

                                    otherClient.ClientData.PropDexterity = 0;
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropDexterity, otherClient.ClientData.PropDexterity, true);

                                    otherClient.ClientData.PropConstitution = 0;
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropConstitution, otherClient.ClientData.PropConstitution, true);

                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropStrengthChangeless, 0, true);
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropIntelligenceChangeless, 0, true);
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropDexterityChangeless, 0, true);
                                    Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sPropConstitutionChangeless, 0, true);

                                    //计算属性点变化
                                    do
                                    {
                                        if (nNewChangeCount > nChangeCount)
                                        {
                                            nChangeCount++;
                                            //nAwardPropPoint += Data.ChangeLifeInfoList[nChangeCount].AwardPropPoint;
                                        }
                                        else
                                        {
                                            nChangeCount--;
                                            //nAwardPropPoint -= Data.ChangeLifeInfoList[nChangeCount].AwardPropPoint;
                                        }
                                    } while (nNewChangeCount != nChangeCount);
                                }
                                else
                                {
                                    nChangeCount = nNewChangeCount;
                                }

                                // 转生成功 -- DB保存住转生计数
                                Global.SaveRoleParamsInt32ValueToDB(otherClient, RoleParamName.sChangeLifeCount, nChangeCount, true);

                                client.ClientData.ChangeLifeCount = nChangeCount;
                            }

                            GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_SPR_EXECUTECHANGELIFE, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.ChangeLifeCount), null);

                            GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
                        }

                        //通知客户端属性变化
                        {
                            //通知客户端属性变化
                            GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);

                            // 总生命值和魔法值变化通知(同一个地图才需要通知)
                            GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);

                            //通知组队中的其他队员自己的级别发生了变化
                            GameManager.ClientMgr.NotifyTeamUpLevel(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);

                            //自动学习技能
                            Global.AutoLearnSkills(client);

                            //判断技能是否能够自动升级
                            //Global.AutoUpLevelSkills(client);
                        }

                        // 添加了新经验(只通知自己)
                        GameManager.ClientMgr.NotifySelfExperience(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, 0);

                        GameManager.SystemServerEvents.AddEvent(string.Format("角色获取经验和级别, roleID={0}({1}), Level={2}, Experience={3}, newExperience={4}",
                            otherClient.ClientData.RoleID, otherClient.ClientData.RoleName, otherClient.ClientData.Level, otherClient.ClientData.Experience, 0),
                            EventLevels.Hint);

                        strinfo = string.Format("为{0}设置了级别{1}", otherRoleName, level);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-clearguildmap" == cmdFields[0]) //给角色添加经验, 负数则进行减去操作
            {
                if (client.ClientData.Faction > 0)
                {
                    Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.GuildCopyMapAwardFlag, 0, true);
                    GameManager.GuildCopyMapDBMgr.ResetGuildCopyMapDB(client.ClientData.Faction);
                }
            }
            else if ("-addexp" == cmdFields[0]) //给角色添加经验, 负数则进行减去操作
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -addexp 角色名称 经验(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int newexp = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加经验{1}", otherRoleName, newexp));

                        //处理角色经验
                        GameManager.ClientMgr.ProcessRoleExperience(otherClient, newexp, false);

                        strinfo = string.Format("为{0}添加了经验{1}", otherRoleName, newexp);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-addexp2" == cmdFields[0]) //给所有在线的角色添加经验
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -addexp2 当前等级需要经验的百分数(最大100)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        int addPercent = SafeConvertToInt32(cmdFields[1]);
                        addPercent = Math.Max(0, addPercent);
                        addPercent = Math.Min(100, addPercent);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为所有在线用户添加经验百分比{0}", addPercent));

                        //为所有在线角色添加经验
                        GameManager.ClientMgr.AddAllOnlieRoleExperience(addPercent);

                        strinfo = string.Format("为所有在线用户添加经验百分比{0}", addPercent);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);

                        //如果成功，则通知线所有服务器
                        string gmCmdData = string.Format("-addexp2 {0}", cmdFields[1]);

                        //转发GM消息到DBServer
                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_SPR_CHAT,
                            string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", client.ClientData.RoleID, "", 0, "", 0, gmCmdData, 0, 0, GameManager.ServerLineID),
                            null);
                    }
                }
                else
                {
                    if (cmdFields.Length >= 2)
                    {
                        int addPercent = SafeConvertToInt32(cmdFields[1]);
                        addPercent = Math.Max(0, addPercent);
                        addPercent = Math.Min(100, addPercent);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为所有在线用户添加经验百分比{0}", addPercent));

                        //为所有在线角色添加经验
                        GameManager.ClientMgr.AddAllOnlieRoleExperience(addPercent);
                    }
                }
            }
            else if ("-addipower" == cmdFields[0]) //给角色添加内力, 负数则进行减去操作
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -addipower 角色名称 内力(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int interPower = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加灵力{1}", otherRoleName, interPower));

                        if (interPower > 0)
                        {
                            GameManager.ClientMgr.AddInterPower(otherClient, interPower, false);
                        }
                        else
                        {
                            GameManager.ClientMgr.SubInterPower(otherClient, -interPower);
                        }

                        strinfo = string.Format("为{0}添加了灵力{1}", otherRoleName, interPower);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-addmoney" == cmdFields[0]) //给角色添加游戏币, 负数则进行减去操作
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -addmoney 角色名称 游戏币(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int money1 = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加游戏币{1}", otherRoleName, money1));

                        //更新用户的铜钱
                        GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, money1, "GM指令添加绑金", true);

                        GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金钱, roleID={0}({1}), Money={2}, newMoney={3}",
                            otherClient.ClientData.RoleID, otherClient.ClientData.RoleName, otherClient.ClientData.Money1, money1), EventLevels.Record);

                        strinfo = string.Format("为{0}添加了游戏币{1}", otherRoleName, money1);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
                else
                {
                    if (cmdFields.Length >= 3)
                    {
                        string otherRoleName = cmdFields[1];
                        int money1 = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            //不在线的情况

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            //不在线的情况

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加游戏币{1}", otherRoleName, money1));

                        //更新用户的铜钱
                        GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, money1, "GM指令添加绑金", true);

                        GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金钱, roleID={0}({1}), Money={2}, newMoney={3}",
                            otherClient.ClientData.RoleID, otherClient.ClientData.RoleName, otherClient.ClientData.Money1, money1), EventLevels.Record);
                    }
                }
            }
            else if ("-addyl" == cmdFields[0]) //给角色添加银两, 负数则进行减去操作
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -addlj 角色名称 银两(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int yinLiang = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加银两{1}", otherRoleName, yinLiang));

                        if (yinLiang >= 0)
                        {
                            GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(yinLiang), "GM指令添加");
                        }
                        else
                        {
                            GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(yinLiang), "GM指令扣除");
                        }

                        GameManager.SystemServerEvents.AddEvent(string.Format("角色获取银两, roleID={0}({1}), YinLiang={2}, newYinLiang={3}",
                            otherClient.ClientData.RoleID, otherClient.ClientData.RoleName, otherClient.ClientData.YinLiang, yinLiang), EventLevels.Record);

                        strinfo = string.Format("为{0}添加了银两{1}", otherRoleName, yinLiang);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);

                        //string writerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //GameManager.DBEventsWriter.CacheGm_yinliang(-1,
                        //    client.ClientData.ZoneID,
                        //    client.ClientData.RoleID,
                        //    client.ClientData.RoleName,
                        //    otherClient.ClientData.RoleID,
                        //    otherClient.ClientData.RoleName,
                        //    yinLiang,
                        //    writerTime);
                    }
                }
                else
                {
                    if (cmdFields.Length >= 3)
                    {
                        string otherRoleName = cmdFields[1];
                        int yinLiang = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            //如果没在线

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            //如果没在线

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加银两{1}", otherRoleName, yinLiang));

                        if (yinLiang >= 0)
                        {
                            GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(yinLiang), "GM指令添加");
                        }
                        else
                        {
                            GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(yinLiang), "GM指令扣除");
                        }

                        GameManager.SystemServerEvents.AddEvent(string.Format("角色获取银两, roleID={0}({1}), YinLiang={2}, newYinLiang={3}",
                            otherClient.ClientData.RoleID, otherClient.ClientData.RoleName, otherClient.ClientData.YinLiang, yinLiang), EventLevels.Record);
                    }
                }
            }
            else if ("-addgold" == cmdFields[0]) //给角色添加金币, 负数则进行减去操作
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -addlj 角色名称 金币(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int gold = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加金币{1}", otherRoleName, gold));

                        if (gold >= 0)
                        {
                            GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(gold), "GM指令");
                        }
                        else
                        {
                            GameManager.ClientMgr.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(gold), "GM指令");
                        }

                        GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金币, roleID={0}({1}), Gold={2}, newGold={3}",
                            otherClient.ClientData.RoleID, otherClient.ClientData.RoleName, otherClient.ClientData.Gold, gold), EventLevels.Record);

                        strinfo = string.Format("为{0}添加了金币{1}", otherRoleName, gold);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);

                        //先屏蔽
                        //string writerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //GameManager.DBEventsWriter.CacheGm_gold(-1,
                        //    client.ClientData.ZoneID,
                        //    client.ClientData.RoleID,
                        //    client.ClientData.RoleName,
                        //    otherClient.ClientData.RoleID,
                        //    otherClient.ClientData.RoleName,
                        //    gold,
                        //    writerTime);
                    }
                }
            }
            else if ("-adddj" == cmdFields[0]) //给角色添加点卷, 负数则进行减去操作
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -adddj 角色名称 元宝(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int dianjuan = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加元宝{1}", otherRoleName, dianjuan));

                        if (dianjuan >= 0)
                        {
                            GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(dianjuan), "GM要求添加");
                        }
                        else
                        {
                            GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(dianjuan), "GM要求扣除");
                        }

                        strinfo = string.Format("为{0}添加了元宝{1}", otherRoleName, dianjuan);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);

                        //string writerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //GameManager.DBEventsWriter.CacheGm_yuanbao(-1,
                        //    client.ClientData.ZoneID,
                        //    client.ClientData.RoleID,
                        //    client.ClientData.RoleName,
                        //    otherClient.ClientData.RoleID,
                        //    otherClient.ClientData.RoleName,
                        //    dianjuan,
                        //    writerTime);
                    }
                }
            }
            else if ("-additem" == cmdFields[0]) //给角色添加点卷, 负数则进行减去操作
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 9)
                    {
                        strinfo = string.Format("请输入： -additem 角色名称 物品名称 个数(1~99) 绑定(0/1) 级别(0~10) 追加 幸运 卓越");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        // gm命令改造 去掉品质 增加 追加等级,是否有幸运,卓越属性 -- additem rolename itemname count bind level appendprop lucky excellenceinfo[1/27/2014 LiaoWei]
                        string otherRoleName = cmdFields[1];
                        string goodsName = cmdFields[2];
                        int gcount = SafeConvertToInt32(cmdFields[3]);
                        gcount = Global.GMax(0, gcount);
                        gcount = Global.GMin(99, gcount);
                        int binding = SafeConvertToInt32(cmdFields[4]);
                        int level = SafeConvertToInt32(cmdFields[5]);
                        int appendprop = SafeConvertToInt32(cmdFields[6]);
                        int lucky = SafeConvertToInt32(cmdFields[7]);
                        int excellenceinfo = SafeConvertToInt32(cmdFields[8]);

                        //string qualityName = cmdFields[5];


                        int goodsID = Global.GetGoodsByName(goodsName);
                        if (-1 == goodsID)
                        {
                            strinfo = string.Format("系统中不存在{0}", goodsName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        level = Global.GMax(0, level);
                        level = Global.GMin(15, level);

                        /*int quality = 0;
                        if ("绿色" == qualityName)
                        {
                            quality = 1;
                        }
                        else if ("蓝色" == qualityName)
                        {
                            quality = 2;
                        }
                        else if ("紫色" == qualityName)
                        {
                            quality = 3;
                        }
                        else if ("金色" == qualityName)
                        {
                            quality = 4;
                        }*/

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        SystemXmlItem systemGoods = null;
                        if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
                        {
                            strinfo = string.Format("系统中不存在{0}", goodsName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        int site = 0;
                        int categoriy = systemGoods.GetIntValue("Categoriy");
                        if (categoriy >= (int)ItemCategories.ElementHrtBegin && categoriy < (int)ItemCategories.ElementHrtEnd)
                        {
                            site = (int)SaleGoodsConsts.ElementhrtsGoodsID;
                        }

                        if (systemGoods.GetIntValue("GridNum") <= 1)
                        {
                            for (int i = 0; i < gcount; i++)
                            {
                                if (!Global.CanAddGoods(otherClient, goodsID, 1, binding))
                                {
                                    strinfo = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", otherRoleName, i, goodsName, gcount - i);
                                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                        client, strinfo);

                                    return true;
                                }

                                LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", otherRoleName, goodsName, 1, level, 0, binding));

                                //添加物品
                                Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, 1, 0, "", level, binding, site, "", true, 1,
                                                            "GM添加", Global.ConstGoodsEndTime, 0, 0, lucky, 0, excellenceinfo, appendprop);
                            }
                        }
                        else
                        {
                            if (!Global.CanAddGoods(otherClient, goodsID, gcount, binding))
                            {
                                strinfo = string.Format("{0}背包已经满，无法添加{1}", otherRoleName, goodsName);
                                GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                    client, strinfo);

                                return true;
                            }

                            LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", otherRoleName, goodsName, gcount, level, 0, binding));

                            //添加物品
                            Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, gcount, 0, "", level, binding, site, "", true, 1,
                                                        "GM添加", Global.ConstGoodsEndTime, 0, 0, lucky, 0, excellenceinfo, appendprop);
                        };

                        strinfo = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}", otherRoleName, goodsName, gcount, level, 0, binding);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);

                        /*string userID = GameManager.OnlineUserSession.FindUserID(otherClient.ClientSocket);
                        string userName = GameManager.OnlineUserSession.FindUserName(otherClient.ClientSocket);
                        string writerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        GameManager.DBEventsWriter.CacheGm_goods(-1,
                            otherClient.ClientData.ZoneID,
                            userID,
                            userName,
                            otherClient.ClientData.RoleID,
                            otherClient.ClientData.RoleName,
                            -1,
                            goodsID,
                            gcount,
                            binding,
                            quality,
                            level,
                            "",
                            0,
                            Global.ConstGoodsEndTime,
                            gcount,
                            "GM添加",
                            writerTime,
                            client.ClientData.RoleID,
                            client.ClientData.RoleName);*/
                    }
                }
            }
            else if ("-additem2" == cmdFields[0]) //-additem2 给角色添加限时的物品(不允许添加装备), 请输入： -additem2 角色名称 物品名称 个数 绑定(0/1) 限制日期(2011-01-01) 限制时间(00$00$00)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 7)
                    {
                        strinfo = string.Format("请输入： -additem2 角色名称 物品名称 个数 绑定(0/1) 限制日期(2011-01-01) 限制时间(00$00$00)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        string goodsName = cmdFields[2];
                        int gcount = SafeConvertToInt32(cmdFields[3]);
                        gcount = Global.GMax(0, gcount);
                        gcount = Global.GMin(99, gcount);
                        int binding = SafeConvertToInt32(cmdFields[4]);
                        string limitDate = cmdFields[5];
                        string limitTime = cmdFields[6];
                        string limitDateTime = string.Format("{0} {1}", limitDate, limitTime);
                        limitDateTime = limitDateTime.Replace("$", ":");
                        if (Global.DateTimeTicks(limitDateTime) <= 0)
                        {
                            strinfo = string.Format("限时格式错误{0} {1}", limitDate, limitTime);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        int goodsID = Global.GetGoodsByName(goodsName);
                        if (-1 == goodsID)
                        {
                            strinfo = string.Format("系统中不存在{0}", goodsName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsID);
                        if (goodsCatetoriy < (int)ItemCategories.EquipMax || (goodsCatetoriy >= (int)ItemCategories.ElementHrtBegin && goodsCatetoriy < (int)ItemCategories.ElementHrtEnd))
                        {
                            strinfo = string.Format("不能添加限时的{0}", goodsName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);
                            return true;
                        }

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        SystemXmlItem systemGoods = null;
                        if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
                        {
                            strinfo = string.Format("系统中不存在{0}", goodsName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        if (systemGoods.GetIntValue("GridNum") <= 1)
                        {
                            for (int i = 0; i < gcount; i++)
                            {
                                if (!Global.CanAddGoods(otherClient, goodsID, 1, binding, limitDateTime))
                                {
                                    strinfo = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", otherRoleName, i, goodsName, gcount - i);
                                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                        client, strinfo);

                                    return true;
                                }

                                LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}, 结束时间:{6}", otherRoleName, goodsName, 1, 0, "白色", binding, limitDateTime));

                                //添加物品
                                Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, 1, 0, "", 0, binding, 0, "", true, 1, "GM添加", limitDateTime);
                            }
                        }
                        else
                        {
                            if (!Global.CanAddGoods(otherClient, goodsID, gcount, binding, limitDateTime))
                            {
                                strinfo = string.Format("{0}背包已经满，无法添加{1}", otherRoleName, goodsName);
                                GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                    client, strinfo);

                                return true;
                            }

                            LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}, 结束时间:{6}", otherRoleName, goodsName, gcount, 0, "白色", binding, limitDateTime));

                            //添加物品
                            Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, gcount, 0, "", 0, binding, 0, "", true, 1, "GM添加", limitDateTime);
                        };

                        strinfo = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}, 结束时间:{6} {7}", otherRoleName, goodsName, gcount, 0, "白色", binding, limitDate, limitTime);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);

                        /*string userID = GameManager.OnlineUserSession.FindUserID(otherClient.ClientSocket);
                        string userName = GameManager.OnlineUserSession.FindUserName(otherClient.ClientSocket);
                        string writerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        GameManager.DBEventsWriter.CacheGm_goods(-1,
                            otherClient.ClientData.ZoneID,
                            userID,
                            userName,
                            otherClient.ClientData.RoleID,
                            otherClient.ClientData.RoleName,
                            -1,
                            goodsID,
                            gcount,
                            binding,
                            0,
                            0,
                            "",
                            0,
                            limitDateTime,
                            gcount,
                            "GM添加",
                            writerTime,
                            client.ClientData.RoleID,
                            client.ClientData.RoleName);*/
                    }
                }
            }
            else if ("-addid" == cmdFields[0]) //给角色添加物品
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 9)
                    {
                        strinfo = string.Format("请输入： -addid 角色名称 物品ID 个数(1~99) 绑定(0/1) 级别(0~10) 追加 幸运 卓越");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        // gm命令改造 去掉品质 增加 追加等级,是否有幸运,卓越属性 -- addid rolename itemid count bind level appendprop lucky excellenceinfo[1/27/2014 LiaoWei]

                        string otherRoleName = cmdFields[1];
                        int goodsID = SafeConvertToInt32(cmdFields[2]);
                        int gcount = SafeConvertToInt32(cmdFields[3]);
                        gcount = Global.GMax(0, gcount);
                        gcount = Global.GMin(99, gcount);
                        int binding = SafeConvertToInt32(cmdFields[4]);
                        int level = SafeConvertToInt32(cmdFields[5]);
                        int appendprop = SafeConvertToInt32(cmdFields[6]);
                        int lucky = SafeConvertToInt32(cmdFields[7]);
                        int excellenceinfo = SafeConvertToInt32(cmdFields[8]);

                        level = Global.GMax(0, level);
                        level = Global.GMin(15, level);

                        int quality = 0;
                        /*if ("绿色" == qualityName)
                        {
                            quality = 1;
                        }
                        else if ("蓝色" == qualityName)
                        {
                            quality = 2;
                        }
                        else if ("紫色" == qualityName)
                        {
                            quality = 3;
                        }
                        else if ("金色" == qualityName)
                        {
                            quality = 4;
                        }*/

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        SystemXmlItem systemGoods = null;
                        if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
                        {
                            strinfo = string.Format("系统中不存在{0}", goodsID);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        int site = 0;
                        int categoriy = systemGoods.GetIntValue("Categoriy");
                        if (categoriy >= (int)ItemCategories.ElementHrtBegin && categoriy < (int)ItemCategories.ElementHrtEnd)
                        {
                            site = (int)SaleGoodsConsts.ElementhrtsGoodsID;
                        }

                        string goodsName = systemGoods.GetStringValue("Title");
                        if (systemGoods.GetIntValue("GridNum") <= 1)
                        {
                            for (int i = 0; i < gcount; i++)
                            {
                                if (!Global.CanAddGoods(otherClient, goodsID, 1, binding))
                                {
                                    strinfo = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", otherRoleName, i, goodsName, gcount - i);
                                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                        client, strinfo);

                                    return true;
                                }

                                LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", otherRoleName, goodsName, 1, level, quality, binding));

                                //添加物品
                                Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, 1, 0, "", level, binding, site, "", true, 1,
                                                            "GM添加", Global.ConstGoodsEndTime, 0, 0, lucky, 0, excellenceinfo, appendprop);
                            }
                        }
                        else
                        {
                            if (!Global.CanAddGoods(otherClient, goodsID, gcount, binding))
                            {
                                strinfo = string.Format("{0}背包已经满，无法添加{1}", otherRoleName, goodsName);
                                GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                    client, strinfo);

                                return true;
                            }

                            LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", otherRoleName, goodsName, gcount, level, quality, binding));

                            //添加物品
                            Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, gcount, 0, "", level, binding, site, "", true, 1,
                                                        "GM添加", Global.ConstGoodsEndTime, 0, 0, lucky, 0, excellenceinfo, appendprop);
                        };

                        strinfo = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}", otherRoleName, goodsName, gcount, level, quality, binding);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);

                        /*string userID = GameManager.OnlineUserSession.FindUserID(otherClient.ClientSocket);
                        string userName = GameManager.OnlineUserSession.FindUserName(otherClient.ClientSocket);
                        string writerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        GameManager.DBEventsWriter.CacheGm_goods(-1,
                            otherClient.ClientData.ZoneID,
                            userID,
                            userName,
                            otherClient.ClientData.RoleID,
                            otherClient.ClientData.RoleName,
                            -1,
                            goodsID,
                            gcount,
                            binding,
                            quality,
                            level,
                            "",
                            0,
                            Global.ConstGoodsEndTime,
                            gcount,
                            "GM添加",
                            writerTime,
                            client.ClientData.RoleID,
                            client.ClientData.RoleName);*/
                    }
                }
                else
                {
                    if (cmdFields.Length >= 9)
                    {
                        // gm命令改造 去掉品质 增加 追加等级,是否有幸运,卓越属性 -- addid rolename itemid count bind level appendprop lucky excellenceinfo[1/27/2014 LiaoWei]

                        string otherRoleName = cmdFields[1];
                        int goodsID = SafeConvertToInt32(cmdFields[2]);
                        int gcount = SafeConvertToInt32(cmdFields[3]);
                        gcount = Global.GMax(0, gcount);
                        gcount = Global.GMin(99, gcount);
                        int binding = SafeConvertToInt32(cmdFields[4]);
                        int level = SafeConvertToInt32(cmdFields[5]);
                        int appendprop = SafeConvertToInt32(cmdFields[6]);
                        int lucky = SafeConvertToInt32(cmdFields[7]);
                        int excellenceinfo = SafeConvertToInt32(cmdFields[8]);

                        level = Global.GMax(0, level);
                        level = Global.GMin(15, level);

                        int quality = 0;
                        /*if ("绿色" == qualityName)
                        {
                            quality = 1;
                        }
                        else if ("蓝色" == qualityName)
                        {
                            quality = 2;
                        }
                        else if ("紫色" == qualityName)
                        {
                            quality = 3;
                        }
                        else if ("金色" == qualityName)
                        {
                            quality = 4;
                        }*/

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("根据GM的要求为{0}添加物品,目标不在线", otherRoleName);
                            LogManager.WriteLog(LogTypes.SQL, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("根据GM的要求为{0}添加物品,目标不在线", otherRoleName);
                            LogManager.WriteLog(LogTypes.SQL, strinfo);

                            return true;
                        }

                        SystemXmlItem systemGoods = null;
                        if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
                        {
                            strinfo = string.Format("根据GM的要求为{1}添加物品,但系统中不存在{0}", goodsID, otherRoleName);
                            LogManager.WriteLog(LogTypes.SQL, strinfo);

                            return true;
                        }

                        int site = 0;
                        int categoriy = systemGoods.GetIntValue("Categoriy");
                        if (categoriy >= (int)ItemCategories.ElementHrtBegin && categoriy < (int)ItemCategories.ElementHrtEnd)
                        {
                            site = (int)SaleGoodsConsts.ElementhrtsGoodsID;
                        }

                        string goodsName = systemGoods.GetStringValue("Title");
                        if (systemGoods.GetIntValue("GridNum") <= 1)
                        {
                            for (int i = 0; i < gcount; i++)
                            {
                                if (!Global.CanAddGoods(otherClient, goodsID, 1, binding))
                                {
                                    strinfo = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{3}个无法添加{1}", otherRoleName, i, goodsName, gcount - i);
                                    LogManager.WriteLog(LogTypes.SQL, strinfo);

                                    return true;
                                }

                                LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", otherRoleName, goodsName, 1, level, quality, binding));

                                //添加物品
                                Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, 1, 0, "", level, binding, site, "", true, 1,
                                                            "GM添加", Global.ConstGoodsEndTime, 0, 0, lucky, 0, excellenceinfo, appendprop);
                            }
                        }
                        else
                        {
                            if (!Global.CanAddGoods(otherClient, goodsID, gcount, binding))
                            {
                                strinfo = string.Format("{0}背包已经满，无法添加{1}", otherRoleName, goodsName);
                                LogManager.WriteLog(LogTypes.SQL, strinfo);

                                return true;
                            }

                            LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", otherRoleName, goodsName, gcount, level, quality, binding));

                            //添加物品
                            Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, gcount, 0, "", level, binding, site, "", true, 1,
                                                        "GM添加", Global.ConstGoodsEndTime, 0, 0, lucky, 0, excellenceinfo, appendprop);
                        };
                    }
                }
            }
            else if ("-setpkv" == cmdFields[0]) //设置角色的pk值, 最小值0  pk点(最小值0)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -setpkv 角色名称 pk值(最小值0) pk点(最小值0)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int pkValue = SafeConvertToInt32(cmdFields[2]);
                        int pkPoint = SafeConvertToInt32(cmdFields[3]);
                        pkValue = Global.GMax(0, pkValue);
                        pkPoint = Global.GMax(0, pkPoint);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置PK值{1}", otherRoleName, pkValue));

                        //设置PK值(限制当前地图)
                        GameManager.ClientMgr.SetRolePKValuePoint(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            otherClient, pkValue, pkPoint);

                        // 给玩家加上红名处罚BUFFER [4/21/2014 LiaoWei]
                        Global.ProcessRedNamePunishForDebuff(otherClient);

                        strinfo = string.Format("为{0}设置了PK值{1}", otherRoleName, pkValue);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-adddjpoint" == cmdFields[0]) //给角色添加点将台积分, 负数则进行减去操作
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -adddjpoint 角色名称 点将积分(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int djPoint = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加点将积分{1}", otherRoleName, djPoint));

                        //修改数据库中的点将积分
                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_ADDDJPOINT,
                            string.Format("{0}:{1}", otherClient.ClientData.RoleID, djPoint),
                            null);

                        strinfo = string.Format("为{0}添加点将积分{1}", otherRoleName, djPoint);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-settaskv" == cmdFields[0]) //给角色指定的任务(填写任务名称)设置完成值
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 5)
                    {
                        strinfo = string.Format("请输入： -settaskv 角色名称 任务名称 目标类型(1/2) 数值(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        string taskName = cmdFields[2];
                        int valType = SafeConvertToInt32(cmdFields[3]);
                        valType = Global.GMax(1, valType);
                        valType = Global.GMin(2, valType);
                        int taskVal = SafeConvertToInt32(cmdFields[4]);
                        taskVal = Global.GMax(0, taskVal);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置任务{1}, 值类型:{2}, 任务值:{3}", otherRoleName, taskName, valType, taskVal));

                        //直接修改任务的数值
                        ProcessTask.ProcessTaskValue(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            otherClient, taskName, valType, taskVal);

                        strinfo = string.Format("为{0}设置任务{1}, 值类型{2}, 任务值{3}", otherRoleName, taskName, valType, taskVal);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-shutdown" == cmdFields[0]) //关闭游戏服务器
            {
                if (!transmit) //非转发的命令
                {
                    LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为关闭服务器: {0}", DateTime.Now));

                    //System.Windows.Application.Current.Dispatcher.BeginInvoke((MethodInvoker)delegate
                    //{
                    //程序主窗口
                    Program.Exit();
                    //});
                }
            }
            else if ("-auth" == cmdFields[0]) //授权使用广告发布执行
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -auth 公告开关(0/1)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        int auth = SafeConvertToInt32(cmdFields[1]);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的切换授权开关: {0}", auth));

                        //通知GM授权消息
                        GameManager.ClientMgr.NotifyGMAuthCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, auth);
                    }
                }
            }
            else if ("-bull" == cmdFields[0]) //发布公告
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 5)
                    {
                        strinfo = string.Format("请输入： -bull 公告ID(文字和数字都可以) 多少分钟(数字, -1表示永久公告) 次数(数字, 客户端播放次数, -1表示一直循环) 公告文字");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string msgID = cmdFields[1].Trim();
                        int minutes = SafeConvertToInt32(cmdFields[2]);
                        int playNum = SafeConvertToInt32(cmdFields[3]);
                        string bulletinText = cmdFields[4];

                        if (string.IsNullOrEmpty(msgID))
                        {
                            strinfo = string.Format("公告ID不能为空");
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求发布公告: {0} {1} {2} {3}", msgID, minutes, playNum, bulletinText));

                        //添加到公告队列中
                        //公告管理消息对象
                        BulletinMsgData bulletinMsgData = GameManager.BulletinMsgMgr.AddBulletinMsg(msgID, minutes, playNum, bulletinText);

                        //将本条消息广播给所有在线的客户端
                        GameManager.ClientMgr.NotifyAllBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, bulletinMsgData);

                        //如果是永久公告，则存入数据库中
                        if (minutes < 0)
                        {
                            //将公告发布到DBServer
                            Global.AddDBBulletinMsg(msgID, playNum, bulletinText);
                        }

                        string gmCmdData = string.Format("-bull {0} {1} {2} {3}", msgID, minutes, playNum, bulletinText);

                        //转发GM消息到DBServer
                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_SPR_CHAT,
                            string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", client.ClientData.RoleID, "", 0, "", 0, gmCmdData, 0, 0, GameManager.ServerLineID),
                            null);
                    }
                }
                else //如果是转发的公告消息
                {
                    if (cmdFields.Length >= 5)
                    {
                        string msgID = cmdFields[1].Trim();
                        int minutes = SafeConvertToInt32(cmdFields[2]);
                        int playNum = SafeConvertToInt32(cmdFields[3]);
                        string bulletinText = cmdFields[4];

                        //添加到公告队列中
                        //公告管理消息对象
                        BulletinMsgData bulletinMsgData = GameManager.BulletinMsgMgr.AddBulletinMsg(msgID, minutes, playNum, bulletinText);

                        //将本条消息广播给所有在线的客户端
                        GameManager.ClientMgr.NotifyAllBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, bulletinMsgData);
                    }
                }
            }
            else if ("-listbull" == cmdFields[0]) //列出所有的公告消息
            {
                if (!transmit) //非转发的命令
                {
                    LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求列举公告"));

                    //添加到公告队列中
                    //将所有的公告消息枚举给指定的GM客户端
                    GameManager.BulletinMsgMgr.SendAllBulletinMsgToGM(client);
                }
            }
            else if ("-rmbull" == cmdFields[0]) //删除指定的公告消息
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -rmbull 公告ID(文字和数字都可以)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string msgID = cmdFields[1].Trim();
                        if (string.IsNullOrEmpty(msgID))
                        {
                            strinfo = string.Format("公告ID不能为空");
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求删除公告: {0}", msgID));

                        //添加到公告队列中
                        //将所有的公告消息枚举给指定的GM客户端
                        BulletinMsgData bulletinMsgData = GameManager.BulletinMsgMgr.RemoveBulletinMsg(msgID);
                        if (null != bulletinMsgData)
                        {
                            //如果是永久公告，则存入数据库中
                            if (bulletinMsgData.PlayMinutes < 0)
                            {
                                //从DBServer删除公告
                                Global.RemoveDBBulletinMsg(msgID);
                            }
                        }
                    }
                }
            }
            else if ("-sysmsg" == cmdFields[0]) //-sysmsg 发布临时公告(此命令主要是程序用) 请输入：-sysmsg 临时公告ID(文字和数字都可以) 临时公告文字
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -sysmsg 临时公告ID(文字和数字都可以) 临时公告文字");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string msgID = cmdFields[1].Trim();
                        int minutes = 0;
                        int playNum = 1;
                        string bulletinText = cmdFields[2];

                        if (string.IsNullOrEmpty(msgID))
                        {
                            strinfo = string.Format("临时公告ID不能为空");
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求发布临时公告: {0} {1} {2} {3}", msgID, minutes, playNum, bulletinText));

                        //添加到公告队列中
                        //公告管理消息对象
                        BulletinMsgData bulletinMsgData = GameManager.BulletinMsgMgr.AddBulletinMsg(msgID, minutes, playNum, bulletinText, 1);

                        //将本条消息广播给所有在线的客户端
                        GameManager.ClientMgr.NotifyAllBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, bulletinMsgData);

                        string gmCmdData = string.Format("-sysmsg {0} {1} {2} {3}", msgID, minutes, playNum, bulletinText);

                        //转发GM消息到DBServer
                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_SPR_CHAT,
                            string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", client.ClientData.RoleID, "", 0, "", 0, gmCmdData, 0, 0, GameManager.ServerLineID),
                            null);
                    }
                }
                else //如果是转发的公告消息
                {
                    if (cmdFields.Length >= 3)
                    {
                        string msgID = cmdFields[1].Trim();
                        int minutes = 0;
                        int playNum = 1;
                        string bulletinText = cmdFields[2];

                        //添加到公告队列中
                        //公告管理消息对象
                        BulletinMsgData bulletinMsgData = GameManager.BulletinMsgMgr.AddBulletinMsg(msgID, minutes, playNum, bulletinText, 1);

                        //将本条消息广播给所有在线的客户端
                        GameManager.ClientMgr.NotifyAllBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, bulletinMsgData);
                    }
                }
            }
            else if ("-hintmsg" == cmdFields[0]) //-hintmsg 发布提示消息(此命令主要是程序内部使用) 请输入：-hintmsg 消息类型 显示类型 提示文字
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 4)
                    {
                        strinfo = string.Format("请输入： -hintmsg 消息类型 显示类型 提示文字");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        int infoType = SafeConvertToInt32(cmdFields[1]);
                        int showType = SafeConvertToInt32(cmdFields[2]);
                        string hintMsgText = cmdFields[3];

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求发布提示消息: {0} {1} {2}", infoType, showType, hintMsgText));

                        GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            null, hintMsgText, (GameInfoTypeIndexes)infoType, (ShowGameInfoTypes)showType);

                        string gmCmdData = string.Format("-hintmsg {0} {1} {2}", infoType, showType, hintMsgText);

                        //转发GM消息到DBServer
                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_SPR_CHAT,
                            string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", client.ClientData.RoleID, "", 0, "", 0, gmCmdData, 0, 0, GameManager.ServerLineID),
                            null);
                    }
                }
                else //如果是转发的公告消息
                {
                    if (cmdFields.Length >= 4)
                    {
                        int infoType = SafeConvertToInt32(cmdFields[1]);
                        int showType = SafeConvertToInt32(cmdFields[2]);
                        string hintMsgText = cmdFields[3];

                        GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            null, hintMsgText, (GameInfoTypeIndexes)infoType, (ShowGameInfoTypes)showType);
                    }
                }
            }
            else if ("-hintmsg2" == cmdFields[0]) //-hintmsg2 发布帮会提示消息(此命令主要是程序内部使用) 请输入：-hintmsg2 帮会ID 消息类型 显示类型 提示文字
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 5)
                    {
                        strinfo = string.Format("请输入： -hintmsg2 帮会ID 消息类型 显示类型 提示文字");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        int faction = SafeConvertToInt32(cmdFields[1]);
                        int infoType = SafeConvertToInt32(cmdFields[2]);
                        int showType = SafeConvertToInt32(cmdFields[3]);
                        string hintMsgText = cmdFields[4];

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求发布帮会提示消息: {0} {1} {2} {3}", faction, infoType, showType, hintMsgText));

                        string gmCmdData = string.Format("-hintmsg2 {0} {1} {2} {3}", faction, infoType, showType, hintMsgText);

                        //转发GM消息到DBServer
                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_SPR_CHAT,
                            string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", client.ClientData.RoleID, "", 0, "", 0, gmCmdData, 0, 0, GameManager.ServerLineID),
                            null);
                    }
                }
                else //如果是转发的公告消息
                {
                    if (cmdFields.Length >= 5)
                    {
                        int faction = SafeConvertToInt32(cmdFields[1]);
                        int infoType = SafeConvertToInt32(cmdFields[2]);
                        int showType = SafeConvertToInt32(cmdFields[3]);
                        string hintMsgText = cmdFields[4];

                        GameManager.ClientMgr.NotifyBangHuiImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            faction, hintMsgText, (GameInfoTypeIndexes)infoType, (ShowGameInfoTypes)showType);
                    }
                }
            }
            else if ("-config" == cmdFields[0]) //设置参数
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -config 参数名称 参数值");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string paramName = cmdFields[1].Trim();
                        string paramValue = cmdFields[2].Trim();

                        if (string.IsNullOrEmpty(paramName))
                        {
                            strinfo = string.Format("参数名称不能为空");
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        if (string.IsNullOrEmpty(paramValue))
                        {
                            strinfo = string.Format("参数值不能为空");
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求更改游戏参数: {0}=>{1}", paramName, paramValue));

                        //更改DB游戏参数
                        Global.UpdateDBGameConfigg(paramName, paramValue);

                        if ("kaifutime" == paramName)
                        {
                            //ReloadXmlManager.ReloadXmlFile_config_gifts_EquipKing();
                            //ReloadXmlManager.ReloadXmlFile_config_gifts_HorseKing();
                            //ReloadXmlManager.ReloadXmlFile_config_gifts_JingMaiKing();
                            //ReloadXmlManager.ReloadXmlFile_config_gifts_LevelKing();
                            //ReloadXmlManager.ReloadXmlFile_config_gifts_ChongZhiKing();
                            //ReloadXmlManager.ReloadXmlFile_config_gifts_FanLi();
                            //ReloadXmlManager.ReloadXmlFile_config_gifts_ChongZhiSong();
                            //ReloadXmlManager.ReloadXmlFile_config_gifts_huodongloginnumgift();

                            //有人会不知道或忘记在这里重新加载配置和清空活动缓存，所以统一重新加载所有配置文件。如果ReloadAllXmlFile都不行，那重启服务器去吧
                            ReloadXmlManager.ReloadAllXmlFile();
                        }
                        else if ("jieristartday" == paramName)
                        {
                            //充值限制掉落的时间项
                            //GameManager.GoodsPackMgr.ResetLimitTimeRange();

                            //有人会不知道或忘记在这里重新加载配置和清空活动缓存，所以统一重新加载所有配置文件。如果ReloadAllXmlFile都不行，那重启服务器去吧
                            ReloadXmlManager.ReloadAllXmlFile();
                        }
                        else if ("hefutime" == paramName)
                        {
                            //HuodongCachingMgr.ResetHeFuActivityConfig();
                            //HuodongCachingMgr.ResetHeFuLoginActivity();
                            //HuodongCachingMgr.ResetHeFuTotalLoginActivity();
                            //HuodongCachingMgr.ResetHeFuRechargeActivity();
                            //HuodongCachingMgr.ResetHeFuPKKingActivity();
                            //HuodongCachingMgr.ResetHeFuAwardTimeActivity();
                            //HuodongCachingMgr.ResetHeFuVIPActivity();
                            //HuodongCachingMgr.ResetHeFuWCKingActivity();

                            //有人会不知道或忘记在这里重新加载配置和清空活动缓存，所以统一重新加载所有配置文件。如果ReloadAllXmlFile都不行，那重启服务器去吧
                            ReloadXmlManager.ReloadAllXmlFile();
                        }
                        else if ("yueduchoujiangstartday" == paramName)
                        {
                            HuodongCachingMgr.ResetYueDuZhuanPanActivity();
                        }
                        else if ("maxposcmdnum" == paramName)
                        {
                            TCPSession.MaxPosCmdNumPer5Seconds = GameManager.GameConfigMgr.GetGameConfigItemInt("maxposcmdnum", 10);
                        }
                        else if ("maxsubticks" == paramName)
                        {
                            TCPSession.MaxAntiProcessJiaSuSubTicks = GameManager.GameConfigMgr.GetGameConfigItemInt("maxsubticks", 1000);
                        }
                        else if ("maxsubnum" == paramName)
                        {
                            TCPSession.MaxAntiProcessJiaSuSubNum = GameManager.GameConfigMgr.GetGameConfigItemInt("maxsubnum", 3);
                        }
                        else if ("whiteiplist" == paramName)
                        {
                            // 重新加载程序配置参数文件
                            Program.LoadIPList(paramValue);
                        }
                    }
                }
                else
                {
                    if (cmdFields.Length == 3)
                    {
                        string paramName = cmdFields[1].Trim();
                        string paramValue = cmdFields[2].Trim();

                        if (string.IsNullOrEmpty(paramName))
                        {
                            return true;
                        }

                        if (string.IsNullOrEmpty(paramValue))
                        {
                            return true;
                        }

                        //更改游戏参数
                        GameManager.GameConfigMgr.SetGameConfigItem(paramName, paramValue);

                        if ("kaifutime" == paramName)
                        {
                            /*ReloadXmlManager.ReloadXmlFile_config_gifts_EquipKing();
                            ReloadXmlManager.ReloadXmlFile_config_gifts_HorseKing();
                            ReloadXmlManager.ReloadXmlFile_config_gifts_JingMaiKing();
                            ReloadXmlManager.ReloadXmlFile_config_gifts_LevelKing();
                            ReloadXmlManager.ReloadXmlFile_config_gifts_ChongZhiKing();
                            ReloadXmlManager.ReloadXmlFile_config_gifts_FanLi();
                            ReloadXmlManager.ReloadXmlFile_config_gifts_ChongZhiSong();
                            ReloadXmlManager.ReloadXmlFile_config_gifts_huodongloginnumgift();*/

                            //有人会不知道或忘记在这里重新加载配置和清空活动缓存，所以统一重新加载所有配置文件。如果ReloadAllXmlFile都不行，那重启服务器去吧
                            ReloadXmlManager.ReloadAllXmlFile();
                        }
                        else if ("jieristartday" == paramName)
                        {
                            //充值限制掉落的时间项
                            //GameManager.GoodsPackMgr.ResetLimitTimeRange();

                            //有人会不知道或忘记在这里重新加载配置和清空活动缓存，所以统一重新加载所有配置文件。如果ReloadAllXmlFile都不行，那重启服务器去吧
                            ReloadXmlManager.ReloadAllXmlFile();

                            HuodongCachingMgr.ResetJieriActivityConfig();
                            HuodongCachingMgr.ResetJieriDaLiBaoActivity();
                            HuodongCachingMgr.ResetJieRiDengLuActivity();
                            HuodongCachingMgr.ResetJieriCZSongActivity();
                            HuodongCachingMgr.ResetJieRiLeiJiCZActivity();
                            HuodongCachingMgr.ResetJieRiTotalConsumeActivity();
                            HuodongCachingMgr.ResetJieRiMultAwardActivity();
                            HuodongCachingMgr.ResetJieRiZiKaLiaBaoActivity();
                            HuodongCachingMgr.ResetJieRiXiaoFeiKingActivity();
                            HuodongCachingMgr.ResetJieRiCZKingActivity();
                            ReloadXmlManager.ReloadXmlFile_config_JieRiQiangGou();
                        }
                        else if ("hefutime" == paramName)
                        {
                            /*HuodongCachingMgr.ResetHeFuActivityConfig();
                            HuodongCachingMgr.ResetHeFuLoginActivity();
                            HuodongCachingMgr.ResetHeFuTotalLoginActivity();
                            HuodongCachingMgr.ResetHeFuRechargeActivity();
                            HuodongCachingMgr.ResetHeFuPKKingActivity();
                            HuodongCachingMgr.ResetHeFuAwardTimeActivity();*/
                            ReloadXmlManager.ReloadXmlFile_config_HeFuQiangGou();

                            //HuodongCachingMgr.ResetHeFuVIPActivity();
                            //HuodongCachingMgr.ResetHeFuWCKingActivity();

                            //有人会不知道或忘记在这里重新加载配置和清空活动缓存，所以统一重新加载所有配置文件。如果ReloadAllXmlFile都不行，那重启服务器去吧
                            ReloadXmlManager.ReloadAllXmlFile();
                        }
                        else if ("whiteiplist" == paramName)
                        {
                            // 重新加载程序配置参数文件
                            Program.LoadIPList(paramValue);
                        }
                    }
                }
            }
            else if ("-listconfig" == cmdFields[0]) //列出所有的游戏配置参数
            {
                if (!transmit) //非转发的命令
                {
                    LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求列举游戏参数"));

                    //将所有的游戏参数消息枚举给指定的GM客户端
                    GameManager.GameConfigMgr.SendAllGameConfigItemsToGM(client);
                }
            }
            else if ("-holdqinggongyan" == cmdFields[0])
            {
                if (cmdFields.Length == 2)
                {
                    GameManager.QingGongYanMgr.HoldQingGongYan(client, Convert.ToInt32(cmdFields[1]));
                }
            }
            else if ("-joinqinggongyan" == cmdFields[0])
            {
                GameManager.QingGongYanMgr.JoinQingGongYan(client);
            }
            else if ("-setjmrate" == cmdFields[0]) //给角色设置冲穴成功率乘以的倍数, 方便测试操作
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -setjmrate 角色名称 冲穴成功率倍数");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int jmRate = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置冲穴的成功率倍数{1}", otherRoleName, jmRate));

                        //修改内存中的临时冲穴成功率倍数
                        otherClient.ClientData.TempJMChongXueRate = jmRate;

                        strinfo = string.Format("为{0}设置冲穴成功率倍数{1}", otherRoleName, jmRate);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-sethrate1" == cmdFields[0]) //给角色设置坐骑强化成功率乘以的倍数, 方便测试操作
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -sethrate1 角色名称 坐骑强化成功率倍数");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int rate = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置坐骑强化的成功率倍数{1}", otherRoleName, rate));

                        //修改内存中的临时冲穴成功率倍数
                        otherClient.ClientData.TempHorseEnchanceRate = rate;

                        strinfo = string.Format("为{0}设置坐骑强化成功率倍数{1}", otherRoleName, rate);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-sethrate2" == cmdFields[0]) //给角色设置坐骑进阶成功率乘以的倍数, 方便测试操作
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -sethrate1 角色名称 坐骑进阶成功率倍数");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int rate = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置坐骑进阶的成功率倍数{1}", otherRoleName, rate));

                        //修改内存中的临时冲穴成功率倍数
                        otherClient.ClientData.TempHorseEnchanceRate = rate;

                        strinfo = string.Format("为{0}设置坐骑进阶成功率倍数{1}", otherRoleName, rate);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-setjmlev" == cmdFields[0]) //给角色指定的经脉直接设置成功的穴位
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 4)
                    {
                        strinfo = string.Format("请输入： -setjmlev 角色名称 经脉ID 穴位ID");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int jingMaiID = SafeConvertToInt32(cmdFields[2]);
                        int jingMaiLevel = SafeConvertToInt32(cmdFields[3]);

                        if (jingMaiID < (int)JingMaiTypes.Yangwei || jingMaiID >= (int)JingMaiTypes.Max)
                        {
                            strinfo = string.Format("经脉的ID{0}超过了范围限制, 应该是:{1}-{2}", jingMaiID, (int)JingMaiTypes.Yangwei, (int)JingMaiTypes.Max - 1);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        jingMaiLevel = Global.GMax(0, jingMaiLevel);
                        jingMaiLevel = Global.GMin(Global.MaxJingMaiLevel, jingMaiLevel);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置经脉{1}的穴位为{2}", otherRoleName, Global.GetJingMaiName(jingMaiID), jingMaiLevel));

                        //设置经脉的穴位
                        {
                            //将经脉的列表属性加入Buffer中
                            Global.UpdateJingMaiListProps(otherClient, false); //先减去

                            int jingMaiBodyLevel = otherClient.ClientData.JingMaiBodyLevel;

                            //处理升级经脉的操作                
                            //处理角色经脉升级操作
                            int ret = Global.ProcessUpJingmaiLevel(otherClient, jingMaiBodyLevel, jingMaiID, ref jingMaiLevel, 0);

                            //将经脉的列表属性加入Buffer中
                            Global.UpdateJingMaiListProps(otherClient, true); //再加上
                            if (ret > 0)
                            {
                                //通知客户端属性变化
                                GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);

                                // 总生命值和魔法值变化通知(同一个地图才需要通知)
                                GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);

                                //通知角色经脉综合信息的指令信息
                                GameManager.ClientMgr.NotifyJingMaiInfoCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
                            }

                            //通知客户端冲脉的结果
                            GameManager.ClientMgr.NotifyJingMaiResult(otherClient, ret, jingMaiID, jingMaiLevel);
                        }

                        strinfo = string.Format("为{0}设置经脉{1}的穴位为{2}", otherRoleName, Global.GetJingMaiName(jingMaiID), jingMaiLevel);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-setskilllev" == cmdFields[0]) //给角色已经学习的技能直接设置级别
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 4)
                    {
                        strinfo = string.Format("请输入： -setskilllev 角色名称 技能ID 级别");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int skillID = SafeConvertToInt32(cmdFields[2]);
                        int skillLevel = SafeConvertToInt32(cmdFields[3]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        SystemXmlItem systemMagicItem = null;
                        if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillID, out systemMagicItem))
                        {
                            strinfo = string.Format("技能ID{0}在系统中不存在", skillID);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        skillLevel = Global.GMax(0, skillLevel);
                        skillLevel = Global.GMin(systemMagicItem.GetIntValue("MaxLevel"), skillLevel);

                        string skillName = systemMagicItem.GetStringValue("Name");
                        SkillData skillData = Global.GetSkillDataByID(otherClient, skillID);
                        if (null == skillData)
                        {
                            strinfo = string.Format("{0}尚未学习{1}技能", otherRoleName, skillName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置技能{1}的级别为{2}", otherRoleName, skillName, skillLevel));

                        //设置技能的级别
                        {
                            //升级技能
                            skillData.SkillLevel = skillLevel;

                            //更新技能信息
                            GameManager.ClientMgr.UpdateSkillInfo(client, skillData, true);

                            // 加载永久属性
                            if (systemMagicItem.GetIntValue("MagicType") < 0) //如果是被动的技能进行了升级
                            {
                                Global.RefreshSkillForeverProps(otherClient);

                                //通知客户端属性变化
                                GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);

                                // 总生命值和魔法值变化通知(同一个地图才需要通知)
                                GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
                            }

                            string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", 0, otherClient.ClientData.RoleID, skillData.DbID, skillData.SkillLevel, skillData.UsedNum);
                            TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, (int)TCPGameServerCmds.CMD_SPR_UPSKILLLEVEL);
                            Global._TCPManager.MySocketListener.SendData(otherClient.ClientSocket, tcpOutPacket);
                        }

                        strinfo = string.Format("为{0}设置技能{1}的级别为{2}", otherRoleName, skillName, skillLevel);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-setskillum" == cmdFields[0]) //-setskillum 给角色已经学习的技能直接设置熟练度
            {
                // 注释掉 取消技能熟练度 [11/13/2013 LiaoWei]
                /*if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 4)
                    {
                        strinfo = string.Format("请输入： -setskillum 角色名称 技能ID 熟练度值");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int skillID = SafeConvertToInt32(cmdFields[2]);
                        int skillUsedNum = SafeConvertToInt32(cmdFields[3]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        SystemXmlItem systemMagicItem = null;
                        if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillID, out systemMagicItem))
                        {
                            strinfo = string.Format("技能ID{0}在系统中不存在", skillID);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        string skillName = systemMagicItem.GetStringValue("Name");
                        SkillData skillData = Global.GetSkillDataByID(otherClient, skillID);
                        if (null == skillData)
                        {
                            strinfo = string.Format("{0}尚未学习{1}技能", otherRoleName, skillName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        //获取升级技能所需要的熟练度
                        int needRoleLevel = 1;
                        int needSkilledDegrees = 0;
                        if (!Global.GetUpSkillLearnCondition(skillData.SkillID, skillData, out needRoleLevel, out needSkilledDegrees, null))
                        {
                            strinfo = string.Format("{0}获取{1}技能的信息失败", otherRoleName, skillName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);
                            return true;
                        }

                        skillUsedNum = Global.GMin(needSkilledDegrees, skillUsedNum);
                        skillUsedNum = Global.GMax(0, skillUsedNum);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置技能{1}的熟练度为{2}", otherRoleName, skillName, skillUsedNum));

                        //设置技能的熟练度
                        skillData.UsedNum = skillUsedNum;

                        //更新技能信息
                        GameManager.ClientMgr.UpdateSkillInfo(otherClient, skillData, true);

                        strinfo = string.Format("为{0}设置技能{1}的熟练度为{2}", otherRoleName, skillName, skillUsedNum);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }*/
            }
            else if ("-del" == cmdFields[0]) //删除某个角色, 请输入：-del 角色名称
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -del 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求从数据库删除:{0}", otherRoleName));

                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_DELROLENAME,
                            string.Format("{0}", otherRoleName),
                            null);

                        strinfo = string.Format("从数据库删除{0}", otherRoleName);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-undel" == cmdFields[0]) //恢复某个删除的角色, 请输入: -undel 角色名称
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -undel 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为恢复删除:{0}", otherRoleName));

                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_UNDELROLENAME,
                            string.Format("{0}", otherRoleName),
                            null);

                        strinfo = string.Format("为{0}恢复删除", otherRoleName);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-modlimit" == cmdFields[0]) //修改服务器在线人数的限制, 请输入: -modlimit 最大在线人数
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -modlimit 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        int maxLimit = SafeConvertToInt32(cmdFields[1]);
                        Global._TCPManager.MaxConnectedClientLimit = maxLimit;

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为修改最大在线人数{0}", maxLimit));

                        strinfo = string.Format("修改最大在线人数限制为{0}", maxLimit);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
                else
                {
                    if (cmdFields.Length >= 2)
                    {
                        int maxLimit = SafeConvertToInt32(cmdFields[1]);
                        Global._TCPManager.MaxConnectedClientLimit = maxLimit;

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为修改最大在线人数{0}", maxLimit));
                    }
                }
            }
            else if ("-listhorse" == cmdFields[0]) //列举某个角色的坐骑的列表(返回结果: ID 名称 幸运值/成功值)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -listhorse 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求列举{0}的坐骑ID列表", otherRoleName));

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        if (null == otherClient.ClientData.HorsesDataList)
                        {
                            strinfo = string.Format("{0}的坐骑ID列表为空", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);
                        }

                        List<string> msgList = new List<string>();
                        lock (otherClient.ClientData.HorsesDataList)
                        {
                            for (int i = 0; i < otherClient.ClientData.HorsesDataList.Count; i++)
                            {
                                //strinfo = string.Format("{0} {1} {2}/{3}", otherClient.ClientData.HorsesDataList[i].DbID, Global.GetHorseNameByID(otherClient.ClientData.HorsesDataList[i].HorseID),
                                //    Global.GetHorseFailedNum(otherClient.ClientData.HorsesDataList[i]),
                                //    Global.GetHorseHorseTwoValue(otherClient.ClientData.HorsesDataList[i].HorseID + 1),
                                //    Global.GetHorseHorseOneValue(otherClient.ClientData.HorsesDataList[i].HorseID + 1));
                                strinfo = string.Format("{0} {1} {2}/{3}/{4}", otherClient.ClientData.HorsesDataList[i].DbID, Global.GetHorseNameByID(otherClient.ClientData.HorsesDataList[i].HorseID),
                                    Global.GetHorseFailedNum(otherClient.ClientData.HorsesDataList[i]),
                                    Global.GetHorseHorseBlessPoint(otherClient.ClientData.HorsesDataList[i].HorseID + 1));
                                GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                    client, strinfo);
                            }
                        }
                    }
                }
            }
            else if ("-sethl" == cmdFields[0]) //设置角色的某个坐骑的进阶幸运值, 请输入：-sethl 角色名称 坐骑ID(-listhorse得到的ID) 幸运值
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 4)
                    {
                        strinfo = string.Format("请输入：-sethl 角色名称 坐骑ID(-listhorse得到的ID) 幸运值");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int horseDbID = Global.SafeConvertToInt32(cmdFields[2]);
                        int luckyNum = Global.SafeConvertToInt32(cmdFields[3]);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置坐骑ID{1}的进阶幸运值", otherRoleName, horseDbID));

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        HorseData horseData = Global.GetHorseDataByDbID(otherClient, horseDbID);
                        if (null == horseData)
                        {
                            strinfo = string.Format("在{0}的坐骑列表中没有找到ID为{1}的坐骑", otherRoleName, horseDbID);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);
                        }

                        //记录失败次数
                        Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, horseData.DbID, horseData.HorseID, luckyNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID);

                        strinfo = string.Format("为{0}的坐骑ID为{1}的设置幸运值{2}", otherRoleName, horseDbID, luckyNum);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-setwlogin" == cmdFields[0]) //设置角色的周连续登录天数, 请输入：-setwlogin 角色名称 天数
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -setwlogin 角色名称 天数");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int dayNum = Global.SafeConvertToInt32(cmdFields[2]);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置本周的连续登录天数{1}", otherRoleName, dayNum));

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        otherClient.ClientData.MyHuodongData.LoginNum = dayNum;

                        strinfo = string.Format("为{0}设置本周的连续登录天数{1}", otherRoleName, dayNum);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-setmtime" == cmdFields[0]) //设置角色的本月的在线时长(秒数), 请输入：-setmtime 角色名称 在线时长(秒数)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -setmtime 角色名称 在线时长(秒数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int onlineSecs = Global.SafeConvertToInt32(cmdFields[2]);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置本月的在线时长{1}", otherRoleName, onlineSecs));

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        otherClient.ClientData.MyHuodongData.CurMTime = onlineSecs;

                        strinfo = string.Format("为{0}设置本周的连续登录天数{1}", otherRoleName, onlineSecs);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-setnstep" == cmdFields[0]) //设置角色的新手见面礼包的当前步骤, 请输入：-setnstep 角色名称 当前步骤(1~10)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -setnstep 角色名称 当前步骤(1~5)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int newStep = Global.SafeConvertToInt32(cmdFields[2]);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置新手见面礼物领取的步骤{1}", otherRoleName, newStep));

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        long nowTicks = DateTime.Now.Ticks / 10000;

                        //设置领取标志
                        otherClient.ClientData.MyHuodongData.NewStep += 1;
                        otherClient.ClientData.MyHuodongData.StepTime = nowTicks;

                        //发送活动数据给客户端
                        GameManager.ClientMgr.NotifyHuodongData(otherClient);

                        strinfo = string.Format("为{0}设置新手见面礼物领取的步骤{1}", otherRoleName, newStep);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-updateBindgold" == cmdFields[0]) //通知在线的账户从数据中中重新获取元宝数量(内部命令)
            {
                if (!transmit) //非转发的命令
                {
                    ;//
                }
                else
                {
                    if (cmdFields.Length == 5)
                    {
                        string userID = cmdFields[1];
                        int rid = Global.SafeConvertToInt32(cmdFields[2]);
                        int bindGold = Global.SafeConvertToInt32(cmdFields[3]);//送的绑钻数
                        string chargeInfo = cmdFields[4];//首充存盘数据
                        TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
                        GameClient otherClient = null;
                        if (null != clientSocket)
                        {
                            otherClient = GameManager.ClientMgr.FindClient(clientSocket);

                        }
                        Global.ProcessSendBindGold(otherClient, bindGold, userID, rid, chargeInfo);
                    }
                }
            }
            else if ("-updateyb" == cmdFields[0]) //通知在线的账户从数据中中重新获取元宝数量(内部命令)
            {
                if (!transmit) //非转发的命令
                {
                    ;//
                }
                else
                {
                    if (cmdFields.Length == 5)
                    {
                        string userID = cmdFields[1];
                        int rid = Global.SafeConvertToInt32(cmdFields[2]);
                        int addMoney = Global.SafeConvertToInt32(cmdFields[3]);//用户充值钱数
                        string oldchargeInfo = cmdFields[4];//首充存盘数据
                        //根据ID查找敌人
                        TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
                        if (null == clientSocket)
                        {
                            return true;
                        }
                        LogManager.WriteLog(LogTypes.SQL, string.Format("通知账户ID{0}的角色更新元宝数量", userID));

                        GameClient otherClient = GameManager.ClientMgr.FindClient(clientSocket);

                        if (null != otherClient)
                        {

                            //添加0个用户元宝，强迫角色更新元宝数量
                            GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, 0, "GM命令强迫更新");

                            // 充值改变时，刷新与充值相关图标状态
                            otherClient._IconStateMgr.FlushChongZhiIconState(otherClient);
                            otherClient._IconStateMgr.CheckJieRiActivity(otherClient, false);
                            otherClient._IconStateMgr.SendIconStateToClient(otherClient);
                        }
                    }
                }
            }
            else if ("-setfbnum" == cmdFields[0]) //设置角色的某个副本的当日次数(可以是负数), 请输入：-setfbnum  角色名称 副本ID 当日次数(可以是负数)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 4)
                    {
                        strinfo = string.Format("请输入： -setfbnum 角色名称 副本ID 当日次数(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int fuBenID = Global.SafeConvertToInt32(cmdFields[2]);
                        int addDayNum = Global.SafeConvertToInt32(cmdFields[3]);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}增加副本ID{1}的当日次数{2}", otherRoleName, fuBenID, addDayNum));

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        //更新副本的数据
                        Global.UpdateFuBenData(otherClient, fuBenID, addDayNum, addDayNum);

                        strinfo = string.Format("为{0}增加副本ID{1}的当日次数{2}", otherRoleName, fuBenID, addDayNum);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-sethdnum" == cmdFields[0]) //设置角色的某个副本的当日次数(可以是负数), 请输入：-setfbnum  角色名称 副本ID 当日次数(可以是负数)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 4)
                    {
                        strinfo = string.Format("请输入： -sethdnum 角色名称 活动代号ID 当日次数(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int huodongID = Global.SafeConvertToInt32(cmdFields[2]);
                        int addDayNum = Global.SafeConvertToInt32(cmdFields[3]);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}增加副本ID{1}的当日次数{2}", otherRoleName, huodongID, addDayNum));

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        //更新活动的数据
                        Global.UpdateDayActivityEnterCountToDB(otherClient, otherClient.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, huodongID, addDayNum); //sethdnum

                        strinfo = string.Format("为{0}增加活动{1}的当日次数{2}", otherRoleName, ((SpecialActivityTypes)huodongID).ToString(), addDayNum);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-setfreshplayer" == cmdFields[0]) //设置角色的某个副本的当日次数(可以是负数), 请输入：-setfbnum  角色名称 副本ID 当日次数(可以是负数)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -setfreshplayer 1(启用)或0(禁用)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        int enabelFlag = Global.SafeConvertToInt32(cmdFields[1]);

                        Global.Flag_EnabelNewPlayerScene = (enabelFlag > 0);
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求{0}新手场景", Global.Flag_EnabelNewPlayerScene ? "启用" : "禁用"));

                        strinfo = string.Format("{0}新手场景", Global.Flag_EnabelNewPlayerScene ? "启用" : "禁用");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-addattack" == cmdFields[0]) //-addattack 临时增加角色的物攻和魔攻, 请输入：-addattack 角色名称 攻击力(值)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入：-addattack 角色名称 攻击力(值)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int addAttack = Global.SafeConvertToInt32(cmdFields[2]);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}临时增加物攻和魔攻{1}", otherRoleName, addAttack));

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        //临时增加物攻和魔攻
                        otherClient.RoleBuffer.AddForeverExtProp((int)ExtPropIndexes.MinAttack, addAttack);
                        otherClient.RoleBuffer.AddForeverExtProp((int)ExtPropIndexes.MaxAttack, addAttack);
                        otherClient.RoleBuffer.AddForeverExtProp((int)ExtPropIndexes.MinMAttack, addAttack);
                        otherClient.RoleBuffer.AddForeverExtProp((int)ExtPropIndexes.MaxMAttack, addAttack);

                        // 属性改造[8/15/2013 LiaoWei]
                        //otherClient.RoleBuffer.AddForeverExtProp((int)ExtPropIndexes.MinDSAttack, addAttack);
                        //otherClient.RoleBuffer.AddForeverExtProp((int)ExtPropIndexes.MaxDSAttack, addAttack);

                        //通知客户端属性变化
                        GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);

                        // 总生命值和魔法值变化通知(同一个地图才需要通知)
                        GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);

                        strinfo = string.Format("为{0}临时增加物攻和魔攻{1}", otherRoleName, addAttack);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-adddefense" == cmdFields[0]) //-adddefense 临时增加角色的物防和魔防, 请输入：-adddefense 角色名称 防御力(值)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入：-adddefense 角色名称 防御力(值)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int addDefense = Global.SafeConvertToInt32(cmdFields[2]);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}临时增加物防和魔防{1}", otherRoleName, addDefense));

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        //临时增加物攻和魔攻
                        otherClient.RoleBuffer.AddForeverExtProp((int)ExtPropIndexes.MinDefense, addDefense);
                        otherClient.RoleBuffer.AddForeverExtProp((int)ExtPropIndexes.MaxDefense, addDefense);
                        otherClient.RoleBuffer.AddForeverExtProp((int)ExtPropIndexes.MinMDefense, addDefense);
                        otherClient.RoleBuffer.AddForeverExtProp((int)ExtPropIndexes.MaxMDefense, addDefense);

                        //通知客户端属性变化
                        GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);

                        // 总生命值和魔法值变化通知(同一个地图才需要通知)
                        GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);

                        strinfo = string.Format("为{0}临时增加物防和魔防{1}", otherRoleName, addDefense);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-setybstate" == cmdFields[0]) //给角色的押镖设置状态 请输入: -setybstate 角色名称 状态(0：成功，1：失败)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入：-setybstate 角色名称 状态(0：成功，1：失败)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int state = Global.SafeConvertToInt32(cmdFields[2]);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}修改押镖的状态{1}", otherRoleName, state));

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        //修改押镖数据
                        if (null != otherClient.ClientData.MyYaBiaoData)
                        {
                            otherClient.ClientData.MyYaBiaoData.State = state;

                            //更新上线状态
                            GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_UPDATEYABIAODATA,
                                string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}",
                                otherClient.ClientData.RoleID,
                                otherClient.ClientData.MyYaBiaoData.YaBiaoID,
                                otherClient.ClientData.MyYaBiaoData.StartTime,
                                otherClient.ClientData.MyYaBiaoData.State,
                                otherClient.ClientData.MyYaBiaoData.LineID,
                                otherClient.ClientData.MyYaBiaoData.TouBao,
                                otherClient.ClientData.MyYaBiaoData.YaBiaoDayID,
                                otherClient.ClientData.MyYaBiaoData.YaBiaoNum,
                                otherClient.ClientData.MyYaBiaoData.TakeGoods
                                ),
                                null);

                            //将新的押镖数据通知客户端
                            GameManager.ClientMgr.NotifyYaBiaoData(otherClient);

                            strinfo = string.Format("为{0}修改押镖的状态{1}", otherRoleName, state);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);
                        }
                        else
                        {
                            strinfo = string.Format("{0}当前无运镖数据", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);
                        }
                    }
                }
            }
            else if ("-setybstate2" == cmdFields[0]) //给角色的押镖设置状态 请输入: -setybstate2 角色ID 状态(0:成功，1:失败)
            {
                if (transmit) //转发的命令
                {
                    if (cmdFields.Length >= 3)
                    {
                        int roleID = Global.SafeConvertToInt32(cmdFields[1]);
                        int state = Global.SafeConvertToInt32(cmdFields[2]);

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            return true;
                        }

                        //修改押镖数据
                        if (null != otherClient.ClientData.MyYaBiaoData)
                        {
                            otherClient.ClientData.MyYaBiaoData.State = state;

                            //更新上线状态
                            GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_UPDATEYABIAODATA,
                                string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}",
                                otherClient.ClientData.RoleID,
                                otherClient.ClientData.MyYaBiaoData.YaBiaoID,
                                otherClient.ClientData.MyYaBiaoData.StartTime,
                                otherClient.ClientData.MyYaBiaoData.State,
                                otherClient.ClientData.MyYaBiaoData.LineID,
                                otherClient.ClientData.MyYaBiaoData.TouBao,
                                otherClient.ClientData.MyYaBiaoData.YaBiaoDayID,
                                otherClient.ClientData.MyYaBiaoData.YaBiaoNum,
                                otherClient.ClientData.MyYaBiaoData.TakeGoods
                                ),
                                null);

                            //将新的押镖数据通知客户端
                            GameManager.ClientMgr.NotifyYaBiaoData(otherClient);
                        }
                    }
                }
            }
            else if ("-reload" == cmdFields[0]) //动态加载参数文件 请输入: -reload xml文件名称,不区分大小写(例如：config/mall.xml)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入：-reload xml文件名称,不区分大小写(例如：config/mall.xml)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string xmlFileName = cmdFields[1];

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求重新加载{0}", xmlFileName));

                        //重新加载程序配置参数文件
                        int retCode = ReloadXmlManager.ReloadXmlFile(xmlFileName);
                        strinfo = string.Format("重新加载参数{0}, 结果：{1}", xmlFileName, retCode);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
                else
                {
                    if (cmdFields.Length >= 2)
                    {
                        string xmlFileName = cmdFields[1];

                        //重新加载程序配置参数文件
                        int retCode = ReloadXmlManager.ReloadXmlFile(xmlFileName);
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据转发的GM的要求重新加载{0}, 结果为:{1}", xmlFileName, retCode));
                    }
                }
            }
            else if ("-loadiplist" == cmdFields[0]) //动态加载IP白名单文件,并设置启用状态
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入：-loadiplist (0|1)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string openState = cmdFields[1];
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求重新加载IP白名单列表,设置启用状态: {0}", openState));

                        //重新加载程序配置参数文件
                        Program.LoadIPList(openState);

                        strinfo = string.Format("根据GM的要求重新加载IP白名单列表,设置启用状态: {0}", openState);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
                else
                {
                    if (cmdFields.Length >= 2)
                    {
                        string openState = cmdFields[1];
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求重新加载IP白名单列表,设置启用状态: {0}", openState));

                        //重新加载程序配置参数文件
                        Program.LoadIPList(openState);
                    }
                }
            }
            else if ("-reloadall" == cmdFields[0]) //动态加载所有参数文件 请输入: -reloadall
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 1)
                    {
                        strinfo = string.Format("请输入：-reloadall");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求重新加载所有配置文件"));

                        //重新加载程序配置参数文件
                        ReloadXmlManager.ReloadAllXmlFile();
                        strinfo = string.Format("重新加载所有参数配置文件");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
                else
                {
                    if (cmdFields.Length >= 1)
                    {
                        //重新加载程序配置参数文件
                        ReloadXmlManager.ReloadAllXmlFile();
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据转发的GM的要求重新加载所有参数配置文件"));
                    }
                }
            }
            else if ("-reload2" == cmdFields[0]) //(所有线)动态加载参数文件 请输入: -reload2 xml文件名称,不区分大小写(例如：config/mall.xml)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入：-reload2 xml文件名称,不区分大小写(例如：config/mall.xml)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string xmlFileName = cmdFields[1];

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求通知所有线重新加载{0}", xmlFileName));

                        //重新加载程序配置参数文件
                        int retCode = ReloadXmlManager.ReloadXmlFile(xmlFileName);
                        strinfo = string.Format("所有线重新加载参数{0}, 结果：{1}", xmlFileName, retCode);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);

                        //如果成功，则通知线所有服务器
                        if (0 == retCode)
                        {
                            string gmCmdData = string.Format("-reload {0}", cmdFields[1]);

                            //转发GM消息到DBServer
                            GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_SPR_CHAT,
                                string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", client.ClientData.RoleID, "", 0, "", 0, gmCmdData, 0, 0, GameManager.ServerLineID),
                                null);
                        }
                    }
                }
            }
            else if ("-reloadgm" == cmdFields[0]) //-reloadgm 动态加载GM配置参数(超级GM用户才有权限) 请输入: -reloadgm
            {
                if (!transmit) //非转发的命令
                {
                    if (isSuperGMUser)
                    {
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据超级GM的要求重新加载GM参数"));

                        int retCode = ReloadGMCommands();
                        strinfo = string.Format("重新加载GM参数, 结果：{0}", retCode);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
                else
                {
                    //if (isSuperGMUser)
                    {
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据超级GM的要求重新加载GM参数"));

                        int retCode = ReloadGMCommands();
                    }
                }
            }
            else if ("-reloadph" == cmdFields[0]) //-reloadph 强制重新加载排行榜
            {
                if (!transmit) //非转发的命令
                {
                    LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求重新加载排行榜"));

                    GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_FORCERELOADPAIHANG,
                        string.Format("{0}", client.ClientData.RoleID),
                        null);

                    strinfo = string.Format("重新加载排行榜成功");
                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                        client, strinfo);
                }
                else
                {
                    LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求重新加载排行榜"));

                    GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_FORCERELOADPAIHANG,
                        string.Format("{0}", 0),
                        null);
                }
            }
            else if ("-clrrolecache" == cmdFields[0]) //-clrrolecache 清空某个角色的缓存数据，强迫重新加载(角色名不带区号)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入：-clrrolecache 角色名称(带区号)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求清空{0}角色的数据库缓存", otherRoleName));

                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_CLEARCACHINGROLEIDATA,
                            string.Format("{0}:{1}", client.ClientData.RoleID, otherRoleName),
                            null);

                        strinfo = string.Format("清空{0}角色的数据库缓存成功", otherRoleName);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
                else
                {
                    if (cmdFields.Length >= 2)
                    {
                        string otherRoleName = cmdFields[1];
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求清空{0}角色的数据库缓存", otherRoleName));

                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_CLEARCACHINGROLEIDATA,
                            string.Format("{0}:{1}", 0, otherRoleName),
                            null);
                    }
                }
            }
            else if ("-clrallrolecache" == cmdFields[0]) //-clrrolecache 清空某个角色的缓存数据，强迫重新加载(角色名不带区号)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 1)
                    {
                        strinfo = string.Format("请输入：-clrallrolecache");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求清空所有角色的数据库缓存"));

                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_CLEARALLCACHINGROLEDATA,
                            string.Format("{0}", client.ClientData.RoleID),
                            null);

                        strinfo = string.Format("清空所有角色的数据库缓存成功");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
                else
                {
                    if (cmdFields.Length >= 1)
                    {
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求清空所有角色的数据库缓存"));

                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_CLEARALLCACHINGROLEDATA,
                            string.Format("{0}", 0),
                            null);
                    }
                }
            }
            else if ("-addheroidx" == cmdFields[0]) //给角色的英雄逐擂设置到达的层数 请输入: -addheroidx 角色名称 层数(大于等于0，小于等于13)
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入：-addheroidx 角色名称 层数(大于等于0，小于等于13)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int heroIndex = Global.SafeConvertToInt32(cmdFields[2]);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}修改英雄逐擂的到达层数{1}", otherRoleName, heroIndex));

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        //通知英雄逐擂到达层数更新(限制当前地图)
                        GameManager.ClientMgr.ChangeRoleHeroIndex(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            otherClient, heroIndex, true);

                        strinfo = string.Format("为{0}修改英雄逐擂的到达层数{1}", otherRoleName, heroIndex);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-setlz" == cmdFields[0]) //给角色设置连斩个数 请输入: -setlz 角色名称 连斩数
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入：-setlz  角色名称 连斩数");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int lianZhanNum = Global.SafeConvertToInt32(cmdFields[2]);
                        lianZhanNum = Global.GMin(899, lianZhanNum);

                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}修改连斩数{1}", otherRoleName, lianZhanNum));

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        //通知连斩值更新(限制当前地图)
                        GameManager.ClientMgr.ChangeRoleLianZhan2(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            otherClient, lianZhanNum, true);

                        strinfo = string.Format("为{0}修改连斩数{1}", otherRoleName, lianZhanNum);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-applytobh" == cmdFields[0]) //通知帮会加入申请的GM命令 请输入: -applytobh 申请的角色ID 帮会ID 帮会管理成员ID(,分割)
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 6)
                    {
                        int roleID = Global.SafeConvertToInt32(cmdFields[1]);
                        string roleName = cmdFields[2];
                        int bhid = Global.SafeConvertToInt32(cmdFields[3]);
                        string bhName = cmdFields[4];
                        string roleList = cmdFields[5];

                        //通知所有在线的帮会管理用户，某个用户申请了加入帮派
                        GameManager.ClientMgr.NotifyOnlineBangHuiMgrRoleApplyMsg(roleID, roleName, bhid, bhName, roleList);
                    }
                }
            }
            else if ("-joinbh" == cmdFields[0]) //通知某个角色加入了某个帮会GM命令 请输入: -joinbh 申请的角色ID 帮会ID 帮会名称
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 4)
                    {
                        int otherRoleID = Global.SafeConvertToInt32(cmdFields[1]);
                        int bhid = Global.SafeConvertToInt32(cmdFields[2]);
                        string bhName = cmdFields[3];

                        GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
                        if (null != otherClient)
                        {
                            //通知某个角色被加入了某个帮派
                            GameManager.ClientMgr.NotifyJoinBangHui(Global._TCPManager.MySocketListener,
                                Global._TCPManager.TcpOutPacketPool, otherClient, bhid, bhName);
                        }
                    }
                }
            }
            else if ("-leavebh" == cmdFields[0]) //通知某个角色离开了某个帮会GM命令 请输入: -leavebh 角色ID 帮会ID 帮会名称 离开类型
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 5)
                    {
                        int otherRoleID = Global.SafeConvertToInt32(cmdFields[1]);
                        int bhid = Global.SafeConvertToInt32(cmdFields[2]);
                        string bhName = cmdFields[3];
                        int leaveType = Global.SafeConvertToInt32(cmdFields[4]);

                        GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
                        if (null != otherClient)
                        {
                            //通知某个角色离开了某个帮派
                            GameManager.ClientMgr.NotifyLeaveBangHui(Global._TCPManager.MySocketListener,
                                Global._TCPManager.TcpOutPacketPool, otherClient, bhid, bhName, leaveType);
                        }
                    }
                }
            }
            else if ("-destroybh" == cmdFields[0]) //通知解散了某个帮会GM命令 请输入: -destroybh 返回值 角色ID 帮会ID
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 4)
                    {
                        int retCode = Global.SafeConvertToInt32(cmdFields[1]);
                        int roleID = Global.SafeConvertToInt32(cmdFields[2]);
                        int bhid = Global.SafeConvertToInt32(cmdFields[3]);

                        //通知所有指定帮会的在线用户帮会已经解散
                        GameManager.ClientMgr.NotifyBangHuiDestroy(retCode, roleID, bhid);
                    }
                }
            }
            else if ("-autodestroybh" == cmdFields[0]) //通知解散了某个帮会GM命令 请输入: -autodestroybh 返回值 角色ID 帮会ID
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 1)
                    {
                        int bhid = Global.SafeConvertToInt32(cmdFields[0]);

                        //通知所有指定帮会的在线用户帮会已经解散
                        GameManager.ClientMgr.NotifyBangHuiDestroy(0, 0, bhid);

                        //帮会解散时发出取消领地所属的帮旗的操作
                        JunQiManager.SendClearJunQiCmd(bhid);

                        //通知GameServer同步帮旗的级别和名称
                        JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);

                        //通知GameServer同步领地帮会分布
                        JunQiManager.NotifySyncBangHuiLingDiItemsDict();

                        //如果自己是皇帝
                        //if (HuangChengManager.GetHuangDiRoleID() == client.ClientData.RoleID)
                        //{
                        //    //处理拥有皇帝特效的角色死亡，而失去皇帝特效的事件
                        //    HuangChengManager.HandleDeadHuangDiRoleChanging(null);
                        //}

                        //清除缓存项
                        Global.RemoveBangHuiMiniData(bhid);
                    }
                }
            }
            else if ("-invitetobh" == cmdFields[0]) //通知解散了某个帮会GM命令 请输入: -invitetobh 角色ID 帮会ID 帮会名称
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 6)
                    {
                        int roleID = Global.SafeConvertToInt32(cmdFields[1]);
                        int inviteRoleID = Global.SafeConvertToInt32(cmdFields[2]);
                        string inviteRoleName = cmdFields[3];
                        int bhid = Global.SafeConvertToInt32(cmdFields[4]);
                        string bhName = cmdFields[5];

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null != otherClient)
                        {
                            //加入帮派邀请通知通知自己
                            GameManager.ClientMgr.NotifyInviteToBangHui(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                otherClient, inviteRoleID, inviteRoleName, bhid, bhName, otherClient.ClientData.ChangeLifeCount);
                        }
                    }
                }
            }
            else if ("-refusetobh" == cmdFields[0]) //通知决绝了加入帮会申请/邀请GM命令 请输入: -refusetobh 角色ID 拒绝的角色名称 帮会名称 拒绝类型
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 5)
                    {
                        int roleID = Global.SafeConvertToInt32(cmdFields[1]);
                        string refreseRoleName = cmdFields[2];
                        string bhName = cmdFields[3];
                        int refuseType = Global.SafeConvertToInt32(cmdFields[4]);

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null != otherClient)
                        {
                            if (0 == refuseType)
                            {
                                //拒绝申请加入帮派的操作
                                GameManager.ClientMgr.NotifyRefuseApplyToBHMember(otherClient, refreseRoleName, bhName);
                            }
                            else
                            {
                                //拒绝邀请加入帮派的操作
                                GameManager.ClientMgr.NotifyRefuseInviteToBHMember(otherClient, refreseRoleName, bhName);
                            }
                        }
                    }
                }
            }
            else if ("-syncjunqi" == cmdFields[0]) //同步帮旗的字典的内部GM命令 请输入: -syncjunqi
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 1)
                    {
                        //从DBServer加载帮旗字典数据
                        JunQiManager.LoadBangHuiJunQiItemsDictFromDBServer();
                    }
                }
            }
            else if ("-synclingdi" == cmdFields[0]) //同步领地帮会分布内部GM命令 请输入: -synclingdi
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 1)
                    {
                        //从DBServer加载领地帮会字典数据
                        JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer();
                    }
                }
            }
            else if ("-syncldzresult" == cmdFields[0]) //同步领地帮会分布内部GM命令 请输入: -syncldzresult
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 6)
                    {
                        int lingDiID = Global.SafeConvertToInt32(cmdFields[1]);
                        int mapCode = Global.SafeConvertToInt32(cmdFields[2]);
                        int bhid = Global.SafeConvertToInt32(cmdFields[3]);
                        int zoneID = Global.SafeConvertToInt32(cmdFields[4]);
                        string bhName = cmdFields[5];

                        //处理领地战的结果
                        JunQiManager.HandleLingDiZhanResultByMapCode2(lingDiID, mapCode, bhid, zoneID, bhName);
                    }
                }
            }
            else if ("-chbhzhiwu" == cmdFields[0]) //通知修改了帮会内某个成员你的职务的内部GM命令 请输入: -chbhzhiwu
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 5)
                    {
                        int bhid = Global.SafeConvertToInt32(cmdFields[1]);
                        int otherRoleID = Global.SafeConvertToInt32(cmdFields[2]);
                        int zhiWu = Global.SafeConvertToInt32(cmdFields[3]);
                        int oldZhiWuRoleID = Global.SafeConvertToInt32(cmdFields[4]);

                        GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
                        if (null != otherClient)
                        {
                            if (otherClient.ClientData.Faction == bhid)
                            {
                                otherClient.ClientData.BHZhiWu = zhiWu;
                                GameManager.ClientMgr.ChangeBangHuiZhiWu(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                    otherClient);
                            }
                        }

                        otherClient = GameManager.ClientMgr.FindClient(oldZhiWuRoleID);
                        if (null != otherClient)
                        {
                            if (otherClient.ClientData.Faction == bhid)
                            {
                                otherClient.ClientData.BHZhiWu = 0;
                                GameManager.ClientMgr.ChangeBangHuiZhiWu(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                    otherClient);
                            }
                        }
                    }
                }
            }
            else if ("-removehuangfei" == cmdFields[0]) //通知删除某个角色的皇后状态的内部GM命令 请输入: -removehuangfei
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 3)
                    {
                        int otherRoleID = Global.SafeConvertToInt32(cmdFields[1]);
                        string huangDiRoleName = cmdFields[2];

                        //处理领地战的结果
                        GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
                        if (null != otherClient)
                        {
                            //更新角色的皇后状态
                            Global.UpdateRoleHuangHou(otherClient, 0, huangDiRoleName);
                        }
                    }
                }
            }
            else if ("-leavelaofang" == cmdFields[0]) //通知将某个角色从牢房中放出的内部GM命令 请输入: -leavelaofang
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 3)
                    {
                        int otherRoleID = Global.SafeConvertToInt32(cmdFields[1]);
                        string huangDiRoleName = cmdFields[2];

                        //处理领地战的结果
                        GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
                        if (null != otherClient)
                        {
                            //从牢房中放出
                            Global.ForceTakeOutLaoFangMap(otherClient, otherClient.ClientData.PKPoint);

                            //被皇帝从牢房放出的提示
                            Global.BroadcastTakeOutLaoFangHint(huangDiRoleName, Global.FormatRoleName(otherClient, otherClient.ClientData.RoleName));
                        }
                    }
                }
            }
            else if ("-clearmap" == cmdFields[0]) //通知将某个地图的帮旗清空的内部GM命令 请输入: -clearmap
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 2)
                    {
                        int bhid = Global.SafeConvertToInt32(cmdFields[1]);

                        //处理删除全部帮旗
                        JunQiManager.ProcessDelAllJunQiByBHID(Global._TCPManager.MySocketListener,
                            Global._TCPManager.TcpOutPacketPool, bhid);
                    }
                }
            }
            else if ("-synchuangdi" == cmdFields[0]) //通知将同步皇帝信息的内部GM命令 请输入: -synchuangdi
            {
                if (transmit) //内部的命令
                {
                    if (cmdFields.Length >= 5)
                    {
                        int oldRoleID = Global.SafeConvertToInt32(cmdFields[1]);
                        int roleID = Global.SafeConvertToInt32(cmdFields[2]);
                        string roleName = cmdFields[3];
                        string bhName = cmdFields[4];

                        //处理提取舍利之源的操作
                        HuangChengManager.ProcessTakeSheLiZhiYuan(roleID, roleName, bhName, false);

                        //通知在线的所有人(不限制地图)皇帝角色ID变更消息
                        GameManager.ClientMgr.NotifyAllChgHuangDiRoleIDMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            oldRoleID, HuangChengManager.GetHuangDiRoleID());
                    }
                }
            }
            else if ("-reloadxml" == cmdFields[0]) //通知所有在线的角色重新刷新进入游戏 请输入: -reloadxml 文字信息
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入：-reloadxml  文字信息");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string notifyMsg = cmdFields[1];
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求发送重新加载xml的通知消息{0}", notifyMsg));

                        notifyMsg = notifyMsg.Replace(":", ""); //防止出现半角的冒号
                        GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null,
                            notifyMsg,
                            GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, (int)HintErrCodeTypes.HintReloadXML);

                        strinfo = string.Format("成功发送重新加载xml的通知信息：{0}成功", notifyMsg);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-notifymail" == cmdFields[0]) //通知在线的用户有新邮件
            {
                if (!transmit) //非转发的命令
                {
                    ;//
                }
                else
                {
                    if (cmdFields.Length == 2)
                    {
                        string[] userIDAndMailIDs = cmdFields[1].Split('_');

                        foreach (var item in userIDAndMailIDs)
                        {
                            string[] pair = item.Split('|');

                            int roleID = Global.SafeConvertToInt32(pair[0]);
                            int mailID = Global.SafeConvertToInt32(pair[1]);

                            //根据用户ID查找客户端连接
                            GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                            if (null != otherClient)
                            {
                                //提示用户新邮件信息
                                GameManager.ClientMgr.NotifyLastUserMail(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, mailID);
                            }
                        }
                    }
                }
            }
            else if ("-modifyastar" == cmdFields[0]) //动态修改寻路算法参数
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length != 2)
                    {
                        strinfo = string.Format("请输入：-modifyastar 大于等于8的整数");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        Global.ModifyMaxOpenNodeCountForAStar(Global.SafeConvertToInt32(cmdFields[1]));
                    }
                }
            }
            else if ("-modifylogtype" == cmdFields[0]) //动态修改日志记录级别
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length != 2)
                    {
                        strinfo = string.Format("请输入：-modifylogtype -1到3的整数");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        //即使小于-1 大于 3 也无所谓
                        LogManager.LogTypeToWrite = (LogTypes)Global.SafeConvertToInt32(cmdFields[1]);
                    }
                }
            }
            else if ("-reloadnpc" == cmdFields[0]) //重新加载某个地图NPC
            {
                //if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 2)
                    {
                        int mapCode = Global.SafeConvertToInt32(cmdFields[1]);

                        //先移除
                        NPCGeneralManager.RemoveMapNpcs(mapCode);

                        //再添加
                        NPCGeneralManager.ReloadMapNPCRoles(mapCode);
                    }
                    else
                    {
                        strinfo = string.Format("请输入：-reloadnpc 地图编号");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-modifyparams" == cmdFields[0]) //修改角色的，主要用于整数参数值
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 4)
                    {
                        string otherRoleName = cmdFields[1];

                        //根据ID查找玩家
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        int paramIndex = Global.SafeConvertToInt32(cmdFields[2]);
                        int modifyValue = Global.SafeConvertToInt32(cmdFields[3]);

                        switch (paramIndex)
                        {
                            case (int)RoleCommonUseIntParamsIndexs.ChengJiu:
                                GameManager.ClientMgr.ModifyChengJiuPointsValue(otherClient, modifyValue, "GM指令增加成就", true, true);
                                //ChengJiuManager.TryToActiveNewChengJiuBuffer(otherClient, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.ZhuangBeiJiFen:
                                GameManager.ClientMgr.ModifyZhuangBeiJiFenValue(otherClient, modifyValue, true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.LieShaZhi:
                                GameManager.ClientMgr.ModifyLieShaValue(otherClient, modifyValue, true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.WuXingZhi:
                                GameManager.ClientMgr.ModifyWuXingValue(otherClient, modifyValue, true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.ZhenQiZhi:
                                GameManager.ClientMgr.ModifyZhenQiValue(otherClient, modifyValue, true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.TianDiJingYuan:
                                GameManager.ClientMgr.ModifyTianDiJingYuanValue(otherClient, modifyValue, "GM指令增加魔晶", true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.ShiLianLing:
                                GameManager.ClientMgr.ModifyShiLianLingValue(otherClient, modifyValue, true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.JingMaiLevel:
                                //GameManager.ClientMgr.ModifyJingMaiLevelValue(otherClient, modifyValue, true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.WuXueLevel:
                                //GameManager.ClientMgr.ModifyWuXueLevelValue(otherClient, modifyValue, true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.ZuanHuangLevel:
                                GameManager.ClientMgr.ModifyZuanHuangLevelValue(otherClient, modifyValue, true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.SystemOpenValue:
                                GameManager.ClientMgr.ModifySystemOpenValue(otherClient, modifyValue, true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.JunGong:
                                GameManager.ClientMgr.ModifyJunGongValue(otherClient, modifyValue, true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.ZhanHun:
                                GameManager.ClientMgr.ModifyZhanHunValue(otherClient, modifyValue, true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.RongYu:
                                GameManager.ClientMgr.ModifyRongYuValue(otherClient, modifyValue, true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.ShengWang:
                                GameManager.ClientMgr.ModifyShengWangValue(otherClient, modifyValue, "GM指令(modifyparams)增加声望", true, true);
                                break;
                            case (int)RoleCommonUseIntParamsIndexs.ShengWangLevel:
                                {
                                    int oldValue = GameManager.ClientMgr.GetShengWangLevelValue(client);
                                    GameManager.ClientMgr.ModifyShengWangLevelValue(otherClient, modifyValue - oldValue, "GM指令(modifyparams)增加声望等级", true, true);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        strinfo = string.Format("请输入：-modifyparams 角色名称 参数索引(0成就 1积分 2猎杀 3悟性 4真气 5天地精元 6试炼令[===>通天令值] 7经脉等级 8武学等级 9钻皇等级 10系统激活项 11军功) 值(正负)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-modifyparams2" == cmdFields[0]) //修改角色的，主要用于整数参数值
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 4)
                    {
                        string otherRoleName = cmdFields[1];

                        //根据ID查找玩家
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        string paramKey = cmdFields[2];
                        string modifyValue = cmdFields[3];

                        Global.UpdateRoleParamByName(client, paramKey, modifyValue, true);
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求修改角色[{0}]的参数[{1}]为[{2}]", otherRoleName, paramKey, modifyValue));
                    }
                    else
                    {
                        strinfo = string.Format("请输入：-modifyparams2 角色名称 参数名 值(正负)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-summon" == cmdFields[0]) //在角色身边召唤怪物
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 3)
                    {
                        int monsterID = Global.SafeConvertToInt32(cmdFields[1]);
                        int num = Global.SafeConvertToInt32(cmdFields[2]);

                        if (num > 0 && monsterID > 0 && num < 50)
                        {
                            GameManager.LuaMgr.AddDynamicMonsters(client, monsterID, num, (int)client.CurrentGrid.X, (int)client.CurrentGrid.Y, 3);
                            strinfo = string.Format("执行GM召唤怪物");
                        }
                        else
                        {
                            strinfo = string.Format("请输入：-summon 怪物id 数量[最多49]");
                        }
                    }
                    else
                    {
                        strinfo = string.Format("请输入：-summon 怪物id 数量[最多49]");
                    }
                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                        client, strinfo);
                }
            }
            else if ("-refreshqg" == cmdFields[0]) //刷新抢购列表
            {
                if (!transmit) //非转发的命令
                {
                    ReloadXmlManager.ReloadXmlFile("config/qianggou.xml");
                }
            }
            else if ("-addgumutime" == cmdFields[0]) //修改角色的，主要用于整数参数值
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 3)
                    {
                        string otherRoleName = cmdFields[1];

                        //根据ID查找玩家
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        int timeMinute = Global.SafeConvertToInt32(cmdFields[2]);
                        Global.AddGuMuMapTime(otherClient, 0, timeMinute * 60);
                    }
                    else
                    {
                        strinfo = string.Format("增减古墓buffer时间请输入：-addgumutime 角色名称 值(正负，单位分钟)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-addlimitlogin" == cmdFields[0]) //修改角色的，限制时间累计登录的次数
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 2)
                    {
                        string otherRoleName = cmdFields[1];

                        //根据ID查找玩家
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        Global.UpdateLimitTimeLoginNum(otherClient); //先强迫走一下，进行初始化

                        HuodongData huodongData = client.ClientData.MyHuodongData;
                        huodongData.LimitTimeLoginNum++;

                        // 增加活动信息DB处理 [1/18/2014 LiaoWei]
                        Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient);

                        GameManager.ClientMgr.NotifyHuodongData(client); //通知客户端更新
                    }
                    else
                    {
                        strinfo = string.Format("增加限制时间累计登录次数请输入：-addlimitlogin 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-runnum" == cmdFields[0]) //修改角色的，副本单次循环的最大限制
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 2)
                    {
                        int maxNum = Global.SafeConvertToInt32(cmdFields[1]);
                        MonsterZoneManager.MaxRunQueueNum = Global.GMax(1, maxNum);

                        strinfo = string.Format(string.Format("副本单次循环的最大限制设置为：{0}", MonsterZoneManager.MaxRunQueueNum));
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("副本单次循环的最大限制设置请输入：-runnum 次数(最大500)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-waitnum" == cmdFields[0]) //修改角色的，副本队列的等待数量
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 2)
                    {
                        int maxNum = Global.SafeConvertToInt32(cmdFields[1]);
                        MonsterZoneManager.MaxWaitingRunQueueNum = Global.GMax(1, maxNum);

                        strinfo = string.Format(string.Format("副本队列的等待数量设置为：{0}", MonsterZoneManager.MaxRunQueueNum));
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("副本队列的等待数量设置请输入：-waitnum 次数(最大500)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-sklevel" == cmdFields[0]) //修怪物的自动寻敌和自动走动的最低级别限制
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 2)
                    {
                        int minLevel = Global.SafeConvertToInt32(cmdFields[1]);
                        MonsterManager.MinSeekRangeMonsterLevel = Global.GMax(0, minLevel);

                        strinfo = string.Format(string.Format("怪物的自动寻敌和自动走动的最低级别设置为：{0}", MonsterManager.MinSeekRangeMonsterLevel));
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("怪物的自动寻敌和自动走动的最低级别设置请输入：-sklevel 级别");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-st9grid" == cmdFields[0]) //修改九宫格的更新相关信息
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 5)
                    {
                        GameManager.MaxSlotOnUpdate9GridsTicks = Math.Max(1000, Global.SafeConvertToInt32(cmdFields[1]));
                        GameManager.MaxSleepOnDoMapGridMoveTicks = Math.Max(5, Global.SafeConvertToInt32(cmdFields[2]));
                        GameManager.MaxCachingMonsterToClientBytesDataTicks = Math.Max(0, Global.SafeConvertToInt32(cmdFields[3]));
                        GameManager.MaxCachingClientToClientBytesDataTicks = Math.Max(0, Global.SafeConvertToInt32(cmdFields[4]));

                        strinfo = string.Format(string.Format("九宫格相关信息设置：九宫更新间隔毫秒={0}, 间歇休眠毫秒={1}, 怪物缓存毫秒={2}, 角色缓存毫秒={3}",
                            GameManager.MaxSlotOnUpdate9GridsTicks, GameManager.MaxSleepOnDoMapGridMoveTicks, GameManager.MaxCachingMonsterToClientBytesDataTicks, GameManager.MaxCachingClientToClientBytesDataTicks));
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("九宫格相关信息设置：-st9grid 九宫更新间隔毫秒 间歇休眠毫秒 怪物缓存毫秒 角色缓存毫秒");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-st9gridinfo" == cmdFields[0]) //修怪物的自动寻敌和自动走动的最低级别限制
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 1)
                    {
                        strinfo = string.Format(string.Format("九宫格相关信息设置：九宫更新间隔毫秒={0}, 间歇休眠毫秒={1}, 怪物缓存毫秒={2}, 角色缓存毫秒={3}",
                            GameManager.MaxSlotOnUpdate9GridsTicks, GameManager.MaxSleepOnDoMapGridMoveTicks, GameManager.MaxCachingMonsterToClientBytesDataTicks, GameManager.MaxCachingClientToClientBytesDataTicks));
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("九宫格相关信息显示：-st9gridinfo");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-st9gridmode" == cmdFields[0]) //修改九宫格的更新模式
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 3)
                    {
                        GameManager.Update9GridUsingPosition = Global.SafeConvertToInt32(cmdFields[1]);
                        GameManager.MaxSlotOnPositionUpdate9GridsTicks = Global.SafeConvertToInt32(cmdFields[2]);

                        strinfo = string.Format(string.Format("九宫格相关信息设置：九宫更新模式={0}, 位置更新九宫格时间间隔={1}",
                            GameManager.Update9GridUsingPosition, GameManager.MaxSlotOnPositionUpdate9GridsTicks));
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("九宫格相关信息设置：-st9gridmode 九宫更新模式 位置更新九宫格时间间隔");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-st9gridnew" == cmdFields[0]) //修改九宫格的更新模式
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 3)
                    {
                        //GameManager.Update9GridUsingNewMode = Global.SafeConvertToInt32(cmdFields[1]);
                        GameManager.MaxSlotOnPositionUpdate9GridsTicks = Global.SafeConvertToInt32(cmdFields[2]);

                        strinfo = string.Format(string.Format("九宫格相关信息设置：九宫更新模式={0}, 位置更新九宫格时间间隔={1}",
                            GameManager.Update9GridUsingNewMode, GameManager.MaxSlotOnPositionUpdate9GridsTicks));
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("九宫格相关信息设置：-st9gridnew 九宫更新模式 位置更新九宫格时间间隔");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-stzipsize" == cmdFields[0]) //压缩的zip大小设置
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 2)
                    {
                        DataHelper.MinZipBytesSize = Global.SafeConvertToInt32(cmdFields[1]);

                        strinfo = string.Format(string.Format("压缩的zip大小：最小压缩={0}",
                            DataHelper.MinZipBytesSize));
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("压缩的zip大小设置：-stzipsize 最小压缩");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
                else
                {
                    if (cmdFields.Length >= 2)
                    {
                        DataHelper.MinZipBytesSize = Global.SafeConvertToInt32(cmdFields[1]);
                        LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为设置Zip压缩最小值为{0}", DataHelper.MinZipBytesSize));
                    }
                }
            }
            else if ("-stroledatamini" == cmdFields[0]) //设置roleDataMini模式
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 2)
                    {
                        GameManager.RoleDataMiniMode = Global.SafeConvertToInt32(cmdFields[1]);

                        strinfo = string.Format(string.Format("设置roleDataMini模式：模式={0}",
                            GameManager.RoleDataMiniMode));
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("设置roleDataMini模式：-stroledatamini 模式(0/1)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-stkaifuawardhour" == cmdFields[0]) //设置开服在线奖励的小时
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 2)
                    {
                        HuodongCachingMgr.ProcessKaiFuGiftAwardHour = Global.SafeConvertToInt32(cmdFields[1]);

                        strinfo = string.Format(string.Format("设置开服在线奖励的小时：hour={0}",
                            HuodongCachingMgr.ProcessKaiFuGiftAwardHour));
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("设置开服在线奖励的小时：-stkaifuawardhour 小时(0~23)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-addonlinesecs" == cmdFields[0]) //增加在线时长
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 3)
                    {
                        string otherRoleName = cmdFields[1];
                        int secs = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));
                        secs = secs * 60;

                        //根据ID查找玩家
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        int oldTotalOnlineHours = (int)(client.ClientData.TotalOnlineSecs / 3600);

                        otherClient.ClientData.TotalOnlineSecs += secs;

                        int newTotalOnlineHours = (int)(client.ClientData.TotalOnlineSecs / 3600);

                        if (oldTotalOnlineHours != newTotalOnlineHours)
                        {
                            //处理角色的在线累计
                            HuodongCachingMgr.ProcessKaiFuGiftAward(client);
                        }

                        strinfo = string.Format("给{0}增加在线时长到：{1}分钟", otherRoleName, otherClient.ClientData.TotalOnlineSecs / 60);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("增加在线时长：-stkaifuawardhour 角色名称 分钟");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-banchat2" == cmdFields[0]) //增加在线时长
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 3)
                    {
                        string otherRoleName = cmdFields[1];
                        int banChatVal = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));

                        //根据ID查找玩家
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, true);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null != otherClient)
                        {
                            otherClient.ClientData.BanChat = banChatVal;
                        }

                        //设置角色的属性
                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_UPDATEROLEPROPS,
                            string.Format("{0}:{1}:{2}", roleID, (int)RolePropIndexs.BanChat, banChatVal),
                            null);

                        strinfo = string.Format("将{0}永久禁言设置为：{1}", otherRoleName, banChatVal);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("永久禁言：-banchat2 角色名称 (0/1)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
                else
                {
                    if (cmdFields.Length >= 3)
                    {
                        int roleID = Global.SafeConvertToInt32(cmdFields[1]);
                        int banChatVal = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null != otherClient)
                        {
                            otherClient.ClientData.BanChat = banChatVal;
                        }

                        //设置角色的属性
                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_UPDATEROLEPROPS,
                            string.Format("{0}:{1}:{2}", roleID, (int)RolePropIndexs.BanChat, banChatVal),
                            null);
                    }
                }
            }
            else if ("-ban2" == cmdFields[0]) //增加在线时长
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -ban2 角色名称 (0/1)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int banValue = Global.SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找敌人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, true);
                        if (-1 != roleID)
                        {
                            if (banValue > 0)
                            {
                                GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                                if (null != otherClient)
                                {
                                    LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将{0}踢出了服务器，并禁止从任何线路再登陆", otherRoleName));

                                    //关闭角色连接
                                    Global.ForceCloseClient(otherClient);
                                }
                                else
                                {
                                    string gmCmdData = string.Format("-kick {0}", cmdFields[1]);

                                    //转发GM消息到DBServer
                                    GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_SPR_CHAT,
                                        string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", roleID, "", 0, "", 0, gmCmdData, 0, 0, GameManager.ServerLineID),
                                        null);
                                }
                            }
                        }

                        //设置角色的属性
                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_UPDATEROLEPROPS,
                            string.Format("{0}:{1}:{2}", roleID, (int)RolePropIndexs.BanLogin, banValue),
                            null);

                        strinfo = string.Format("将{0}永久禁止登陆设置为：{1}", otherRoleName, banValue);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
                else
                {
                    if (cmdFields.Length >= 3)
                    {
                        int roleID = Global.SafeConvertToInt32(cmdFields[1]);
                        int banValue = Global.SafeConvertToInt32(cmdFields[2]);

                        if (-1 != roleID)
                        {
                            if (banValue > 0)
                            {
                                GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                                if (null != otherClient)
                                {
                                    //关闭角色连接
                                    Global.ForceCloseClient(otherClient);
                                }
                            }
                        }

                        //设置角色的属性
                        GameManager.DBCmdMgr.AddDBCmd((int)TCPGameServerCmds.CMD_DB_UPDATEROLEPROPS,
                            string.Format("{0}:{1}:{2}", roleID, (int)RolePropIndexs.BanLogin, banValue),
                            null);
                    }
                }
            }
            else if ("-getmapalivemon" == cmdFields[0]) //获取指定地图上活着的怪物的数量
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -getmapalivemon 地图编号");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        int mapCode = Global.SafeConvertToInt32(cmdFields[1]);

                        int lifeVZeroNum = 0;
                        int NoAliveNum = 0;
                        int totalMonsterNum = 0;

                        List<Object> objList = GameManager.MonsterMgr.GetObjectsByMap(mapCode);
                        if (null != objList)
                        {
                            totalMonsterNum = objList.Count;
                            for (int i = 0; i < objList.Count; i++)
                            {
                                if ((objList[i] as Monster).VLife <= 0)
                                {
                                    lifeVZeroNum++;
                                }

                                if (!(objList[i] as Monster).Alive)
                                {
                                    NoAliveNum++;
                                }
                            }
                        }

                        strinfo = string.Format("本地图的怪物信息，totalMonsterNum={0}，lifeVZeroNum={1}，NoAliveNum={2}", totalMonsterNum, lifeVZeroNum, NoAliveNum);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-setmapalivemon" == cmdFields[0]) //获取指定地图上活着的怪物的数量
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -setmapalivemon 地图编号");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        int mapCode = Global.SafeConvertToInt32(cmdFields[1]);

                        int totalMonsterNum = 0;

                        List<Object> objList = GameManager.MonsterMgr.GetObjectsByMap(mapCode);
                        if (null != objList)
                        {
                            totalMonsterNum = objList.Count;
                            for (int i = 0; i < objList.Count; i++)
                            {
                                if ((objList[i] as Monster).VLife <= 0)
                                {
                                    if ((objList[i] as Monster).Alive)
                                    {
                                        (objList[i] as Monster).Alive = false;
                                    }
                                }
                            }
                        }

                        strinfo = string.Format("矫正本地图的怪物复活信息状态，totalMonsterNum={0}", totalMonsterNum);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-forcemapalivemon" == cmdFields[0]) //获取指定地图上活着的怪物的数量
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -forcemapalivemon 地图编号");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        int mapCode = Global.SafeConvertToInt32(cmdFields[1]);

                        int totalMonsterNum = 0;

                        List<Object> objList = GameManager.MonsterMgr.GetObjectsByMap(mapCode);
                        if (null != objList)
                        {
                            totalMonsterNum = objList.Count;
                            for (int i = 0; i < objList.Count; i++)
                            {
                                GameManager.MonsterMgr.AddDelayDeadMonster((objList[i] as Monster));
                            }
                        }

                        strinfo = string.Format("强制本地图的怪物重新死亡，totalMonsterNum={0}", totalMonsterNum);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-addactivevalue" == cmdFields[0]) // 给角色添加活跃值 [3/9/2014 LiaoWei]
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -adddj 角色名称 活跃值(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int activevalue = SafeConvertToInt32(cmdFields[2]);

                        //根据ID查找人
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        int n = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveValue);

                        client.ClientData.DailyActiveValues += activevalue;
                        if (activevalue >= 0)
                        {
                            DailyActiveManager.ModifyDailyActiveInfor(client, (uint)client.ClientData.DailyActiveValues, DailyActiveDataField1.DailyActiveValue, true);
                        }
                        else
                        {
                            DailyActiveManager.ModifyDailyActiveInfor(client, (uint)client.ClientData.DailyActiveValues, DailyActiveDataField1.DailyActiveValue, true);
                        }

                        n = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveValue);

                        DailyActiveManager.NotifyClientDailyActiveData(client);

                        strinfo = string.Format("为{0}添加了活跃{1}", otherRoleName, activevalue);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-addsw" == cmdFields[0]) // 给角色添加声望值 [4/7/2014 LiaoWei]
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -adddj 角色名称 声望值(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int nValue = SafeConvertToInt32(cmdFields[2]);

                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameManager.ClientMgr.ModifyShengWangValue(otherClient, nValue, "GM指令增加声望", true, true);

                        strinfo = string.Format("为{0}添加了声望{1}", otherRoleName, nValue);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-pkking" == cmdFields[0]) //获取指定地图上活着的怪物的数量
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -pkking 角色名称");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];

                        //从DBServer获取角色的所在的线路
                        string[] dbFields = Global.ExecuteDBCmd((int)TCPGameServerCmds.CMD_SPR_QUERYIDBYNAME, string.Format("{0}:{1}:0", client.ClientData.RoleID, otherRoleName));
                        if (null == dbFields || dbFields.Length < 5)
                        {
                            strinfo = string.Format("获取{0}的角色ID时连接数据库失败", otherRoleName);
                        }
                        else
                        {
                            int otherRoleID = Global.SafeConvertToInt32(dbFields[3]);

                            GameManager.ArenaBattleMgr.SetPKKingRoleID(otherRoleID);

                            //重新恢复显示PK之王
                            GameManager.ArenaBattleMgr.ReShowPKKing();

                            strinfo = string.Format("{0}已经设置为当前的PK之王", otherRoleName);
                        }

                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-testdata" == cmdFields[0]) //用于开发时做测试的指令,修改运行时数据
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -testdata 功能号 {参数1|参数2|参数3...}");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string result = "";
                        int funcID = Global.SafeConvertToInt32(cmdFields[1]);

                        if (funcID == 0) //修改角色属性
                        {
                            ExtPropIndexes attribIndex = (ExtPropIndexes)Global.SafeConvertToInt32(cmdFields[2]);
                            int attribValue = Global.SafeConvertToInt32(cmdFields[3]);
                            client.RoleBuffer.AddForeverExtProp((int)attribIndex, attribValue);
                        }
                        else if (funcID == 1)
                        {
                            int fallNumber = 0;
                            int fallNothing = 0;
                            for (int i = 0; i < 10000; i++)
                            {
                                int fallID = Global.SafeConvertToInt32(cmdFields[2]);
                                int count = Global.SafeConvertToInt32(cmdFields[3]);
                                List<GoodsData> goodsDataListLimitTime = GameManager.GoodsPackMgr.GetGoodsDataList(client, fallID, count, 0);
                                if (null != goodsDataListLimitTime && goodsDataListLimitTime.Count > 0)
                                {
                                    fallNumber += goodsDataListLimitTime.Count;
                                }
                                else
                                {
                                    fallNothing++;
                                }
                            }
                            result = string.Format("总计：{0}，轮空：{1}", fallNumber, fallNothing);
                        }
                        else if (funcID == 2)
                        {
                            //修改天使神殿成长值
                            double AngelTempleMonsterUpgradePercent = Global.SafeConvertToDouble(cmdFields[2]);
                            Global.UpdateDBGameConfigg(GameConfigNames.AngelTempleMonsterUpgradeNumber, AngelTempleMonsterUpgradePercent.ToString("0.00"));
                            result = string.Format("将天使神殿Boss成长比例设置为{0}", AngelTempleMonsterUpgradePercent);
                        }
                        else if (funcID == 100)
                        {
                            #region 修改性能分析测试开关

                            //修改性能分析测试开关
                            if (cmdFields.Length >= 4)
                            {
                                int subFucnID = Global.SafeConvertToInt32(cmdFields[2]);
                                switch (subFucnID)
                                {
                                    case 1:
                                        //GameManager.FlagSkipSendDataCall = cmdFields[3] == "1";
                                        result = string.Format("设置测试开关,跳过SocketData调用: {0}", GameManager.FlagSkipSendDataCall);
                                        break;
                                    case 2:
                                        //GameManager.FlagSkipAddBuffCall = cmdFields[3] == "1";
                                        result = string.Format("设置测试开关,跳过AddBuff调用: {0}", GameManager.FlagSkipAddBuffCall);
                                        break;
                                    case 3:
                                        //GameManager.FlagSkipTrySendCall = cmdFields[3] == "1";
                                        result = string.Format("设置测试开关,跳过TrySend调用: {0}", GameManager.FlagSkipTrySendCall);
                                        break;
                                    case 4:
                                        //GameManager.FlagSkipSocketSend = cmdFields[3] == "1";
                                        result = string.Format("设置测试开关,跳过Socket发送: {0}", GameManager.FlagSkipSocketSend);
                                        break;
                                    default:
                                        result = string.Format("请输入子功能号(1-4)和值(0,1)");
                                        break;
                                }
                            }
                            else
                            {
                                result = string.Format("请输入子功能号(1-4)和值(0,1)");
                            }

                            #endregion 修改性能分析测试开关
                        }

                        strinfo = string.Format("执行测试功能{0}{1}", funcID, result);

                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-showprops" == cmdFields[0]) //显示角色的属性加成情况
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -showprops 角色名");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameManager.ThreadStaticPropSB = new StringBuilder();
                        Global.PrintSomeProps(client, ref GameManager.ThreadStaticPropSB);
                        Global.RefreshEquipProp(client);
                        strinfo = string.Format("{0}的属性信息\n{1}", otherRoleName, GameManager.ThreadStaticPropSB.ToString());

                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-nameserver" == cmdFields[0]) //运行时修改名字服务器设置
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -nameserver 1或0(1启用 0禁用)} [名字服务器IP] [名字服务器端口] [本服务器的平台ID编号]\n中括号内的参数可不填,'*'号表示不修改");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string result = "";
                        int funcID = Global.SafeConvertToInt32(cmdFields[1]);

                        if (funcID == 0) //修改角色属性
                        {
                            Global.Flag_NameServer = false;
                            result = string.Format("禁用名字服务器,不通过验证就可创建");
                        }
                        else
                        {
                            Global.Flag_NameServer = true;
                            if (cmdFields.Length >= 3 && cmdFields[2] != "*") NameServerNamager.NameServerIP = cmdFields[2];
                            if (cmdFields.Length >= 4 && cmdFields[3] != "*") NameServerNamager.NameServerPort = SafeConvertToInt32(cmdFields[3]);
                            if (cmdFields.Length >= 5 && cmdFields[4] != "*") NameServerNamager.NameServerConfig = SafeConvertToInt32(cmdFields[4]);
                            result = string.Format("启用名字服务器，需通过名字认证服务器验证才可创建，\nIP：{0}    端口：{1}    配置选项：{2}",
                                                        NameServerNamager.NameServerIP, NameServerNamager.NameServerPort, NameServerNamager.NameServerConfig);
                        }

                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, result);
                    }
                }
            }
            else if ("-testproc" == cmdFields[0]) //用于开发时做测试的指令
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -testproc 功能号 {参数1|参数2|参数3...}");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string result = "";
                        int funcID = Global.SafeConvertToInt32(cmdFields[1]);

                        if (funcID == 0)
                        {
                            GameManager.AngelTempleMgr.GiveAwardAngelTempleScene(cmdFields.Length >= 3 && cmdFields[2] == "1");
                        }
                        else if (funcID == 1)
                        {
                            int fallNumber = 0;
                            int fallNothing = 0;
                            for (int i = 0; i < 10000; i++)
                            {
                                int fallID = Global.SafeConvertToInt32(cmdFields[2]);
                                int count = Global.SafeConvertToInt32(cmdFields[3]);
                                List<GoodsData> goodsDataListLimitTime = GameManager.GoodsPackMgr.GetGoodsDataList(client, fallID, count, 0);
                                if (null != goodsDataListLimitTime && goodsDataListLimitTime.Count > 0)
                                {
                                    fallNumber += goodsDataListLimitTime.Count;
                                }
                                else
                                {
                                    fallNothing++;
                                }
                            }
                            result = string.Format("总计：{0}，轮空：{1}", fallNumber, fallNothing);
                        }
                        else if (funcID == 2)
                        {
                            //GameManager.FlagSleepTime = 1000;
                        }

                        strinfo = string.Format("执行测试功能{0}{1}", funcID, result);

                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-hidefakerole" == cmdFields[0]) //显示或隐藏假人
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 2)
                    {
                        strinfo = string.Format("请输入： -hidefakerole 1或0(1隐藏 0不隐藏)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        GameManager.TestGameShowFakeRoleForUser = Global.SafeConvertToInt32(cmdFields[1]) == 0;
                        strinfo = GameManager.TestGameShowFakeRoleForUser ? "设置为显示(挂机)假人" : "设置为隐藏(挂机)假人";
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-addxh" == cmdFields[0]) // 给角色添加星魂值 [8/5/2014 LiaoWei]
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length < 3)
                    {
                        strinfo = string.Format("请输入： -adddj 角色名称 声望值(可以是负数)");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        string otherRoleName = cmdFields[1];
                        int nValue = SafeConvertToInt32(cmdFields[2]);

                        int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName);
                        if (-1 == roleID)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
                        if (null == otherClient)
                        {
                            strinfo = string.Format("{0}不在线", otherRoleName);
                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                                client, strinfo);

                            return true;
                        }

                        otherClient.ClientData.StarSoul += nValue;

                        Global.SaveRoleParamsInt32ValueToDB(client, RoleParamName.StarSoul, otherClient.ClientData.StarSoul, true);

                        GameManager.ClientMgr.NotifySelfParamsValueChange(otherClient, RoleCommonUseIntParamsIndexs.StarSoulValue, otherClient.ClientData.StarSoul);

                        strinfo = string.Format("为{0}添加了星魂{1}", otherRoleName, nValue);
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else if ("-stminsendsize" == cmdFields[0]) //设置发送缓冲的大小
            {
                if (!transmit) //非转发的命令
                {
                    if (cmdFields.Length == 2)
                    {
                        SendBuffer.ConstMinSendSize = Global.SafeConvertToInt32(cmdFields[1]);

                        strinfo = string.Format(string.Format("发送缓冲的大小：最小缓冲={0}",
                            SendBuffer.ConstMinSendSize));
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                    else
                    {
                        strinfo = string.Format("发送缓冲的大小设置：-stminsendsize 最小缓冲");
                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                            client, strinfo);
                    }
                }
            }
            else
            {
                ;
            }    

            return true;
        }

        #endregion 聊天发送时的GM命令处理
    }
}
