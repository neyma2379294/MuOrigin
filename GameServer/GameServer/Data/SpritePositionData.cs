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
    public class SpritePositionData
    {
        [ProtoMember(1)]
        public int roleID = 0;

        [ProtoMember(2)]
        public int mapCode = 0;

        [ProtoMember(3)]
        public int toX = 0;

        [ProtoMember(4)]
        public int toY = 0;

        [ProtoMember(5)]
        public long currentPosTicks = 0;
    }
}
