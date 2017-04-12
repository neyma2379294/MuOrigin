using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 自己的弱引用实现
    /// </summary>
    public class MyWeakReference
    {
        /// <summary>
        /// 线程间的锁对象
        /// </summary>
        private object _ThreadMutex = new object();


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="target"></param>
        public MyWeakReference(object target)
        {
            _Target = target;
        }

        /// <summary>
        /// 是否存活?
        /// </summary>
        public bool IsAlive
        {
            get 
            {
                lock (_ThreadMutex)
                {
                    return (null != _Target);
                }
            }
        }

        /// <summary>
        /// 目标对象
        /// </summary>
        private object _Target = null;

        /// <summary>
        /// 目标对象
        /// </summary>
        public object Target
        {
            get
            {
                lock (_ThreadMutex)
                {
                    return _Target;
                }
            }

            set
            {
                lock (_ThreadMutex)
                {
                    _Target = value;
                }
            }
        }
    }
}
