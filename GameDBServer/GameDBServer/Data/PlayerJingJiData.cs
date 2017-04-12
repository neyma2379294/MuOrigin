using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using GameDBServer.DB;
using Server.Tools;
using GameDBServer.Logic;
using Server.Data;

namespace Server.Data
{
    /// <summary>
    /// 竞技场机器人数据
    /// </summary>
    [ProtoContract]
    public class PlayerJingJiData
    {
        public PlayerJingJiData()
        {
            this.rankingData = new PlayerJingJiRankingData(this);
        }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool isOnline = false;

        /// <summary>
        /// 玩家ID
        /// </summary>
        [ProtoMember(1)]
        [DBMapping(ColumnName = "roleId")]
        public int roleId;

        /// <summary>
        /// 玩家名称
        /// </summary>
        [ProtoMember(2)]
        [DBMapping(ColumnName = "roleName")]
        public string roleName;

        /// <summary>
        /// 玩家等级
        /// </summary>
        [ProtoMember(3)]
        [DBMapping(ColumnName = "level")]
        public int level;

        /// <summary>
        /// 玩家转生等级
        /// </summary>
        [ProtoMember(4)]
        [DBMapping(ColumnName = "changeLiveCount")]
        public int changeLiveCount;

        /// <summary>
        /// 玩家职业ID
        /// </summary>
        [ProtoMember(5)]
        [DBMapping(ColumnName = "occupationId")]
        public int occupationId;

        /// <summary>
        /// 连胜次数
        /// </summary>
        [ProtoMember(6)]
        [DBMapping(ColumnName = "winCount")]
        public int winCount = 0;

        /// <summary>
        /// 排名
        /// </summary>
        [ProtoMember(7)]
        [DBMapping(ColumnName = "ranking")]
        public int ranking = -1;

        /// <summary>
        /// 下次领取奖励时间戳
        /// </summary>
        [ProtoMember(8)]
        [DBMapping(ColumnName = "nextRewardTime")]
        public long nextRewardTime;

        /// <summary>
        /// 下次挑战时间戳
        /// </summary>
        [ProtoMember(9)]
        [DBMapping(ColumnName = "nextChallengeTime")]
        public long nextChallengeTime;

        /// <summary>
        /// 玩家基础属性
        /// </summary>
        [ProtoMember(10)]
        public double[] baseProps;

        /// <summary>
        /// 玩家基础属性（BLOB）
        /// </summary>
        [DBMapping(ColumnName = "baseProps")]
        public string stringBaseProps;

        /// <summary>
        /// 玩家扩展属性
        /// </summary>
        [ProtoMember(11)]
        public double[] extProps;

        /// <summary>
        /// 玩家属性扩展数据（BLOB）
        /// </summary>
        [DBMapping(ColumnName = "extProps")]
        public string stringExtProps;

        /// <summary>
        /// 装备数据
        /// </summary>
        [ProtoMember(12)]
        public List<PlayerJingJiEquipData> equipDatas;

        /// <summary>
        /// 装备数据（BLOB）
        /// </summary>
        [DBMapping(ColumnName = "equipDatas")]
        public string stringEquipDatas;

        /// <summary>
        /// 技能数据
        /// </summary>
        [ProtoMember(13)]
        public List<PlayerJingJiSkillData> skillDatas;

        /// <summary>
        /// 技能数据（BLOB）
        /// </summary>
        [DBMapping(ColumnName = "skillDatas")]
        public string stringSkillDatas;

        /// <summary>
        /// 战力
        /// </summary>
        [ProtoMember(14)]
        [DBMapping(ColumnName = "CombatForce")]
        public int combatForce = 0;

        /// <summary>
        /// 版本号
        /// </summary>
        [DBMapping(ColumnName = "version")]
        public int version;

        /// <summary>
        /// 性别
        /// </summary>
        [ProtoMember(15)]
        [DBMapping(ColumnName = "sex")]
        public int sex;

        /// <summary>
        /// 名称（不带区号）
        /// </summary>
        [ProtoMember(16)]
        [DBMapping(ColumnName = "name")]
        public string name;

        /// <summary>
        /// 区ID
        /// </summary>
        [ProtoMember(17)]
        [DBMapping(ColumnName = "zoneId")]
        public int zoneId;

        /// <summary>
        /// 将数据对象转换成BLOB
        /// </summary>
        public void convertString()
        {
            stringBaseProps = this.convertBasePropsToString(baseProps);
            stringExtProps = this.convertExtPropsToString(extProps);
            stringEquipDatas = this.convertEquipDatasToString(equipDatas);
            stringSkillDatas = this.convertSkillDatasToString(skillDatas);
        }

        /// <summary>
        /// 将BLOB转换成数据对象
        /// </summary>
        public void convertObject()
        {
            if (version == JingJiChangConstants.Current_Data_Version)
            {
                baseProps = this.convertStringToBaseProps(stringBaseProps);
                extProps = this.convertStringToExtProps(stringExtProps);
                equipDatas = this.convertStringToEquipDatas(stringEquipDatas);
                skillDatas = this.convertStringToSkillDatas(stringSkillDatas);
            }
            else
            {
                //如果数据库版本过低，要进行处理
            }
            
        }

