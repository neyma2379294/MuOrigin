using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySQLDriverCS;
using System.Data;
using System.Windows;
using Server.Tools;
using System.Threading;

namespace GameDBServer.DB
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class DBUserInfo
    {
        #region 基本数据

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID
        {
            get;
            set;
        }

        //角色ID列表
        private List<int> _ListRoleIDs = new List<int>();

        /// <summary>
        /// 角色ID列表
        /// </summary>
        public List<int> ListRoleIDs
        {
            get { return _ListRoleIDs; }
        }

        //角色性别列表
        private List<int> _ListRoleSexes = new List<int>();

        /// <summary>
        /// 角色性别列表
        /// </summary>
        public List<int> ListRoleSexes
        {
            get { return _ListRoleSexes; }
        }

        //角色职业列表
        private List<int> _ListRoleOccups = new List<int>();

        /// <summary>
        /// 角色职业列表
        /// </summary>
        public List<int> ListRoleOccups
        {
            get { return _ListRoleOccups; }
        }

        //角色名称列表
        private List<string> _ListRoleNames = new List<string>();

        /// <summary>
        /// 角色名称列表
        /// </summary>
        public List<string> ListRoleNames
        {
            get { return _ListRoleNames; }
        }

        //角色级别列表
        private List<int> _ListRoleLevels = new List<int>();

        /// <summary>
        /// 角色级别列表
        /// </summary>
        public List<int> ListRoleLevels
        {
            get { return _ListRoleLevels; }
        }

        //角色区列表
        private List<int> _ListRoleZoneIDs = new List<int>();

        /// <summary>
        /// 角色区列表
        /// </summary>
        public List<int> ListRoleZoneIDs
        {
            get { return _ListRoleZoneIDs; }
        }

        // 增加转生计数 [12/14/2013 LiaoWei]
        // 转生计数列表
        private List<int> _ListRoleChangeLifeCount = new List<int>();

        /// <summary>
        /// 转生计数列表
        /// </summary>
        public List<int> ListRoleChangeLifeCount
        {
            get { return _ListRoleChangeLifeCount; }
        }

        /// <summary>
        /// 用户的元宝
        /// </summary>
        public int Money
        {
            get;
            set;
        }

        /// <summary>
        /// 用户的充值的真实钱数(只增加不减少)
        /// </summary>
        public int RealMoney
        {
            get;
            set;
        }

        /// <summary>
        /// 计算充值积分的活动ID
        /// </summary>
        public int GiftID
        {
            get;
            set;
        }

        /// <summary>
        /// 充值的的积分
        /// </summary>
        public int GiftJiFen
        {
            get;
            set;
        }

        /// <summary>
        /// 推送ID [4/23/2014 LiaoWei]
        /// </summary>
        public string PushMessageID
        {
            get;
            set;
        }

        #endregion 基本数据

        #region 扩展数据

        /// <summary>
        /// 上次使用访问的时间
        /// </summary>
        private long _LastReferenceTicks = DateTime.Now.Ticks / 10000;

        /// <summary>
        /// 上次使用访问的时间
        /// </summary>
        public long LastReferenceTicks
        {
            get { return _LastReferenceTicks; }
            set { _LastReferenceTicks = value; }
        }

        #endregion 扩展数据

        #region 从数据库查询信息

        /// <summary>
        /// 从数据库中查询
        /// </summary>
        public bool Query( MySQLConnection conn, string userID )
        {
            LogManager.WriteLog(LogTypes.Info, string.Format("从数据库加载用户数据: {0}", userID));

            this.UserID = userID;

            MySQLSelectCommand cmd = new MySQLSelectCommand(conn, new string[] { "rid", "userid", "rname", "sex", "occupation", "level", "zoneid", "changelifecount" }, new string[] { "t_roles" }, new object[,] { { "userid", "=", userID }, { "isdel", "=", 0 } }, null, null);
            if (cmd.Table.Rows.Count > 0)
            {
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    ListRoleIDs.Add(Convert.ToInt32(cmd.Table.Rows[i]["rid"].ToString()));
                    ListRoleNames.Add(cmd.Table.Rows[i]["rname"].ToString());
                    ListRoleSexes.Add(Convert.ToInt32(cmd.Table.Rows[i]["sex"].ToString()));
                    ListRoleOccups.Add(Convert.ToInt32(cmd.Table.Rows[i]["occupation"].ToString()));
                    ListRoleLevels.Add(Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString()));
                    ListRoleZoneIDs.Add(Convert.ToInt32(cmd.Table.Rows[i]["zoneid"].ToString()));
                    ListRoleChangeLifeCount.Add(Convert.ToInt32(cmd.Table.Rows[i]["changelifecount"].ToString()));
                }
            }

            this.Money = 0; //不充值的情况下，无记录
            cmd = new MySQLSelectCommand(conn, new string[] { "money", "realmoney", "giftid", "giftjifen" }, new string[] { "t_money" }, new object[,] { { "userid", "=", userID } }, null, null);
            if (cmd.Table.Rows.Count > 0)
            {
                this.Money = Convert.ToInt32(cmd.Table.Rows[0]["money"].ToString());
                this.RealMoney = Convert.ToInt32(cmd.Table.Rows[0]["realmoney"].ToString());
                this.GiftID = Convert.ToInt32(cmd.Table.Rows[0]["giftid"].ToString());
                this.GiftJiFen = Convert.ToInt32(cmd.Table.Rows[0]["giftjifen"].ToString());
            }

            // 推送信息 [4/23/2014 LiaoWei]
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "userid", "pushid", "lastlogintime" },
                     new string[] { "t_pushmessageinfo" }, new object[,] { { "userid", "=", userID } }, null, null);
            
            if (cmd.Table.Rows.Count > 0)
            {
                this.PushMessageID = cmd.Table.Rows[0]["pushid"].ToString();
            }

            return true;
        }

        #endregion 从数据库查询信息
    }
}
