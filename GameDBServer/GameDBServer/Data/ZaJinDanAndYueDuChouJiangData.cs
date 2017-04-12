using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    [ProtoContract]
    public class YueDuChouJiangData
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [ProtoMember(1)]
        public int RoleID = 0;

        /// <summary>
        /// 角色名
        /// </summary>
        [ProtoMember(2)]
        public string RoleName = "";


        /// <summary>
        /// 得到物品id
        /// </summary>
        [ProtoMember(3)]
        public int GainGoodsId = 0;

        /// <summary>
        /// 得到物品数量[一个]
        /// </summary>
        [ProtoMember(4)]
        public int GainGoodsNum = 0;

        /// <summary>
        /// 得到金币
        /// </summary>
        [ProtoMember(5)]
        public int GainGold = 0;

        /// <summary>
        /// 得到银两
        /// </summary>
        [ProtoMember(6)]
        public int GainYinLiang = 0;

        /// <summary>
        /// 得到经验
        /// </summary>
        [ProtoMember(7)]
        public int GainExp = 0;

        /// <summary>
        /// 月度转盘操作时间
        /// </summary>
        [ProtoMember(8)]
        public string OperationTime = "";

    }
}

