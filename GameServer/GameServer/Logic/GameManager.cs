using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Task.Tool;
//using LuaInterface;
using Server.Data;
using Server.TCP;
using System.Net.Sockets;
//using System.Windows;

namespace GameServer.Logic
{
    /// <summary>
    /// 游戏的全局管理
    /// </summary>
    public class GameManager
    {
        #region 地图障碍物格子大小

        /// <summary>
        /// 地图障碍物格子宽度
        /// </summary>
        public static int MapGridWidth = 100;

        /// <summary>
        /// 地图障碍物格子高度
        /// </summary>
        public static int MapGridHeight = 100;

        #endregion 地图障碍物格子大小

        #region 全局变量

        /// <summary>
        /// 程序主窗口
        /// </summary>
        public static Program AppMainWnd = null;

        /// <summary>
        /// 默认的新手村地图编号
        /// </summary>
        public static int DefaultMapCode = 1;

        /// <summary>
        /// 默认的主城地图编号
        /// </summary>
        public static int MainMapCode = 2;

        // 新增加一个默认的地图编号 DefaultMapCode 这个在配置文件里面配成6090 新手场景了 [12/12/2013 LiaoWei]
        /// <summary>
        /// 默认的地图编号
        /// </summary>
        public static int NewDefaultMapCode = 1;

        /// <summary>
        /// Server线路ID
        /// </summary>
        public static int ServerLineID = 1;

        /// <summary>
        /// 自动给予到仓库的物品的ID列表
        /// </summary>
        public static List<int> AutoGiveGoodsIDPortableList = null;

        /// <summary>
        /// 自动给予到仓的物品的ID列表
        /// </summary>
        public static List<int> AutoGiveGoodsIDList = null;

        /// <summary>
        /// 最大的九宫格间隔时间
        /// </summary>
        public static int MaxSlotOnUpdate9GridsTicks = 2000;

        /// <summary>
        /// 最大的休眠时间
        /// </summary>
        public static int MaxSleepOnDoMapGridMoveTicks = 5;

        /// <summary>
        /// 最大的缓存怪物发送给客户端的对象数据时间
        /// </summary>
        public static int MaxCachingMonsterToClientBytesDataTicks = (30 * 1000);

        /// <summary>
        /// 最大的缓存客户端发送给客户端的对象数据时间
        /// </summary>
        public static int MaxCachingClientToClientBytesDataTicks = (30 * 1000);

        /// <summary>
        /// 更新九宫格的模式
        /// </summary>
        public static int Update9GridUsingPosition = 1;

        /// <summary>
        /// 更新移动位置时九宫格的时间
        /// </summary>
        public static int MaxSlotOnPositionUpdate9GridsTicks = 2000;

        /// <summary>
        /// 是否启用新的驱动的九宫格模式
        /// </summary>
        public const int Update9GridUsingNewMode = 0;

        /// <summary>
        /// 启用RoleDataMin模式
        /// </summary>
        public static int RoleDataMiniMode = 1;

        /// <summary>
        /// 是否启用多段攻击
        /// </summary>
        public const bool FlagManyAttack = true;

        /// <summary>
        /// 是否启用锁优化(将锁和Socket绑定)
        /// </summary>
        public const bool FlagOptimizeLock = true;

        /// <summary>
        /// 去掉发送完成时锁BuffLock的过程
        /// </summary>
        public const bool FlagOptimizeLock2 = true;

        /// <summary>
        /// TCPSession锁
        /// </summary>
        public const bool FlagOptimizeLock3 = true;

        /// <summary>
        /// 优化路径字符串处理的消耗
        /// </summary>
        public const bool FlagOptimizePathString = false;

        /// <summary>
        /// 是否禁用记录socket错误状态计数
        /// </summary>
        public const bool FlagOptimizeLockTrace = false;

        /// <summary>
        /// 优化一些运算过程
        /// </summary>
        public const bool FlagOptimizeAlgorithm = true;

        /// <summary>
        /// 线程绑定的缓存(内存)池,本线程用,本线程还,不会跨线程
        /// </summary>
        public const bool FlagOptimizeThreadPool = true;

        /// <summary>
        /// 线程绑定的缓存(内存)池,会跨线程还回
        /// </summary>
        public const bool FlagOptimizeThreadPool2 = false;

        /// <summary>
        /// 线程绑定的缓存(参数)池,会跨线程还回
        /// </summary>
        public const bool FlagOptimizeThreadPool3 = true;

        /// <summary>
        /// 优化BuffLock的锁占用时间
        /// </summary>
        public const bool FlagOptimizeThreadPool4 = true;

        /// <summary>
        /// 优化SendBuff的内存块申请和还回次数,这项优化要求必须开启FlagOptimizeThreadPool4
        /// </summary>
        public const bool FlagOptimizeThreadPool5 = true;

        /// <summary>
        /// 测试参数,禁用所有发送逻辑
        /// </summary>
        public const bool FlagSkipSendDataCall = false;

        /// <summary>
        /// 测试参数,禁用调用AddBuff函数
        /// </summary>
        public const bool FlagSkipAddBuffCall = false;

