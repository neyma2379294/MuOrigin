using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using GameDBServer.DB;

namespace Server.Data
{
    /// <summary>
    /// Stall log entries
    /// </summary>
    [ProtoContract]    
    public class BaiTanLogItemData
    {
        /// <summary>
        /// Primary key ID
        /// </summary>
        [DBMapping(ColumnName = "Id")]
        public int Id = 0;

        /// <summary>
        /// The role ID of the stall owner
        /// </summary>
        [ProtoMember(1)]
        [DBMapping(ColumnName = "rid")]
        public int rid = 0;

        /// <summary>
        /// The role ID of the buyer
        /// </summary>
        [ProtoMember(2)]
        [DBMapping(ColumnName = "otherroleid")]
        public int OtherRoleID = 0;

        /// <summary>
        /// The name of the buyer's role
        /// </summary>
        [ProtoMember(3)]
        [DBMapping(ColumnName = "otherrname")]
        public string OtherRName = "";

        /// <summary>
        /// Item ID
        /// </summary>
        [ProtoMember(4)]
        [DBMapping(ColumnName = "goodsid")]
        public int GoodsID = 0;

        /// <summary>
        /// number of the stuffs
        /// </summary>
        [ProtoMember(5)]
        [DBMapping(ColumnName = "goodsnum")]
        public int GoodsNum = 0;

        /// <summary>
        /// Item enhancement level
        /// </summary>
        [ProtoMember(6)]
        [DBMapping(ColumnName = "forgelevel")]
        public int ForgeLevel = 0;

        /// <summary>
        /// Item ingot price
        /// </summary>
        [ProtoMember(7)]
        [DBMapping(ColumnName = "totalprice")]
        public int TotalPrice = 0;

        /// <summary>
        /// Remaining ingot
        /// </summary>
        [ProtoMember(8)]
        [DBMapping(ColumnName = "leftyuanbao")]
        public int LeftYuanBao = 0;

        /// <summary>
        /// Event time
        /// </summary>
        [ProtoMember(9)]
        [DBMapping(ColumnName = "buytime")]
        public string BuyTime = "";

        /// <summary>
        /// Item Unbundled Gold Price
        /// </summary>
        [ProtoMember(10)]
        [DBMapping(ColumnName = "yinliang")]
        public int YinLiang = 0;

        /// <summary>
        /// The remaining gold coins
        /// </summary>
        [ProtoMember(11)]
        [DBMapping(ColumnName = "left_yinliang")]
        public int LeftYinLiang = 0;

        /// <summary>
        /// The remaining gold coins
        /// </summary>
        [ProtoMember(12)]
        [DBMapping(ColumnName = "tax")]
        public int Tax = 0;

        /// <summary>
        /// Excellent attribute
        /// </summary>
        [ProtoMember(13)]
        [DBMapping(ColumnName = "excellenceinfo")]
        public int Excellenceinfo = 0;
    }
}
