using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Interface;
using Server.Data;
using GameServer.Logic.JingJiChang;

/*
 属性改造 [8/16/2013 LiaoWei]
 1.去掉重量相关
 
 */
namespace GameServer.Logic
{
    /// <summary>
    /// 角色的相关属性和攻击伤害计算算法
    /// </summary>
    public class RoleAlgorithm
    {
        #region 基础属性值公式

        // 属性改造 注释掉重量 [8/15/2013 LiaoWei]
        //重量
        /*public static double GetWeight(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.Weight] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.Weight);
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.Weight, origVal) - origVal);
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.Weight);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.Weight, val);
        }

        //重量
        public static double GetWeight(Monster monster)
        {
            return 0.0;
        }*/

        // 属性改造 新增属性 begin [8/15/2013 LiaoWei]
        
        /// <summary>
        /// 力量
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetStrength(GameClient client, bool bAddBuff = true)
        {
            double dValue = 0.0;

            dValue = (double)client.ClientData.PropStrength + client.RoleBuffer.GetBaseProp((int)UnitPropIndexes.Strength) + client.ClientData.PictureJudgeProp.FirstPropsValue[(int)UnitPropIndexes.Strength] +
                        client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[(int)UnitPropIndexes.Strength];

            if (bAddBuff)
            {
                dValue += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPStrength);
            }
            
            return dValue;
        }

        /// <summary>
        /// 智力
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetIntelligence(GameClient client, bool bAddBuff = true)
        {
            double dValue = 0.0;

            dValue = (double)client.ClientData.PropIntelligence + client.RoleBuffer.GetBaseProp((int)UnitPropIndexes.Intelligence) + client.ClientData.PictureJudgeProp.FirstPropsValue[(int)UnitPropIndexes.Intelligence] +
                        client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[(int)UnitPropIndexes.Intelligence];

            if (bAddBuff)
            {
                dValue += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPIntelligsence);
            }