        /// <summary>
        /// 测试参数,禁用调用TrySend函数
        /// </summary>
        public const bool FlagSkipTrySendCall = false;

        /// <summary>
        /// 测试参数,禁用发送调用
        /// </summary>
        public const bool FlagSkipSocketSend = false;

        /// <summary>
        /// 内存泄漏检测
        /// </summary>
        public const bool FlagTraceMemoryPool = false;

        /// <summary>
        /// 详细记录一些信息
        /// </summary>
        public const bool FlagTraceTCPEvent = false;

        /// <summary>
        /// 详细属性信息
        /// </summary>
        public const bool FlagTracePropsValues = false;

        /// <summary>
        /// 是否禁用名字服务器
        /// </summary>
        public const bool FlagDisableNameServer = true;

        /// <summary>
        /// 不能跳过的发送包数
        /// </summary>
        public const int CostSkipSendCount = 900;

        /// <summary>
        /// 测试特别问题时使用
        /// </summary>
        public static int FlagSleepTime = 0;

        /// <summary>
        /// 指令时间统计记录模式,0 记录较大的时间(不太准确),1 记录所有的指令的时间(不太准确),2 精确记录
        /// </summary>
        public const int StatisticsMode = 0;

        /// <summary>
        /// 配置文件中的配置,内存池各内存块缓存的数量(Size,Num)
        /// </summary>
        public static Dictionary<int, int> MemoryPoolConfigDict = new Dictionary<int, int>();

        /// <summary>
        /// 开启压力测试模式,在此期间登录的帐号视为压力测试帐号
        /// </summary>
        public static bool TestGamePerformanceMode = false;

        public static List<System.Windows.Point> TestBirthPointList1 = new List<System.Windows.Point>();
        public static List<System.Windows.Point> TestBirthPointList2 = new List<System.Windows.Point>();

        /// <summary>
        /// 压力测试,且测试新手场景
        /// </summary>
        public static int TestGamePerformanceMapCode = 1;
        public static int TestGamePerformanceMapMode = 0; //压测模式: 0 指定地图, 1 新手场景, 2 多主线地图, 3 剧情副本地图
        public static bool TestGamePerformanceAllPK = false; //开启全体PK模式
        public static bool TestGamePerformanceLockLifeV = true; //是否锁定血量,不见血
        public static bool TestGamePerformanceForAllUser = false; //为所有角色开启压测模式

        /// <summary>
        /// 是否显示假人
        /// </summary>
        public static bool TestGameShowFakeRoleForUser = false;

        /// <summary>
        /// 压测帐号的装备列表
        /// </summary>
        public static int[][] TestRoleEquipsArrays = new int[3][]
        {
            new int[]{ 1005005, 1005005, 1000105, 1000005, 1000505, 1000205, 1000605, 1000605, 1000305, 1000405, /*套装*/1032212/*12阶护符*/},
            new int[]{ 1015105, /*单手*/ 1000105, 1010005, 1010505, 1010205, 1010605, 1010605, 1010305, 1010405, /*套装*/1032212/*12阶护符*/},
            new int[]{ 1025405, 1025505, 1020105, 1020005, 1020505, 1020205, 1020605, 1020605, 1020305, 1020405, /*套装*/1032212/*12阶护符*/},
        };

        [ThreadStatic]
        public static StringBuilder ThreadStaticPropSB;

        #endregion 全局变量

        #region 全局对象

        /// <summary>
        /// lua 管理器，用于编写提供给lua脚本调用的c# 函数
        /// </summary>
        public static LuaManager LuaMgr = new LuaManager();

        /// <summary>
        /// lua语言解析器对象(线程安全)
        /// </summary>
        //public static LuaVM SystemLuaVM = new LuaVM();

        /// <summary>
        /// 在线用户回话管理对象
        /// </summary>
        public static UserSession OnlineUserSession = new UserSession();

        /// <summary>
        /// 怪物的ID生成对象
        /// </summary>
        public static MonsterIDManager MonsterIDMgr = new MonsterIDManager();

        /// <summary>
        /// 宠物和卫兵的ID生成对象
        /// </summary>
        public static PetIDManager PetIDMgr  = new PetIDManager();

        /// <summary>
        /// 镖车的ID生成对象
        /// </summary>
        public static BiaoCheIDManager BiaoCheIDMgr = new BiaoCheIDManager();

        /// <summary>
        /// 帮旗的ID生成对象
        /// </summary>
        public static JunQiIDManager JunQiIDMgr = new JunQiIDManager();

        /// <summary>
        /// 假人的ID生成对象
        /// </summary>
        public static FakeRoleIDManager FakeRoleIDMgr = new FakeRoleIDManager();

        /// <summary>
        /// 地图管理对象
        /// </summary>
        public static MapManager MapMgr = new MapManager();

        /// <summary>
        /// 地图格子管理对象
        /// </summary>
        public static MapGridManager MapGridMgr = new MapGridManager();

        /// <summary>
        /// 在线客户的管理对象
        /// </summary>
        public static ClientManager ClientMgr = new ClientManager();

