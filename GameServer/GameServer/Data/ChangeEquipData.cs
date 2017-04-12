using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 更换装备的消息数据
    /// </summary>
    [ProtoContract]
    public class ChangeEquipData
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [ProtoMember(1)]
        public int RoleID = 0;

        /// <summary>
        /// 角色ID
        /// </summary>
        [ProtoMember(2)]
        public GoodsData EquipGoodsData = null;
    }
}
