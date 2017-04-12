using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 副本的顺序ID管理
    /// </summary>
    public class FuBenSeqIDMgr
    {
        /// <summary>
        /// 多线程访问的锁对象
        /// </summary>
        private static object Mutex = new object();

        /// <summary>
        /// 基础的副本顺序ID值
        /// </summary>
        private static int BaseFuBenSeqID = 1;

        /// <summary>
        /// 获取一个新的副本顺序ID值
        /// </summary>
        public static int GetFuBenSeqID()
        {
            lock (Mutex)
            {
                return BaseFuBenSeqID++;
            }
        }
    }
}
