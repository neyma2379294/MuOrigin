using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDBServer.Logic
{
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
        MDefense= 3,

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
        /// 最大值
        /// </summary>
        MaxVal,
    }

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
    /// 排行榜的类型
    /// </summary>
    public enum PaiHangTypes
    {
        None = 0, //无定义
        EquipJiFen = 1, //装备积分
        XueWeiNum = 2, //穴位个数
        SkillLevel = 3, //技能级别
        HorseJiFen = 4, //坐骑积分
        RoleLevel = 5, //角色等级
        RoleYinLiang = 6, //角色银两
        LianZhan = 7, //角色连斩
        KillBoss = 8, //杀BOSS数量
        BattleNum = 9, //角斗场称号次数
        HeroIndex = 10, //英雄逐擂的到达层数
        RoleGold = 11, //角色金币
        CombatForceList = 12, // 战斗力 [12/18/2013 LiaoWei]
        JingJi = 13, //竞技场
        WanMoTa = 14, //万魔塔
        Wing = 15, //万魔塔
        MaxVal, //最大值
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
        /// 皇宫
        /// </summary>
        HuangGong = 6,

        /// <summary>
        /// 最大值
        /// </summary>
        MaxVal = 7,
    };

    /// <summary>
    /// 皇帝特权次数
    /// </summary>
    public enum HuanDiTeQuanNum
    {
        Max = 3,
    }

    /// <summary>
    /// 帮会人数上限
    /// </summary>
    public enum BangHuiNum
    {
        Max = 100,
    }

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
        JieriDaLiBao = 9, //大型节日大礼包
        JieriDengLuHaoLi = 10, //节日登录豪礼
        JieriVIP = 11, //VIP大回馈
        JieriCZSong = 12, //节日期间充值大回馈
        JieriLeiJiCZ = 13, //节日期间累计充值
        JieriZiKa = 14, //节日期间字卡换礼盒
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

        MeiRiChongZhiHaoLi = 27,  // 每日充值豪礼 [7/15/2013 LiaoWei]
        ChongJiLingQuShenZhuang = 28,   // 冲级领取神装 [7/15/2013 LiaoWei]
        ShenZhuangJiQingHuiKui = 29,  // 神装激情回赠 [7/15/2013 LiaoWei]
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
        MaxVal, //最大值
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
    /// 角色创建常量信息
    /// </summary>
    public enum RoleCreateConstant
    {
        GridNum = 50,//角色创建时背包有50个格子
    }

    /// <summary>
    /// 角色参数名称--->注意 长度不能超过16字符！！！！！！！！！！！！！！！！存储内容不能超过60字节!!!!!!!!!!!!!!!!!!!!!!!
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
        public const String ZhanHun = "ZhanHun"; //战魂
        public const String RongYu = "RongYu"; //荣誉
        public const String ZhanHunLevel = "ZhanHunLevel"; //战魂等级
        public const String RongYuLevel = "RongYuLevel"; //荣誉等级
        public const String ZJDJiFen = "ZJDJiFen"; //砸金蛋的积分
        public const String ZJDJiFenDayID = "ZJDJiFenDayID"; //砸金蛋的积分天ID
        public const String ZJDJiFenBits = "ZJDJiFenBits"; //砸金蛋的积分领取记录
        public const String ZJDJiFenBitsDayID = "ZJDJiFenBitsDayID"; //砸金蛋的积分领取记录
        
    }

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
    /// 战盟建筑类型定义
    /// </summary>
    public enum ZhanMengBuilds
    {
        ZhanQi = 1,             //战旗
        JiTan = 2,              //祭坛
        JunXie = 3,             //军械
        GuangHuan = 4,          //光环
    }
}
