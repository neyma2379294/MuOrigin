using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using Server.Protocol;
using Server.Data;
using Server.TCP;
using Server.Tools;
using System.Windows;
//using System.Windows.Documents;
using GameServer.Server;
using GameServer.Interface;

namespace GameServer.Logic
{
    /// <summary>
    /// 性能测试辅助
    /// </summary>
    public class PerformanceTest
    {
        private static object testLock1 = new object();
        private static object testLock2 = new object();
        private static object testLock3 = new object();
        private static byte[] _Buffer = new byte[4096];

        public static List<Object> GetClientsByMap(int mapCode)
        {
            List<Object> objsList = new List<Object>();
            //objsList = GameManager.ClientMgr.GetMapClients(mapCode);

            GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];

            GameClient client = null;
            int totalNum = 0;
            int index = 0;
            List<GameClient> clientsList = GameManager.ClientMgr.GetAllClients();
            while ((client = clientsList[index]) != null)
            {
                Point clientGrid = new Point((int)(client.ClientData.PosX / gameMap.MapGridWidth), (int)(client.ClientData.PosY / gameMap.MapGridHeight));
                Point monsterGrid = new Point(0, 0);
                if (Math.Abs(clientGrid.X - monsterGrid.X) <= Global.MaxCache9XGridNum &&
                    Math.Abs(clientGrid.Y - monsterGrid.Y) <= Global.MaxCache9YGridNum)
                {
                    objsList.Add(client);
                }
                totalNum++;
                if (totalNum > 1000)
                {
                    break;
                }
            }

            return objsList;
        }

        //* tmp
        //用于测试怪物移动时间消耗 ===>这个函数的行为
        public static bool NotifyOthersToMovingForMonsterTest(SocketListener sl, TCPOutPacketPool pool, IObject obj, int mapCode, int copyMapID, int roleID, long startMoveTicks, int currentX, int currentY, int action, int toX, int toY, int cmd, double moveCost = 1.0, String pathString = "", List<Object> objsList = null)
        {
            if (null == objsList)
            {
                if (null == obj)
                {
                    objsList = Global.GetAll9Clients2(mapCode, currentX, currentY, copyMapID);
                }
                else
                {
                    objsList = Global.GetAll9Clients(obj);
                }
            }


            objsList = GetClientsByMap(mapCode);

            //if (null == objsList) return true;//作为测试代码，不用管这儿

            string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", roleID, mapCode, action, toX, toY, moveCost, 0, currentX, currentY, startMoveTicks, pathString);//镖车，怪物和宠物的移动历史路径为空

            TCPOutPacket tcpOutPacket = null;

            int runNum = Global.GetRandomNumber(1, 10);

            lock (testLock1)
            {
                //模拟周围有10个人，构造1个数据包
                for (int n = 0; n < runNum; n++)
                {
                    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, cmd);

                    lock (testLock2)
                    {
                        lock (testLock3)
                        {
                            DataHelper.CopyBytes(_Buffer, 0, tcpOutPacket.GetPacketBytes(), 0, tcpOutPacket.PacketDataSize);
                        }
                    }

                    //还回tcpoutpacket
                    Global._TCPManager.TcpOutPacketPool.Push(tcpOutPacket);
                }
            }

            tcpOutPacket = null;

            return true;
        }
    }
}