        /// <summary>
        /// 地图爆怪区域管理类
        /// </summary>
        public static MonsterZoneManager MonsterZoneMgr = new MonsterZoneManager();

        /// <summary>
        /// 地图爆怪管理对象
        /// </summary>
        public static MonsterManager MonsterMgr = new MonsterManager();

        /// <summary>
        /// 数据库命令队列管理
        /// </summary>
        public static DBCmdManager DBCmdMgr = new DBCmdManager();

        /// <summary>
        /// 日志数据库命令队列管理
        /// </summary>
        public static LogDBCmdManager logDBCmdMgr = new LogDBCmdManager();

        /// <summary>
        /// NPC交易列表
        /// </summary>
        public static NPCSaleList NPCSaleListMgr = new NPCSaleList();

        /// <summary>
        /// 物品名字索引管理
        /// </summary>
        public static SystemGoodsManager SystemGoodsNamgMgr = new SystemGoodsManager();

        /// <summary>
        /// 任务奖励管理
        /// </summary>
        public static TaskAwards TaskAwardsMgr = new TaskAwards();

        /// <summary>
        /// 装备属性缓存管理
        /// </summary>
        public static EquipProps EquipPropsMgr = new EquipProps();

        /// <summary>
        /// 技能列表快速索引管理
        /// </summary>
        public static SystemMagicManager SystemMagicQuickMgr = new SystemMagicManager();

        /// <summary>
        /// 技能公式列表管理
        /// </summary>
        public static SystemMagicAction SystemMagicActionMgr = new SystemMagicAction();

        /// <summary>
        /// 技能公式列表管理2
        /// </summary>
        public static SystemMagicAction SystemMagicActionMgr2 = new SystemMagicAction();

        /// <summary>
        /// 技能公式列表管理(攻击范围)
        /// </summary>
        public static SystemMagicAction SystemMagicScanTypeMgr = new SystemMagicAction();

        /// <summary>
        /// NPC和任务的映射管理
        /// </summary>
        public static NPCTasksManager NPCTasksMgr = new NPCTasksManager();

        /// <summary>
        /// 物品掉落管理
        /// </summary>
        public static GoodsPackManager GoodsPackMgr = new GoodsPackManager();

        /// <summary>
        /// 物品交易管理
        /// </summary>
        public static GoodsExchangeManager GoodsExchangeMgr = new GoodsExchangeManager();

        /// <summary>
        /// 组队管理
        /// </summary>
        public static TeamManager TeamMgr = new TeamManager();

        /// <summary>
        /// 炎黄战场调度和管理对象
        /// </summary>
        public static BattleManager BattleMgr = new BattleManager();

        /// <summary>
        /// 竞技场决斗赛调度和管理对象
        /// </summary>
        public static ArenaBattleManager ArenaBattleMgr = new ArenaBattleManager();

        /// <summary>
        /// 点将台房间管理对象
        /// </summary>
        public static DJRoomManager DJRoomMgr = new DJRoomManager();

        /// <summary>
        /// 副本地图管理
        /// </summary>
        public static CopyMapManager CopyMapMgr = new CopyMapManager();

        /// <summary>
        /// GM命令处理
        /// </summary>
        public static GMCommands systemGMCommands = new GMCommands();

        /// <summary>
        /// 公告管理消息对象
        /// </summary>
        public static BulletinMsgManager BulletinMsgMgr = new BulletinMsgManager();

        /// <summary>
        /// 游戏配置对象
        /// </summary>
        public static GameConfig GameConfigMgr = new GameConfig();

        /// <summary>
        /// 系统参数配置列表
        /// </summary>
        public static SystemParamsList systemParamsList = new SystemParamsList();

        /// <summary>
        /// 帮旗队列管理对象
        /// </summary>
        public static JunQiManager JunQiMgr = new JunQiManager();

        /// <summary>
        /// 写数据库日志对象
        /// </summary>
        //public static EventsSet DBEventsWriter = new EventsSet();

        /// <summary>
        /// 生肖竞猜调度和管理对象
        /// </summary>
        public static ShengXiaoGuessManager ShengXiaoGuessMgr = new ShengXiaoGuessManager();

        /// <summary>
        /// 基于格子的魔法
        /// </summary>
        public static MapGridMagicHelper GridMagicHelperMgr = new MapGridMagicHelper();

        /// <summary>
        /// 基于格子的魔法,判断间隔减小到100毫秒,灵活的伤害范围公式
        /// </summary>
        public static MapGridMagicHelper GridMagicHelperMgrEx = new MapGridMagicHelper();

        /// <summary>
        /// 血色堡垒管理器
        /// </summary>
        public static BloodCastleManager BloodCastleMgr = new BloodCastleManager();

        /// <summary>
        /// 天使神殿管理器
        /// </summary>
        public static AngelTempleManager AngelTempleMgr = new AngelTempleManager();

        /// <summary>
        /// BOSS之家管理器
        /// </summary>
        public static BossHomeManager BosshomeMgr = new BossHomeManager();

        /// <summary>
        /// 黄金神庙管理器
        /// </summary>
        public static GoldTempleManager GoldTempleMgr = new GoldTempleManager();

