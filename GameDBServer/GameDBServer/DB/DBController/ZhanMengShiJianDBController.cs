using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Data;

namespace GameDBServer.DB.DBController
{
    public class ZhanMengShiJianDBController : DBController<ZhanMengShiJianData>
    {
        private static ZhanMengShiJianDBController instance = new ZhanMengShiJianDBController();

        private ZhanMengShiJianDBController() : base() { }

        public static ZhanMengShiJianDBController getInstance()
        {
            return instance;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        public List<ZhanMengShiJianData> getZhanMengShiJianDataList()
        {
            string sql = "select * from t_zhanmengshijian order by bhid, createTime desc";
            return this.queryForList(sql);
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="dataList"></param>
        public int insert(List<ZhanMengShiJianData> dataList)
        {
            int i = 0;

            foreach (ZhanMengShiJianData data in dataList)
            {
                i += insert(data);
            }

            return i;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="data"></param>
        public int insert(ZhanMengShiJianData data)
        {
            string sql = string.Format("insert into t_zhanmengshijian (bhId,shijianType,roleName,createTime,subValue1,subValue2,subValue3) values ({0},{1},'{2}','{3}',{4},{5},{6});", data.BHID, data.ShiJianType, data.RoleName, data.CreateTime, data.SubValue1, data.SubValue2, data.SubValue3);
            return this.insert(sql); 
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="bhId"></param>
        /// <param name="createTime"></param>
        public int delete(int bhId, string createTime)
        {
            string sql = string.Format("delete from t_zhanmengshijian where bhId={0} and createTime<'{1}';", bhId, createTime);
            return this.delete(sql);
        }

        /// <summary>
        /// 删除战盟所有信息
        /// </summary>
        /// <param name="bhId"></param>
        /// <returns></returns>
        public int delete(int bhId)
        {
            string sql = string.Format("delete from t_zhanmengshijian where bhId={0};", bhId);
            return this.delete(sql);
        }
    }
}
