using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 帮会列表项数据
    /// </summary>
    [ProtoContract]
    public class BangHuiItemData
    {
        /// <summary>
        /// 帮派的ID
        /// </summary>
        [ProtoMember(1)]
        public int BHID = 0;

        /// <summary>
        /// 帮派的名称
        /// </summary>
        [ProtoMember(2)]
        public string BHName = "";

        /// <summary>
        /// 区ID
        /// </summary>
        [ProtoMember(3)]
        public int ZoneID = 0;

        /// <summary>
        /// 帮主的ID
        /// </summary>
        [ProtoMember(4)]
        public int BZRoleID = 0;

        /// <summary>
        /// 帮主的名称
        /// </summary>
        [ProtoMember(5)]
        public string BZRoleName = "";

        /// <summary>
        /// 帮主的职业
        /// </summary>
        [ProtoMember(6)]
        public int BZOccupation = 0;

        /// <summary>
        /// 帮成员总的个数
        /// </summary>
        [ProtoMember(7)]
        public int TotalNum = 0;

        /// <summary>
        /// 帮成员总的级别
        /// </summary>
        [ProtoMember(8)]
        public int TotalLevel = 0;

        /// <summary>
        /// 帮成员总的级别
        /// </summary>
        [ProtoMember(9)]
        public int QiLevel = 0;
        
        /// <summary>
        /// 帮成员总的级别
        /// </summary>
        [ProtoMember(10)]
        public int IsVerfiy = 0;

        // MU 新增 [12/28/2013 LiaoWei]
        /// <summary>
        /// 帮会成员总战斗力
        /// </summary>
        [ProtoMember(11)]
        public int TotalCombatForce = 0;
    }
}
