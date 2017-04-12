using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 帮会仓库数据
    /// </summary>
    [ProtoContract]
    public class BangHuiBagData
    {
        /// <summary>
        /// 仓库道具1的数量
        /// </summary>
        [ProtoMember(1)]
        public int Goods1Num = 0;

        /// <summary>
        /// 仓库道具2的数量
        /// </summary>
        [ProtoMember(2)]
        public int Goods2Num = 0;

        /// <summary>
        /// 仓库道具3的数量
        /// </summary>
        [ProtoMember(3)]
        public int Goods3Num = 0;

        /// <summary>
        /// 仓库道具4的数量
        /// </summary>
        [ProtoMember(4)]
        public int Goods4Num = 0;

        /// <summary>
        /// 仓库道具5的数量
        /// </summary>
        [ProtoMember(5)]
        public int Goods5Num = 0;

        /// <summary>
        /// 仓库铜钱的数量
        /// </summary>
        [ProtoMember(6)]
        public int TongQian = 0;

        /// <summary>
        /// 帮贡历史列表数据(最近的20个)
        /// </summary>
        [ProtoMember(7)]
        public List<BangGongHistData> BbangGongHistList = null;
    }
}
