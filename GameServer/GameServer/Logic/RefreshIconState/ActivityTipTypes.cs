	/// summary
	/// 活动提示类型(节点)
	/// summary
	public enum ActivityTipTypes
	{
        Root,                           // 根节点

        MainHuoDongIcon = 1000,         // 主活动图标 (客户端判断)
        RiChangHuoDong,                 // 日常活动 (客户端判断)
		ShiJieBoss,	                    // 世界Boss 
        VIPHuoDong,	                    // 付费Boss (客户端判断)
        ShouFeiBoss,                    // 付费Boss (客户端判断)
        HuangJinBoss,                   // 黄金部队
        RiChangHuoDongOther,            // 其他日常活动(除黄金Boss) (客户端判断)
        AngelTemple,                    // 天使神殿

        MainFuBenIcon = 2000,           // 主副本图标 (客户端判断)
        JuQingFuBen,                    // 剧情副本 (客户端判断)
        ZuDuiFuBen,                     // 组队副本 (客户端判断)
        RiChangFuBen,                   // 日常副本 (客户端判断)

        MainFuLiIcon = 3000,            // 主福利图标
        FuLiChongZhiHuiKui,             // 充值回馈
        ShouCiChongZhi,                 // 充值回馈-首次充值 (OK)
        MeiRiChongZhi,                  // 充值回馈-每日充值 (OK)
        LeiJiChongZhi,                  // 充值回馈-累积充值
        LeiJiXiaoFei,                   // 充值回馈-累计消费
        FuLiMeiRiHuoYue,                // 每日活跃 (OK)
        FuLiLianXuDengLu,               // 连续登录 (OK)
        FuLiLeiJiDengLu,                // 累计登陆 (OK)
        FuLiMeiRiZaiXian,               // 每日在线 (OK)
        FuLiUpLevelGift,                // 等级奖励 (OK)
        ShouCiChongZhi_YiLingQu,        // 首次充值-已领取
        MeiRiChongZhi_YiLingQu,         // 每日充值-已领取

        MainJingJiChangIcon = 4000,     // 主竞技场图标
        JingJiChangJiangLi,             // 奖励预览
        JingJiChangJunXian,             // 军衔提升
        JingJiChangLeftTimes,           // 剩余挑战次数

        MainGongNeng = 5000,            // 主功能图标 (客户端判断)
        MainMingXiangIcon = 5001,       // 功能里的冥想图标 (客户端判断)
        MainEmailIcon = 5002,           // 功能里的邮件图标

        MainXinFuIcon = 6000,           // 主新服图标
        XinFuLevel = 6001,              // 练级狂人 (客户端判断)
        XinFuKillBoss = 6002,           // 屠魔勇士
        XinFuChongZhiMoney = 6003,      // 充值达人
        XinFuUseMoney = 6004,           // 消费达人
        XinFuFreeGetMoney = 6005,       // 劲爆返利

        MainMeiRiBiZuoIcon = 7000,      // 每日必做图标
        ZiYuanZhaoHui = 7001,           // 资源找回

        QiFuIcon = 8000,                //祈福功能

        MainChengJiuIcon = 9000,        // 主成就图标

        VIPGongNeng = 10000,            //vip功能
        VIPGifts = 10001,               //vip礼包

        BuChangIcon = 11000,            //补偿

        HeFuActivity    = 12000,        // 合服活动总叹号
        HeFuLogin       = 12001,        // 合服登陆
        HeFuTotalLogin  = 12002,        // 合服累计登陆
        HeFuRecharge    = 12003,        // 合服充值返利 
        HeFuPKKing      = 12004,        // 合服战场之神

        ShuiJingHuangJin = 13000,       //水晶幻境

        JieRiActivity   = 14000,        // 节日活动总叹号
        JieRiLogin      = 14001,        // 节日登陆
        JieRiTotalLogin = 14002,        // 节日累计登陆
        JieRiDayCZ      = 14003,        // 节日每日充值 
        JieRiLeiJiXF    = 14004,        // 节日累计消费 
        JieRiLeiJiCZ    = 14005,        // 节日累计充值 
        JieRiCZKING     = 14006,        // 节日充值王 
        JieRiXFKING     = 14007,        // 节日消费王 
        
        GuildIcon       = 15000,        // 战盟界面
        GuildCopyMap    = 15001,        // 有没领取的战盟副本的奖励
	}
