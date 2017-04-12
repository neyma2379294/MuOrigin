using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 竞技场机器人装备数据
    /// </summary>
    [ProtoContract]
    public class PlayerJingJiEquipData
    {

        /// <summary>
        /// 装备ID
        /// </summary>
        [ProtoMember(1)]
        public int EquipId;

        /// <summary>
        /// 强化等级
        /// </summary>
        [ProtoMember(2)]
        public int Forge_level;

        /// <summary>
        /// 卓越属性
        /// </summary>
        [ProtoMember(3)]
        public int ExcellenceInfo;

        public string getStringValue()
        {
            return string.Format("{0},{1},{2}", this.EquipId, this.Forge_level, this.ExcellenceInfo);
        }

        public static PlayerJingJiEquipData createPlayerJingJiEquipData(string value)
        {
            if (value == null || value.Equals(""))
                return null ;

            PlayerJingJiEquipData data = new PlayerJingJiEquipData();

            string[] _value = value.Split(',');

            if (_value.Length != 3)
                return null;

            data.EquipId = Convert.ToInt32(_value[0]);
            data.Forge_level = Convert.ToInt32(_value[1]);
            data.ExcellenceInfo = Convert.ToInt32(_value[2]);

            return data;
        }
    }
}
