using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySQLDriverCS;
using System.Data;
using System.Windows;
using Server.Tools;
using System.Threading;
using Server.Data;
using GameDBServer.Logic;

namespace GameDBServer.DB
{
    /// <summary>
    /// 角色信息
    /// </summary>
    public class DBRoleInfo
    {
        #region 基本数据

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleID
        {
            get;
            set;
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID
        {
            get;
            set;
        }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName
        {
            get;
            set;
        }

        /// <summary>
        /// 角色性别
        /// </summary>
        public int RoleSex
        {
            get;
            set;
        }

        /// <summary>
        /// 角色职业(门派)
        /// </summary>
        public int Occupation
        {
            get;
            set;
        }

        /// <summary>
        /// 角色级别
        /// </summary>
        public int Level
        {
            get;
            set;
        }

        /// <summary>
        /// 角色头像
        /// </summary>
        public int RolePic
        {
            get;
            set;
        }

        /// <summary>
        /// 角色帮派
        /// </summary>
        public int Faction
        {
            get;
            set;
        }

        /// <summary>
        /// 角色的绑定钱币
        /// </summary>
        public int Money1
        {
            get;
            set;
        }

        /// <summary>
        /// 角色的非绑定钱币
        /// </summary>
        public int Money2
        {
            get;
            set;
        }

        /// <summary>
        /// 角色的当前经验
        /// </summary>
        public long Experience
        {
            get;
            set;
        }

        /// <summary>
        /// 角色的PK模式
        /// </summary>
        public int PKMode
        {
            get;
            set;
        }

        /// <summary>
        /// 角色的PK值
        /// </summary>
        public int PKValue
        {
            get;
            set;
        }

        /// <summary>
        /// 角色的位置
        /// </summary>
        public string Position
        {
            get;
            set;
        }

        /// <summary>
        /// 注册时间
        /// </summary>
        public string RegTime
        {
            get;
            set;
        }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public long LastTime
        {
            get;
            set;
        }

        /// <summary>
        /// 当前背包的页数(总个数 - 1)
        /// </summary>
        public int BagNum
        {
            get;
            set;
        }

        /// <summary>
        /// 称号
        /// </summary>
        public string OtherName
        {
            get;
            set;
        }

        /// <summary>
        /// 主快捷面板的映射
        /// </summary>
        public string MainQuickBarKeys
        {
            get;
            set;
        }

        /// <summary>
        /// 辅助快捷面板的映射
        /// </summary>
        public string OtherQuickBarKeys
        {
            get;
            set;
        }

        /// <summary>
        /// 登录的次数
        /// </summary>
        public int LoginNum
        {
            get;
            set;
        }

        /// <summary>
        /// 剩余的自动挂机时间
        /// </summary>
        public int LeftFightSeconds
        {
            get;
            set;
        }

        /// <summary>
        /// 线路ID
        /// </summary>
        public int ServerLineID
        {
            get;
            set;
        }

        /// <summary>
        /// 当前坐骑的ID
        /// </summary>
        public int HorseDbID
        {
            get;
            set;
        }

        /// <summary>
        /// 当前宠物的ID
        /// </summary>
        public int PetDbID
        {
            get;
            set;
        }

        /// <summary>
        /// 角色的内力值
        /// </summary>
        public int InterPower
        {
            get;
            set;
        }

        /// <summary>
        /// 总的在线秒数
        /// </summary>
        public int TotalOnlineSecs
        {
            get;
            set;
        }

        /// <summary>
        /// 防止沉迷在线秒数
        /// </summary>
        public int AntiAddictionSecs
        {
            get;
            set;
        }

        /// <summary>
        /// 上次离线时间
        /// </summary>
        public long LogOffTime
        {
            get;
            set;
        }

        /// <summary>
        /// 本次闭关开始的时间
        /// </summary>
        public long BiGuanTime
        {
            get;
            set;
        }

        /// <summary>
        /// 系统绑定的银两
        /// </summary>
        public int YinLiang
        {
            get;
            set;
        }

        /// <summary>
        ///  从别人冲脉获取的经验值(累加)
        /// </summary>
        public int TotalJingMaiExp
        {
            get;
            set;
        }

        /// <summary>
        ///  从别人冲脉获取的经验的次数
        /// </summary>
        public int JingMaiExpNum
        {
            get;
            set;
        }

        /// <summary>
        /// 上一次的坐骑ID
        /// </summary>
        public int LastHorseID
        {
            get;
            set;
        }

        /// <summary>
        /// 缺省的技能ID
        /// </summary>
        public int DefaultSkillID
        {
            get;
            set;
        }

        /// <summary>
        /// 自动补血喝药的百分比
        /// </summary>
        public int AutoLifeV
        {
            get;
            set;
        }

        /// <summary>
        /// 自动补蓝喝药的百分比
        /// </summary>
        public int AutoMagicV
        {
            get;
            set;
        }

        /// <summary>
        /// 自动增加熟练度的被动技能ID
        /// </summary>
        public int NumSkillID
        {
            get;
            set;
        }

        /// <summary>
        /// 已经完成的主线任务的ID
        /// </summary>
        public int MainTaskID
        {
            get;
            set;
        }

        /// <summary>
        /// 当前的PK点
        /// </summary>
        public int PKPoint
        {
            get;
            set;
        }

        /// <summary>
        /// 最高连斩数
        /// </summary>
        public int LianZhan
        {
            get;
            set;
        }

        /// <summary>
        /// 杀BOSS的总个数
        /// </summary>
        public int KillBoss
        {
            get;
            set;
        }

        /// <summary>
        /// 角斗场荣誉称号开始时间
        /// </summary>
        public long BattleNameStart
        {
            get;
            set;
        }

        /// <summary>
        /// 角斗场荣誉称号
        /// </summary>
        public int BattleNameIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 充值TaskID
        /// </summary>
        public int CZTaskID
        {
            get;
            set;
        }

        /// <summary>
        /// 角斗场称号次数
        /// </summary>
        public int BattleNum
        {
            get;
            set;
        }

        /// <summary>
        /// 英雄逐擂的层数
        /// </summary>
        public int HeroIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 登录日ID
        /// </summary>
        public int LoginDayID
        {
            get;
            set;
        }

        /// <summary>
        /// 登录日次数
        /// </summary>
        public int LoginDayNum
        {
            get;
            set;
        }

        /// <summary>
        /// 区ID
        /// </summary>
        public int ZoneID
        {
            get;
            set;
        }

        /// <summary>
        /// 帮会名称
        /// </summary>
        public string BHName
        {
            get;
            set;
        }

        /// <summary>
        /// 被邀请加入帮会时是否验证
        /// </summary>
        public int BHVerify
        {
            get;
            set;
        }

        /// <summary>
        /// 帮会职务
        /// </summary>
        public int BHZhiWu
        {
            get;
            set;
        }

        /// <summary>
        /// 帮会每日贡日ID1
        /// </summary>
        public int BGDayID1
        {
            get;
            set;
        }

        /// <summary>
        /// 帮会每日铜钱帮贡
        /// </summary>
        public int BGMoney
        {
            get;
            set;
        }

        /// <summary>
        /// 帮会每日贡日ID2
        /// </summary>
        public int BGDayID2
        {
            get;
            set;
        }

        /// <summary>
        /// 帮会每日道具帮贡
        /// </summary>
        public int BGGoods
        {
            get;
            set;
        }

        /// <summary>
        /// 帮会帮贡
        /// </summary>
        public int BangGong
        {
            get;
            set;
        }

        /// <summary>
        /// 是否皇后
        /// </summary>
        public int HuangHou
        {
            get;
            set;
        }

        /// <summary>
        /// 劫镖的日ID
        /// </summary>
        public int JieBiaoDayID
        {
            get;
            set;
        }

        /// <summary>
        /// 劫镖的日次数
        /// </summary>
        public int JieBiaoDayNum
        {
            get;
            set;
        }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// 上次的mailID
        /// </summary>
        public int LastMailID
        {
            get;
            set;
        }

        /// <summary>
        /// 单次奖励记录标志位
        /// </summary>
        public long OnceAwardFlag
        {
            get;
            set;
        }

        /// <summary>
        /// 系统绑定的金币
        /// </summary>
        public int Gold
        {
            get;
            set;
        }

        /// <summary>
        /// 永久禁止聊天
        /// </summary>
        public int BanChat
        {
            get;
            set;
        }

        /// <summary>
        /// 永久禁止登陆
        /// </summary>
        public int BanLogin
        {
            get;
            set;
        }

        // MU项目增加字段 [11/30/2013 LiaoWei]
        /// <summary>
        /// 新人标记
        /// </summary>
        public int IsFlashPlayer
        {
            get;
            set;
        }

        // MU项目增加字段 [12/10/2013 LiaoWei]
        /// <summary>
        /// 转生计数
        /// </summary>
        public int ChangeLifeCount
        {
            get;
            set;
        }

        // MU项目增加字段 [12/10/2013 LiaoWei]
        /// <summary>
        /// 被崇拜计数
        /// </summary>
        public int AdmiredCount
        {
            get;
            set;
        }

        //  MU项目增加字段 [12/17/2013 LiaoWei]
        /// <summary>
        /// 战斗力
        /// </summary>
        public int CombatForce
        {
            get;
            set;
        }

        // MU项目增加字段 [3/3/2014 LiaoWei]
        /// <summary>
        /// 自动分配属性点
        /// </summary>
        public int AutoAssignPropertyPoint
        {
            get;
            set;
        }

        // MU项目增加字段 [4/23/2014 LiaoWei]
        /// <summary>
        /// 消息推送ID
        /// </summary>
        public string PushMsgID
        {
            get;
            set;
        }

        // MU项目增加字段 [8/21/2014 LiaoWei]
        /// <summary>
        /// vip奖励领取标记
        /// </summary>
        public int VipAwardFlag
        {
            get;
            set;
        }

        /// <summary>
        /// VIP等级
        /// </summary>
        public int VIPLevel
        {
            get;
            set;
        }
        
        #endregion 基本数据

        #region 扩展数据

        /// <summary>
        /// 角色参数表
        /// </summary>
        public Dictionary<string, RoleParamsData> RoleParamsDict
        {
            get;
            set;
        }

        /// <summary>
        /// 数据库中的位置更新时间
        /// </summary>
        public long UpdateDBPositionTicks
        {
            get;
            set;
        }

        /// <summary>
        /// 数据库中的在线时长更新时间
        /// </summary>
        public long UpdateDBTimeTicks
        {
            get;
            set;
        }

        /// <summary>
        /// 数据库中的内力值更新时间
        /// </summary>
        public long UpdateDBInterPowerTimeTicks
        {
            get;
            set;
        }

        /// <summary>
        /// 已经完成的任务列表
        /// </summary>
        public List<OldTaskData> OldTasks
        {
            get;
            set;
        }

        /// <summary>
        /// 已经接受的任务列表
        /// </summary>
        public List<TaskData> DoingTaskList
        {
            get;
            set;
        }

        /// <summary>
        /// 已经获取的物品的列表
        /// </summary>
        public List<GoodsData> GoodsDataList
        {
            get;
            set;
        }

        /// <summary>
        /// 已经使用的物品限制列表
        /// </summary>
        public List<GoodsLimitData> GoodsLimitDataList
        {
            get;
            set;
        }

        /// <summary>
        /// 朋友数据
        /// </summary>
        public List<FriendData> FriendDataList
        {
            get;
            set;
        }

        /// <summary>
        /// 坐骑列表
        /// </summary>
        public List<HorseData> HorsesDataList
        {
            get;
            set;
        }

        /// <summary>
        /// 宠物列表
        /// </summary>
        public List<PetData> PetsDataList
        {
            get;
            set;
        }

        /// <summary>
        /// 点将积分数据的查询时间
        /// </summary>
        public long LastDJPointDataTikcs
        {
            get;
            set;
        }

        /// <summary>
        /// 点将积分数据
        /// </summary>
        public DJPointData RoleDJPointData
        {
            get;
            set;
        }

        /// 角色的经脉数据
        /// </summary>
        public List<JingMaiData> JingMaiDataList
        {
            get;
            set;
        }

        /// 角色的技能数据
        /// </summary>
        public List<SkillData> SkillDataList
        {
            get;
            set;
        }

        /// <summary>
        /// Buffer的数据列表
        /// </summary>
        public List<BufferData> BufferDataList
        {
            get;
            set;
        }

        /// <summary>
        /// 跑环任务的数据
        /// </summary>
        public List<DailyTaskData> MyDailyTaskDataList
        {
            get;
            set;
        }

        /// <summary>
        /// 每日冲穴的次数数据
        /// </summary>
        public DailyJingMaiData MyDailyJingMaiData
        {
            get;
            set;
        }

        /// <summary>
        /// 随身仓库数据
        /// </summary>
        public PortableBagData MyPortableBagData
        {
            get;
            set;
        }

        /// <summary>
        /// 活动送礼相关数据是否已经存在？
        /// </summary>
        public bool ExistsMyHuodongData
        {
            get;
            set;
        }

        /// <summary>
        /// 活动送礼相关数据
        /// </summary>
        public HuodongData MyHuodongData
        {
            get;
            set;
        }

        /// <summary>
        /// 副本数据
        /// </summary>
        public List<FuBenData> FuBenDataList
        {
            get;
            set;
        }

        /// <summary>
        /// 角色每日数据
        /// </summary>
        public RoleDailyData MyRoleDailyData
        {
            get;
            set;            
        }

        /// <summary>
        /// 押镖的数据
        /// </summary>
        public YaBiaoData MyYaBiaoData
        {
            get;
            set;
        }

        /// <summary>
        /// 上次使用访问的时间
        /// </summary>
        private long _LastReferenceTicks = DateTime.Now.Ticks / 10000;

        /// <summary>
        /// 上次使用访问的时间
        /// </summary>
        public long LastReferenceTicks
        {
            get { return _LastReferenceTicks; }
            set { _LastReferenceTicks = value; }
        }

        /// <summary>
        /// 自己在排行中的位置字典
        /// </summary>
        public Dictionary<int, int> PaiHangPosDict
        {
            get;
            set;
        }

        /// <summary>
        /// vip日数据
        /// </summary>
        public List<VipDailyData> VipDailyDataList
        {
            get;
            set;
        }

        public YangGongBKDailyJiFenData YangGongBKDailyJiFen
        {
            get;
            set;
        }

        /// <summary>
        /// 我的翅膀
        /// </summary>
        public WingData MyWingData
        {
            get;
            set;
        }

        /// <summary>
        /// 图鉴提交信息 [5/17/2014 LiaoWei]
        /// </summary>
        public Dictionary<int, int> PictureJudgeReferInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 星座信息 [8/1/2014 LiaoWei]
        /// </summary>
        public Dictionary<int, int> StarConstellationInfo
        {
            get;
            set;
        }

        #endregion 扩展数据

        #region 从数据库查询信息

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd, int index)
        {
            dbRoleInfo.RoleID = Convert.ToInt32(cmd.Table.Rows[index]["rid"]);
            dbRoleInfo.UserID = cmd.Table.Rows[index]["userid"].ToString();
            dbRoleInfo.RoleName = cmd.Table.Rows[index]["rname"].ToString();
            dbRoleInfo.RoleSex = Convert.ToInt32(cmd.Table.Rows[index]["sex"]);
            dbRoleInfo.Occupation = Convert.ToInt32(cmd.Table.Rows[index]["occupation"]);
            dbRoleInfo.Level = Convert.ToInt32(cmd.Table.Rows[index]["level"]);
            dbRoleInfo.RolePic = Convert.ToInt32(cmd.Table.Rows[index]["pic"]);
            dbRoleInfo.Faction = Convert.ToInt32(cmd.Table.Rows[index]["faction"]);
            dbRoleInfo.Money1 = Convert.ToInt32(cmd.Table.Rows[index]["money1"]);
            dbRoleInfo.Money2 = Convert.ToInt32(cmd.Table.Rows[index]["money2"]);
            dbRoleInfo.Experience = Convert.ToInt64(cmd.Table.Rows[index]["experience"]);
            dbRoleInfo.PKMode = Convert.ToInt32(cmd.Table.Rows[index]["pkmode"]);
            dbRoleInfo.PKValue = Convert.ToInt32(cmd.Table.Rows[index]["pkvalue"]);
            dbRoleInfo.Position = cmd.Table.Rows[index]["position"].ToString();
            dbRoleInfo.RegTime = cmd.Table.Rows[index]["regtime"].ToString();
            dbRoleInfo.LastTime = DataHelper.ConvertToTicks(cmd.Table.Rows[index]["lasttime"].ToString());
            dbRoleInfo.BagNum = Convert.ToInt32(cmd.Table.Rows[index]["bagnum"]);
            dbRoleInfo.OtherName = cmd.Table.Rows[index]["othername"].ToString();
            dbRoleInfo.MainQuickBarKeys = cmd.Table.Rows[index]["main_quick_keys"].ToString();
            dbRoleInfo.OtherQuickBarKeys = cmd.Table.Rows[index]["other_quick_keys"].ToString();
            dbRoleInfo.LoginNum = Convert.ToInt32(cmd.Table.Rows[index]["loginnum"].ToString());
            dbRoleInfo.LeftFightSeconds = Convert.ToInt32(cmd.Table.Rows[index]["leftfightsecs"].ToString());
            dbRoleInfo.HorseDbID = Convert.ToInt32(cmd.Table.Rows[index]["horseid"].ToString());
            dbRoleInfo.PetDbID = Convert.ToInt32(cmd.Table.Rows[index]["petid"].ToString());
            dbRoleInfo.InterPower = Convert.ToInt32(cmd.Table.Rows[index]["interpower"].ToString());
            dbRoleInfo.TotalOnlineSecs = Convert.ToInt32(cmd.Table.Rows[index]["totalonlinesecs"].ToString());
            dbRoleInfo.AntiAddictionSecs = Convert.ToInt32(cmd.Table.Rows[index]["antiaddictionsecs"].ToString());
            dbRoleInfo.LogOffTime = DataHelper.ConvertToTicks(cmd.Table.Rows[index]["logofftime"].ToString());
            dbRoleInfo.BiGuanTime = DataHelper.ConvertToTicks(cmd.Table.Rows[index]["biguantime"].ToString());
            dbRoleInfo.YinLiang = Convert.ToInt32(cmd.Table.Rows[index]["yinliang"].ToString());
            dbRoleInfo.TotalJingMaiExp = Convert.ToInt32(cmd.Table.Rows[index]["total_jingmai_exp"].ToString());
            dbRoleInfo.JingMaiExpNum = Convert.ToInt32(cmd.Table.Rows[index]["jingmai_exp_num"].ToString());
            dbRoleInfo.LastHorseID = Convert.ToInt32(cmd.Table.Rows[index]["lasthorseid"].ToString());
            dbRoleInfo.DefaultSkillID = Convert.ToInt32(cmd.Table.Rows[index]["skillid"].ToString());
            dbRoleInfo.AutoLifeV = Convert.ToInt32(cmd.Table.Rows[index]["autolife"].ToString());
            dbRoleInfo.AutoMagicV = Convert.ToInt32(cmd.Table.Rows[index]["automagic"].ToString());
            dbRoleInfo.NumSkillID = Convert.ToInt32(cmd.Table.Rows[index]["numskillid"].ToString());
            dbRoleInfo.MainTaskID = Convert.ToInt32(cmd.Table.Rows[index]["maintaskid"].ToString());
            dbRoleInfo.PKPoint = Convert.ToInt32(cmd.Table.Rows[index]["pkpoint"].ToString());
            dbRoleInfo.LianZhan = Convert.ToInt32(cmd.Table.Rows[index]["lianzhan"].ToString());
            dbRoleInfo.KillBoss = Convert.ToInt32(cmd.Table.Rows[index]["killboss"].ToString());
            dbRoleInfo.BattleNameStart = Convert.ToInt64(cmd.Table.Rows[index]["battlenamestart"].ToString());
            dbRoleInfo.BattleNameIndex = Convert.ToInt32(cmd.Table.Rows[index]["battlenameindex"].ToString());
            dbRoleInfo.CZTaskID = Convert.ToInt32(cmd.Table.Rows[index]["cztaskid"].ToString());
            dbRoleInfo.BattleNum = Convert.ToInt32(cmd.Table.Rows[index]["battlenum"].ToString());
            dbRoleInfo.HeroIndex = Convert.ToInt32(cmd.Table.Rows[index]["heroindex"].ToString());
            dbRoleInfo.LoginDayID = Convert.ToInt32(cmd.Table.Rows[index]["logindayid"].ToString());
            dbRoleInfo.LoginDayNum = Convert.ToInt32(cmd.Table.Rows[index]["logindaynum"].ToString());
            dbRoleInfo.ZoneID = Convert.ToInt32(cmd.Table.Rows[index]["zoneid"].ToString());
            dbRoleInfo.BHName = cmd.Table.Rows[index]["bhname"].ToString();
            dbRoleInfo.BHVerify = Convert.ToInt32(cmd.Table.Rows[index]["bhverify"].ToString());
            dbRoleInfo.BHZhiWu = Convert.ToInt32(cmd.Table.Rows[index]["bhzhiwu"].ToString());
            dbRoleInfo.BGDayID1 = Convert.ToInt32(cmd.Table.Rows[index]["bgdayid1"].ToString());
            dbRoleInfo.BGMoney = Convert.ToInt32(cmd.Table.Rows[index]["bgmoney"].ToString());
            dbRoleInfo.BGDayID2 = Convert.ToInt32(cmd.Table.Rows[index]["bgdayid2"].ToString());
            dbRoleInfo.BGGoods = Convert.ToInt32(cmd.Table.Rows[index]["bggoods"].ToString());
            dbRoleInfo.BangGong = Convert.ToInt32(cmd.Table.Rows[index]["banggong"].ToString());
            dbRoleInfo.HuangHou = Convert.ToInt32(cmd.Table.Rows[index]["huanghou"].ToString());
            dbRoleInfo.JieBiaoDayID = Convert.ToInt32(cmd.Table.Rows[index]["jiebiaodayid"].ToString());
            dbRoleInfo.JieBiaoDayNum = Convert.ToInt32(cmd.Table.Rows[index]["jiebiaonum"].ToString());
            dbRoleInfo.UserName = cmd.Table.Rows[index]["username"].ToString();
            dbRoleInfo.LastMailID = Convert.ToInt32(cmd.Table.Rows[index]["lastmailid"].ToString());
            dbRoleInfo.OnceAwardFlag = Convert.ToInt64(cmd.Table.Rows[index]["onceawardflag"].ToString());
            dbRoleInfo.Gold = Convert.ToInt32(cmd.Table.Rows[index]["money2"].ToString());
            dbRoleInfo.BanChat = Convert.ToInt32(cmd.Table.Rows[index]["banchat"].ToString());
            dbRoleInfo.BanLogin = Convert.ToInt32(cmd.Table.Rows[index]["banlogin"].ToString());
            dbRoleInfo.IsFlashPlayer = Convert.ToInt32(cmd.Table.Rows[index]["isflashplayer"].ToString());
            dbRoleInfo.ChangeLifeCount = Convert.ToInt32(cmd.Table.Rows[index]["changelifecount"].ToString());
            dbRoleInfo.AdmiredCount = Convert.ToInt32(cmd.Table.Rows[index]["admiredcount"].ToString());
            dbRoleInfo.CombatForce = Convert.ToInt32(cmd.Table.Rows[index]["combatforce"].ToString());
            dbRoleInfo.AutoAssignPropertyPoint = Convert.ToInt32(cmd.Table.Rows[index]["autoassignpropertypoint"].ToString());
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_角色参数表
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_Params(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                Dictionary<string, RoleParamsData> dict = new Dictionary<string, RoleParamsData>(cmd.Table.Rows.Count);
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    RoleParamsData roleParamsData = new RoleParamsData()
                    {
                        ParamName = cmd.Table.Rows[i]["pname"].ToString(),
                        ParamValue = cmd.Table.Rows[i]["pvalue"].ToString(),
                    };

                    dict[roleParamsData.ParamName] = roleParamsData;
                }

                dbRoleInfo.RoleParamsDict = dict;
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_旧任务数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_OldTasks(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                List<OldTaskData> oldTasks = new List<OldTaskData>(cmd.Table.Rows.Count);
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    oldTasks.Add(new OldTaskData()
                    {
                        TaskID = Convert.ToInt32(cmd.Table.Rows[i]["taskid"].ToString()),
                        DoCount = Convert.ToInt32(cmd.Table.Rows[i]["count"].ToString()),
                    });
                }

                dbRoleInfo.OldTasks = oldTasks;
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_正在做任务数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_DoingTasks(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.DoingTaskList = new List<TaskData>();
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    dbRoleInfo.DoingTaskList.Add(new TaskData()
                    {
                        DbID = Convert.ToInt32(cmd.Table.Rows[i]["id"].ToString()),
                        DoingTaskID = Convert.ToInt32(cmd.Table.Rows[i]["taskid"].ToString()),
                        DoingTaskVal1 = Convert.ToInt32(cmd.Table.Rows[i]["value1"].ToString()),
                        DoingTaskVal2 = Convert.ToInt32(cmd.Table.Rows[i]["value2"].ToString()),
                        DoingTaskFocus = Convert.ToInt32(cmd.Table.Rows[i]["focus"].ToString()),
                        AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["addtime"].ToString()),
                        StarLevel = Convert.ToInt32(cmd.Table.Rows[i]["starlevel"].ToString()),
                    });
                }
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_物品数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_Goods(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.GoodsDataList = new List<GoodsData>();
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    byte[] props = Convert.FromBase64String(cmd.Table.Rows[i]["ehinfo"].ToString());
                    List<int> list = DataHelper.BytesToObject<List<int>>(props, 0, props.Length);
                    dbRoleInfo.GoodsDataList.Add(new GoodsData()
                    {
                        Id = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
                        GoodsID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString()),
                        Using = Convert.ToInt32(cmd.Table.Rows[i]["isusing"].ToString()),
                        Forge_level = Convert.ToInt32(cmd.Table.Rows[i]["forge_level"].ToString()),
                        Starttime = cmd.Table.Rows[i]["starttime"].ToString(),
                        Endtime = cmd.Table.Rows[i]["endtime"].ToString(),
                        Site = Convert.ToInt32(cmd.Table.Rows[i]["site"].ToString()),
                        Quality = Convert.ToInt32(cmd.Table.Rows[i]["quality"].ToString()),
                        Props = cmd.Table.Rows[i]["Props"].ToString(),
                        GCount = Convert.ToInt32(cmd.Table.Rows[i]["gcount"].ToString()),
                        Binding = Convert.ToInt32(cmd.Table.Rows[i]["binding"].ToString()),
                        Jewellist = cmd.Table.Rows[i]["jewellist"].ToString(),
                        BagIndex = Convert.ToInt32(cmd.Table.Rows[i]["bagindex"].ToString()),
                        SaleMoney1 = Convert.ToInt32(cmd.Table.Rows[i]["salemoney1"].ToString()),
                        SaleYuanBao = Convert.ToInt32(cmd.Table.Rows[i]["saleyuanbao"].ToString()),
                        SaleYinPiao = Convert.ToInt32(cmd.Table.Rows[i]["saleyinpiao"].ToString()),
                        AddPropIndex = Convert.ToInt32(cmd.Table.Rows[i]["addpropindex"].ToString()),
                        BornIndex = Convert.ToInt32(cmd.Table.Rows[i]["bornindex"].ToString()),
                        Lucky = Convert.ToInt32(cmd.Table.Rows[i]["lucky"].ToString()),
                        Strong = Convert.ToInt32(cmd.Table.Rows[i]["strong"].ToString()),
                        ExcellenceInfo = Convert.ToInt32(cmd.Table.Rows[i]["excellenceinfo"].ToString()),
                        AppendPropLev = Convert.ToInt32(cmd.Table.Rows[i]["appendproplev"].ToString()),
                        ChangeLifeLevForEquip = Convert.ToInt32(cmd.Table.Rows[i]["equipchangelife"].ToString()),
                        ElementhrtsProps = list,
                    });
                }
            }
        }

        public static void DBTableRow2RoleInfo_GoodsProps(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0 && dbRoleInfo.GoodsDataList.Count > 0)
            {
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    int id = Convert.ToInt32(cmd.Table.Rows[i]["id"]);
                    int type = Convert.ToInt32(cmd.Table.Rows[i]["type"]);
                    DataRow row = cmd.Table.Rows[i];
                    byte[] props = Convert.FromBase64String(cmd.Table.Rows[i]["props"].ToString());
                    List<int> list = DataHelper.BytesToObject<List<int>>(props, 0, props.Length);

                    for (int j = 0; j < dbRoleInfo.GoodsDataList.Count; j++)
                    {
                        if (dbRoleInfo.GoodsDataList[j].Id == id)
                        {
                            if (type == (int)UpdatePropIndexes.WashProps)
                            {
                                dbRoleInfo.GoodsDataList[j].WashProps = list;
                            }

                            if (type == (int)UpdatePropIndexes.ElementhrtsProps)
                            {
                                dbRoleInfo.GoodsDataList[j].ElementhrtsProps = list;
                            }

                            break;
                        }
                    }

                    //while (j < dbRoleInfo.GoodsDataList.Count)
                    //{
                    //    if (dbRoleInfo.GoodsDataList[j].Id < id)
                    //    {
                    //        j++;
                    //    }
                    //    else if (dbRoleInfo.GoodsDataList[j].Id == id)
                    //    {
                    //        if (type == (int)UpdatePropIndexes.WashProps)
                    //        {
                    //            dbRoleInfo.GoodsDataList[j].WashProps = list;
                    //        }
                    //        break;
                    //    }
                    //    else //没有找到
                    //    {
                    //        j = 0;
                    //        break;
                    //    }
                    //}

                }
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_物品限制数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_GoodsLimit(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.GoodsLimitDataList = new List<GoodsLimitData>();
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    dbRoleInfo.GoodsLimitDataList.Add(new GoodsLimitData()
                    {
                        GoodsID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString()),
                        DayID = Convert.ToInt32(cmd.Table.Rows[i]["dayid"].ToString()),
                        UsedNum = Convert.ToInt32(cmd.Table.Rows[i]["usednum"].ToString()),
                    });
                }
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_好友数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_Friends(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.FriendDataList = new List<FriendData>();
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    dbRoleInfo.FriendDataList.Add(new FriendData()
                    {
                        DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
                        OtherRoleID = Convert.ToInt32(cmd.Table.Rows[i]["otherid"].ToString()),
                        FriendType = Convert.ToInt32(cmd.Table.Rows[i]["friendType"].ToString()),
                    });
                }
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_坐骑数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_Horses(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.HorsesDataList = new List<HorseData>();
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    dbRoleInfo.HorsesDataList.Add(new HorseData()
                    {
                        DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
                        HorseID = Convert.ToInt32(cmd.Table.Rows[i]["horseid"].ToString()),
                        BodyID = Convert.ToInt32(cmd.Table.Rows[i]["bodyid"].ToString()),
                        PropsNum = cmd.Table.Rows[i]["propsNum"].ToString(),
                        PropsVal = cmd.Table.Rows[i]["PropsVal"].ToString(),
                        AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["addtime"].ToString()),
                        JinJieFailedNum = Convert.ToInt32(cmd.Table.Rows[i]["failednum"].ToString()),
                        JinJieTempTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["temptime"].ToString()),
                        JinJieTempNum = Convert.ToInt32(cmd.Table.Rows[i]["tempnum"].ToString()),
                        JinJieFailedDayID = Convert.ToInt32(cmd.Table.Rows[i]["faileddayid"].ToString()),
                    });
                }
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_宠物数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_Pets(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.PetsDataList = new List<PetData>();
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    dbRoleInfo.PetsDataList.Add(new PetData()
                    {
                        DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
                        PetID = Convert.ToInt32(cmd.Table.Rows[i]["petid"].ToString()),
                        PetName = cmd.Table.Rows[i]["petname"].ToString(),
                        PetType = Convert.ToInt32(cmd.Table.Rows[i]["pettype"].ToString()),
                        FeedNum = Convert.ToInt32(cmd.Table.Rows[i]["feednum"].ToString()),
                        ReAliveNum = Convert.ToInt32(cmd.Table.Rows[i]["realivenum"].ToString()),
                        AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["addtime"].ToString()),
                        PetProps = cmd.Table.Rows[i]["props"].ToString(),
                        Level = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString()),
                    });
                }
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_经脉数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_JingMais(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.JingMaiDataList = new List<JingMaiData>();
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    dbRoleInfo.JingMaiDataList.Add(new JingMaiData()
                    {
                        DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
                        JingMaiID = Convert.ToInt32(cmd.Table.Rows[i]["jmid"].ToString()),
                        JingMaiLevel = Convert.ToInt32(cmd.Table.Rows[i]["jmlevel"].ToString()),
                        JingMaiBodyLevel = Convert.ToInt32(cmd.Table.Rows[i]["bodylevel"].ToString()),
                    });
                }
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_技能数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_Skills(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.SkillDataList = new List<SkillData>();
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    dbRoleInfo.SkillDataList.Add(new SkillData()
                    {
                        DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
                        SkillID = Convert.ToInt32(cmd.Table.Rows[i]["skillid"].ToString()),
                        SkillLevel = Convert.ToInt32(cmd.Table.Rows[i]["skilllevel"].ToString()),
                        UsedNum = Convert.ToInt32(cmd.Table.Rows[i]["usednum"].ToString()),
                    });
                }
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_Buffer数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_Buffers(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.BufferDataList = new List<BufferData>();
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    dbRoleInfo.BufferDataList.Add(new BufferData()
                    {
                        BufferID = Convert.ToInt32(cmd.Table.Rows[i]["bufferid"].ToString()),
                        StartTime = Convert.ToInt64(cmd.Table.Rows[i]["starttime"].ToString()),
                        BufferSecs = Convert.ToInt32(cmd.Table.Rows[i]["buffersecs"].ToString()),
                        BufferVal = Convert.ToInt64(cmd.Table.Rows[i]["bufferval"].ToString()),
                        BufferType = 0,
                    });
                }
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_跑环任务数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_DailyTasks(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (null == dbRoleInfo.MyDailyTaskDataList)
            {
                dbRoleInfo.MyDailyTaskDataList = new List<DailyTaskData>();
            }

            for (int i = 0; i < cmd.Table.Rows.Count; i++)
            {
                DailyTaskData dailyTaskData = new DailyTaskData()
                {
                    HuanID = Convert.ToInt32(cmd.Table.Rows[i]["huanid"].ToString()),
                    RecTime = cmd.Table.Rows[i]["rectime"].ToString(),
                    RecNum = Convert.ToInt32(cmd.Table.Rows[i]["recnum"].ToString()),
                    TaskClass = Convert.ToInt32(cmd.Table.Rows[i]["taskClass"].ToString()),
                    ExtDayID = Convert.ToInt32(cmd.Table.Rows[i]["extdayid"].ToString()),
                    ExtNum = Convert.ToInt32(cmd.Table.Rows[i]["extnum"].ToString()),
                };

                dbRoleInfo.MyDailyTaskDataList.Add(dailyTaskData);
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_每日冲穴次数数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_DailyJingMai(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.MyDailyJingMaiData = new DailyJingMaiData()
                {
                    JmTime = cmd.Table.Rows[0]["jmtime"].ToString(),
                    JmNum = Convert.ToInt32(cmd.Table.Rows[0]["jmnum"].ToString()),
                };
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_随身仓库数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_PortableBag(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            dbRoleInfo.MyPortableBagData = new PortableBagData()
            {
                GoodsUsedGridNum = 0,
            };

            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.MyPortableBagData.ExtGridNum = Convert.ToInt32(cmd.Table.Rows[0]["extgridnum"].ToString());
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_送礼活动数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_HuodongData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            dbRoleInfo.ExistsMyHuodongData = false;
            dbRoleInfo.MyHuodongData = new HuodongData();

            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.ExistsMyHuodongData = true;

                dbRoleInfo.MyHuodongData.LastWeekID = cmd.Table.Rows[0]["loginweekid"].ToString();
                dbRoleInfo.MyHuodongData.LastDayID = cmd.Table.Rows[0]["logindayid"].ToString();
                dbRoleInfo.MyHuodongData.LoginNum = Convert.ToInt32(cmd.Table.Rows[0]["loginnum"].ToString());
                dbRoleInfo.MyHuodongData.NewStep = Convert.ToInt32(cmd.Table.Rows[0]["newstep"].ToString());
                dbRoleInfo.MyHuodongData.StepTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["steptime"].ToString());
                dbRoleInfo.MyHuodongData.LastMTime = Convert.ToInt32(cmd.Table.Rows[0]["lastmtime"].ToString());
                dbRoleInfo.MyHuodongData.CurMID = cmd.Table.Rows[0]["curmid"].ToString();
                dbRoleInfo.MyHuodongData.CurMTime = Convert.ToInt32(cmd.Table.Rows[0]["curmtime"].ToString());
                dbRoleInfo.MyHuodongData.SongLiID = Convert.ToInt32(cmd.Table.Rows[0]["songliid"].ToString());
                dbRoleInfo.MyHuodongData.LoginGiftState = Convert.ToInt32(cmd.Table.Rows[0]["logingiftstate"].ToString());
                dbRoleInfo.MyHuodongData.OnlineGiftState = Convert.ToInt32(cmd.Table.Rows[0]["onlinegiftstate"].ToString());
                dbRoleInfo.MyHuodongData.LastLimitTimeHuoDongID = Convert.ToInt32(cmd.Table.Rows[0]["lastlimittimehuodongid"].ToString());
                dbRoleInfo.MyHuodongData.LastLimitTimeDayID = Convert.ToInt32(cmd.Table.Rows[0]["lastlimittimedayid"].ToString());
                dbRoleInfo.MyHuodongData.LimitTimeLoginNum = Convert.ToInt32(cmd.Table.Rows[0]["limittimeloginnum"].ToString());
                dbRoleInfo.MyHuodongData.LimitTimeGiftState = Convert.ToInt32(cmd.Table.Rows[0]["limittimegiftstate"].ToString());
                dbRoleInfo.MyHuodongData.EveryDayOnLineAwardStep = Convert.ToInt32(cmd.Table.Rows[0]["everydayonlineawardstep"].ToString());
                dbRoleInfo.MyHuodongData.GetEveryDayOnLineAwardDayID = Convert.ToInt32(cmd.Table.Rows[0]["geteverydayonlineawarddayid"].ToString());
                dbRoleInfo.MyHuodongData.SeriesLoginGetAwardStep = Convert.ToInt32(cmd.Table.Rows[0]["serieslogingetawardstep"].ToString());
                dbRoleInfo.MyHuodongData.SeriesLoginAwardDayID = Convert.ToInt32(cmd.Table.Rows[0]["seriesloginawarddayid"].ToString());
                dbRoleInfo.MyHuodongData.SeriesLoginAwardGoodsID = cmd.Table.Rows[0]["seriesloginawardgoodsid"].ToString();
                dbRoleInfo.MyHuodongData.EveryDayOnLineAwardGoodsID = cmd.Table.Rows[0]["everydayonlineawardgoodsid"].ToString();
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_副本数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_FuBenData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.FuBenDataList = new List<FuBenData>();
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    dbRoleInfo.FuBenDataList.Add(new FuBenData()
                    {
                        FuBenID = Convert.ToInt32(cmd.Table.Rows[i]["fubenid"].ToString()),
                        DayID = Convert.ToInt32(cmd.Table.Rows[i]["dayid"].ToString()),
                        EnterNum = Convert.ToInt32(cmd.Table.Rows[i]["enternum"].ToString()),
                        QuickPassTimer = Convert.ToInt32(cmd.Table.Rows[i]["quickpasstimer"].ToString()),
                        FinishNum = Convert.ToInt32(cmd.Table.Rows[i]["finishnum"].ToString()),
                    });
                }
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_日常数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_DailyData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.MyRoleDailyData = new RoleDailyData()
                {
                    ExpDayID = Convert.ToInt32(cmd.Table.Rows[0]["expdayid"].ToString()),
                    TodayExp = Convert.ToInt32(cmd.Table.Rows[0]["todayexp"].ToString()),
                    LingLiDayID = Convert.ToInt32(cmd.Table.Rows[0]["linglidayid"].ToString()),
                    TodayLingLi = Convert.ToInt32(cmd.Table.Rows[0]["todaylingli"].ToString()),
                    KillBossDayID = Convert.ToInt32(cmd.Table.Rows[0]["killbossdayid"].ToString()),
                    TodayKillBoss = Convert.ToInt32(cmd.Table.Rows[0]["todaykillboss"].ToString()),
                    FuBenDayID = Convert.ToInt32(cmd.Table.Rows[0]["fubendayid"].ToString()),
                    TodayFuBenNum = Convert.ToInt32(cmd.Table.Rows[0]["todayfubennum"].ToString()),
                    WuXingDayID = Convert.ToInt32(cmd.Table.Rows[0]["wuxingdayid"].ToString()),
                    WuXingNum = Convert.ToInt32(cmd.Table.Rows[0]["wuxingnum"].ToString()),
                };
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_押镖数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_YaBiaoData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.MyYaBiaoData = new YaBiaoData()
                {
                    YaBiaoID = Convert.ToInt32(cmd.Table.Rows[0]["yabiaoid"].ToString()),
                    StartTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["starttime"].ToString()),
                    State = Convert.ToInt32(cmd.Table.Rows[0]["state"].ToString()),
                    LineID = Convert.ToInt32(cmd.Table.Rows[0]["lineid"].ToString()),
                    TouBao = Convert.ToInt32(cmd.Table.Rows[0]["toubao"].ToString()),
                    YaBiaoDayID = Convert.ToInt32(cmd.Table.Rows[0]["yabiaodayid"].ToString()),
                    YaBiaoNum = Convert.ToInt32(cmd.Table.Rows[0]["yabiaonum"].ToString()),
                    TakeGoods = Convert.ToInt32(cmd.Table.Rows[0]["takegoods"].ToString()),
                };
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_vip日常数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_VipDailyData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.VipDailyDataList = new List<VipDailyData>();
                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    dbRoleInfo.VipDailyDataList.Add(new VipDailyData()
                    {
                        PriorityType = Convert.ToInt32(cmd.Table.Rows[i]["prioritytype"].ToString()),
                        DayID = Convert.ToInt32(cmd.Table.Rows[i]["dayid"].ToString()),
                        UsedTimes = Convert.ToInt32(cmd.Table.Rows[i]["usedtimes"].ToString()),
                    });
                }
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_杨公宝库积分日数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_YangGongBKDailyJiFenData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.YangGongBKDailyJiFen = new YangGongBKDailyJiFenData()
                {
                    JiFen = Convert.ToInt32(cmd.Table.Rows[0]["jifen"].ToString()),
                    DayID = Convert.ToInt32(cmd.Table.Rows[0]["dayid"].ToString()),
                    AwardHistory = Convert.ToInt64(cmd.Table.Rows[0]["awardhistory"].ToString()),
                };
            }
        }

        /// <summary>
        /// 将数据库中获取的数据转换为角色数据_翅膀数据
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_Wings(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                dbRoleInfo.MyWingData = new WingData()
                {
                    DbID = Convert.ToInt32(cmd.Table.Rows[0]["Id"].ToString()),
                    WingID = Convert.ToInt32(cmd.Table.Rows[0]["wingid"].ToString()),
                    ForgeLevel = Convert.ToInt32(cmd.Table.Rows[0]["forgeLevel"].ToString()),
                    AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["addtime"].ToString()),
                    JinJieFailedNum = Convert.ToInt32(cmd.Table.Rows[0]["failednum"].ToString()),
                    Using = Convert.ToInt32(cmd.Table.Rows[0]["equiped"].ToString()),
                    StarExp = Convert.ToInt32(cmd.Table.Rows[0]["starexp"].ToString())
                };
            }
        }

        /// <summary>
        /// 角色图鉴提交信息 [5/17/2014 LiaoWei]
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_picturejudgeinfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                Dictionary<int, int> Tmpdict = new Dictionary<int, int>(cmd.Table.Rows.Count);

                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    int nID         = -1;
                    int nNum        = -1;

                    nID             = Convert.ToInt32(cmd.Table.Rows[i]["picturejudgeid"].ToString());
                    nNum            = Convert.ToInt32(cmd.Table.Rows[i]["refercount"].ToString());

                    Tmpdict[nID]    = nNum;
                }

                dbRoleInfo.PictureJudgeReferInfo = Tmpdict;
            }
        }

        /// <summary>
        /// 角色星座信息 [5/17/2014 LiaoWei]
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        public static void DBTableRow2RoleInfo_starconstellationinfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
        {
            if (cmd.Table.Rows.Count > 0)
            {
                Dictionary<int, int> Tmpdict = new Dictionary<int, int>(cmd.Table.Rows.Count);

                for (int i = 0; i < cmd.Table.Rows.Count; i++)
                {
                    int nStarsiteid = -1;
                    int nStarslotid = -1;

                    nStarsiteid = Convert.ToInt32(cmd.Table.Rows[i]["starsiteid"].ToString());
                    nStarslotid = Convert.ToInt32(cmd.Table.Rows[i]["starslotid"].ToString());

                    Tmpdict[nStarsiteid] = nStarslotid;
                }

                dbRoleInfo.StarConstellationInfo = Tmpdict;
            }
        }

        /// <summary>
        /// 从数据库中查询
        /// </summary>
        /// <param name="bUseIsdel">是否判断是删除的角色</param>
        public bool Query(MySQLConnection conn, int roleID, bool bUseIsdel = true)
        {
            LogManager.WriteLog(LogTypes.Info, string.Format("从数据库加载角色数据: {0}", roleID));

            MySQLSelectCommand cmd = null;

            // 已经删除的角色不能返回
            if (bUseIsdel)
            {
                cmd = new MySQLSelectCommand(conn,
                     new string[] { "rid", "userid", "rname", "sex", "occupation", "level", "pic", "faction", "money1", "money2", "experience", "pkmode", "pkvalue", "position", "regtime", "lasttime", "bagnum", "othername", "main_quick_keys", "other_quick_keys", "loginnum", "leftfightsecs", "horseid", "petid", "interpower", "totalonlinesecs", "antiaddictionsecs", "logofftime", "biguantime", "yinliang", "total_jingmai_exp", "jingmai_exp_num", "lasthorseid", "skillid", "autolife", "automagic", "numskillid", "maintaskid", "pkpoint", "lianzhan", "killboss", "battlenamestart", "battlenameindex", "cztaskid", "battlenum", "heroindex", "logindayid", "logindaynum", "zoneid", "bhname", "bhverify", "bhzhiwu", "bgdayid1", "bgmoney", "bgdayid2", "bggoods", "banggong", "huanghou", "jiebiaodayid", "jiebiaonum", "username", "lastmailid", "onceawardflag", "banchat", "banlogin", "isflashplayer", "changelifecount", "admiredcount", "combatforce", "autoassignpropertypoint" },
                     new string[] { "t_roles" }, new object[,] { { "rid", "=", roleID }, { "isdel", "=", 0 } }, null, null);
            }
            else
            {
                cmd = new MySQLSelectCommand(conn,
                     new string[] { "rid", "userid", "rname", "sex", "occupation", "level", "pic", "faction", "money1", "money2", "experience", "pkmode", "pkvalue", "position", "regtime", "lasttime", "bagnum", "othername", "main_quick_keys", "other_quick_keys", "loginnum", "leftfightsecs", "horseid", "petid", "interpower", "totalonlinesecs", "antiaddictionsecs", "logofftime", "biguantime", "yinliang", "total_jingmai_exp", "jingmai_exp_num", "lasthorseid", "skillid", "autolife", "automagic", "numskillid", "maintaskid", "pkpoint", "lianzhan", "killboss", "battlenamestart", "battlenameindex", "cztaskid", "battlenum", "heroindex", "logindayid", "logindaynum", "zoneid", "bhname", "bhverify", "bhzhiwu", "bgdayid1", "bgmoney", "bgdayid2", "bggoods", "banggong", "huanghou", "jiebiaodayid", "jiebiaonum", "username", "lastmailid", "onceawardflag", "banchat", "banlogin", "isflashplayer", "changelifecount", "admiredcount", "combatforce", "autoassignpropertypoint" },
                     new string[] { "t_roles" }, new object[,] { { "rid", "=", roleID } }, null, null);
            }

            if (cmd.Table.Rows.Count <= 0)
            {
                return false;
            }

            /// 将数据库中获取的数据转换为角色数据
            DBRoleInfo.DBTableRow2RoleInfo(this, cmd, 0);

            //查询已经完成的任务信息
            cmd = new MySQLSelectCommand(conn,
                 new string[] { "pname", "pvalue" },
                 new string[] { "t_roleparams" }, new object[,] { { "rid", "=", roleID } }, null, null);

            /// 将数据库中获取的数据转换为角色数据_角色参数表
            DBRoleInfo.DBTableRow2RoleInfo_Params(this, cmd);

            //查询已经完成的任务信息
            cmd = new MySQLSelectCommand(conn,
                 new string[] { "rid", "taskid", "count" },
                 new string[] { "t_taskslog" }, new object[,] { { "rid", "=", roleID } }, null, null);

            /// 将数据库中获取的数据转换为角色数据_旧任务数据
            DBRoleInfo.DBTableRow2RoleInfo_OldTasks(this, cmd);

            //查询正在做的任务信息
            cmd = new MySQLSelectCommand(conn,
                 new string[] { "Id", "rid", "taskid", "focus", "value1", "value2", "addtime", "starlevel" },
                 new string[] { "t_tasks" }, new object[,] { { "rid", "=", roleID }, { "isdel", "=", 0} }, null, null);

            /// 将数据库中获取的数据转换为角色数据_正在做任务数据
            DBRoleInfo.DBTableRow2RoleInfo_DoingTasks(this, cmd);

            //查询已经获取的物品列表信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "id", "goodsid", "isusing", "forge_level", "starttime", "endtime", "site", "quality", "Props", "gcount", "binding", "jewellist", "bagindex", "salemoney1", "saleyuanbao", "saleyinpiao", "addpropindex", "bornindex", "lucky", "strong", "excellenceinfo", "appendproplev", "equipchangelife", "ehinfo" },
                     new string[] { "t_goods" }, new object[,] { { "rid", "=", roleID }, { "gcount", ">", 0 } }, null, new string[,] {{"id", "asc"}});

            /// 将数据库中获取的数据转换为角色数据_物品数据
            DBRoleInfo.DBTableRow2RoleInfo_Goods(this, cmd);

            cmd = new MySQLSelectCommand(conn,
                     new string[] { "id", "rid", "type", "props", "isdel" },
                     new string[] { "t_goodsprops" }, new object[,] { { "rid", "=", roleID }, { "isdel", "=", 0 } }, null, new string[,] { { "id", "asc" }});

            DBRoleInfo.DBTableRow2RoleInfo_GoodsProps(this, cmd);

            //查询已经获取的物品限制列表信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "goodsid", "dayid", "usednum" },
                     new string[] { "t_goodslimit" }, new object[,] { { "rid", "=", roleID } }, null, null);

            /// 将数据库中获取的数据转换为角色数据_物品数据
            DBRoleInfo.DBTableRow2RoleInfo_GoodsLimit(this, cmd);

            //查询已经学习的朋友列表信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "Id", "otherid", "friendType" },
                     new string[] { "t_friends" }, new object[,] { { "myid", "=", roleID } }, null, null);

            /// 将数据库中获取的数据转换为角色数据_好友数据
            DBRoleInfo.DBTableRow2RoleInfo_Friends(this, cmd);

            //查询坐骑的列表信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "Id", "horseid", "bodyid", "propsNum", "PropsVal", "addtime", "failednum", "temptime", "tempnum", "faileddayid" },
                     new string[] { "t_horses" }, new object[,] { { "rid", "=", roleID }, { "isdel", "=", 0 } }, null, null);

            /// 将数据库中获取的数据转换为角色数据_坐骑数据
            DBRoleInfo.DBTableRow2RoleInfo_Horses(this, cmd);

            //查询宠物的列表信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "Id", "petid", "petname", "pettype", "feednum", "realivenum", "addtime", "props", "level" },
                     new string[] { "t_pets" }, new object[,] { { "rid", "=", roleID }, { "isdel", "=", 0 } }, null, null);

            /// 将数据库中获取的数据转换为角色数据_宠物数据
            DBRoleInfo.DBTableRow2RoleInfo_Pets(this, cmd);

            //查询经脉的列表信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "Id", "jmid", "jmlevel", "bodylevel" },
                     new string[] { "t_jingmai" }, new object[,] { { "rid", "=", roleID } }, null, null);

            /// 将数据库中获取的数据转换为角色数据_经脉数据
            DBRoleInfo.DBTableRow2RoleInfo_JingMais(this, cmd);

            //查询经脉的列表信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "Id", "skillid", "skilllevel", "usednum" },
                     new string[] { "t_skills" }, new object[,] { { "rid", "=", roleID } }, null, null);

            /// 将数据库中获取的数据转换为角色数据_技能数据
            DBRoleInfo.DBTableRow2RoleInfo_Skills(this, cmd);

            //查询Buffer的列表信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "bufferid", "starttime", "buffersecs", "bufferval" },
                     new string[] { "t_buffer" }, new object[,] { { "rid", "=", roleID } }, null, null);

            /// 将数据库中获取的数据转换为角色数据_Buffer数据
            DBRoleInfo.DBTableRow2RoleInfo_Buffers(this, cmd);

            //查询跑环任务的列表信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "huanid", "rectime", "recnum", "taskClass", "extdayid", "extnum" },
                     new string[] { "t_dailytasks" }, new object[,] { { "rid", "=", roleID } }, null, null);

            /// 将数据库中获取的数据转换为角色数据_跑环任务数据
            DBRoleInfo.DBTableRow2RoleInfo_DailyTasks(this, cmd);

            //查询每日冲穴次数的信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "jmtime", "jmnum" },
                     new string[] { "t_dailyjingmai" }, new object[,] { { "rid", "=", roleID } }, null, null);

            /// 将数据库中获取的数据转换为角色数据_跑环任务数据
            DBRoleInfo.DBTableRow2RoleInfo_DailyJingMai(this, cmd);

            //查询随身仓库的信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "extgridnum" },
                     new string[] { "t_ptbag" }, new object[,] { { "rid", "=", roleID } }, null, null);

            //将数据库中获取的数据转换为角色数据_随身仓库数据
            DBRoleInfo.DBTableRow2RoleInfo_PortableBag(this, cmd);

            //查询角色活动送礼的信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "loginweekid", "logindayid", "loginnum", "newstep", "steptime", "lastmtime", "curmid", "curmtime", "songliid", "logingiftstate", "onlinegiftstate", "lastlimittimehuodongid", "lastlimittimedayid", "limittimeloginnum", "limittimegiftstate", "everydayonlineawardstep", "geteverydayonlineawarddayid", "serieslogingetawardstep", "seriesloginawarddayid", "seriesloginawardgoodsid", "everydayonlineawardgoodsid"},
                     new string[] { "t_huodong" }, new object[,] { { "rid", "=", roleID } }, null, null);

            //将数据库中获取的数据转换为角色数据_送礼活动数据
            DBRoleInfo.DBTableRow2RoleInfo_HuodongData(this, cmd);

            //查询角色副本信息 --  修正注释 [11/15/2013 LiaoWei]
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "fubenid", "dayid", "enternum", "quickpasstimer", "finishnum" },
                     new string[] { "t_fuben" }, new object[,] { { "rid", "=", roleID } }, null, null);

            //将数据库中获取的数据转换为角色数据_副本数据
            DBRoleInfo.DBTableRow2RoleInfo_FuBenData(this, cmd);

            //查询角色日常数据的信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "expdayid", "todayexp", "linglidayid", "todaylingli", "killbossdayid", "todaykillboss", "fubendayid", "todayfubennum", "wuxingdayid", "wuxingnum" },
                     new string[] { "t_dailydata" }, new object[,] { { "rid", "=", roleID } }, null, null);

            //将数据库中获取的数据转换为角色数据_日常数据
            DBRoleInfo.DBTableRow2RoleInfo_DailyData(this, cmd);

            //查询角色押镖数据的信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "yabiaoid", "starttime", "state", "lineid", "toubao", "yabiaodayid", "yabiaonum", "takegoods" },
                     new string[] { "t_yabiao" }, new object[,] { { "rid", "=", roleID } }, null, null);

            //将数据库中获取的数据转换为角色数据_押镖数据
            DBRoleInfo.DBTableRow2RoleInfo_YaBiaoData(this, cmd);

            //vip每日数据
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "rid", "prioritytype", "dayid", "usedtimes" },
                     new string[] { "t_vipdailydata" }, new object[,] { { "rid", "=", roleID } }, null, null);

            //将数据库中获取的数据转换为角色数据_日常数据
            DBRoleInfo.DBTableRow2RoleInfo_VipDailyData(this, cmd);

            //杨公宝库每日数据
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "rid", "jifen", "dayid", "awardhistory" },
                     new string[] { "t_yangguangbkdailydata" }, new object[,] { { "rid", "=", roleID } }, null, null);

            //将数据库中获取的数据转换为角色数据_日常数据
            DBRoleInfo.DBTableRow2RoleInfo_YangGongBKDailyJiFenData(this, cmd);

            //查询翅膀的列表信息
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "Id", "wingid", "forgeLevel", "addtime", "failednum", "equiped", "starexp" },
                     new string[] { "t_wings" }, new object[,] { { "rid", "=", roleID }, { "isdel", "=", 0 } }, null, null);

            /// 将数据库中获取的数据转换为角色数据_翅膀数据
            DBRoleInfo.DBTableRow2RoleInfo_Wings(this, cmd);

            // 角色图鉴提交信息 [5/17/2014 LiaoWei]
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "Id", "roleid", "picturejudgeid", "refercount" },
                     new string[] { "t_picturejudgeinfo" }, new object[,] { { "roleid", "=", roleID } }, null, null);

            DBRoleInfo.DBTableRow2RoleInfo_picturejudgeinfo(this, cmd);

            // 角色星座信息 [8/1/2014 LiaoWei]
            cmd = new MySQLSelectCommand(conn,
                     new string[] { "Id", "roleid", "starsiteid", "starslotid" },
                     new string[] { "t_starconstellationinfo" }, new object[,] { { "roleid", "=", roleID } }, null, null);

            DBRoleInfo.DBTableRow2RoleInfo_starconstellationinfo(this, cmd);

            cmd = null;
            return true;
        }

        #endregion 从数据库查询信息
    }
}
