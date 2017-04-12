using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Logic;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
    /// <summary>
    /// 玩家死亡事件
    /// </summary>
    public class PlayerDeadEventObject : EventObject
    {

        private Monster attacker;

        private GameClient player;

        public PlayerDeadEventObject(GameClient player, Monster attacker)
            : base((int)EventTypes.PlayerDead)
        {
            this.player = player;
            this.attacker = attacker;
        }

        public Monster getAttacker()
        {
            return attacker;
        }

        public GameClient getPlayer()
        {
            return player;
        }
    }
}
