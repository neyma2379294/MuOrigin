using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 帮会成员数据
    /// </summary>
    [ProtoContract]
    public class BangHuiMemberData
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
        /// 帮中职务
        /// </summary>
        [ProtoMember(5)]
        public int BHZhiwu = 0;

        /// <summary>
        /// 帮中称号
        /// </summary>
        [ProtoMember(6)]
        public string ChengHao = "";

        /// <summary>
        /// 帮会贡献
        /// </summary>
        [ProtoMember(7)]
        public int BangGong = 0;

        /// <summary>
        /// 角色的级别
        /// </summary>
        [ProtoMember(8)]
        public int Level = 0;

        /// <summary>
        /// 经脉的进度
        /// </summary>
        [ProtoMember(9)]
        public int XueWeiNum = 0;

        /// <summary>
        /// 经脉的进度
        /// </summary>
        [ProtoMember(10)]
        public int SkillLearnedNum = 0;

        /// <summary>
        /// 是否在线
        /// </summary>
        [ProtoMember(11)]
        public int OnlineState = 0;

        // MU 新增 [1/3/2014 LiaoWei]
        /// <summary>
        /// 帮会成员战斗力
        /// </summary>
        [ProtoMember(12)]
        public int BangHuiMemberCombatForce = 0;

        // MU 新增 [1/13/2014 LiaoWei]
        /// <summary>
        /// 帮会成员转生等级
        /// </summary>
        [ProtoMember(13)]
        public int BangHuiMemberChangeLifeLev = 0;
    }
}