        /// <summary>
        /// 血色堡垒副本管理器
        /// </summary>
        public static BloodCastleCopySceneManager BloodCastleCopySceneMgr = new BloodCastleCopySceneManager();

        /// <summary>
        /// 恶魔广场副本管理器
        /// </summary>
        public static DaimonSquareCopySceneManager DaimonSquareCopySceneMgr = new DaimonSquareCopySceneManager();

        /// <summary>
        /// 星座管理器
        /// </summary>
        public static StarConstellationManager StarConstellationMgr = new StarConstellationManager();

        /// <summary>
        /// 转生管理器
        /// </summary>
        public static ChangeLifeManager ChangeLifeMgr = new ChangeLifeManager();

        /// <summary>
        /// 帮会副本的数据
        /// </summary>
        public static GuildCopyMapManager GuildCopyMapMgr = new GuildCopyMapManager();

        /// <summary>
        /// 帮会副本DB数据的缓存
        /// </summary>
        public static GuildCopyMapDBManager GuildCopyMapDBMgr = new GuildCopyMapDBManager();

        /// <summary>
        /// 庆功宴管理器
        /// </summary>
        public static QingGongYanManager QingGongYanMgr = new QingGongYanManager();
		
        #endregion 全局对象

        #region Xml缓存对象

        /// <summary>
        /// 任务列表管理对象
        /// </summary>        
        public static SystemXmlItems SystemTasksMgr = new SystemXmlItems();

        /// <summary>
        /// NPC列表管理
        /// </summary>
        public static SystemXmlItems SystemNPCsMgr = new SystemXmlItems();

        /// <summary>
        /// 系统操作列表管理
        /// </summary>
        public static SystemXmlItems SystemOperasMgr = new SystemXmlItems();

        /// <summary>
        /// 技能列表管理
        /// </summary>
        public static SystemXmlItems SystemMagicsMgr = new SystemXmlItems();

        /// <summary>
        /// 物品列表管理
        /// </summary>
        public static SystemXmlItems SystemGoods = new SystemXmlItems();

        /// <summary>
        /// 爆怪的物品列表xml管理
        /// </summary>
        public static SystemXmlItems SystemMonsterGoodsList = new SystemXmlItems();

        /// <summary>
        /// 限制时间爆怪的物品列表xml管理
        /// </summary>
        public static SystemXmlItems SystemLimitTimeMonsterGoodsList = new SystemXmlItems();

        /// <summary>
        /// 爆怪的物品品质ID列表管理
        /// </summary>
        public static SystemXmlItems SystemGoodsQuality = new SystemXmlItems();

        /// <summary>
        /// 爆怪的物品级别ID列表管理
        /// </summary>
        public static SystemXmlItems SystemGoodsLevel = new SystemXmlItems();

        /// <summary>
        /// 爆怪的物品天生ID列表管理
        /// </summary>
        public static SystemXmlItems SystemGoodsBornIndex = new SystemXmlItems();

        /// <summary>
        /// 爆怪的物品追加ID列表管理
        /// </summary>
        public static SystemXmlItems SystemGoodsZhuiJia = new SystemXmlItems();

        /// <summary>
        /// 爆怪的物品卓越ID列表管理
        /// </summary>
        public static SystemXmlItems SystemGoodsExcellenceProperty = new SystemXmlItems();

        /// <summary>
        /// 炎黄战场的调度xml管理
        /// </summary>
        public static SystemXmlItems SystemBattle = new SystemXmlItems();

        /// <summary>
        /// 阵营战的排名奖励表
        /// </summary>
        public static SystemXmlItems SystemBattlePaiMingAwards = new SystemXmlItems();

        /// <summary>
        /// 竞技场决斗赛的调度xml管理
        /// </summary>
        public static SystemXmlItems SystemArenaBattle = new SystemXmlItems();

        /// <summary>
        /// NPC功能脚本列表管理
        /// </summary>
        public static SystemXmlItems systemNPCScripts = new SystemXmlItems();

        /// <summary>
        /// 宠物列表管理
        /// </summary>
        public static SystemXmlItems systemPets = new SystemXmlItems();

        /// <summary>
        /// 坐骑数据字典
        /// </summary>
        public static Dictionary<int, SystemXmlItems> SystemHorseDataDict = new Dictionary<int, SystemXmlItems>();

        /// <summary>
        /// 物品合成类型列表管理
        /// </summary>
        public static SystemXmlItems systemGoodsMergeTypes = new SystemXmlItems();

        /// <summary>
        /// 物品合成类型项管理
        /// </summary>
        public static SystemXmlItems systemGoodsMergeItems = new SystemXmlItems();

        /// <summary>
        /// 闭关收益表
        /// </summary>
        public static SystemXmlItems systemBiGuanMgr = new SystemXmlItems();

        /// <summary>
        /// 商城物品列表
        /// </summary>
        public static SystemXmlItems systemMallMgr = new SystemXmlItems();

        /// <summary>
        /// 冲穴经验收益表
        /// </summary>
        public static SystemXmlItems systemJingMaiExpMgr = new SystemXmlItems();

