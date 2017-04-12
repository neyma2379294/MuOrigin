using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Server.Tools;

namespace GameServer.Server
{
    /// <summary>
    /// 连接到DBServer的客户端连接池类
    /// </summary>
    public class TCPClientPool
    {

        private static TCPClientPool instance = new TCPClientPool();

        private static TCPClientPool logInstance = new TCPClientPool();

        private TCPClientPool() { }

        public static TCPClientPool getInstance()
        {
            return instance;
        }

        public static TCPClientPool getLogInstance()
        {
            return logInstance;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="capacity"></param>
        public void initialize(int capacity)
        {
            this.pool = new Queue<TCPClient>(capacity);
        }

        /// <summary>
        /// 总的容量
        /// </summary>
        private int _InitCount = 0;

        /// <summary>
        /// 总的容量
        /// </summary>
        public int InitCount
        {
            get { return _InitCount; }
        }

        /// <summary>
        /// 错误的数量
        /// </summary>
        private int ErrCount = 0;

        /// <summary>
        /// 连接项的数量
        /// </summary>
        private int ItemCount = 0;

        /// <summary>
        /// 远端IP
        /// </summary>
        private string RemoteIP = "";

        /// <summary>
        /// 远端端口
        /// </summary>
        private int RemotePort = 0;

        /// <summary>
        /// 连接栈对象
        /// </summary>
        private Queue<TCPClient> pool;

        /// <summary>
        /// 控制获取连接数
        /// </summary>
        private Semaphore semaphoreClients;

        /// <summary>
        /// 主窗口对象
        /// </summary>
        public Program RootWindow
        {
            get;
            set;
        }

        private string ServerName = "";

        /// <summary>
        /// 初始化连接池
        /// </summary>
        /// <param name="count"></param>
        public void Init(int count, string ip, int port, string serverName)
        {
            ServerName = serverName;

            _InitCount = count;
            ItemCount = 0;
            RemoteIP = ip;
            RemotePort = port;
            this.semaphoreClients = new Semaphore(count, count);
            for (int i = 0; i < count; i++)
            {
                TCPClient tcpClient = new TCPClient() { RootWindow = RootWindow, ListIndex = ItemCount };

                RootWindow.AddDBConnectInfo(ItemCount, string.Format("{0}, 准备连接到{1}: {2}{3}", ItemCount, ServerName, RemoteIP, RemotePort));

                tcpClient.Connect(RemoteIP, RemotePort, ServerName);
                this.pool.Enqueue(tcpClient);
                ItemCount++;
            }
        }

        /// <summary>
        /// 删除连接池
        /// </summary>
        public void Clear()
        {
            lock (this.pool)
            {
                for (int i = 0; i < this.pool.Count; i++)
                {
                    TCPClient tcpClient = this.pool.ElementAt(i);
                    tcpClient.Disconnect();
                }

                this.pool.Clear();
            }
        }

        /// <summary>
        /// 连接池剩余的可用连接个数
        /// </summary>
        public int GetPoolCount()
        {
            lock (this.pool)
            {
                return this.pool.Count;
            }
        }

        /// <summary>
        ///  补充断开的连接
        /// </summary>
        public void Supply()
        {
            lock (this.pool)
            {
                if (ErrCount <= 0)
                {
                    return;
                }

                if (ErrCount > 0)
                {
                    try
                    {
                        TCPClient tcpClient = new TCPClient() { RootWindow = RootWindow, ListIndex = ItemCount };

                        RootWindow.AddDBConnectInfo(ItemCount, string.Format("{0}, 准备连接到{1}: {2}{3}", ItemCount, ServerName, RemoteIP, RemotePort));

                        ItemCount++;

                        tcpClient.Connect(RemoteIP, RemotePort, ServerName);
                        this.pool.Enqueue(tcpClient);
                        ErrCount--;

                        this.semaphoreClients.Release();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 获取一个连接
        /// </summary>
        /// <returns></returns>
        public TCPClient Pop()
        {
            this.semaphoreClients.WaitOne(); //防止无法获取， 阻塞等待
            lock (this.pool)
            {
                return this.pool.Dequeue();
            }
        }

        /// <summary>
        /// 压入一个连接
        /// </summary>
        /// <param name="tcpClient"></param>
        /// <returns></returns>
        public void Push(TCPClient tcpClient)
        {
            //如果是已经无效的连接，则不再放入缓存池
            if (!tcpClient.IsConnected())
            {
                lock (this.pool)
                {
                    ErrCount++;
                }

                return;
            }

            lock (this.pool)
            {
                this.pool.Enqueue(tcpClient);
            }

            this.semaphoreClients.Release();
        }
    }
}
