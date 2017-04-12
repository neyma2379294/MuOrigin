using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 帮会贡献项数据
    /// </summary>
    [ProtoContract]
    public class BangGongHistData
    {
        /// <summary>
        /// 区ID
        /// </summary>
        [ProtoMember(1)]
        public int ZoneID = 0;

        /// <summary>
        /// 角色的ID
        /// </summary>
        [ProtoMember(2)]
        public int RoleID = 0;

        /// <summary>
        /// 角色的名称
        /// </summary>
        [ProtoMember(3)]
        public string RoleName = "";

        /// <summary>
        /// 角色的职业
        /// </summary>
        [ProtoMember(4)]
        public int Occupation = 0;

        /// <summary>
        /// 角色的级别
        /// </summary>
        [ProtoMember(5)]
        public int RoleLevel = 0;

        /// <summary>
        /// 帮会职务
        /// </summary>
        [ProtoMember(6)]
        public int BHZhiWu = 0;

        /// <summary>
        /// 帮会称号
        /// </summary>
        [ProtoMember(7)]
        public string BHChengHao = "";

        /// <summary>
        /// 仓库道具1的数量
        /// </summary>
        [ProtoMember(8)]
        public int Goods1Num = 0;

        /// <summary>
        /// 仓库道具2的数量
        /// </summary>
        [ProtoMember(9)]
        public int Goods2Num = 0;

        /// <summary>
        /// 仓库道具3的数量
        /// </summary>
        [ProtoMember(10)]
        public int Goods3Num = 0;

        /// <summary>
        /// 仓库道具4的数量
        /// </summary>
        [ProtoMember(11)]
        public int Goods4Num = 0;

        /// <summary>
        /// 仓库道具5的数量
        /// </summary>
        [ProtoMember(12)]
        public int Goods5Num = 0;

        /// <summary>
        /// 仓库道具5的数量
        /// </summary>
        [ProtoMember(13)]
        public int TongQian = 0;

        /// <summary>
        /// 角色的获取的帮贡
        /// </summary>
        [ProtoMember(14)]
        public int BangGong = 0;
    }
}
