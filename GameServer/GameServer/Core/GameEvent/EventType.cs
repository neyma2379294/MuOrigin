using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Core.GameEvent
{
    /// <summary>
    /// 事件类型定义，只允许定义大事件，同一类事件需要细分类型，请在各自逻辑内定义自己的常量来区分（示例：参见战盟事件 logic/BangHui/ZhanMengShiJian/ZhanMengShiJianManager.cs)
    /// </summary>
    public enum EventTypes
    {
        ZhanMengShiJian,    //战盟事件
        XueSeChengBao,      //血色城堡时间 
        EMoGuangChang,      //恶魔广场
        HuangJinBuDui,      //黄金部队
        YeWaiBoss,          //野外boss
        PKKingHuoDong,      //PK之王活动
        TanWeiGouMai,       //摊位购买
        MingXiang,          //冥想
        ZhaoHuanUserID,     //用户不登录召唤
        PlayerLevelup,      //玩家升级
        PlayerDead,         //玩家死亡
        MonsterDead,        //怪物死亡
        PlayerLogout,       //玩家退出
        PlayerLeaveFuBen,   //玩家离开副本
        //JingJiFuBenEndForTime, //竞技场副本结束（时间到）
        PlayerInitGame,         //初始化游戏角色
        MonsterBirthOn,         //怪物出生
        MonsterInjured,        //怪物第一次被攻击后
        MonsterBlooadChanged,   //怪物血量变化
        MonsterAttacked,        //怪物攻击后
        MonsterLivingTime,      //怪物存活时间（每隔一分钟触发一次)
    }
}
