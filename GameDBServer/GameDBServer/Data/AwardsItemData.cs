using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// Task reward data
    /// </summary>
    [ProtoContract]
    public class AwardsItemData
    {
        /// <summary>
        /// Occupation logo
        /// </summary>
        [ProtoMember(1)]
        public int Occupation = 0;

        /// <summary>
        /// Item ID
        /// </summary>
        [ProtoMember(2)]        
        public int GoodsID = 0;

        /// <summary>
        /// number of the stuffs
        /// </summary>
        [ProtoMember(3)]        
        public int GoodsNum = 0;

        /// <summary>
        /// Whether to bind items
        /// </summary>
        [ProtoMember(4)]        
        public int Binding = 0;

        /// <summary>
        /// The level of the item
        /// </summary>
        [ProtoMember(5)]        
        public int Level = 0;

        /// <summary>
        /// The quality of the item
        /// </summary>
        [ProtoMember(6)]        
        public int Quality = 0;

        /// <summary>
        /// The deadline of the item
        /// </summary>
        [ProtoMember(7)]
        public string EndTime = "";

        /// <summary>
        /// Items are born
        /// </summary>
        [ProtoMember(8)]
        public int BornIndex = 0;

        /// <summary>
        /// Sex mark
        /// </summary>
        [ProtoMember(9)]
        public int RoleSex = 0;
    }
}
