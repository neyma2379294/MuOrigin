using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySQLDriverCS;
using System.Data;
using System.Windows;
using Server.Tools;
using System.Threading;

namespace GameDBServer.DB
{
    /// <summary>
    /// 数据库连接池类
    /// </summary>
    public class DBConnections
    {
        /// <summary>
        /// 设置客户端的连接字符集
        /// </summary>
        public static string dbNames = "";

        /// <summary>
        /// 控制获取连接数
        /// </summary>
        private Semaphore SemaphoreClients = null;

        /// <summary>
        /// 全局的数据库连接对象队列(长连接)
        /// </summary>
        private Queue<MySQLConnection> DBConns = new Queue<MySQLConnection>(100);

        /// <summary>
        /// 建立数据库连接池
        /// </summary>
        /// <param name="connStr"></param>
        public void BuidConnections(MySQLConnectionString connStr, int maxCount)
        {
            MySQLConnection dbConn = null;
            for (int i = 0; i < maxCount; i++)
            {
                dbConn = new MySQLConnection(connStr.AsString);

                try
                {
                    dbConn.Open(); //打开数据库连接
                }
                catch (Exception ex)
                {
                    //System.Windows.Application.Current.Dispatcher.Invoke((MethodInvoker)delegate
                    //{
                    // 格式化异常错误信息
                    DataHelper.WriteFormatExceptionLog(ex, "BuidConnections", false);
                    throw ex;
                    //});
                }

                MySQLCommand cmd = null;

                //执行链接查询的字符集
                if (!string.IsNullOrEmpty(dbNames))
                {
                    cmd = new MySQLCommand(string.Format("SET names '{0}'", dbNames), dbConn);
                    cmd.ExecuteNonQuery();
                }

                DBConns.Enqueue(dbConn); //添加到队列中
            }

            SemaphoreClients = new Semaphore(maxCount, maxCount);
        }

        /// <summary>
        /// 获取连接池中连接的个数
        /// </summary>
        /// <returns></returns>
        public int GetDBConnsCount()
        {
            lock (this.DBConns)
            {
                return this.DBConns.Count;
            }
        }

        /// <summary>
        /// 从连接队列中取一个空闲数据库连接
        /// </summary>
        /// <returns></returns>
        public MySQLConnection PopDBConnection()
        {
            SemaphoreClients.WaitOne(); //防止无法获取， 阻塞等待
            lock (this.DBConns)
            {
                return this.DBConns.Dequeue();
            }
        }

        /// <summary>
        /// 将数据库连接添加到空闲数据库连接中
        /// </summary>
        /// <param name="conn"></param>
        public void PushDBConnection(MySQLConnection conn)
        {
            lock (this.DBConns)
            {
                this.DBConns.Enqueue(conn);
            }

            SemaphoreClients.Release();
        }
    }
}
