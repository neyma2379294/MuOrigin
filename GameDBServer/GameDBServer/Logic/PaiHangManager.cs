using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;
using GameDBServer.Logic.WanMoTa;
using GameDBServer.Logic.Wing;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 数据库统计管理
    /// </summary>
    public class PaiHangManager
    {
        #region 时间校验数据

        /// <summary>
        /// 上次检测排行的日期
        /// </summary>
        private static int LastCheckPaiHangDayID = DateTime.Now.DayOfYear;

        /// <summary>
        /// 上次检测排行的的时间之
        /// </summary>
        private static int LastCheckPaiHangTimer = (DateTime.Now.Hour * 60) + DateTime.Now.Minute;

        /// <summary>
        /// 进行排行的时间
        /// </summary>
        private static int PaiHangTimer = (7 * 60) + 0;

        /// <summary>
        /// DB更新时间间隔 -- 30分钟 [5/10/2014 LiaoWei]
        /// </summary>
        private const long UpdatePaiHangIntervalTimer = (30 * 60 * 1 * 1000);

        /// <summary>
        /// 上次排行的TICK时间 [5/10/2014 LiaoWei]
        /// </summary>
        private static long LastUpdatePaiHangTickTimer = 0;

        #endregion 时间校验数据

        #region 排行列表

        /// <summary>
        /// 装备排行
        /// </summary>
        private static List<PaiHangItemData> RoleEquipPaiHangList = null;

        /// <summary>
        /// 冲穴个数排行
        /// </summary>
        private static List<PaiHangItemData> RoleXueWeiNumPaiHangList = null;

        /// <summary>
        /// 技能层数排行
        /// </summary>
        private static List<PaiHangItemData> RoleSkillLevelPaiHangList = null;

        /// <summary>
        /// 坐骑积分排行
        /// </summary>
        private static List<PaiHangItemData> RoleHorseJiFenPaiHangList = null;

        /// <summary>
        /// 角色级别
        /// </summary>
        private static List<PaiHangItemData> RoleLevelPaiHangList = null;

        /// <summary>
        /// 银两排行
        /// </summary>
        private static List<PaiHangItemData> RoleYinLiangPaiHangList = null;

        /// <summary>
        /// 连斩排行
        /// </summary>
        private static List<PaiHangItemData> RoleLianZhanPaiHangList = null;

        /// <summary>
        /// 杀BOSS排行
        /// </summary>
        private static List<PaiHangItemData> RoleKillBossPaiHangList = null;

        /// <summary>
        /// 角斗场称号次数
        /// </summary>
        private static List<PaiHangItemData> RoleBattleNumPaiHangList = null;

        /// <summary>
        /// 英雄逐擂到达层数
        /// </summary>
        private static List<PaiHangItemData> RoleHeroIndexPaiHangList = null;

        /// <summary>
        /// 角色金币排行
        /// </summary>
        private static List<PaiHangItemData> RoleGoldPaiHangList = null;

        // 战斗力 [12/17/2013 LiaoWei]
        /// <summary>
        /// 战斗力排行
        /// </summary>
        private static List<PaiHangItemData> CombatForcePaiHangList = null;

        #endregion 排行列表

        #region 处理函数

        /// <summary>
        /// 加载排行列表
        /// </summary>
        private static void LoadPaiHangLists(DBManager dbMgr)
        {
            //装备排行
            RoleEquipPaiHangList = DBQuery.GetRoleEquipPaiHang(dbMgr);

            //冲穴个数排行===>相当于现在的经脉排行榜===对应经脉王
            //RoleXueWeiNumPaiHangList = DBQuery.GetRoleXueWeiNumPaiHang(dbMgr);
            RoleXueWeiNumPaiHangList = DBQuery.GetRoleParamsTablePaiHang(dbMgr, RoleParamName.JingMaiLevel);

            //技能层数排行===>相当于现在的武学排行榜===对应武学王
            //RoleSkillLevelPaiHangList = DBQuery.GetRoleSkillLevelPaiHang(dbMgr);
            RoleSkillLevelPaiHangList = DBQuery.GetRoleParamsTablePaiHang(dbMgr, RoleParamName.WuXueLevel);

            //坐骑积分排行
            RoleHorseJiFenPaiHangList = DBQuery.GetRoleHorseJiFenPaiHang(dbMgr);

            //角色级别
            RoleLevelPaiHangList = DBQuery.GetRoleLevelPaiHang(dbMgr);

            //银两排行
            RoleYinLiangPaiHangList = DBQuery.GetRoleYinLiangPaiHang(dbMgr);

            //连斩排行
            RoleLianZhanPaiHangList = DBQuery.GetRoleLianZhanPaiHang(dbMgr);

            //杀BOSS排行
            RoleKillBossPaiHangList = DBQuery.GetRoleKillBossPaiHang(dbMgr);

            //角斗场称号次数
            RoleBattleNumPaiHangList = DBQuery.GetRoleBattleNumPaiHang(dbMgr);

            //英雄逐擂到达层数
            RoleHeroIndexPaiHangList = DBQuery.GetRoleHeroIndexPaiHang(dbMgr);

            //角色金币排行
            RoleGoldPaiHangList = DBQuery.GetRoleGoldPaiHang(dbMgr);

            // 战斗力 [12/17/2013 LiaoWei]
            CombatForcePaiHangList = DBQuery.GetRoleCombatForcePaiHang(dbMgr);

            //为活动记录排行信息
            StorePaiHangForHuoDong(dbMgr);
        }

        /// <summary>
        /// 处理排行榜
        /// </summary>
        /// <param name="dbMgr"></param>
        public static void ProcessPaiHang(DBManager dbMgr, bool force = false)
        {
            //如果是强制执行排行的操作
            if (force)
            {
                //加载排行列表
                LoadPaiHangLists(dbMgr);

                return;
            }

            // 半小时更新下排行榜 [5/10/2014 LiaoWei]
            DateTime dateTime = DateTime.Now;
            long nowTicks = dateTime.Ticks / 10000;

            if (nowTicks - LastUpdatePaiHangTickTimer >= UpdatePaiHangIntervalTimer)
            {
                LastUpdatePaiHangTickTimer = nowTicks;
                
                LoadPaiHangLists(dbMgr);
                
                return;
            }

            /*DateTime dateTime = DateTime.Now;
            int dayID = dateTime.DayOfYear;
            int nowTimer = dateTime.Hour * 60 + dateTime.Minute;

            if (dayID != LastCheckPaiHangDayID)
            {
                LastCheckPaiHangDayID = dayID;
                LastCheckPaiHangTimer = nowTimer;
                return;
            }

            //判断是否可以执行排行操作
            if (nowTimer >= PaiHangTimer && LastCheckPaiHangTimer < PaiHangTimer)
            {
                LastCheckPaiHangDayID = dayID;
                LastCheckPaiHangTimer = nowTimer;

                //加载排行列表
                LoadPaiHangLists(dbMgr);

                return;
            }

            LastCheckPaiHangDayID = dayID;
            LastCheckPaiHangTimer = nowTimer;*/
        }

        /// <summary>
        /// 获取排行列表数据
        /// </summary>
        /// <param name="paiHangType"></param>
        /// <returns></returns>
        public static PaiHangData GetPaiHangData(int paiHangType)
        {
            List<PaiHangItemData> paiHangItemList = null;
            switch (paiHangType)
            {
                case (int)PaiHangTypes.EquipJiFen:
                    {
                        paiHangItemList = RoleEquipPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.XueWeiNum:
                    {
                        paiHangItemList = RoleXueWeiNumPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.SkillLevel:
                    {
                        paiHangItemList = RoleSkillLevelPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.HorseJiFen:
                    {
                        paiHangItemList = RoleHorseJiFenPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.RoleLevel:
                    {
                        paiHangItemList = RoleLevelPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.RoleYinLiang:
                    {
                        paiHangItemList = RoleYinLiangPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.LianZhan:
                    {
                        paiHangItemList = RoleLianZhanPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.KillBoss:
                    {
                        paiHangItemList = RoleKillBossPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.BattleNum:
                    {
                        paiHangItemList = RoleBattleNumPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.HeroIndex:
                    {
                        paiHangItemList = RoleHeroIndexPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.RoleGold:
                    {
                        paiHangItemList = RoleGoldPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.CombatForceList:
                    {
                        paiHangItemList = CombatForcePaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.JingJi:
                    {
                        paiHangItemList = JingJiChangManager.getInstance().getRankingList(0);
                    }
                    break;
                case (int)PaiHangTypes.WanMoTa:
                    {
                        paiHangItemList = WanMoTaManager.getInstance().getRankingList(0);
                    }
                    break;
                case (int)PaiHangTypes.Wing:
                    {
                        paiHangItemList = WingPaiHangManager.getInstance().getRankingList(0);
                    }
                    break;
                default:
                    {
                        ;//
                    }
                    break;
            };

            return new PaiHangData()
            {
                PaiHangType = paiHangType,
                PaiHangList = paiHangItemList,
            };
        }

        /// <summary>
        /// 记录排行信息,用于活动奖励【这个函数必须在每次加载排行榜数据的时候调用】
        /// 默认每天早上七点加载一次，如果碰到服务器维护，会在每次服务器重启时加载
        /// </summary>
        /// <param name="dbMgr"></param>
        protected static void StorePaiHangForHuoDong(DBManager dbMgr)
        {
            //充值王活动【在其他位置动态添加】 

            //冲级王活动
            StorePaiHangPos(dbMgr, (int)PaiHangTypes.RoleLevel, (int)ActivityTypes.LevelKing);

            //装备王活动===>转换为boss王活动
            //StorePaiHangPos(dbMgr, (int)PaiHangTypes.EquipJiFen, (int)ActivityTypes.EquipKing);===>
            StorePaiHangPos(dbMgr, (int)PaiHangTypes.KillBoss, (int)ActivityTypes.EquipKing);

            //坐骑王活动===>转换为武学王活动
            //StorePaiHangPos(dbMgr, (int)PaiHangTypes.HorseJiFen, (int)ActivityTypes.HorseKing);===>
            StorePaiHangPos(dbMgr, (int)PaiHangTypes.SkillLevel, (int)ActivityTypes.HorseKing);

            //经脉王活动
            StorePaiHangPos(dbMgr, (int)PaiHangTypes.XueWeiNum, (int)ActivityTypes.JingMaiKing);
            //屠魔勇士
            StorePaiHangPos(dbMgr, (int)PaiHangTypes.KillBoss, (int)ActivityTypes.NewZoneBosskillKing);
            //新服活动，冲级狂人
            StorePaiHangPos(dbMgr, (int)PaiHangTypes.RoleLevel, (int)ActivityTypes.NewZoneUpLevelMadman);
        }

        /// <summary>
        /// 存储排行位置信息，主要用于活动奖励
        /// </summary>
        /// <param name="paiHangType"></param>
        /// <param name="maxPaiHang">最大排行值，只记录小于等于该值的排行，默认是10，只记录前十名</param>
        protected static void StorePaiHangPos(DBManager dbMgr, int paiHangType, int huoDongType, int maxPaiHang = 10)
        {
            List<PaiHangItemData> paiHangItemList = null;
            switch (paiHangType)
            {
                case (int)PaiHangTypes.EquipJiFen:
                    {
                        paiHangItemList = RoleEquipPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.XueWeiNum:
                    {
                        paiHangItemList = RoleXueWeiNumPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.SkillLevel:
                    {
                        paiHangItemList = RoleSkillLevelPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.HorseJiFen:
                    {
                        paiHangItemList = RoleHorseJiFenPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.RoleLevel:
                    {
                        paiHangItemList = RoleLevelPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.RoleYinLiang:
                    {
                        paiHangItemList = RoleYinLiangPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.LianZhan:
                    {
                        paiHangItemList = RoleLianZhanPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.KillBoss:
                    {
                        paiHangItemList = RoleKillBossPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.BattleNum:
                    {
                        paiHangItemList = RoleBattleNumPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.HeroIndex:
                    {
                        paiHangItemList = RoleHeroIndexPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.RoleGold:
                    {
                        paiHangItemList = RoleGoldPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.CombatForceList:
                    {
                        paiHangItemList = CombatForcePaiHangList;   // 战斗力排行榜
                    }
                    break;

                default:
                    {
                        ;//
                    }
                    break;
            };

            if (null == paiHangItemList) return;

            //同类活动采用同一个时间
            string paiHangTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            int phvalue = 0;

            //依次记录排行maxPaiHang 以及之前的排名信息
            for (int i = 0; i < paiHangItemList.Count && i < maxPaiHang; i++)
            {
                DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(paiHangItemList[i].RoleID);
                if (null != dbRoleInfo)
                {
                    //用于计算排行的值
                    phvalue = paiHangItemList[i].Val1;

                    DBWriter.AddHongDongPaiHangRecord(dbMgr, paiHangItemList[i].RoleID, dbRoleInfo.RoleName, dbRoleInfo.ZoneID, huoDongType, i + 1, paiHangTime, phvalue);
                }
            }
        }

        /// <summary>
        /// 获取指定角色在排行列表数据中的位置(只算前10)
        /// </summary>
        /// <param name="paiHangType"></param>
        /// <returns></returns>
        public static int GetPaiHangPosByRoleID(int paiHangType, int roleID)
        {
            List<PaiHangItemData> paiHangItemList = null;
            switch (paiHangType)
            {
                case (int)PaiHangTypes.EquipJiFen:
                    {
                        paiHangItemList = RoleEquipPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.XueWeiNum:
                    {
                        paiHangItemList = RoleXueWeiNumPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.SkillLevel:
                    {
                        paiHangItemList = RoleSkillLevelPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.HorseJiFen:
                    {
                        paiHangItemList = RoleHorseJiFenPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.RoleLevel:
                    {
                        paiHangItemList = RoleLevelPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.RoleYinLiang:
                    {
                        paiHangItemList = RoleYinLiangPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.LianZhan:
                    {
                        paiHangItemList = RoleLianZhanPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.KillBoss:
                    {
                        paiHangItemList = RoleKillBossPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.BattleNum:
                    {
                        paiHangItemList = RoleBattleNumPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.HeroIndex:
                    {
                        paiHangItemList = RoleHeroIndexPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.RoleGold:
                    {
                        paiHangItemList = RoleGoldPaiHangList;
                    }
                    break;
                case (int)PaiHangTypes.CombatForceList:
                    {
                        paiHangItemList = CombatForcePaiHangList;
                    }
                    break;
                default:
                    {
                        ;//
                    }
                    break;
            };

            if (null == paiHangItemList) return -1;
            for (int i = 0; i < paiHangItemList.Count && i < 10; i++)
            {
                if (paiHangItemList[i].RoleID == roleID)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 获取指定角色在排行列表数据中的位置(只算前10)的字典
        /// </summary>
        /// <param name="paiHangType"></param>
        /// <returns></returns>
        public static Dictionary<int, int> CalcPaiHangPosDictRoleID(int roleID)
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            for (int i = (int)PaiHangTypes.EquipJiFen; i < (int)PaiHangTypes.MaxVal; i++)
            {
                dict[i] = GetPaiHangPosByRoleID(i, roleID);
            }

            return dict;
        }

        #endregion 处理函数
    }
}
