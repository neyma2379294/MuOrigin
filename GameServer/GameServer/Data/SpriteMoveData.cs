using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 精灵移动数据封装类
    /// </summary>
    [ProtoContract]
    public class SpriteMoveData
    {
        [ProtoMember(1)]
        public int roleID = 0;

        [ProtoMember(2)]
        public int mapCode = 0;

        [ProtoMember(3)]
        public int action = 0;

        [ProtoMember(4)]
        public int toX = 0;

        [ProtoMember(5)]
        public int toY = 0;

        [ProtoMember(6)]
        public int extAction = 0;

        [ProtoMember(7)]
        public int fromX = 0;

        [ProtoMember(8)]
        public int fromY = 0;

        [ProtoMember(9)]
        public long startMoveTicks = 0;

        [ProtoMember(10)]
        public string pathString = "";

    }
}
