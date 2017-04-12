using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using GameDBServer.DB;

namespace Server.Data
{
    /// <summary>
    /// 翅膀排行榜数据
    /// </summary>
    [ProtoContract]
    public class WingRankingInfo
    {
        public WingRankingInfo()
        {
            this.rankingData = new PlayerWingRankingData(this);
        }

        /// <summary>
        /// 翅膀塔排行榜数据
        /// </summary>
        private PlayerWingRankingData rankingData;

        /// <summary>
        /// 角色ID
        /// </summary>
        [ProtoMember(1)]
        [DBMapping(ColumnName = "rid")]
        public int nRoleID;

        /// <summary>
        /// 角色名
        /// </summary>
        [ProtoMember(2)]
        [DBMapping(ColumnName = "rname")]
        public string strRoleName;

        /// <summary>
        /// 角色职业
        /// </summary>
        [ProtoMember(3)]
        [DBMapping(ColumnName = "occupation")]
        public int nOccupation;

        /// <summary>
        /// 进阶数
        /// </summary>
        [ProtoMember(4)]
        [DBMapping(ColumnName = "wingid")]
        public int nWingID = 0;

        // <summary>
        /// 升星数
        /// </summary>
        [ProtoMember(5)]
        [DBMapping(ColumnName = "forgeLevel")]
        public int nStarNum = 0;

        /// <summary>
        /// 翅膀创建时间
        /// </summary>
        [ProtoMember(6)]
        [DBMapping(ColumnName = "addtime")]
        public String strAddTime = "";

        /// <summary>
        /// 获取玩家万魔塔排行榜数据
        /// </summary>
        /// <returns></returns>
        public PlayerWingRankingData getPlayerWingRankingData()
        {
            return rankingData;
        }
    }

    /// <summary>
    /// 翅膀排行榜数据封装类
    /// </summary>
    [ProtoContract]
    public class PlayerWingRankingData : IComparable<PlayerWingRankingData>
    {
        public WingRankingInfo wingData;

        public PlayerWingRankingData(WingRankingInfo wingData)
        {
            this.wingData = wingData;
        }

        /// <summary>
        /// 玩家ID
        /// </summary>
        [ProtoMember(1)]
        public int roleId 
        {
            get { return wingData.nRoleID; }
        }

        /// <summary>
        /// 玩家名称
        /// </summary>
        [ProtoMember(2)]
        public string roleName
        {
            get { return wingData.strRoleName; }
        }

        /// <summary>
        /// 玩家职业
        /// </summary>
        [ProtoMember(3)]
        public int Occupation
        {
            get { return wingData.nOccupation; }
        }

        /// <summary>
        /// 翅膀添加时间
        /// </summary>
        [ProtoMember(4)]
        public string WingAddTime
        {
            get { return wingData.strAddTime; }
        }

        /// <summary>
        /// 翅膀进阶数
        /// </summary>
        [ProtoMember(5)]
        public int WingID
        {
            get { return wingData.nWingID; }
        }

        /// <summary>
        /// 翅膀升星数
        /// </summary>
        [ProtoMember(6)]
        public int WingStarNum
        {
            get { return wingData.nStarNum; }
        }

        private PaiHangItemData paiHangItemData = new PaiHangItemData();

        public PaiHangItemData getPaiHangItemData()
        {
            paiHangItemData.RoleID = roleId;
            paiHangItemData.RoleName = roleName;
            paiHangItemData.Val1 = WingID;
            paiHangItemData.Val2 = WingStarNum;
            paiHangItemData.Val3 = Occupation;

            return paiHangItemData;
        }


        public int CompareTo(PlayerWingRankingData other)
        {
            if (this.WingID == other.WingID)
            {
                if (this.WingStarNum == other.WingStarNum)
                {
                    int nRet = String.Compare(this.WingAddTime, other.WingAddTime);
                    // 阶数和星数相同，则按刷新时间从小到大排序
                    return nRet < 0 ? -1 : nRet == 0 ? 0 : 1;
                }
                else
                {
                    return this.WingStarNum < other.WingStarNum ? 1 : -1;
                }
            }
            else
            {
                return this.WingID < other.WingID ? 1 : -1;
            }
        }
    }
}
