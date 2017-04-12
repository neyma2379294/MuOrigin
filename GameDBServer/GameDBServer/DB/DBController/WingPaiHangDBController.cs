using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using MySQLDriverCS;
using GameDBServer.Logic;
using GameDBServer.Logic.Wing;
using System.Data;
using Server.Tools;

namespace GameDBServer.DB.DBController
{
    /// <summary>
    /// 翅膀排行榜数据处理类
    /// </summary>
    public class WingPaiHangDBController : DBController<WingRankingInfo>
    {
        private static WingPaiHangDBController instance = new WingPaiHangDBController();

        private WingPaiHangDBController() : base() { }

        public static WingPaiHangDBController getInstance()
        {
            return instance;
        }

        /// <summary>
        /// 根据ID获取翅膀排行榜数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public WingRankingInfo getWingDataById(int Id)
        {
            string sql = string.Format("select * from t_wings where rid = {0};",Id);

            return this.queryForObject(sql);
        }

        /// <summary>
        /// 获取排名前500的万魔塔数据
        /// </summary>
        /// <returns></returns>
        public List<WingRankingInfo> getPlayerWingDataList()
        {
            // string sql = string.Format("select * from t_wanmota where passLayerCount > 0 order by passLayerCount desc, flushTime asc limit {0};", WanMoTaManager.RankingList_Max_Num);
            string sql = string.Format("select * from t_wings order by wingid desc, forgeLevel desc, addtime asc limit {0};", WingPaiHangManager.RankingList_Max_Num);

            return this.queryForList(sql);
        }
    }
}
