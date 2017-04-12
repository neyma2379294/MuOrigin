using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 奇珍阁历史记录项数据
    /// </summary>
    [ProtoContract]
    public class ShengXiaoGuessHistory
    {
        /// <summary>
        /// 竞猜的角色ID
        /// </summary>
        [ProtoMember(1)]
        public int RoleID = 0;

        /// <summary>
        /// 竞猜的角色名
        /// </summary>
        [ProtoMember(2)]
        public string RoleName = "";

        /// <summary>
        /// 竞猜关键字
        /// </summary>
        [ProtoMember(3)]
        public int GuessKey = 0;

        /// <summary>
        /// 注码
        /// </summary>
        [ProtoMember(4)]
        public int Mortgage = 0;

        /// <summary>
        /// 结果关键字
        /// </summary>
        [ProtoMember(5)]
        public int ResultKey = 0;

        /// <summary>
        /// 赢取数量
        /// </summary>
        [ProtoMember(6)]
        public int GainNum = 0;

        /// <summary>
        /// 剩余注码
        /// </summary>
        [ProtoMember(7)]
        public int LeftMortgage = 0;

        /// <summary>
        /// 竞猜时间
        /// </summary>
        [ProtoMember(8)]
        public string GuessTime = "";
    }
}