        private string convertBasePropsToString(double[] baseProps)
        {
            StringBuilder _baseProps = new StringBuilder();

            for (int i = 0; i < baseProps.Length; i++)
            {
                if (i == (baseProps.Length - 1))
                {
                    _baseProps.Append(Convert.ToString(baseProps[i]));
                }
                else
                {
                    _baseProps.Append(Convert.ToString(baseProps[i]));
                    _baseProps.Append(',');
                }
            }

            return _baseProps.Length != 0 ? _baseProps.ToString() : "";
        }

        private string convertExtPropsToString(double[] extProps)
        {

            StringBuilder _extProps = new StringBuilder();

            for (int i = 0; i < extProps.Length; i++)
            {
                if (i == (extProps.Length - 1))
                {
                    _extProps.Append(Convert.ToString(extProps[i]));
                }
                else
                {
                    _extProps.Append(Convert.ToString(extProps[i]));
                    _extProps.Append(',');
                }
            }

            return _extProps.Length != 0 ? _extProps.ToString() : "";
        }

        private string convertEquipDatasToString(List<PlayerJingJiEquipData> equipDatas)
        {
            if (equipDatas == null)
            {
                return "";
            }

            StringBuilder _equipDatas = new StringBuilder();

            for (int i = 0; i < equipDatas.Count; i++)
            {
                if (i == (equipDatas.Count - 1))
                {
                    _equipDatas.Append(equipDatas[i].getStringValue());
                }
                else
                {
                    _equipDatas.Append(equipDatas[i].getStringValue());
                    _equipDatas.Append('|');
                }
            }

            return _equipDatas.Length != 0 ? _equipDatas.ToString() : "";
        }

        private string convertSkillDatasToString(List<PlayerJingJiSkillData> skillDatas)
        {
            StringBuilder _skillDatas = new StringBuilder();

            for (int i = 0; i < skillDatas.Count; i++)
            {
                if (i == (skillDatas.Count - 1))
                {
                    _skillDatas.Append(skillDatas[i].getStringValue());
                }
                else
                {
                    _skillDatas.Append(skillDatas[i].getStringValue());
                    _skillDatas.Append('|');
                }
            }

            return _skillDatas.Length != 0 ? _skillDatas.ToString() : "";
        }

        private double[] convertStringToBaseProps(string value)
        {
            string[] _value = value.Split(',');
            double[] baseProps = new double[_value.Length];

            for (int i = 0; i < _value.Length; i++)
            {
                baseProps[i] = Convert.ToDouble(_value[i]);
            }

            return baseProps;
        }

        private double[] convertStringToExtProps(string value)
        {
            string[] _value = value.Split(',');
            double[] extProps = new double[_value.Length];

            for (int i = 0; i < _value.Length; i++)
            {
                extProps[i] = Convert.ToDouble(_value[i]);
            }

            return extProps;
        }

        private List<PlayerJingJiEquipData> convertStringToEquipDatas(string value)
        {
            List<PlayerJingJiEquipData> equipDatas = new List<PlayerJingJiEquipData>();

            if (null == value || value.Equals(""))
                return equipDatas;
            
            string[] _value = value.Split('|');

            for (int i = 0; i < _value.Length; i++)
            {
                PlayerJingJiEquipData data = PlayerJingJiEquipData.createPlayerJingJiEquipData(_value[i]);

                if(null != data)
                    equipDatas.Add(data);
            }

            return equipDatas;
        }

        private List<PlayerJingJiSkillData> convertStringToSkillDatas(string value)
        {
            List<PlayerJingJiSkillData> skillDatas = new List<PlayerJingJiSkillData>();

            if (null == value || value.Equals(""))
                return skillDatas;

            string[] _value = value.Split('|');

            for (int i = 0; i < _value.Length; i++)
            {
                PlayerJingJiSkillData data = PlayerJingJiSkillData.createPlayerJingJiSkillData(_value[i]);

                if(null != data)
                    skillDatas.Add(data);
            }

            return skillDatas;
        } 

        /// <summary>
        /// mini数据
        /// </summary>
        private PlayerJingJiMiniData miniData = new PlayerJingJiMiniData();

        /// <summary>
        /// 排行榜数据
        /// </summary>
        private PlayerJingJiRankingData rankingData /*= new PlayerJingJiRankingData(this)*/;


        /// <summary>
        /// 获取玩家竞技场mini数据
        /// </summary>
        /// <returns></returns>
        public PlayerJingJiMiniData getPlayerJingJiMiniData()
        {
            miniData.roleId = this.roleId;
            miniData.roleName = this.roleName;
            miniData.ranking = this.ranking;
            miniData.occupationId = this.occupationId;
            miniData.combatForce = this.combatForce;

            return miniData;
        }

        /// <summary>
        /// 获取玩家竞技场排行榜数据
        /// </summary>
        /// <returns></returns>
        public PlayerJingJiRankingData getPlayerJingJiRankingData()
        {
//             rankingData.roleId = this.roleId;
//             rankingData.roleName = this.roleName;
               rankingData.ranking = this.ranking;
//             rankingData.combatForce = this.combatForce;

            return rankingData;
        }
    }
}