            return dValue;
        }

        /// <summary>
        /// 敏捷
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetDexterity(GameClient client, bool bAddBuff = true)
        {
            double dValue = 0.0;

            dValue = (double)client.ClientData.PropDexterity + client.RoleBuffer.GetBaseProp((int)UnitPropIndexes.Dexterity) + client.ClientData.PictureJudgeProp.FirstPropsValue[(int)UnitPropIndexes.Dexterity] +
                        client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[(int)UnitPropIndexes.Dexterity];

            if (bAddBuff)
            {
                dValue += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPDexterity);
            }

            return dValue;
        }

        /// <summary>
        /// 体力
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetConstitution(GameClient client, bool bAddBuff = true)
        {
            double dValue = 0.0;

            dValue = (double)client.ClientData.PropConstitution + client.RoleBuffer.GetBaseProp((int)UnitPropIndexes.Constitution) + client.ClientData.PictureJudgeProp.FirstPropsValue[(int)UnitPropIndexes.Constitution] +
                        client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[(int)UnitPropIndexes.Constitution];

            if (bAddBuff)
            {
                dValue += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPConstitution);
            }

            return dValue;
        }

        /// <summary>
        /// 魔法技能增幅
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMagicSkillIncrease(GameClient client)
        {
            double dValue = 0.0;
            
            // 区分职业 -- 只有法师(OccupationID = 1)有魔法技能增幅
            int nOcc = Global.CalcOriginalOccupationID(client);
            
            if (nOcc == 1)
            {
                RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];

                dValue = GetStrength(client) / 10000.0; //根据力量计算

                dValue += roleBasePropItem.MagicSkillIncreasePercent;
            }
            
            return dValue;
        }

        /// <summary>
        /// 物理技能增幅
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetPhySkillIncrease(GameClient client)
        {
            double dValue = 0.0;
            
            // 区分职业 -- 战士和弓箭手有物理技能增幅
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];

            if (nOcc == 0 || nOcc == 2)
                dValue = GetIntelligence(client) / 10000.0; // 根据智力计算

            dValue += roleBasePropItem.PhySkillIncreasePercent;

            return dValue;
        }

        /// <summary>
        /// 攻击速度-客户端显示用
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetAttackSpeed(GameClient client)
        {
            // 1攻击速度基础值读配置文件 2攻击速度将被一级属性(敏捷)、BUFF、武器影响
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];

            double dValue = 0.0;

            double AddValue = 0.0;
            AddValue = (int)DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPATTACKSPEED);

            dValue = roleBasePropItem.AttackSpeed;

            //dValue = GetDexterity(client) / 10.0;

            dValue = Data.MaxAttackSlotTick * (1.0 + dValue / 100.0) * (1.0 + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.AttackSpeed] / 100.0) *
                            (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.AttackSpeed) / 100.0);
            
            return dValue;
        }

        /// <summary>
        /// 攻击速度-给服务器运算用
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetAttackSpeedServer(GameClient client)
        {
            // 1攻击速度基础值读配置文件 2攻击速度将被一级属性(敏捷)、BUFF、武器影响

            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];

            double dValue = 0.0;

            dValue = roleBasePropItem.AttackSpeed;

            dValue += client.RoleBuffer.GetExtProp((int)ExtPropIndexes.AttackSpeed) + (int)DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPATTACKSPEED);

            //dValue = GetDexterity(client) / 10.0;

            dValue = Data.MaxAttackSlotTick * (1.0 - dValue / 100.0) * (1.0 - client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.AttackSpeed] / 100.0) *
                            (1.0 - client.RoleBuffer.GetExtProp((int)ExtPropIndexes.AttackSpeed) / 100.0);

            //dValue -= client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP5];    // 卓越属性影响攻击速度

            return dValue;
        }

        /// <summary>
        /// 卓越一击
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetFatalAttack(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.FatalAttack] * 100.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.FatalAttack) * 100 + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.FatalAttack] * 100 +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.FatalAttack] * 100 + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.FatalAttack] * 100;
            double origVal = val;

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.FatalAttack);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.FatalAttack, val);

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP0] * 100; // 卓越属性影响卓越一击

            val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPFATALATTACK) * 100;
            
            return val; 
        }

        /// <summary>
        /// 怪的卓越一击概率
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetFatalAttack(Monster monster)
        {
            double val = monster.MonsterInfo.MonsterFatalAttack*100;

            return val;
        }

        /// <summary>
        /// 双倍一击
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetDoubleAttack(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.DoubleAttack] * 100.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.DoubleAttack) * 100 + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.DoubleAttack] * 100 +
                                client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.DoubleAttack] * 100 + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.DoubleAttack] * 100;
            double origVal = val;

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DoubleAttack);

            val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPDOUBLEATTACK) * 100.0;

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.DoubleAttack, val);

            if (val > 0)
            {
                val = 0;
            }

            return val;
        }

        /// <summary>
        /// 怪的双倍一击概率
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetDoubleAttack(Monster monster)
        {
            double val = monster.MonsterInfo.MonsterDoubleAttack * 100;

            return val;
        }

        /// <summary>
        /// 移动速度
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMoveSpeed(GameClient client)
        {
            double val = 1.0; // 移动速度的base值
            // 填的就是百分比，不应该除以100
            val = val * (1.0 + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MoveSpeed]/* / 100.0*/) * (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MoveSpeed)/* / 100.0*/);
            
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MoveSpeed);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MoveSpeed, val);
            if (client.RoleBuffer.GetExtProp((int)ExtPropIndexes.StateDingSheng) > 0.1)
            {
                val = 0.0;
            }

            if (val < 0.0)
            {
                val = 0.0;
            }

            return val;
        }

        /// <summary>
        /// 伤害反弹(百分比)
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetDamageThornPercent(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.DamageThornPercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.DamageThornPercent) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.DamageThornPercent] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.DamageThornPercent] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.DamageThornPercent];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.DamageThornPercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DamageThornPercent);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.DamageThornPercent, val);

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP11];  // 卓越属性影响伤害反弹百分比

            return val;
        }

        /// <summary>
        /// 怪的伤害反弹(百分比)
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetDamageThornPercent(Monster monster)
        {
            double val = monster.MonsterInfo.MonsterDamageThornPercent;

            return val;
        }

        /// <summary>
        /// 伤害反弹(固定值)
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetDamageThorn(GameClient client)
        {
            double val = 0.0;
            val = val * (1.0 + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.DamageThorn] / 100.0) * (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.DamageThorn) / 100.0) + (client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.DamageThorn] / 100.0) +
                            (client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.DamageThorn] / 100.0) + (client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.DamageThorn] / 100.0);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DamageThorn);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.DamageThorn, val);

            return val;
        }

        /// <summary>
        /// 怪的伤害反弹(固定值)
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetDamageThorn(Monster monster)
        {
            double val = monster.MonsterInfo.MonsterDamageThorn;

            return val;
        }

        // 属性改造 新增属性 end [8/15/2013 LiaoWei]

        //耐久
        public static double GetStrong(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.Strong] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.Strong) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.Strong] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.Strong] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.Strong];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.Strong, origVal) - origVal);
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.Strong);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.Strong, val);
        }

        /// <summary>
        /// 耐久
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetStrong(Monster monster)
        {
            return 0.0;
        }

        /// <summary>
        /// 最小物理防御力
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMinADefenseV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.MinDefenseV;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MinDefense] + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.MinDefense] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.MinDefense] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.MinDefense];

            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MinDefense, origVal) - origVal);

            //防御符咒
            val *= (1.0 + DBRoleBufferManager.ProcessAddTempDefense(client));
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MinDefense);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MinDefense, val);

            if (nOcc == 0)
            {
                val += GetDexterity(client) * 0.88 * 0.6;
            }
            else if (nOcc == 1)
            {
                val += GetDexterity(client) * 0.64 * 0.6;
            }
            else if (nOcc == 2)
            {
                val += GetDexterity(client) * 0.76 * 0.6;
            }

            val += GetAddDefenseV(client);

            // 防御提升
            val *= (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.IncreasePhyDefense));

            val += client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MinDefense);

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP9] + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP14]; // 卓越属性影响防御力

            val += DBRoleBufferManager.ProcessTimeAddPkKingAttackProp(client, ExtPropIndexes.MinDefense);

            // 红名惩罚 [4/21/2014 LiaoWei]
            val = val - val * client.RedNameDebuffInfo[1];

            return Global.GMax(0, val);
        }

        /// <summary>
        /// 怪
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetMinADefenseV(Monster monster)
        {
            // 防御提升
            double val = monster.MonsterInfo.Defense;

            val *= (1.0 + monster.TempPropsBuffer.GetExtProp((int)ExtPropIndexes.IncreasePhyDefense));

            return val;
        }

        /// <summary>
        /// 最大物理防御力
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMaxADefenseV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.MaxDefenseV;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MaxDefense] + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.MaxDefense] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.MaxDefense] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.MaxDefense];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxDefense, origVal) - origVal);

            //防御符咒
            val *= (1.0 + DBRoleBufferManager.ProcessAddTempDefense(client));

            //持续时间加属性
            val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.TimeAddDefense);
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxDefense);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxDefense, val);

            if (nOcc == 0)
            {
                val += GetDexterity(client) * 0.88;
            }
            else if (nOcc == 1)
            {
                val += GetDexterity(client) * 0.64;
            }
            else if (nOcc == 2)
            {
                val += GetDexterity(client) * 0.76;
            }

            val += GetAddDefenseV(client);

            // 防御提升
            val *= (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.IncreasePhyDefense));

            val += client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MaxDefense);

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP9] + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP14]; // 卓越属性影响防御力

            val += DBRoleBufferManager.ProcessTimeAddPkKingAttackProp(client, ExtPropIndexes.MaxDefense);

            // 红名惩罚 [4/21/2014 LiaoWei]
            val = val - val * client.RedNameDebuffInfo[1];

            return Global.GMax(0, val);
        }

        /// <summary>
        /// 怪
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetMaxADefenseV(Monster monster)
        {
            double val = monster.MonsterInfo.Defense;

            // 防御提升
            val *= (1.0 + monster.TempPropsBuffer.GetExtProp((int)ExtPropIndexes.IncreasePhyDefense));
            return val;
        }

        /// <summary>
        /// 最小魔法防御值
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMinMDefenseV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.MinMDefenseV;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MinMDefense] + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.MinMDefense] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.MinMDefense] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.MinMDefense];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MinMDefense, origVal) - origVal);

            //防御符咒
            val *= (1.0 + DBRoleBufferManager.ProcessAddTempDefense(client));
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MinMDefense);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MinMDefense, val);

            if (nOcc == 0)
            {
                val += GetDexterity(client) * 0.6 * 0.6;
            }
            else if (nOcc == 1)
            {
                val += GetDexterity(client) * 0.84 * 0.6;
            }
            else if (nOcc == 2)
            {
                val += GetDexterity(client) * 0.72 * 0.6;
            }

            val += GetAddDefenseV(client);

            // 防御提升
            val *= (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.IncreaseMagDefense));

            val += client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MinMDefense);

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP9] + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP14]; // 卓越属性影响防御力

            val += DBRoleBufferManager.ProcessTimeAddPkKingAttackProp(client, ExtPropIndexes.MinMDefense);

            // 红名惩罚 [4/21/2014 LiaoWei]
            val = val - val * client.RedNameDebuffInfo[1];

            return Global.GMax(0, val);
        }

        /// <summary>
        /// 怪
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetMinMDefenseV(Monster monster)
        {
            double val = monster.MonsterInfo.MDefense;

            // 防御提升
            val *= (1.0 + monster.TempPropsBuffer.GetExtProp((int)ExtPropIndexes.IncreaseMagDefense));

            return val;
        }

        /// <summary>
        /// 最大魔法防御值
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMaxMDefenseV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.MaxMDefenseV;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MaxMDefense] + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.MaxMDefense] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.MaxMDefense] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.MaxMDefense];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxMDefense, origVal) - origVal);

            //防御符咒
            val *= (1.0 + DBRoleBufferManager.ProcessAddTempDefense(client));

            //持续时间加属性
            val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.TimeAddMDefense);
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxMDefense);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxMDefense, val);

            if (nOcc == 0)
            {
                val += GetDexterity(client) * 0.6;
            }
            else if (nOcc == 1)
            {
                val += GetDexterity(client) * 0.84;
            }
            else if (nOcc == 2)
            {
                val += GetDexterity(client) * 0.72;
            }

            val += GetAddDefenseV(client);

            // 防御提升
            val *= (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.IncreaseMagDefense));

            val += client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MaxMDefense);

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP9] + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP14]; // 卓越属性影响防御力

            val += DBRoleBufferManager.ProcessTimeAddPkKingAttackProp(client, ExtPropIndexes.MaxMDefense);

            // 红名惩罚 [4/21/2014 LiaoWei]
            val = val - val * client.RedNameDebuffInfo[1];

            return Global.GMax(0, val);
        }

        /// <summary>
        /// 怪
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetMaxMDefenseV(Monster monster)
        {
            double val = monster.MonsterInfo.MDefense;

            // 防御提升
            val *= (1.0 + monster.TempPropsBuffer.GetExtProp((int)ExtPropIndexes.IncreaseMagDefense));

            return val;
        }

        /// <summary>
        /// 物理攻击力最小值
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMinAttackV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.MinAttackV;

            if (nOcc == 0)
            {
                val += GetStrength(client) * 0.76 * 0.6;
            }
            else if (nOcc == 2)
            {
                val += GetStrength(client) * 0.8 * 0.6;
            }

            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MinAttack] + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.MinAttack] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.MinAttack] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.MinAttack];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MinAttack, origVal) - origVal);

            //buffer中的放大基础属性
            val *= (1.0 + DBRoleBufferManager.ProcessAddTempAttack(client));
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MinAttack);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MinAttack, val);

            val += GetAddAttackV(client);

            // 物理攻击提升
            val *= (1.0 + DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IncreasePhyAttack));
            
            val *= (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.IncreasePhyAttack));

            val += client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MinAttack);

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP1];   // 卓越属性影响最小物理攻击力

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP2];

            val += DBRoleBufferManager.ProcessTimeAddPkKingAttackProp(client, ExtPropIndexes.MinAttack);

            val *= (1 + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP4]);

            val *= (1 + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP3]);

            // 红名惩罚 [4/21/2014 LiaoWei]
            val = val - val * client.RedNameDebuffInfo[0];

            return Global.GMax(0, val);
        }

        /// <summary>
        /// 怪
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetMinAttackV(Monster monster)
        {
            double attackVal = monster.MonsterInfo.MinAttack;

            // 物理攻击提升
            attackVal *= (1.0 + monster.TempPropsBuffer.GetExtProp((int)ExtPropIndexes.IncreasePhyAttack));

            return attackVal;
        }

        /// <summary>
        /// 物理攻击力最大值
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMaxAttackV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.MaxAttackV;

            if (nOcc == 0)
            {
                val += GetStrength(client) * 0.76;
            }
            else if (nOcc == 2)
            {
                val += GetStrength(client) * 0.8;
            }

            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MaxAttack] + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.MaxAttack] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.MaxAttack] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.MaxAttack];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxAttack, origVal) - origVal);

            //buffer中的放大基础属性
            val *= (1.0 + DBRoleBufferManager.ProcessAddTempAttack(client));

            //持续时间加属性
            val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.TimeAddAttack);
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxAttack);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxAttack, val);
            val += client.RoleOnceBuffer.GetExtProp((int)ExtPropIndexes.MaxAttack);
            val += DBRoleBufferManager.ProcessTimeAddPkKingAttackProp(client, ExtPropIndexes.MaxAttack);
            val += DBRoleBufferManager.ProcessTimeAddJunQiProp(client, ExtPropIndexes.MaxAttack);

            val += GetAddAttackV(client);

            // 物理攻击提升
            val *= (1.0 + DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IncreasePhyAttack));

            val *= (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.IncreasePhyAttack));

            val += client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MaxAttack);

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP1];   // 卓越属性影响最大物理攻击力

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP2];

            val *= (1 + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP4]);

            val *= (1 + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP3]);

            // 红名惩罚 [4/21/2014 LiaoWei]
            val = val - val * client.RedNameDebuffInfo[0];

            return Global.GMax(0, val);
        }

        /// <summary>
        /// 怪
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetMaxAttackV(Monster monster)
        {
            int attackVal = monster.MonsterInfo.MaxAttack;
            return attackVal;
        }

        /// <summary>
        /// 魔法攻击力最小值
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMinMagicAttackV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.MinMAttackV;

            if (nOcc == 1)
            {
                val += GetIntelligence(client) * 0.88 * 0.6;
            }
            
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MinMAttack] + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.MinMAttack] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.MinMAttack] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.MinMAttack];
            double origVal = val;

            val += client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MinMAttack);

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MinMAttack, origVal) - origVal);

            //狂攻符咒
            val *= (1.0 + DBRoleBufferManager.ProcessAddTempAttack(client));
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MinMAttack);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MinMAttack, val);

            val += GetAddAttackV(client);

            // 魔法攻击提升
            val *= (1.0 + DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IncreaseMagAttack));

            val *= (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.IncreaseMagAttack));

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP2];   // 卓越属性影响最小魔法攻击力

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP1];

            val += DBRoleBufferManager.ProcessTimeAddPkKingAttackProp(client, ExtPropIndexes.MinMAttack);

            val *= (1 + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP4]);

            val *= (1 + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP3]);

            // 红名惩罚 [4/21/2014 LiaoWei]
            val = val - val * client.RedNameDebuffInfo[0];
            
            return Global.GMax(0, val);
        }

        /// <summary>
        /// 怪
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetMinMagicAttackV(Monster monster)
        {
            double attackVal = monster.MonsterInfo.MinAttack;

            // 魔法攻击提升
            attackVal *= (1.0 + monster.TempPropsBuffer.GetExtProp((int)ExtPropIndexes.IncreaseMagAttack));

            return attackVal;
        }

        /// <summary>
        /// 魔法攻击力最大值
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMaxMagicAttackV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.MaxMAttackV;
            
            if (nOcc == 1)
            {
                val += GetIntelligence(client) * 0.88;
            }

            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MaxMAttack] + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.MaxMAttack] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.MaxMAttack] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.MaxMAttack];
            double origVal = val;

            val += client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MaxMAttack);

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxMAttack, origVal) - origVal);

            val += GetAddAttackV(client);

            //狂攻符咒
            val *= (1.0 + DBRoleBufferManager.ProcessAddTempAttack(client));

            //持续时间加属性
            val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.TimeAddMAttack);
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxMAttack);
            val += DBRoleBufferManager.ProcessTimeAddPkKingAttackProp(client, ExtPropIndexes.MaxMAttack);
            val += DBRoleBufferManager.ProcessTimeAddJunQiProp(client, ExtPropIndexes.MaxMAttack);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxMAttack, val);
            val += client.RoleOnceBuffer.GetExtProp((int)ExtPropIndexes.MaxMAttack);

            // 魔法攻击提升
            val *= (1.0 + DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IncreaseMagAttack));

            val *= (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.IncreaseMagAttack));

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP2];   // 卓越属性影响最大魔法攻击力

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP1];

            val *= (1 + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP4]);
            
            val *= (1 + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP3]);

            // 红名惩罚 [4/21/2014 LiaoWei]
            val = val - val * client.RedNameDebuffInfo[0];

            return Global.GMax(0, val);
        }

        /// <summary>
        /// 怪
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetMaxMagicAttackV(Monster monster)
        {
            int attackVal = monster.MonsterInfo.MaxAttack;
            return attackVal;
        }

        // 属性改造 [8/15/2013 LiaoWei]
        //道术攻击力最小值
        /*public static double GetMinDSAttackV(GameClient client)
        {
            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[client.ClientData.Occupation][client.ClientData.Level];
            double val = roleBasePropItem.MinDSAttackV;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MinDSAttack] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MinDSAttack);
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MinDSAttack, origVal) - origVal);

            //狂攻符咒
            val *= (1.0 + DBRoleBufferManager.ProcessAddTempAttack(client));
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MinDSAttack);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MinDSAttack, val);
            
            return Global.GMax(0, val);
        }

        //怪
        public static double GetMinDSAttackV(Monster monster)
        {
            int attackVal = monster.MinAttack;
            return attackVal;
        }

        //道术攻击力最大值
        public static double GetMaxDSAttackV(GameClient client)
        {
            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[client.ClientData.Occupation][client.ClientData.Level];
            double val = roleBasePropItem.MaxDSAttackV;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MaxDSAttack] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MaxDSAttack);
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxDSAttack, origVal) - origVal);

            //狂攻符咒
            val *= (1.0 + DBRoleBufferManager.ProcessAddTempAttack(client));

            //持续时间加属性
            val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.TimeAddDSAttack);
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxDSAttack);
            val += DBRoleBufferManager.ProcessTimeAddPkKingAttackProp(client, ExtPropIndexes.MaxDSAttack);
            val += DBRoleBufferManager.ProcessTimeAddJunQiProp(client, ExtPropIndexes.MaxDSAttack);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxDSAttack, val);
            val += client.RoleOnceBuffer.GetExtProp((int)ExtPropIndexes.MaxDSAttack);

            return Global.GMax(0, val);
        }

        //怪
        public static double GetMaxDSAttackV(Monster monster)
        {
            int attackVal = monster.MaxAttack;
            return attackVal;
        }*/

        /// <summary>
        /// 最大生命值
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMaxLifeV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.LifeV;

            if (nOcc == 0)
            {
                val += GetConstitution(client) * 5;
            }
            else if (nOcc == 1)
            {
                val += GetConstitution(client) * 3.6;
            }
            else if (nOcc == 2)
            {
                val += GetConstitution(client) * 4.2;
            }

            val *= (1 + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP8]); //2014-12-27

            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MaxLifeV] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MaxLifeV) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.MaxLifeV] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.MaxLifeV] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.MaxLifeV];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxLifeV, origVal) - origVal); //2014-12-27 百分比

            //处理生命符咒
            val *= (1.0 + DBRoleBufferManager.ProcessUpLifeLimit(client));
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxLifeV);
            val += DBRoleBufferManager.ProcessTimeAddJunQiProp(client, ExtPropIndexes.MaxLifeV);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxLifeV, val);

            val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.MU_ADDMAXHPVALUE);

            val = val * (1.0 + GetMaxLifePercentV(client));

            return val;
        }

        /// <summary>
        /// 最大生命值(加成比例)
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMaxLifePercentV(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MaxLifePercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MaxLifePercent) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.MaxLifePercent] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.MaxLifePercent] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.MaxLifePercent];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxLifePercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxLifePercent);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxLifePercent, val);

            // 红名惩罚 [4/21/2014 LiaoWei]
            val -= client.RedNameDebuffInfo[2];

            return val;
        }

        /// <summary>
        /// 击中生命恢复
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetLifeStealV(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.LifeSteal] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.LifeSteal) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.LifeSteal] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.LifeSteal] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.LifeSteal];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.LifeSteal, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.LifeSteal);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.LifeSteal, val);

            return val;
        }

        /// <summary>
        /// 添加攻击力
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static double GetAddAttackV(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.AddAttack] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.AddAttack) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.AddAttack] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.AddAttack] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.AddAttack];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.AddAttack, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.AddAttack);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.AddAttack, val);

            return val;
        }

        /// <summary>
        /// 添加防御力
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private static double GetAddDefenseV(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.AddDefense] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.AddDefense) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.AddDefense] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.AddDefense] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.AddDefense];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.AddDefense, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.AddDefense);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.AddDefense, val);

            return val;
        }

        /// <summary>
        /// 最大魔法值
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMaxMagicV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.MagicV;

            /*if (nOcc == 0)
            {
                val += GetConstitution(client) * 6.0;
            }
            else if (nOcc == 1)
            {
                val += GetConstitution(client) * 12.0;
            }
            else if (nOcc == 2)
            {
                val += GetConstitution(client) * 9.0;
            }*/

            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MaxMagicV] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MaxMagicV) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.MaxMagicV] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.MaxMagicV] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.MaxMagicV];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxMagicV, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxMagicV);

            val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.MU_ADDMAXMPVALUE);

            val = val * (1.0 + GetMaxMagicPercent(client));

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxMagicV, val);
        }

        /// <summary>
        /// 幸运值：
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetLuckV(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.Lucky] * 100.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.Lucky) * 100 + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.Lucky] * 100 +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.Lucky] * 100 + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.Lucky] * 100;
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.Lucky, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.Lucky);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.Lucky, val);

            val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPLUCKYATTACK);

            val += client.ClientData.LuckProp;
            

            return val; // 属性改造 直接返回lucky值 [8/15/2013 LiaoWei]     //val - GetCurseV(client); //外部真正使用到的是幸运减去诅咒的值
        }

        /// <summary>
        /// 怪的幸运值
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetLuckV(Monster monster)
        {
            double val = monster.MonsterInfo.MonsterLucky;
            return val;
        }

        // 属性改造 [8/15/2013 LiaoWei]
        //诅咒：
        /*public static double GetCurseV(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.Curse] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.Curse);
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.Curse, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.Curse);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.Curse, val);
            return val;
        }*/

        /// <summary>
        /// 命中值：
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetHitV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.HitV;

            val *= (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.HitV));

            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.HitV] + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.HitV] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.HitV] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.HitV];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.HitV, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.HitV);

            val += GetDexterity(client) * 0.5;
            
            val *= (1 + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP6]);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.HitV, val);
        }

        /// <summary>
        /// 怪
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetHitV(Monster monster)
        {
            double val = monster.MonsterInfo.HitV;
            val *= (1.0 + monster.TempPropsBuffer.GetExtProp((int)ExtPropIndexes.HitV));
            return val;
        }

        /// <summary>
        /// 闪避值：
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetDodgeV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.Dodge;

            val += GetDexterity(client) * 0.5;

            val *= (1 + client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP12]);

            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.Dodge] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.Dodge) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.Dodge] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.Dodge] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.Dodge];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.Dodge, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.Dodge);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.Dodge, val);
        }

        /// <summary>
        /// 怪
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetDodgeV(Monster monster)
        {
            return monster.MonsterInfo.Dodge;
        }

        // 属性改造 [8/15/2013 LiaoWei]
        //魔法闪避值(百分比)：
        /*public static double GetMagicDodgePercentV(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MagicDodgePercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MagicDodgePercent);
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MagicDodgePercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MagicDodgePercent);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MagicDodgePercent, val);
        }

        //怪
        public static double GetMagicDodgePercentV(Monster monster)
        {
            return 0.0;
        }

        //中毒恢复(百分比)：
        public static double GetPoisoningReoverPercentV(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.PoisoningReoverPercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.PoisoningReoverPercent);
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.PoisoningReoverPercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.PoisoningReoverPercent);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.PoisoningReoverPercent, val);
        }

        //中毒闪避(百分比):
        public static double GetPoisoningDodgeV(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.PoisoningDodge] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.PoisoningDodge);
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.PoisoningDodge, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.PoisoningDodge);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.PoisoningDodge, val);
        }*/

        /// <summary>
        /// 定时生命回复比例：
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetLifeRecoverValPercentV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.RecoverLifeV;

            return val;
        }

        /// <summary>
        /// 生命回复增加比例
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetLifeRecoverAddPercentV(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.LifeRecoverPercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.LifeRecoverPercent) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.LifeRecoverPercent] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.LifeRecoverPercent] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.LifeRecoverPercent];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.LifeRecoverPercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.LifeRecoverPercent);

            val += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.MU_ADDLIFERECOVERPERCENT);
            
            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.LifeRecoverPercent, val);
            
        }

        /// <summary>
        /// 怪
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetLifeRecoverValPercentV(Monster monster)
        {
            return monster.MonsterInfo.RecoverLifeV;
        }

        /// <summary>
        /// 魔法回复速度：
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMagicRecoverValPercentV(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.RecoverMagicV;

            return val;
        }

        /// <summary>
        /// 魔法回复增加比例
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMagicRecoverAddPercentV(GameClient client)
        {
            /*double val = 0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MagicRecoverPercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MagicRecoverPercent);
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MagicRecoverPercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MagicRecoverPercent);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MagicRecoverPercent, val);*/
            return 0.0;
        }

        /// <summary>
        /// 怪
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetMagicRecoverValPercentV(Monster monster)
        {
            return monster.MonsterInfo.RecoverMagicV;
        }

        // 属性改造 [8/15/2013 LiaoWei]
        // 伤害吸收魔法/物理(百分比)
        public static double GetSubAttackInjurePercent(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.SubAttackInjurePercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.SubAttackInjurePercent) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.SubAttackInjurePercent] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.SubAttackInjurePercent] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.SubAttackInjurePercent];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.SubAttackInjurePercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.SubAttackInjurePercent);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.SubAttackInjurePercent, val);
        }

        /// <summary>
        /// 怪的伤害吸收魔法/物理(百分比)
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetSubAttackInjurePercent(Monster monster)
        {
            double val = monster.MonsterInfo.MonsterSubAttackInjurePercent;
            return val;
        }

        /// <summary>
        /// 伤害吸收魔法/物理(固定值)
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetSubAttackInjureValue(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.SubAttackInjure] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.SubAttackInjure) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.SubAttackInjure] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.SubAttackInjure] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.SubAttackInjure];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.SubAttackInjure, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.SubAttackInjure);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.SubAttackInjure, val);
        }

        /// <summary>
        /// 怪的伤害吸收魔法/物理(固定值)
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetSubAttackInjureValue(Monster monster)
        {
            double val = monster.MonsterInfo.MonsterSubAttackInjure;
            return val;
        }

        // 属性改造 [8/15/2013 LiaoWei]
        /*//吸收魔法伤害(百分比)
        public static double GetSubMAttackInjurePercent(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.SubMAttackInjurePercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.SubMAttackInjurePercent);
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.SubMAttackInjurePercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.SubMAttackInjurePercent);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.SubMAttackInjurePercent, val);
        }

        //吸收魔法伤害(百分比)
        public static double GetSubMAttackInjurePercent(Monster monster)
        {
            double val = 0.0;
            return val;
        }*/

        /*/// <summary>
        /// 腕力
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetHandFuZhong(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.HandFuZhong;

            return val;
        }*/

        /*/// <summary>
        /// 背包负重
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetBagFuZhong(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.BagFuZhong;

            return val;
        }*/

        /*/// <summary>
        /// 穿戴负重
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetDressFuZhong(GameClient client)
        {
            // 属性改造 加上一级属性公式 区分职业[8/15/2013 LiaoWei]
            int nOcc = Global.CalcOriginalOccupationID(client);

            RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[nOcc][client.ClientData.Level];
            double val = roleBasePropItem.DressFuZhong;

            return val;
        }*/

        /// <summary>
        /// 魔法上限增加(百分比)
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetMaxMagicPercent(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.MaxMagicPercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.MaxMagicPercent) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.MaxMagicPercent] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.MaxMagicPercent] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.MaxMagicPercent];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxMagicPercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxMagicPercent);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.MaxMagicPercent, val);

            return val;
        }

        /// <summary>
        /// 无视攻击对象的物理防御(概率)
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetIgnoreDefensePercent(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.IgnoreDefensePercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.IgnoreDefensePercent) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.IgnoreDefensePercent] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.IgnoreDefensePercent] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.IgnoreDefensePercent];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.IgnoreDefensePercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IgnoreDefensePercent);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.IgnoreDefensePercent, val);

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP14];

            return val;
        }

        /// <summary>
        /// 怪物无视攻击对象的物理防御(概率)
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetIgnoreDefensePercent(Monster monster)
        {
            double val = monster.MonsterInfo.MonsterIgnoreDefensePercent;
            return val;
        }

        // 属性改造 [8/15/2013 LiaoWei]
        /*//无视攻击对象的魔法防御(概率)
        public static double GetIgnoreMDefensePercent(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.IgnoreMDefensePercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.IgnoreMDefensePercent);
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.IgnoreMDefensePercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IgnoreMDefensePercent);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.IgnoreMDefensePercent, val);
            return val;
        }*/

        /// <summary>
        /// 伤害减少百分比(物理、魔法) [1/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetDecreaseInjurePercent(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.DecreaseInjurePercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.DecreaseInjurePercent) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.DecreaseInjurePercent] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.DecreaseInjurePercent] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.DecreaseInjurePercent];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.DecreaseInjurePercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DecreaseInjurePercent);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.DecreaseInjurePercent, val);
        }

        /// <summary>
        /// 怪物伤害减少百分比(物理、魔法) [1/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetDecreaseInjurePercent(Monster monster)
        {
            double val = 0.0;
            return val;
        }

        /// <summary>
        /// 伤害减少数值(物理、魔法) [1/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetDecreaseInjureValue(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.DecreaseInjureValue] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.DecreaseInjureValue) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.DecreaseInjureValue] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.DecreaseInjureValue] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.DecreaseInjureValue];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.DecreaseInjureValue, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DecreaseInjureValue);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.DecreaseInjureValue, val);
        }

        /// <summary>
        /// 怪物伤害减少数值(物理、魔法) [1/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetDecreaseInjureValue(Monster monster)
        {
            double val = 0.0;
            return val;
        }

        /// <summary>
        /// 伤害抵挡百分比(物理、魔法) [1/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetCounteractInjurePercent(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.CounteractInjurePercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.CounteractInjurePercent) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.CounteractInjurePercent] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.CounteractInjurePercent] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.CounteractInjurePercent];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.CounteractInjurePercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.CounteractInjurePercent);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.CounteractInjurePercent, val);
        }

        /// <summary>
        /// 怪物伤害抵挡百分比(物理、魔法) [1/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetCounteractInjurePercent(Monster monster)
        {
            double val = 0.0;
            return val;
        }

        /// <summary>
        /// 伤害抵挡数值(物理、魔法) [1/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetCounteractInjureValue(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.CounteractInjureValue] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.CounteractInjureValue) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.CounteractInjureValue] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.CounteractInjureValue] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.CounteractInjureValue];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.CounteractInjureValue, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.CounteractInjureValue);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.CounteractInjureValue, val);
        }

        /// <summary>
        /// 怪物伤害抵挡数值(物理、魔法) [1/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetCounteractInjureValue(Monster monster)
        {
            double val = 0.0;
            return val;
        }

        /// <summary>
        // 伤害加成魔法/物理(百分比) [3/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetAddAttackInjurePercent(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.AddAttackInjurePercent] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.AddAttackInjurePercent) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.AddAttackInjurePercent] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.AddAttackInjurePercent] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.AddAttackInjurePercent];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.AddAttackInjurePercent, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.AddAttackInjurePercent);

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP5] / 100;

            val += DBRoleBufferManager.ProcessTimeAddPkKingAttackProp(client, ExtPropIndexes.AddAttackInjurePercent);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.AddAttackInjurePercent, val);
        }

        /// <summary>
        /// 伤害加成魔法/物理(百分比) [3/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetAddAttackInjurePercent(Monster monster)
        {
            double val = 0.0;
            return val;
        }

        /// <summary>
        // 伤害加成魔法/物理(固定值) [3/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetAddAttackInjureValue(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.AddAttackInjure] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.AddAttackInjure) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.AddAttackInjure] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.AddAttackInjure] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.AddAttackInjure];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.AddAttackInjure, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.AddAttackInjure);

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.AddAttackInjure, val);
        }

        /// <summary>
        /// 伤害加成魔法/物理(固定值) [3/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetAddAttackInjureValue(Monster monster)
        {
            double val = 0.0;
            return val;
        }

        /// <summary>
        // 无视伤害的比例 [3/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetIgnoreDefenseRate(GameClient client)
        {
            double val = 0.0;
            val = val + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.IgnoreDefenseRate] + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.IgnoreDefenseRate) + client.ClientData.PictureJudgeProp.SecondPropsValue[(int)ExtPropIndexes.IgnoreDefenseRate] +
                            client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[(int)ExtPropIndexes.IgnoreDefenseRate] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[(int)ExtPropIndexes.IgnoreDefenseRate];
            double origVal = val;

            val += (client.AllThingsMultipliedBuffer.GetExtProp((int)ExtPropIndexes.IgnoreDefenseRate, origVal) - origVal);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IgnoreDefenseRate);

            val += client.ClientData.ExcellenceProp[(int)ExcellencePorp.EXCELLENCEPORP7];

            return client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.IgnoreDefenseRate, val);
        }

        /// <summary>
        /// 怪无视伤害的比例 [3/6/2014 LiaoWei]
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static double GetIgnoreDefenseRate(Monster monster)
        {
            double val = monster.MonsterInfo.MonsterIgnoreDefenseRate;
            return val;
        }

        #endregion 基础属性值公式

        #region 攻击伤害计算公式

        /// <summary>
        /// 获取攻击力的一个公式
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="val"></param>
        /// <param name="luck"></param>
        /// <param name="nFatalValue"></param>
        /// <param name="nDoubleValue"></param>
        /// <returns></returns>
        private static double GetAttackPower(IObject obj, int damage, int val, int luck, int nFatalValue, int nDoubleValue, out int nDamageType, int nMaxAttackValue)
        {
            nDamageType = (int)DamageType.DAMAGETYPE_DEFAULT;   // 默认伤害类型

            GameClient client = null;

            if (val < 0) val = 0;

            int result = 0;
            if (luck > 0)
            {
                int maxRandNum = 100 - Math.Min(99, luck); 
                if (Global.GetRandomNumber(0, maxRandNum) <= 0)
                {
                    result = damage + val;

                    double dValue = 0.0;

                    if (obj is GameClient)
                    {
                        client = obj as GameClient;

                        dValue = DBRoleBufferManager.ProcessSpecialAttackValueBuff(client, (int)BufferItemTypes.MU_ADDLUCKYATTACKPERCENTTIMER);
                    }

                    result = result + (int)(result * dValue);

                    nDamageType = (int)DamageType.DAMAGETYPE_LUCKYATTACK;   // 幸运一击
                }
                else
                {
                    result = damage + Global.GetRandomNumber(0, val + 1);
                }
            }
            else
            {
                result = damage + Global.GetRandomNumber(0, val + 1);
                if (val < 0)
                {
                    int maxRandNum = 10 - Math.Max(0, -luck);
                    if (Global.GetRandomNumber(0, maxRandNum) <= 0)
                    {
                        result = damage;
                    }
                }
            }

            // 属性改造 增加2个二级属性[8/15/2013 LiaoWei]
            // 1.卓越一击
            int nRan = Global.GetRandomNumber(0, 101);
            if (nRan < nFatalValue)
            {
                //result = (int)(result * 1.2);

                result = (int)(nMaxAttackValue * 1.2);
                
                double dValue = 0.0;

                client = obj as GameClient;
                if (client != null)
                    dValue = DBRoleBufferManager.ProcessSpecialAttackValueBuff(client, (int)BufferItemTypes.MU_ADDFATALATTACKPERCENTTIMER);

                result = result + (int)(result * dValue);

                nDamageType = (int)DamageType.DAMAGETYPE_EXCELLENCEATTACK;   // 卓越一击
            }

            // 2.双倍一击
            nRan = Global.GetRandomNumber(0, 101);
            if (nRan < nDoubleValue)
            {
                result = result * 2;

                double dValue = 0.0;

                client = obj as GameClient;
                if (client != null)
                    dValue = DBRoleBufferManager.ProcessSpecialAttackValueBuff(client, (int)BufferItemTypes.MU_ADDDOUBLEATTACKPERCENTTIMER);

                result = result + (int)(result * dValue);

                nDamageType = (int)DamageType.DAMAGETYPE_DOUBLEATTACK;   // 双倍一击
            }

            return result;
        }

        /// <summary>
        /// 获取防御力一个公式
        /// </summary>
        /// <param name="baseDefense"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private static double GetDefensePower(int baseDefense, int val)
        {
            if (val < 0) val = 0;
            return baseDefense + Global.GetRandomNumber(0, val + 1);
        }

        /// <summary>
        /// 获取防御力的一个公式
        /// </summary>
        /// <param name="minDefense"></param>
        /// <param name="maxDefense"></param>
        /// <returns></returns>
        private static double GetDefenseValue(int minDefense, int maxDefense)
        {
            return GetDefensePower(minDefense, maxDefense - minDefense);
        }

        /// <summary>
        /// 计算物理攻击力
        /// </summary>
        /// <param name="minAttackV"></param>
        /// <param name="maxAttackV"></param>
        /// <param name="lucky"></param>
        /// <param name="nFatalValue"></param>
        /// <param name="nDoubleValue"></param>
        /// <returns></returns>
        public static double CalcAttackValue(IObject obj, int minAttackV, int maxAttackV, int lucky, int nFatalValue, int nDoubleValue, out int nDamageType)
        {
            nDamageType = 0;

            return GetAttackPower(obj, minAttackV, maxAttackV - minAttackV, lucky, nFatalValue, nDoubleValue, out nDamageType, maxAttackV);
        }

        /// <summary>
        /// 真正的伤害
        /// </summary>
        /// <param name="attackV"></param>
        /// <param name="defenseV"></param>
        /// <returns></returns>
        public static double GetRealInjuredValue(long attackV, long defenseV)
        {
            double newJnjureV = 0.0;
            
            // MU修正伤害计算 [1/22/2014 LiaoWei]
            //defenseV = defenseV / 2;

            //newJnjureV = Math.Max(attackV - defenseV, 0.0);

            /*double percent = Math.Max((1.0 -((double)defenseV / ((double)defenseV + 1000.0))), 0.05);
            newJnjureV = (int)Math.Max(attackV * percent, 0.0);
            newJnjureV = Math.Max(newJnjureV, 1);*/

            // MU 再次修改伤害公式 伤害=自己攻击^2/（自身攻击+对方防御） [2/11/2014 LiaoWei]
            //newJnjureV = (int)Math.Max((attackV * attackV) / (attackV + defenseV), 1.0);

            // MU 再次修改伤害公式 伤害=MAX((攻击-防御/4),攻击*0.1) [5/8/2014 LiaoWei]
            //newJnjureV = (int)Math.Max((attackV - defenseV / 4), attackV * 0.1);

            // MU 再次修改伤害公式 伤害=MAX((攻击-对方防御/4),MAX(攻击*0.1,5)) [5/8/2014 LiaoWei]
            // MU 再次修改伤害公式 伤害=MAX((攻击-对方防御),MAX(攻击*0.1,5)) [6/19/2014 LiaoWei]
            newJnjureV = (int)Math.Max((attackV - defenseV), (int)Math.Max(attackV * 0.1, 5));

            return newJnjureV;
        }

        /// <summary>
        /// 命中率=攻击方命中值/（攻击方命中值+被攻击方闪避值/4.5）
        /// 命中率最小为75%，当命中率小于75%时按75%计算
        /// Rand<=命中率*100 为命中，否则为闪避
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="hitV"></param>
        /// <param name="dodgeV"></param>
        /// <returns></returns>
        public static double GetRealHitRate(double hitV, double dodgeV)
        {
            // 属性改造  命中率=max(攻击方命中值/(攻击方命中值+被攻击方闪避值), 3%)   [8/15/2013 LiaoWei]
            // 再次修改命中 命中率=max(攻击方命中值/(攻击方命中值+被攻击方闪避值/2), 3%) [1/22/2014 LiaoWei]
            // 命中公式调整为：命中率=max(自身命中/(自身命中+对方闪避/4), 3%) [1/23/2014 LiaoWei]
            // 再次再次修改命中公式：命中率=max(自己命中/（自己命中+对方闪避/10）,3%) [3/6/2014 LiaoWei]

            if (dodgeV <= 0.0)
            {
                return 1;
            }

            int rndNum = Global.GetRandomNumber(0, 101);

            //double newHitV = hitV / dodgeV;
            //int value = (int)(newHitV * 100.0);

            int nHit = (int)(hitV / (hitV + dodgeV / 10) * 100.0);

            int value = Global.GMax(nHit, 3);

            return (rndNum <= value) ? 1 : 0;
        }

        /// <summary>
        /// 获取闪避率
        /// </summary>
        /// <param name="DodgePercent"></param>
        /// <returns></returns>
        public static double GetRealHitRate(double DodgePercent)
        {
            if (DodgePercent <= 0.0)
            {
                return 1;
            }

            int rndNum = Global.GetRandomNumber(0, 101);
            int value = (int)(DodgePercent * 100.0);
            return (rndNum <= value) ? 0 : 1; //意思反过来，闪避掉，不闪避
        }

        /// <summary>
        /// 计算伤害吸收
        /// </summary>
        /// <param name="injured"></param>
        /// <returns></returns>
        public static int CalcAttackInjure(int attackType, IObject obj, int injured)
        {
            // 属性改造 加上新增加的"伤害吸收魔法/物理(固定值)" [8/15/2013 LiaoWei]
            // 增加 "伤害减少百分比(物理、魔法) 伤害减少数值(物理、魔法) 伤害抵挡百分比(物理、魔法) 伤害抵挡数值(物理、魔法)" [1/6/2014 LiaoWei]
            
            var subPercent      = 0.0;
            var subValue        = 0.0; 
            var DecreasePercent = 0.0;
            var DecreaseValue   = 0.0;
            var CounteractPercent = 0.0;
            var CounteractValue = 0.0;
            
            // 1.不区分物理、魔法攻击类型 2.增加伤害吸收魔法/物理(固定值)
            if (obj is GameClient)
            {
                /*if (0 == attackType)
                {
                    subPercent = RoleAlgorithm.GetSubAttackInjurePercent(obj as GameClient);
                }
                else
                {
                    subPercent = RoleAlgorithm.GetSubMAttackInjurePercent(obj as GameClient);
                }*/
                subPercent      = RoleAlgorithm.GetSubAttackInjurePercent(obj as GameClient);
                
                subValue        = RoleAlgorithm.GetSubAttackInjureValue(obj as GameClient);

                DecreasePercent = RoleAlgorithm.GetDecreaseInjurePercent(obj as GameClient);

                DecreaseValue   = RoleAlgorithm.GetDecreaseInjureValue(obj as GameClient);

                CounteractPercent = RoleAlgorithm.GetCounteractInjurePercent(obj as GameClient);

                CounteractValue = RoleAlgorithm.GetCounteractInjureValue(obj as GameClient);
            }
            else if (obj is Monster)
            {
                /*if (0 == attackType)
                {
                     subPercent = RoleAlgorithm.GetSubAttackInjurePercent(obj as Monster);
                }
                else
                {
                    subPercent = RoleAlgorithm.GetSubMAttackInjurePercent(obj as Monster);
                }*/
                subPercent  = RoleAlgorithm.GetSubAttackInjurePercent(obj as Monster);

                subValue    = RoleAlgorithm.GetSubAttackInjureValue(obj as Monster);

                DecreasePercent = RoleAlgorithm.GetDecreaseInjurePercent(obj as Monster);

                DecreaseValue = RoleAlgorithm.GetDecreaseInjureValue(obj as Monster);

                CounteractPercent = RoleAlgorithm.GetCounteractInjurePercent(obj as Monster);

                CounteractValue = RoleAlgorithm.GetCounteractInjureValue(obj as Monster);
            }

            injured = (int)(((((injured * (1.0 - subPercent) - subValue) * (1 - DecreasePercent)) - DecreaseValue) * (1 - CounteractPercent)) - CounteractValue);
            
            return injured;
        }

        /// <summary>
        /// 获取忽视对方防御后的防御值
        /// </summary>
        /// <param name="DodgePercent"></param>
        /// <returns></returns>
        public static int GetDefenseByCalcingIgnoreDefense(int attackType, IObject self, int defense)
        {
            // 属性改造 [8/15/2013 LiaoWei]
            var ignorePercent = 0.0;
            /*if (0 == attackType) //物理防御
            {
                ignorePercent = RoleAlgorithm.GetIgnoreDefensePercent(self as GameClient);
            }
            else
            {
                ignorePercent = RoleAlgorithm.GetIgnoreMDefensePercent(self as GameClient);
            }*/

            if (self is GameClient)
                ignorePercent = RoleAlgorithm.GetIgnoreDefensePercent(self as GameClient);
            else if (self is Monster)
                ignorePercent = RoleAlgorithm.GetIgnoreDefensePercent(self as Monster);

            if (ignorePercent <= 0.0)
            {
                return defense;
            }

            int rndNum = Global.GetRandomNumber(0, 101);
            int value = (int)(ignorePercent * 100.0);
            return (rndNum <= value) ? 0 : defense;
        }

        #endregion //攻击伤害计算公式

        #region 物理攻击伤害计算

        /// <summary>
        /// 角色攻击怪的计算公式
        /// </summary>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        /// <param name="burst"></param>
        /// <param name="injure"></param>
        public static void AttackEnemy(GameClient client, Monster monster, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue)
        {
            burst = 0;
            injure = 0;

            int minAttackV = (int)RoleAlgorithm.GetMinAttackV(client);
            int maxAttackV = (int)RoleAlgorithm.GetMaxAttackV(client);
            int lucky = (int)RoleAlgorithm.GetLuckV(client);
            int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(client);    // 属性改造 卓越一击 [8/15/2013 LiaoWei]
            int nDoubleValue = (int)RoleAlgorithm.GetDoubleAttack(client);  // 属性改造 双倍一击 [8/15/2013 LiaoWei]
            int attackV = (int)RoleAlgorithm.CalcAttackValue(client, minAttackV+addAttackMin, maxAttackV+addAttackMax, lucky, nFatalValue, nDoubleValue, out burst);
            attackV = (int)(attackV * attackPercent);
            //attackV += addAttack;

            int minDefenseV = (int)RoleAlgorithm.GetMinADefenseV(monster);
            int maxDefenseV = (int)RoleAlgorithm.GetMaxADefenseV(monster);
            int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);

            if (ignoreDefenseAndDodge)
            {
                defenseV = 0;
            }

            //获取忽视对方防御后的防御值
            defenseV = GetDefenseByCalcingIgnoreDefense(0, client, defenseV);

            // 无视对方防御比例 [3/6/2014 LiaoWei]
            if (defenseV > 0)
                defenseV = (int)(defenseV * (1 - GetIgnoreDefenseRate(client)));
            else
                burst = (int)DamageType.DAMAGETYPE_IGNOREDEFENCE;   // 无视防御了

            injure = (int)RoleAlgorithm.GetRealInjuredValue(attackV, defenseV);

            // 注意  injure = injure * (1 + 伤害加成百分比) + 伤害加成值 [3/6/2014 LiaoWei]
            injure = (int)(injure * (1 + GetAddAttackInjurePercent(client)) + GetAddAttackInjureValue(client));

            //获取怪物的减伤比例
            injure = (int)(injure * injurePercnet);
            injure += addInjure;

            if (injure <= 0)
            {
                injure = -1;
                return;
            }

            //计算伤害吸收
            injure = RoleAlgorithm.CalcAttackInjure(0, monster, injure);

            // 处理物理技能增幅
            injure = (int)(injure * (1 + GetPhySkillIncrease(client)));

            if (ignoreDefenseAndDodge)
            {
                return;
            }

            // 技能改造[3/13/2014 LiaoWei]
            injure = (int)(injure * baseRate + addVlue); 

            //如果不是暴击判断是否命中
            double hitV = RoleAlgorithm.GetHitV(client);
            double dodgeV = RoleAlgorithm.GetDodgeV(monster);

            int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);

            if (hit <= 0) //表示对方闪避掉了自己的攻击
            {
                injure = 0;
            }
        }

        /// <summary>
        /// 怪攻击角色的计算公式
        /// </summary>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        /// <param name="burst"></param>
        /// <param name="injure"></param>
        public static void AttackEnemy(Monster monster, GameClient client, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue)
        {
            burst = 0;
            injure = 0;

            int minAttackV = (int)RoleAlgorithm.GetMinAttackV(monster);
            int maxAttackV = (int)RoleAlgorithm.GetMaxAttackV(monster);
            int lucky = (int)RoleAlgorithm.GetLuckV(monster);
            int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(monster);    // 属性改造 卓越一击 [8/15/2013 LiaoWei]
            int nDoubleValue = (int)RoleAlgorithm.GetDoubleAttack(monster);  // 属性改造 双倍一击 [8/15/2013 LiaoWei]

            if (monster is Robot)
            {
                Robot robot = monster as Robot;

                lucky = robot.Lucky;
                nFatalValue = robot.FatalValue;    // 属性改造 卓越一击 [8/15/2013 LiaoWei]
                nDoubleValue = robot.DoubleValue;  // 属性改造 双倍一击 [8/15/2013 LiaoWei]
            }

            int attackV = (int)RoleAlgorithm.CalcAttackValue(monster, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, nDoubleValue, out burst);
            attackV = (int)(attackV * attackPercent);
            //attackV += addAttack;

            int minDefenseV = (int)RoleAlgorithm.GetMinADefenseV(client);
            int maxDefenseV = (int)RoleAlgorithm.GetMaxADefenseV(client);
            int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);

            if (ignoreDefenseAndDodge)
            {
                defenseV = 0;
            }

            // 技能改造[3/13/2014 LiaoWei]
            injure = (int)(injure * baseRate + addVlue);

            //获取忽视对方防御后的防御值
            defenseV = GetDefenseByCalcingIgnoreDefense(0, monster, defenseV);

            // 无视对方防御比例 [3/6/2014 LiaoWei]
            if (defenseV > 0)
                defenseV = (int)(defenseV * (1 - GetIgnoreDefenseRate(monster)));
            
            injure = (int)RoleAlgorithm.GetRealInjuredValue(attackV, defenseV);

            // 注意  injure = injure * (1 + 伤害加成百分比) + 伤害加成值 [3/6/2014 LiaoWei]
            injure = (int)(injure * (1 + GetAddAttackInjurePercent(monster)) + GetAddAttackInjureValue(monster));

            injure = (int)(injure * injurePercnet);
            injure += addInjure;

            // 竞技场的伤害为50% ChenXiaojun
            if (monster is Robot)
            {
                injure /= 2;
            }

            if (injure <= 0)
            {
                injure = -1;
                return;
            }

            //计算伤害吸收
            injure = RoleAlgorithm.CalcAttackInjure(0, client, injure);

            if (ignoreDefenseAndDodge)
            {
                return;
            }

            //如果不是暴击判断是否命中
            double hitV = RoleAlgorithm.GetHitV(monster);
            double dodgeV = RoleAlgorithm.GetDodgeV(client);
            int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
            if (hit <= 0) //表示对方闪避掉了自己的攻击
            {
                injure = 0;
            }
        }

        /// <summary>
        /// 怪攻击怪物的计算公式
        /// </summary>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        /// <param name="burst"></param>
        /// <param name="injure"></param>
        public static void AttackEnemy(Monster monster, Monster enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge)
        {
            burst = 0;
            injure = 0;

            int minAttackV = (int)RoleAlgorithm.GetMinAttackV(monster);
            int maxAttackV = (int)RoleAlgorithm.GetMaxAttackV(monster);
            int lucky = (int)RoleAlgorithm.GetLuckV(monster);
            int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(monster);    // 属性改造 卓越一击 [8/15/2013 LiaoWei]
            int nDoubleValue = (int)RoleAlgorithm.GetDoubleAttack(monster);  // 属性改造 双倍一击 [8/15/2013 LiaoWei]

            if (monster is Robot)
            {
                Robot robot = monster as Robot;

                lucky = robot.Lucky;
                nFatalValue = robot.FatalValue;    // 属性改造 卓越一击 [8/15/2013 LiaoWei]
                nDoubleValue = robot.DoubleValue;  // 属性改造 双倍一击 [8/15/2013 LiaoWei]
            }
            
            int attackV = (int)RoleAlgorithm.CalcAttackValue(monster, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, nDoubleValue, out burst);
            attackV = (int)(attackV * attackPercent);
            //attackV += addAttack;

            int minDefenseV = (int)RoleAlgorithm.GetMinADefenseV(enemy);
            int maxDefenseV = (int)RoleAlgorithm.GetMaxADefenseV(enemy);
            int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);

            if (ignoreDefenseAndDodge)
            {
                defenseV = 0;
            }

            //获取忽视对方防御后的防御值
            defenseV = GetDefenseByCalcingIgnoreDefense(0, monster, defenseV);

            // 无视对方防御比例 [3/6/2014 LiaoWei]
            if (defenseV > 0)
                defenseV = (int)(defenseV * (1 - GetIgnoreDefenseRate(monster)));

            injure = (int)RoleAlgorithm.GetRealInjuredValue(attackV, defenseV);

            // 注意  injure = injure * (1 + 伤害加成百分比) + 伤害加成值 [3/6/2014 LiaoWei]
            injure = (int)(injure * (1 + GetAddAttackInjurePercent(monster)) + GetAddAttackInjureValue(monster));

            injure = (int)(injure * injurePercnet);
            injure += addInjure;

            if (injure <= 0)
            {
                injure = -1;
                return;
            }

            //计算伤害吸收
            injure = RoleAlgorithm.CalcAttackInjure(0, enemy, injure);

            if (ignoreDefenseAndDodge)
            {
                return;
            }

            //如果不是暴击判断是否命中
            double hitV = RoleAlgorithm.GetHitV(monster);
            double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
            int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
            if (hit <= 0) //表示对方闪避掉了自己的攻击
            {
                injure = 0;
            }
        }

        /// <summary>
        /// 角色攻击角色的计算公式
        /// </summary>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        /// <param name="burst"></param>
        /// <param name="injure"></param>
        public static void AttackEnemy(GameClient client, GameClient enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue)
        {
            burst = 0;
            injure = 0;

            int minAttackV = (int)RoleAlgorithm.GetMinAttackV(client);
            int maxAttackV = (int)RoleAlgorithm.GetMaxAttackV(client);
            int lucky = (int)RoleAlgorithm.GetLuckV(client);
            int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(client);    // 属性改造 卓越一击 [8/15/2013 LiaoWei]
            int nDoubleValue = (int)RoleAlgorithm.GetDoubleAttack(client);  // 属性改造 双倍一击 [8/15/2013 LiaoWei]
            int attackV = (int)RoleAlgorithm.CalcAttackValue(client, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, nDoubleValue, out burst);
            attackV = (int)(attackV * attackPercent);
            //attackV += addAttack;

            int minDefenseV = (int)RoleAlgorithm.GetMinADefenseV(enemy);
            int maxDefenseV = (int)RoleAlgorithm.GetMaxADefenseV(enemy);
            int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);

            if (ignoreDefenseAndDodge)
            {
                defenseV = 0;
            }

            //获取忽视对方防御后的防御值
            defenseV = GetDefenseByCalcingIgnoreDefense(0, client, defenseV);

            // 无视对方防御比例 [3/6/2014 LiaoWei]
            if (defenseV > 0)
                defenseV = (int)(defenseV * (1 - GetIgnoreDefenseRate(client)));
            else
                burst = (int)DamageType.DAMAGETYPE_IGNOREDEFENCE;   // 无视防御了

            injure = (int)RoleAlgorithm.GetRealInjuredValue(attackV, defenseV);

            // 注意  injure = injure * (1 + 伤害加成百分比) + 伤害加成值 [3/6/2014 LiaoWei]
            injure = (int)(injure * (1 + GetAddAttackInjurePercent(client)) + GetAddAttackInjureValue(client));

            injure = (int)(injure * injurePercnet);

            // 处理物理技能增幅
            injure = (int)(injure * (1 + GetPhySkillIncrease(client)));

            injure += addInjure;

            if (injure <= 0)
            {
                injure = -1;
                return;
            }

            //计算伤害吸收
            injure = RoleAlgorithm.CalcAttackInjure(0, enemy, injure);

            if (ignoreDefenseAndDodge)
            {
                return;
            }

            // 技能改造[3/13/2014 LiaoWei]
            injure = (int)(injure * baseRate + addVlue);

            //如果不是暴击判断是否命中
            double hitV = RoleAlgorithm.GetHitV(client);
            double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
            int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
            if (hit <= 0) //表示对方闪避掉了自己的攻击
            {
                injure = 0;
            }
        }

        #endregion //物理攻击伤害计算

        #region 魔法攻击伤害计算

        /// <summary>
        /// 角色攻击怪的计算公式
        /// </summary>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        /// <param name="burst"></param>
        /// <param name="injure"></param>
        public static void MAttackEnemy(GameClient client, Monster monster, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue)
        {
            burst = 0;
            injure = 0;

            int minAttackV = (int)RoleAlgorithm.GetMinMagicAttackV(client);
            int maxAttackV = (int)RoleAlgorithm.GetMaxMagicAttackV(client);
            int lucky = (int)RoleAlgorithm.GetLuckV(client);
            int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(client);    // 属性改造 卓越一击 [8/15/2013 LiaoWei]
            int nDoubleValue = (int)RoleAlgorithm.GetDoubleAttack(client);  // 属性改造 双倍一击 [8/15/2013 LiaoWei]
            int attackV = (int)RoleAlgorithm.CalcAttackValue(client, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, nDoubleValue, out burst);
            attackV = (int)(attackV * attackPercent);
            //attackV += addAttack;

            int minDefenseV = (int)RoleAlgorithm.GetMinMDefenseV(monster);
            int maxDefenseV = (int)RoleAlgorithm.GetMaxMDefenseV(monster);
            int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);

            if (ignoreDefenseAndDodge)
            {
                defenseV = 0;
            }

            //获取忽视对方防御后的防御值
            defenseV = GetDefenseByCalcingIgnoreDefense(1, client, defenseV);

            // 无视对方防御比例 [3/6/2014 LiaoWei]
            if (defenseV > 0)
                defenseV = (int)(defenseV * (1 - GetIgnoreDefenseRate(client)));
            else
                burst = (int)DamageType.DAMAGETYPE_IGNOREDEFENCE;   // 无视防御了

            // 处理魔法技能增幅
            injure = (int)(injure * (1 + GetMagicSkillIncrease(client)));

            injure = (int)RoleAlgorithm.GetRealInjuredValue(attackV, defenseV);

            // 注意  injure = injure * (1 + 伤害加成百分比) + 伤害加成值 [3/6/2014 LiaoWei]
            injure = (int)(injure * (1 + GetAddAttackInjurePercent(client)) + GetAddAttackInjureValue(client));

            //获取怪物的减伤比例
            injure = (int)(injure * injurePercnet);
            injure += addInjure;

            if (injure <= 0)
            {
                injure = -1;
                return;
            }

            //计算伤害吸收
            injure = RoleAlgorithm.CalcAttackInjure(1, monster, injure);

            if (ignoreDefenseAndDodge)
            {
                return;
            }

            // 技能改造[3/13/2014 LiaoWei]
            injure = (int)(injure * baseRate + addVlue);

            // 属性改造 去掉了”魔法闪避“[8/15/2013 LiaoWei]
            //如果不是暴击判断是否命中
            double hitV = RoleAlgorithm.GetHitV(client);
            double dodgeV = RoleAlgorithm.GetDodgeV(monster);
            int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
            //魔法闪避值(百分比)
            //int hit = (int)GetRealHitRate(GetMagicDodgePercentV(monster));
            if (hit <= 0) //表示对方闪避掉了自己的攻击
            {
                injure = 0;
            }
        }

        /// <summary>
        /// 怪攻击角色的计算公式
        /// </summary>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        /// <param name="burst"></param>
        /// <param name="injure"></param>
        public static void MAttackEnemy(Monster monster, GameClient client, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue)
        {
            burst = 0;
            injure = 0;

            int minAttackV = (int)RoleAlgorithm.GetMinMagicAttackV(monster);
            int maxAttackV = (int)RoleAlgorithm.GetMaxMagicAttackV(monster);
            int lucky = 0;
            int nFatalValue = 0;    // 属性改造 卓越一击 [8/15/2013 LiaoWei]
            int nDoubleValue = 0;  // 属性改造 双倍一击 [8/15/2013 LiaoWei]
            int attackV = (int)RoleAlgorithm.CalcAttackValue(monster, minAttackV + addAttackMin, maxAttackV + addAttackMax, lucky, nFatalValue, nDoubleValue, out burst);
            attackV = (int)(attackV * attackPercent);
            //attackV += addAttack;

            int minDefenseV = (int)RoleAlgorithm.GetMinMDefenseV(client);
            int maxDefenseV = (int)RoleAlgorithm.GetMaxMDefenseV(client);
            int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);

            if (ignoreDefenseAndDodge)
            {
                defenseV = 0;
            }

            // 无视对方防御比例 [3/6/2014 LiaoWei]
            if (defenseV > 0)
                defenseV = (int)(defenseV * (1 - GetIgnoreDefenseRate(monster)));

            injure = (int)RoleAlgorithm.GetRealInjuredValue(attackV, defenseV);

            // 注意  injure = injure * (1 + 伤害加成百分比) + 伤害加成值 [3/6/2014 LiaoWei]
            injure = (int)(injure * (1 + GetAddAttackInjurePercent(monster)) + GetAddAttackInjureValue(monster));

            //获取怪物的减伤比例
            injure = (int)(injure * injurePercnet);
            injure += addInjure;

            if (injure <= 0)
            {
                injure = -1;
                return;
            }

            //计算伤害吸收
            injure = RoleAlgorithm.CalcAttackInjure(1, client, injure);

            if (ignoreDefenseAndDodge)
            {
                return;
            }

            // 技能改造[3/13/2014 LiaoWei]
            injure = (int)(injure * baseRate + addVlue);

            // 属性改造 去掉了”魔法闪避“[8/15/2013 LiaoWei]
            //如果不是暴击判断是否命中
            double hitV = RoleAlgorithm.GetHitV(monster);
            double dodgeV = RoleAlgorithm.GetDodgeV(client);
            int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
            //魔法闪避值(百分比)
            //int hit = (int)GetRealHitRate(GetMagicDodgePercentV(client));
            if (hit <= 0) //表示对方闪避掉了自己的攻击
            {
                injure = 0;
            }
        }

        /// <summary>
        /// 怪攻击怪的计算公式
        /// </summary>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        /// <param name="burst"></param>
        /// <param name="injure"></param>
        public static void MAttackEnemy(Monster monster, Monster enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge)
        {
            burst = 0;
            injure = 0;

            int minAttackV = (int)RoleAlgorithm.GetMinMagicAttackV(monster);
            int maxAttackV = (int)RoleAlgorithm.GetMaxMagicAttackV(monster);
            int lucky = 0;
            int nFatalValue = 0;    // 属性改造 卓越一击 [8/15/2013 LiaoWei]
            int nDoubleValue = 0;  // 属性改造 双倍一击 [8/15/2013 LiaoWei]
            int attackV = (int)RoleAlgorithm.CalcAttackValue(monster, minAttackV+addAttackMin, maxAttackV+addAttackMax, lucky, nFatalValue, nDoubleValue, out burst);
            attackV = (int)(attackV * attackPercent);
            //attackV += addAttack;

            int minDefenseV = (int)RoleAlgorithm.GetMinMDefenseV(enemy);
            int maxDefenseV = (int)RoleAlgorithm.GetMaxMDefenseV(enemy);
            int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);

            if (ignoreDefenseAndDodge)
            {
                defenseV = 0;
            }

            // 无视对方防御比例 [3/6/2014 LiaoWei]
            if (defenseV > 0)
                defenseV = (int)(defenseV * (1 - GetIgnoreDefenseRate(monster)));

            injure = (int)RoleAlgorithm.GetRealInjuredValue(attackV, defenseV);

            // 注意  injure = injure * (1 + 伤害加成百分比) + 伤害加成值 [3/6/2014 LiaoWei]
            injure = (int)(injure * (1 + GetAddAttackInjurePercent(monster)) + GetAddAttackInjureValue(monster));

            //获取怪物的减伤比例
            injure = (int)(injure * injurePercnet);
            injure += addInjure;

            if (injure <= 0)
            {
                injure = -1;
                return;
            }

            //计算伤害吸收
            injure = RoleAlgorithm.CalcAttackInjure(1, enemy, injure);

            if (ignoreDefenseAndDodge)
            {
                return;
            }

            // 属性改造 去掉了”魔法闪避“[8/15/2013 LiaoWei]
            //如果不是暴击判断是否命中
            double hitV = RoleAlgorithm.GetHitV(monster);
            double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
            int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);

            //魔法闪避值(百分比)
            //int hit = (int)GetRealHitRate(GetMagicDodgePercentV(enemy));
            if (hit <= 0) //表示对方闪避掉了自己的攻击
            {
                injure = 0;
            }
        }

        /// <summary>
        /// 角色攻击角色的计算公式
        /// </summary>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        /// <param name="burst"></param>
        /// <param name="injure"></param>
        public static void MAttackEnemy(GameClient client, GameClient enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue)
        {
            burst = 0;
            injure = 0;

            int minAttackV = (int)RoleAlgorithm.GetMinMagicAttackV(client);
            int maxAttackV = (int)RoleAlgorithm.GetMaxMagicAttackV(client);
            int lucky = (int)RoleAlgorithm.GetLuckV(client);
            int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(client);    // 属性改造 卓越一击 [8/15/2013 LiaoWei]
            int nDoubleValue = (int)RoleAlgorithm.GetDoubleAttack(client);  // 属性改造 双倍一击 [8/15/2013 LiaoWei]
            int attackV = (int)RoleAlgorithm.CalcAttackValue(client, minAttackV+addAttackMin, maxAttackV+addAttackMax, lucky, nFatalValue, nDoubleValue, out burst);
            attackV = (int)(attackV * attackPercent);
            //attackV += addAttack;

            int minDefenseV = (int)RoleAlgorithm.GetMinMDefenseV(enemy);
            int maxDefenseV = (int)RoleAlgorithm.GetMaxMDefenseV(enemy);
            int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);

            if (ignoreDefenseAndDodge)
            {
                defenseV = 0;
            }

            //获取忽视对方防御后的防御值
            defenseV = GetDefenseByCalcingIgnoreDefense(1, client, defenseV);

            // 无视对方防御比例 [3/6/2014 LiaoWei]
            if (defenseV > 0)
                defenseV = (int)(defenseV * (1 - GetIgnoreDefenseRate(client)));
            else
                burst = (int)DamageType.DAMAGETYPE_IGNOREDEFENCE;   // 无视防御了

            if (defenseV < 0)
                defenseV = 0;

            // 处理魔法技能增幅
            injure = (int)(injure * (1 + GetMagicSkillIncrease(client)));

            injure = (int)RoleAlgorithm.GetRealInjuredValue(attackV, defenseV);

            // 注意  injure = injure * (1 + 伤害加成百分比) + 伤害加成值 [3/6/2014 LiaoWei]
            injure = (int)(injure * (1 + GetAddAttackInjurePercent(client)) + GetAddAttackInjureValue(client));

            injure = (int)(injure * injurePercnet);
            injure += addInjure;

            if (injure <= 0)
            {
                injure = -1;
                return;
            }

            //计算伤害吸收
            injure = RoleAlgorithm.CalcAttackInjure(1, enemy, injure);

            if (ignoreDefenseAndDodge)
            {
                return;
            }

            // 技能改造[3/13/2014 LiaoWei]
            injure = (int)(injure * baseRate + addVlue);

            // 属性改造 去掉了”魔法闪避“[8/15/2013 LiaoWei]
            //如果不是暴击判断是否命中
            double hitV = RoleAlgorithm.GetHitV(client);
            double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
            int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);

            //魔法闪避值(百分比)
            //int hit = (int)GetRealHitRate(GetMagicDodgePercentV(enemy));
            if (hit <= 0) //表示对方闪避掉了自己的攻击
            {
                injure = 0;
            }
        }

        #endregion //魔法攻击伤害计算

        #region 状态命中属性计算

        public static double GetRoleNegativeRate(GameClient client, double baseVal, ExtPropIndexes extPropIndex)
        {
            double val = 0.0;
            // 填的就是百分比，不应该除以100
            val = (client.ClientData.EquipProp.ExtProps[(int)extPropIndex]) + (client.RoleBuffer.GetExtProp((int)extPropIndex));

            val += DBRoleBufferManager.ProcessTempBufferProp(client, extPropIndex);

            val = client.RoleMultipliedBuffer.GetExtProp((int)extPropIndex, val);
            val += baseVal;

            return val;
        }

        /// <summary>
        /// 定身状态命中
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetRoleStateDingSheng(GameClient client, double baseVal)
        {
            //double val = 1.0;
            //// 填的就是百分比，不应该除以100
            //val = val * (1.0 + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.StateDingSheng]/* / 100.0*/) * (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.StateDingSheng)/* / 100.0*/);

            double val = 0.0;
            // 填的就是百分比，不应该除以100
            val = (client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.StateDingSheng]/* / 100.0*/) + (client.RoleBuffer.GetExtProp((int)ExtPropIndexes.StateDingSheng)/* / 100.0*/);
            
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateDingSheng);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.StateDingSheng, val);
            val += baseVal;

            val += 0.1 * client.ClientData.ChangeLifeCount;
            return val;
        }

        /// <summary>
        /// 减速状态命中
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetRoleStateMoveSpeed(GameClient client, double baseVal)
        {
            //double val = 1.0;
            //// 填的就是百分比，不应该除以100
            //val = val * (1.0 + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.StateMoveSpeed]/* / 100.0*/) * (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.StateMoveSpeed)/* / 100.0*/);

            double val = 0.0;
            // 填的就是百分比，不应该除以100
            val = (client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.StateMoveSpeed]/* / 100.0*/) + (client.RoleBuffer.GetExtProp((int)ExtPropIndexes.StateMoveSpeed)/* / 100.0*/);
            
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateMoveSpeed);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.StateMoveSpeed, val);
            val += baseVal;

            val += 0.1 * client.ClientData.ChangeLifeCount;
            return val;
        }

        /// <summary>
        /// 击退状态命中
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetRoleStateJiTui(GameClient client, double baseVal)
        {
            //double val = 1.0;
            //// 填的就是百分比，不应该除以100
            //val = (1.0 + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.StateJiTui]/* / 100.0*/) * (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.StateJiTui)/* / 100.0*/);

            double val = 0.0;
            // 填的就是百分比，不应该除以100
            val = (client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.StateJiTui]/* / 100.0*/) + (client.RoleBuffer.GetExtProp((int)ExtPropIndexes.StateJiTui)/* / 100.0*/);

            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateJiTui);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.StateJiTui, val);
            val += baseVal;

            return val;
        }

        /// <summary>
        /// 昏迷状态命中
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static double GetRoleStateHunMi(GameClient client, double baseVal)
        {
            //double val = 1.0;
            //// 填的就是百分比，不应该除以100
            //val = val * (1.0 + client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.StateHunMi]/* / 100.0*/) * (1.0 + client.RoleBuffer.GetExtProp((int)ExtPropIndexes.StateHunMi)/* / 100.0*/);

            double val = 0.0;
            // 填的就是百分比，不应该除以100
            val = (client.ClientData.EquipProp.ExtProps[(int)ExtPropIndexes.StateHunMi]/* / 100.0*/) + (client.RoleBuffer.GetExtProp((int)ExtPropIndexes.StateHunMi)/* / 100.0*/);
            
            val += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateHunMi);

            val = client.RoleMultipliedBuffer.GetExtProp((int)ExtPropIndexes.StateHunMi, val);
            val += baseVal;

            val += 0.1 * client.ClientData.ChangeLifeCount;
            return val;
        }

        #endregion 状态命中属性计算

        #region 道术攻击伤害计算

        // 属性改造 去掉道术攻击[8/15/2013 LiaoWei]

        /// <summary>
        /// 角色攻击怪的计算公式
        /// </summary>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        /// <param name="burst"></param>
        /// <param name="injure"></param>
        /*public static void DSAttackEnemy(GameClient client, Monster monster, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttack, out int burst, out int injure, bool ignoreDefenseAndDodge, bool ignoreDoge = false)
        {
            
            
            burst = 0;
            injure = 0;

            int minAttackV = (int)RoleAlgorithm.GetMinDSAttackV(client);
            int maxAttackV = (int)RoleAlgorithm.GetMaxDSAttackV(client);
            int lucky = (int)RoleAlgorithm.GetLuckV(client);
            int attackV = (int)RoleAlgorithm.CalcAttackValue(minAttackV, maxAttackV, lucky);
            attackV = (int)(attackV * attackPercent);
            attackV += addAttack;

            int minDefenseV = (int)RoleAlgorithm.GetMinMDefenseV(monster);
            int maxDefenseV = (int)RoleAlgorithm.GetMaxMDefenseV(monster);
            int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);

            if (ignoreDefenseAndDodge)
            {
                defenseV = 0;
            }

            //获取忽视对方防御后的防御值
            defenseV = GetDefenseByCalcingIgnoreDefense(1, client, defenseV);

            injure = (int)RoleAlgorithm.GetRealInjuredValue(attackV, defenseV);

            //获取怪物的减伤比例
            injure = (int)(injure * injurePercnet);
            injure += addInjure;

            if (injure <= 0)
            {
                injure = -1;
                return;
            }

            if (ignoreDefenseAndDodge)
            {
                return;
            }

            //计算伤害吸收
            injure = RoleAlgorithm.CalcAttackInjure(2, monster, injure);

            if (ignoreDoge)
            {
                return;
            }

            // 属性改造 去掉了”魔法闪避“[8/15/2013 LiaoWei]
            //如果不是暴击判断是否命中
            double hitV = RoleAlgorithm.GetHitV(client);
            double dodgeV = RoleAlgorithm.GetDodgeV(monster);
            int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
            //魔法闪避值(百分比)
            //int hit = (int)GetRealHitRate(GetMagicDodgePercentV(monster));
            if (hit <= 0) //表示对方闪避掉了自己的攻击
            {
                injure = 0;
            }
        }*/

        /// <summary>
        /// 怪攻击角色的计算公式
        /// </summary>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        /// <param name="burst"></param>
        /// <param name="injure"></param>
        /*public static void DSAttackEnemy(Monster monster, GameClient client, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttack, out int burst, out int injure, bool ignoreDefenseAndDodge, bool ignoreDoge = false)
        {
            burst = 0;
            injure = 0;

            int minAttackV = (int)RoleAlgorithm.GetMinDSAttackV(monster);
            int maxAttackV = (int)RoleAlgorithm.GetMaxDSAttackV(monster);
            int lucky = 0;
            int attackV = (int)RoleAlgorithm.CalcAttackValue(minAttackV, maxAttackV, lucky);
            attackV = (int)(attackV * attackPercent);
            attackV += addAttack;

            int minDefenseV = (int)RoleAlgorithm.GetMinMDefenseV(client);
            int maxDefenseV = (int)RoleAlgorithm.GetMaxMDefenseV(client);
            int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);

            if (ignoreDefenseAndDodge)
            {
                defenseV = 0;
            }

            injure = (int)RoleAlgorithm.GetRealInjuredValue(attackV, defenseV);

            //获取怪物的减伤比例
            injure = (int)(injure * injurePercnet);
            injure += addInjure;

            if (injure <= 0)
            {
                injure = -1;
                return;
            }

            if (ignoreDefenseAndDodge)
            {
                return;
            }

            //计算伤害吸收
            injure = RoleAlgorithm.CalcAttackInjure(2, client, injure);

            if (ignoreDoge)
            {
                return;
            }

            // 属性改造 去掉了”魔法闪避“[8/15/2013 LiaoWei]
            //如果不是暴击判断是否命中
            double hitV = RoleAlgorithm.GetHitV(monster);
            double dodgeV = RoleAlgorithm.GetDodgeV(client);
            int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
            //魔法闪避值(百分比)
            //int hit = (int)GetRealHitRate(GetMagicDodgePercentV(client));
            if (hit <= 0) //表示对方闪避掉了自己的攻击
            {
                injure = 0;
            }
        }

        /// <summary>
        /// 怪攻击怪的计算公式
        /// </summary>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        /// <param name="burst"></param>
        /// <param name="injure"></param>
        public static void DSAttackEnemy(Monster monster, Monster enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttack, out int burst, out int injure, bool ignoreDefenseAndDodge, bool ignoreDoge = false)
        {
            burst = 0;
            injure = 0;

            int minAttackV = (int)RoleAlgorithm.GetMinDSAttackV(monster);
            int maxAttackV = (int)RoleAlgorithm.GetMaxDSAttackV(monster);
            int lucky = 0;
            int attackV = (int)RoleAlgorithm.CalcAttackValue(minAttackV, maxAttackV, lucky);
            attackV = (int)(attackV * attackPercent);
            attackV += addAttack;

            int minDefenseV = (int)RoleAlgorithm.GetMinMDefenseV(enemy);
            int maxDefenseV = (int)RoleAlgorithm.GetMaxMDefenseV(enemy);
            int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);

            if (ignoreDefenseAndDodge)
            {
                defenseV = 0;
            }

            injure = (int)RoleAlgorithm.GetRealInjuredValue(attackV, defenseV);

            //获取怪物的减伤比例
            injure = (int)(injure * injurePercnet);
            injure += addInjure;

            if (injure <= 0)
            {
                injure = -1;
                return;
            }

            if (ignoreDefenseAndDodge)
            {
                return;
            }

            //计算伤害吸收
            injure = RoleAlgorithm.CalcAttackInjure(2, enemy, injure);

            if (ignoreDoge)
            {
                return;
            }

            // 属性改造 去掉了”魔法闪避“[8/15/2013 LiaoWei]
            //如果不是暴击判断是否命中
            double hitV = RoleAlgorithm.GetHitV(monster);
            double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
            int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
            //魔法闪避值(百分比)
            //int hit = (int)GetRealHitRate(GetMagicDodgePercentV(enemy));
            if (hit <= 0) //表示对方闪避掉了自己的攻击
            {
                injure = 0;
            }
        }

        /// <summary>
        /// 角色攻击角色的计算公式
        /// </summary>
        /// <param name="client"></param>
        /// <param name="monster"></param>
        /// <param name="burst"></param>
        /// <param name="injure"></param>
        public static void DSAttackEnemy(GameClient client, GameClient enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttack, out int burst, out int injure, bool ignoreDefenseAndDodge, bool ignoreDoge = false)
        {
            burst = 0;
            injure = 0;

            int minAttackV = (int)RoleAlgorithm.GetMinDSAttackV(client);
            int maxAttackV = (int)RoleAlgorithm.GetMaxDSAttackV(client);
            int lucky = (int)RoleAlgorithm.GetLuckV(client);
            int attackV = (int)RoleAlgorithm.CalcAttackValue(minAttackV, maxAttackV, lucky);
            attackV = (int)(attackV * attackPercent);
            attackV += addAttack;

            int minDefenseV = (int)RoleAlgorithm.GetMinMDefenseV(enemy);
            int maxDefenseV = (int)RoleAlgorithm.GetMaxMDefenseV(enemy);
            int defenseV = (int)RoleAlgorithm.GetDefenseValue(minDefenseV, maxDefenseV);

            if (ignoreDefenseAndDodge)
            {
                defenseV = 0;
            }

            //获取忽视对方防御后的防御值
            defenseV = GetDefenseByCalcingIgnoreDefense(1, client, defenseV);

            injure = (int)RoleAlgorithm.GetRealInjuredValue(attackV, defenseV);

            injure = (int)(injure * injurePercnet);
            injure += addInjure;

            if (injure <= 0)
            {
                injure = -1;
                return;
            }

            if (ignoreDefenseAndDodge)
            {
                return;
            }

            //计算伤害吸收
            injure = RoleAlgorithm.CalcAttackInjure(2, enemy, injure);

            if (ignoreDoge)
            {
                return;
            }

            // 属性改造 去掉了”魔法闪避“[8/15/2013 LiaoWei]
            //如果不是暴击判断是否命中
            double hitV = RoleAlgorithm.GetHitV(client);
            double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
            int hit = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
            //魔法闪避值(百分比)
            //int hit = (int)GetRealHitRate(GetMagicDodgePercentV(enemy));
            if (hit <= 0) //表示对方闪避掉了自己的攻击
            {
                injure = 0;
            }
        }*/

        #endregion 道术攻击伤害计算
    }
}
