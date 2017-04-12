using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// The gang manages the member data
    /// </summary>
    [ProtoContract]
    public class BangHuiMgrItemData
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
        /// Help in the job
        /// </summary>
        [ProtoMember(5)]
        public int BHZhiwu = 0;

        /// <summary>
        /// Help in the title
        /// </summary>
        [ProtoMember(6)]
        public string ChengHao = "";

        /// <summary>
        /// Gang announcement
        /// </summary>
        [ProtoMember(7)]
        public int BangGong = 0;

        /// <summary>
        /// The level of the character
        /// </summary>
        [ProtoMember(8)]
        public int Level = 0;
    }

    /// <summary>
    /// Gang detailed data
    /// </summary>
    [ProtoContract]
    public class BangHuiDetailData
    {
        /// <summary>
        /// Gang ID
        /// </summary>
        [ProtoMember(1)]
        public int BHID = 0;

        /// <summary>
        /// 帮派的名称
        /// </summary>
        [ProtoMember(2)]
        public string BHName = "";

        /// <summary>
        /// Area ID
        /// </summary>
        [ProtoMember(3)]
        public int ZoneID = 0;

        /// <summary>
        /// The owner of the ID
        /// </summary>
        [ProtoMember(4)]
        public int BZRoleID = 0;

        /// <summary>
        /// Name of the Lord
        /// </summary>
        [ProtoMember(5)]
        public string BZRoleName = "";

        /// <summary>
        /// The job of the Lord
        /// </summary>
        [ProtoMember(6)]
        public int BZOccupation = 0;

        /// <summary>
        /// The total number of members
        /// </summary>
        [ProtoMember(7)]
        public int TotalNum = 0;

        /// <summary>
        /// Help members of the overall level
        /// </summary>
        [ProtoMember(8)]
        public int TotalLevel = 0;

        /// <summary>
        /// Gang announcement
        /// </summary>
        [ProtoMember(9)]
        public string BHBulletin = "";

        /// <summary>
        /// Build time
        /// </summary>
        [ProtoMember(10)]
        public string BuildTime = "";

        /// <summary>
        /// Flag name
        /// </summary>
        [ProtoMember(11)]
        public string QiName = "";

        /// <summary>
        /// Help members of the overall level
        /// </summary>
        [ProtoMember(12)]
        public int QiLevel = 0;

        /// <summary>
        /// Manage the list of members
        /// </summary>
        [ProtoMember(13)]
        public List<BangHuiMgrItemData> MgrItemList = null;

        /// <summary>
        /// Whether to verify
        /// </summary>
        [ProtoMember(14)]
        public int IsVerify = 0;

        // MU added [3/7/2014 LiaoWei]
        /// <summary>
        /// Gang money
        /// </summary>
        [ProtoMember(15)]
        public int TotalMoney = 0;

        // MU added [3/7/2014 LiaoWei]
        /// <summary>
        /// Players today won the war effort
        /// </summary>
        [ProtoMember(16)]
        public int TodayZhanGongForGold = 0;

        // MU added [3/7/2014 LiaoWei]
        /// <summary>
        /// Players today won the war effort
        /// </summary>
        [ProtoMember(17)]
        public int TodayZhanGongForDiamond = 0;

        /// <summary>
        /// altar
        /// </summary>
        [ProtoMember(18)]
        public int JiTan = 0;

        /// <summary>
        /// ordnance
        /// </summary>
        [ProtoMember(19)]
        public int JunXie = 0;

        /// <summary>
        /// Halo
        /// </summary>
        [ProtoMember(20)]
        public int GuangHuan = 0;
    }
}
