using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.Net.Sockets;
using Server.Protocol;
using System.IO;
using ProtoBuf;
using Server.Data;
using Server.TCP;
using Server.Tools;
//using System.Windows.Forms;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
using GameServer.Server;
using GameServer.Interface;
using GameServer.Logic.JingJiChang;
using GameServer.Core.Executor;

using GameServer.Logic.RefreshIconState;
using System.Threading;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.NewBufferExt;

namespace GameServer.Logic
{
    /// <summary>
    /// 地图爆怪管理类
    /// </summary>
    public class MonsterManager
    {
        #region 怪物只能寻敌GM控制

        /// <summary>
        /// 最小的寻敌的范围
        /// </summary>
        public static int MinSeekRangeMonsterLevel = 0;


        public void initialize(IEnumerable<XElement> mapItems)
        {
            MyMonsterContainer.initialize(mapItems);
        }

        /// <summary>
        /// 判断是否能够寻敌
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static bool CanMonsterSeekRange(Monster monster)
        {
            if (monster.MonsterType != (int)MonsterTypes.Noraml)
            {
                return true;
            }

            if (monster.MonsterInfo.VLevel > MinSeekRangeMonsterLevel)
            {
                return true;
            }

            return false;
        }

        #endregion 怪物只能寻敌GM控制

        #region 通知事件

        /// <summary>
        /// 循环中事件通知
        /// </summary>
        //public event EventHandler CycleExecute;

        #endregion 通知事件

        #region 基本属性和方法

        /// <summary>
        /// 怪物容器对象
        /// </summary>
        private MonsterContainer MyMonsterContainer = new MonsterContainer();

        /// <summary>
        /// 怪物移动算法对象
        /// </summary>
        private MonsterMoving monsterMoving = new MonsterMoving();

        /// <summary>
        /// 添加一个怪物到管理队列中
        /// </summary>
        /// <param name="monster"></param>
        public void AddMonster(Monster monster)
        {
            //添加到容器中
            MyMonsterContainer.AddObject(monster.RoleID, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster);
        }

        /// <summary>
        /// 将一个怪物从管理队列中删除(副本动态刷怪会用到)
        /// </summary>
        /// <param name="monster"></param>
        public void RemoveMonster(Monster monster)
        {
            //从容器中删除
            MyMonsterContainer.RemoveObject(monster.RoleID, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster);
        }

        /// <summary>
        /// 获取所有地图上的怪物的总的个数
        /// </summary>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        public int GetTotalMonstersCount()
        {
            return MyMonsterContainer.ObjectList.Count;
        }

        /// <summary>
        /// 获取指定地图上的怪物(可以选择是否排除自己)
        /// </summary>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        public List<Object> GetObjectsByMap(int mapCode)
        {
            return MyMonsterContainer.GetObjectsByMap(mapCode);
        }

        /// <summary>
        /// 获取指定地图上的怪物的个数
        /// </summary>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        public int GetMapMonstersCount(int mapCode)
        {
            return MyMonsterContainer.GetObjectsCountByMap(mapCode);
        }

        /// <summary>
        /// 根据副本的ID获取怪物列表
        /// </summary>
        /// <param name="copyMapID"></param>
        /// <returns></returns>
        public List<object> GetCopyMapIDMonsterList(int copyMapID)
        {
            return MyMonsterContainer.GetObjectsByCopyMapID(copyMapID);
        }

        /// <summary>
        /// 获取指定副本ID上的怪物的个数
        /// </summary>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        public int GetCopyMapIDMonstersCount(int copyMapID, int aliveType = -1)
        {
            return MyMonsterContainer.GetObjectsCountByCopyMapID(copyMapID, aliveType);
        }

        /// <summary>
        /// 副本中是否有怪的Alive标志为true
        /// </summary>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        public bool IsAnyMonsterAliveByCopyMapID(int copyMapID)
        {
            return MyMonsterContainer.IsAnyMonsterAliveByCopyMapID(copyMapID);
        }

        /// <summary>
        /// 通过ID查找一个客户端
        /// </summary>
        /// <param name="client"></param>
        public Monster FindMonster(int mapCode, int roleID)
        {
            object obj = MyMonsterContainer.FindObject(roleID, mapCode);
            return (obj as Monster);
        }

        /// <summary>
        /// 通过ExtensionID查找一个怪物
        /// </summary>
        /// <param name="client"></param>
        public List<object> FindMonsterByExtensionID(int copyMapID, int extensionID)
        {
            return MyMonsterContainer.FindObjectsByExtensionID(extensionID, copyMapID);
        }

        #endregion 基本属性和方法

        #region 查找指定范围内的怪物

        /// <summary>
        /// 查找指定圆周范围内的敌人
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void LookupEnemiesInCircle(int mapCode, int copyMapID, int toX, int toY, int radius, List<int> enemiesList)
        {
            List<Object> objList = new List<object>();
            LookupEnemiesInCircle(mapCode, copyMapID, toX, toY, radius, objList);
            for (int i = 0; i < objList.Count; i++)
            {
                enemiesList.Add((objList[i] as GameClient).ClientData.RoleID);
            }
        }

        /// <summary>
        /// 查找指定圆周范围内的敌人
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void LookupEnemiesInCircle(int mapCode, int copyMapID, int toX, int toY, int radius, List<Object> enemiesList)
        {
            MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
            List<Object> objList = mapGrid.FindObjects((int)toX, (int)toY, radius);
            if (null == objList) return;

            Point center = new Point(toX, toY);
            for (int i = 0; i < objList.Count; i++)
            {
                if (!(objList[i] is Monster))
                {
                    continue;
                }

                //不在同一个副本
                if (copyMapID != (objList[i] as Monster).CopyMapID)
                {
                    continue;
                }

                if (Global.InCircle((objList[i] as Monster).SafeCoordinate, center, (double)radius))
                {
                    enemiesList.Add((objList[i]));
                }
            }
        }

        /// <summary>
        /// 查找指定半圆周范围内的敌人
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapID, int toX, int toY, int radius, List<int> enemiesList, double angle, bool near180)
        {
            List<Object> objList = new List<Object>();
            LookupEnemiesInCircleByAngle(direction, mapCode, copyMapID, toX, toY, radius, objList, angle, near180);
            for (int i = 0; i < objList.Count; i++)
            {
                enemiesList.Add((objList[i] as Monster).RoleID);
            }
        }