        /// <summary>
        /// 物品包配置管理
        /// </summary>
        public static SystemXmlItems systemGoodsBaoGuoMgr = new SystemXmlItems();

        /// <summary>
        /// 挖宝设置表
        /// </summary>
        public static SystemXmlItems systemWaBaoMgr = new SystemXmlItems();

        /// <summary>
        /// 周连续登录送礼配置表
        /// </summary>
        public static SystemXmlItems systemWeekLoginGiftMgr = new SystemXmlItems();

        /// <summary>
        /// 当月在线时长送礼配置表
        /// </summary>
        public static SystemXmlItems systemMOnlineTimeGiftMgr = new SystemXmlItems();

        /// <summary>
        /// 新手见面送礼配置表
        /// </summary>
        public static SystemXmlItems systemNewRoleGiftMgr = new SystemXmlItems();

        /// <summary>
        /// 升级有礼配置表
        /// </summary>
        public static SystemXmlItems systemUpLevelGiftMgr = new SystemXmlItems();

        /// <summary>
        /// 副本配置表
        /// </summary>
        public static SystemXmlItems systemFuBenMgr = new SystemXmlItems();

        /// <summary>
        /// 押镖配置表
        /// </summary>
        public static SystemXmlItems systemYaBiaoMgr = new SystemXmlItems();

        /// <summary>
        /// 特殊的时间表
        /// </summary>
        public static SystemXmlItems systemSpecialTimeMgr = new SystemXmlItems();

        /// <summary>
        /// 英雄逐擂配置表
        /// </summary>
        public static SystemXmlItems systemHeroConfigMgr = new SystemXmlItems();

        /// <summary>
        /// 帮旗升级配置表
        /// </summary>
        public static SystemXmlItems systemBangHuiFlagUpLevelMgr = new SystemXmlItems();

        /// <summary>
        /// 帮旗属性配置表
        /// </summary>
        public static SystemXmlItems systemJunQiMgr = new SystemXmlItems();

        /// <summary>
        /// 旗座位置配置表
        /// </summary>
        public static SystemXmlItems systemQiZuoMgr = new SystemXmlItems();

        /// <summary>
        /// 领地所属地图旗帜配置表
        /// </summary>
        public static SystemXmlItems systemLingQiMapQiZhiMgr = new SystemXmlItems();

        /// <summary>
        /// 奇珍阁物品配置表
        /// </summary>
        public static SystemXmlItems systemQiZhenGeGoodsMgr = new SystemXmlItems();

        /// <summary>
        /// 皇城复活点配置表
        /// </summary>
        public static SystemXmlItems systemHuangChengFuHuoMgr = new SystemXmlItems();

        /// <summary>
        /// 隋唐战场定时经验表
        /// </summary>
        public static SystemXmlItems systemBattleExpMgr = new SystemXmlItems();

        /// <summary>
        /// 皇城，血战地府，领地战定时给予的收益表
        /// </summary>
        public static SystemXmlItems systemBangZhanAwardsMgr = new SystemXmlItems();

        /// <summary>
        /// 隋唐战场出生点表
        /// </summary>
        public static SystemXmlItems systemBattleRebirthMgr = new SystemXmlItems();

        /// <summary>
        /// 隋唐战场奖励表
        /// </summary>
        public static SystemXmlItems systemBattleAwardMgr = new SystemXmlItems();

        /// <summary>
        /// 装备天生洗练表
        /// </summary>
        public static SystemXmlItems systemEquipBornMgr = new SystemXmlItems();

        /// <summary>
        /// 装备天生属性级别名称表
        /// </summary>
        public static SystemXmlItems systemBornNameMgr = new SystemXmlItems();

        /// <summary>
        /// Vip每日奖励缓存表
        /// </summary>
        public static SystemXmlItems systemVipDailyAwardsMgr = new SystemXmlItems();

        /// <summary>
        /// 活动引导提示缓存表
        /// </summary>
        public static SystemXmlItems systemActivityTipMgr = new SystemXmlItems();

        /// <summary>
        /// 杨公宝库幸运值奖励缓存表
        /// </summary>
        public static SystemXmlItems systemLuckyAwardMgr = new SystemXmlItems();

        /// <summary>
        /// 幸运金蛋幸运值奖励缓存表
        /// </summary>
        public static SystemXmlItems systemLuckyAward2Mgr = new SystemXmlItems();

        /// <summary>
        /// 杨公宝库幸运值规则表
        /// </summary>
        public static SystemXmlItems systemLuckyMgr = new SystemXmlItems();

        /// <summary>
        /// 成就配置管理
        /// </summary>
        public static SystemXmlItems systemChengJiu = new SystemXmlItems();

        /// <summary>
        /// 成就Buffer配置管理
        /// </summary>
        public static SystemXmlItems systemChengJiuBuffer = new SystemXmlItems();

        /// <summary>
        /// 武器通灵配置管理
        /// </summary>
        public static SystemXmlItems systemWeaponTongLing = new SystemXmlItems();

