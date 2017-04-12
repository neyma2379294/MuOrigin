using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySQLDriverCS;
using System.Data;
using System.Windows;
using Server.Tools;
using Server.Data;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 点将积分热门排行榜
    /// </summary>
    public class DJPointsHotList
    {
            #region 数据定义

            /// <summary>
            /// 数据列表
            /// </summary>
            private List<DJPointData> DJPointsHostList = new List<DJPointData>();

            /// <summary>
            /// 最后一次查询的时间
            /// </summary>
            private long LastQueryTicks = 0;

            #endregion 数据定义

            #region 方法定义

            /// <summary>
            /// 从数据库总加载点将积分的热门排行榜
            /// </summary>
            /// <param name="conn"></param>
            public List<DJPointData> GetDJPointsHostList(DBManager dbMgr)
            {
                List<DJPointData> djPointsHostList = null;
                lock (this)
                {
                    long ticks = DateTime.Now.Ticks / 10000;
                    if (ticks - LastQueryTicks <= (5 * 60 * 1000))
                    {
                        djPointsHostList = DJPointsHostList; //原子操作
                    }
                    else
                    {   
                        LastQueryTicks = ticks;

                        /// 查询点将积分数据热门排行榜
                        DJPointsHostList = DBQuery.QueryDJPointData(dbMgr);
                        djPointsHostList = DJPointsHostList;
                    }
                }

                return djPointsHostList;
            }

            #endregion 方法定义
    }
}
