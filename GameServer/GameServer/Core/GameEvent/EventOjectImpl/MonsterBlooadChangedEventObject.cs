using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
    /// <summary>
    /// 怪物血量变化事件对象
    /// </summary>
    public class MonsterBlooadChangedEventObject : EventObject
    {
        private Monster monster;

        public MonsterBlooadChangedEventObject(Monster monster)
            : base((int)EventTypes.MonsterBlooadChanged)
        {
            this.monster = monster;
        }

        public Monster getMonster()
        {
            return monster;
        }
    }
}
