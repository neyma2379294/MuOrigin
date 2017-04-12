using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Core.GameEvent;
using GameServer.Server;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server.CmdProcesser;
using Server.Data;
using System.Threading;
using Server.TCP;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic.Copy
{
    public static class CopyTeamErrorCodes
    {
        public const int Success = 1; //成功
        public const int NoTeam = -1; //你已经不在队伍中(没有队伍)
        public const int TeamIsDestoryed = -2; //队伍已解散
        public const int AllreadyHasTeam = -3; //已有队伍
        public const int NotTeamLeader = -4; //你不是队长(队长才能执行)
        public const int TeamIsFull = -5; //队伍已满
        public const int ZhanLiLow = -6; //未达到队伍的最低战力要求
        public const int NoAcceptableTeam = -7; //没有合适的队伍


        public const int LeaveTeam = -11; //离开队伍
        public const int BeRemovedFromTeam = -12; //被请出队伍
    }

    /// <summary>
    /// 战盟事件管理器
    /// </summary>
    public class CopyTeamManager : IManager
    {
        #region 标准接口

        private static CopyTeamManager instance = new CopyTeamManager();

        private HashSet<int> RecordDamagesFuBenIDHashSet = new HashSet<int>();

        private CopyTeamManager() { }

        public static CopyTeamManager getInstance()
        {
            return instance;
        }

        public bool initialize()
        {
            //注册指令处理器
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_SPR_COPYTEAM, 5, CopyTeamCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_COPYTEAM));
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_SPR_REGEVENTNOTIFY, 4, CopyTeamCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_REGEVENTNOTIFY));
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_SPR_LISTCOPYTEAMS, 4, CopyTeamCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_LISTCOPYTEAMS));

            //向事件源注册监听器
            GlobalEventSource.getInstance().registerListener((int)EventTypes.PlayerLeaveFuBen, CopyTeamEventListener.getInstance());
            GlobalEventSource.getInstance().registerListener((int)EventTypes.PlayerLogout, CopyTeamEventListener.getInstance());

            //初始化组队副本的列表
            lock (SceneIndexRoleIDListDict)
            {
                lock (_SceneIndexDict)
                {
                    foreach (var systemFuBenItem in GameManager.systemFuBenMgr.SystemXmlItemDict.Values)
                    {
                        int copyType = systemFuBenItem.GetIntValue("CopyType");
                        if (Global.ConstTeamCopyType == copyType)
                        {
                            int copyID = systemFuBenItem.GetIntValue("ID");
                            _SceneIndexDict.Add(copyID, copyID);
                            SceneIndexRoleIDListDict.Add(copyID, new HashSet<int>());
                        }
                    }
                }
            }

            //需要记录伤害排名的副本ID集合
            RecordDamagesFuBenIDHashSet.Add(4000);

            return true;
        }

        public bool startup()
        {
            return true;
        }

        public bool showdown()
        {
            return true;
        }

        public bool destroy()
        {
            //向事件源删除监听器
            GlobalEventSource.getInstance().removeListener((int)EventTypes.PlayerLeaveFuBen, CopyTeamEventListener.getInstance());
            GlobalEventSource.getInstance().removeListener((int)EventTypes.PlayerLogout, CopyTeamEventListener.getInstance());
            return true;
        }

        #endregion 标准接口

        #region 功能接口

        public const int MaxTeamMemberCount = 5;

        public const int ConstCopyType = 1;

        private Dictionary<int, int> _SceneIndexDict = new Dictionary<int, int>();

        public Dictionary<int, HashSet<int>> SceneIndexRoleIDListDict = new Dictionary<int, HashSet<int>>();

        public void RegisterCopyTeamListNotify(GameClient client, int sceneIndex)
        {
            int oldTeamID = FindRoleID2TeamID(client.ClientData.RoleID);
            if (oldTeamID > 0) //如果有队伍
            {
                QuitFromTeam(client);
            } 
            
            int roleID = client.ClientData.RoleID;
            lock (SceneIndexRoleIDListDict)
            {
                List<int> keyList;
                HashSet<int> list;
                keyList = SceneIndexRoleIDListDict.Keys.ToList();
                foreach (var key in keyList)
                {
                    list = SceneIndexRoleIDListDict[key];
                    if (key == sceneIndex)
                    {
                        if (!list.Contains(roleID))
                        {
                            list.Add(roleID);
                        }
                    }
                    else
                    {
                        list.Remove(roleID);
                    }
                }
            }

            ListAllTeams(client, 0, sceneIndex);
        }

        public void UnRegisterCopyTeamListNotifyForOfflineClient(List<int> removelist, int sceneIndex)
        {
            if (null == removelist) return;

            lock (SceneIndexRoleIDListDict)
            {
                HashSet<int> list;
                if (SceneIndexRoleIDListDict.TryGetValue(sceneIndex, out list))
                {
                    foreach (var id in removelist)
                    {
                        list.Remove(id);
                    }
                }
            }
        }

        public void UnRegisterCopyTeamListNotify(GameClient client)
        {
            int roleID = client.ClientData.RoleID;
            lock (SceneIndexRoleIDListDict)
            {
                List<int> keyList;
                HashSet<int> list;
                keyList = SceneIndexRoleIDListDict.Keys.ToList();
                foreach (var key in keyList)
                {
                    list = SceneIndexRoleIDListDict[key];
                    list.Remove(roleID);
                }
            }
        }

        public void CreateCopyTeam(GameClient client, int sceneIndex, int minZhanLi, int autoStart)
        {
            int roleID = client.ClientData.RoleID;
            int teamID = FindRoleID2TeamID(roleID);
            if (teamID > 0)
            {
                QuitFromTeam(client);
            }

            teamID = GetNextAutoID();
            AddRoleID2TeamID(roleID, teamID);

            CopyTeamData td = new CopyTeamData()
            {
                TeamID = teamID,
                LeaderRoleID = roleID,
                SceneIndex = sceneIndex,
                MinZhanLi = minZhanLi,
                AutoStart = autoStart > 0,
            };

            if (null == td.TeamRoles)
            {
                td.TeamRoles = new List<CopyTeamMemberData>();
            }

            td.TeamRoles.Add(ClientDataToTeamMemberData(client.ClientData));
            td.TeamRoles[0].IsReady = true;
            td.TeamName = td.TeamRoles[0].RoleName;
            td.MemberCount = td.TeamRoles.Count;

            //存入组队管理队列
            AddData(teamID, td);

            //通知组队数据的指令信息
            NotifyTeamCmd(client, CopyTeamErrorCodes.Success, (int)TeamCmds.Create, teamID, td.TeamName);
            NotifyTeamData(td);

            //添加队伍需要向注册通知的用户发送变化列表
            NotifyTeamListChange(td);
        }

        public void ApplyCopyTeam(GameClient client, int teamID, bool autoQuitOldTeam = false)
        {
            int roleID = client.ClientData.RoleID;
            int oldTeamID = FindRoleID2TeamID(client.ClientData.RoleID);
            if (oldTeamID > 0) //如果有队伍
            {
                if (autoQuitOldTeam)
                {
                    QuitFromTeam(client);
                }
                else
                {
                    NotifyTeamCmd(client, CopyTeamErrorCodes.AllreadyHasTeam, (int)TeamCmds.Apply, 0, "");
                    return;
                }
            }

            CopyTeamData td = FindData(teamID);
            if (null == td)
            {
                NotifyListTeamRemove(client, teamID);
                RemoveData(teamID);
                return;
            }

            lock (td)
            {
                //判断是否超过了最大人数限制
                if (td.TeamRoles.Count >= 5)
                {
                    //通知角色组队的指令信息
                    NotifyTeamCmd(client, CopyTeamErrorCodes.TeamIsFull, (int)TeamCmds.Apply, 0, "");
                    return;
                }

                //是否能加入队伍
                //if (!CopyTeamManager.getInstance().CanAddToTeam(roleID, otherClient.ClientData.TeamID, 0))
                if(client.ClientData.CombatForce < td.MinZhanLi)
                {
                    //通知角色组队的指令信息
                    NotifyTeamCmd(client, CopyTeamErrorCodes.ZhanLiLow, (int)TeamCmds.Apply, 0, "");
                    return;
                }

                int index = td.TeamRoles.FindIndex((x) => x.RoleID == roleID);
                if (index >= 0)
                {
                    td.TeamRoles[index] = ClientDataToTeamMemberData(client.ClientData);
                }
                else
                {
                    td.TeamRoles.Add(ClientDataToTeamMemberData(client.ClientData));
                }
                td.MemberCount = td.TeamRoles.Count;
            }

            AddRoleID2TeamID(roleID, teamID);

            //通知角色组队的指令信息
            NotifyTeamCmd(client, CopyTeamErrorCodes.Success, (int)TeamCmds.Apply, teamID, td.TeamName);
            NotifyTeamData(td);
            NotifyTeamListChange(td);
        }

        public void RemoveFromCopyTeam(GameClient client, int otherRoleID)
        {
            int teamType = (int)TeamCmds.Remove;
            int teamID = FindRoleID2TeamID(client.ClientData.RoleID);
            if (teamID <= 0) //如果没有队伍
            {
                //通知角色组队的指令信息
                NotifyTeamCmd(client, CopyTeamErrorCodes.NoTeam, teamType, 0, "");
                return;
            }

            //查找组队的数据
            CopyTeamData td = FindData(teamID);
            if (null == td) //没有找到组队数据
            {
                //清空组队ID
                RemoveRoleID2TeamID(client.ClientData.RoleID);

                //通知角色组队的指令信息
                NotifyTeamCmd(client, CopyTeamErrorCodes.TeamIsDestoryed, teamType, 0, "");
                return;
            }

            bool destroy = false;
            lock (td)
            {
                //判断是否是队长
                if (td.LeaderRoleID != client.ClientData.RoleID)
                {
                    //通知角色组队的指令信息
                    NotifyTeamCmd(client, CopyTeamErrorCodes.NotTeamLeader, teamType, 0, "");
                    return;
                }

                if (td.TeamRoles.Count > 1) //转交队长
                {
                    for (int i = 0; i < td.TeamRoles.Count; i++)
                    {
                        if (td.TeamRoles[i].RoleID == otherRoleID)
                        {
                            td.TeamRoles.RemoveAt(i);
                            break;
                        }
                    }

                    //判断是否是队长
                    if (td.LeaderRoleID == client.ClientData.RoleID)
                    {
                        td.LeaderRoleID = td.TeamRoles[0].RoleID; //转交队长
                        td.TeamRoles[0].IsReady = true;
                        td.TeamName = td.TeamRoles[0].RoleName;
                    }
                }
                else
                {
                    destroy = true;
                    td.TeamRoles.Clear();
                    td.LeaderRoleID = -1; //强迫解散
                }
                td.MemberCount = td.TeamRoles.Count;
            }

            if (destroy)
            {
                //删除组队数据
                RemoveData(teamID);
            }

            //通知组队数据的指令信息
            NotifyTeamData(td); //发送null数据，强迫组队解散

            //清空组队ID
            RemoveRoleID2TeamID(otherRoleID);

            GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
            if (null != otherClient)
            {
                NotifyTeamStateChanged(otherClient, CopyTeamErrorCodes.BeRemovedFromTeam, otherRoleID, 0);
            }

            NotifyTeamListChange(td);
        }

        public void Ready(GameClient client, int ready)
        {
            //int teamType = (int)TeamCmds.Ready;
            int roleID = client.ClientData.RoleID;
            int teamID = FindRoleID2TeamID(client.ClientData.RoleID);
            if (teamID <= 0)
            {
                NotifyTeamStateChanged(client, CopyTeamErrorCodes.NoTeam, roleID, 0);
                return;
            }

            CopyTeamData td = FindData(teamID);
            if (null != td)
            {
                int readyCount = 0;
                bool someoneOffline = false;
                lock (td)
                {
                    for (int i = 0; i < td.TeamRoles.Count; i++)
                    {
                        GameClient gc;
                        if (td.TeamRoles[i].RoleID == roleID)
                        {
                            td.TeamRoles[i].IsReady = ready > 0; //更新状态
                            gc = client;
                        }
                        else
                        {
                            gc = GameManager.ClientMgr.FindClient(td.TeamRoles[i].RoleID);
                        }

                        if (null == gc)
                        {
                            td.TeamRoles[i].IsReady = false;
                            someoneOffline = true;
                            continue;
                        }

                        //状态变化通知
                        NotifyTeamStateChanged(gc, teamID, roleID, ready);

                        if (!someoneOffline && td.TeamRoles[i].IsReady && td.AutoStart)
                        {
                            readyCount++;
                            if (readyCount == MaxTeamMemberCount)
                            {
                                GameClient leader = GameManager.ClientMgr.FindClient(td.LeaderRoleID);
                                NotifyTeamCmd(leader, CopyTeamErrorCodes.Success, (int)TeamCmds.Start, 0, "");
                            }
                        }
                    }
                }

                if (someoneOffline)
                {
                    NotifyTeamData(td);
                }
            }
        }

        /// <summary>
        /// 程序退出时脱离组队信息
        /// </summary>
        /// <param name="client"></param>
        public void QuitFromTeam(GameClient client, bool notifyOther = true)
        {
            int roleID = client.ClientData.RoleID;
            int teamID = FindRoleID2TeamID(client.ClientData.RoleID);
            if (teamID <= 0) //如果没有队伍
            {
                return;
            }

            //查找组队的数据
            CopyTeamData td = FindData(teamID);
            if (null == td) //没有找到组队数据
            {
                //清空组队ID
                RemoveRoleID2TeamID(roleID);
                return;
            }

            bool updateTeamList = false;
            bool destroy = false;
            lock (td)
            {
                if (td.TeamRoles.Count > 1) //转交队长
                {
                    for (int i = 0; i < td.TeamRoles.Count; i++)
                    {
                        if (td.TeamRoles[i].RoleID == client.ClientData.RoleID)
                        {
                            td.TeamRoles.RemoveAt(i);
                            break;
                        }
                    }

                    //判断是否是队长
                    if (td.LeaderRoleID == client.ClientData.RoleID)
                    {
                        td.LeaderRoleID = td.TeamRoles[0].RoleID; //转交队长
                        td.TeamRoles[0].IsReady = true;
                        td.TeamName = td.TeamRoles[0].RoleName;
                    }
                    td.MemberCount = td.TeamRoles.Count;
                }
                else
                {
                    destroy = true;
                    td.LeaderRoleID = -1; //强迫解散
                    td.TeamRoles.Clear();
                }
                td.MemberCount = td.TeamRoles.Count;

                if (td.StartTime == 0)
                {
                    updateTeamList = true;
                }
            }

            if (destroy)
            {
                //删除组队数据
                RemoveData(teamID);
            }

            //清空组队ID
            RemoveRoleID2TeamID(roleID);

            //发送队伍状态
            NotifyTeamStateChanged(client, CopyTeamErrorCodes.LeaveTeam, roleID, 0);

            if (notifyOther)
            {
                //通知组队数据的指令信息
                NotifyTeamData(td); //发送null数据，强迫组队解散
            }

            if (updateTeamList)
            {
                //组队状态变化通知
                NotifyTeamListChange(td);
            }
        }

        /// <summary>
        /// 快速加入队伍
        /// </summary>
        /// <param name="?"></param>
        /// <param name="ed"></param>
        public void QuickJoinTeam(GameClient client, int sceneIndex)
        {
            int oldTeamID = FindRoleID2TeamID(client.ClientData.RoleID);
            if (oldTeamID > 0) //如果有队伍
            {
                NotifyTeamCmd(client, CopyTeamErrorCodes.AllreadyHasTeam, (int)TeamCmds.QuickJoinTeam, 0, "");
                return;
            }

            int zhanLi = client.ClientData.CombatForce;
            CopyTeamData td = null;
            lock (_TeamDataDict)
            {
                foreach (var teamData in _TeamDataDict.Values)
                {
                    if (sceneIndex == teamData.SceneIndex &&
                        teamData.StartTime == 0 &&
                        zhanLi >= teamData.MinZhanLi &&
                        teamData.MemberCount < MaxTeamMemberCount)
                    {
                        lock (teamData)
                        {
                            CopyTeamMemberData tm = ClientDataToTeamMemberData(client.ClientData);
                            teamData.TeamRoles.Add(tm);
                            teamData.MemberCount = teamData.TeamRoles.Count;
                        }
                        td = teamData;
                    }
                }
            }

            if (null != td)
            {
                AddRoleID2TeamID(client.ClientData.RoleID, td.TeamID);

                //通知角色组队的指令信息
                NotifyTeamCmd(client, CopyTeamErrorCodes.Success, (int)TeamCmds.QuickJoinTeam, td.TeamID, td.TeamName);
                NotifyTeamData(td);
                NotifyTeamListChange(td);
            }
            else
            {
                NotifyTeamCmd(client, CopyTeamErrorCodes.NoAcceptableTeam, (int)TeamCmds.QuickJoinTeam, -1, "");
            }
        }

        public bool CanEnterScene(GameClient client, out CopyTeamData td)
        {
            int roleID = client.ClientData.RoleID;
            int teamID = FindRoleID2TeamID(client.ClientData.RoleID);
            if (teamID <= 0)
            {
                NotifyTeamStateChanged(client, -1, roleID, 0);
                td = null;
                return false;
            }

            td = FindData(teamID);
            if (null != td && roleID == td.LeaderRoleID)
            {
                int readyCount = 0;
                bool someoneOffline = false;
                lock (td)
                {
                    for (int i = 0; i < td.TeamRoles.Count; i++)
                    {
                        GameClient gc = GameManager.ClientMgr.FindClient(td.TeamRoles[i].RoleID);
                        if (gc == null)
                        {
                            td.TeamRoles[i].IsReady = false;
                            someoneOffline = true;
                            break;
                        }
                        if (td.TeamRoles[i].IsReady)
                        {
                            readyCount++;
                        }
                    }
                }

                if (someoneOffline)
                {
                    NotifyTeamData(td);
                }

                if (readyCount == td.MemberCount)
                {
                    return true;
                }
            }

            td = null;
            return false;
        }

        #endregion 功能接口

        #region 组队流水ID

        /// <summary>
        /// 组队流水ID
        /// </summary>
        private int BaseAutoID = 0;

        /// <summary>
        /// 获取下一个组队流水ID
        /// </summary>
        /// <returns></returns>
        public int GetNextAutoID()
        {
            Interlocked.Add(ref BaseAutoID, 1);

            int n = 0;
            Interlocked.Exchange(ref n, BaseAutoID);
            return n;
        }

        #endregion 组队流水ID

        #region 角色ID和组队ID的映射管理

        /// <summary>
        /// 角色ID到组队ID的映射字典
        /// </summary>
        private Dictionary<int, int> _RoleID2TeamIDDict = new Dictionary<int, int>();

        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="?"></param>
        /// <param name="ed"></param>
        public void AddRoleID2TeamID(int roleID, int teamID)
        {
            lock (_RoleID2TeamIDDict)
            {
                _RoleID2TeamIDDict[roleID] = teamID;
            }
        }

        /// <summary>
        /// 删除项
        /// </summary>
        /// <param name="?"></param>
        /// <param name="ed"></param>
        public void RemoveRoleID2TeamID(int roleID)
        {
            lock (_RoleID2TeamIDDict)
            {
                if (_RoleID2TeamIDDict.ContainsKey(roleID))
                {
                    _RoleID2TeamIDDict.Remove(roleID);
                }
            }
        }

        /// <summary>
        /// 查找项
        /// </summary>
        /// <param name="?"></param>
        /// <param name="ed"></param>
        public int FindRoleID2TeamID(int roleID)
        {
            int teamID;
            lock (_RoleID2TeamIDDict)
            {
                if (_RoleID2TeamIDDict.TryGetValue(roleID, out teamID))
                {
                    return teamID;
                }
            }

            return -1;
        }

        #endregion 角色ID和组队ID的映射管理

        #region 组队项管理

        /// <summary>
        /// 组队项字典
        /// </summary>
        private Dictionary<int, CopyTeamData> _TeamDataDict = new Dictionary<int, CopyTeamData>();

        /// <summary>
        /// 副本顺序ID -> 队伍ID
        /// </summary>
        private Dictionary<int, int> _FuBenSeqID2TeamDataDict = new Dictionary<int, int>();

        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="?"></param>
        /// <param name="ed"></param>
        public void AddData(int teamID, CopyTeamData td)
        {
            lock (_TeamDataDict)
            {
                _TeamDataDict[teamID] = td;
            }
        }

        /// <summary>
        /// 删除项
        /// </summary>
        /// <param name="?"></param>
        /// <param name="ed"></param>
        public void RemoveData(int teamID)
        {
            lock (_TeamDataDict)
            {
                if (_TeamDataDict.ContainsKey(teamID))
                {
                    _TeamDataDict.Remove(teamID);
                }
            }
        }

        /// <summary>
        /// 查找项
        /// </summary>
        /// <param name="?"></param>
        /// <param name="ed"></param>
        public CopyTeamData FindData(int teamID)
        {
            CopyTeamData td = null;
            lock (_TeamDataDict)
            {
                _TeamDataDict.TryGetValue(teamID, out td);
            }

            return td;
        }

        /// <summary>
        /// 获取总的个数
        /// </summary>
        /// <returns></returns>
        public int GetTotalDataCount()
        {
            int count = 0;
            lock (_TeamDataDict)
            {
                count = _TeamDataDict.Count;
            }

            return count;
        }

        /// <summary>
        /// 返回从指定位置开始的指定的个数
        /// </summary>
        /// <param name="?"></param>
        /// <param name="ed"></param>
        public List<CopyTeamData> GetTeamDataList(int startIndex, int count, int sceneIndex, int zhanLi)
        {
            int index = 0;
            List<CopyTeamData> teamDataList = new List<CopyTeamData>();
            lock (_TeamDataDict)
            {
                foreach (var teamData in _TeamDataDict.Values)
                {
                    if (index >= startIndex &&
                        sceneIndex == teamData.SceneIndex &&
                        teamData.StartTime == 0 &&
                        zhanLi >= teamData.MinZhanLi &&
                        teamData.MemberCount < MaxTeamMemberCount)
                    {
                        teamDataList.Add(teamData.SimpleClone());
                        if (teamDataList.Count >= count)
                        {
                            break;
                        }
                    }
                    index++;
                }
            }

            return teamDataList;
        }

        /// <summary>
        /// 返回从指定位置开始的指定的个数
        /// </summary>
        /// <param name="?"></param>
        /// <param name="ed"></param>
        public List<CopyTeamData> GetTeamDataListInCopyMap(int sceneIndex = -1)
        {
            List<CopyTeamData> teamDataList = new List<CopyTeamData>();
            lock (_TeamDataDict)
            {
                foreach (var teamData in _TeamDataDict.Values)
                {
                    if (sceneIndex >= 0 && sceneIndex != teamData.SceneIndex)
                    {
                        continue;
                    }

                    if (teamData.StartTime > 0 &&
                        teamData.FuBenSeqID > 0)
                    {
                        teamDataList.Add(teamData);
                    }
                }
            }

            return teamDataList;
        }

        #endregion 组队项管理

        #region 其他

        /// <summary>
        /// 将ClientData 类型转换为 TeamMemberData类型(组队时使用)
        /// </summary>
        /// <param name="clientData"></param>
        /// <returns></returns>
        public CopyTeamMemberData ClientDataToTeamMemberData(SafeClientData clientData)
        {
            CopyTeamMemberData teamMemberData = new CopyTeamMemberData()
            {
                RoleID = clientData.RoleID,
                RoleName = Global.FormatRoleName2(clientData, clientData.RoleName),
                RoleSex = clientData.RoleSex,
                Level = clientData.Level,
                Occupation = clientData.Occupation,
                RolePic = clientData.RolePic,
                MapCode = clientData.MapCode,
                OnlineState = 1,
                MaxLifeV = clientData.LifeV,
                CurrentLifeV = clientData.CurrentLifeV,
                MaxMagicV = clientData.MagicV,
                CurrentMagicV = clientData.CurrentMagicV,
                PosX = clientData.PosX,
                PosY = clientData.PosY,
                CombatForce = clientData.CombatForce,
                ChangeLifeLev = clientData.ChangeLifeCount,
            };

            return teamMemberData;
        }

        public void RoleLeaveFuBen(GameClient client)
        {
#if true
            QuitFromTeam(client);
#else
            int roleID = client.ClientData.RoleID;
            int teamID = FindRoleID2TeamID(client.ClientData.RoleID);
            if (teamID <= 0) //如果没有队伍
            {
                return;
            }

            //查找组队的数据
            CopyTeamData td = FindData(teamID);
            if (null == td) //没有找到组队数据
            {
                //清空组队ID
                RemoveRoleID2TeamID(roleID);
                return;
            }

            bool destroy = false;
            lock (td)
            {
                if (td.MemberCount > 1) //转交队长
                {
                    for (int i = 0; i < td.TeamRoles.Count; i++)
                    {
                        if (td.TeamRoles[i].RoleID == client.ClientData.RoleID)
                        {
                            td.TeamRoles[i].OnlineState = 2;
                            td.MemberCount--;
                            break;
                        }
                    }
                }
                else
                {
                    destroy = true;
                }
            }

            if (destroy)
            {
                //删除组队数据
                RemoveData(teamID);
            }

            //清空组队ID
            RemoveRoleID2TeamID(roleID);

            //NotifyTeamStateChanged(client, CopyTeamErrorCodes.LeaveTeam, roleID, 0);//通知组队数据的指令信息
            NotifyTeamData(td);
#endif
        }

        public void OnPlayerLogout(GameClient client)
        {
            QuitFromTeam(client);
        }

        public int[] GetTeamCopyMapCodes()
        {
            List<int> list = new List<int>();
            lock (_SceneIndexDict)
            {
                list.AddRange(_SceneIndexDict.Keys);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 是否需要记录角色伤害信息
        /// </summary>
        /// <param name="fuBenID"></param>
        /// <returns></returns>
        public bool NeedRecordDamageInfoFuBenID(int fuBenID)
        {
            return RecordDamagesFuBenIDHashSet.Contains(fuBenID) || GameManager.GuildCopyMapMgr.IsGuildCopyMap(fuBenID);
        }

        #endregion

        #region 组队相关

        public void NotifyTeamListChange(CopyTeamData ctd)
        {
            List<GameClient> roleList = new List<GameClient>();
            List<int> removeList = null;
            lock(SceneIndexRoleIDListDict)
            {
                HashSet<int> list;
                if(SceneIndexRoleIDListDict.TryGetValue(ctd.SceneIndex, out list))
                {
                    foreach (var id in list)
                    {
                        GameClient client = GameManager.ClientMgr.FindClient(id);
                        if (null == client)
                        {
                            if (null == removeList) removeList = new List<int>();
                            removeList.Add(id);
                        }
                        else if (ctd.MemberCount == 0 || (ctd.MinZhanLi <= client.ClientData.CombatForce))
                        {
                            roleList.Add(client);
                        }
                    }
                }
            }

            for (int i = 0; i < roleList.Count; i++)
            {
                NotifyListTeamData(roleList[i], ctd);
            }

            UnRegisterCopyTeamListNotifyForOfflineClient(removeList, ctd.SceneIndex);
        }

        public void NotifyListTeamData(GameClient client, CopyTeamData ctd)
        {
            int memberCount = ctd.StartTime > 0 ? 0 : ctd.MemberCount; //如果开始了
            string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", ctd.SceneIndex, ctd.TeamID, ctd.TeamName, ctd.MemberCount, ctd.MinZhanLi);
            client.sendCmd((int)TCPGameServerCmds.CMD_SPR_LISTCOPYTEAMDATA, strcmd);
        }

        public void NotifyListTeamRemove(GameClient client, int teamID, int sceneIndex = -1)
        {
            string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", sceneIndex, teamID, "", 0, 0);
            client.sendCmd((int)TCPGameServerCmds.CMD_SPR_LISTCOPYTEAMDATA, strcmd);
        }

        /// <summary>
        /// 通知角色组队的指令信息
        /// </summary>
        /// <param name="client"></param>
        public void NotifyTeamCmd(GameClient client, int status, int teamType, int extTag1, string extTag2, int nOccu = -1, int nLev = -1, int nChangeLife = -1)
        {
            string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", status, client.ClientData.RoleID, teamType, extTag1, extTag2, nOccu, nLev, nChangeLife);
            client.sendCmd((int)TCPGameServerCmds.CMD_SPR_COPYTEAM, strcmd);
        }

        /// <summary>
        /// 通知组队数据的指令信息
        /// </summary>
        /// <param name="client"></param>
        public void NotifyTeamData(CopyTeamData td)
        {
            if (null != td)
            {
                lock (td)
                {
                    for (int i = 0; i < td.TeamRoles.Count; i++)
                    {
                        GameClient client = GameManager.ClientMgr.FindClient(td.TeamRoles[i].RoleID);
                        if (null == client) continue;
                        client.sendCmd<CopyTeamData>((int)TCPGameServerCmds.CMD_SPR_COPYTEAMDATA, td);
                    }
                }
            }
        }

        /// <summary>
        /// 组队队员状态变化通知
        /// </summary>
        /// <param name="client"></param>
        public void NotifyTeamStateChanged(GameClient client, int teamID, int roleID, int isReady)
        {
            string strcmd = string.Format("{0}:{1}:{2}", roleID, teamID, isReady);
            client.sendCmd((int)TCPGameServerCmds.CMD_SPR_COPYTEAMSTATE, strcmd);
        }

        /// <summary>
        /// 通知组队副本进入的消息
        /// </summary>
        /// <param name="roleIDsList"></param>
        /// <param name="minLevel"></param>
        /// <param name="maxLevel"></param>
        /// <param name="mapCode"></param>
        public void NotifyTeamFuBenEnterMsg(List<int> roleIDsList, int minLevel, int maxLevel, int leaderMapCode, int leaderRoleID, int fuBenID, int fuBenSeqID, int enterNumber, int maxFinishNum, bool igoreNumLimit = false)
        {
            if (null == roleIDsList || roleIDsList.Count <= 0) return;
            for (int i = 0; i < roleIDsList.Count; i++)
            {
                GameClient otherClient = GameManager.ClientMgr.FindClient(roleIDsList[i]);
                if (null == otherClient) continue; //不在线，则不通知

                //级别不匹配，则不通知
                int unionLevel = Global.GetUnionLevel(otherClient.ClientData.ChangeLifeCount, otherClient.ClientData.Level);
                if (unionLevel < minLevel || unionLevel > maxLevel)
                {
                    continue;
                }

                if (!igoreNumLimit)
                {
                    FuBenData fuBenData = Global.GetFuBenData(otherClient, fuBenID);
                    int nFinishNum;
                    int haveEnterNum = Global.GetFuBenEnterNum(fuBenData, out nFinishNum);
                    if ((enterNumber >= 0 && haveEnterNum >= enterNumber) || (maxFinishNum >= 0 && nFinishNum >= maxFinishNum))
                    {
                        continue;
                    }
                }

                //通知组队副本进入的消息
                GameManager.ClientMgr.NotifyTeamMemberFuBenEnterMsg(otherClient, leaderRoleID, fuBenID, fuBenSeqID);
            }
        }

        #endregion 组队相关

        #region 队伍查询

        /// <summary>
        /// 列举组队的队伍并返回列表
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="startIndex"></param>
        public void ListAllTeams(GameClient client, int startIndex, int sceneIndex)
        {
            CopySearchTeamData searchTeamData = new CopySearchTeamData()
            {
                StartIndex = startIndex,
                TotalTeamsCount = 0,
                PageTeamsCount = (int)SearchResultConsts.MaxSearchTeamsNum * 10,
                TeamDataList = null,
            };

            searchTeamData.TotalTeamsCount = GetTotalDataCount();
            if (searchTeamData.TotalTeamsCount <= 0)
            {
                SendListTeamsDataResult(client, searchTeamData);
                return;
            }

            if (startIndex >= searchTeamData.TotalTeamsCount)
            {
                startIndex = 0; //从0开始
            }

            searchTeamData.TeamDataList = GetTeamDataList(startIndex, searchTeamData.PageTeamsCount, sceneIndex, client.ClientData.CombatForce);
            SendListTeamsDataResult(client, searchTeamData);
        }

        /// <summary>
        /// 发送队伍列表的数据给客户端
        /// </summary>
        /// <param name="listRolesData"></param>
        private void SendListTeamsDataResult(GameClient client, CopySearchTeamData searchTeamData)
        {
            client.sendCmd<CopySearchTeamData>((int)TCPGameServerCmds.CMD_SPR_LISTCOPYTEAMS, searchTeamData);
        }

        #endregion 队伍查询
    }

    /// <summary>
    /// 副本组队监听器
    /// </summary>
    public class CopyTeamEventListener : IEventListener
    {
        private static CopyTeamEventListener instance = new CopyTeamEventListener();

        private CopyTeamEventListener() { }

        public static CopyTeamEventListener getInstance()
        {
            return instance;
        }

        public void processEvent(EventObject eventObject)
        {
            if (eventObject.getEventType() == (int)EventTypes.PlayerLeaveFuBen)
            {
                PlayerLeaveFuBenEventObject eventObj = (PlayerLeaveFuBenEventObject)eventObject;
                CopyTeamManager.getInstance().RoleLeaveFuBen(eventObj.getPlayer());
            }
            else if (eventObject.getEventType() == (int)EventTypes.PlayerInitGame)
            {
                //PlayerInitGameEventObject eventObj = (PlayerInitGameEventObject)eventObject;
                //CopyTeamManager.getInstance().OnPlayerInitGame(eventObj.getPlayer());
            }
            else if (eventObject.getEventType() == (int)EventTypes.PlayerLogout)
            {
                PlayerLogoutEventObject eventObj = (PlayerLogoutEventObject)eventObject;
                CopyTeamManager.getInstance().OnPlayerLogout(eventObj.getPlayer());
            }
        }
    }
}
