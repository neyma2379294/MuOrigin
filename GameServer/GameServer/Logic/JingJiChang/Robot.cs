using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Logic.JingJiChang.FSM;
using Server.Data;

namespace GameServer.Logic.JingJiChang
{
    public class Robot : Monster
    {
        private FinishStateMachine FSM = null;

        private RoleDataMini roleDataMini = null;

        public Dictionary<int, int> skillInfos = new Dictionary<int, int>();

        /// <summary>
        /// 性别
        /// </summary>
        private int _sex;

        private int _playerId;

        private int _lucky = 0;
        private int _fatalValue = 0;
        private int _doubleValue = 0;

        public int Lucky
        {
            get { return this._lucky; }
            set { this._lucky = value; }
        }

        public int FatalValue
        {
            get { return this._fatalValue; }
            set { this._fatalValue = value; }
        }

        public int DoubleValue
        {
            get { return this._doubleValue; }
            set { this._doubleValue = value; }
        }

        public int Sex
        {
            get { return this._sex; }
            set { this._sex = value; }
        }

        public int PlayerId
        {
            get { return this._playerId; }
            set { this._playerId = value; }
        }

        public Robot(GameClient player, RoleDataMini roleDataMini)
        {
            this.MonsterInfo = new MonsterStaticInfo();

            base.MonsterType = (int)MonsterTypes.JingJiChangRobot;
            this.roleDataMini = roleDataMini;
            FSM = new FinishStateMachine(player, this);
        }

        public RoleDataMini getRoleDataMini()
        {
            return this.roleDataMini;
        }

        public void onUpdate()
        {
            FSM.onUpdate();
        }

        public void startAttack()
        {
            FSM.switchState(AIState.ATTACK);
        }

        public void stopAttack()
        {
            FSM.switchState(AIState.RETURN);
        }

        #region 技能执行队列

        /// <summary>
        /// 执行分段攻击的技能执行队列
        /// </summary>
        public MagicsManyTimeDmageQueue MyMagicsManyTimeDmageQueue = new MagicsManyTimeDmageQueue();

        #endregion 技能执行队列
    }
}
