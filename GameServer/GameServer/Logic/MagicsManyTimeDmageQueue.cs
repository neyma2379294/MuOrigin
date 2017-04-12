using GameServer.Core.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Logic
{
    /// <summary>
    /// 多段伤害队列项
    /// </summary>
    public class ManyTimeDmageQueueItem
    {
        /// <summary>
        /// 执行的时间点
        /// </summary>
        public long ToExecTicks = 0;

        /// <summary>
        /// 敌人的ID
        /// </summary>
        public int enemy = -1;

        /// <summary>
        /// 敌人的坐标X
        /// </summary>
        public int enemyX = 0;
        
        /// <summary>
        /// 敌人的坐标Y
        /// </summary>
        public int enemyY = 0;

        /// <summary>
        /// 真实的敌人坐标X
        /// </summary>
        public int realEnemyX = 0;

        /// <summary>
        /// 真是的敌人坐标Y
        /// </summary>
        public int realEnemyY = 0;
        
        /// <summary>
        /// 技能ID
        /// </summary>
        public int magicCode = 0;

        /// <summary>
        /// 分段顺序
        /// </summary>
        public int manyRangeIndex = 0;

        /// <summary>
        /// 分段伤害比例
        /// </summary>
        public double manyRangeInjuredPercent = 1.0;
    }

    /// <summary>
    /// 多段伤害队列管理
    /// </summary>
    public class MagicsManyTimeDmageQueue
    {
        /// <summary>
        /// 等待执行的分段技能项
        /// </summary>
        private List<ManyTimeDmageQueueItem> ManyTimeDmageQueueItemList = new List<ManyTimeDmageQueueItem>();

        /// <summary>
        /// 添加一个新的项
        /// </summary>
        /// <param name="manyTimeDmageQueueItem"></param>
        public void AddManyTimeDmageQueueItem(ManyTimeDmageQueueItem manyTimeDmageQueueItem)
        {
            lock (ManyTimeDmageQueueItemList)
            {
                ManyTimeDmageQueueItemList.Add(manyTimeDmageQueueItem);
            }
        }

        /// <summary>
        /// 获取现在的项数
        /// </summary>
        /// <param name="manyTimeDmageQueueItem"></param>
        public int GetManyTimeDmageQueueItemNum()
        {
            lock (ManyTimeDmageQueueItemList)
            {
                return ManyTimeDmageQueueItemList.Count;
            }
        }

        /// <summary>
        /// 获取可以执行的项
        /// </summary>
        /// <returns></returns>
        public List<ManyTimeDmageQueueItem> GetCanExecItems()
        {
            long ticks = TimeUtil.NOW();
            List<ManyTimeDmageQueueItem> canExecItemList = new List<ManyTimeDmageQueueItem>();
            lock (ManyTimeDmageQueueItemList)
            {
                for (int i = 0; i < ManyTimeDmageQueueItemList.Count; i++)
                {
                    if (ticks >= ManyTimeDmageQueueItemList[i].ToExecTicks)
                    {
                        canExecItemList.Add(ManyTimeDmageQueueItemList[i]);
                    }
                }

                for (int i = 0; i < canExecItemList.Count; i++)
                {
                    ManyTimeDmageQueueItemList.Remove(canExecItemList[i]);
                }
            }

            return canExecItemList;
        }

        /// <summary>
        /// 清空队列
        /// </summary>
        public void Clear()
        {
            lock (ManyTimeDmageQueueItemList)
            {
                ManyTimeDmageQueueItemList.Clear();
            }
        }
    }
}