        /// <summary>
        /// 查找指定半圆周范围内的敌人
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapID, int toX, int toY, int radius, List<Object> enemiesList, double angle, bool near180)
        {
            MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
            List<Object> objList = mapGrid.FindObjects((int)toX, (int)toY, radius);
            if (null == objList) return;

            double loAngle = 0.0, hiAngle = 0.0;
            Global.GetAngleRangeByDirection(direction, angle, out loAngle, out hiAngle);

            double loAngleNear = 0.0, hiAngleNear = 0.0;
            Global.GetAngleRangeByDirection(direction, 360, out loAngleNear, out hiAngleNear);

            Point center = new Point(toX, toY);
            for (int i = 0; i < objList.Count; i++)
            {
                if (!(objList[i] is Monster))
                {
                    continue;
                }

                //不在同一个副本
                if (copyMapID != (objList[i] as Monster).CopyMapID)
                {
                    continue;
                }

                if (Global.InCircleByAngle((objList[i] as Monster).SafeCoordinate, center, (double)radius, loAngle, hiAngle))
                {
                    enemiesList.Add((objList[i]));
                }
                //else if (Global.InCircleByAngle((objList[i] as Monster).SafeCoordinate, center, (double)200, loAngleNear, hiAngleNear))
                else if (Global.InCircle((objList[i] as Monster).SafeCoordinate, center, (double)100))
                {
                    enemiesList.Add((objList[i]));
                }
            }
        }


        // 增加扫描类型 矩形扫描 [11/27/2013 LiaoWei]
        /// <summary>
        /// 查找指定矩形范围内的怪
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void LookupRolesInSquare(GameClient client, int mapCode, int radius, int nWidth, List<Object> rolesList)
        {
            MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
            List<Object> objsList = mapGrid.FindObjects((int)client.ClientData.PosX, (int)client.ClientData.PosY, radius);
            if (null == objsList) return;

            // 源点
            Point source = new Point(client.ClientData.PosX, client.ClientData.PosY);

            Point toPos = Global.GetAPointInCircle(source, radius, client.ClientData.RoleYAngle);

            int toX = (int)toPos.X;
            int toY = (int)toPos.Y;

            // 矩形的中心点
            Point center = new Point();
            center.X = (client.ClientData.PosX + toX) / 2;
            center.Y = (client.ClientData.PosY + toY) / 2;

            // 矩形方向向量
            int fDirectionX = toX - client.ClientData.PosX;
            int fDirectionY = toY - client.ClientData.PosY;
            //Point center = new Point(toX, toY);

            for (int i = 0; i < objsList.Count; i++)
            {
                if (!(objsList[i] is Monster))
                    continue;

                if ((objsList[i] as Monster).VLife <= 0)
                    continue;

                // 不在同一个副本
                if (null != client && client.ClientData.CopyMapID != (objsList[i] as Monster).CopyMapID)
                    continue;

                Point target = new Point((objsList[i] as Monster).CurrentPos.X, (objsList[i] as Monster).CurrentPos.Y);
                
                if (Global.InSquare(center, target, radius, nWidth, fDirectionX, fDirectionY))
                    rolesList.Add(objsList[i]);
                else if (Global.InCircle(target, source, (double)100))  // 补充扫描
                    rolesList.Add((objsList[i])); 
            }
        }

        #endregion 查找指定范围内的怪物

        #region 查找指定格子内的怪物


        /// <summary>
        /// 查找指定格子内的敌人
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void LookupEnemiesAtGridXY(IObject attacker, int gridX, int gridY, List<Object> enemiesList)
        {
            MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[attacker.CurrentMapCode];

            List<Object> objsList = mapGrid.FindObjects((int)gridX, (int)gridY);
            if (null == objsList) return;

            for (int i = 0; i < objsList.Count; i++)
            {
                if (!(objsList[i] is Monster))
                {
                    continue;
                }

                //不在同一个副本
                if (attacker.CurrentCopyMapID != (objsList[i] as Monster).CopyMapID)
                {
                    continue;
                }

                enemiesList.Add(objsList[i]);
            }
        }

        /// <summary>
        /// 查找指定格子内的敌人
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void LookupAttackEnemies(IObject attacker, int direction, List<Object> enemiesList)
        {
            MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[attacker.CurrentMapCode];

            Point grid = attacker.CurrentGrid;
            int gridX = (int)grid.X;
            int gridY = (int)grid.Y;

            Point p = Global.GetGridPointByDirection(direction, gridX, gridY);

            //查找指定格子内的敌人
            LookupEnemiesAtGridXY(attacker, (int)p.X, (int)p.Y, enemiesList);
        }

        /// <summary>
        /// 查找指定格子内的敌人
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void LookupAttackEnemyIDs(IObject attacker, int direction, List<int> enemiesList)
        {
            List<Object> objList = new List<Object>();
            LookupAttackEnemies(attacker, direction, objList);
            for (int i = 0; i < objList.Count; i++)
            {
                enemiesList.Add((objList[i] as Monster).RoleID);
            }
        }

        /// <summary>
        /// 查找指定给子范围内的敌人
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void LookupRangeAttackEnemies(IObject obj, int toX, int toY, int direction, string rangeMode, List<Object> enemiesList)
        {
            MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];

            int gridX = toX / mapGrid.MapGridWidth;
            int gridY = toY / mapGrid.MapGridHeight;

            //根据传入的格子坐标和方向返回指定方向的格子列表
            List<Point> gridList = Global.GetGridPointByDirection(direction, gridX, gridY, rangeMode);
            if (gridList.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < gridList.Count; i++)
            {
                //查找指定格子内的敌人
                LookupEnemiesAtGridXY(obj, (int)gridList[i].X, (int)gridList[i].Y, enemiesList);
            }
        }

        /// <summary>
        /// 查找指定矩形范围内的玩家(矩形扫描)
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void LookupRolesInSquare(int mapCode, int copyMapId, int srcX, int srcY, int toX, int toY, int radius, int nWidth, List<Object> rolesList)
        {
            MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
            List<Object> objsList = mapGrid.FindObjects(srcX, srcY, radius);
            if (null == objsList) return;

            // 源点
            Point source = new Point(srcX, srcY);

            // 矩形的中心点
            Point center = new Point();
            center.X = (srcX + toX) / 2;
            center.Y = (srcY + toY) / 2;

            // 矩形方向向量
            int fDirectionX = toX - srcX;
            int fDirectionY = toY - srcY;

            for (int i = 0; i < objsList.Count; i++)
            {
                if (!(objsList[i] is GameClient))
                    continue;

                if ((objsList[i] as GameClient).ClientData.LifeV <= 0)
                    continue;

                // 不在同一个副本
                if (copyMapId != (objsList[i] as GameClient).ClientData.CopyMapID)
                    continue;

                Point target = new Point((objsList[i] as GameClient).ClientData.PosX, (objsList[i] as GameClient).ClientData.PosY);

                if (Global.InSquare(center, target, radius, nWidth, fDirectionX, fDirectionY))
                    rolesList.Add(objsList[i]);
            }
        }

        /// <summary>
        /// 查找指定半圆范围内的敌人
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapCode, int srcX, int srcY, int toX, int toY, int radius, List<Object> enemiesList, double angle, bool near180)
        {
            MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
            List<Object> objsList = mapGrid.FindObjects((int)toX, (int)toY, radius);
            if (null == objsList) return;

            double loAngle = 0.0, hiAngle = 0.0;
            Global.GetAngleRangeByDirection(direction, angle, out loAngle, out hiAngle);

            double loAngleNear = 0.0, hiAngleNear = 0.0;
            Global.GetAngleRangeByDirection(direction, 360, out loAngleNear, out hiAngleNear);

            Point center = new Point(toX, toY);
            for (int i = 0; i < objsList.Count; i++)
            {
                if (!(objsList[i] is GameClient))
                {
                    continue;
                }

                //不在同一个副本
                if (copyMapCode != (objsList[i] as GameClient).ClientData.CopyMapID)
                {
                    continue;
                }

                Point target = new Point((objsList[i] as GameClient).ClientData.PosX, (objsList[i] as GameClient).ClientData.PosY);
                if (Global.InCircleByAngle(target, center, (double)radius, loAngle, hiAngle))
                {
                    enemiesList.Add((objsList[i]));
                }
                else if (Global.InCircleByAngle(target, center, (double)200, loAngleNear, hiAngleNear))
                {
                    enemiesList.Add((objsList[i]));
                }
            }
        }

        /// <summary>
        /// 查找指定圆周范围内的敌人
        /// </summary>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void LookupEnemiesInCircle(int mapCode, int copyMapCode, int srcX, int srcY, int toX, int toY, int radius, List<Object> enemiesList)
        {
            MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
            List<Object> objsList = mapGrid.FindObjects((int)toX, (int)toY, radius);
            if (null == objsList) return;

            Point center = new Point(toX, toY);
            for (int i = 0; i < objsList.Count; i++)
            {
                if (!(objsList[i] is GameClient))
                {
                    continue;
                }

                //不在同一个副本
                if (copyMapCode != (objsList[i] as GameClient).ClientData.CopyMapID)
                {
                    continue;
                }

                Point target = new Point((objsList[i] as GameClient).ClientData.PosX, (objsList[i] as GameClient).ClientData.PosY);
                if (Global.InCircle(target, center, (double)radius))
                {
                    enemiesList.Add(objsList[i]);
                }
            }
        }
        #endregion 查找指定格子内的怪物

        #region 扩展属性和方法

        /// <summary>
        /// 将怪物数据发送到客户端列表
        /// </summary>
        /// <param name="client"></param>
        public void SendMonsterToClients(SocketListener sl, Monster monster, TCPOutPacketPool pool, List<Object> objList, int cmd)
        {
            if (null == objList) return;

            if (monster.VLife <= 0 || !monster.Alive)   
            {
                return;
            }

            MonsterData md = monster.GetMonsterData();

            //再次判断生命值
            if (md.LifeV <= 0)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("怪物 Role{0} 生命值为0， 不再发送", monster.RoleID));
                return;
            }

            //根据地图编号，将怪物数据发送到客户端列表
            for (int i = 0; i < objList.Count; i++)
            {
                if (!(objList[i] is GameClient))
                {
                    continue;
                }

                TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MonsterData>(md, pool, cmd);
                if (!sl.SendData((objList[i] as GameClient).ClientSocket, tcpOutPacket))
                {
                    /*LogManager.WriteLog(LogTypes.Error, string.Format("向用户发送tcp数据失败: ID={0}, Size={1}, RoleID={2}, RoleName={3}",
                        tcpOutPacket.PacketCmdID,
                        tcpOutPacket.PacketDataSize,
                        (objList[i] as GameClient).ClientData.RoleID,
                        (objList[i] as GameClient).ClientData.RoleName));*/
                }
            }
        }

        /// <summary>
        /// 将所有的怪物数据发送到指定的客户端
        /// </summary>
        /// <param name="client"></param>
        public int SendMonstersToClient(SocketListener sl, GameClient client, TCPOutPacketPool pool, List<Object> objList, int cmd)
        {
            if (null == objList) return 0;

            int totalCount = 0;

            //根据地图编号，将所有的怪都发送给客户端
            for (int i = 0; i < objList.Count && i < 50; i++ )
            {
                if (!(objList[i] is Monster))
                {
                    continue;
                }

                if (objList[i] is Robot)
                {
                    continue;
                }

                if ((objList[i] as Monster).VLife <= 0 || !(objList[i] as Monster).Alive)
                {
                    continue;
                }

                //System.Diagnostics.Debug.WriteLine(string.Format("九宫格: 发送新的怪物给客户端: {0}, {1}", (objList[i] as Monster).VSName, (objList[i] as Monster).Name));

                MonsterData md = (objList[i] as Monster).GetMonsterData();
                TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MonsterData>(md, pool, cmd);
                //TCPOutPacket tcpOutPacket = (objList[i] as Monster).GetTCPOutPacketFromCaching(cmd);

                if (!sl.SendData(client.ClientSocket, tcpOutPacket))
                {
                    /*LogManager.WriteLog(LogTypes.Error, string.Format("向用户发送tcp数据失败: ID={0}, Size={1}, RoleID={2}, RoleName={3}",
                        tcpOutPacket.PacketCmdID,
                        tcpOutPacket.PacketDataSize,
                        client.ClientData.RoleID,
                        client.ClientData.RoleName));*/
                    break;
                }

                totalCount++;
            }

            return totalCount;
        }

        /// <summary>
        /// 处理怪物死亡
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="client"></param>
        /// <param name="enemy"></param>
        public void ProcessMonsterDead(SocketListener sl, TCPOutPacketPool pool, IObject attacker, Monster monster, int enemyExperience, int enemyMoney, int injure)
        {
            if (attacker is GameClient)
            {
                ProcessMonsterDeadByClient(sl, pool, attacker as GameClient, monster, enemyExperience, enemyMoney);
            }
            else if (attacker is Monster)
            {
                ProcessMonsterDeadByMonster(sl, pool, attacker as Monster, monster, enemyExperience, enemyMoney);
            }

            if ((int)(Math.Pow(2, 31) - 1) != injure)
            {
                if ((int)MonsterTypes.DSPetMonster == monster.MonsterType && null != monster.OwnerClient)
                {
                    GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                        monster.OwnerClient, StringUtil.substitute(Global.GetLang("您的{0}被杀死了，请使用技能重新召唤"), monster.MonsterInfo.VSName), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, (int)HintErrCodeTypes.CallAutoSkill);
                }

                //int decoID = Global.GetRandomNumber(520, 525);

                //添加死亡特效
                //DecorationManager.AddDecoToMap(monster.MonsterZoneNode.MapCode, monster.CopyMapID,
                //    monster.SafeCoordinate, decoID, 3 * 1000, 1 * 1000, true);
            }
        }   

        /// <summary>
        /// 处理怪物死亡(被玩家杀死)
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="client"></param>
        /// <param name="enemy"></param>
        private void ProcessMonsterDeadByClient(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster, int enemyExperience, int enemyMoney)
        {
            if (monster.HandledDead)
            {
                return;
            }

            monster.HandledDead = true;

            // 天使神殿 记住击杀BOSS的玩家
            GameManager.AngelTempleMgr.KillAngelBoss(client, monster);

            //0无归属;1掉落物品归属为为仇恨列表最高者(通过修改击杀者实现);2归属为击杀者
            if (monster.MonsterInfo.FallBelongTo == 1)
            {
                int ownerRoleID = monster.GetAttackerFromList(); //根据血量计算
                if (ownerRoleID >= 0 && ownerRoleID != client.ClientData.RoleID)
                {
                    GameClient findClient = GameManager.ClientMgr.FindClient(ownerRoleID);
                    if (null != findClient)
                    {
                        client = findClient;
                    }
                }
            }

            bool isTeamSharingMap = true;
            if (//client.ClientData.MapCode == GameManager.BattleMgr.BattleMapCode || 
                client.ClientData.MapCode == GameManager.ArenaBattleMgr.BattleMapCode) //角斗场模式下，不进行组队分配 但炎黄战场允许
            {
                isTeamSharingMap = false;
            }

            TeamData td = null;
            if (client.ClientData.TeamID > 0 && isTeamSharingMap)
            {
                //查找组队数据
                td = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
            }

            if (null == td)
            {
                //处理角色杀死怪物的经验和任务处理
                ProcessSpriteKillMonster(sl, pool, client, monster, enemyExperience, enemyMoney);
            }
            else
            {
                int totalTeamMemberNum = 0;
                //int totalLevel = 0;
                List<GameClient> clientsList = new List<GameClient>();

                //锁定组队数据
                lock (td)
                {
                    for (int i = 0; i < td.TeamRoles.Count; i++)
                    {
                        if (td.TeamRoles[i].RoleID == client.ClientData.RoleID)
                        {
                            totalTeamMemberNum += 1;
                            //totalLevel += client.ClientData.Level;
                            clientsList.Add(client);
                            continue;
                        }

                        //不在同一个地图上不参与分配
                        GameClient gc = GameManager.ClientMgr.FindClient(td.TeamRoles[i].RoleID);
                        if (null == gc)
                        {
                            continue;
                        }

                        //如果不在同一个地图上，则不处理
                        if (gc.ClientData.MapCode != client.ClientData.MapCode)
                        {
                            continue;
                        }

                        //如果不在同一个副本地图上，则不处理
                        if (gc.ClientData.CopyMapID != client.ClientData.CopyMapID)
                        {
                            continue;
                        }

                        //如果对方具体则不处理
                        if (!Global.InCircle(new Point(gc.ClientData.PosX, gc.ClientData.PosY), monster.SafeCoordinate, 4000))
                        {
                            continue;
                        }

                        totalTeamMemberNum += 1;
                        //totalLevel += gc.ClientData.Level;
                        clientsList.Add(gc);
                    }
                }

                //只有组队后，在附近的队员超过1个的情况下，才会参与分配
                if (clientsList.Count >= 1)
                {
                    //根据个数分配经验
                    //int realExperience = (int)(enemyExperience * (1.0 + 0.10 * (clientsList.Count - 1)));
                    int shareExperience = (int)(enemyExperience * (clientsList.Count - 1) * 0.1) / clientsList.Count;
                    //int singleMoney = enemyMoney / clientsList.Count;
                    //int singleExperience = realExperience / clientsList.Count;
                    for (int i = 0; i < clientsList.Count; i++)
                    {
                        //int singleExperience = (int)(clientsList[i].ClientData.Level * (realExperience / (double)totalLevel));

                        //处理角色杀死怪物的经验和任务处理
                        //ProcessSpriteKillMonster(sl, pool, clientsList[i], monster, singleExperience, 0);
                        if (client == clientsList[i]) //是否击杀者
                        {
                            ProcessSpriteKillMonster(sl, pool, clientsList[i], monster, enemyExperience + shareExperience, 0);
                        }
                        else
                        {
                            ProcessSpriteKillMonster(sl, pool, clientsList[i], monster, shareExperience, 0);
                        }
                    }
                }
                else
                {
                    //处理角色杀死怪物的经验和任务处理
                    ProcessSpriteKillMonster(sl, pool, client, monster, enemyExperience, enemyMoney);
                }
            }

            //记录是谁杀死了怪物
            monster.WhoKillMeID = client.ClientData.RoleID;
            monster.WhoKillMeName = Global.FormatRoleName(client, client.ClientData.RoleName);

            // 检查是否帮会副本的bossid
            // monster.MonsterInfo.ExtensionID
            GameManager.GuildCopyMapMgr.ProcessMonsterDead(client, monster);

            List<int> attackerList = monster.GetAttackerList(); //摸着怪物就算
            for (int i = 0; i < attackerList.Count; i++)
            {
                if (client.ClientData.RoleID == attackerList[i])
                {
                    continue;
                }

                //不在同一个地图上不参与分配
                GameClient gc = GameManager.ClientMgr.FindClient(attackerList[i]);
                if (null == gc)
                {
                    continue;
                }

                //如果不在同一个地图上，则不处理
                if (gc.ClientData.MapCode != client.ClientData.MapCode)
                {
                    continue;
                }

                //如果不在同一个副本地图上，则不处理
                if (gc.ClientData.CopyMapID != client.ClientData.CopyMapID)
                {
                    continue;
                }

                //如果对方具体则不处理
                if (!Global.InCircle(new Point(gc.ClientData.PosX, gc.ClientData.PosY), monster.SafeCoordinate, 500))
                {
                    continue;
                }

                // 处理任务
                ProcessTask.Process(sl, pool, gc, monster.RoleID, monster.MonsterInfo.ExtensionID, -1, TaskTypes.KillMonster);
            }

            //处理掉落
            GameManager.GoodsPackMgr.ProcessMonster(sl, pool, client, monster);

            //通知连斩值更新(限制当前地图)
            GameManager.ClientMgr.ChangeRoleLianZhan(sl, pool, client, monster);

            //谁Kill了Boss
            Global.BroadcastXKilledMonster(client, monster);

            //更新杀BOSS的数量
            GameManager.ClientMgr.UpdateKillBoss(client, 1, monster, false);

            /// 增加大乱斗中杀死的敌人的数量  // 阵营战场改造 [12/23/2013 LiaoWei]
            Global.AddBattleKilledNum(client, monster, monster.MonsterInfo.BattlePersonalJiFen, monster.MonsterInfo.BattleZhenYingJiFen);

            /// 当角色杀死怪物时需要额外处理的事件
            ProcessOtherEventsOnMonsterDead(sl, pool, client, monster);

            /// 处理怪物死亡的事件
            ProcessMonsterDeadEvents(sl, pool, client, monster);

            /// 处理BOSS死亡时的图标状态刷新
            if ((int)MonsterTypes.Boss == monster.MonsterType)
            {
                TimerBossManager.getInstance().RemoveBoss(monster.MonsterInfo.ExtensionID);
            }

            /// 成就系统处理 每日活跃处理 [3/5/2014 LiaoWei]
            if (client.ClientData.IsFlashPlayer != 1 && client.ClientData.MapCode != (int)FRESHPLAYERSCENEINFO.FRESHPLAYERMAPCODEID)
            {
                ChengJiuManager.OnMonsterKilled(client, monster);

                DailyActiveManager.ProcessDailyActiveKillMonster(client, monster);
            }
            

            ///处理替身娃娃的经验
            DBRoleBufferManager.ProcessWaWaGiveExperience(client, monster);

            // 血色堡垒杀怪处理 [11/8/2013 LiaoWei]
            //if (Global.IsBloodCastleSceneID(client.ClientData.MapCode))
            //    GameManager.BloodCastleMgr.KillMonsterABloodCastScene(client, monster);

            // 恶魔广场杀怪处理 [11/8/2013 LiaoWei]
            //if (Global.IsDaimonSquareSceneID(client.ClientData.MapCode))
            //    DaimonSquareSceneManager.DaimonSquareSceneKillMonster(client, monster);

            // 新手场景杀怪处理 [12/1/2013 LiaoWei]
            if (client.ClientData.MapCode == (int)FRESHPLAYERSCENEINFO.FRESHPLAYERMAPCODEID)
                FreshPlayerCopySceneManager.KillMonsterInFreshPlayerScene(client, monster);

            // 经验副本杀怪处理 [3/18/2014 LiaoWei]
            if (Global.IsInExperienceCopyScene(client.ClientData.MapCode))
                ExperienceCopySceneManager.ExperienceCopyMapKillMonster(client, monster);

            // 血色城堡副本杀怪处理 [7/8/2014 LiaoWei]
            if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(client.ClientData.FuBenID))
            {
                GameManager.BloodCastleCopySceneMgr.KillMonsterABloodCastCopyScene(client, monster);
            }

            // 恶魔广场副本杀怪处理 [7/11/2014 LiaoWei]
            if (GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(client.ClientData.FuBenID))
            {
                GameManager.DaimonSquareCopySceneMgr.DaimonSquareSceneKillMonster(client, monster);
            }

            // 卓越属性 [12/27/2013 LiaoWei]
            /*if (client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP6] != 0) // 击杀怪物回血 
                client.ClientData.CurrentLifeV += (int)client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP6];

            if (client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP7] != 0) // 击杀怪物回蓝 
                client.ClientData.CurrentMagicV += (int)client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP7];*/

        }

        /// <summary>
        /// 处理怪物死亡(被其他怪物杀死)
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="client"></param>
        /// <param name="enemy"></param>
        private void ProcessMonsterDeadByMonster(SocketListener sl, TCPOutPacketPool pool, Monster attacker, Monster monster, int enemyExperience, int enemyMoney)
        {
            if (monster.HandledDead)
            {
                return;
            }

            //判断攻击者的召唤主人是谁?
            if (null != attacker.OwnerClient)
            {
                ProcessMonsterDeadByClient(sl, pool, attacker.OwnerClient, monster, enemyExperience, enemyMoney);
                return;
            }

            //必须在这儿调用，否则 ProcessMonsterDeadByClient 函数内部不会处理 !!!!!
            monster.HandledDead = true;

            //处理掉落
            GameManager.GoodsPackMgr.ProcessMonster(sl, pool, attacker, monster);

            /// 处理怪物死亡的事件
            ProcessMonsterDeadEvents(sl, pool, attacker, monster);
        }

        /// <summary>
        /// 处理角色杀死怪物的经验和任务处理
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="client"></param>
        /// <param name="enemy"></param>
        private void ProcessSpriteKillMonster(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster, int enemyExperience, int enemyMoney)
        {
            int oldLevel = client.ClientData.Level;

            //经验计算公式
            //怪物基础经验*（1-abs（怪物等级-角色等级）*5%）
            //最新传奇版本的计算公式:
            //超过10级之后每级衰减经验4.5%
            //越级打怪无加成

            // 经验衰减注释掉 等待策划新的衰减公式 [12/9/2013 LiaoWei]
            //int overlevel = client.ClientData.Level - monster.MonsterInfo.VLevel;
            int overlevel = 0; //禁用经验和金币收益衰减
            /*if (overlevel > 10) //如果超过10级范围外
            {
                enemyExperience = (int)(enemyExperience * (1.0 - (overlevel - 10) * 0.045));
                enemyExperience = Global.GMax(0, enemyExperience);
            }*/

            int origEnemyExperience = enemyExperience;

            //处理双倍经验的buffer
            double dblExperience = DBRoleBufferManager.ProcessDblAndThreeExperience(client);
            if (SpecailTimeManager.JugeIsDoulbeExperienceAndLingli())
            {
                dblExperience += 1.0;
            }

            // 活动时间内 && 非副本怪
            HeFuAwardTimesActivity activity = HuodongCachingMgr.GetHeFuAwardTimesActivity();
            if (null != activity && activity.InActivityTime()
                && monster.CopyMapID <= 0)
            {
                // 检查是否在活动子时间段内
                if (activity.activityTimes > 0.0 && SpecailTimeManager.InSpercailTime(activity.specialTimeID))
                    dblExperience += (activity.activityTimes - 1.0);
            }

            //判断怪物所在的领地所辖的地图的经验加成比例
            dblExperience += Global.ProcessLingDiMonsterExperience(client, monster);

            BufferData bufferData = Global.GetBufferDataByID(client, (int)BufferItemTypes.MU_SPECMACH_EXP);
            if (null != bufferData && !Global.IsBufferDataOver(bufferData))
            {
                dblExperience += ((double)bufferData.BufferVal / 100.0);
            }

            //处理VIP月卡
            if (DBRoleBufferManager.ProcessMonthVIP(client) > 0.0)
            {
                double gumuExp = Global.GetVipGuMuExperienceMultiple(client);
                dblExperience += gumuExp;
            }

            //PK 王  MU 项目取消
            /*double pkWangExp = DBRoleBufferManager.ProcessTimeAddPkKingExpProp(client);
            dblExperience += pkWangExp;*/

            //判断怪物所在的领地所辖的地图的经验加成比例
            dblExperience += Global.ProcessLingDiMonsterExperience(client, monster);

            enemyExperience = (int)(enemyExperience * dblExperience);

            int nWorldLevelAddExp = 0;
            if (Global.CanMapUseBuffer(client.ClientData.GetRoleData().MapCode, (int)BufferItemTypes.MU_WORLDLEVEL))
            {
                if (client.ClientData.nTempWorldLevelPer > 0)
                {
                    nWorldLevelAddExp = (int)(origEnemyExperience * client.ClientData.nTempWorldLevelPer);
                }
            }

            enemyExperience += nWorldLevelAddExp;

            //处理角色经验
            GameManager.ClientMgr.ProcessRoleExperience(client, enemyExperience, true, false);

            //铜钱计算公式
            //怪物基础铜钱*（1-abs（怪物等级-角色等级）*5%）
            if (overlevel > 10) //如果超过10级范围外
            {
                enemyMoney = (int)(enemyMoney * (1.0 - (overlevel - 10) * 0.045));
                enemyMoney = Global.GMax(0, enemyMoney);
            }

            //过滤金币奖励
            enemyMoney = Global.FilterValue(client, enemyMoney);

            //是否获取金钱
            if (enemyMoney > 0)
            {
                //处理双倍金币的Buffer
                enemyMoney = (int)(enemyMoney * DBRoleBufferManager.ProcessDblAndThreeMoney(client));

                GameManager.ClientMgr.AddMoney1(sl, Global._TCPManager.tcpClientPool, pool, client, enemyMoney, "杀死怪物", false);
            }

            // 处理任务
            ProcessTask.Process(sl, pool, client, monster.RoleID, monster.MonsterInfo.ExtensionID, -1, TaskTypes.KillMonster);

            //处理副本杀死怪物的个数
            GameManager.CopyMapMgr.ProcessKilledMonster(client, monster);
        }

        /// <summary>
        /// 处理怪物死亡的事件---->这个函数在处理伤害的函数内部线程调用，意味着应该在通用客户端数据包处理线程内部调用
        /// 这个函数被调用之后，怪物的DeathAction 才会被设置，由此，经过一个比较长的时间，isDeath 才是true，之后再进过
        /// 一个时间，Alive 才会是false, 重新爆怪 需要 Alive 为false，这导致某些时候怪物刷不出来
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        public void ProcessMonsterDeadEvents(SocketListener sl, TCPOutPacketPool pool, IObject attacker, Monster monster)
        {
            if ((int)MonsterTypes.ShengXiaoYunCheng == monster.MonsterType) //如果是生肖运程怪物，则特殊处理事件
            {
                GameManager.ShengXiaoGuessMgr.OnBossKilled();
            }
        }

        /// <summary>
        /// 切换在地图上的位置
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="client"></param>
        /// <param name="toMapX"></param>
        /// <param name="toMapY"></param>
        /// <param name="toMapDirection"></param>
        /// <param name="nID"></param>
        /// <returns></returns>
        public bool ChangePosition(SocketListener sl, TCPOutPacketPool pool, Monster monster, int toMapX, int toMapY, int toMapDirection, int nID, int animation = 0)
        {
            //此处不用做互斥，因为已经将客户端从队列中拿出了, 客户端切换地图时一定要启用阻塞的操作，防止用户再操作
            if (toMapDirection > 0)
            {
                monster.Direction = toMapDirection;
            }

            //失去锁定对象
            monster.LastSeekEnemyTicks = (DateTime.Now.Ticks / 10000);
            monster.VisibleItemList = null;
            monster.LockObject = -1;
            monster.LockFocusTime = 0;

            monster.Coordinate = new Point(toMapX, toMapY);

            List<Object> objsList = Global.GetAll9Clients(monster);
            if (null == objsList) return true;

            string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", monster.RoleID, toMapX, toMapY, toMapDirection, animation);
            GameManager.ClientMgr.SendToClients(sl, pool, null, objsList, strcmd, nID);

            //System.Diagnostics.Debug.WriteLine(string.Format("MonsterManager ChangePosition, toMapX={0}, toMapY={1}, toGridX={2}, toGridY={3}", toMapX, toMapY, monster.CurrentGrid.X, monster.CurrentGrid.Y));

            if (monster._Action != GActions.Stand)
            {
                //通知其他人自己开始做动作
                List<Object> listObjs = objsList;
                GameManager.ClientMgr.NotifyOthersDoAction(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeDirection, (int)GActions.Stand, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, 0, 0, (int)TCPGameServerCmds.CMD_SPR_ACTTION, listObjs);

                //本地动作
                //monster.MoveToPos = new Point(-1, -1); //防止重入
                monster.DestPoint = new Point(-1, -1);
                Global.RemoveStoryboard(monster.Name);
                monster.Action = GActions.Stand;
            }

            return true;
        }

        /// <summary>
        /// 当角色杀死怪物时需要额外处理的事件
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        public void ProcessOtherEventsOnMonsterDead(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster)
        {
            //处理随机的真气值
            //if (monster.ZhenQiMinValue != monster.ZhenQiMaxValue && monster.ZhenQiMaxValue > 0)
            //{
            //    int randZhenQiVal = Global.GetRandomNumber(monster.ZhenQiMinValue, monster.ZhenQiMaxValue);
            //    if (randZhenQiVal > 0)
            //    {
            //        GameManager.ClientMgr.ModifyZhenQiValue(client, randZhenQiVal, false, true);
            //    }
            //}
        }

        #endregion 扩展属性和方法

        #region 怪物加减血

        /// <summary>
        /// 给某个客户端加血
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="addedVal"></param>
        public void AddSpriteLifeV(SocketListener sl, TCPOutPacketPool pool, Monster monster, double lifeV)
        {
            //如果已经死亡，则不再调度
            if (monster.VLife <= 0)
            {
                return;
            }

            //判断如果血量少于最大血量
            if (monster.VLife < monster.MonsterInfo.VLifeMax)
            {
                monster.VLife = (int)Global.GMin(monster.MonsterInfo.VLifeMax, monster.VLife + lifeV);

                //GameManager.SystemServerEvents.AddEvent(string.Format("怪物加血, roleID={0}({1}), Add={2}, Life={3}", monster.RoleID, monster.VSName, lifeV, monster.VLife), EventLevels.Debug);

                //通知客户端怪已经加血加魔                                    
                List<Object> listObjs = Global.GetAll9Clients(monster);
                GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.Direction, monster.VLife, monster.VMana, (int)TCPGameServerCmds.CMD_SPR_RELIFE, listObjs);
            }
        }

        /// <summary>
        /// 给某个客户端减血
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="addedVal"></param>
        public void SubSpriteLifeV(SocketListener sl, TCPOutPacketPool pool, Monster monster, double lifeV)
        {
            //如果已经死亡，则不再调度
            if (monster.VLife <= 0)
            {
                return;
            }

            //判断如果血量少于最大血量
            if (monster.VLife < monster.MonsterInfo.VLifeMax)
            {
                monster.VLife = (int)Global.GMax(0, monster.VLife - lifeV);

                //GameManager.SystemServerEvents.AddEvent(string.Format("怪物减血, roleID={0}({1}), Add={2}, Life={3}", monster.RoleID, monster.VSName, lifeV, monster.VLife), EventLevels.Debug);

                //通知客户端怪已经加血加魔                                    
                List<Object> listObjs = Global.GetAll9Clients(monster);
                GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.Direction, monster.VLife, monster.VMana, (int)TCPGameServerCmds.CMD_SPR_RELIFE, listObjs);
            }
        }

        #endregion 怪物加减血

        #region 怪物加减魔

        /// <summary>
        /// 给某个客户端加魔
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="addedVal"></param>
        public void AddSpriteMagicV(SocketListener sl, TCPOutPacketPool pool, Monster monster, double magicV)
        {
            //如果已经死亡，则不再调度
            if (monster.VLife <= 0)
            {
                return;
            }

            monster.VMana = (int)Global.GMin(monster.MonsterInfo.VManaMax, monster.VMana + magicV);

            //GameManager.SystemServerEvents.AddEvent(string.Format("角色加魔, roleID={0}({1}), Sub={2}, Magic={3}", monster.RoleID, monster.VSName, magicV, monster.VMana), EventLevels.Debug);

            //通知客户端怪已经加血加魔                                    
            List<Object> listObjs = Global.GetAll9Clients(monster);
            GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.Direction, monster.VLife, monster.VMana, (int)TCPGameServerCmds.CMD_SPR_RELIFE, listObjs);

        }

        /// <summary>
        /// 给某个客户端减魔
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="addedVal"></param>
        public void SubSpriteMagicV(SocketListener sl, TCPOutPacketPool pool, Monster monster, double magicV)
        {
            //如果已经死亡，则不再调度
            if (monster.VLife <= 0)
            {
                return;
            }

            monster.VMana = (int)Global.GMax(0.0, monster.VMana - magicV);

            //GameManager.SystemServerEvents.AddEvent(string.Format("角色减魔, roleID={0}({1}), Sub={2}, Magic={3}", monster.RoleID, monster.VSName, magicV, monster.VMana), EventLevels.Debug);

            //通知客户端怪已经加血加魔                                    
            List<Object> listObjs = Global.GetAll9Clients(monster);
            GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.Direction, monster.VLife, monster.VMana, (int)TCPGameServerCmds.CMD_SPR_RELIFE, listObjs);
        }

        #endregion 怪物加减魔

        #region 伤害转化

        /// <summary>
        /// 将对敌人的伤害进行处理
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="client"></param>
        /// <param name="injured"></param>
        public int InjureToEnemy(Monster monster, int injured)
        {
            injured = DBMonsterBuffer.ProcessHuZhaoSubLifeV(monster, Math.Max(0, injured));
            injured = DBMonsterBuffer.ProcessWuDiHuZhaoNoInjured(monster, Math.Max(0, injured));

            return Math.Max(0, injured);
        }

        #endregion 伤害转化

        #region 角色攻击怪物计算

        /// <summary>
        /// 通知其他人被攻击(单攻)，并且被伤害(同一个地图才需要通知)
        /// </summary>
        /// <param name="client"></param>
        public int NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, int skillLevel, double skillBaseAddPercent, double skillUpAddPercent, bool ignoreDefenseAndDodge = false, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
        {
            int ret = 0;
            object obj = enemy;
            {
                //怪物必须或者才操作
                if ((obj as Monster).VLife > 0 && (obj as Monster).Alive)
                {
                    bool selfLifeChanged = false;
                    if (injure <= 0)
                    {
                        /// 角色攻击怪的计算公式
                        if (0 == attackType) //物理攻击
                        {
                            RoleAlgorithm.AttackEnemy(client, (obj as Monster), forceBurst, injurePercent, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue);
                        }
                        else if (1 == attackType) //魔法攻击
                        {
                            RoleAlgorithm.MAttackEnemy(client, (obj as Monster), forceBurst, injurePercent, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue);
                        }
                        else //道术攻击
                        {
                            // 属性改造 去掉 道术攻击[8/15/2013 LiaoWei]
                            //RoleAlgorithm.DSAttackEnemy(client, (obj as Monster), forceBurst, injurePercent, addInjure, attackPercent, addAttack, out burst, out injure, ignoreDefenseAndDodge);
                        }
                    }

                    /*if (!Global.InCircle(new Point((obj as Monster).SafeCoordinate.X, (obj as Monster).SafeCoordinate.Y), new Point(client.ClientData.PosX, client.ClientData.PosY), Data.MaxAttackDistance)) //如果敌人已经离开了攻击点半径则视为闪避
                    {
                        System.Diagnostics.Debug.WriteLine("GetRealHitRate{0}, out of circle, radius", 0);
                        injure = 0;
                    }*/

                    if (injure > 0)
                    {
                        int lifeSteal = (int)RoleAlgorithm.GetLifeStealV(client);
                        if (lifeSteal > 0 && client.ClientData.CurrentLifeV < client.ClientData.LifeV)
                        {
                            selfLifeChanged = true;
                            client.ClientData.CurrentLifeV += lifeSteal;
                        }
                        if (client.ClientData.CurrentLifeV > client.ClientData.LifeV)
                        {
                            client.ClientData.CurrentLifeV = client.ClientData.LifeV;
                        }
                    }

                    //将对敌人的伤害进行处理
                    injure = InjureToEnemy((obj as Monster), injure);

                    //处理BOSS克星
                    injure = DBRoleBufferManager.ProcessAntiBoss(client, (obj as Monster), injure);

                    // 竞技场的伤害为50% ChenXiaojun
                    if (obj is Robot)
                    {
                        injure /= 2;
                    }

                    ret = injure;

                    double nTemp = (obj as Monster).VLife;

                    (obj as Monster).VLife -= (int)Global.GMax(0, injure); //是否需要锁定
                    (obj as Monster).VLife = Global.GMax((obj as Monster).VLife, 0.0);
                    double enemyLife = (obj as Monster).VLife;

                    GlobalEventSource.getInstance().fireEvent(new MonsterBlooadChangedEventObject((obj as Monster)));

                    if (nTemp >= (obj as Monster).MonsterInfo.VLifeMax) //第一次受伤的事件触发
                    {
                        GlobalEventSource.getInstance().fireEvent(new MonsterInjuredEventObject((obj as Monster), client));
                    }

                    // 卓越属性 有几率完全恢复血和蓝 [12/27/2013 LiaoWei]
                    if (client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP15] > 0.0)
                    {
                        int nRan = Global.GetRandomNumber(0, 101);
                        if (nRan <= client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP15] * 100)
                            client.ClientData.CurrentLifeV = client.ClientData.LifeV;
                    }

                    if (client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP16] > 0.0)
                    {
                        int nRan = Global.GetRandomNumber(0, 101);
                        if (nRan <= client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP16] * 100)
                            client.ClientData.CurrentMagicV = client.ClientData.MagicV;
                    }

                    //bool hitFly = (injure > ((obj as Monster).VLifeMax / 3));
                    bool hitFly = false; // (6 == client.ClientData.MapCode / 1000); //如果是血色城堡系列
                    if ((obj as Monster).VLife <= 0.0)
                    {
                        hitFly = true; // (0 == Global.GetRandomNumber(0, 2));
                        //if (!hitFly)
                        //{
                        //    hitFly = (6 == client.ClientData.MapCode / 1000); //如果是血色城堡系列
                        //}
                    }

                    Point hitToGrid = new Point(-1, -1);
                    if (hitFly/* || (0 == Global.GetRandomNumber(0, 5))*/)
                    {
                        hitToGrid = ChuanQiUtils.HitFly(client, (obj as Monster), (obj as Monster).VLife <= 0 ? 2 : 1);
                        if ((int)hitToGrid.X > 0 && (int)hitToGrid.Y > 0)
                        {
                            //(obj as Monster).LastFatalInjuredTicks = DateTime.Now.Ticks / 10000;
                        }
                    }

                    // 击飞处理 [3/15/2014 LiaoWei]
                    if (!hitFly && nHitFlyDistance > 0)
                    {
                        MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode];

                        int nGridNum = nHitFlyDistance*100 / mapGrid.MapGridWidth;

                        if (nGridNum > 0)
                            hitToGrid = ChuanQiUtils.HitFly(client, enemy, nGridNum);
                    }

                    int nValue = 0;
                    if ((obj as Monster).VLife == 0.0)
                        nValue = (int)nTemp;
                    else
                        nValue = injure;

                    // 天使神殿处理 [3/23/2014 LiaoWei]
                    if (client.ClientData.MapCode == GameManager.AngelTempleMgr.m_AngelTempleData.MapCode)
                    {
                        GameManager.AngelTempleMgr.ProcessAttackBossInAngelTempleScene(client, enemy, nValue);
                    }

                    if (Global.IsInTeamCopyScene(client.ClientData.MapCode))
                    {
                        CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.CopyMapID);
                        Interlocked.Add(ref client.SumDamageForCopyTeam, nValue);
                    }
                    if (GameManager.GuildCopyMapMgr.IsGuildCopyMap(client.ClientData.MapCode))
                    {
                        CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.CopyMapID);
                        Interlocked.Add(ref client.SumDamageForCopyTeam, nValue);
                    }

                    //判断是否将给敌人的伤害转化成自己的血量增长
                    GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);

                    //将攻击者加入历史列表
                    (obj as Monster).AddAttacker(client.ClientData.RoleID, Global.GMax(0, injure));

                    //记录角色攻击目标
                    client.ClientData.RoleIDAttackebByMyself = (obj as Monster).RoleID;

                    //攻击就取消隐身
                    if (client.ClientData.DSHideStart > 0)
                    {
                        Global.RemoveBufferData(client, (int)BufferItemTypes.DSTimeHideNoShow);
                        client.ClientData.DSHideStart = 0;
                        GameManager.ClientMgr.NotifyDSHideCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
                    }

                    //GameManager.SystemServerEvents.AddEvent(string.Format("怪物减血, Injure={0}, Life={1}", injure, enemyLife), EventLevels.Debug);

                    //int enemyExperience = 0;

                    //判断怪物是否死亡
                    if ((obj as Monster).VLife <= 0.0)
                    {
                        // 注释掉 利用接口 [12/27/2013 LiaoWei]
                        /*enemyExperience = (obj as Monster).MonsterZoneNode.VExperience;

                        //锁定攻击自己的敌人
                        (obj as Monster).LockObject = -1;
                        (obj as Monster).LockFocusTime = 0;

                        //GameManager.SystemServerEvents.AddEvent(string.Format("怪物死亡, roleID={0}", (obj as Monster).RoleID), EventLevels.Debug);

                        /// 处理怪物死亡
                        ProcessMonsterDead(sl, pool, client, (obj as Monster), enemyExperience, (obj as Monster).MonsterZoneNode.VMoney, injure);

                        // 转入界面线程
                        //System.Windows.Application.Current.Dispatcher.BeginInvoke((MethodInvoker)delegate
                        //{
                        //    (obj as Monster).MoveToPos = new Point(-1, -1); //防止重入
                        //    (obj as Monster).DestPoint = new Point(-1, -1);
                        //    Global.RemoveStoryboard((obj as Monster).Name);
                        //    (obj as Monster).Action = GActions.Death;
                        //});
                        AddDelayDeadMonster(obj as Monster);*/

                        Global.ProcessMonsterDieForRoleAttack(sl, pool, client, enemy, injure);
                    }
                    else
                    {
                        //锁定攻击自己的敌人
                        if ((obj as Monster).LockObject < 0)
                        {
                            (obj as Monster).LockObject = client.ClientData.RoleID;
                            (obj as Monster).LockFocusTime = DateTime.Now.Ticks / 10000;
                        }
                    }

                    int ownerRoleID = (obj as Monster).GetAttackerFromList();
                    if (ownerRoleID >= 0 && ownerRoleID != client.ClientData.RoleID)
                    {
                        GameClient findClient = GameManager.ClientMgr.FindClient(ownerRoleID);
                        if (null != findClient)
                        {
                            //通知其他在线客户端
                            GameManager.ClientMgr.NotifySpriteInjured(sl, pool, findClient, findClient.ClientData.MapCode, findClient.ClientData.RoleID, (obj as Monster).RoleID, 0, 0, enemyLife, findClient.ClientData.Level, hitToGrid);

                            //向自己发送敌人受伤的信息
                            ClientManager.NotifySelfEnemyInjured(sl, pool, findClient, findClient.ClientData.RoleID, enemy.RoleID, 0, 0, enemyLife, 0);
                        }
                    }

                    //通知其他在线客户端
                    GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (obj as Monster).RoleID, burst, injure, enemyLife, client.ClientData.Level, hitToGrid);

                    //向自己发送敌人受伤的信息
                    ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, enemy.RoleID, burst, injure, enemyLife, 0);

                    // 反射伤害处理 [7/3/2014 LiaoWei]
                    Global.ProcessDamageThorn(sl, pool, client, enemy, injure);

                    if (selfLifeChanged)
                    {
                        GameManager.ClientMgr.NotifyOthersLifeChanged(sl, pool, client);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// 通知其他人被攻击(群攻)，并且被伤害(同一个地图才需要通知)
        /// </summary>
        /// <param name="client"></param>
        public void NotifyInjuredEx(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int burst, int injure, int cmd, List<Monster> toMonsters)
        {
            if (null == toMonsters) return;

            double enemyLife = 0;
            for (int i = 0; i < toMonsters.Count; i++)
            {
                //怪物必须或者才操作
                if (toMonsters[i].VLife > 0.0 && toMonsters[i].Alive)
                {
                    Monster enemy = toMonsters[i];

                    //将对敌人的伤害进行处理
                    injure = InjureToEnemy(toMonsters[i], injure);

                    //处理BOSS克星
                    injure = DBRoleBufferManager.ProcessAntiBoss(client, toMonsters[i], injure);

                    double nTemp = (toMonsters[i] as Monster).VLife;

                    toMonsters[i].VLife -= (int)injure; //是否需要锁定
                    toMonsters[i].VLife = Global.GMax(toMonsters[i].VLife, 0.0);
                    enemyLife = toMonsters[i].VLife;

                    GlobalEventSource.getInstance().fireEvent(new MonsterBlooadChangedEventObject(toMonsters[i]));

                    if (nTemp >= toMonsters[i].MonsterInfo.VLifeMax) //第一次受伤的事件触发
                    {
                        GlobalEventSource.getInstance().fireEvent(new MonsterInjuredEventObject(toMonsters[i], client));
                    }

                    // 卓越属性 有几率完全恢复血和蓝 [12/27/2013 LiaoWei]
                    if (client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP15] > 0.0)
                    {
                        int nRan = Global.GetRandomNumber(0, 101);
                        if (nRan <= client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP15] * 100)
                            client.ClientData.CurrentLifeV = client.ClientData.LifeV;
                    }

                    if (client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP16] > 0.0)
                    {
                        int nRan = Global.GetRandomNumber(0, 101);
                        if (nRan <= client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP16] * 100)
                            client.ClientData.CurrentMagicV = client.ClientData.MagicV;
                    }

                    //bool hitFly = (injure > (toMonsters[i].MonsterInfo.VLifeMax / 3));
                    bool hitFly = false; // (6 == client.ClientData.MapCode / 1000); //如果是血色城堡系列
                    if (toMonsters[i].VLife <= 0.0)
                    {
                        hitFly = true;
                    }

                    Point hitToGrid = new Point(-1, -1);
                    if (hitFly/* || (0 == Global.GetRandomNumber(0, 5))*/)
                    {
                        hitToGrid = ChuanQiUtils.HitFly(client, toMonsters[i], toMonsters[i].VLife <= 0 ? 2 : 1);
                        if ((int)hitToGrid.X > 0 && (int)hitToGrid.Y > 0)
                        {
                            //toMonsters[i].LastFatalInjuredTicks = DateTime.Now.Ticks / 10000;
                        }
                    }

                    int nValue = 0;
                    if (toMonsters[i].VLife == 0.0)
                        nValue = (int)nTemp;
                    else
                        nValue = injure;

                    // 天使神殿处理 [3/23/2014 LiaoWei]
                    if (client.ClientData.MapCode == GameManager.AngelTempleMgr.m_AngelTempleData.MapCode)
                    {
                        GameManager.AngelTempleMgr.ProcessAttackBossInAngelTempleScene(client, enemy, nValue);
                    }

                    if (Global.IsInTeamCopyScene(client.ClientData.MapCode))
                    {
                        CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.CopyMapID);
                        Interlocked.Add(ref client.SumDamageForCopyTeam, nValue);
                    }

                    if (GameManager.GuildCopyMapMgr.IsGuildCopyMap(client.ClientData.MapCode))
                    {
                        CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.CopyMapID);
                        Interlocked.Add(ref client.SumDamageForCopyTeam, nValue);
                    }

                    //判断是否将给敌人的伤害转化成自己的血量增长
                    GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);

                    //将攻击者加入历史列表
                    toMonsters[i].AddAttacker(client.ClientData.RoleID, Global.GMax(0, injure));

                    //记录角色攻击目标
                    client.ClientData.RoleIDAttackebByMyself = toMonsters[i].RoleID;

                    //GameManager.SystemServerEvents.AddEvent(string.Format("怪物减血, Injure={0}, Life={1}", injure, enemyLife), EventLevels.Debug);

                    //攻击就取消隐身
                    if (client.ClientData.DSHideStart > 0)
                    {
                        Global.RemoveBufferData(client, (int)BufferItemTypes.DSTimeHideNoShow);
                        client.ClientData.DSHideStart = 0;
                        GameManager.ClientMgr.NotifyDSHideCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
                    }

                    //int enemyExperience = 0;

                    //判断怪物是否死亡
                    if ((int)toMonsters[i].VLife <= 0.0)
                    {
                        // 注释掉 利用接口 [12/27/2013 LiaoWei]
                        /*enemyExperience = toMonsters[i].MonsterZoneNode.VExperience;

                        //锁定攻击自己的敌人
                        toMonsters[i].LockObject = -1;
                        toMonsters[i].LockFocusTime = 0;

                        //GameManager.SystemServerEvents.AddEvent(string.Format("怪物死亡, roleID={0}", toMonsters[i].RoleID), EventLevels.Debug);

                        /// 处理怪物死亡
                        ProcessMonsterDead(sl, pool, client, toMonsters[i], enemyExperience, toMonsters[i].MonsterZoneNode.VMoney, injure);

                        // 转入界面线程
                        //System.Windows.Application.Current.Dispatcher.BeginInvoke((MethodInvoker)delegate
                        //{
                        //    toMonsters[i].MoveToPos = new Point(-1, -1); //防止重入
                        //    toMonsters[i].DestPoint = new Point(-1, -1);
                        //    Global.RemoveStoryboard(toMonsters[i].Name);
                        //    toMonsters[i].Action = GActions.Death;
                        //});
                        AddDelayDeadMonster(toMonsters[i]);*/

                        Global.ProcessMonsterDieForRoleAttack(sl, pool, client, toMonsters[i], injure);
                    }
                    else
                    {
                        //锁定攻击自己的敌人
                        if (toMonsters[i].LockObject < 0)
                        {
                            toMonsters[i].LockObject = roleID;
                            toMonsters[i].LockFocusTime = DateTime.Now.Ticks / 10000;
                        }
                    }

                    int ownerRoleID = toMonsters[i].GetAttackerFromList();
                    if (ownerRoleID >= 0 && ownerRoleID != client.ClientData.RoleID)
                    {
                        GameClient findClient = GameManager.ClientMgr.FindClient(ownerRoleID);
                        if (null != findClient)
                        {
                            //通知其他在线客户端
                            GameManager.ClientMgr.NotifySpriteInjured(sl, pool, findClient, findClient.ClientData.MapCode, findClient.ClientData.RoleID, toMonsters[i].RoleID, 0, 0, enemyLife, findClient.ClientData.Level, hitToGrid);

                            //向自己发送敌人受伤的信息
                            ClientManager.NotifySelfEnemyInjured(sl, pool, findClient, findClient.ClientData.RoleID, toMonsters[i].RoleID, 0, 0, enemyLife, 0);
                        }
                    }

                    //通知其他在线客户端
                    GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, toMonsters[i].RoleID, burst, injure, enemyLife, client.ClientData.Level, hitToGrid);

                    //向自己发送敌人受伤的信息
                    ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, toMonsters[i].RoleID, burst, injure, enemyLife, 0);

                    // 反射伤害处理 [7/3/2014 LiaoWei]
                    Global.ProcessDamageThorn(sl, pool, client, enemy, injure);
                }
            }
        }

        #endregion 角色攻击怪物计算

        #region 怪物攻击怪物计算

        /// <summary>
        /// 通知其他人被攻击(单攻)，并且被伤害(同一个地图才需要通知)
        /// </summary>
        /// <param name="client"></param>
        public int Monster_NotifyInjured(SocketListener sl, TCPOutPacketPool pool, Monster attacker, Monster enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, int skillLevel, double skillBaseAddPercent, double skillUpAddPercent, bool ignoreDefenseAndDodge = false, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
        {
            int ret = 0;
            object obj = enemy;
            {
                //怪物必须或者才操作
                if ((obj as Monster).VLife > 0 && (obj as Monster).Alive)
                {
                    if (injure <= 0)
                    {
                        /// 角色攻击怪的计算公式
                        if (0 == attackType) //物理攻击
                        {
                            RoleAlgorithm.AttackEnemy(attacker, (obj as Monster), forceBurst, injurePercent, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge);
                        }
                        else if (1 == attackType) //魔法攻击
                        {
                            RoleAlgorithm.MAttackEnemy(attacker, (obj as Monster), forceBurst, injurePercent, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge);
                        }
                        else //道术攻击
                        {
                            // 属性改造 去掉 道术攻击[8/15/2013 LiaoWei]
                            //RoleAlgorithm.DSAttackEnemy(attacker, (obj as Monster), forceBurst, injurePercent, addInjure, attackPercent, addAttack, out burst, out injure, ignoreDefenseAndDodge);
                        }
                    }

                    ret = injure;

                    (obj as Monster).VLife -= (int)Global.GMax(0, injure); //是否需要锁定
                    (obj as Monster).VLife = Global.GMax((obj as Monster).VLife, 0);
                    double enemyLife = (obj as Monster).VLife;

                    //判断是否将给敌人的伤害转化成自己的血量增长
                    //SpriteInjure2Blood(sl, pool, attacker, injure);

                    //只有有主人的怪物才计算伤害量
                    if (null != attacker.OwnerClient)
                    {
                        //将攻击者加入历史列表
                        (obj as Monster).AddAttacker(attacker.OwnerClient.ClientData.RoleID, Global.GMax(0, injure));

                        //记录角色攻击目标
                        attacker.OwnerClient.ClientData.RoleIDAttackebByMyself = (obj as Monster).RoleID;
                    }

                    //GameManager.SystemServerEvents.AddEvent(string.Format("怪物减血, Injure={0}, Life={1}", injure, enemyLife), EventLevels.Debug);

                    int enemyExperience = 0;

                    //判断怪物是否死亡
                    if ((obj as Monster).VLife <= 0)
                    {
                        enemyExperience = (obj as Monster).MonsterInfo.VExperience;

                        //锁定攻击自己的敌人
                        (obj as Monster).LockObject = -1;
                        (obj as Monster).LockFocusTime = 0;

                        //GameManager.SystemServerEvents.AddEvent(string.Format("怪物死亡, roleID={0}", (obj as Monster).RoleID), EventLevels.Debug);

                        /// 处理怪物死亡
                        ProcessMonsterDead(sl, pool, attacker, (obj as Monster), enemyExperience, (obj as Monster).MonsterInfo.VMoney, injure);

                        AddDelayDeadMonster(obj as Monster);
                    }
                    else
                    {
                        //锁定攻击自己的敌人
                        if ((obj as Monster).LockObject < 0)
                        {
                            (obj as Monster).LockObject = attacker.RoleID;
                            (obj as Monster).LockFocusTime = DateTime.Now.Ticks / 10000;
                        }
                    }

                    //通知其他在线客户端
                    GameManager.ClientMgr.NotifySpriteInjured(sl, pool, attacker, attacker.MonsterZoneNode.MapCode, attacker.RoleID, (obj as Monster).RoleID, burst, injure, enemyLife, attacker.MonsterInfo.VLevel, new Point(-1, -1));
                }
            }

            return ret;
        }

        /// <summary>
        /// 通知其他人被攻击(群攻)，并且被伤害(同一个地图才需要通知)
        /// </summary>
        /// <param name="client"></param>
        public void Monster_NotifyInjuredEx(SocketListener sl, TCPOutPacketPool pool, Monster attacker, int roleID, int burst, int injure, int cmd, List<Monster> toMonsters)
        {
            if (null == toMonsters) return;

            double enemyLife = 0;
            for (int i = 0; i < toMonsters.Count; i++)
            {
                //怪物必须或者才操作
                if (toMonsters[i].VLife > 0 && toMonsters[i].Alive)
                {
                    toMonsters[i].VLife -= (int)injure; //是否需要锁定
                    toMonsters[i].VLife = Global.GMax(toMonsters[i].VLife, 0);
                    enemyLife = toMonsters[i].VLife;

                    //判断是否将给敌人的伤害转化成自己的血量增长
                    //SpriteInjure2Blood(sl, pool, attacker, injure);

                    //只有有主人的怪物才计算伤害量
                    if (null != toMonsters[i].OwnerClient)
                    {
                        //将攻击者加入历史列表
                        toMonsters[i].AddAttacker(attacker.OwnerClient.ClientData.RoleID, Global.GMax(0, injure));

                        //记录角色攻击目标
                        attacker.OwnerClient.ClientData.RoleIDAttackebByMyself = toMonsters[i].RoleID;
                    }                    

                    //GameManager.SystemServerEvents.AddEvent(string.Format("怪物减血, Injure={0}, Life={1}", injure, enemyLife), EventLevels.Debug);

                    int enemyExperience = 0;

                    //判断怪物是否死亡
                    if ((int)toMonsters[i].VLife <= 0)
                    {
                        enemyExperience = toMonsters[i].MonsterInfo.VExperience;

                        //锁定攻击自己的敌人
                        toMonsters[i].LockObject = -1;
                        toMonsters[i].LockFocusTime = 0;

                        //GameManager.SystemServerEvents.AddEvent(string.Format("怪物死亡, roleID={0}", toMonsters[i].RoleID), EventLevels.Debug);

                        /// 处理怪物死亡
                        ProcessMonsterDead(sl, pool, attacker, toMonsters[i], enemyExperience, toMonsters[i].MonsterInfo.VMoney, injure);

                        // 转入界面线程
                        //System.Windows.Application.Current.Dispatcher.BeginInvoke((MethodInvoker)delegate
                        //{
                        //    toMonsters[i].MoveToPos = new Point(-1, -1); //防止重入
                        //    toMonsters[i].DestPoint = new Point(-1, -1);
                        //    Global.RemoveStoryboard(toMonsters[i].Name);
                        //    toMonsters[i].Action = GActions.Death;
                        //});
                        AddDelayDeadMonster(toMonsters[i]);
                    }
                    else
                    {
                        //锁定攻击自己的敌人
                        if (toMonsters[i].LockObject < 0)
                        {
                            toMonsters[i].LockObject = roleID;
                            toMonsters[i].LockFocusTime = DateTime.Now.Ticks / 10000;
                        }
                    }

                    GameManager.ClientMgr.NotifySpriteInjured(sl, pool, attacker, attacker.MonsterZoneNode.MapCode, attacker.RoleID, toMonsters[i].RoleID, burst, injure, enemyLife, attacker.MonsterInfo.VLevel, new Point(-1, -1));
                }
            }
        }

        #endregion 怪物攻击怪物

        #region 怪物生命心跳调度

        /// <summary>
        /// 怪物生命心跳调度函数
        /// </summary>
        public void DoMonsterHeartTimer(int mapCode = -1, int subMapCode = -1)
        {
            long ticks = TimeUtil.NOW();
            //foreach (var obj in MyMonsterContainer.ObjectList)
            List<object> objectsList = MyMonsterContainer._ObjectList;
            if (mapCode != -1) //地图驱动模式
            {
                objectsList = MyMonsterContainer.GetObjectsByMap(mapCode, subMapCode);
            }

            if (null == objectsList)
            {
                return;
            }

            foreach (var obj in objectsList)
            {
                Monster monster = obj as Monster;

                //如果已经死亡，则不再调度
                if (!monster.Alive)
                {
                    continue;
                }

                //跳过竞技场的怪物
                if (monster.MonsterType == (int)MonsterTypes.JingJiChangRobot)
                {
                    continue;
                }

                //如果怪物处于站立状态，则不必调度
                if (monster._Action == GActions.Stand)
                {
                    continue;
                }

                //如果还没到时间，则跳过
                if (ticks - monster.LastExecTimerTicks < monster._HeartInterval)
                {
                    continue;
                }

                monster.LastExecTimerTicks = ticks;
                monster.Timer_Tick(null, EventArgs.Empty);
            }

        }

        #endregion 怪物生命心跳调度

        #region 战斗调度算法(主界面线程调用)

        /// <summary>
        /// 判断怪物是否在障碍物中
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        //private bool JugeMonsterInObs(Monster monster)
        //{
        //    if (Global.InObs(ObjectTypes.OT_MONSTER, monster.MonsterZoneNode.MapCode, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        /// <summary>
        /// 对于通天大盗和夺宝奇兵类型的移动到随机的地点
        /// </summary>
        /// <param name="monster"></param>
        private void MoveToRandomPoint(SocketListener sl, TCPOutPacketPool pool, Monster monster)
        {
        }

        /// <summary>
        /// 校正怪物是否正在站立中
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        private void CheckMonsterStandStatus(Monster monster)
        {
            return; //强制不再矫正，耗费资源

            //如果不是移动状态
            if (monster._Action != GActions.Walk) return;

            //查找故事板中是否存在
            //if (Global.ExistStoryboard(monster.Name))
            if (monster.IsMoving)
            {
                return;
            }

            //monster.MoveToPos = new Point(-1, -1); //防止重入
            monster.DestPoint = new Point(-1, -1);
            monster.Action = GActions.Stand; //不需要通知，同一会执行动作

            //通知其他人自己开始做动作
            List<Object> listObjs = Global.GetAll9Clients(monster);
            GameManager.ClientMgr.NotifyOthersDoAction(
                Global._TCPManager.MySocketListener, 
                Global._TCPManager.TcpOutPacketPool,
                monster,
                monster.MonsterZoneNode.MapCode, 
                monster.CopyMapID, monster.RoleID, 
                (int)monster.Direction, 
                (int)GActions.Stand, 
                (int)monster.Coordinate.X, 
                (int)monster.Coordinate.Y, 
                0, 0, (int)TCPGameServerCmds.CMD_SPR_ACTTION,
                listObjs);
        }

        /// <summary>
        /// 检测是否在障碍物上
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="monster"></param>
        /// <param name="ticks"></param>
        private bool CheckMonsterInObs(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks)
        {
            ///模仿傲视遮天的bug，和主人重叠不进行处理
            if ((int)MonsterTypes.DSPetMonster == monster.MonsterType)
            {
                if (null != monster.OwnerClient)
                {
                    Point monsterGrid = monster.CurrentGrid;
                    Point clientGrid = monster.OwnerClient.CurrentGrid;
                    if ((int)monsterGrid.X == (int)clientGrid.X && (int)monsterGrid.Y == (int)clientGrid.Y)
                    {
                        return false;
                    }
                }
            }

            //血色城堡副本太特殊，不能走这个路径
            int mapCodeMod = monster.MonsterZoneNode.MapCode / 1000;
            if (6 == mapCodeMod || 5 == mapCodeMod || 7 == mapCodeMod) //如果是血色城堡系列, 经验副本系列, 恶魔广场些列
            {
                return false;
            }

            //如果正在移动，则不处理
            if (monster.IsMoving)
            {
                return false;
            }

            if (ticks - monster.LastInObsJugeTicks >= (3 * 1000)) //3秒钟才判断一次
            {
                monster.LastInObsJugeTicks = ticks; //记录判断时间

                //判断是否不小心掉到了障碍物中，或者和其他的对象在地图格子上重叠了
                if (Global.InObs(ObjectTypes.OT_MONSTER, monster.MonsterZoneNode.MapCode, (int)monster.Coordinate.X, (int)monster.Coordinate.Y, 1))
                {
                    Point grid = monster.CurrentGrid;
                    bool toMove = true;
                    if (monster.CopyMapID > 0)
                    {
                        //判断怪物是否能在副本地图上移动
                        if (ChuanQiUtils.CanMonsterMoveOnCopyMap(monster, (int)grid.X, (int)grid.Y))
                        {
                            toMove = false;
                        }
                    }

                    if (toMove)
                    {
                        //GameManager.SystemServerEvents.AddEvent(string.Format("怪物掉进了障碍中或者重叠了, roleID={0}, Map={1}, X={2}, Y={3}, ExtensionID={4}", monster.RoleID, monster.MonsterZoneNode.MapCode, (int)monster.Coordinate.X, (int)monster.Coordinate.Y, monster.MonsterInfo.ExtensionID), EventLevels.Debug);

                        int nCurrX = (int)grid.X;
                        int nCurrY = (int)grid.Y;

                        int nOldX = nCurrX;
                        int nOldY = nCurrY;

                        ChuanQiUtils.WalkTo(monster, (Dircetions)(int)Global.GetRandomNumber(0, 8));
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 搜索视野范围内的可以攻击的对象
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="monster"></param>
        /// <param name="ticks"></param>
        private void SearchViewRange(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks, int rolesNum)
        {
            if ((ticks - monster.LastSeekEnemyTicks) >= monster.NextSeekEnemyTicks) //3~5秒钟才搜寻一次
            {
                monster.LastSeekEnemyTicks = ticks; //记录时间

                //判断如果指定的地图上的玩家人数大于0, 才搜索，否则不搜索
                //if (GameManager.ClientMgr.GetMapClientsCount(monster.MonsterZoneNode.MapCode) > 0)
                if (rolesNum > 0)
                {
                    //判断是否自动搜索敌人并锁定
                    GameManager.ClientMgr.SeekSpriteToLock(monster);
                }
            }
        }

        private void SelectTarget(Monster monster, IObject obj, long ticks)
        {
            monster.LockObject = obj.GetObjectID();
            monster.LockFocusTime = ticks;
        }

        /// <summary>
        /// 判断是否能够锁定目标
        /// 系统战斗识别依赖于两个概念，一个是锁定，一个是敌人，能被怪物锁定的，一定是怪物的敌人
        /// 怪物的敌人却不一定能被怪物锁定
        /// 对玩家而言，只需要识别是否敌人，至于是否锁定，玩家自己决定【当然，有时候我们可能不让他们锁定】
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="obj"></param>
        private Boolean CanLock(Monster monster, IObject obj)
        {
            //不是敌人当然不能锁定
            if (!Global.IsOpposition(monster, obj))
            {
                return false;
            }

            //可能为NULL
            GameClient targetClient = obj as GameClient;
            Monster targetMonster = obj as Monster;

            if (null != targetMonster)
            {
                targetClient = targetMonster.OwnerClient;//仍然可能为null
            }

            //隐身时，玩家不可以被攻击，但怪任然可以被攻击
            if (null == targetMonster && null != targetClient)
            {
                if (!Global.RoleIsVisible(targetClient))
                {
                    return false;
                }
            }
            /*
            //如果是非逍遥召唤的怪，且锁定对象是玩家 或者 玩家的怪，则怪无法攻击
            if ((int)MonsterTypes.DSPetMonster != monster.MonsterType && null != targetClient)
            {
                if (!Global.RoleIsVisible(targetClient))
                {
                    return false;
                }
            }

            //逍遥怪的主人隐身，则怪自己也不能攻击任何敌人===> 根据策划要求，自己隐身后宠物还能攻击敌人
            if (((int)MonsterTypes.DSPetMonster == monster.MonsterType && null != monster.OwnerClient))
            {
                if (!Global.RoleIsVisible(monster.OwnerClient))
                {
                    return false;
                }
            }
            */

            if ((int)MonsterTypes.DSPetMonster != monster.MonsterType)
            {
                if (monster.DynamicMonster)
                {
                    //如果超过了最大追击范围，则不再追击
                    if (monster.DynamicPursuitRadius > 0)
                    {
                        if (Global.GetTwoPointDistance(obj.CurrentPos, monster.FirstCoordinate) >= monster.DynamicPursuitRadius)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //如果超过了最大追击范围，则不再追击
                    if (monster.MonsterZoneNode.PursuitRadius > 0)
                    {
                        if (Global.GetTwoPointDistance(obj.CurrentPos, monster.FirstCoordinate) >= monster.MonsterZoneNode.PursuitRadius)
                        {
                            return false;
                        }
                    }
                }
            }

            //根据主动怪物细分类型进行细分过滤 过滤不能攻击的条件
            switch (monster.MonsterType)
            {
                //1 怪 总 攻击非同阵营怪
                //2 怪 可能攻击红名的玩家，所有玩家，不攻击玩家
                //如果是城池守卫，只攻击红名玩家 
                case (int)MonsterTypes.CityGuard:
                    {
                        //玩家不红名且玩家没有攻击自己，则强制不能攻击
                        if (null != targetClient && !Global.IsRedName(targetClient) && !monster.IsAttackedBy(targetClient.ClientData.RoleID))
                        {
                            return false;
                        }
                        else
                        {
                            if (null != targetClient && obj is GameClient)
                            {
                                //System.Diagnostics.Debug.WriteLine("守卫攻击玩家 IsRedName = {0}, IsAttackedBy = {1} \r\n", Global.IsRedName(targetClient) ? 1 : 0,
                                    //monster.IsAttackedBy(targetClient.ClientData.RoleID)? 1:0);
                            }
                        }
                    }
                    break;
                //逍遥召唤的怪
                case (int)MonsterTypes.DSPetMonster:
                    {
                        //一般不可能
                        if (null == monster.OwnerClient) break;

                        //如果有主人，且是逍遥召唤的怪，而且，攻击控制类型为攻击主人攻击的目标，则这时只能锁定攻击主人的敌人或怪
                        //或者主人攻击的敌人或怪
                        if ((int)PetMonsterControlType.AttackMasterTarget == monster.PetAiControlType)//攻击和主人一致的目标
                        {
                            //主人没有攻击敌人，敌人没有攻击主人,不能锁定,根据规则，此时如果对方攻击自己，
                            //怪物本身不反击
                            if (!Global.IsInBattle(monster.OwnerClient, obj))
                            {
                                return false;
                            }
                        }
                        else if ((int)PetMonsterControlType.FreeAttack == monster.PetAiControlType)//自由攻击
                        {
                            if (obj is GameClient)
                            {
                                //主人没有攻击敌人，敌人没有攻击主人,不能锁定,根据规则，此时如果对方攻击自己，
                                //怪物本身不反击
                                if (!Global.IsInBattle(monster.OwnerClient, obj))
                                {
                                    return false;
                                }
                            }

                            //没事别攻击城市守卫 ===>平民怪 和 玩家本身就是友好的，isoposition 里面已经判定他们是友好的，这儿主要进行模糊界定判断
                            if (obj is Monster && (obj as Monster).MonsterType == (int)MonsterTypes.CityGuard)
                            {
                                if (!Global.IsInBattle(monster.OwnerClient, obj))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    break;
                default: break;
            }

            return true;
        }

        /// <summary>
        /// 尝试锁定搜索视野范围内怪物
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="monster"></param>
        /// <param name="ticks"></param>
        private void TryToLockObject(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks)
        {
            if ((ticks - monster.LastLockEnemyTicks > 8000) || ((ticks - monster.LastLockEnemyTicks > 1000) && -1 == monster.LockObject))
            {
                monster.LastLockEnemyTicks = ticks;

                //if (monster.MonsterInfo.ExtensionID == 5)
                //{
                //    System.Diagnostics.Debug.WriteLine("abc");
                //}

                if (null != monster.VisibleItemList)
                {
                    for (int i = 0; i < monster.VisibleItemList.Count; i++)
                    {
                        //不是能锁定就算了，不是敌人就一定能锁定，怪有时候只锁定攻击自己的敌人
                        if (monster.VisibleItemList[i].ItemType == ObjectTypes.OT_CLIENT)
                        {
                            GameClient gameClient = GameManager.ClientMgr.FindClient(monster.VisibleItemList[i].ItemID);

                            if (null != gameClient)
                            {
                                //不是敌人就算了
                                if (!CanLock(monster, gameClient))
                                {
                                    continue;
                                }

                                Point monsterGrid = monster.CurrentGrid;
                                int nCurrX = (int)monsterGrid.X;
                                int nCurrY = (int)monsterGrid.Y;

                                Point defenserGrid = gameClient.CurrentGrid;
                                int nTargetCurrX = (int)defenserGrid.X;
                                int nTargetCurrY = (int)defenserGrid.Y;

                                if (Math.Abs(nCurrX - nTargetCurrX) + Math.Abs(nCurrY - nTargetCurrY) < 999)
                                {
                                    SelectTarget(monster, gameClient, ticks);
                                    break;
                                }
                            }
                        }

                        //对于玩家召唤的怪，可以让其锁定其它怪，对于野外怪物，可以让其锁定玩家的的宠物怪
                        if (monster.VisibleItemList[i].ItemType == ObjectTypes.OT_MONSTER)
                        {
                            Monster enemyMonster = GameManager.MonsterMgr.FindMonster(monster.CurrentMapCode, monster.VisibleItemList[i].ItemID);

                            //不是能锁定就算了，不是敌人就一定能锁定，怪有时候只锁定攻击自己的敌人
                            if (!CanLock(monster, enemyMonster))
                            {
                                continue;
                            }

                            //副本id 要一致，要么都-1， 且主人要不一样
                            if (null != enemyMonster && enemyMonster.CurrentCopyMapID == monster.CurrentCopyMapID)
                            {
                                //都有主人，且主人一样，不能相互攻击
                                if (null != enemyMonster.OwnerClient && enemyMonster.OwnerClient == monster.OwnerClient)
                                {
                                    continue;
                                }

                                Point monsterGrid = monster.CurrentGrid;
                                int nCurrX = (int)monsterGrid.X;
                                int nCurrY = (int)monsterGrid.Y;

                                Point defenserGrid = enemyMonster.CurrentGrid;
                                int nTargetCurrX = (int)defenserGrid.X;
                                int nTargetCurrY = (int)defenserGrid.Y;

                                if (Math.Abs(nCurrX - nTargetCurrX) + Math.Abs(nCurrY - nTargetCurrY) < 999)
                                {
                                    SelectTarget(monster, enemyMonster, ticks);
                                    break;
                                }
                            }
                        }

                        //如果是玩家召唤的怪，应该还可以锁定镖车
                    }
                }
            }
        }

        /// <summary>
        /// 锁定的对象是否依然有效
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="gameClient"></param>
        /// <returns></returns>
        private bool IsLockObjectValid(Monster monster, GameClient gameClient, long ticks)
        {
            if (null == gameClient)
            {
                return false;
            }

            if (gameClient.ClientData.CurrentLifeV <= 0)
            {
                return false;
            }

            if ((ticks - monster.LockFocusTime) > 30000)
            {
                return false;
            }

            Point monsterGrid = monster.CurrentGrid;
            int nCurrX = (int)monsterGrid.X;
            int nCurrY = (int)monsterGrid.Y;

            Point defenserGrid = gameClient.CurrentGrid;
            int nTargetCurrX = (int)defenserGrid.X;
            int nTargetCurrY = (int)defenserGrid.Y;

            //if ((Math.Abs(nTargetCurrX - nCurrX) > 15) || (Math.Abs(nTargetCurrY - nCurrY) > 15))
            if ((Math.Abs(nTargetCurrX - nCurrX) > 12) || (Math.Abs(nTargetCurrY - nCurrY) > 12)) //减少追击范围
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检测锁定的对象是否依然有效 怪对怪 【召唤的对野外怪】
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="targetMonster"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        private bool IsLockObjectValid(Monster monster, Monster targetMonster, long ticks)
        {
            if (null == targetMonster)
            {
                return false;
            }

            if (targetMonster.VLife <= 0)
            {
                return false;
            }

            if ((ticks - monster.LockFocusTime) > 30000)
            {
                return false;
            }

            Point monsterGrid = monster.CurrentGrid;
            int nCurrX = (int)monsterGrid.X;
            int nCurrY = (int)monsterGrid.Y;

            Point defenserGrid = targetMonster.CurrentGrid;
            int nTargetCurrX = (int)defenserGrid.X;
            int nTargetCurrY = (int)defenserGrid.Y;

            //if ((Math.Abs(nTargetCurrX - nCurrX) > 15) || (Math.Abs(nTargetCurrY - nCurrY) > 15))
            if ((Math.Abs(nTargetCurrX - nCurrX) > 12) || (Math.Abs(nTargetCurrY - nCurrY) > 12)) //减少追击范围
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 锁定的对象是否依然有效
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="gameClient"></param>
        /// <returns></returns>
        private bool IsLockObjectValid(Monster monster, IObject lockObject, long ticks)
        {
            if (lockObject is GameClient)
            {
                return IsLockObjectValid(monster, lockObject as GameClient, ticks);
            }
            else
            {
                if (lockObject is Monster)
                {
                    return IsLockObjectValid(monster, lockObject as Monster, ticks);
                }
            }

            return false;
        }

        /// <summary>
        /// 检测锁定的对象是否有效
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="monster"></param>
        /// <param name="ticks"></param>
        private bool CheckLockObject(SocketListener sl, TCPOutPacketPool pool, Monster monster, IObject lockObject, long ticks)
        {
            //这个是检测已经锁定的对象是否还合法的验证，玩家可能会更改自己的攻击的模式
            if (!CanLock(monster, lockObject))
            {
                //失去锁定对象
                monster.LockObject = -1;
                monster.LockFocusTime = 0;

                return false;
            }

            if (!IsLockObjectValid(monster, lockObject, ticks))
            {
                //失去锁定对象
                monster.LockObject = -1;
                monster.LockFocusTime = 0;

                if (monster._Action != GActions.Stand)
                {
                    //通知其他人自己开始做动作
                    List<Object> listObjs = Global.GetAll9Clients(monster);
                    GameManager.ClientMgr.NotifyOthersDoAction(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeDirection, (int)GActions.Stand, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, 0, 0, (int)TCPGameServerCmds.CMD_SPR_ACTTION, listObjs);

                    //本地动作
                    //monster.MoveToPos = new Point(-1, -1); //防止重入
                    monster.DestPoint = new Point(-1, -1);
                    Global.RemoveStoryboard(monster.Name);
                    monster.Action = GActions.Stand;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// 检测锁定的对象是否有效
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="monster"></param>
        /// <param name="ticks"></param>
        private bool CheckLockObject(SocketListener sl, TCPOutPacketPool pool, Monster monster, Monster targetMonster, long ticks)
        {
            if (!IsLockObjectValid(monster, targetMonster, ticks))
            {
                //失去锁定对象
                monster.LockObject = -1;
                monster.LockFocusTime = 0;

                if (monster._Action != GActions.Stand)
                {
                    //通知其他人自己开始做动作
                    List<Object> listObjs = Global.GetAll9Clients(monster);
                    GameManager.ClientMgr.NotifyOthersDoAction(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeDirection, (int)GActions.Stand, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, 0, 0, (int)TCPGameServerCmds.CMD_SPR_ACTTION, listObjs);

                    //本地动作
                    //monster.MoveToPos = new Point(-1, -1); //防止重入
                    monster.DestPoint = new Point(-1, -1);
                    Global.RemoveStoryboard(monster.Name);
                    monster.Action = GActions.Stand;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取走动的方向
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        private Dircetions GetWonderingWalkDir(Monster monster)
        {
            //* tmp
            /*double distance = Global.GetTwoPointDistance(new Point(monster.MonsterZoneNode.ToX, monster.MonsterZoneNode.ToY), monster.CurrentGrid);
            if (distance >= (int)(monster.MonsterZoneNode.Radius * 2.0))
            {
                Point pos = monster.SafeCoordinate;
                return (Dircetions)Global.GetDirectionByTan(monster.MonsterZoneNode.ToX, monster.MonsterZoneNode.ToY, pos.X, pos.Y);
            }*/

            monster.CurrentDir = (Dircetions)Global.GetRandomNumber(0, 8); //* tmp
            return monster.CurrentDir;
        }

        /// <summary>
        /// 无聊时的动作
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="monster"></param>
        /// <param name="ticks"></param>
        private void Wondering(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks)
        {
            //if (monster.MonsterInfo.ExtensionID == 5)
            //{
            //    System.Diagnostics.Debug.WriteLine("abc");
            //}

            ///模仿傲视遮天的bug，和主人重叠不进行处理
            if ((int)MonsterTypes.DSPetMonster == monster.MonsterType)
            {
                if (null != monster.OwnerClient)
                {
                    Point monsterGrid = monster.CurrentGrid;
                    Point clientGrid = monster.OwnerClient.CurrentGrid;
                    if ((int)monsterGrid.X == (int)clientGrid.X && (int)monsterGrid.Y == (int)clientGrid.Y)
                    {
                        return;
                    }
                }
            }

            if (monster.LockObject >= 0) //如果有锁定的目标
            {
                return;
            }

            //判断是否能够寻敌
            if (!CanMonsterSeekRange(monster))
            {
                return;
            }

            if ((int)MonsterTypes.DSPetMonster != monster.MonsterType)
            {
                if (monster.DynamicMonster)
                {
                    //如果超过了最大追击范围，则不再追击
                    if (monster.DynamicPursuitRadius > 0)
                    {
                        if (Global.GetTwoPointDistance(monster.CurrentPos, monster.FirstCoordinate) >= monster.DynamicPursuitRadius)
                        {
                            //获取走动的方向
                            Dircetions nDir = (Dircetions)Global.GetDirectionByTan(monster.FirstCoordinate.X, monster.FirstCoordinate.Y, monster.CurrentPos.X, monster.CurrentPos.Y);
                            ChuanQiUtils.WalkTo(monster, nDir);
                            return;
                        }
                    }
                }
                else
                {
                    //如果超过了最大追击范围，则不再追击
                    if (monster.MonsterZoneNode.PursuitRadius > 0)
                    {
                        if (Global.GetTwoPointDistance(monster.CurrentPos, monster.FirstCoordinate) >= monster.MonsterZoneNode.PursuitRadius)
                        {
                            //获取走动的方向
                            Dircetions nDir = (Dircetions)Global.GetDirectionByTan(monster.FirstCoordinate.X, monster.FirstCoordinate.Y, monster.CurrentPos.X, monster.CurrentPos.Y);
                            ChuanQiUtils.WalkTo(monster, nDir);
                            return;
                        }
                    }
                }
            }

            if (Global.GetRandomNumber(0, 10) == 0)//* tmp
            {
                //if (Global.GetRandomNumber(0, 4) == 0 || monster.MoveSpeed <= 0)
                if (monster.MoveSpeed <= 0)
                {
                    //客户端看不出来，效果不明显，还不如不发, 所以只针对弓箭卫士等，不能移动的怪物做这个动作
                    ChuanQiUtils.TurnTo(monster, (Dircetions)Global.GetRandomNumber(0, 8));
                }
                else if (Global.GetRandomNumber(0, 4) != 0)
                {
                    //获取走动的方向
                    Dircetions nDir = GetWonderingWalkDir(monster);
                    ChuanQiUtils.WalkTo(monster, nDir);
                }
            }
        }

        /// <summary>
        /// 通知怪物站立工作
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="monster"></param>
        /// <param name="ticks"></param>
        private void DoMonsterStandAction(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks)
        {
            //通知其他人自己开始做动作
            List<Object> listObjs = Global.GetAll9Clients(monster);
            GameManager.ClientMgr.NotifyOthersDoAction(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeDirection, (int)GActions.Stand, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, 0, 0, (int)TCPGameServerCmds.CMD_SPR_ACTTION, listObjs);

            //本地动作
            //monster.MoveToPos = new Point(-1, -1); //防止重入
            monster.DestPoint = new Point(-1, -1);
            Global.RemoveStoryboard(monster.Name);
            monster.Action = GActions.Stand;
        }

        /// <summary>
        /// 简单的AI调度
        /// 1. 空闲超过指定时间后，使用随机数判断是否走动
        /// 2. 走动时如果走到了限制范围边缘，则立刻回身，随机走到某个位置
        /// </summary>
        private void DoMonsterAI(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks, int count, int IndexOfMonsterAiAttack)
        {
            //如果怪物是在攻击中, 则取消攻击动作
            if (monster._Action == GActions.Attack)
            {
                //通知怪物站立工作
                DoMonsterStandAction(sl, pool, monster, ticks);
            }

            //如果是通天大盗，或者是夺宝奇兵的怪物，则不执行搜索敌人，而是执行随机跑步的操作
            if ((int)MonsterTypes.DaDao == monster.MonsterType ||
                    (int)MonsterTypes.QiBao == monster.MonsterType)
            {
                /*if (ticks - monster.LastSeekEnemyTicks >= (1 * 1000)) //1秒钟才搜寻一次
                {
                    monster.LastSeekEnemyTicks = ticks; //记录时间

                    //判断如果指定的地图上的玩家人数大于0, 才逃跑，否则不逃跑
                    if (GameManager.ClientMgr.GetMapClientsCount(monster.MonsterZoneNode.MapCode) > 0)
                    {
                        //对于通天大盗和夺宝奇兵类型的移动到随机的地点---->通天大盗先别移动了
                        MoveToRandomPoint(sl, pool, monster);
                    }
                }*/
            }
            else if ((int)MonsterTypes.CaiJi == monster.MonsterType || (int)MonsterTypes.BloodCastleGateAndCrystal == monster.MonsterType || (int)MonsterTypes.JUSTMOVE == monster.MonsterType)
            {
                //对于采集怪物类型，不做处理
            }
            else
            {
                if (!monster.isReturn)
                {
                    //无聊时的动作
                    Wondering(sl, pool, monster, ticks);
                }
                else
                {
                    if(monster.CurrentGrid.X == monster.getFirstGrid().X && monster.CurrentGrid.Y == monster.getFirstGrid().Y)
                    {
                        monster.isReturn = false;    
                    }
                    else
                    {
                        MonsterReturn(monster);
                    }
                }
            }
        }


        /// <summary>
        /// 怪物脱战返回出生点
        /// </summary>
        /// <param name="monster"></param>
        private void MonsterReturn(Monster monster)
        {
            int nDir = (int)monster.CurrentDir;
            int nCurrX = (int)monster.CurrentGrid.X;
            int nCurrY = (int)monster.CurrentGrid.Y;
            int nX = (int)monster.getFirstGrid().X;
            int nY = (int)monster.getFirstGrid().Y;

            if (nCurrX == nX && nCurrY == nY)
                return;

            while (true)
            {
                if (nX > nCurrX)
                {
                    nDir = (int)Dircetions.DR_RIGHT;

                    if (nY > nCurrY)
                        nDir = (int)Dircetions.DR_UPRIGHT;
                    else if (nY < nCurrY)
                        nDir = (int)Dircetions.DR_DOWNRIGHT;

                    break;
                }

                if (nX < nCurrX)
                {
                    nDir = (int)Dircetions.DR_LEFT;

                    if (nY > nCurrY)
                        nDir = (int)Dircetions.DR_UPLEFT;
                    else if (nY < nCurrY)
                        nDir = (int)Dircetions.DR_DOWNLEFT;

                    break;
                }

                if (nY > nCurrY)
                {
                    nDir = (int)Dircetions.DR_UP;
                    break;
                }

                if (nY < nCurrY)
                {
                    nDir = (int)Dircetions.DR_DOWN;
                    break;
                }

                break;
            }

            ChuanQiUtils.WalkTo(monster, (Dircetions)nDir);

        }

        /// <summary>
        /// 追击锁定的目标, justMove 为true 表示只向目标obj靠近，不重新锁定,主要用于召唤怪跟随自己的主人
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="monster"></param>
        /// <param name="ticks"></param>
        private void GoToLockObject(SocketListener sl, TCPOutPacketPool pool, Monster monster, IObject obj, long ticks, bool justMove= false)
        {
            if (monster.IsMoving)
            {
                return;
            }

            if (monster.Action == GActions.Attack)
            {
                return;
            }

            if (monster.MoveSpeed <= 0)
            {
                return;
            }

            if (monster.IsMonsterDongJie()) //如果被冻结了
            {
                return;
            }

            int minTicks = 0;

            //血色城堡副本太特殊，不能走这个路径
            int mapCodeMod = monster.MonsterZoneNode.MapCode / 1000;
            if (6 == mapCodeMod) //如果是血色城堡系列
            {
                minTicks = 1000;
            }
            else if (5 == mapCodeMod || 7 == mapCodeMod) //经验副本系列, 恶魔广场些列
            {
                minTicks = 100;
            }

            ///模仿傲视遮天宝宝间隔很小
            if ((int)MonsterTypes.DSPetMonster == monster.MonsterType)
            {
                minTicks = 0;
            }

            if (ticks - monster.LastActionTick < minTicks)
            {
                return;
            }

            //if (ticks - monster.LastFatalInjuredTicks < 250L) //受了致命伤
            //{
            //    return;
            //}

            if ((int)MonsterTypes.DSPetMonster != monster.MonsterType)
            {
                if (monster.DynamicMonster)
                {
                    //如果超过了最大追击范围，则不再追击
                    if (monster.DynamicPursuitRadius > 0)
                    {
                        if (Global.GetTwoPointDistance(monster.CurrentPos, monster.FirstCoordinate) >= monster.DynamicPursuitRadius)
                        {
                            //失去锁定对象
                            monster.LockObject = -1;
                            monster.LockFocusTime = 0;
                            return;
                        }
                    }
                }
                else
                {
                    //如果超过了最大追击范围，则不再追击
                    if (monster.MonsterZoneNode.PursuitRadius > 0)
                    {
                        if (Global.GetTwoPointDistance(monster.CurrentPos, monster.FirstCoordinate) >= monster.MonsterZoneNode.PursuitRadius)
                        {
                            //失去锁定对象
                            monster.LockObject = -1;
                            monster.LockFocusTime = 0;
                            return;
                        }
                    }
                }
            }

            Point monsterGrid = monster.CurrentGrid;
            int nCurrX = (int)monsterGrid.X;
            int nCurrY = (int)monsterGrid.Y;

            Point objGrid = obj.CurrentGrid;
            int nTargetCurrX = (int)objGrid.X;
            int nTargetCurrY = (int)objGrid.Y;

            int nDir = (int)monster.Direction;
            if ((nCurrX != nTargetCurrX) || (nCurrY != nTargetCurrY))
            {
                //如果视线内有精灵，且要求不仅仅是移动，则尝试锁定最近的对象
                if (null != monster.VisibleItemList && !justMove)
                {
                    for (int i = 0; i < monster.VisibleItemList.Count; i++)
                    {
                        //如果视野里面的是玩家
                        if (monster.VisibleItemList[i].ItemType == ObjectTypes.OT_CLIENT)
                        {
                            GameClient gameClient = GameManager.ClientMgr.FindClient(monster.VisibleItemList[i].ItemID);
                            if (null != gameClient)
                            {
                                //不是敌人就算了
                                if (!Global.IsOpposition(monster, gameClient)) continue;

                                if (gameClient.ClientData.CurrentLifeV > 0)
                                {
                                    Point clientGrid = gameClient.CurrentGrid;
                                    int nNewTargetCurrX = (int)clientGrid.X;
                                    int nNewTargetCurrY = (int)clientGrid.Y;

                                    if (Math.Abs(nCurrX - nNewTargetCurrX) + Math.Abs(nCurrY - nNewTargetCurrY) <
                                        Math.Abs(nCurrX - nTargetCurrX) + Math.Abs(nCurrY - nTargetCurrY))
                                    {
                                        SelectTarget(monster, gameClient, ticks);
                                        return;
                                    }
                                }
                            }
                        }
                       
                        //如果视野里面的是怪物
                        if (monster.VisibleItemList[i].ItemType == ObjectTypes.OT_MONSTER)
                        {
                            Monster targetMonster = GameManager.MonsterMgr.FindMonster(monster.CurrentMapCode, monster.VisibleItemList[i].ItemID);
                            if (null != targetMonster)
                            {
                                //不是敌人就算了
                                if (!Global.IsOpposition(monster, targetMonster)) continue;

                                if (targetMonster.VLife > 0)
                                {
                                    Point clientGrid = targetMonster.CurrentGrid;
                                    int nNewTargetCurrX = (int)clientGrid.X;
                                    int nNewTargetCurrY = (int)clientGrid.Y;

                                    if (Math.Abs(nCurrX - nNewTargetCurrX) + Math.Abs(nCurrY - nNewTargetCurrY) <
                                        Math.Abs(nCurrX - nTargetCurrX) + Math.Abs(nCurrY - nTargetCurrY))
                                    {
                                        SelectTarget(monster, targetMonster, ticks);
                                        return;
                                    }
                                }
                            }
                        }

                        //如果视野里面的是镖车等其它可攻击物品
                    }
                }

                //判断如果和目标对象的距离超过了攻击距离，才追击
                //if (Global.GetTwoPointDistance(monster.CurrentPos, obj.CurrentPos) < monster.AttackRange)
                //{
                //    return;
                //}

                int nX = nTargetCurrX;
                int nY = nTargetCurrY;

                while (true)
                {
                    if (nX > nCurrX)
                    {
                        nDir = (int)Dircetions.DR_RIGHT;

                        if (nY > nCurrY)
                            nDir = (int)Dircetions.DR_UPRIGHT;
                        else if (nY < nCurrY)
                            nDir = (int)Dircetions.DR_DOWNRIGHT;

                        break;
                    }

                    if (nX < nCurrX)
                    {
                        nDir = (int)Dircetions.DR_LEFT;

                        if (nY > nCurrY)
                            nDir = (int)Dircetions.DR_UPLEFT;
                        else if (nY < nCurrY)
                            nDir = (int)Dircetions.DR_DOWNLEFT;

                        break;
                    }

                    if (nY > nCurrY)
                    {
                        nDir = (int)Dircetions.DR_UP;
                        break;
                    }

                    if (nY < nCurrY)
                    {
                        nDir = (int)Dircetions.DR_DOWN;
                        break;
                    }

                    break;
                }

                int nOldX = nCurrX;
                int nOldY = nCurrY;

                //if (!ChuanQiUtils.RunTo(monster, (Dircetions)nDir))
                {
                    ChuanQiUtils.WalkTo(monster, (Dircetions)nDir);
                }                   

                monsterGrid = monster.CurrentGrid;
                nCurrX = (int)monsterGrid.X;
                nCurrY = (int)monsterGrid.Y;

                for (int i = 0; i < 7; i++)
                {
                    if (nOldX == nCurrX && nOldY == nCurrY)
                    {
                        if (Global.GetRandomNumber(0, 3) > 0) nDir++;
                        else if (nDir > 0) nDir--;
                        else
                            nDir = 7;

                        if (nDir > 7) nDir = 0;

                        ChuanQiUtils.WalkTo(monster, (Dircetions)nDir);

                        monsterGrid = monster.CurrentGrid;
                        nCurrX = (int)monsterGrid.X;
                        nCurrY = (int)monsterGrid.Y;
                    }
                    else
                        break;
                }
            }
        }

        /// <summary>
        /// 判断是否可以向锁定的对象发起攻击
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defenser"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool TargetInAttackRange(Monster attacker, IObject defenser, out int direction)
        {
            direction = (int)attacker.Direction;
            if (defenser == null)
            {
                return false;
            }

            if (attacker.MonsterZoneNode.MapCode != defenser.CurrentMapCode)
            {
                return false; //如果不在同一个地图, 不能再攻击
            }

            Point defenserGrid = defenser.CurrentGrid;
            int nTargetCurrX = (int)defenserGrid.X;
            int nTargetCurrY = (int)defenserGrid.Y;

            Point monsterGrid = attacker.CurrentGrid;
            int nCurrX = (int)monsterGrid.X;
            int nCurrY = (int)monsterGrid.Y;

            if ((nTargetCurrX == nCurrX) && (nTargetCurrY == nCurrY))
            {
                ChuanQiUtils.WalkTo(attacker, (Dircetions)(int)Global.GetRandomNumber(0, 8));
                return false;
            }

            int autoUseSkillID = attacker.GetAutoUseSkillID();
            int attackNum = GetSkillAttackGridNum(attacker, autoUseSkillID);

            // 如果技能已经能打到，不需要再进行移动
            if (attackNum >= Global.GetTwoPointDistance(attacker.CurrentPos, defenser.CurrentPos) / 100)
            {
                return true;
            }

            //direction = (int)Global.GetDirectionByTan(nTargetCurrX, nTargetCurrY, nTargetCurrX, nTargetCurrY);
            //bool canAttack = false;
            //List<Point> gridPointList = Global.GetGridPointByDirection(direction, nCurrX, nCurrY, attackNum);
            //for (int i = 0; i < gridPointList.Count; i++)
            //{
            //    if (gridPointList[i].X == nTargetCurrX &&
            //        gridPointList[i].Y == nTargetCurrY)
            //    {
            //        canAttack = true;
            //        break;
            //    }
            //}

            // 怪追击完善(出现怪在不能攻击的地方 一直攻击玩家) [5/16/2014 LiaoWei]
            bool bCanGo = false;

            if (attackNum <= 2)
            {
                bCanGo = true;
            }

            

            if (!bCanGo && attackNum < Global.GetTwoPointDistance(attacker.CurrentPos, defenser.CurrentPos) / 100)
            {
                --attackNum;    // 减一个格子 让怪追击 体验好些!! [6/25/2014 LiaoWei]
                bCanGo = true;
            }

            if (bCanGo)
            {
                int verifyDirection = (int)Global.GetDirectionByAspect(nTargetCurrX, nTargetCurrY, nCurrX, nCurrY);
                List<Point> gridList = Global.GetGridPointByDirection(verifyDirection, nCurrX, nCurrY, attackNum);
                for (int i = 0; i < gridList.Count; i++)
                {
                    if (nTargetCurrX == (int)gridList[i].X && nTargetCurrY == (int)gridList[i].Y)
                    {
                        return true;
                    }
                }

                return false;
            }

            //double angle = Math.Atan2((nTargetCurrY - nCurrY), (nTargetCurrX - nCurrX)) * 180 / Math.PI;
            
            if ((nTargetCurrX >= nCurrX - attackNum) && (nTargetCurrX <= nCurrX + attackNum) &&
                (nTargetCurrY >= nCurrY - attackNum) && (nTargetCurrY <= nCurrY + attackNum))
            {
                if ((nTargetCurrX < nCurrX) && (nTargetCurrY == nCurrY))
                {
                    direction = (int)Dircetions.DR_LEFT;
                    return true;
                }

                if ((nTargetCurrX > nCurrX) && (nTargetCurrY == nCurrY))
                {
                    direction = (int)Dircetions.DR_RIGHT;
                    return true;
                }

                if ((nTargetCurrX == nCurrX) && (nTargetCurrY < nCurrY))
                {
                    direction = (int)Dircetions.DR_DOWN;
                    return true;
                }

                if ((nTargetCurrX == nCurrX) && (nTargetCurrY > nCurrY))
                {
                    direction = (int)Dircetions.DR_UP;
                    return true;
                }

                if ((nTargetCurrX < nCurrX) && (nTargetCurrY < nCurrY))
                {
                    direction = (int)Dircetions.DR_DOWNLEFT;
                    return true;
                }

                if ((nTargetCurrX > nCurrX) && (nTargetCurrY < nCurrY))
                {
                    direction = (int)Dircetions.DR_DOWNRIGHT;
                    return true;
                }

                if ((nTargetCurrX < nCurrX) && (nTargetCurrY > nCurrY))
                {
                    direction = (int)Dircetions.DR_UPLEFT;
                    return true;
                }

                if ((nTargetCurrX > nCurrX) && (nTargetCurrY > nCurrY))
                {
                    direction = (int)Dircetions.DR_UPRIGHT;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 怪物进行攻击
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="pool"></param>
        /// <param name="monster"></param>
        /// <param name="ticks"></param>
        /// <param name="count"></param>
        /// <param name="IndexOfMonsterAiAttack"></param>
        public void MonsterAttack(SocketListener sl, TCPOutPacketPool pool, Monster monster, IObject enemyObject, int direction, long ticks)
        {
            if (monster.IsMoving)
            {
                return;
            }

            if (null == enemyObject)
            {
                return;
            }

            Point enemyPos = enemyObject.CurrentPos;
            bool doAttackNow = false;
            if (monster._Action != GActions.Attack && monster.Action != GActions.PreAttack)
            {
                if (monster._ToExecSkillID > 0)
                {
                    doAttackNow = true;
                }
                else
                {
                    if (/*monster._Action == GActions.Stand && */(ticks - monster.LastAttackActionTick) >= monster.MaxAttackTimeSlot) //刘惠城 2014-06-23
                    {
                        doAttackNow = true;
                    }
                }
            }
            else //如果已经在攻击判断方向是否对，否则重发攻击命令
            {
                if (monster._Action == GActions.PreAttack || monster._Action == GActions.Stand) //刘惠城 2014-06-23
                {
                    //计算方向是否还一致
                    double newDirection = monsterMoving.CalcDirection(monster, enemyPos);
                    if (newDirection != monster.SafeDirection && monster.CurrentMagic < 1) //方向不同
                    {
                        direction = (int)newDirection;
                        doAttackNow = true;
                    }
                    else if (monster.EnemyTarget != enemyPos && monster.CurrentMagic < 1)
                    {
                        doAttackNow = true;
                    }
                }
            }

            if (doAttackNow)
            {
                InstantAttack(monster, direction, enemyPos);
            }
        }

        /// <summary>
        /// 获取指定的地图上的角色个数
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="mapCode"></param>
        /// <returns></returns>
        private int GetMapRolesCount(Dictionary<int, int> dict, int mapCode)
        {
            int roleNum = 0;
            if (dict.TryGetValue(mapCode, out roleNum))
            {
                return roleNum;
            }

            //判断如果指定的地图上的玩家人数大于0, 才搜索，否则不搜索
            roleNum = GameManager.ClientMgr.GetMapClientsCount(mapCode);
            dict[mapCode] = roleNum;
            return roleNum;
        }

        /// <summary>
        /// 简单的战斗调度
        /// </summary>
        public void DoMonsterAttack(SocketListener sl, TCPOutPacketPool pool, int IndexOfMonsterAiAttack, int mapCode = -1, int subMapCode = -1)
        {
            Dictionary<int, int> mapRolesNumDict = new Dictionary<int, int>();
            int count = 0;
            long ticks = DateTime.Now.Ticks / 10000;

            //这儿不用 foreach ，因为列表内容可能被删除，这样会导致不能枚举循环的异常
            Monster monster = null;
            List<object> objectsList = MyMonsterContainer._ObjectList;
            if (mapCode != -1)
            {
                objectsList = MyMonsterContainer.GetObjectsByMap(mapCode, subMapCode);
            }

            if (null == objectsList || objectsList.Count <= 0)
            {
                return;
            }

            //foreach (var monster in MyMonsterContainer._ObjectList)
            for (int nIndex = 0; nIndex < objectsList.Count; nIndex++)
            {
                //continue; //临时禁止

                try
                {
                    monster = objectsList[nIndex] as Monster;
                }
                catch (Exception)
                {
                    continue;
                }

                

                if (null == monster)
                {
                    continue;
                }


                if (monster.MonsterType == (int)MonsterTypes.JingJiChangRobot)
                {
                    continue;
                }                

                //if (monster.MonsterType == (int)MonsterTypes.XianFactionGuard)
                //{
                //    System.Diagnostics.Debug.WriteLine("abc");
                //}

                //if (monster.MonsterType == (int)MonsterTypes.MoFactionGuard)
                //{
                //    System.Diagnostics.Debug.WriteLine("abc");
                //}

                //通知外部处理，防止循环占用事件片过长
                count++;
                //这儿暂时屏蔽，因为外部定时器会循环调用
                //if (count % 10 == 0)
                //{
                //    if (null != CycleExecute)
                //    {
                //        CycleExecute(this, EventArgs.Empty);
                //    }
                //}

                //Monster monster = obj as Monster;

                //如果已经死亡，则不再调度
                if (monster.VLife <= 0 || !monster.Alive)
                {
                    continue;
                }

                //执行多段攻击的操作,移动到下面
                //SpriteAttack.ExecMagicsManyTimeDmageQueue(monster);

                //对于有主人的怪物，让他们追随自己的主人
                if (monster.OwnerClient != null)
                {
                    if (DispatchMonsterOwnedByRole(monster, sl, pool, ticks))
                    {
                        continue;
                    }
                }

                // 处理金币副本中怪的行走 -- 注意 即便没有玩家了 也要让怪能正常的行走！！  [6/11/2014 LiaoWei]
                if (monster.ManagerType == SceneUIClasses.EMoLaiXiCopy)
                {
                    EMoLaiXiCopySceneManager.MonsterMoveStepEMoLaiXiCopySenceCopyMap(monster);
                    continue;
                }
                else if (monster.MonsterType == (int)MonsterTypes.JUSTMOVE)
                {
                    GlodCopySceneManager.MonsterMoveStepGoldCopySceneCopyMap(monster);
                    continue;
                }

                //获取指定的地图上的角色个数
                int rolesNum = GetMapRolesCount(mapRolesNumDict, monster.MonsterZoneNode.MapCode);

                //补血补魔
                DoMonsterLifeMagicV(sl, pool, monster, ticks, rolesNum);

                //* tmp
                if (rolesNum <= 0) //如果地图上没有角色存在，则不调度怪物
                {
                    //失去锁定对象
                    monster.LockObject = -1;
                    monster.LockFocusTime = 0;
                    monster.VisibleClientsNum = 0;

                    //释放缓存
                    //monster.ReleaseBytesDataFromCaching(true);

                    continue;
                }

                //执行多段攻击的操作,移动到下面
                SpriteAttack.ExecMagicsManyTimeDmageQueue(monster);

                //如果怪物周边没有人，则也不做AI调度
                if (monster.VisibleClientsNum <= 0) //会不会导致，怪物追丢的问题????
                {
                    //失去锁定对象
                    monster.LockObject = -1;
                    monster.LockFocusTime = 0;

                    //释放缓存
                    //monster.ReleaseBytesDataFromCaching(false);

                    continue;
                }

                //校正怪物是否正在站立中
                //CheckMonsterStandStatus(monster);

                //检测是否在障碍物上
                if (CheckMonsterInObs(sl, pool, monster, ticks))
                {
                    continue;
                }

                //搜索视野范围内的可以攻击的对象
                SearchViewRange(sl, pool, monster, ticks, rolesNum);

                //尝试锁定搜索视野范围内怪物---这个函数内部会根据时间间隔 和 是否有锁定进行相关处理
                TryToLockObject(sl, pool, monster, ticks);

                //追击锁定的目标
                if (monster.MonsterInfo.SeekRange > 0 && null == monster.VisibleItemList) ///防止在被野蛮冲撞后，立刻就进行追击的动作
                {
                    //失去锁定对象
                    monster.LockObject = -1;
                    monster.LockFocusTime = 0;
                }

                //如果当前没有有锁定的目标则不处理, 如果是通天大盗和夺宝奇兵则不处理
                if (-1 == monster.LockObject ||
                    ((int)MonsterTypes.DaDao == monster.MonsterType ||
                    (int)MonsterTypes.QiBao == monster.MonsterType ||
                    (int)MonsterTypes.NoAttack == monster.MonsterType ||
                    (int)MonsterTypes.BiaoChe == monster.MonsterType ||
                    (int)MonsterTypes.ShengXiaoYunCheng == monster.MonsterType ||
                    (int)MonsterTypes.CaiJi == monster.MonsterType ||
                    (int)MonsterTypes.BloodCastleGateAndCrystal == monster.MonsterType ||      // 血色堡垒 门和水晶棺 [11/12/2013 LiaoWei]
                    (int)MonsterTypes.JUSTMOVE == monster.MonsterType   // 金币副本怪--只按路径行走 [6/12/2014 LiaoWei]
                    ))
                {

                    DoMonsterAI(sl, pool, monster, ticks, count, IndexOfMonsterAiAttack);

                    //释放缓存
                    //monster.ReleaseBytesDataFromCaching(false);

                    continue;
                }

                //锁定对象可能是玩家 也可能是 怪
                IObject lockObject = GameManager.ClientMgr.FindClient(monster.LockObject);

                //如过不是玩家，就查看怪物列表
                if (null == lockObject)
                {
                    lockObject = GameManager.MonsterMgr.FindMonster(monster.CurrentMapCode, monster.LockObject);
                }

                //锁定了怪 或者 玩家等
                if (null != lockObject)
                {

                    //检测锁定的对象是否有效(如果已经失效则不再处理)
                    if (!CheckLockObject(sl, pool, monster, lockObject, ticks))
                    {

                        //DoMonsterAI(sl, pool, monster, ticks); 本次循环不处理, 等待下次循环
                        continue;
                    }

                }
                else
                {
                    //失去锁定对象
                    monster.LockObject = -1;
                    monster.LockFocusTime = 0;
                    continue;
                }

                //如果怪物正在移动，则不处理
                if (monster.IsMoving)
                {
                    continue;
                }

                monster.isReturn = true;

                //以下的动作，如果怪物不是boss，则由奇数和偶数随机判断
                if (monster.MonsterType != (int)MonsterTypes.Boss)
                {
                    //将普通怪物分成四拨，这样怪物的行为稍稍不一样
                    /*if ((count + IndexOfMonsterAiAttack) % 4 == 0)
                    {
                        continue;
                    }*/
                }                

                //一旦攻击对象逃跑(不在自己的攻击距离内)则追踪
                int direction = 0;

                if (!TargetInAttackRange(monster, lockObject, out direction))
                {
                    GoToLockObject(sl, pool, monster, lockObject, ticks);
                }
                else
                {
                    MonsterAttack(sl, pool, monster, lockObject, direction, ticks);
                }
            }
        }

        #endregion 战斗调度算法(主界面线程调用)

        #region 角色拥有怪物的智能调度 包括移动 和 战斗

        /// <summary>
        /// 调度玩家召唤的自己拥有的怪物, 返回 false 表示外面还需要进一步处理
        /// </summary>
        /// <param name="monster"></param>
        public bool DispatchMonsterOwnedByRole(Monster monster, SocketListener sl, TCPOutPacketPool pool, long ticks)
        {
            if (monster.OwnerClient != null)
            {
                //如果怪物和主人不在同一个地图，则尝试将怪物移动到主人身边，包括跨地图，本来可以在
                //WaitingNotifyChangeMap == false 的时候做这个操作，但那样会在其他线程处理，可能有
                //冲突，简单点，直接在这判断,大概 每 4秒一次可以 了,但这儿有问题，因为切换地图时，客户端
                //会有延迟，推送给客户端的九宫格信息，只会在旧地图场景中处理，导致新地图场景没处理？
                //这儿的判断需要确保 不在副本的时候大家的副本编号都是 -1 
                if (monster.CurrentMapCode != monster.OwnerClient.CurrentMapCode
                    || monster.CurrentCopyMapID != monster.OwnerClient.CurrentCopyMapID)
                {
                    //如果角色还在切换地图中，直接返回，稍后再处理
                    if (monster.OwnerClient.ClientData.WaitingNotifyChangeMap)
                    {
                        return true;
                    }

                    MonsterZone newMonsterZone = GameManager.MonsterZoneMgr.GetDynamicMonsterZone(monster.OwnerClient.CurrentMapCode);

                    if (null != newMonsterZone)
                    {
                        int oldMapCode = monster.CurrentMapCode;

                        //System.Diagnostics.Debug.WriteLine(String.Format("地图切换 monster.RoleID={0} oldMapCode={1}  newMapCode={2}", monster.RoleID, oldMapCode, newMonsterZone.MapCode));
                        //进行怪物移动操作，首先将怪物从当前的monsterzone 移除 然后再加到新的monsterzone，同时修改mapid 和 copymap id
                        // 之后移动位置
                        monster.MonsterZoneNode.RemoveMonsterFromMyZone(monster);

                        //先修改副本地图ID，再添加到新刷怪区域
                        monster.CurrentCopyMapID = monster.OwnerClient.CurrentCopyMapID;
                        monster.LockObject = -1;
                        monster.LockFocusTime = 0;

                        //添加到新区域
                        newMonsterZone.AddMonsterToMyZone(monster);

                        //从旧地图移除

                        //monster.CurrentMapCode = monster.OwnerClient.CurrentMapCode;
 
                        //移动到主人身边随机位置
                        ChuanQiUtils.TransportTo(monster, (int)monster.OwnerClient.CurrentGrid.X, (int)monster.OwnerClient.CurrentGrid.Y, (Dircetions)((int)monster.Direction), oldMapCode);
                    }

                    return true;//这种情况发生了，怪物就别移动了
                }

                //如果自己离自己的主人很远[相距8格，差值是7]，则随机移动到主人身边 =>10，8太小了。
                if (Math.Abs(monster.CurrentGrid.X - monster.OwnerClient.CurrentGrid.X) >= 10 ||
                    Math.Abs(monster.CurrentGrid.Y - monster.OwnerClient.CurrentGrid.Y) >= 10
                    )
                {
                    monster.LockObject = -1;
                    monster.LockFocusTime = 0;

                    //移动到主人身边随机位置
                    ChuanQiUtils.TransportTo(monster, (int)monster.OwnerClient.CurrentGrid.X, (int)monster.OwnerClient.CurrentGrid.Y, (Dircetions)((int)monster.Direction), -1);

                    return true;
                }

                //移动中就先不管了
                if (monster.IsMoving)
                {
                    return true;
                }

                //有锁定的目标，则让怪进行攻击性操作
                if (monster.LockObject > 0)
                {
                    return false;
                }

                Boolean allowGo = true;

                //如果自己离自己的主人很近，且怪没锁定目标，则有60%的几率不移动
                if (Math.Abs(monster.CurrentGrid.X - monster.OwnerClient.CurrentGrid.X) <= 1 &&
                    Math.Abs(monster.CurrentGrid.Y - monster.OwnerClient.CurrentGrid.Y) <= 1
                    )
                {
                    if (Global.GetRandomNumber(0, 10001) >= 4000)
                    {
                        allowGo = false;
                    }
                }

                if (allowGo)
                {
                    //追击锁定的目标 [其实是追随自己的主人]
                    GoToLockObject(sl, pool, monster, monster.OwnerClient, ticks, true);

                    return true;
                }
            }

            return false;
        }

        #endregion 角色拥有怪物的智能调度 包括移动 和 战斗

        #region 补血补魔调度(主界面线程调用)

        /// <summary>
        /// 怪物补血补魔
        /// </summary>
        public void DoMonsterLifeMagicV(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks, int mapRoleNum)
        {
            ///处理道士加血的buffer，定时不计生命
            DBMonsterBuffer.ProcessDSTimeAddLifeNoShow(monster);

            ///处理道士释放毒的buffer, 定时伤害
            DBMonsterBuffer.ProcessDSTimeSubLifeNoShow(monster);

            //处理持续伤害的新的扩展buffer, 定时伤害
            DBMonsterBuffer.ProcessAllTimeSubLifeNoShow(monster);

            //如果还没到时间，则跳过
            if (ticks - monster.LastLifeMagicTick < (10 * 1000))
            {
                return;
            }

            monster.LastLifeMagicTick = ticks;
                
            bool doRelife = false;

            //判断如果血量少于最大血量
            if (monster.VLife < monster.MonsterInfo.VLifeMax)
            {
                doRelife = true;

                double percent = RoleAlgorithm.GetLifeRecoverValPercentV(monster) + DBMonsterBuffer.ProcessHuZhaoRecoverPercent(monster);
                double lifeMax = percent * monster.MonsterInfo.VLifeMax;
                lifeMax += monster.VLife;
                monster.VLife = Global.GMin(monster.MonsterInfo.VLifeMax, lifeMax);

                //GameManager.SystemServerEvents.AddEvent(string.Format("怪物加血, roleID={0}, Add={1}, Life={2}", monster.RoleID, percent * monster.MonsterInfo.VLifeMax, monster.VLife), EventLevels.Debug);
            }

            //判断如果魔量少于最大魔量
            if (monster.VMana < monster.MonsterInfo.VManaMax)
            {
                doRelife = true;

                double percent = RoleAlgorithm.GetMagicRecoverValPercentV(monster);
                double magicMax = percent * monster.MonsterInfo.VManaMax;
                magicMax += monster.VMana;
                monster.VMana = Global.GMin(monster.MonsterInfo.VManaMax, magicMax);

                //GameManager.SystemServerEvents.AddEvent(string.Format("怪物加魔, roleID={0}, Add={1}, Magic={2}", monster.RoleID, percent * monster.VManaMax, monster.VMana), EventLevels.Debug);
            }

            if (doRelife/* && mapRoleNum > 0*/)
            {
                //通知客户端怪已经加血加魔  
                List<Object> listObjs = Global.GetAll9Clients(monster);
                GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.SafeDirection, monster.VLife, monster.VMana, (int)TCPGameServerCmds.CMD_SPR_RELIFE, listObjs);

                GlobalEventSource.getInstance().fireEvent(new MonsterBlooadChangedEventObject(monster));
            }
        }

        #endregion 补血补魔调度(主界面线程调用)

        #region 死亡调度(主循环线程调用)

        /// <summary>
        /// 死亡怪物容器
        /// </summary>
        private List<Monster> ListDelayDeadMonster = new List<Monster>();

        public void AddDelayDeadMonster(Monster obj)
        {
            lock (ListDelayDeadMonster)
            {
                if (ListDelayDeadMonster.IndexOf(obj) < 0)
                {
                    obj.AddToDeadQueueTicks = DateTime.Now.Ticks / 10000;
                    ListDelayDeadMonster.Add(obj);
                }
            }
        }

        /// <summary>
        /// 立即让怪物消失
        /// </summary>
        /// <param name="obj"></param>
        public void DeadMonsterImmediately(Monster obj)
        {
            obj.OnDead();
            AddDelayDeadMonster(obj);
        }

        /// <summary>
        /// 怪物死亡调用
        /// </summary>
        public void DoMonsterDeadCall()
        {
            long nowTicks = DateTime.Now.Ticks / 10000;
            List<Monster> lsMonster = new List<Monster>();

            lock (ListDelayDeadMonster)
            {
                //if (ListDelayDeadMonster.Count > 0)
                //{
                //    lsMonster.AddRange(ListDelayDeadMonster);
                //    ListDelayDeadMonster.Clear();
                //}

                for (int i = 0; i < ListDelayDeadMonster.Count; i++)
                {
                    long maxTicks;
                    if ((int)MonsterTypes.Noraml == ListDelayDeadMonster[i].MonsterType)
                    {
                        maxTicks = 1500;
                    }
                    else if ((int)MonsterTypes.JingJiChangRobot == ListDelayDeadMonster[i].MonsterType || (int)MonsterTypes.Boss == ListDelayDeadMonster[i].MonsterType) 
                    {
                        //竞技场机器人或Boss死亡移除的时间加长为3000毫秒
                        maxTicks = 3000;
                    }
                    if ((int)MonsterTypes.CaiJi == ListDelayDeadMonster[i].MonsterType)
                    {
                        maxTicks = 0;
                    }
                    else 
                    {
                        maxTicks = 1500;
                    }

                    if (nowTicks - ListDelayDeadMonster[i].AddToDeadQueueTicks >= maxTicks)
                    {
                        lsMonster.Add(ListDelayDeadMonster[i]);
                    }
                }
            }

            //死亡调度
            foreach (var monster in lsMonster)
            {
                lock (ListDelayDeadMonster)
                {
                    ListDelayDeadMonster.Remove(monster);
                }

                monster.OnDead();
            }
        }

        #endregion

        #region 怪物技能管理

		/// <summary>
		/// 获取技能的攻击距离
		/// </summary>
		/// <returns></returns>
        protected int GetSkillAttackGridNum(Monster monster, int skillID)
		{
			if (skillID < 0) //物理攻击
			{
				return 1;
			}
			
            SystemXmlItem systemMagic = null;
            if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillID, out systemMagic))
            {
                return 1;
            }
			
			int skillType = systemMagic.GetIntValue("SkillType");
			if (SkillTypes.NormalAttack == skillType)
			{
				return 1;
			}
			
			if (SkillTypes.CiShaAttack == skillType)
			{
				return 2;
			}

            // 怪物追击完善 [5/4/2014 LiaoWei]
            int nDistance = systemMagic.GetIntValue("AttackDistance");
            if (nDistance > 0)
            {
                return nDistance / 100; // 每个格子 1米
            }


            return Global.MaxCache9XGridNum; //不限制距离
		}

        /// <summary>
        /// 执行唯一接口处理的攻击动作
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="direction"></param>
        /// <param name="enemyPos"></param>
        protected void InstantAttack(Monster monster, double direction, Point enemyPos)
		{
            int autoUseSkillID = monster.GetAutoUseSkillID();
            int attackNum = GetSkillAttackGridNum(monster, autoUseSkillID);
            int nRet = 0;

            if (autoUseSkillID > 0)
            {
                nRet = DoMagicAttack(monster, autoUseSkillID, monster.LockObject, true);
            }

            if (-1 != attackNum) //物理攻击，才有物理攻击动作
            {
                double newDirection = monsterMoving.CalcDirection(monster, enemyPos);
                monster.EnemyTarget = enemyPos;

                // System.Console.WriteLine(string.Format("InstantAttack播放技能动作{0}：{1}:{2}", monster.CurrentMagic, DateTime.Now.Ticks, monster.Action));

                //做攻击动作
                DoAttackAction(monster, newDirection);

                // 如果是带多段攻击、火墙的技能，立即解析执行
                if (monster.MagicFinish == -1)
                {
                    List<ManyTimeDmageItem> manyTimeDmageItemList = MagicsManyTimeDmageCachingMgr.GetManyTimeDmageItems(monster.CurrentMagic);
                    if (null != manyTimeDmageItemList && manyTimeDmageItemList.Count > 0)
                    {
                        Global.DoInjure(monster, monster.LockObject, monster.EnemyTarget);
                        return;
                    }

                    if (monster.CurrentMagic > 0)
                    {
                        SystemXmlItem systemMagic = null;
                        if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(monster.CurrentMagic, out systemMagic))
                        {
                            return;
                        }

                        if (systemMagic.GetIntValue("InjureType") == 1)
                        {
                            Global.DoInjure(monster, monster.LockObject, monster.EnemyTarget);
                        }
                    }

                    //List<MagicActionItem> magicActionItemList = null;
                    //GameManager.SystemMagicActionMgr.MagicActionsDict.TryGetValue(monster.CurrentMagic, out magicActionItemList);
                    //if (null != magicActionItemList && magicActionItemList.Count == 1)
                    //{
                    //    if (magicActionItemList[0].MagicActionID >= MagicActionIDs.MU_FIRE_WALL1
                    //        && magicActionItemList[0].MagicActionID <= MagicActionIDs.MU_FIRE_STRAIGHT)
                    //    {
                    //        Global.DoInjure(monster, monster.LockObject, monster.EnemyTarget);
                    //    }
                    //}
                }
            }

            //因为太消耗资源，暂时也用不到，所以暂时不触发
            //GlobalEventSource.getInstance().fireEvent(new MonsterAttackedEventObject(monster, monster.LockObject));
		}

        /// <summary>
        /// 做攻击动作
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="direction"></param>
        /// <param name="enemyPos"></param>
        public void DoAttackAction(Monster monster, double direction)
        {
            //如果已经死亡
            if (monster.VLife <= 0)
            {
                return;
            }

            // 昏迷(冻结！) [5/7/2014 LiaoWei]
            if (monster.IsMonsterDongJie())
                monster.Action = GActions.Stand;
            else
                monster.Action = GActions.Attack;

            Point enemyPos = monster.EnemyTarget;
            double newDirection = direction;

            //通知其他人自己开始做动作
            List<Object> listObjs = Global.GetAll9Clients(monster);
            GameManager.ClientMgr.NotifyOthersDoAction(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool,
                monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)newDirection, (int)monster.Action,
                (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)enemyPos.X, (int)enemyPos.Y, (int)TCPGameServerCmds.CMD_SPR_ACTTION, listObjs);

            //本地动作
            //monster.MoveToPos = new Point(-1, -1); //防止重入
            monster.DestPoint = new Point(-1, -1);
            Global.RemoveStoryboard(monster.Name);
            
            //if (monster.Action == GActions.Attack)
            //    monsterMoving.ChangeDirection(monster, newDirection);
        }

        /// <summary>
        /// 执行魔法攻击动作
        /// </summary>
        /// <param name="magicCode"></param>
        /// <param name="enemyObject"></param>
        /// <returns></returns>
		public int DoMagicAttack(Monster monster, int magicCode, int lockObject, bool doAttackAction = false)
		{			
            SystemXmlItem systemMagic = null;
            if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemMagic))
            {
                return -3;
            }           

            ////获取对象
            IObject enemyObject = Global.GetTargetObject(monster.MonsterZoneNode.MapCode, lockObject);

			monster.EnemyTarget = new Point(-1, -1);
			IObject targetSprite = null;
			if (1 == systemMagic.GetIntValue("TargetPos"))
			{
				//自身位置
			}
			else if (-1 == systemMagic.GetIntValue("TargetPos") || 2 == systemMagic.GetIntValue("TargetPos"))
			{
                int attackDistance = systemMagic.GetIntValue("AttackDistance");
                if (!SpriteAttack.JugeMagicDistance(systemMagic, monster as IObject, lockObject, (int)enemyObject.CurrentPos.X, (int)enemyObject.CurrentPos.Y, magicCode))
                {
                    return -1;
                }
    			targetSprite = enemyObject;
			}
			else if (3 == systemMagic.GetIntValue("TargetPos"))
			{
                //怪物无此功能
			}

            //判断技能的CD是否到时
            if (!monster.MyMagicCoolDownMgr.SkillCoolDown(magicCode))
            {
                return -1;
            }

            // 判断技能是否放完
            if (monster.MagicFinish < 0)
            {
                return -1;
            }

            //加入CD控制
            monster.MyMagicCoolDownMgr.AddSkillCoolDown(monster, magicCode);
			
            //设置技能
			monster.CurrentMagic = magicCode;
            monster.MagicFinish = -1;

            // System.Console.WriteLine(string.Format("开始释放技能{0}：{1}", magicCode, DateTime.Now.Ticks));

            // 通知其他人，自己开始准备攻击要准备的技能
            GameManager.ClientMgr.NotifyOthersMagicCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, 
                monster, monster.RoleID, monster.MonsterZoneNode.MapCode, magicCode, (int)TCPGameServerCmds.CMD_SPR_MAGICCODE);

            //System.Diagnostics.Debug.WriteLine(string.Format("monster.CurrentMagic = {0}, {1}", monster.CurrentMagic, DateTime.Now.Ticks  / 10000 / 1000));

			//Debug.WriteLine("Leader.CurrentMagic = " + magicCode);
			
			//ActionType 到底什么意思?
			//if (systemMagic.GetIntValue("ActionType") == 0)
            if (!doAttackAction)
			{
				if (null != targetSprite)
				{
					monster.EnemyTarget = targetSprite.CurrentPos;
				}
			}
			else /*if (systemMagic.GetIntValue("ActionType") == 1)*/
			{
				if (1 == systemMagic.GetIntValue("MagicType"))
				{
					if (null != targetSprite)
					{
						monster.EnemyTarget =targetSprite.CurrentPos;
					}
				}
				else
				{
					if (2 == systemMagic.GetIntValue("TargetPos"))
					{
						if (null != targetSprite)
						{
                            monster.EnemyTarget = targetSprite.CurrentPos;
						}
					}
				}
				
				MagicAttack(monster, magicCode, 1024, (1 == systemMagic.GetIntValue("TargetPos")));
			}
			return 0;
		}

        /// <summary>
        /// 执行魔法攻击
        /// </summary>
        /// <param name="magicCode"></param>
        /// <param name="magicRange"></param>
        /// <param name="notChangeDirection"></param>
        protected void MagicAttack(Monster monster, int magicCode, int magicRange, Boolean notChangeDirection = false)
		{
	        SpellCasting(monster, magicCode, notChangeDirection);
		}

		protected double CalcDirection(Monster monster, Point p)
		{
			return Global.GetDirectionByTan(p.X, p.Y, monster.Coordinate.X, monster.Coordinate.Y);
		}

		protected void SpellCasting(Monster monster, int magicCode, Boolean notChangeDirection = false)
		{
			double newDirection = monster.Direction;
			if (!notChangeDirection)
			{
				newDirection = CalcDirection(monster, monster.EnemyTarget);
			}

            // System.Console.WriteLine(string.Format("SpellCasting播放技能动作{0}：{1}:{2}", monster.CurrentMagic, DateTime.Now.Ticks, monster.Action));

            //做攻击动作
            DoAttackAction(monster, newDirection);
		}

        #endregion 怪物技能管理
    }
}