        /// <summary>
        /// 乾坤袋配置管理
        /// </summary>
        //public static SystemXmlItems systemQianKunMgr = new SystemXmlItems();

        /// <summary>
        /// 祈福分级配置管理 [8/28/2014 LiaoWei]
        /// </summary>
        public static SystemXmlItems systemImpetrateByLevelMgr = new SystemXmlItems();
        
        /// <summary>
        /// 幸运抽奖配置管理
        /// </summary>
        public static SystemXmlItems systemXingYunChouJiangMgr = new SystemXmlItems();

        /// <summary>
        /// 月度抽奖配置管理
        /// </summary>
        public static SystemXmlItems systemYueDuZhuanPanChouJiangMgr = new SystemXmlItems();

        /// <summary>
        /// 每日在线奖励管理 [1/12/2014 LiaoWei]
        /// </summary>
        public static SystemXmlItems systemEveryDayOnLineAwardMgr = new SystemXmlItems();

        /// <summary>
        /// 连续登陆奖励管理 [1/17/2014 LiaoWei]
        /// </summary>
        public static SystemXmlItems systemSeriesLoginAwardMgr = new SystemXmlItems();

        /// <summary>
        /// 怪物管理
        /// </summary>
        public static SystemXmlItems systemMonsterMgr = new SystemXmlItems();

        /// <summary>
        /// 经脉等级管理
        /// </summary>
        public static SystemXmlItems SystemJingMaiLevel = new SystemXmlItems();

        /// <summary>
        /// 武学等级管理
        /// </summary>
        public static SystemXmlItems SystemWuXueLevel = new SystemXmlItems();

        /// <summary>
        /// 过场动画文件管理
        /// </summary>
        public static SystemXmlItems SystemTaskPlots = new SystemXmlItems();

        /// <summary>
        /// 抢购管理
        /// </summary>
        public static SystemXmlItems SystemQiangGou = new SystemXmlItems();

        /// <summary>
        /// 合服抢购管理
        /// </summary>
        public static SystemXmlItems SystemHeFuQiangGou = new SystemXmlItems();

        /// <summary>
        /// 节日抢购管理
        /// </summary>
        public static SystemXmlItems SystemJieRiQiangGou = new SystemXmlItems();

        /// <summary>
        /// 钻皇等级管理
        /// </summary>
        public static SystemXmlItems SystemZuanHuangLevel = new SystemXmlItems();

        /// <summary>
        /// 系统激活项管理
        /// </summary>
        public static SystemXmlItems SystemSystemOpen = new SystemXmlItems();

        /// <summary>
        /// 系统掉落金钱管理管理
        /// </summary>
        public static SystemXmlItems SystemDropMoney = new SystemXmlItems();

        /// <summary>
        /// 系统限时连续登录送大礼活动配置文件
        /// </summary>
        public static SystemXmlItems SystemDengLuDali = new SystemXmlItems();

        /// <summary>
        /// 系统限时补偿活动配置文件
        /// </summary>
        public static SystemXmlItems SystemBuChang = new SystemXmlItems();

        /// <summary>
        /// 战魂等级管理
        /// </summary>
        public static SystemXmlItems SystemZhanHunLevel = new SystemXmlItems();

        /// <summary>
        /// 荣誉等级管理
        /// </summary>
        public static SystemXmlItems SystemRongYuLevel = new SystemXmlItems();

//         /// <summary>
//         /// 军衔等级管理
//         /// </summary>
//         public static SystemXmlItems SystemShengWangLevel = new SystemXmlItems();

        /// <summary>
        /// 魔晶和祈福兑换
        /// </summary>
        public static SystemXmlItems SystemExchangeMoJingAndQiFu = new SystemXmlItems();

        /// <summary>
        /// 冥想
        /// </summary>
        //public static SystemXmlItems SystemMeditateInfo = new SystemXmlItems();

        /// <summary>
        /// 每日活跃信息配置管理
        /// </summary>
        public static SystemXmlItems systemDailyActiveInfo = new SystemXmlItems();

        /// <summary>
        /// 每日活跃奖励配置管理
        /// </summary>
        public static SystemXmlItems systemDailyActiveAward = new SystemXmlItems();

        /// <summary>
        /// 天使神殿配置数据
        /// </summary>
        public static SystemXmlItems systemAngelTempleData = new SystemXmlItems();

        /// <summary>
        /// 天使神殿排名奖励表
        /// </summary>
        public static SystemXmlItems AngelTempleAward = new SystemXmlItems();

        /// <summary>
        /// 天使神殿幸运奖励表
        /// </summary>
        public static SystemXmlItems AngelTempleLuckyAward = new SystemXmlItems();

        /// <summary>
        /// 任务章节
        /// </summary>
        public static SystemXmlItems TaskZhangJie = new SystemXmlItems();

        /// <summary>
        /// 任务ID区间对应的这个任务需要加成的章节属性配置
        /// </summary>
        public static List<RangeKey> TaskZhangJieDict = new List<RangeKey>();

        /// <summary>
        /// 交易物品类别表
        /// </summary>
        public static SystemXmlItems JiaoYiTab = new SystemXmlItems();

