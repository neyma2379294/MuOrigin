using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 领地占领数据(复杂)
    /// </summary>
    [ProtoContract]
    public class BangHuiLingDiInfoData
    {
        /// <summary>
        /// 领地ID
        /// </summary>
        [ProtoMember(1)]
        public int LingDiID = 0;

        /// <summary>
        /// 帮派的ID
        /// </summary>
        [ProtoMember(2)]
        public int BHID = 0;

        /// <summary>
        /// 区的ID
        /// </summary>
        [ProtoMember(3)]
        public int ZoneID = 0;

        /// <summary>
        /// 帮派的名称
        /// </summary>
        [ProtoMember(4)]
        public string BHName = "";

        /// <summary>
        /// 领地税率
        /// </summary>
        [ProtoMember(5)]
        public int LingDiTax = 0;

        /// <summary>
        /// 当日领取的日ID
        /// </summary>
        [ProtoMember(6)]
        public int TakeDayID = 0;

        /// <summary>
        /// 当日领取次数
        /// </summary>
        [ProtoMember(7)]
        public int TakeDayNum = 0;

        /// <summary>
        /// 昨日税收
        /// </summary>
        [ProtoMember(8)]
        public int YestodayTax = 0;

        /// <summary>
        /// 税收日ID
        /// </summary>
        [ProtoMember(9)]
        public int TaxDayID = 0;

        /// <summary>
        /// 今日税收日ID
        /// </summary>
        [ProtoMember(10)]
        public int TodayTax = 0;

        /// <summary>
        /// 总的税收
        /// </summary>
        [ProtoMember(11)]
        public int TotalTax = 0;

        /// <summary>
        /// 帮会战争请求字段
        /// </summary>
        [ProtoMember(12)]
        public String WarRequest = "";

        /// <summary>
        /// 领地每日奖励领取日
        /// </summary>
        [ProtoMember(13)]
        public int AwardFetchDay = 0;
    }
}
