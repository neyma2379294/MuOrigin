using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 玩家竞技场技能数据
    /// </summary>
    [ProtoContract]
    public class PlayerJingJiSkillData
    {
        /// <summary>
        /// 技能类型ID
        /// </summary>
        [ProtoMember(1)]
        public int skillID;

        /// <summary>
        /// 技能类型级别
        /// </summary>
        [ProtoMember(2)]
        public int skillLevel;

        public string getStringValue()
        {
            return string.Format("{0},{1}", this.skillID, this.skillLevel);
        }

        public static PlayerJingJiSkillData createPlayerJingJiSkillData(string value)
        {
            if (value == null || value.Equals(""))
                return null;

            string[] _value = value.Split(',');

            if (_value.Length != 2)
                return null;

            PlayerJingJiSkillData data = new PlayerJingJiSkillData();

            data.skillID = Convert.ToInt32(_value[0]);
            data.skillLevel = Convert.ToInt32(_value[1]);

            return data;
        }
    }
}
