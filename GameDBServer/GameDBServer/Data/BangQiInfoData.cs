using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 领地数据
    /// </summary>
    [ProtoContract]
    public class BHLingDiOwnData
    {
        /// <summary>
        /// 领地ID
        /// </summary>
        [ProtoMember(1)]
        public int LingDiID = 0;

        /// <summary>
        /// 区ID
        /// </summary>
        [ProtoMember(2)]
        public int ZoneID = 0;

        /// <summary>
        /// 帮派的ID
        /// </summary>
        [ProtoMember(3)]
        public int BHID = 0;

        /// <summary>
        /// 帮派的名称
        /// </summary>
        [ProtoMember(4)]
        public string BHName = "";

        /// <summary>
        /// 帮会的名称
        /// </summary>
        [ProtoMember(5)]
        public string BangQiName = "";

        /// <summary>
        /// 帮旗的等级
        /// </summary>
        [ProtoMember(6)]
        public int BangQiLevel = 0;
    }

    /// <summary>
    /// 帮旗数据数据
    /// </summary>
    [ProtoContract]
    public class BangQiInfoData
    {
        /// <summary>
        /// 帮旗的名称
        /// </summary>
        [ProtoMember(1)]
        public string BangQiName = "";

        /// <summary>
        /// 帮旗的等级
        /// </summary>
        [ProtoMember(2)]
        public int BangQiLevel = 0;

        /// <summary>
        /// 领地势力分布
        /// </summary>
        [ProtoMember(3)]
        public Dictionary<int, BHLingDiOwnData> BHLingDiOwnDict = null;
    }
}