        /// <summary>
        /// 交易物品类别定义表
        /// </summary>
        public static SystemXmlItems JiaoYiType = new SystemXmlItems();

        /// <summary>
        /// 战盟建设
        /// </summary>
        public static SystemXmlItems SystemZhanMengBuild = new SystemXmlItems();

        /// <summary>
        /// 翅膀进阶配置表
        /// </summary>
        public static SystemXmlItems SystemWingsUp = new SystemXmlItems();

        /// <summary>
        /// Boss AI配置表
        /// </summary>
        public static SystemXmlItems SystemBossAI = new SystemXmlItems();

        /// <summary>
        /// 拓展属性配置表
        /// </summary>
        public static SystemXmlItems SystemExtensionProps = new SystemXmlItems();

        /// <summary>
        /// 采集物管理
        /// </summary>
        public static SystemXmlItems systemCaiJiMonsterMgr = new SystemXmlItems();

        #endregion Xml缓存对象

        #region 事件日志对象

        /// <summary>
        /// 服务器端普通日志事件
        /// </summary>
        public static ServerEvents SystemServerEvents = new ServerEvents() { EventRootPath = "Events", EventPreFileName = "Event" };

        /// <summary>
        /// 服务器端角色登录日志事件
        /// </summary>
        public static ServerEvents SystemRoleLoginEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Login" };

        /// <summary>
        /// 服务器端角色登出日志事件
        /// </summary>
        public static ServerEvents SystemRoleLogoutEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Logout" };

        /// <summary>
        /// 服务器端角色完成任务日志事件
        /// </summary>
        public static ServerEvents SystemRoleTaskEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Task" };

        /// <summary>
        /// 服务器端角色死亡日志事件
        /// </summary>
        public static ServerEvents SystemRoleDeathEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Death" };

        /// <summary>
        /// 服务器端角色铜钱购买日志事件
        /// </summary>
        public static ServerEvents SystemRoleBuyWithTongQianEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "TongQianBuy" };

