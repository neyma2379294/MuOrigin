using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 精灵攻击结果
    /// </summary>
    [ProtoContract]
    public class SpriteAttackResultData
    {
        [ProtoMember(1)]
        public int enemy = 0;

        [ProtoMember(2)]
        public int burst = 0;

        [ProtoMember(3)]
        public int injure = 0;

        [ProtoMember(4)]
        public double enemyLife = 0;

        [ProtoMember(5)]
        public long newExperience = 0;

        [ProtoMember(6)]
        public long currentExperience = 0;

        [ProtoMember(7)]
        public int newLevel = 0;
    }
}
