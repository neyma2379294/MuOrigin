using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 玩家动作数据封装类
    /// </summary>
    [ProtoContract]
    public class SpriteActionData
    {
        [ProtoMember(1)]
        public int roleID = 0;

        [ProtoMember(2)]
        public int mapCode = 0;

        [ProtoMember(3)]
        public int direction = 0;

        [ProtoMember(4)]
        public int action = 0;

        [ProtoMember(5)]
        public int toX = 0;

        [ProtoMember(6)]
        public int toY = 0;

        [ProtoMember(7)]
        public int targetX = 0;

        [ProtoMember(8)]
        public int targetY = 0;

        [ProtoMember(9)]
        public int yAngle = 0;

        [ProtoMember(10)]
        public int moveToX = 0;

        [ProtoMember(11)]
        public int moveToY = 0;
    }
}
