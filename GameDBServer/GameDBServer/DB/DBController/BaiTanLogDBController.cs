using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;

namespace GameDBServer.DB.DBController
{
    /// <summary>
    /// 摆摊日志数据控制器
    /// </summary>
    public class BaiTanLogDBController : DBController<BaiTanLogItemData>
    {
        private static BaiTanLogDBController instance = new BaiTanLogDBController();

        private BaiTanLogDBController() : base() { }

        public static BaiTanLogDBController getInstance()
        {
            return instance;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        public List<BaiTanLogItemData> getBaiTanLogItemDataList()
        {
            string sql = "select * from t_baitanbuy order by rid, buytime desc";
            return this.queryForList(sql);
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="dataList"></param>
        public int insert(List<BaiTanLogItemData> dataList)
        {
            int i = 0;

            foreach (BaiTanLogItemData data in dataList)
            {
                i += insert(data);
            }

            return i;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="data"></param>
        public int insert(BaiTanLogItemData data)
        {
            string sql = string.Format("insert into t_baitanbuy (rid,otherroleid,otherrname,goodsid,goodsnum,forgelevel,totalprice,leftyuanbao,buytime,yinliang,left_yinliang,tax,excellenceinfo) values ({0},{1},'{2}',{3},{4},{5},{6},{7},'{8}',{9},{10},{11},{12});",
                data.rid, data.OtherRoleID, data.OtherRName, data.GoodsID, data.GoodsNum, data.ForgeLevel, data.TotalPrice, data.LeftYuanBao, data.BuyTime, data.YinLiang, data.LeftYinLiang, data.Tax, data.Excellenceinfo);
            return this.insert(sql); 
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="bhId"></param>
        /// <param name="createTime"></param>
        public int delete(int rid, string buytime)
        {
            string sql = string.Format("delete from t_baitanbuy where rid={0} and buytime<'{1}';", rid, buytime);
            return this.delete(sql);
        }

        /// <summary>
        /// 删除角色所有信息
        /// </summary>
        /// <param name="bhId"></param>
        /// <returns></returns>
        public int delete(int rid)
        {
            string sql = string.Format("delete from t_baitanbuy where rid={0};", rid);
            return this.delete(sql);
        }
    }
}
