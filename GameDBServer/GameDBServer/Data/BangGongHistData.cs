using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// Gang contribution data
    /// </summary>
    [ProtoContract]
    public class BangGongHistData
    {
        /// <summary>
        /// Area ID
        /// </summary>
        [ProtoMember(1)]
        public int ZoneID = 0;

        /// <summary>
        /// ID of the role
        /// </summary>
        [ProtoMember(2)]
        public int RoleID = 0;

        /// <summary>
        /// The name of the role
        /// </summary>
        [ProtoMember(3)]
        public string RoleName = "";

        /// <summary>
        /// The role of the profession
        /// </summary>
        [ProtoMember(4)]
        public int Occupation = 0;

        /// <summary>
        /// The level of the character
        /// </summary>
        [ProtoMember(5)]
        public int RoleLevel = 0;

        /// <summary>
        /// Gang post
        /// </summary>
        [ProtoMember(6)]
        public int BHZhiWu = 0;

        /// <summary>
        /// Gang title
        /// </summary>
        [ProtoMember(7)]
        public string BHChengHao = "";

        /// <summary>
        /// The number of warehouse props 1
        /// </summary>
        [ProtoMember(8)]
        public int Goods1Num = 0;

        /// <summary>
        /// The number of warehouse props 2
        /// </summary>
        [ProtoMember(9)]
        public int Goods2Num = 0;

        /// <summary>
        /// The number of warehouse props 3
        /// </summary>
        [ProtoMember(10)]
        public int Goods3Num = 0;

        /// <summary>
        /// The number of warehouse props 4
        /// </summary>
        [ProtoMember(11)]
        public int Goods4Num = 0;

        /// <summary>
        /// The number of warehouse props 5
        /// </summary>
        [ProtoMember(12)]
        public int Goods5Num = 0;

        /// <summary>
        /// The number of coins in the warehouse
        /// </summary>
        [ProtoMember(13)]
        public int TongQian = 0;

        /// <summary>
        /// The role of the get help tribute
        /// </summary>
        [ProtoMember(14)]
        public int BangGong = 0;
    }
}
