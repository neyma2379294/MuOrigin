using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    [ProtoContract]
    public class HeFuPKKingData
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [ProtoMember(1)]
        public int RoleID = 0;

        /// <summary>
        /// 角色名称
        /// </summary>
        [ProtoMember(2)]
        public string RoleName = "";

        /// <summary>
        /// 区ID
        /// </summary>
        [ProtoMember(3)]
        public int ZoneID = 0;

        /// <summary>
        /// 状态
        /// </summary>
        [ProtoMember(4)]
        public int State = 0;
    }
}
