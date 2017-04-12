using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 奇珍阁历史记录项数据
    /// </summary>
    [ProtoContract]
    public class QizhenGeBuItemData
    {
        /// <summary>
        /// 购买者的角色ID
        /// </summary>
        [ProtoMember(1)]
        public int RoleID = 0;

        /// <summary>
        /// 购买者的角色名
        /// </summary>
        [ProtoMember(2)]
        public string RoleName = "";

        /// <summary>
        /// 购买的物品ID
        /// </summary>
        [ProtoMember(3)]
        public int GoodsID = 0;

        /// <summary>
        /// 购买的物品数量
        /// </summary>
        [ProtoMember(4)]
        public int GoodsNum = 0;
    }
}