        /// <summary>
        /// 服务器端角色银两购买日志事件
        /// </summary>
        public static ServerEvents SystemRoleBuyWithYinLiangEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "YinLiangBuy" };

        /// <summary>
        /// 服务器端角色军贡购买日志事件
        /// </summary>
        public static ServerEvents SystemRoleBuyWithJunGongEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "JunGongBuy" };

        /// <summary>
        /// 服务器端角色银票购买日志事件
        /// </summary>
        public static ServerEvents SystemRoleBuyWithYinPiaoEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "YinPiaoBuy" };

        /// <summary>
        /// 服务器端角色元宝购买日志事件
        /// </summary>
        public static ServerEvents SystemRoleBuyWithYuanBaoEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "YuanBaoBuy" };

        /// <summary>
        /// 服务器端角色元宝奇珍阁购买日志事件
        /// </summary>
        public static ServerEvents SystemRoleQiZhenGeBuyWithYuanBaoEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "QiZhenGeBuy" };

        /// <summary>
        /// 服务器端角色元宝商城抢购购买日志事件
        /// </summary>
        public static ServerEvents SystemRoleQiangGouBuyWithYuanBaoEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "QiangGouBuy" };


        /// <summary>
        /// 服务器端角色出售物品日志事件
        /// </summary>
        public static ServerEvents SystemRoleSaleEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Sale" };

        /// <summary>
        /// 服务器端角色交易日志事件(物品交易)
        /// </summary>
        public static ServerEvents SystemRoleExchangeEvents1 = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Exchange1" };

        /// <summary>
        /// 服务器端角色交易日志事件(银两交易)
        /// </summary>
        public static ServerEvents SystemRoleExchangeEvents2 = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Exchange2" };

        /// <summary>
        /// 服务器端角色交易日志事件(元宝交易)
        /// </summary>
        public static ServerEvents SystemRoleExchangeEvents3 = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Exchange3" };

        /// <summary>
        /// 服务器端升级日志事件
        /// </summary>
        public static ServerEvents SystemRoleUpgradeEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Upgrade" };

        /// <summary>
        /// 服务器端物品相关日志事件
        /// </summary>
        public static ServerEvents SystemRoleGoodsEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Goods" };

        /// <summary>
        /// 服务器端掉落被拾取的物品相关日志事件
        /// </summary>
        public static ServerEvents SystemRoleFallGoodsEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "FallGoods" };

        /// <summary>
        /// 服务器端银两获取和使用的相关日志事件
        /// </summary>
        public static ServerEvents SystemRoleYinLiangEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "YinLiang" };

        /// <summary>
        /// 服务器端坐骑幸运点日志事件
        /// </summary>
        public static ServerEvents SystemRoleHorseEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Horse" };

        /// <summary>
        /// 服务器端帮贡获取和使用的相关日志事件
        /// </summary>
        public static ServerEvents SystemRoleBangGongEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "BangGong" };

        /// <summary>
        /// 服务器端经脉相关日志事件
        /// </summary>
        public static ServerEvents SystemRoleJingMaiEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "JingMai" };

        /// <summary>
        /// 服务器端刷新奇珍阁日志事件
        /// </summary>
        public static ServerEvents SystemRoleRefreshQiZhenGeEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "RefreshQiZhenGe" };

        /// <summary>
        /// 服务器端挖宝事件
        /// </summary>
        public static ServerEvents SystemRoleWaBaoEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "WaBao" };

        /// <summary>
        /// 服务器端地图进入事件
        /// </summary>
        public static ServerEvents SystemRoleMapEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Map" };

        /// <summary>
        /// 副本奖励领取事件
        /// </summary>
        public static ServerEvents SystemRoleFuBenAwardEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "FuBenAward" };

        /// <summary>
        /// 五行奇阵奖励领取事件
        /// </summary>
        public static ServerEvents SystemRoleWuXingAwardEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "WuXingAward" };

        /// <summary>
        /// 跑环完成事件
        /// </summary>
        public static ServerEvents SystemRolePaoHuanOkEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "PaoHuanOk" };

        /// <summary>
        /// 押镖事件
        /// </summary>
        public static ServerEvents SystemRoleYaBiaoEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "YaBiao" };

        /// <summary>
        /// 连斩事件
        /// </summary>
        public static ServerEvents SystemRoleLianZhanEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "LianZhan" };

        /// <summary>
        /// 活动怪物的事件
        /// </summary>
        public static ServerEvents SystemRoleHuoDongMonsterEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "HuoDongMonster" };

        /// <summary>
        /// 服务器端角色精雕细琢[钥匙类]挖宝事件
        /// </summary>
        public static ServerEvents SystemRoleDigTreasureWithYaoShiEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "DigTreasureWithYaoShi" };

        /// <summary>
        /// 服务器端自动扣除元宝事件
        /// </summary>
        public static ServerEvents SystemRoleAutoSubYuanBaoEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "AutoSubYuanBao" };

        /// <summary>
        /// 服务器端自动扣除金币事件
        /// </summary>
        public static ServerEvents SystemRoleAutoSubGoldEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "AutoSubGold" };

        /// <summary>
        /// 服务器端自动扣除金币-元宝事件
        /// </summary>
        public static ServerEvents SystemRoleAutoSubEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "AutoSub" };

        /// <summary>
        /// 角色提取邮件元宝，银两，铜钱事件
        /// </summary>
        public static ServerEvents SystemRoleFetchMailMoneyEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "MailMoneyFetch" };

        /// <summary>
        /// 服务器端角色天地精元兑换日志事件
        /// </summary>
        public static ServerEvents SystemRoleBuyWithTianDiJingYuanEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "TianDiJingYuanBuy" };

        /// <summary>
        /// 角色Vip奖励的元宝，银两，铜钱, 灵力事件
        /// </summary>
        public static ServerEvents SystemRoleFetchVipAwardEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "VipAwardGet" };

        /// <summary>
        /// 服务器端角色金币购买日志事件
        /// </summary>
        public static ServerEvents SystemRoleBuyWithGoldEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "GoldBuy" };

        /// <summary>
        /// 服务器端金币获取和使用的相关日志事件
        /// </summary>
        public static ServerEvents SystemRoleGoldEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "Gold" };

        //**************
        /// <summary>
        /// 服务器端角色精元值购买日志事件
        /// </summary>
        public static ServerEvents SystemRoleBuyWithJingYuanZhiEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "JingYuanZhiBuy" };

        /// <summary>
        /// 服务器端角色猎杀值购买日志事件
        /// </summary>
        public static ServerEvents SystemRoleBuyWithLieShaZhiEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "LieShaZhiBuy" };

        /// <summary>
        /// 服务器端角色装备积分值购买日志事件
        /// </summary>
        public static ServerEvents SystemRoleBuyWithZhuangBeiJiFenEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "ZhuangBeiJiFenBuy" };

        /// <summary>
        /// 服务器端角色军功值购买日志事件===》注意，军功值需要和旧的 帮会的 军贡值相区别
        /// </summary>
        public static ServerEvents SystemRoleBuyWithJunGongZhiEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "JunGongZhiBuy" };

        /// <summary>
        /// 服务器端角色战魂值购买日志事件
        /// </summary>
        public static ServerEvents SystemRoleBuyWithZhanHunEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "ZhanHunBuy" };

        /// <summary>
        /// 服务器端角色元宝日志事件
        /// </summary>
        public static ServerEvents SystemRoleUserMoneyEvents = new ServerEvents() { EventRootPath = "RoleEvents", EventPreFileName = "UserMoney" };

        #endregion 事件日志对象

        #region cpu和内存测试辅助

        /// <summary>
        /// 临时测试内存用
        /// </summary>
        public static Dictionary<int, SafeClientData> RoleDataExDictForTestMem = new Dictionary<int, SafeClientData>();

        public static bool CanSkipCmd(TMSKSocket s)
        {
            if (s.SendCount > GameManager.CostSkipSendCount)
            {
                return true;
            }
            else
            {
                s.SendCount++;
                return false;
            }
        }

        #endregion cpu和内存测试辅助
    }
}
