using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Task.Tool;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 全局管理定义
    /// </summary>
    public class GameDBManager
    {
        public const int StatisticsMode = 3;

        /// <summary>
        /// 游戏的区号
        /// </summary>
        public static int ZoneID = 1;

        /// <summary>
        /// 数据库名
        /// </summary>
        public static string DBName = "mu_game";

        /// <summary>
        /// 服务器端日志事件
        /// </summary>
        public static ServerEvents SystemServerSQLEvents = new ServerEvents() { EventRootPath = "SQLs", EventPreFileName = "sql" };

        /// <summary>
        /// 点将积分热门排行榜
        /// </summary>
        public static DJPointsHotList SysDJPointsHotList = new DJPointsHotList();

        /// <summary>
        /// 公告消息管理
        /// </summary>
        public static BulletinMsgManager BulletinMsgMgr = new BulletinMsgManager();

        /// <summary>
        /// 游戏配置对象
        /// </summary>
        public static GameConfig GameConfigMgr = new GameConfig();

        /// <summary>
        /// 帮会军旗管理对象
        /// </summary>
        public static BangHuiJunQiManager BangHuiJunQiMgr = new BangHuiJunQiManager();

        /// <summary>
        /// 帮会领地管理对象
        /// </summary>
        public static BangHuiLingDiManager BangHuiLingDiMgr = new BangHuiLingDiManager();

        /// <summary>
        /// 写数据库日志对象
        /// </summary>
        //public static EventsSet DBEventsWriter = new EventsSet();

        /// <summary>
        /// 数据库自增长单区基础范围步长值 默认 一百万
        /// </summary>
        public static int DBAutoIncreaseStepValue = 1000000;

        /// <summary>
        /// 是否立即删除物品表的个数为0的数据行
        /// </summary>
        public static bool Flag_t_goods_delete_immediately = false;

        /// <summary>
        /// 根据需求 对充值排行进行管理
        /// </summary>
        public static DayRechargeRankManager DayRechargeRankMgr = new DayRechargeRankManager();


    }
}
