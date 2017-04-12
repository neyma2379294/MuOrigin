using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 点将排名数据中的角色信息
    /// </summary>
    [ProtoContract]     
    public class DJRoleInfoData
    {
        /// <summary>
        /// 对方的角色ID
        /// </summary>
        [ProtoMember(1)]
        public int RoleID = 0;

        /// <summary>
        /// 对方的名称
        /// </summary>
        [ProtoMember(2)]
        public string RoleName = "";

        /// <summary>
        /// 对方的等级
        /// </summary>
        [ProtoMember(3)]
        public int Level = 0;

        /// <summary>
        /// 对方的职业
        /// </summary>
        [ProtoMember(4)]
        public int Occupation = 0;

        /// <summary>
        /// 对方的在线状态
        /// </summary>
        [ProtoMember(5)]
        public int OnlineState = 0;
    }
}
