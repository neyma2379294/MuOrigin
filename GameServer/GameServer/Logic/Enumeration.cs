﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Logic
{
    //00000 站立
    //00001 行走
    //00002 奔跑
    //00003 武力攻击(近战)
    //00006 魔法攻击(远战)
    //00009 弓箭攻击(远战)
    //00012 死亡
    //00014 骑马站立
    //00015 骑马走
    //00016 骑马奔跑
    //00020 骑马死亡
    //00023 打坐

    /// <summary>
    /// 精灵动作
    /// </summary>
    public enum GActions
    {
        /// <summary>
        /// 站立
        /// </summary>
        Stand = 0,

        /// <summary>
        /// 行走
        /// </summary>
        Walk = 1,

        /// <summary>
        /// 奔跑
        /// </summary>
        Run = 2,

        /// <summary>
        /// 武力攻击
        /// </summary>
        Attack = 3,

        /// <summary>
        /// 被击
        /// </summary>
        Injured = 4,

        /// <summary>
        /// 魔法攻击
        /// </summary>
        Magic = 6,

        /// <summary>
        /// 弓箭攻击
        /// </summary>
        Bow = 9,

        /// <summary>
        /// 死亡
        /// </summary>
        Death = 12,

        /// <summary>
        /// 骑马站立
        /// </summary>
        HorseStand = 14,

        /// <summary>
        /// 骑马奔跑
        /// </summary>
        HorseRun = 16,

        /// <summary>
        /// 骑马死亡
        /// </summary>
        HorseDead = 20,

        /// <summary>
        /// 打坐
        /// </summary>
        Sit = 23,

        /// <summary>
        /// 攻击待机
        /// </summary>
        PreAttack = 24,

        /// <summary>
        /// 休闲待机
        /// </summary>
        IdleStand = 25,

        /// <summary>
        /// 斜靠墙壁
        /// </summary>
        Italic = 26,

        /// <summary>
        /// 采集
        /// </summary>
        Collect = 27,

        /// <summary>
        /// 问好
        /// </summary>
        Wenhao = 28,

        /// <summary>
        /// 跟我来
        /// </summary>
        Genwolai = 29,

        /// <summary>
        /// 鼓掌
        /// </summary>
        Guzhang = 30,

        /// <summary>
        /// 欢呼
        /// </summary>
        Huanhu = 31,

        /// <summary>
        /// 沮丧
        /// </summary>
        Jushang = 32,

        /// <summary>
        /// 行礼
        /// </summary>
        Xingli = 33,

        /// <summary>
        /// 冲锋
        /// </summary>
        Chongfeng = 34,

        /// <summary>
        /// 膜拜
        /// </summary>
        Mobai = 35,

        /// <summary>
        /// 挑衅
        /// </summary>
        Tiaoxin = 36,

        /// <summary>
        /// 坐下
        /// </summary>
        Zuoxia = 37,

        /// <summary>
        /// 睡觉
        /// </summary>
        Shuijiao = 38,

        /// <summary>
        /// 最大的动作值
        /// </summary>
        MaxAction,
    }

    /// <summary>
    /// 精灵的类型
    /// </summary>
    public enum GSpriteTypes
    {
        /// <summary>
        /// 主角
        /// </summary>
        Leader = 0,

        /// <summary>
        /// 其他玩家
        /// </summary>
        Other,

        /// <summary>
        /// 怪物
        /// </summary>
        Monster,

        /// <summary>
        /// NPC
        /// </summary>
        NPC,

        /// <summary>
        /// 宠物
        /// </summary>
        Pet,

        /// <summary>
        /// 镖车
        /// </summary>
        BiaoChe,

        /// <summary>
        /// 帮旗
        /// </summary>
        JunQi,

        /// <summary>
        /// 假人
        /// </summary>
        FakeRole,
    }

    /// <summary>
    /// 精灵(角色/怪)的PK模式
    /// </summary>
    public enum GPKModes
    {
        /// <summary>
        /// 普通模式(用户无法攻击其他玩家，也不会被其他玩家所攻击; 玩家对怪物不受此规则限制)
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 全体模式(打开后在系统允许的区域（非安全区）可自由攻击其他所有非安全模式下的用户（和攻击怪物一致）; 等级低于10级的用户不能被攻击，也不能攻击其他用户)
        /// </summary>
        Whole = 1,

        /// <summary>
        /// 帮派模式(对于怪无意义)
        /// </summary>
        Faction = 2,

        /// <summary>
        /// 组队模式(对于怪无意义)
        /// </summary>
        Team = 3,

        /// <summary>
        /// 善恶模式(只可对红名玩家发起PK)
        /// </summary>
        Kind = 4,
    }

    /// <summary>
    /// 群攻的类型
    /// </summary>
    public enum AttackRangeTypes
    {
        /// <summary>
        /// 以某点为中心的圆形攻击
        /// </summary>
        Circle = 0
    }

    // 属性改造 begin [8/15/2013 LiaoWei]

    /// <summary>
    /// 一级属性
    /// </summary>
    public enum UnitPropIndexes
    {
        Strength = 0,       // 力量--物理攻击力 物理防御 魔法技能增幅(新增)
        Intelligence,   // 智力--魔法攻击力 魔法防御 物理技能增幅(新增)
        Dexterity,      // 敏捷--命中       闪避     攻击速度(新增)
        Constitution,   // 体力--生命上限   魔法上限 
        Max,
    }

    /// <summary>
    /// 扩展属性索引值
    /// </summary>
    public enum ExtPropIndexes
    {
        Strong               = 0,       // 耐久
        AttackSpeed          = 1,       // 攻击速度
        MoveSpeed            = 2,       // 移动速度
        MinDefense           = 3,       // 最小物防	
        MaxDefense           = 4,       // 最大物防	
        MinMDefense          = 5,       // 最小魔防	
        MaxMDefense          = 6,       // 最大魔防	
        MinAttack            = 7,       // 最小物攻	
        MaxAttack            = 8,       // 最大物攻	
        MinMAttack           = 9,       // 最小魔攻	
        MaxMAttack           = 10,      // 最大魔攻
        IncreasePhyAttack    = 11,      // 物理攻击提升
        IncreaseMagAttack    = 12,      // 魔法攻击提升
        MaxLifeV             = 13,      // 生命上限	
        MaxLifePercent       = 14,      // 生命上限加成比例(百分比)	
        MaxMagicV            = 15,      // 魔法上限
        MaxMagicPercent      = 16,      // 魔法上限加成比例(百分比)
        Lucky                = 17,      // 幸运
        HitV                 = 18,      // 准确	
        Dodge                = 19,      // 闪避
        LifeRecoverPercent   = 20,      // 生命恢复(百分比)
        MagicRecoverPercent  = 21,      // 魔法恢复(百分比)
        LifeRecover          = 22,      // 单位时间恢复的生命恢复(固定值)
        MagicRecover         = 23,      // 单位时间恢复的魔法恢复(固定值)
        SubAttackInjurePercent= 24,     // 伤害吸收魔法/物理(百分比)
        SubAttackInjure      = 25,      // 伤害吸收魔法/物理(固定值)
        AddAttackInjurePercent= 26,     // 伤害加成魔法/物理(百分比)
        AddAttackInjure      = 27,      // 伤害加成魔法/物理(固定值)
        IgnoreDefensePercent = 28,      // 无视攻击对象的物理/魔法防御(概率)
        DamageThornPercent   = 29,      // 伤害反弹(百分比)
        DamageThorn          = 30,      // 伤害反弹(固定值)
        PhySkillIncreasePercent= 31,    // 物理技能增幅(百分比)
        PhySkillIncrease     = 32,      // 物理技能增幅(固定值)    
        MagicSkillIncreasePercent= 33,  // 魔法技能增幅(百分比)
        MagicSkillIncrease   = 34,      // 魔法技能增幅(固定值)
        FatalAttack          = 35,      // 卓越一击
        DoubleAttack         = 36,      // 双倍一击
        DecreaseInjurePercent = 37,     // 伤害减少百分比(物理、魔法)
        DecreaseInjureValue  = 38,      // 伤害减少数值(物理、魔法)
        CounteractInjurePercent = 39,   // 伤害抵挡百分比(物理、魔法)
        CounteractInjureValue = 40,     // 伤害抵挡数值(物理、魔法)
        IgnoreDefenseRate     = 41,     // 无视防御的比例
        IncreasePhyDefense    = 42,     // 物理防御提升
        IncreaseMagDefense    = 43,     // 魔法防御提升
        LifeSteal = 44,                 // 击中恢复,角色每次成功击中1名敌人时，恢复指定数量的生命值,角色多段攻击时，每段均会触发击中恢复
        AddAttack = 45,                 // 增加物理攻击最小值、物理攻击最大值,增加魔法攻击最小值、魔法攻击最大值
        AddDefense = 46,                // 增加物理防御最小值、物理防御最大值,增加魔法防御最小值、魔法防御最大值

        StateDingSheng,                 // 定身状态加成 ChenXiaojun 如增加其他属性，必须加在前面
        StateMoveSpeed,                 // 速度改变状态 ChenXiaojun 如增加其他属性，必须加在前面
        StateJiTui,                     // 击退状态 ChenXiaojun 如增加其他属性，必须加在前面
        StateHunMi,                     // 昏迷状态 ChenXiaojun 如增加其他属性，必须加在前面
        Max,
        Max_Configed = 47,              // 在物品表的属性中配置的数量最大值
    }

    // 注释掉 重新写
    /*/// <summary>
    /// 25个扩展属性索引值
    /// </summary>
    public enum ExtPropIndexes
    {
        Weight = 0, //重量
        Strong = 1, //耐久	
        MinDefense = 2, //最小物防	
        MaxDefense = 3, //最大物防	
        MinMDefense = 4, //最小魔防	
        MaxMDefense = 5, //最大魔防	
        MinAttack = 6, //最小物攻	
        MaxAttack = 7, //最大物攻	
        MinMAttack = 8, //最小魔攻	
        MaxMAttack = 9, //最大魔攻	
        MinDSAttack = 10, //最小道攻	
        MaxDSAttack = 11, //最大道攻
        MaxLifeV = 12, //生命上限	
        MaxLifePercent = 13, //生命上限加成比例(百分比)	
        MaxMagicV = 14, //魔法上限	
        Lucky = 15, //幸运	
        Curse = 16, //诅咒
        HitV = 17, //准确	
        Dodge = 18, //闪避	
        MagicDodgePercent = 19, //魔法闪避(百分比)	
        PoisoningReoverPercent = 20, //中毒恢复(百分比)	
        PoisoningDodge = 21, //中毒闪避(百分比)	
        LifeRecoverPercent = 22, //生命恢复(百分比)
        MagicRecoverPercent = 23, //魔法恢复(百分比)
        SubAttackInjurePercent = 24, //吸收物理伤害(百分比)
        SubMAttackInjurePercent = 25, //吸收魔法伤害(百分比)
        MaxMagicPercent = 26, //魔法上限加成比例(百分比)	
        IgnoreDefensePercent = 27, //无视攻击对象的物理防御(概率)
        IgnoreMDefensePercent = 28, //无视攻击对象的魔法防御(概率)
        Max, //最大值
    };*/
    
    //注释掉 不用了
    /// <summary>
    /// 3个重量属性索引值
    /// </summary>
    /*public enum WeightIndexes
    {
        HandWeight = 0, //腕力
        BagWeight = 1, //背包负重
        DressWeight = 2, //穿戴负重
        Max, //最大值
    }*/

    // 属性改造 end [8/15/2013 LiaoWei]

    /// <summary>
    /// 任务类型
    /// </summary>
    public enum TaskTypes
    {
        None = -1, //无定义
        Talk = 0, //对话
        KillMonster = 1, //杀怪 
        MonsterSomething = 2, //杀怪拾取
        BuySomething = 3, //购买
        UseSomething = 4, //使用物品
        TransferSomething = 5, //物品传送
        GetSomething = 6, //NPC自动给予物品
        NeedYuanBao = 7, //与NPC对话时判断有无元宝
        CaiJiGoods = 8, //采集物品
        ZhiLiao = 9, //治疗
        FangHuo = 10, //防火
        KillMonsterForLevel = 11, //击杀等级不小于要求等级的怪
    };
    /*
    /// <summary>
    /// 物品种类 10 跟11 是神兵 跟 神甲
    /// </summary>
    public enum ItemCategories
    {
        /// <summary>
        /// 任务道具
        /// </summary>
        ItemTask = 50,
        /// <summary>
        /// 骑宠类
        /// </summary>
        ItemHorsePet = 60,
        /// <summary>
        /// 书籍类
        /// </summary>
        ItemBook = 70,
        /// <summary>
        /// 杂物类
        /// </summary>
        ItemOther = 80,
        /// <summary>
        /// 宝石类
        /// </summary>
        ItemJewel = 90,
        /// <summary>
        /// 卷轴类
        /// </summary>
        ItemMagic = 100,
        /// <summary>
        /// 合成材料类
        /// </summary>
        ItemMakings = 110,
        /// <summary>
        /// 消耗材料类 (包括装备、坐骑等进阶强化等需要消耗的材料)
        /// </summary>
        ItemMaterial = 120,
        /// <summary>
        /// 药品类
        /// </summary>
        ItemDrug = 180,
        /// <summary>
        /// 一次性增加属性 (经验、灵力、货币等直接给角色增加数值)
        /// </summary>
        ItemAddVal = 230,
        /// <summary>
        /// (包括临时加攻、防等道具，以及储备类、双倍经验、灵力类的道具)
        /// </summary>
        ItemBuffer = 250,
        /// <summary>
        /// 荣耀护体
        /// </summary>
        RongYuHuTi = 255,
        /// <summary>
        /// 战魂护体
        /// </summary>
        ZhanHunHuTi = 256,
        /// <summary>
        /// 帮旗护体
        /// </summary>
        BangQiHuTi = 258,
        /// <summary>
        /// <summary>
        /// 普通包裹
        /// </summary>
        ItemNormalPack = 301,
        /// <summary>
        /// 升级包裹
        /// </summary>
        ItemUpPack = 302,
        /// <summary>
        /// 银两包
        /// </summary>
        YinLiangPack = 401,
        /// <summary>
        /// 铜钱包
        /// </summary>
        MoneyPack = 501,
        /// <summary>
        /// 绑定铜钱符
        /// </summary>
        BindMoneyFu = 601,

        /// <summary>
        /// 武器
        /// </summary>
        Weapon = 0,
        /// <summary>
        /// 衣服
        /// </summary>
        Clothes = 1,
        /// <summary>
        /// 腰带
        /// </summary>
        Belt = 2,
        /// <summary>
        /// 手镯
        /// </summary>
        Bracelet = 3,
        /// <summary>
        /// 头饰
        /// </summary>
        Headdress = 4,
        /// <summary>
        /// <summary>
        /// 戒指
        /// </summary>
        Ring = 5,
        /// <summary>
        /// 项链
        /// </summary>
        Necklace = 6,
        /// <summary>
        /// 护符
        /// </summary>
        Decorate = 7,
        /// <summary>
        /// 时装
        /// </summary>
        FashionCothes = 8,
        /// <summary>
        /// 炫武
        /// </summary>
        FashionWeapon = 9,
        /// <summary>
        /// 神兵
        /// </summary>
        ShenBing = 10,
        /// <summary>
        /// 神甲
        /// </summary>
        ShenJia = 11,
        /// <summary>
        /// 神器
        /// </summary>
        Pet = 12,
        /// <summary>
        /// 靴子
        /// </summary>
        Boot = 13,
        /// <summary>
        /// 护肩
        /// </summary>
        ShoulderProtection = 14,
        /// <summary>
        /// 手套
        /// </summary>
        Glove = 15,
        /// <summary>
        /// 徽章
        /// </summary>
        ChestProtection = 16,
        /// <summary>
        /// 装备最大值
        /// </summary>
        EquipMax = 17,
    }*/

    /// <summary>
    /// 新的MU物品种类
    /// </summary>
    public enum ItemCategories
    {
        /// <summary>
        /// 任务道具
        /// </summary>
        ItemTask = 50,
        /// <summary>
        /// 骑宠类
        /// </summary>
        ItemHorsePet = 60,
        /// <summary>
        /// 书籍类
        /// </summary>
        ItemBook = 70,
        /// <summary>
        /// 杂物类
        /// </summary>
        ItemOther = 80,
        /// <summary>
        /// 宝石类
        /// </summary>
        ItemJewel = 90,
        /// <summary>
        /// 卷轴类
        /// </summary>
        ItemMagic = 100,
        /// <summary>
        /// 合成材料类
        /// </summary>
        ItemMakings = 110,
        /// <summary>
        /// 消耗材料类 (包括装备、坐骑等进阶强化等需要消耗的材料)
        /// </summary>
        ItemMaterial = 120,
        /// <summary>
        /// 药品类
        /// </summary>
        ItemDrug = 180,
        /// <summary>
        /// 一次性增加属性 (经验、灵力、货币等直接给角色增加数值)
        /// </summary>
        ItemAddVal = 230,
        /// <summary>
        /// (包括临时加攻、防等道具，以及储备类、双倍经验、灵力类的道具)
        /// </summary>
        ItemBuffer = 250,
        /// <summary>
        /// 经脉类物品
        /// </summary>
        JingMai = 251,
        /// <summary>
        /// 武学类物品
        /// </summary>
        WuXue = 252,
        /// <summary>
        /// 成就类物品
        /// </summary>
        ChengJiu = 253,
        /// <summary>
        /// 荣耀护体
        /// </summary>
        RongYuHuTi = 255,
        /// <summary>
        /// 战魂护体
        /// </summary>
        ZhanHunHuTi = 256,
        /// <summary>
        /// 帮旗护体
        /// </summary>
        BangQiHuTi = 258,
        /// <summary>
        /// <summary>
        /// 普通包裹
        /// </summary>
        ItemNormalPack = 301,
        /// <summary>
        /// 升级包裹
        /// </summary>
        ItemUpPack = 302,
        /// <summary>
        /// 银两包
        /// </summary>
        YinLiangPack = 401,
        /// <summary>
        /// 铜钱包
        /// </summary>
        MoneyPack = 501,
        /// <summary>
        /// 绑定铜钱符
        /// </summary>
        BindMoneyFu = 601,
        /// <summary>
        /// 宝箱类型    --  点击后 掉落物品在地上 [1/7/2014 LiaoWei]
        /// </summary>
        TreasureBox = 701,

        //新的命名故意使用拼音命名，来避免和以前的混淆，防止修改时无法发现出错

        /// <summary>
        /// 头盔
        /// </summary>
        TouKui = 0,
        /// <summary>
        /// 铠甲
        /// </summary>
        KaiJia = 1,
        /// <summary>
        /// 护手
        /// </summary>
        HuShou = 2,
        /// <summary>
        /// 护腿
        /// </summary>
        HuTui = 3,
        /// <summary>
        /// 靴子
        /// </summary>
        XueZi = 4,
        /// <summary>
        /// <summary>
        /// 项链
        /// </summary>
        XiangLian = 5,
        /// <summary>
        /// 戒指
        /// </summary>
        JieZhi = 6,
        /// <summary>
        /// 坐骑
        /// </summary>
        ZuoJi = 7,
        /// <summary>
        /// 翅膀
        /// </summary>
        ChiBang = 8,
        /// <summary>
        /// 守护宠物
        /// </summary>
        ShouHuChong = 9,
        /// <summary>
        /// 跟随宠物
        /// </summary>
        ChongWu = 10,
        /// <summary>
        /// 武器-剑
        /// </summary>
        WuQi_Jian = 11,
        /// <summary>
        /// 武器-斧
        /// </summary>
        WuQi_Fu = 12,
        /// <summary>
        /// 槌
        /// </summary>
        WuQi_Chui = 13,
        /// <summary>
        /// 弓
        /// </summary>
        WuQi_Gong = 14,
        /// <summary>
        /// 弩
        /// </summary>
        WuQi_Nu = 15,
        /// <summary>
        /// 矛
        /// </summary>
        WuQi_Mao = 16,
        /// <summary>
        /// 杖
        /// </summary>
        WuQi_Zhang = 17,
        /// <summary>
        /// 盾
        /// </summary>
        WuQi_Dun = 18,
        /// <summary>
        /// 刀
        /// </summary>
        WuQi_Dao = 19,
        /// <summary>
        /// 弓箭筒
        /// </summary>
        WuQi_GongJianTong = 20,
        /// <summary>
        /// 弩箭筒
        /// </summary>
        WuQi_NuJianTong = 21,
        /// <summary>
        /// 护身符
        /// </summary>
        HuFu = 22,
        /// <summary>
        /// 装备最大值
        /// </summary>
        EquipMax = 23,

        /// <summary>
        /// 特殊的不能装备的元素之心
        /// </summary>
        ElementHrtBegin = 800,
        SpecialElementHrt = 810,
        ElementHrtEnd = 811,
    }

    public enum TaskClasses
    {
        MainTask = 0,       //主线
        SpecialTask = 2,        //也是一种循环任务？
        CircleTaskStart = 3, //环式任务开始
        LieshaTask = 3,     //猎杀日常
        WuxueTask = 4,      //武学日常
        JungongTask = 5,    //军功日常
        MozuTask = 6,       //魔族帮会
        BanghuiTask = 7,        //帮会任务
        DailyTask = 8,      //日常任务
        TaofaTask = 9,      //讨伐任务
        CircleTaskEnd = 9,   //环式任务结束
    }

    /// <summary>
    /// NPC的任务状态类型
    /// </summary>
    public enum NPCTaskStates
    {
        NONE = 0, //无
        NEWTASK = 1, //有新任务 
        DOINGTASK = 2, //有未完成任务
        OKTASK = 3, //有完成任务
    };

    /// <summary>
    /// 聊天频道索引
    /// </summary>
    public enum ChatChannelIndexes
    {
        All = 0, //综合
        World = 1, //世界 
        Faction = 2, //帮派
        Team = 3, //组队
        Private = 4, //私聊
        System = 5, //系统
        Max = 6, //最大值
    };

    /// <summary>
    /// 聊天消息类型索引
    /// </summary>
    public enum ChatTypeIndexes
    {
        System = 0, //系统
        Map = 1, //附近
        World = 2, //世界 
        Faction = 3, //帮派
        Team = 4, //组队
        Private = 5, //私聊
        Bulletin = 6, //公告
        Max = 7, //最大值
    };

    /// <summary>
    /// 游戏提示信息类型
    /// </summary>
    public enum GameInfoTypeIndexes
    {
        Normal = 0, //一般提示
        Error = 1, //错误
        Hot = 2, //重点提示
        Max = 3, //最大
    };

    /// <summary>
    /// 物品品质
    /// </summary>
    public enum GoodsQuality
    {
        White = 0,
        Green = 1,
        Blue = 2,
        Purple = 3,
        Gold = 4,
        Max = 5
    }

    /// <summary>
    /// 用户背包一页的数量
    /// </summary>
    public enum RoleBagNumPerPage
    {
        PageNum = 50,
    }

    /// <summary>
    /// 修改物品的操作类型
    /// </summary>
    public enum ModGoodsTypes
    {
        Abandon = 0, //丢弃
        EquipLoad = 1, //添加装备
        EquipUnload = 2, //卸载装备
        ModValue = 3, //修改数值
        Destroy = 4, //摧毁物品,用户什么也得不到
        SaleToNpc = 5,//出售物品给npc,绑定物品得到绑定铜钱，非绑定物品得到铜钱
    }

    /// <summary>
    /// 角色间交易的指令类型定义
    /// </summary>
    public enum GoodsExchangeCmds
    {
        None = 0,
        Request, //请求交易
        Refuse, //拒绝交易
        Agree, //同意交易
        Cancel, //取消交易
        AddGoods, //添加物品
        RemoveGoods, //删除物品
        UpdateMoney, //修改金币
        UpdateYuanBao, //修改元宝
        Lock,  //锁定交易
        Unlock, //停止交易
        Done,  //完成交易
    }

    /// <summary>
    /// 角色摆摊/购买的指令类型定义
    /// </summary>
    public enum GoodsStallCmds
    {
        None = 0,
        Request, //请求摆摊
        Start, //开始摆摊
        Cancel, //取消摆摊
        AddGoods, //添加物品
        RemoveGoods, //删除物品
        UpdateMessage, //修改留言
        ShowStall, //显示摊位
        BuyGoods,  //购买物品
    }

    /// <summary>
    /// 角色组队指令类型定义
    /// </summary>
    public enum TeamCmds
    {
        None = 0,
        Create, //创建队伍
        Destroy, //解散队伍
        Invite, //邀请组队
        Apply, //申请组队
        Refuse, //拒绝申请/邀请
        AgreeInvite, //同意邀请
        AgreeApply, //同意申请
        Remove, //踢出队伍
        Quit, //离开组队
        AppointLeader, //任命队长
        GetThingOpt, //修改自由拾取选
        Ready, //准备
        QuickJoinTeam, //快速组队
        Start, //开始(仅用于勾选人满自动开始时,服务器通知队长开始)
    }

    /// <summary>
    /// 角色大乱斗指令类型定义
    /// </summary>
    public enum BattleCmds
    {
        None = 0,
        Invite, //邀请参加
        Time, //计时通知
        Refuse, //拒绝参加
        Enter, //同意进入
        Leave, //主动离开
    }

    /// <summary>
    /// 角色自动挂机战斗指令类型定义
    /// </summary>
    public enum AutoFightCmds
    {
        None = 0,
        Start, //开始战斗
        Update, //计时通知
        End, //结束战斗
    }

    /// <summary>
    /// 角色骑乘指令类型定义
    /// </summary>
    public enum HorseCmds
    {
        None = 0,
        On, //乘上坐骑
        Off, //离开坐骑
    }

    /// <summary>
    /// 角色宠物指令类型定义
    /// </summary>
    public enum PetCmds
    {
        None = 0,
        Show, //放出宠物
        Hide, //收回宠物
        Rename, //宠物改名
    }

    /// <summary>
    /// 角色点将台指令类型定义
    /// </summary>
    public enum DianJiangCmds
    {
        None = 0,
        CreateRoom, //创建房间
        RemoveRoom, //删除房间
        EnterRoom, //进入房间
        LeaveRoom, //离开房间
        KickRole, //踢人出房间
        ChangeTeam, //切换队伍
        ChangeState, //准备状态
        ViewFight, //观看战斗
        ToViewer, //转成观看模式
        ToLeave, //离开战斗场景
    }

    /// <summary>
    /// 坐骑的扩展属性索引定义
    /// </summary>
    public enum HorseExtIndexes
    {
        /// <summary>
        /// 物理攻击
        /// </summary>
        Attack = 0,

        /// <summary>
        /// 物理防御
        /// </summary>
        Defense = 1,

        /// <summary>
        /// 魔法攻击
        /// </summary>
        MAttack = 2,

        /// <summary>
        /// 魔法防御
        /// </summary>
        MDefense = 3,

        /// <summary>
        /// 暴击
        /// </summary>
        Burst = 4,

        /// <summary>
        /// 命中
        /// </summary>
        Hit = 5,

        /// <summary>
        /// 闪避
        /// </summary>
        Dodge = 6,

        /// <summary>
        /// 生命值上限的次数
        /// </summary>
        MaxLife = 7,

        /// <summary>
        /// 魔法值上限的次数
        /// </summary>   
        MaxMagic = 8,

        /// <summary>
        /// 暴抗上限的次数
        /// </summary>   
        BurstPercent = 9,

        /// <summary>
        /// 最大值
        /// </summary>
        MaxVal,
    }

    /// <summary>
    /// 修改宠物的操作定义
    /// </summary>
    public enum ModPetCmds
    {
        Rename = 0, //改名
        Remove = 1, //撵走
        Feed = 2, //喂养
        Realive = 3, //复活
        UpLevel = 4, //升级到高级
        GetThing = 5, //修改拾取项
    }

    /// <summary>
    /// 地图类型定义
    /// </summary>
    public enum MapTypes
    {
        Normal = 0, //常规地图
        NormalCopy = 1, //普通副本
        DianJiangCopy = 2, //点将台副本
        CaiShenMiaoCopy = 3, //福神庙副本
        TaskCopy = 4, //任务剧情副本
        JingJiChang = 5,//竞技场副本
        Max, //最大值
    }

    /// <summary>
    /// 点将台战斗状态定义
    /// </summary>
    public enum DJFightStates
    {
        NoFight = 0, //无战斗
        WaitingFight = 1, //等待战斗倒计时(此时伤害无效)
        StartFight = 2, //开始战斗(倒计时中)
        EndFight = 3, //结束战斗(此时伤害无效)
        ClearRoom = 4, //清空战斗场景
    };

    /// <summary>
    /// 经脉类型定义
    /// </summary>
    public enum JingMaiTypes
    {
        Yangwei = 0,
        Yinwei = 1,
        Yangqiao = 2,
        Yinqiao = 3,
        Daimai = 4,
        Congmai = 5,
        Renmai = 6,
        Dumai = 7,
        Max = 8,
    };

    /// <summary>
    /// 游戏提示信息显示类型
    /// </summary>
    public enum ShowGameInfoTypes
    {
        None = 0, //不显示
        OnlySysHint = 1, //系统提示位置显示
        OnlyBox = 2, //窗口位置显示
        OnlyErr = 3, //仅仅错误提示
        ErrAndBox = 4, //错误提示和窗口位置显示
        HintAndBox = 5, //系统提示和窗口位置显示
        LittleSysHint = 6, //系统提示位置显示小字体
        SysHintAndChatBox = 7, //系统提示和聊天窗口
        OnlyChatBox = 8, //聊天窗口
    };

    /// <summary>
    /// 防止沉迷的时间类型
    /// </summary>
    public enum AntiAddictionTimeTypes
    {
        None = 0, //非沉迷时间类型
        ThreeHours = 1, //3小时时间类型
        FiveHoures = 2, //超过5个小时的深度沉迷类型
    };

    /// <summary>
    /// 宝石镶嵌的操作类型
    /// </summary>
    public enum EnchaseJewelTypes
    {
        Enchase = 0, //镶嵌
        UnEnchase = 1, //取出
    };

    /// <summary>
    /// 返回出售中的物品列表的最大数
    /// </summary>
    public enum SaleGoodsConsts
    {
        /// <summary>
        /// 出售中的物品的ID
        /// </summary>
        SaleGoodsID = -1,

        /// <summary>
        /// 随身仓库中的物品ID
        /// </summary>
        PortableGoodsID = -1000,

        /// <summary>
        /// 同时出售的物品数量
        /// </summary>
        MaxSaleNum = 16,

        /// <summary>
        /// 返回列表的最大数量
        /// </summary>
        MaxReturnNum = 250,

        /// <summary>
        /// 金蛋仓库位置【0表示背包，-1000表示随身仓库，这个值2000表示砸金蛋的仓库】
        /// </summary>
        JinDanGoodsID = 2000,

        /// <summary>
        /// 元素之心背包
        /// </summary>
        ElementhrtsGoodsID = 3000,

        /// <summary>
        /// 元素之心装备栏
        /// </summary>
        UsingElementhrtsGoodsID = 3001,

        /// <summary>
        /// 特殊的摆摊金币物品ID
        /// </summary>
        BaiTanJinBiGoodsID = 50200,
    }

    /// <summary>
    /// 交易市场搜索类型
    /// </summary>
    public enum MarketSearchTypes
    {
        SearchAll = 0,
        SearchRoleName = 1,
        SearchGoodsIDs = 2,
        TypeAndFilterOpts = 3,
    };

    /// <summary>
    /// 灵力的常量
    /// </summary>
    public enum LingLiConsts
    {
        MaxLingLiVal = 30000,
    }

    /// <summary>
    /// Buffer项的类型
    /// </summary>
    public enum BufferItemTypes
    {
        None = 0, //无定义
        DblExperience = 1, //双倍经验卡
        DblMoney = 2, //双倍金钱卡
        DblLingLi = 3, //双倍灵力卡
        LifeVReserve = 4, //生命储备
        MagicVReserve = 5, //魔法储备
        AddTempAttack = 6, //狂攻符咒
        AddTempDefense = 7, //防御符咒
        UpLifeLimit = 8, //生命符咒
        UpMagicLimit = 9, //精气符咒
        LingLiVReserve = 10, //灵力储备
        AntiBoss = 11, //BOSS克星
        AntiRole = 12, //决斗长buffer
        MonthVIP = 13, //VIP月卡
        SheLiZhiYuan = 14, //舍利之源
        DiWanZhiYou = 15, //帝王之佑
        JunQi = 16, //帮旗加成
        DblSkillUp = 17, //双倍技能卡
        ThreeExperience = 18, //三倍经验卡
        ThreeMoney = 19, //三倍金钱卡
        AutoFightingProtect = 20, //挂机保护卡
        TimeExp = 21, //持续给经验
        TimeAddDefense = 22, //持续增加物理防御力
        TimeAddMDefense = 23, //持续增加魔法防御力
        TimeAddAttack = 24, //持续增加物理攻击力
        TimeAddMAttack = 25, //持续增加魔法攻击力
        TimeAddDSAttack = 26, //持续增加道术攻击力
        TimeAddLifeMagic = 27, //持续增加生命值和魔法值
        WaWaExp = 28, //替身娃娃经验buffer
        ZhuFu = 29, //祝福Buffer
        FallTianSheng = 30, //掉落天生装备的Buffer
        ChengJiu = 31, //成就Buffer
        JingMai = 32, //经脉Buffer
        WuXue = 33, //武学Buffer
        GuMuTimeLimit = 34, //古墓限时Buffer 古墓密令 和 角色第一次登录后得到
        MingJieMapLimit = 35, //冥界限时地图Buffer 冥界令牌使用后得到
        FiveExperience = 36, //五倍经验卡
        TimeAddLifeNoShow = 37, //持续增加生命值(客户端不显示)
        TimeAddMagicNoShow = 38, //持续增加魔法值(客户端不显示)
        PKKingBuffer = 39, //角斗场 武林争霸 [现在叫决战地府]  pk 王 buffer
        DSTimeAddLifeNoShow = 40, //持续增加生命值(客户端不显示)(道士加血使用)
        DSTimeHideNoShow = 41, //隐身(客户端不显示)(道士给其他角色使用)
        DSTimeShiDuNoShow = 42, //道士施放毒(客户端不显示)(道士给其他角色使用)
        DSTimeAddDefenseNoShow = 43, //道士加防(客户端不显示)(道士给其他角色使用)
        DSTimeAddMDefenseNoShow = 44, //道士加攻毒(客户端不显示)(道士给其他角色使用)
        FSAddHuDunNoShow = 45, //法师加护盾(客户端不显示)
        MutilExperience = 46, //多倍经验卡
        JieRiChengHao = 47, //节日称号buffer
        ErGuoTou = 48, //二锅头酒的buffer
        ZuanHuang = 49, //钻皇Buffer
        ZhanHun = 50, //战魂Buffer
        RongYu = 51, //荣誉Buffer
        // 属性改造 begin [8/15/2013 LiaoWei]
        ADDTEMPStrength = 52,      //增加角色力量值 (值,持续时间)
        ADDTEMPIntelligsence = 53, //增加角色智力值 (值,持续时间)
        ADDTEMPDexterity = 54,     //增加角色敏捷值 (值,持续时间)
        ADDTEMPConstitution = 55,  //增加角色体力值 (值,持续时间)
        ADDTEMPATTACKSPEED = 56,   //持续一段时间内增加角色攻击速度值 (值, 持续时间)
        ADDTEMPLUCKYATTACK = 57,   //持续一段时间内增加角色幸运一击概率值 (值, 持续时间)
        ADDTEMPFATALATTACK = 58,   //持续一段时间内增加角色卓越一击概率值 (值, 持续时间)
        ADDTEMPDOUBLEATTACK = 59,   //持续一段时间内增加角色双倍一击概率值 (值, 持续时间)
        // 属性改造 end [8/15/2013 LiaoWei]

        // MU项目BUFF   begin [10/24/2013 LiaoWei]
        MU_SUBDAMAGEPERCENTTIMER        = 60,   // 一段时间内减少伤害百分比 (时间，百分比)
        MU_MAXLIFEPERCENT               = 61,   // 一段时间增加百分比的生命值 (时间，百分比，每级值增加的步长)
        MU_ADDDEFENSETIMER              = 62,   // 一段时间增加物理和魔法防御力(时间，数值，每级值增加的步长)
        MU_ADDATTACKTIMER               = 63,   // 一段时间增加物理和魔法攻击力(时间，数值，每级值增加的步长)
        MU_ADDLUCKYATTACKPERCENTTIMER   = 64,   // 一段时间提升百分比幸运一击效果(百分比，时间)
        MU_ADDFATALATTACKPERCENTTIMER   = 65,   // 一段时间提升百分比卓越一击效果(百分比，时间)
        MU_ADDDOUBLEATTACKPERCENTTIMER  = 66,   // 一段时间提升百分比双倍一击效果(百分比，时间)
        MU_ADDMAXHPVALUE                = 67,   // 一段时间提升HP上限(值，时间)
        MU_ADDMAXMPVALUE                = 68,   // 一段时间提升MP上限(值，时间)
        MU_ADDLIFERECOVERPERCENT        = 69,   // 一段时间提升生命恢复百分比(值，时间)
        MU_FRESHPLAYERBUFF              = 70,   // 新玩家BUFF
        MU_SUBDAMAGEPERCENTTIMER1       = 71,   // 一段时间内减少伤害百分比 (时间，百分比) -- 由于客户端显示需要不同的ID 所以 再声明一个ID
        
        // 3期新增 Begin [3/14/2014 LiaoWei]
        MU_SUBATTACKPERCENTTIMER        = 72,   // 一段时间内减小攻击力百分比 (时间, 百分比)  袭风刺
        MU_ADDATTACKPERCENTTIMER        = 73,   // 一段时间内增加攻击力百分比 (时间, 百分比)

        MU_SUBATTACKVALUETIMER          = 74,   // 一段时间内减小攻击力值 (时间, 值)
        MU_ADDATTACKVALUETIMER          = 75,   // 一段时间内增加攻击力值 (时间, 值)

        MU_SUBDEFENSEPERCENTTIMER       = 76,   // 一段时间内减少防御百分比(时间,百分比)
        MU_ADDDEFENSEPERCENTTIMER       = 77,   // 一段时间内增加防御百分比(时间,百分比)

        MU_SUBDEFENSEVALUETIMER         = 78,   // 一段时间内减少防御值(时间,值)
        MU_ADDDEFENSEVALUETIMER         = 79,   // 一段时间内增加防御值(时间,值)
        
        MU_SUBMOVESPEEDPERCENTTIMER     = 80,   // 一段时间内降低移动速度 (时间, 百分比)
        MU_ADDMAXLIFEPERCENTANDVALUE    = 81,   // 一段时间内增加生命上限百分比和值(时间, 百分比, 值) 生命之光

        MU_SUBHITPERCENTTIMER           = 82,   // 一段时间内降低命中 (时间)
        MU_SUBDAMAGEPERCENTVALUETIMER   = 83,   // 一段时间内减少伤害百分比和值 (时间,百分比,值) 守护之魂
        MU_ADDATTACKANDDEFENSEEPERCENTVALUETIMER = 84,   // 一段时间内增加攻击和防御的百分比和值 (时间,百分比,值)  战神之力

        MU_ANGELTEMPLEBUFF1 = 85,   // 天使神殿BUFF1
        MU_ANGELTEMPLEBUFF2 = 86,   // 天使神殿BUFF2
        MU_JINGJICHANG_JUNXIAN = 87,   //竞技场军衔Buff
        // 3期新增 Begin [3/14/2014 LiaoWei]

        // MU项目BUFF   end[10/24/2013 LiaoWei]

        MU_ZHANMENGBUILD_ZHANQI = 88,   //战盟建筑buffer，战旗
        MU_ZHANMENGBUILD_JITAN = 89,   //战盟建筑buffer，祭坛
        MU_ZHANMENGBUILD_JUNXIE = 90,   //战盟建筑buffer，军械
        MU_ZHANMENGBUILD_GUANGHUAN = 91,   //战盟建筑buffer，光环

        MU_REDNAME_DEBUFF = 92,       // 红名惩罚DEBUFF [4/21/2014 LiaoWei]

        TimeFEIXUENoShow = 93, //腐蚀沸血
        TimeZHONGDUNoShow = 94, //毒爆术
        TimeLINGHUNoShow = 95, //灵魂奔腾
        TimeRANSHAONoShow = 96, //生命燃烧
        TimeHUZHAONoShow = 97, //重生
        TimeWUDIHUZHAONoShow = 98, //无敌

        MU_WORLDLEVEL = 99, //世界等级
        MU_SPECMACH_EXP = 100, // 特殊设备提升经验的buff

        MaxVal, //最大值
    }

    /// <summary>
    /// 怪物的类型
    /// </summary>
    public enum MonsterTypes
    {
        None = 0, //无定义
        Noraml = 101, //普通怪
        Task = 201, //剧情怪
        Rarity = 301, //精英怪
        Boss = 401, //Boss
        DaDao = 501, //通天大盗
        QiBao = 601, //夺宝奇兵
        NoAttack = 701, //被打不还击的怪
        BiaoChe = 801, //镖车怪
        ShengXiaoYunCheng = 901, //生肖运程怪
        DSPetMonster = 1001,//召唤怪2 逍遥召唤 骷髅 或者神兽 同时刻只能有一个 新召唤一个，就杀死旧的
        Reserve1 = 1101, //备用1
        CityGuard = 1201,//城池守卫
        XianFactionGuard = 1301,//仙阵营守卫
        MoFactionGuard = 1302,//魔阵营守卫
        CivilianMonster = 1401,//平民怪，不打人的正常怪
        JUSTMOVE = 1501, // 不攻击玩家 只走路 [6/11/2014 LiaoWei]
        CaiJi = 1601,   //采集物
        BloodCastleGateAndCrystal = 1701, // 血色堡垒大门和水晶棺 [11/12/2013 LiaoWei]
        JingJiChangRobot = 1801,//竞技场机器人
    }

    /// <summary>
    /// 好友相关常量定义
    /// </summary>
    public enum FriendsConsts
    {
        /// <summary>
        /// 好友上限
        /// </summary>
        MaxFriendsNum = 50,

        /// <summary>
        /// 黑名单上限
        /// </summary>
        MaxBlackListNum = 20,

        /// <summary>
        /// 仇人
        /// </summary>
        MaxEnemiesNum = 20,
    }

    /// <summary>
    /// 搜索的返回结果常量定义
    /// </summary>
    public enum SearchResultConsts
    {
        /// <summary>
        /// 搜索角色的返回个数
        /// </summary>
        MaxSearchRolesNum = 10,

        /// <summary>
        /// 搜索队伍的返回个数
        /// </summary>
        MaxSearchTeamsNum = 10,
    };

    /// <summary>
    /// 搜索的返回结果常量定义
    /// </summary>
    public enum FilterNPCScriptIDs
    {
        /// <summary>
        /// 进入副本下一层的ID
        /// </summary>
        EnterNextMap = 1,

        /// <summary>
        /// 领取副本奖励
        /// </summary>
        GetMapAward = 2,

        /// <summary>
        /// 领取五行奇珍的奖励
        /// </summary>
        GetWuXingAward = 3,

        /// <summary>
        /// 出狱
        /// </summary>
        LeaveLaoFang = 4,

        /// <summary>
        /// 提取舍利之源
        /// </summary>
        TakeSheLiZhiYuan = 5,
    };

    /// <summary>
    /// 提示信息错误码的类型
    /// </summary>
    public enum HintErrCodeTypes
    {
        None = 0, //无定义
        NoBagGrid = 1, //背包中无空格
        NoChongXueNum = 2, //提示无冲穴次数
        NoLingLi = 3, //提示无灵力
        NoLingFu = 4, //提示无灵符
        NoYuanBao = 5, //提示无灵符
        CangBaoGeStart = 6, //藏宝阁副本开放了
        YaBiaoStart = 7, //押镖活动开始了
        NoXiaoLaBa = 8, //提示无小喇叭
        NoYaBiaoLing = 9, //提示无押镖令牌
        WuXingStart = 10, //五行奇阵活动开始了
        DblExpAndLingLiStart = 11, //双倍经验和灵力时间开始
        LingDiZhanStart = 12, //领地战开始了
        HuangChengZhanStart = 13, //皇城战开始了
        AddShenFenZheng = 14, //完善身份信息
        EnterAFProtect = 15, //进入自动战斗保护状态
        HintReloadXML = 16, //提示重新加载xml
        ForceShenFenZheng = 17, //强制完善身份信息
		HorseLuckyFull = 18, //坐骑幸运值已经满
		ToBuyGoodsID = 19, //要购买的物品ID
        ToBuyFromQiZhenGe = 20, //从奇珍阁购买的物品ID
        ToBuyFromMarket = 21, //从交易市场购买的物品ID
        ToBuyFromYinLiang = 22, //从银两商购买的物品ID
        ToBuyMagicDrugs = 23, //从药店购买蓝药
        NoShiLianLing = 24, //提示无试炼令
        NoChuanSongJuan = 25, //提示无传送卷
        ToVip = 26, //成为vip
        NoTongQian = 27, //没有铜钱
        CallAutoSkill = 28, //自动使用技能
        NoBindZuanShi = 29, //绑定钻石不足
        NoZuanShi = 30, //钻石不足
        NoYuMao = 31, //羽毛不足
        NoShenYingHuoZhong = 32, //神鹰火种不足
        NoZhuFuJingShi = 33, //祝福晶石不足
        NoLingHunJingShi = 34, //灵魂晶石不足
        NoMaYaJingShi = 35, //玛雅晶石不足
        NoShengMingJingShi = 36, //生命晶石不足
        NoChuangZaoJingShi = 37, //创造晶石不足
        NoShenYouJingShi = 38, //神佑晶石不足
        LevelNotEnough = 39, //等级不足
        LevelOutOfRange = 40, //等级超出范围(也可能是不足)
        NeedZhuanSheng = 41, //等级达到转生条件,需要转生
        VIPNotEnough = 41, //VIP等级不足
        NeedUpdateApp = 42, //需要更新App
        NeedUpdateRes = 43, //需要更新Res

        TeamChatTypeIndex = 200, //组队聊天信息,非错误

        MaxVal, //最大值
    }

    /// <summary>
    /// 用户行为消息类型
    /// </summary>
    public enum RoleActionsMsgTypes
    {
        None = 0, //无定义
        Bulletin = 1, //公告
        HintMsg = 2, //提示信息
    }

    /// <summary>
    /// NPC功能ID过滤定义
    /// </summary>
    public enum FilterNPCOperationIDs
    {
        /// <summary>
        /// 接受运镖
        /// </summary>
        StartYaBiao = 1,

        /// <summary>
        ///     
        /// </summary>
        EndYaBiao = 2,

        /// <summary>
        /// 运镖时提取货物
        /// </summary>
        YaBiaoTakeGoods = 3,
    };

    /// <summary>
    /// 特殊时间表常量
    /// </summary>
    public enum SpecialTimeIDs
    {
        /// <summary>
        /// 打坐时经验和灵力翻倍
        /// </summary>
        DoubleExpAndLingLi = 1,

        /// <summary>
        /// 烤火双倍时间
        /// </summary>
        KaoHuo = 2,
    };

    /// <summary>
    /// 排队命令ID
    /// </summary>
    public enum QueueCmdIDs
    {
        /// <summary>
        /// 执行经脉完成操作
        /// </summary>
        NotifyEndJingMai = 1,
    };

    /// <summary>
    /// 添加的帮贡类型
    /// </summary>
    public enum AddBangGongTypes
    {
        None        = 0, //无定义
        BGGold      = 1, //贡献铜钱获取的帮贡
        BGGoods     = 2, //BGGoods = 2, //贡献道具获取的帮贡
    };

    /// <summary>
    /// 领地战的状态类型
    /// </summary>
    public enum LingDiZhanStates
    {
        None = 0, //无定义
        Fighting = 1, //战斗中
    };

    /// <summary>
    /// 皇城战的状态类型
    /// </summary>
    public enum HuangChengZhanStates
    {
        None = 0, //无定义
        Fighting = 1, //战斗中
    };

    /// <summary>
    /// 王城战的状态类型
    /// </summary>
    public enum WangChengZhanStates
    {
        None = 0, //无定义
        Fighting = 1, //战斗中
    };

    /// <summary>
    /// 领地的ID定义
    /// </summary>
    public enum LingDiIDs
    {
        /// <summary>
        /// 扬州城
        /// </summary>
        YanZhou = 1,

        /// <summary>
        /// 皇城
        /// </summary>
        HuangCheng = 2,

        /// <summary>
        /// 幽州城
        /// </summary>
        YouZhou = 3,

        /// <summary>
        /// 太原城
        /// </summary>
        TaiYuan = 4,

        /// <summary>
        /// 荥阳城
        /// </summary>
        XingYang = 5,

        /// <summary>
        /// 皇宫地图
        /// </summary>
        HuangGong = 6,

        /// <summary>
        /// 最大值
        /// </summary>
        MaxVal = 7,
    };

    /// <summary>
    /// 隐藏属性Buffer的ID定义(对应任务表中的YinCangID)
    /// </summary>
    public enum YinCangIDs
    {
        /// <summary>
        /// 全紫
        /// </summary>
        QuanZi = 1,

        /// <summary>
        /// 全金
        /// </summary>
        QuanJin = 2,

        /// <summary>
        /// 满星5
        /// </summary>
        ManXing5 = 5,

        /// <summary>
        /// 满星7
        /// </summary>
        ManXing7 = 7,

        /// <summary>
        /// 满星10
        /// </summary>
        ManXing10 = 10,

        /// <summary>
        /// 满星13
        /// </summary>
        ManXing13 = 13,

        /// <summary>
        /// 满星15
        /// </summary>
        ManXing15 = 15,

        /// <summary>
        /// 宝石4
        /// </summary>
        BaoShi4 = 104,

        /// <summary>
        /// 宝石5
        /// </summary>
        BaoShi5 = 105,

        /// <summary>
        /// 宝石6
        /// </summary>
        BaoShi6 = 106,

        /// <summary>
        /// 宝石7
        /// </summary>
        BaoShi7 = 107,

        /// <summary>
        /// 最大值
        /// </summary>
        MaxVal = 108,
    };

    /// <summary>
    /// 大乱斗中的阵营ID类型
    /// </summary>
    public enum BattleWhichSides
    {
        Xian = 1, //仙帝阵营
        Mo = 2, //魔帝阵营
    };

    /// <summary>
    /// 运行状态定义
    /// </summary>
    enum BattleStates
    {
        NoBattle = 0, //无战斗
        PublishMsg = 1, //广播大乱斗的消息给在线用户
        WaitingFight = 2, //等待战斗倒计时(此时禁止新用户进入, 此时伤害无效)
        StartFight = 3, //开始战斗(倒计时中)
        EndFight = 4, //结束战斗(此时伤害无效)
        ClearBattle = 5, //清空战斗场景
    };

    /// <summary>
    /// 掉落算法
    /// </summary>
    enum FallAlgorithm
    {
        MonsterFall = 0, //怪物掉落
        BaoXiang = 1, //宝箱
    };

    /// <summary>
    /// 活动类型
    /// </summary>
    public enum ActivityTypes
    {
        None = 0, //无定义
        InputFirst = 1, //首充大礼
        InputFanLi = 2, //充值返利
        InputJiaSong = 3, //充值加送
        InputKing = 4, //充值王 
        LevelKing = 5, //冲级王
        EquipKing = 6, //装备王====>修改成boss王 
        HorseKing = 7, //坐骑王====>修改成武学王
        JingMaiKing = 8, //经脉王====>采用新的经脉系统计算
        JieriDaLiBao = 9, //节日礼包
        JieriDengLuHaoLi = 10, //节日登录
        JieriVIP = 11, //VIP大回馈
        JieriCZSong = 12, //节日每日充值
        JieriLeiJiCZ = 13, //节日期间累计充值
        JieriZiKa = 14, //节日兑换
        JieriPTXiaoFeiKing = 15, //节日期间平台消费王
        JieriPTCZKing = 16, //节日期间平台充值王
        JieriBossAttack = 17, //节日期间Boss攻城
        //HeFuDaLiBao = 20, //合服大礼包*/
        //HeFuCZSong = 21, //合服充值大回馈
        //HeFuVIP = 22, //合服VIP大回馈
        //HeFuCZFanLi = 23, //合服充值返利
        //HeFuPKKing = 24, //合服PK王
        //HeFuWanChengKing = 25, //合服王城霸主
        //HeFuBossAttack = 26, //合服BOSS攻城

        HeFuLogin = 20, //合服登陆好礼
        HeFuTotalLogin = 21, //合服累计登陆
        HeFuShopLimit = 22, //合服商店限购
        HeFuRecharge = 23, //合服充值返利
        HeFuPKKing = 24, //合服PK王
        HeFuAwardTime = 25,	// 奖励翻倍（为战而生）
        HeFuBossAttack = 26, //BOSS之战
        
        MeiRiChongZhiHaoLi       = 27,  // 每日充值豪礼 [7/15/2013 LiaoWei]
        ChongJiLingQuShenZhuang  = 28,   // 冲级领取神装 [7/15/2013 LiaoWei]
        ShenZhuangJiQingHuiKui   = 29,  // 神装激情回赠 [7/15/2013 LiaoWei]
        XinCZFanLi = 30, //新的开区充值返利 
        XingYunChouJiang = 31,          // 幸运抽奖 [7/15/2013 LiaoWei]
        YuDuZhuanPanChouJiang = 32,          // 月度转盘 [7/15/2013 LiaoWei]

        NewZoneUpLevelMadman = 33, //-----------冲级狂人---new
        NewZoneRechargeKing = 34,//-------------充值达人---new
        NewZoneConsumeKing = 35,//------------- 消费达人---new
        NewZoneBosskillKing = 36,//-------------屠魔勇士---new
        NewZoneFanli = 37,//--------------------劲爆返利---new

        TotalCharge = 38,//累计充值   回馈
        TotalConsume = 39,//累计消费  回馈
        JieriTotalConsume = 40,     // 节日累计消费
        JieriDuoBei = 41,           // 节日多倍奖励
        JieriQiangGou = 41,           // 节日多倍奖励
        MaxVal, //最大值
    }

    /// <summary>
    /// 目前能够支持节日多倍奖励的活动类型
    /// </summary>
    public enum MultActivityType
    { 
        AngelTemple = 1,        // 天使神殿
        CampBattle = 2,         // 阵营战
        TheKingOfPK = 3,        // PK之王
        OldBattlefield = 4,     // 古战场
        TeamCopyMap = 5,        // 组队副本

        ZhuanHuanCount = 6,

        /*DiamondToCount = 6,     // 钻石转换次数
        BDiamondToCount = 7,    // 绑钻转换次数
        BMoneyToCount = 8,      // 绑金转换次数*/

        ZhuanHuanAward = 9,
        /*DiamondToAward = 9,     // 钻石转换奖励
        BDiamondToAward = 10,   // 绑钻转换奖励
        BMoneyToAward = 11,     // 绑金转换奖励*/

        DiaoXiangCount = 12,    // 雕像膜拜次数
        MergeFruitCoe = 13,     // 合成果实概率
    }

    /// <summary>
    /// 特殊活动类型
    /// </summary>
    public enum SpecialActivityTypes
    {
        BloodCastle = 1,    // 血色堡垒[11/6/2013 LiaoWei]
        DemoSque    = 2,    // 恶魔广场[11/6/2013 LiaoWei]
        CampBattle  = 3,    // 阵营战场[12/23/2013 LiaoWei]
        TheKingOfPK = 4,    // PK之王 [3/22/2014 LiaoWei]
        AngelTemple = 5,    // 天使神殿 [3/23/2014 LiaoWei]
        OldBattlefield = 6,    // 古战场 [7/12/2014 GeWenzheng]
    }

    /// <summary>
    /// VIP特权类型
    /// </summary>
    public enum VipPriorityTypes
    {
        None = 0, //无定义
        GetDailyYuanBao = 1,//每日上线即可免费领取200非绑定元宝
        GetDailyLingLi = 5,//每日上线即可免费领取10000灵力
        GetDailyYinLiang = 6,//每日上线即可免费领取2000银两
        GetDailyAttackFuZhou = 7,//每日免费领取狂攻符咒一个
        GetDailyDefenseFuZhou = 8,//每日免费领取防御符咒一个
        GetDailyLifeFuZhou = 9,//每日免费领取生命符咒一个
        GetDailyTongQian = 10,//每日上线可免费领取100000铜钱
        GetDailyZhenQi = 21,//每日上线可免费领取【幻境阵旗】20个
        MaxVal = 100, //最大值
    }

    /// <summary>
    /// 单次奖励掩码值，用于定位64位变量中的相应位，扩展时将16进制的每一个数字设置成1、2、4、8就能保证
    /// 为了支持64位，不用enum，enum是int32类型
    /// </summary>
    public class OnceAwardMask
    {
        public const long DownloadTinyClient = 0x0000000000000001; //下载微端
        public const long UseMonthVipCard = 0x0000000000000002; //第一次使用vip月卡奖励
        public const long UseSeasonVipCard = 0x0000000000000004; //第一次使用vip季卡奖励
        public const long UseHalfYearVipCard = 0x0000000000000008; //第一次使用vip半年卡奖励
        public const long Other = 0x4000000000000000;//其它
    }

    /// <summary>
    /// 生肖运程竞猜的生肖类型[2字节16位就]
    /// </summary>
    public enum ShengXiaoTypes
    {
        Shu = 0x0001, //鼠
        Niu = 0x0002, //牛
        Hu = 0x0004, //虎
        Tu = 0x0008, //兔
        Long = 0x0010, //龙
        She = 0x0020, //蛇
        Ma = 0x0040, //马
        Yang = 0x0080, //羊
        Hou = 0x0100, //猴
        Ji = 0x0200, //鸡
        Gou = 0x0400, //狗
        Zhu = 0x0800, //猪
    }

    /// <summary>
    /// 生肖运程竞猜运行状态定义
    /// </summary>
    enum ShengXiaoGuessStates
    {
        NoMortgage = 0, //无抵押状态，此时等待第一个人下注
        MortgageCountDown  = 1, //下注倒计时
        BossCountDown = 2, //Boss倒计时，boss没人杀，超时后自动死亡，并生成某个随机结果
        EndKillBoss = 3, //boss已经被杀死,如果给予了奖励，状态将转换到NoMortgage
    };

    /// <summary>
    /// 方向定义
    /// </summary>
    public enum Dircetions
    {
        DR_UP = 0,
        DR_UPRIGHT = 1,
        DR_RIGHT = 2,
        DR_DOWNRIGHT = 3,
        DR_DOWN = 4,
        DR_DOWNLEFT = 5,
        DR_LEFT = 6,
        DR_UPLEFT = 7,
    }

    /// <summary>
    /// 对象的类型
    /// </summary>
    public enum ObjectTypes
    {
        OT_CLIENT = 0,
        OT_MONSTER = 1,
        OT_GOODSPACK = 2,
        OT_BIAOCHE = 3,
        OT_JUNQI = 4,
        OT_NPC = 5,
        OT_DECO = 6,
        OT_FAKEROLE = 7,
    }

    /// <summary>
    /// 角色参数名称--->注意 长度不能超过32字符！！！！！！！！！！！！！！！！存储内容不能超过60字节!!!!!!!!!!!!!!!!!!!!!!!
    /// </summary>
    public class RoleParamName
    {
        public const String MapPosRecord = "MapPosRecord";//地图定位参数，存放规则 mapid,x,y,mapid,x,y 全部是unsigned short 存放
        public const String ChengJiuFlags = "ChengJiuFlags";//成就完成与否 和 奖励领取标志位 每两bit一小分组表示一个成就
        public const String ChengJiuExtraData = "ChengJiuData";//成就相关辅助数据 比如成就点数  总杀怪数量 连续日登录次数 总日登录次数，每个都采用4字节存放
        public const String ZhuangBeiJiFen = "ZhuangBeiJiFen";//装备积分 单个整数
        public const String LieShaZhi = "LieShaZhi";//猎杀值 单个整数
        public const String WuXingZhi = "WuXingZhi";//悟性值 单个整数
        public const String ZhenQiZhi = "ZhenQiZhi";//真气值 单个整数
        public const String TianDiJingYuan = "TianDiJingYuan";//天地精元值 单个整数
        public const String ShiLianLing = "ShiLianLing";//试炼令值 单个整数 ===>通天令值
        public const String MapLimitSecs = "MapLimitSecs_";//地图时间限制前缀, 存储格式为: MapLimitSecs_XXX(地图编号), 日ID,今日已经停留时间(秒),道具额外加的时间(秒)
        public const String JingMaiLevel = "JingMaiLevel";//经脉等级值 单个整数
        public const String WuXueLevel = "WuXueLevel";//武学等级值 单个整数
        public const String ZuanHuangLevel = "ZuanHuangLevel";//砖皇等级值 单个整数
        public const String ZuanHuangAwardTime = "ZHAwardTime";//上次领取钻皇奖励的时间 相对 1970年的毫秒数字符串
        public const String SystemOpenValue = "SystemOpenValue";//系统激活项，主要用于辅助客户端记忆经脉等随等级提升的图标显示 单个整数 按位表示各个激活项，最多32个
        public const String JunGong = "JunGong";//军功值 单个整数
        public const String GuMuAwardDayID = "GuMuAwardDayID";//古墓限时奖励 单个整数
        public const String BossFuBenExtraEnterNum = "BossFuBenNum";//boss副本额外进入次数 单个整数
        public const String KaiFuOnlineDayID = "KaiFuOnlineDayID";//开服在线奖励天ID
        public const String KaiFuOnlineDayBit = "KaiFuOnlineDayBit";//开服在线奖励天的位标志
        public const String KaiFuOnlineDayTimes = "KaiFuOnlineDayTimes_";//开服在线奖励每天的在线时长
        public const String To60or100 = "To60or100"; //达到60或者100级的记忆
        public const String DayGift1 = "MeiRiChongZhiHaoLi1";  //每日充值豪礼1 [7/16/2013 LiaoWei]
        public const String DayGift2 = "MeiRiChongZhiHaoLi2";  //每日充值豪礼2 [7/16/2013 LiaoWei]
        public const String DayGift3 = "MeiRiChongZhiHaoLi3";  //每日充值豪礼3 [7/16/2013 LiaoWei]
        public const String JieriLoginNum = "JieriLoginNum"; //节日的登录次数
        public const String JieriLoginDayID = "JieriLoginDayID"; //节日的登录天ID
        public const String ZiKaDayNum = "ZiKaDayNum"; //当日已经兑换字卡的数量
        public const String ZiKaDayID = "ZiKaDayID"; //当日已经兑换字卡的天ID
        public const String FreeCSNum = "FreeCSNum"; //当日已经免费传送的次数
        public const String FreeCSDayID = "FreeCSDayID"; //当日已经免费传送的天ID
        public const String MaxTongQianNum = "MaxTongQianNum"; //角色的最大铜钱值
        public const String ErGuoTouNum = "ErGuoTouNum"; //二锅头今日的消费次数
        public const String ErGuoTouDayID = "ErGuoTouDayID"; //二锅头今日的天ID
        public const String BuChangFlag = "BuChangFlag"; //补偿的标志
        public const String ZhanHun = "ZhanHun"; //战魂       -- MU 改成 声望 add by liaowei
        public const String RongYu = "RongYu"; //荣誉
        public const String ZhanHunLevel = "ZhanHunLevel"; //战魂等级
        public const String RongYuLevel = "RongYuLevel"; //荣誉等级
        public const String ZJDJiFen = "ZJDJiFen"; //砸金蛋的积分
        public const String ZJDJiFenDayID = "ZJDJiFenDayID"; //砸金蛋的积分天ID
        public const String ZJDJiFenBits = "ZJDJiFenBits"; //砸金蛋的积分领取记录
        public const String ZJDJiFenBitsDayID = "ZJDJiFenBitsDayID"; //砸金蛋的积分领取记录
        public const String FuHuoJieZhiCD = "FuHuoJieZhiCD";            // 复活戒指COOLDOWN[7/31/2013 LiaoWei]
        public const String CurHP = "CurHP";    // 当前血量[7/31/2013 LiaoWei]
        public const String CurMP = "CurMP";    // 当前蓝量[7/31/2013 LiaoWei]
        public const String TotalPropPoint = "TotalPropPoint";              // 总属性点 [8/16/2013 LiaoWei]
        public const String AddProPointForLevelUp = "AddProPointForLevelUp";// 升级获得的属性点 [8/16/2013 LiaoWei]
        public const String sPropStrength = "PropStrength";                 // 力量 [8/19/2013 LiaoWei]
        public const String sPropIntelligence = "PropIntelligence";         // 智力 [8/19/2013 LiaoWei]
        public const String sPropDexterity = "PropDexterity";               // 敏捷 [8/19/2013 LiaoWei]   
        public const String sPropConstitution = "PropConstitution";         // 体力 [8/19/2013 LiaoWei]
        public const String sPropStrengthChangeless = "PropStrengthChangeless";         // 不变的力量 [1/27/2014 LiaoWei]
        public const String sPropIntelligenceChangeless = "PropIntelligenceChangeless"; // 不变的智力 [1/27/2014 LiaoWei]
        public const String sPropDexterityChangeless = "PropDexterityChangeless";       // 不变的敏捷 [1/27/2014 LiaoWei]
        public const String sPropConstitutionChangeless = "PropConstitutionChangeless"; // 不变的体力 [1/27/2014 LiaoWei]
        public const String sChangeLifeCount = "ChangeLifeCount";           // 转生计数 [9/28/2013 LiaoWei]
        public const String AdmireCount     = "AdmireCount";                // 崇拜计数 [11/19/2013 LiaoWei]
        public const String AdmireDayID     = "AdmireDayID";                // 崇拜日期 [11/19/2013 LiaoWei]
        public const String DayOnlineSecond = "DayOnlineSecond";            // 每日在线时长--秒[1/16/2014 LiaoWei]
        public const String SeriesLoginCount = "SeriesLoginCount";          // 连续登陆计数[1/16/2014 LiaoWei]
        public const String TotalLoginAwardFlag = "TotalLoginAwardFlag";    // 累计登陆奖励领取标记 [2/11/2014 LiaoWei]
        public const String SpendDiamond = "SpendDiamond";                  // 钻石计费 [2/20/2014 LiaoWei]
        //public const String VIPLevel = "VIPLevel";                          // VIP等级 [2/19/2014 LiaoWei]
        public const String VIPExp = "VIPExp";                              // VIP经验值--充值以外获得的VIP经验值 比如使用物品 [2/19/2014 LiaoWei]
        //public const String VIPGetAwardFlag = "VIPGetAwardFlag";            // VIP领奖标记 [2/19/2014 LiaoWei]
        public const String BloodCastlePlayerPoint = "BloodCastlePlayerPoint"; // 玩家血色城堡积分 [12/14/2013 LiaoWei]
        public const String BloodCastleSceneid = "BloodCastleSceneid";      // 玩家血色城堡场景ID [12/14/2013 LiaoWei]
        public const String BloodCastleSceneFinishFlag = "BloodCastleSceneFinishFlag";      // 玩家血色城堡完成标记 [12/14/2013 LiaoWei]
        public const String DaimonSquarePlayerPoint = "DaimonSquarePlayerPoint"; // 玩家恶魔广场积分 [12/14/2013 LiaoWei]
        public const String DaimonSquareSceneid = "DaimonSquareSceneid";      // 玩家恶魔广场场景ID [12/14/2013 LiaoWei]
        public const String DaimonSquareSceneFinishFlag = "DaimonSquareSceneFinishFlag";      // 玩家恶魔广场完成标记 [12/14/2013 LiaoWei]
        public const String DaimonSquareSceneTimer = "DaimonSquareSceneTimer";      // 玩家恶魔广场完成时的剩余时长 [12/14/2013 LiaoWei]
        /*public const String BloodCastleTotalPoint   = "BloodCastleTotalPoint";  // 血色城堡最高积分 [12/14/2013 LiaoWei]
        public const String CampBattlePlayerPoint   = "CampBattlePlayerPoint";  // 阵营战场玩家积分 [12/23/2013 LiaoWei]
        public const String CampBattleTotalPoint    = "CampBattleTotalPoint";   // 阵营战场最高积分 [12/23/2013 LiaoWei]*/
        public const String FightGetThings = "FightGetThings"; //挂机拾取选项的记录
        public const String DailyActiveDayID = "DailyActiveDayID";      // 每日活跃DayID [2/27/2014 LiaoWei]
        public const String DailyActiveInfo1 = "DailyActiveInfo1";      // 每日活跃信息 [2/27/2014 LiaoWei]
        public const String DailyActiveFlag = "DailyActiveFlag";        // 活跃完成与否 和 奖励领取标志位 每两bit一小分组表示一个活跃 [2/27/2014 LiaoWei]
        public const String DailyActiveAwardFlag = "DailyActiveAwardFlag"; // 活跃奖励的领取状态[2/27/2014 LiaoWei]
        public const String DefaultSkillLev = "DefaultSkillLev"; // 默认技能的等级 [3/15/2014 LiaoWei]
        public const String DefaultSkillUseNum = "DefaultSkillUseNum"; // 默认技能的熟练度 [3/15/2014 LiaoWei]
        public const String MeditateTime = "MeditateTime";              // 冥想时间 [3/18/2014 LiaoWei]
        public const String NotSafeMeditateTime = "NotSafeMeditateTime"; // 野外冥想时间 [3/18/2014 LiaoWei]
        public const String PKKingAdmireCount = "PKKingAdmireCount";    // Pk之王崇拜计数 [11/19/2013 LiaoWei]
        public const String PKKingAdmireDayID = "PKKingAdmireDayID";    // Pk之王崇拜日期 [11/19/2013 LiaoWei]
        public const String ShengWang = "ShengWang"; //声望
        public const String ShengWangLevel = "ShengWangLevel"; //声望等级
        public const String LiXianBaiTanTicks = "LiXianBaiTanTicks"; //离线摆摊的市场(毫秒)
        public const String OpenGridTick = "OpenGridTick"; // 开启包裹的时间戳 [4/7/2014 LiaoWei]
        public const String OpenPortableGridTick = "OpenPortableGridTick"; // 开启随身仓库包裹的时间戳 [4/7/2014 LiaoWei]
        public const String PictureJudgeFlags = "PictureJudgeFlags"; // 图鉴标记 [5/3/2014 LiaoWei] 
        public const String WanMoTaCurrLayerOrder = "WanMoTaCurrLayerOrder"; // 万魔塔当前层编号 [6/6/2014 ChenXiaojun] 
        public const String UpLevelGiftFlags = "UpLevelGiftFlags"; // 升级有礼条件达到和领取标识 [6/16/2014 LiTeng]
        public const String ImpetrateTime = "ImpetrateTime"; // MU祈福时间 [7/30/2014 LiaoWei]
        public const String ChongJiGiftList = "ChongJiGiftList";//新服冲级狂人是否领取标示
        public const String DailyChargeGiftFlags = "DailyChargeGiftFlags";//日常充值奖励是否领取标示
        public const String StarSoul = "StarSoul";// 星魂值 [8/4/2014 LiaoWei]
        public const String DailyShare = "DailyShare";
        public const String LianZhiJinBiCount = "LianZhiJinBiCount"; //当日炼制金币次数
        public const String LianZhiBangZuanCount = "LianZhiBangZuanCount"; //当日炼制绑钻次数
        public const String LianZhiZuanShiCount = "LianZhiZuanShiCount"; //当日炼制钻石次数
        public const String LianZhiJinBiDayID = "LianZhiJinBiDayID"; //炼制金币的日期
        public const String LianZhiBangZuanDayID = "LianZhiBangZuanDayID"; //炼制绑钻的日期
        public const String LianZhiZuanShiDayID = "LianZhiZuanShiDayID"; //炼制钻石的日期	
        public const String ChengJiuLevel = "ChengJiuLevel";             //成就等级   
        public const String CaiJiCrystalDayID = "CaiJiCrystalDayID";     //水晶幻境采集的日期
        public const String CaiJiCrystalNum = "CaiJiCrystalNum";         //水晶幻境采集的次数
        public const String HeFuLoginFlag = "HeFuLoginFlag";             // 合服登陆好礼标记
        public const String HeFuTotalLoginDay = "HeFuTotalLoginDay";     // 合服累计登陆最后一次在哪天登陆的
        public const String HeFuTotalLoginNum = "HeFuTotalLoginNum";     // 合服累计登陆记录
        public const String HeFuTotalLoginFlag = "HeFuTotalLoginFlag";   // 合服累计登陆领取记录
        public const String HeFuPKKingFlag = "HeFuPKKingFlag";   // 领取合服战场之神的奖励
        public const String JieRiExchargeFlag = "JRExcharge"; // 节日兑换
        public const String VerifyBuffProp = "VerifyBuffProp"; // 校验BUFF属性标记
        public const String GuildCopyMapAwardDay = "GuildCopyMapAwardDay";
        public const String GuildCopyMapAwardFlag = "GuildCopyMapAwardFlag";
        public const String GuildZhanGong = "GuildZhanGong";
        public const String ElementPowderCount = "ElementPowder"; // 元素粉末
        public const String ElementGrade = "ElementGrade"; // 过去元素的档次
        public const String QingGongYanJoinFlag = "QingGongYanJoinFlag"; // 庆功宴参加次数
    }

    /// <summary>
    /// 角色常用整形参数索引 对应 RoleData中的变量RoleCommonUseIntPamamValueList
    /// </summary>
    public enum RoleCommonUseIntParamsIndexs
    {
        ChengJiu = 0,//成就
        ZhuangBeiJiFen = 1,//装备积分===>用道具杀怪获得
        LieShaZhi = 2,//猎杀值
        WuXingZhi = 3,//悟性值
        ZhenQiZhi = 4,//真气值
        TianDiJingYuan = 5,//天地精元值
        ShiLianLing = 6,//试炼令值===>通天令值
        JingMaiLevel = 7,//经脉等级值---通过真气值升级
        WuXueLevel = 8,//武学等级值---通过悟性值升级
        ZuanHuangLevel = 9,//钻皇等级值---通过累积充值元宝值升级
        SystemOpenValue = 10,//系统激活项，主要用于辅助客户端记忆经脉等随等级提升的图标显示 单个整数 按位表示各个激活项，最多32个
        JunGong = 11,//军功值，玩家做任务获取
        KaiFuOnlineDayID = 12,//开服在线奖励DayID
        To60or100 = 13,//达到60或者100级的记忆
        ZhanHun = 14,//战魂
        RongYu = 15,//荣誉
        ZhanHunLevel = 16,//战魂等级
        RongYuLevel = 17,//荣誉等级
        ShengWang = 18,//声望
        ShengWangLevel = 19,//声望等级
        WanMoTaCurrLayerOrder = 20,  // 万魔塔当前层编号
        StarSoulValue = 21,  // 星座值 [8/5/2014 LiaoWei]
        ChengJiuLevel = 22,  // 成就等级
        ZhanGong = 23,   // 战功
		YuansuFenmo = 24,   // 元素粉末
    }

    /// <summary>
    /// 角色ChengJiuExtraData 参数逗号隔开的字段 不要乱改每一个值，可以添加
    /// </summary>
    public enum ChengJiuExtraDataField
    {
        ChengJiuPoints = 0, //成就点数
        TotalKilledMonsterNum = 1,//总杀怪数量
        ContinuousDayLogin = 2,//连续日登录次数
        TotalDayLogin = 3, //总日登录次数
        
        // MU新增 Begin [3/12/2014 LiaoWei]
        TotalKilledBossNum          = 4,    // 击杀指定的BOSS
        CompleteNormalCopyMapNum    = 5,    // 通过普通副本的计数
        CompleteHardCopyMapNum      = 6,    // 通过精英副本的计数
        CompleteDifficltCopyMapNum  = 7,    // 通过炼狱副本的计数
//         ForgeNum                    = 8,    // 强化计数
//         AppendNum                   = 9,    // 追加计数
//         MergeData                   = 10,   // 合成数据
        // MU新增 End [3/12/2014 LiaoWei]
    }

    /// <summary>
    /// 成就类型 太多 用class，不用enum 程序内部不用类型转换
    /// </summary>
    public class ChengJiuTypes
    {
        // 成就系统改造 [3/12/2014 LiaoWei]
        
        public const int FirstKillMonster   = 100; // 第一次杀怪
        public const int FirstAddFriend     = 101; // 第一次加好友
        public const int FirstInTeam        = 102; // 第一次组队
        public const int FirstInFaction     = 103; // 第一次入会
        public const int FirstHeCheng       = 104; // 第一次合成
        public const int FirstQiangHua      = 105; // 第一次强化
        public const int FirstZhuiJia       = 106; // 第一次追加 [3/12/2014 LiaoWei] //第一次洗炼
        public const int FirstJiCheng       = 107; // 第一次强化继承 [3/12/2014 LiaoWei]
        public const int FirstBaiTan        = 108; // 第一次摆摊 [3/12/2014 LiaoWei]

        public const int LevelStart         = 200;  // 等级需求成就开始
        public const int LevelEnd           = 204;  // 等级需求成就结束

        public const int LevelChengJiuStart = 300;  // 转生等级需求成就开始
        public const int LevelChengJiuEnd   = 304;  // 转生等级需求成就结束

        public const int SkillLevelUpStart  = 350;  // 技能升级成就开始 MU新增 [3/30/2014 LiaoWei]
        public const int SkillLevelUpEnd    = 356;  // 技能升级成就结束 MU新增 [3/30/2014 LiaoWei]

        public const int ContinuousLoginChengJiuStart   = 400;  // 连续登录成就开始
        public const int ContinuousLoginChengJiuEnd     = 405;  // 连续登录成就结束

        public const int TotalLoginChengJiuStart    = 500;  // 累积登录成就开始
        public const int TotalLoginChengJiuEnd      = 508;  // 累积登录成就结束

        public const int ToQianChengJiuStart    = 600;  // 铜钱成就开始 -->财富积累--财富之路
        public const int ToQianChengJiuEnd      = 608;  // 铜钱成就结束 -->财富积累

        public const int MonsterChengJiuStart   = 700;  // 怪物成就开始-->浴血沙场---历练之路
        public const int MonsterChengJiuEnd     = 708;  // 怪物成就结束-->浴血沙场

        public const int BossChengJiuStart      = 800;  // boss成就开始-->浴血沙场---历练之路
        public const int BossChengJiuEnd        = 803;  // boss 成就结束-->浴血沙场

        public const int CompleteCopyMapCountNormalStart    = 900;  // 副本通关--普通[3/12/2014 LiaoWei]
        public const int CompleteCopyMapCountNormalEnd      = 905;
        public const int CompleteCopyMapCountHardStart      = 1000;  // 副本通关--精英[3/12/2014 LiaoWei]
        public const int CompleteCopyMapCountHardEnd        = 1005;
        public const int CompleteCopyMapCountDifficltStart  = 1100;  // 副本通关--炼狱[3/12/2014 LiaoWei]
        public const int CompleteCopyMapCountDifficltEnd    = 1105;  // 副本通关--炼狱[3/12/2014 LiaoWei]

        public const int QiangHuaChengJiuStart  = 1200;   // 强化成就开始
        public const int QianHuaChengJiuEnd     = 1210;   // 强化成就结束

        public const int ZhuiJiaChengJiuStart   = 1300;   // 追加成就开始
        public const int ZhuiJiaChengJiuEnd     = 1308;   // 追加成就结束

        public const int HeChengChengJiuStart   = 1400;  // 合成成就开始
        public const int HeChengChengJiuEnd     = 1411;  // 合成成就结束

        public const int GuildChengJiuStart     = 2000;  // 战盟成就开始 MU新增 [3/30/2014 LiaoWei]
        public const int GuildChengJiuEnd       = 2004;  // 战盟成就结束 MU新增 [3/30/2014 LiaoWei]

        public const int JunXianChengJiuStart   = 2050;  // 军衔成就开始 MU新增 [3/30/2014 LiaoWei]
        public const int JunXianChengJiuEnd     = 2061;  // 军衔成就结束 MU新增 [3/30/2014 LiaoWei]

        public const int MainLineTaskStart      = 2100;  // 主线任务成就开始 MU新增 [8/5/2014 LiaoWei]
        public const int MainLineTaskEnd        = 2110;  // 主线任务成就结束 MU新增 [8/5/2014 LiaoWei]

        /*public const int JingMaiChengJiuStart = 700;//经脉成就开始
        public const int JingMaiChengJiuEnd = 709;//经脉成就结束

        public const int WuXueChengJiuStart = 800;//武学成就开始
        public const int WuXueChengJiuEnd = 809;//武学成就结束*/
    }

    //系统运行期间，不管新加任何成就，需要在这添加代码，并初始化相关存储索引项
    public class ChengJiuTypesExt
    {
    }

    /// <summary>
    /// 成就类型 太多 用class，不用enum 程序内部不用类型转换, 同时Flash转换方便
    /// </summary>
    public class SkillTypes
    {
        public const int NormalAttack = 1;//普通物理攻击
        public const int NormalMagic = 2;//普通魔法攻击
        public const int CiShaAttack = 10;//刺杀物理攻击
        public const int FiveBallMagic = 101; //5连击的魔法攻击
        public const int FSHunDunMagic = 102;//法师的护盾
        public const int DSHideMagic = 103;//法师的隐身法术
        public const int ZSChongZhuang = 104;//战士的野蛮冲撞
        public const int ZSAutoUseSkill = 105;//战士的自动触发的技能
        public const int LieHuoJianQi = 107;//战士的的烈火剑气
    }

    /// <summary>
    /// 宠物怪攻击控制类型
    /// </summary>
    public enum PetMonsterControlType
    {
        //JustFollow = 0,//只是跟随主人，不进行任何攻击
        FreeAttack = 1,//自由攻击====>默认配置
        AttackMasterTarget//攻击主人的目标====>玩家设置
    };

    /// <summary>
    /// vip类型
    /// </summary>
    public enum VIPTypes
    {
        NoVip = 0,//非vip
        Month = 1,//月卡vip 1月卡
        Season = 3,//季卡vip 3月卡
        HalfYear = 6//半年卡vip 6月卡
    };

    /// <summary>
    /// 特殊效果的对齐方式
    /// </summary>
    public enum GameEffectAlignModes
    {
        None = -1, //不做处理，左上角
        Center = 0,//上下都居中
        Title = 1,//平铺
        Scale = 2, //拉伸
        HCenter = 3, //左右居中
        VCenter = 4, //上下居中
    };

    /// <summary>
    /// 装饰类型
    /// </summary>
    public enum GDecorationTypes
    {
        None = 0, //无
        Loop = 1,//循环播放
        AutoRemove = 2,//播放一次结束后移除
    };

    /// <summary>
    /// 装备需求限制类型对应物品配置文件中的 ToType 字段中的值
    /// </summary>
    public class EquipRequirementTypes 
	{	
		public const String VIP = "VIP"; //vip对应的值不管,默认采用-1
		public const String JinMai  = "JingMai"; //经脉
		public const String MaxAttack = "MaxAttack"; //最大攻击
		public const String MaxMAttack = "MaxMAttack"; //最大魔法攻击
		public const String MaxDSAttack = "MaxDSAttack"; //最大道术攻击
		public const String UseYuanBao = "UseYuanBao"; //消耗元宝打开
		public const String AddIntoBH = "AddIntoBH"; //加入帮会后
		public const String WuXue = "WuXue"; //武学达到几重
		public const String ChengJiu = "ChengJiu"; //成就达到几重
        public const String ZhanHunLevel = "ZhanHunLevel"; //战魂
        public const String RongYuLevel = "RongYuLevel"; //荣誉
        public const String CanNotBeyondLevel = "CanNotBeyondLevel"; // 不能超过等级限制  [8/15/2014 LiaoWei]
        public const String FEIANQUANQU = "FEIANQUANQU";  //非安全区
	}

    /// <summary>
    /// 货币类型 用class 不用转换类型
    /// </summary>
    public enum MoneyTypes
    {
        None = 0,
        TongQian = 1, //铜钱==>绑定铜钱
        YinLiang = 8, //银两===>铜钱
        JingYuanZhi = 13, //精元值 // MU里的魔精  [8/26/2014 LiaoWei]
        JunGongZhi = 14, //军功值
        ImpetratePoint = 15, // 祈福积分 [8/26/2014 LiaoWei]
        LieShaZhi = 20, //猎杀值
        JiFenZhi = 30, //积分值
        YuanBao = 40, //元宝
        BindYuanBao = 50, //绑定元宝
        ZhanHun = 90, //战魂值
        ZhanGong = 95,// 战盟战功
    }

    /// <summary>
    /// 外部设置的强制阻挡的类型位设置
    /// </summary>
    public enum ForceHoldBitSets
    {
        None = 0, //无
        HoldRole = 0x01, //强制角色阻挡
        HoldMonster = 0x02, //强制怪物阻挡
        HoldNPC = 0x04, //强制NPC阻挡
    };

    /// <summary>
    /// 角色复活类型
    /// </summary>
    public enum RoleReliveTypes
    {
        HomeOrHere = 0,    //回城 复活  或者 原地复活
        Home = 1, //回城复活
        TimeWaiting = 2, //等待一定时间之后复活
        TimeWaitingRandomAlive = 3, //等待一定时间之后, 随机复活
        TimeWaitingOrRelifeNow = 4, // 等待倒计时结束自动复活或花费钻石立即复活,在本场景复活点复活,天使神殿专用  [3/27/2014 LiaoWei]
        CrystalCollectRelive = 6, //水晶幻境复活：倒计时结束后手动点选“原地复活”或者“回城复活” [2014/11/21 Tanglong]
    };

    /// <summary>
    /// 地区区域刷该方式
    /// </summary>
    public enum MonsterBirthTypes
    {
        TimeSpan = 0,    //按照时间段的爆怪机制
        TimePoint = 1,   //按照时间点的爆怪机制
        CopyMapLike = 2,     //副本刷怪机制，当怪物死亡，不会主动复活，必须由特殊用途函数指定才能复活 a 副本地图 b 生肖竞猜类非副本地图[不允许配置文件配置]
        CrossMap = 3,    //穿越地图刷怪机制，主要用于 道士主动召唤宠物，这些宠物能够跨越任意地图，这种刷怪区域每个地图都固有一个，也用于动态随意召唤怪物[不允许配置文件配置]
        AfterKaiFuDays = 4,//开服某些天后才开始的刷怪机制，TimePoints 配置形式 开服多少天;连续刷多少天[负数0表示一直];刷怪方式0或1;0或1的配置
        AfterHeFuDays = 5,//合服后某些天后才开始的刷怪机制，TimePoints 配置形式 开服多少天;连续刷多少天[负数0表示一直];刷怪方式0或1;0或1的配置
        AfterJieRiDays = 6,//节日后某些天后才开始的刷怪机制，TimePoints 配置形式 开服多少天;连续刷多少天[负数0表示一直];刷怪方式0或1;0或1的配置
        CreateDayOfWeek = 7, // 一周中的哪天刷 TimePoints 配置形式 周几,时间点|周几,时间点|周几,时间点 [1/10/2014 LiaoWei]
    };

    /// <summary>
    /// 角色的状态命令ID
    /// </summary>
    public enum RoleStatusIDs
    {
        HuDun = 0,    //法师的护盾
        ZhongDu = 1,  //中毒
        DongJie = 2,  //冻结
        Faint   = 3,  // 昏迷 [3/14/2014 LiaoWei]
        SlowDown= 4,  // 减速 [3/14/2014 LiaoWei]
        HitDown = 5,  // 命中降低[6/5/2014 liuhuicheng]
        AttackDown = 6,  // 攻击降低[6/5/2014 liuhuicheng]
        DefenseDown = 7,  // 防御降低[6/5/2014 liuhuicheng]        
        DingShen = 8, // 定身 [7/30/2014 ChenXiaojun]
    };

    /// <summary>
    /// 角色的一些常见的属性
    /// </summary>
    public enum RolePropIndexs
    {
        None = 0, //无定义
        BanChat = 1,//永久禁言
        BanLogin = 2,//永久禁止登陆
    }

    /// <summary>
    /// 一键操作的类型
    /// </summary>
    public enum OneKeyOTypes
    {
        None = 0, //无定义
        BatchSaleOut = 1,//批量出售
        BatchSaleBack = 2,//批量回收
    }

    /// <summary>
    /// 豪礼相关按键索引
    /// </summary>
    public enum GiftBtnIndex
    {
        BTN1 = 1, 
        BTN2 = 2, 
        BTN3 = 3,
        BTN4 = 4,
        BTN5 = 5, 
    }

    /// <summary>
    /// 全局的配置相关的名称
    /// </summary>
    public class GameConfigNames
    {
        public const String ChongJiGift1 = "ChongJiLingQuShenZhuang1";      //冲级领取神装1 [7/16/2013 LiaoWei]
        public const String ChongJiGift2 = "ChongJiLingQuShenZhuang2";      //冲级领取神装2 [7/16/2013 LiaoWei]
        public const String ChongJiGift3 = "ChongJiLingQuShenZhuang3";      //冲级领取神装3 [7/16/2013 LiaoWei]
        public const String ChongJiGift4 = "ChongJiLingQuShenZhuang4";      //冲级领取神装4 [7/16/2013 LiaoWei]
        public const String ChongJiGift5 = "ChongJiLingQuShenZhuang5";      //冲级领取神装5 [7/16/2013 LiaoWei]
        public const String ShenZhuangHuiKuiGift    = "ShenZhuangHuiKuiGift";//神装回馈 [7/16/2013 LiaoWei]
        public const String PKKingRole              = "PKKingRole";          // PK之王  [3/24/2014 LiaoWei]
        public const String AngelTempleRole         = "AngelTempleRole";     // 天使神殿击杀BOSS [3/24/2014 LiaoWei]
        public const String BloodCastlePushMsgDayID = "BloodCastlePushMsgDayID";// 血色堡垒消息推送Dayid [1/24/2014 LiaoWei]
        public const String DemoSquarePushMsgDayID  = "DemoSquarePushMsgDayID"; // 恶魔广场推送Dayid [1/24/2014 LiaoWei]
        public const String BattlePushMsgDayID      = "BattlePushMsgDayID";     // 阵营战消息推送Dayid [1/24/2014 LiaoWei]
        public const String PKKingPushMsgDayID      = "PKKingPushMsgDayID";     // PK之王消息推送Dayid [1/24/2014 LiaoWei]
        public const String AngelTempleMonsterUpgradeNumber = "AngelTempleMonsterUpgradeNumber";     // 天使神殿击杀BOSS [3/24/2014 LiaoWei]
        public const String ChongJiGiftList = "ChongJiGiftList";
    }

    // 增加攻击类型枚举 [10/25/2013 LiaoWei]
    /// <summary>
    /// 攻击类型
    /// </summary>
    public enum AttackType
    {
        PHYSICAL_ATTACK = 0,
        MAGIC_ATTACK    = 1,
    }

    /// <summary>
    /// 延迟动作枚举   [10/25/2013 LiaoWei]
    /// </summary>
    public enum DelayActionType
    {
        DB_NULL  = 0, // 空
        DA_BLINK = 1, // 闪现
    }


    /// <summary>
    /// 血色堡垒mapcode信息 [11/5/2013 LiaoWei]
    /// </summary>
    public enum BloodCastleMapCode
    {
        BLOODCASTLEMAPCODE1 = 5000,
        BLOODCASTLEMAPCODE2 = 5001,
        BLOODCASTLEMAPCODE3 = 5002,
        BLOODCASTLEMAPCODE4 = 5003,
        BLOODCASTLEMAPCODE5 = 5004,

    }

    /// <summary>
    /// 血色堡垒战斗状态枚举信息 [11/5/2013 LiaoWei]
    /// </summary>
    public enum BloodCastleStatus
    {
        FIGHT_STATUS_NULL    = 0,    // 战斗状态-无
        FIGHT_STATUS_PREPARE = 1,    // 战斗状态-准备
        FIGHT_STATUS_BEGIN   = 2,    // 战斗状态-开始
        FIGHT_STATUS_END     = 3,    // 战斗状态-结束

    }

    /// <summary>
    /// 血色堡垒水晶棺掉落物品 [11/5/2013 LiaoWei]
    /// </summary>
    public enum BloodCastleCrystalItemID
    {
        BloodCastleCrystalItemID1 = 10000,
        BloodCastleCrystalItemID2 = 10001,
        BloodCastleCrystalItemID3 = 10002,
    }

    /// <summary>
    /// 恶魔战斗状态枚举信息 [11/5/2013 LiaoWei]
    /// </summary>
    public enum DaimonSquareStatus
    {
        FIGHT_STATUS_NULL       = 0,    // 战斗状态-无
        FIGHT_STATUS_PREPARE    = 2,    // 战斗状态-准备
        FIGHT_STATUS_BEGIN      = 3,    // 战斗状态-开始
        FIGHT_STATUS_END        = 4,    // 战斗状态-结束
    }

    /// <summary>
    /// MU武器手持种类
    /// </summary>
    public enum WeaponHandType
    {
        WHT_LEFTTYPE    = 0, // 左手佩戴
        WHT_RIGHTTYPE   = 1, // 右手佩戴
        WHT_BOTHTYPE    = 2, // 左右手均可佩戴
    }

    // 新手场景信息 [12/1/2013 LiaoWei]
    public enum FRESHPLAYERSCENEINFO
    {
        FRESHPLAYERMAPID = 3000,
        FRESHPLAYERMAPCODEID = 6090,
        FRESHPLAYERBUFFGOODSID = 2000700,
        FRESHPLAYERBUFFTASKID = 101,
    }

    // 崇拜相关 [12/10/2013 LiaoWei]
    public enum ADMIREINFO
    {
        ADMIRE_MAXCOUNTONEDAY = 10,
    }

    // 勇者大陆地图ID [12/1/2013 LiaoWei]
    public enum MAINCITYMAPCODE
    {
        YONGZHEDALUMAPID = 1,
    }


    // 合成系统 - 翅膀合成相关 [12/12/2013 LiaoWei]
    public enum WINGMERGEINFO
    {
        WINGMERGE_FIRST_LEVEL_ID    = 4,
        WINGMERGE_SECOND_LEVEL_ID   = 5,
        WINGMERGE_THIRD_LEVEL_ID    = 6,
    }

    // 卓越属性编号 [12/13/2013 LiaoWei]
    public enum ExcellencePorp
    {
        EXCELLENCEPORP0 = 0,
        EXCELLENCEPORP1,
        EXCELLENCEPORP2,
        EXCELLENCEPORP3,
        EXCELLENCEPORP4,
        EXCELLENCEPORP5,
        EXCELLENCEPORP6,
        EXCELLENCEPORP7, 
        EXCELLENCEPORP8,
        EXCELLENCEPORP9,
        EXCELLENCEPORP10,
        EXCELLENCEPORP11,
        EXCELLENCEPORP12,
        EXCELLENCEPORP13,
        EXCELLENCEPORP14,
        EXCELLENCEPORP15,
        EXCELLENCEPORP16,
        EXCELLENCEPORP17,
        EXCELLENCEPORP18,
        EXCELLENCEPORP19,
        EXCELLENCEPORP20,
        EXCELLENCEPORP21,
        EXCELLENCEPORP22,
        EXCELLENCEPORP23,
        EXCELLENCEPORP24,
        EXCELLENCEPORP25,
        EXCELLENCEPORP26,
        EXCELLENCEPORP27,
        EXCELLENCEPORP28,
        EXCELLENCEPORP29,
        EXCELLENCEPORP30,
        EXCELLENCEPORPMAXINDEX,
    }

    /// <summary>
    /// 定义挂机要拾取的类型
    /// </summary>
    public enum GetThingsIndexes
    {
        Color_Bai = 0,
        Color_Lv = 1,
        Color_Lan = 2,
        Color_Zi = 3,

        BaoShi = 24,
        YuMao = 25,
        YaoPin = 26,
        JinBi = 27,
        MenPiaoCaiLiao = 28,
        QiTaDaoJu = 29,
    }

    /// <summary>
    /// VIP枚举值
    /// </summary>
    public enum VIPEumValue
    {
        VIPENUMVALUE_MAXLEVEL = 10,     // VIP最大等级
        VIPENUMVALUE_GOODFRUIT= 5001,   // VIP影响的果实ID
        VIPENUMVALUE_XIULILEV = 0,      // VIP一键修理的等级限制
        VIPENUMVALUE_TRANSPORT= 1,      // VIP传送的等级限制
        VIPENUMVALUE_OPENSHOPPINGLEV= 7,// VIP打开商店等级限制
        VIPENUMVALUE_GOODGOLD = 7037,   // VIP影响的果实ID
    }

    /// <summary>
    /// VIP类型枚举值
    /// </summary>
    public enum VIPTYPEEum
    {
        VIPTYPEEUM_REALIVE          = 1000000,   // 原地复活
        VIPTYPEEUM_ENTERBLOODCASTLE = 1000001,   // 进入血色堡垒
        VIPTYPEEUM_ENTERDAIMONSQUARE= 1000002,   // 进入恶魔广场
        VIPTYPEEUM_USEFRUIT         = 1000003,   // 使用果实
        VIPTYPEEUM_USEFGOLD         = 1000004,   // 使用金条
        VIPTYPEEUM_ENTEREXPCOPYMAP  = 1000005,   // 进入经验副本
    }

    /// <summary>
    /// 每日活跃类型
    /// </summary>
    public class DailyActiveTypes
    {
        public const int LoginGameCount = 100;    // 每日登陆游戏计数
        public const int SeriesLogin    = 200;    // 连续登陆
        public const int MallBuyCount   = 300;    // 商城购买物品

        public const int CompleteDailyTaskCount1 = 400; // 完成日常任务计数1
        public const int CompleteDailyTaskCount2 = 401; // 完成日常任务计数2

        public const int CompleteNormalCopyMapCount1    = 500; // 完成普通副本计数1
        public const int CompleteHardCopyMapCount1      = 600; // 完成精英副本计数1
        public const int CompleteDifficltCopyMapCount1  = 700; // 完成精英副本计数1

        public const int CompleteBloodCastle    = 800; // 完成血色堡垒
        public const int CompleteDaimonSquare   = 900; // 完成恶魔广场
        public const int CompleteBattle         = 1000; // 完成阵营战
        public const int EquipForge             = 1100; // 装备强化
        public const int EquipAppend            = 1200;// 装备追加

        public const int KillMonster1           = 1300;// 击杀怪物1
        public const int KillMonster2           = 1301;// 击杀怪物2
        public const int KillMonster3           = 1302;// 击杀怪物3

        public const int KillBoss               = 1400;// 击杀Boss
        public const int CompleteChangeLife     = 1500;// 完成转生
        public const int MergeFruit             = 1600;// 完成合成果实

    }

    /// <summary>
    /// 角色DailyActiveDataField1 参数逗号隔开的字段 不要乱改每一个值，可以添加
    /// </summary>
    public enum DailyActiveDataField1
    {
        DailyActiveValue                    = 0,    // 每日活跃值
        DailyActiveTotalKilledMonsterNum    = 1,    // 每日总杀怪数量
        DailyActiveDayLoginNum              = 2,    // 每日登陆次数
        DailyActiveBuyItemInMall            = 3,    // 每日商城消费
        DailyActiveCompleteDailyTask        = 4,    // 每日完成日常任务
        DailyActiveCompleteCopyMap1         = 5,    // 每日完成普通副本
        DailyActiveCompleteCopyMap2         = 6,    // 每日完成精英副本
        DailyActiveCompleteCopyMap3         = 7,    // 每日完成炼狱副本
        DailyActiveCompleteBloodCastle      = 8,    // 每日完成血色堡垒活动
        DailyActiveCompleteDaimonSquare     = 9,    // 每日完成恶魔广场活动
        DailyActiveCompleteBattle           = 10,   // 每日完成阵营战活动
        DailyActiveEquipForge               = 11,   // 每日完成装备强化
        DailyActiveEquipAppend              = 12,   // 每日完成装备追加
        DailyActiveChangeLife               = 13,   // 每日完成转生
        DailyActiveMergeFruit               = 14,   // 每日完成合成果实
        DailyActiveOnline                   = 15,   // 每日完成在线时长
        DailyActiveTotalKilledBossNum       = 16,   // 每日总杀Boss数量
        // DailyActiveTotalWanmotaSweepNum     = 17,   // 每日扫荡万魔塔数量产品的活跃 目前还不需要
    }

    /// <summary>
    /// 技能枚举
    /// </summary>
    public enum SKILLINFO
    {
        SKILLINFO_MAXLEVEL = 100, // 技能最大等级
    }

    /// <summary>
    /// 副本难度枚举
    /// </summary>
    public enum COPYMAPLEVEL
    {
        COPYMAPLEVEL_NORMAL     = 1, // 普通难度
        COPYMAPLEVEL_HARD       = 2, // 精英难度
        COPYMAPLEVEL_DIFFICLT   = 3, // 炼狱难度
    }

    /// <summary>
    /// 开启包裹相关枚举
    /// </summary>
    public enum OPENBAGGRID
    {
        OPENBAGGRID_ONEGRIDNEEDTIME1 = 50 * 60, // 开启一个背包格子需要的在线时间 秒
        OPENBAGGRID_ONEGRIDNEEDTIME2 = 25 * 60, // 开启一个仓库格子需要的在线时间 秒
    }

    /// <summary>
    /// 经验副本 [3/18/2014 LiaoWei]
    /// </summary>
    public enum EXPERIENCESCENEINFO
    {
        EXPERIENCSCENEMAPCODEID1 = 5000,
        EXPERIENCSCENEMAPCODEID2 = 5001,
        EXPERIENCSCENEMAPCODEID3 = 5002,
        EXPERIENCSCENEMAPCODEID4 = 5003,
        EXPERIENCSCENEMAPCODEID5 = 5004,
        EXPERIENCSCENEMAPCODEID6 = 5005,
        EXPERIENCSCENEMAPCODEID7 = 5006,
        EXPERIENCSCENEMAPCODEID8 = 5007,
        EXPERIENCSCENEMAPCODEID9 = 5008,
    }

    /// <summary>
    ///  PK之王枚举信息 [3/22/2014 LiaoWei]
    /// </summary>
    public enum THEKINGOFPKINFO
    {
        THEKINGOFPKINFO_MAPCODE = 10000,
    }

    /// <summary>
    /// 天使神殿场景状态 [3/23/2014 LiaoWei]
    /// </summary>
    public enum AngelTempleStatus
    {
        FIGHT_STATUS_NULL       = 0,    // 战斗状态-无
        FIGHT_STATUS_PREPARE    = 1,    // 战斗状态-准备
        FIGHT_STATUS_BEGIN      = 2,    // 战斗状态-开始
        FIGHT_STATUS_END        = 3,    // 战斗状态-结束
    }

    /// <summary>
    /// 场景UI类型定义,每种类型包括一组mapCode
    /// </summary>
    public enum SceneUIClasses
    {
        Normal = 0, //常规地图
        NormalCopy = 1, //普通副本
        DianJiangCopy = 2, //点将台副本
        CaiShenMiaoCopy = 3, //财神庙副本
        TaskCopy = 4, //任务剧情副本
        BloodCastle = 5, //血色城堡
        Demon = 6,	//恶魔广场
        Battle = 7, //阵营战
        NewPlayerMap, //新手场景地图
        JingYanFuBen, //经验副本 5000-5009
        KaLiMaTemple, //多人副本 - 卡利玛神庙
        EMoLaiXiCopy,//多人副本 - 恶魔来袭
        PKKing, //PK之王
        AngelTemple, //天使神殿
        BossFamily, //Boss之家
        HuangJinShengDian, //黄金圣殿
        JingJiChang, //竞技场[chenshu]
        PaTa, //爬塔
        JinBiFuBen,//个人金币副本
        QiJiMiJing, //奇迹密境
        GuZhanChang, //古战场
        ShuiJingHuanJing, //水晶幻境

        Max, //最大值
    }

    /// <summary>
    /// 假人的类型
    /// </summary>
    public enum FakeRoleTypes
    {
        DiaoXiang = 0,    // 雕像膜拜
        LiXiaBaiTan = 1,    //离线摆摊
        LiXianGuaJi = 2,    //离线挂机
    }

    public enum GongNengIDs
    {
        None = -1,
        MingXiang = 14, //冥想
        GuaJi = 15, //挂机
        GamePayerRolePartChiBang = 19, //翅膀
        JinBiFuBen = 20, //金币副本
        PaTa = 21, //万魔塔
        GuZhanChang = 22, //古战场
        ZuDuiFuBen = 23, //组队副本
        BaiTan = 2, //摆摊
        XiLian = 31, //洗炼
        GamePayerRolePartXingZuo = 32, //星座
        RiChangRenWu = 35, //日常任务
        PaiHangBang = 10, //排行榜
        TaofaRenWu = 41, //讨伐任务
        CrystalCollect = 42, //水晶幻境
    }
    /// <summary>
    /// 战盟建筑类型定义
    /// </summary>
    public enum ZhanMengBuilds
    {
        ZhanQi = 1,             //战旗
        JiTan = 2,              //祭坛
        JunXie = 3,             //军械
        GuangHuan = 4,          //光环
    }

    /// <summary>
    /// 转生信息 [4/21/2014 LiaoWei]
    /// </summary>
    public enum ChangeLifeEnum
    {
        CHANGELIFEINFO_MAXLEV = 10, // 最大转生等级
    }

    /// <summary>
    /// 伤害类型 [4/21/2014 LiaoWei]
    /// </summary>
    public enum DamageType
    {
        DAMAGETYPE_DEFAULT          = 0, // 默认值
        DAMAGETYPE_IGNOREDEFENCE    = 1, // 无视防御
        DAMAGETYPE_DOUBLEATTACK     = 2, // 双倍一击
        DAMAGETYPE_EXCELLENCEATTACK = 3, // 卓越一击
        DAMAGETYPE_LUCKYATTACK      = 4, // 幸运一击
        DAMAGETYPE_THORNDAMAGE      = 5, // 反弹伤害
    }

    /// <summary>
    /// 消息推送相关 [4/23/2014 LiaoWei]
    /// </summary>
    public enum PushMessageInfo
    {
        PUSHMSG_TIGGER_INTERVAL_DAY1 = 3,   // 离线间隔天数
        PUSHMSG_TIGGER_INTERVAL_DAY2 = 7,   // 离线间隔天数
        PUSHMSG_TIGGER_INTERVAL_DAY3 = 15,  // 离线间隔天数       
    }

    /// <summary>
    /// 图鉴系统类型ID [5/3/2014 LiaoWei]
    /// </summary>
    public class PictureJudgeTypesID
    {
        public const int YongZheDaLuPictureJudgeStart   = 100;      // 勇者大陆图鉴开始
        public const int YongZheDaLuPictureJudgeEnd     = 115;      // 勇者大陆图鉴结束

        public const int XianZongLinPictureJudgeStart   = 200;      // 仙踪林图鉴开始
        public const int XianZongLinPictureJudgeEnd     = 215;      // 仙踪林图鉴结束

        public const int BingFengGuPictureJudgeStart    = 300;      // 冰风谷图鉴开始
        public const int BingFengGuPictureJudgeEnd      = 315;      // 冰风谷图鉴结束

        public const int DiXiaChengPictureJudgeStart    = 400;      // 地下城图鉴开始
        public const int DiXiaChengPictureJudgeEnd      = 415;      // 地下城图鉴结束

        public const int ShiLuoZhiTaPictureJudgeStart   = 500;      // 失落之塔图鉴开始
        public const int ShiLuoZhiTaPictureJudgeEnd     = 515;      // 失落之塔鉴结束

        public const int YaTeLanDiShiPictureJudgeStart  = 600;      // 亚特兰蒂斯图鉴开始
        public const int YaTeLanDiShiPictureJudgeEnd    = 615;      // 亚特兰蒂斯鉴结束

        public const int SiWangShaMoPictureJudgeStart   = 700;      // 死亡沙漠图鉴开始
        public const int SiWangShaMoPictureJudgeEnd     = 715;      // 死亡沙漠图鉴结束

        public const int TianKongZhiChengPictureJudgeStart  = 800;  // 天空之城图鉴开始
        public const int TianKongZhiChengPictureJudgeEnd    = 815;  // 天空之城图鉴结束

    }

    public enum GoldCopySceneEnum
    {
        GOLDCOPYSCENEMAPCODEID = 5100,
    }
    public enum ServerNotifyOpenWindowTypes
    {
        /// <summary>
        ///npc傀儡窗口 这类窗口通过npc本身就可以打开USE_GOODSFORDLG(0, npcid)
        /// </summary>
        NpcDummy = 0,

        /// <summary>
        ///地图位置定位窗 USE_GOODSFORDLG(0, fileID)
        /// </summary>
        MapPosRecord = 1,

        /// <summary>
        ///倒计时窗口(时间,类型默认0,提示文字默认null)
        /// </summary>
        CountDownWindow = 2,

    }

    public enum CountDownWindowTypes
    {
        Normal, //显示通用的倒计时和战斗开始
        NumberOnly, //仅显示倒计时
        NumberWithMessage,//显示倒计时和文本信息

        ConstMaxNumber = 3, //常量-限制最大显示的倒计时数字
    }

    public enum CandoType
    {
        DailyTask = 1,//日常任务
        StoryCopy,//剧情副本
        EXPCopy,//经验副本
        GoldCopy,//金币副本
        DemonSquare = 5,//恶魔广场
        BloodCity = 6,//血色城堡
        AngelTemple = 7,//天使神殿
        WanmoTower,//万魔塔
        OldBattlefield,//古战场
        PartWar,//阵营战
        PKKing, // pk之王
        GroupCopy,//组队副本      
        Arena = 13, //竞技场
        //14,雕像膜拜
        TaofaTaskCanDo = 15, //讨伐任务
        CrystalCollectCanDo = 16, //水晶幻境
        MaxCandoTypeNum = 16,
    }
    /// <summary>
    /// Boss AI 触发类型
    /// </summary>
    public enum BossAITriggerTypes
    {
        BirthOn = 0, //自身出生后
        BloodChanged = 1, //自身血量变化
        Injured = 2, //自身受击后
        Dead = 3, //自身死亡后
        Attacked = 4, //自身攻击后
        DeadAll = 5, //同类怪物全部死亡后
        LivingTime = 6 //怪物存活多长时间后        
    }


    // 活动给客户端返回的结果枚举
    enum ActivityErrorType
    {

        // 合服活动的错误提示
        HEFULOGIN_NOTVIP = -100,

        FATALERROR      = -60,      // 致命错误 比如db返回参数不对等
        AWARDCFG_ERROR = -50,     // 奖励配置错误
        AWARDTIME_OUT = -40,    // 不在配置的领取时间内

        // 以前的值转换成枚举了
        NOTCONDITION = -30,     // 条件不满足
        BAG_NOTENOUGH = -20,    // 背包不足
        ALREADY_GETED = -10,    // 已领取
        MINAWARDCONDIONVALUE = -5,  // 每日充值豪礼充值不满足的返回值
        ACTIVITY_NOTEXIST = -1,     // 活动不存在
        RECEIVE_SUCCEED = 0,          // 成功领取
    };


    // 合服登陆标记的枚举
    public enum HeFuLoginFlagTypes
    {
        HeFuLogin_Null = 0,
        HeFuLogin_Login = 1,    //登陆的标志
        HeFuLogin_NormalAward = 2,    //领取普通奖励的标志
        HeFuLogin_VIPAward = 3,    //领取VIP奖励的标志
    }

}
