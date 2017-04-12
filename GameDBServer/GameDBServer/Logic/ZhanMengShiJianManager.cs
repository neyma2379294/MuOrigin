using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Data;
using GameDBServer.DB.DBController;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.Executor;
using GameDBServer.Server;
using GameDBServer.Server.CmdProcessor;
using GameDBServer.Core;

namespace GameDBServer.Logic
{

    /// <summary>
    /// 战盟事件静态常量参数
    /// </summary>
    public class ZhanMengShiJianConstants
    {
        //事件类型=====begin========

        //战盟创建
        public static readonly int CreateZhanMeng = 0;
        //脱离战盟
        public static readonly int LeaveZhanMeng = 1;
        //加入战盟
        public static readonly int JoinZhanMeng = 2;
        //玩家捐赠
        public static readonly int ZhanMengJuanZeng = 3;
        //职位变更
        public static readonly int ChangeZhiWu = 4;
        //建设升级
        public static readonly int ZhanMengLevelup = 5;
        //道具捐赠
        public static readonly int ZhanMengGooodsJuanZeng = 6;

        //事件类型======end=========

        //每页显示个数
        public static readonly int PageShowNum = 10;

        //每个帮会最大缓存个数
        public static readonly int MaxCacheNum = 100;

        //删除多余数据间隔时间
        public static readonly long deleteTime = TimeUtil.HOUR;
    }

    /// <summary>
    ///  战盟事件全局管理器
    /// </summary>
    public class ZhanMengShiJianManager : ZhanMengShiJianConstants, ScheduleTask, IManager
    {
        private static ZhanMengShiJianManager instance = new ZhanMengShiJianManager();

        private long executeSaveTime = TimeUtil.NOW() + deleteTime;

        //异步任务调度器
        private ScheduleExecutor executor = new ScheduleExecutor(1);

        //战盟时间数据缓存
        private Dictionary<int, List<ZhanMengShiJianData>> dataCache = new Dictionary<int, List<ZhanMengShiJianData>>();

        private ZhanMengShiJianManager(){ }

        public static ZhanMengShiJianManager getInstance()
        {
            return instance;
        }


        public bool initialize()
        {
            //战盟事件指令处理器
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_ADD_ZHANMENGSHIJIAN, ZhanMengShiJianCmdProcessor.getInstance());
            //战盟事件详情指令处理器
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_DB_ZHANMENGSHIJIAN_DETAIL, ZhanMengShiJianDetailCmdProcessor.getInstance());

            List<ZhanMengShiJianData> dataList = ZhanMengShiJianDBController.getInstance().getZhanMengShiJianDataList();

            if (null == dataList)
                return true ;

            foreach (ZhanMengShiJianData data in dataList)
            {
                List<ZhanMengShiJianData> _dataList = null;
                if (!dataCache.TryGetValue(data.BHID, out _dataList))
                {
                    _dataList = new List<ZhanMengShiJianData>();
                    dataCache.Add(data.BHID, _dataList);
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

            foreach (List<ZhanMengShiJianData> list in dataCache.Values)
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
        /// 根据帮会ID，页码查询战盟事件数据
        /// 页码下标从0开始
        /// </summary>
        /// <returns></returns>
        public List<ZhanMengShiJianData> getDetailByPageIndex(int bhId, int pageIndex)
        {

            int maxPageNum = MaxCacheNum / PageShowNum;

            if (pageIndex >= maxPageNum)
                return null;

            List<ZhanMengShiJianData> zhanmengDataList = null;

            //理论上不可能
            if (!dataCache.TryGetValue(bhId, out zhanmengDataList))
            {
                return null;
            }

            int minIndex = pageIndex * PageShowNum;
            int getNum = PageShowNum;

            if (minIndex >= zhanmengDataList.Count)
            {
                return null;
            }

            if (minIndex + getNum >= zhanmengDataList.Count)
            {
                getNum = zhanmengDataList.Count - minIndex;
            }

            if (getNum == 0)
            {
                return null;
            }

            return zhanmengDataList.GetRange(minIndex, getNum);

        }

        /// <summary>
        /// 定期删除多余的数据
        /// </summary>
        private void deleteData()
        {
            foreach (int bhId in dataCache.Keys)
            {
                List<ZhanMengShiJianData> list = null;
                dataCache.TryGetValue(bhId, out list);

                if(null == list || list.Count == 0)
                    continue;

                string minTime = list[list.Count - 1].CreateTime;

                int deleteCount = ZhanMengShiJianDBController.getInstance().delete(bhId, minTime);

            }
            
        }

        /// <summary>
        /// 战盟解散处理
        /// </summary>
        /// <param name="bhId"></param>
        public void onZhanMengJieSan(int bhId)
        {
            if (!dataCache.Remove(bhId))
            {
                return ;
            }

            ZhanMengShiJianDBController.getInstance().delete(bhId);
        }

        /// <summary>
        /// 战盟事件触发处理
        /// </summary>
        /// <param name="bhId"></param>
        /// <param name="roleName"></param>
        /// <param name="shijianType"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        public void onAddZhanMengShiJian(ZhanMengShiJianData data)
        {
            List<ZhanMengShiJianData> _dataList = null;

            lock (dataCache)
            {
                if (!dataCache.TryGetValue(data.BHID, out _dataList))
                {
                    _dataList = new List<ZhanMengShiJianData>();
                    dataCache.Add(data.BHID, _dataList);
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

            ZhanMengShiJianDBController.getInstance().insert(data);
        }
    }

}
