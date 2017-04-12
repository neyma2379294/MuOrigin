using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using GameDBServer.DB;

namespace Server.Data
{
    /// <summary>
    /// 排行榜数据封装类
    /// </summary>
    [ProtoContract]
    public class PlayerJingJiRankingData : IComparable<PlayerJingJiRankingData>
    {

        public PlayerJingJiData jingjiData;

        public PlayerJingJiRankingData(PlayerJingJiData jingjiData)
        {
            this.jingjiData = jingjiData;
        }

        /// <summary>
        /// 玩家ID
        /// </summary>
        [ProtoMember(1)]
        public int roleId 
        {
            get { return jingjiData.roleId; }
        }

        /// <summary>
        /// 玩家名称
        /// </summary>
        [ProtoMember(2)]
        public string roleName
        {
            get { return jingjiData.roleName; }
        }

        /// <summary>
        /// 玩家战力
        /// </summary>
        [ProtoMember(3)]
        public int combatForce
        {
            get { return jingjiData.combatForce; }
        }

        /// <summary>
        /// 玩家排名
        /// </summary>
        [ProtoMember(4)]
        public int ranking
        {
            get { return jingjiData.ranking; }
            set { jingjiData.ranking = value; }
        }

        private PaiHangItemData paiHangItemData = new PaiHangItemData();

        public PaiHangItemData getPaiHangItemData()
        {
            paiHangItemData.RoleID = roleId;
            paiHangItemData.RoleName = roleName;
            paiHangItemData.Val1 = ranking;
            paiHangItemData.Val2 = combatForce;

            return paiHangItemData;
        }


        public int CompareTo(PlayerJingJiRankingData other)
        {
            return this.ranking < other.ranking ? -1 : this.ranking == other.ranking ? 0 : 1; 
        }
    }
}
