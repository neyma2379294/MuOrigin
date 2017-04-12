using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 奇珍阁购买历史记录管理
    /// </summary>
    public class QiZhenGeBuManager
    {
        /// <summary>
        /// 历史记录列表
        /// </summary>
        private static List<QizhenGeBuItemData> QizhenGeBuItemDataList = null;

        /// <summary>
        /// 其次更新历史记录列表的时间
        /// </summary>
        private static long LastQueryTicks = 0;

        /// <summary>
        /// 获取奇珍阁购买历史记录
        /// </summary>
        /// <returns></returns>
        public static List<QizhenGeBuItemData> GetQizhenGeBuItemDataList(DBManager dbMgr)
        {
            long ticks = DateTime.Now.Ticks / 10000;
            if (ticks - LastQueryTicks >= (10 * 60 * 1000))
            {
                //重新查询
                QizhenGeBuItemDataList = DBQuery.QueryQizhenGeBuItemDataList(dbMgr);
                LastQueryTicks = ticks;
            }

            return QizhenGeBuItemDataList;
        }
    }
}
