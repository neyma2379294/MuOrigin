using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// Gang warehouse data
    /// </summary>
    [ProtoContract]
    public class BangHuiBagData
    {
        /// <summary>
        /// The number of warehouse props 1
        /// </summary>
        [ProtoMember(1)]
        public int Goods1Num = 0;

        /// <summary>
        /// The number of warehouse props 2
        /// </summary>
        [ProtoMember(2)]
        public int Goods2Num = 0;

        /// <summary>
        /// The number of warehouse props 3
        /// </summary>
        [ProtoMember(3)]
        public int Goods3Num = 0;

        /// <summary>
        /// The number of warehouse props 4
        /// </summary>
        [ProtoMember(4)]
        public int Goods4Num = 0;

        /// <summary>
        /// The number of warehouse props 5
        /// </summary>
        [ProtoMember(5)]
        public int Goods5Num = 0;

        /// <summary>
        /// The number of coins in the warehouse
        /// </summary>
        [ProtoMember(6)]
        public int TongQian = 0;

        /// <summary>
        /// To help tribute history list data (the last 20)
        /// </summary>
        [ProtoMember(7)]
        public List<BangGongHistData> BbangGongHistList = null;
    }
}
