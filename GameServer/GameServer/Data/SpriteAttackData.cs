using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 精灵攻击数据
    /// </summary>
    [ProtoContract]
    public class SpriteAttackData
    {
        [ProtoMember(1)]
        public int roleID = 0;

        [ProtoMember(2)]
        public int roleX = 0;

        [ProtoMember(3)]
        public int roleY = 0;

        [ProtoMember(4)]
        public int enemy = 0;

        [ProtoMember(5)]
        public int enemyX = 0;

        [ProtoMember(6)]
        public int enemyY = 0;

        [ProtoMember(7)]
        public int realEnemyX = 0;

        [ProtoMember(8)]
        public int realEnemyY = 0;

        [ProtoMember(9)]
        public int magicCode = 0;
    }
}
