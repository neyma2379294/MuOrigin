using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using GameDBServer.DB.DBController;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.Executor;
using GameDBServer.Server;
using GameDBServer.Server.CmdProcessor;
using GameDBServer.Core;

namespace GameDBServer.Logic.BaiTan
{
    /// <summary>
    ///  摆摊全局管理器
    /// </summary>
    public class BaiTanManager :  ScheduleTask, IManager
    {
        //每页显示个数
        public static readonly int PageShowNum = 10;

        //每个角色最大缓存个数
        public static readonly int MaxCacheNum = 100;

        //删除多余数据间隔时间
        public static readonly long deleteTime = TimeUtil.HOUR;

        private static BaiTanManager instance = new BaiTanManager();

        private long executeSaveTime = TimeUtil.NOW() + deleteTime;

        //异步任务调度器
        private ScheduleExecutor executor = new ScheduleExecutor(1);

        //战盟时间数据缓存
        private Dictionary<int, List<BaiTanLogItemData>> dataCache = new Dictionary<int, List<BaiTanLogItemData>>();

        private BaiTanManager(){ }

        public static BaiTanManager getInstance()
        {
            return instance;
        }


        public bool initialize()
        {
            List<BaiTanLogItemData> dataList = BaiTanLogDBController.getInstance().getBaiTanLogItemDataList();

            if (null == dataList)
                return true ;

            foreach (BaiTanLogItemData data in dataList)
            {
                List<BaiTanLogItemData> _dataList = null;
                //卖出日志
                if (!dataCache.TryGetValue(data.rid, out _dataList))
                {
                    _dataList = new List<BaiTanLogItemData>();
                    dataCache.Add(data.rid, _dataList);
                }

                if (_dataList.Count >= MaxCacheNum)
                    continue;

                _dataList.Add(data);

                //购买日志
                if (!dataCache.TryGetValue(data.OtherRoleID, out _dataList))
                {
                    _dataList = new List<BaiTanLogItemData>();
                    dataCache.Add(data.OtherRoleID, _dataList);
                }

                if (_dataList.Count >= MaxCacheNum)
                    continue;

                _dataList.Add(data);
            }

            return true;
        }

        public bool startup()
        {
            executor.start();

            //延迟2秒执行，每5秒调度一次
            executor.scheduleExecute(this, 2 * TimeUtil.SECOND, 5 * TimeUtil.SECOND);

            return true;
        }

        public bool showdown()
        {
            executor.stop();
            deleteData();

            return true;
        }

        public bool destroy()
        {
            executor = null;

            if (dataCache == null)
                return true;

            foreach (List<BaiTanLogItemData> list in dataCache.Values)
            {
                list.Clear();
            }

            dataCache.Clear();
            dataCache = null;

            return true;
        }


        public void run()
        {
            if (TimeUtil.NOW() > executeSaveTime)
            {
                deleteData();
                executeSaveTime += deleteTime;
            }
        }

        /// <summary>
        /// 根据角色ID，页码查询摆摊日志数据
        /// 页码下标从0开始
        /// </summary>
        /// <returns></returns>
        public List<BaiTanLogItemData> getDetailByPageIndex(int bhId, int pageIndex)
        {

            int maxPageNum = MaxCacheNum / PageShowNum;

            if (pageIndex >= maxPageNum)
                return null;

            List<BaiTanLogItemData> baiTanLogItemDataList = null;

            //理论上不可能
            if (!dataCache.TryGetValue(bhId, out baiTanLogItemDataList))
            {
                return null;
            }

            int minIndex = pageIndex * PageShowNum;
            int getNum = PageShowNum;

            if (minIndex >= baiTanLogItemDataList.Count)
            {
                return null;
            }

            if (minIndex + getNum >= baiTanLogItemDataList.Count)
            {
                getNum = baiTanLogItemDataList.Count - minIndex;
            }

            if (getNum == 0)
            {
                return null;
            }

            return baiTanLogItemDataList.GetRange(minIndex, getNum);

        }

        /// <summary>
        /// 定期删除多余的数据
        /// </summary>
        private void deleteData()
        {
            foreach (int rid in dataCache.Keys)
            {
                List<BaiTanLogItemData> list = null;
                dataCache.TryGetValue(rid, out list);

                if(null == list || list.Count == 0)
                    continue;

                string minTime = list[list.Count - 1].BuyTime;

                int deleteCount = BaiTanLogDBController.getInstance().delete(rid, minTime);

            }
            
        }

        /// <summary>
        /// 摆摊日志触发处理
        /// </summary>
        /// <param name="bhId"></param>
        /// <param name="roleName"></param>
        /// <param name="shijianType"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        public void onAddBaiTanLog(BaiTanLogItemData data)
        {
            List<BaiTanLogItemData> _dataList = null;
            List<BaiTanLogItemData> _dataList2 = null;

            lock (dataCache)
            {
                if (!dataCache.TryGetValue(data.rid, out _dataList))
                {
                    _dataList = new List<BaiTanLogItemData>();
                    dataCache.Add(data.rid, _dataList);
                }
                if (!dataCache.TryGetValue(data.OtherRoleID, out _dataList2))
                {
                    _dataList2 = new List<BaiTanLogItemData>();
                    dataCache.Add(data.OtherRoleID, _dataList2);
                }
            }

            lock (_dataList)
            {
                //将最新的数据插入到最前面 
                _dataList.Insert(0, data);

                //超过规定容量，删除最后一个
                if (_dataList.Count > MaxCacheNum)
                {
                    _dataList.RemoveAt(_dataList.Count - 1);
                }
            }

            lock (_dataList2)
            {
                //将最新的数据插入到最前面 
                _dataList2.Insert(0, data);

                //超过规定容量，删除最后一个
                if (_dataList2.Count > MaxCacheNum)
                {
                    _dataList2.RemoveAt(_dataList2.Count - 1);
                }
            }

            BaiTanLogDBController.getInstance().insert(data);

            
        }
    }
}
