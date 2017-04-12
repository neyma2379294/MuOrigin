using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 刷新图标状态数据
    /// </summary>
    [ProtoContract]
    public class LoadAlreadyData
    {
        [ProtoMember(1)]
        public int RoleID = 0;

        [ProtoMember(2)]
        public int MapCode = 0;

        [ProtoMember(3)]
        public long StartMoveTicks = 0;

        [ProtoMember(4)]
        public int CurrentX = 0;

        [ProtoMember(5)]
        public int CurrentY = 0;

        [ProtoMember(6)]
        public int CurrentDirection = 0;

        [ProtoMember(7)]
        public int Action = 0;

        [ProtoMember(8)]
        public int ToX = 0;

        [ProtoMember(9)]
        public int ToY = 0;

        [ProtoMember(10)]
        public double MoveCost = 1.0;

        [ProtoMember(11)]
        public int ExtAction = 0;

        [ProtoMember(12)]
        public string PathString = "";

        [ProtoMember(13)]
        public int CurrentPathIndex = 0;
    }
}
