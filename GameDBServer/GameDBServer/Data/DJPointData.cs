using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 点将积分数据
    /// </summary>
    [ProtoContract]       
    public class DJPointData
    {
        /// <summary>
        /// 数据库流水ID
        /// </summary>
        [ProtoMember(1)]
        public int DbID = 0;

        /// <summary>
        /// 角色的ID
        /// </summary>
        [ProtoMember(2)]
        public int RoleID = 0;

        /// <summary>
        /// 积分
        /// </summary>
        [ProtoMember(3)]
        public int DJPoint = 0;

        /// <summary>
        /// 总的比赛场次
        /// </summary>
        [ProtoMember(4)]
        public int Total = 0;

        /// <summary>
        /// 获胜的场次
        /// </summary>
        [ProtoMember(5)]
        public int Wincnt = 0;

        /// <summary>
        /// 昨日排名
        /// </summary>
        [ProtoMember(6)]
        public int Yestoday = 0;

        /// <summary>
        /// 上周排名
        /// </summary>
        [ProtoMember(7)]
        public int Lastweek = 0;

        /// <summary>
        /// 上个月排名
        /// </summary>
        [ProtoMember(8)]
        public int Lastmonth = 0;

        /// <summary>
        /// 日升降
        /// </summary>
        [ProtoMember(9)]
        public int Dayupdown = 0;

        /// <summary>
        /// 周升降
        /// </summary>
        [ProtoMember(10)]
        public int Weekupdown = 0;

        /// <summary>
        /// 月升降
        /// </summary>
        [ProtoMember(11)]
        public int Monthupdown = 0;

        /// <summary>
        /// 角色信息数据
        /// </summary>
        [ProtoMember(12)]
        public DJRoleInfoData djRoleInfoData = null;
    }
}
