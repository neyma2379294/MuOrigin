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
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;

namespace GameDBServer.DB
{
    /// <summary>
    /// 用户角色管理(使用弱引用，当系统内存不足时，让系统自动释放内存)
    /// </summary>
    public class DBRoleMgr
    {
        /// <summary>
        /// 记录用户信息的词典对象
        /// </summary>
        private Dictionary<int, MyWeakReference> DictRoleInfos = new Dictionary<int, MyWeakReference>(10000);

        /// <summary>
        /// 记录用户名称映射到ID的词典对象
        /// </summary>
        private Dictionary<string, int> DictRoleName2ID = new Dictionary<string, int>(10000);

        /// <summary>
        /// 获取个数
        /// </summary>
        /// <returns></returns>
        public int GetRoleInfoCount()
        {
            lock (DictRoleInfos)
            {
                return DictRoleInfos.Count;
            }
        }

        /// <summary>
        /// 定位用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DBRoleInfo FindDBRoleInfo(int roleID)
        {
            DBRoleInfo dbRoleInfo = null;
            MyWeakReference weakRef = null;
            lock (DictRoleInfos)
            {
                if (DictRoleInfos.Count > 0)
                {
                    if (DictRoleInfos.TryGetValue(roleID, out weakRef))
                    {
                        if (weakRef.IsAlive) //判断是否仍在存活
                        {
                            dbRoleInfo = weakRef.Target as DBRoleInfo;
                        }
                    }
                }
            }

            if (null != dbRoleInfo)
            {
                lock (dbRoleInfo)
                {
                    dbRoleInfo.LastReferenceTicks = DateTime.Now.Ticks / 10000;
                }
            }

            return dbRoleInfo;
        }

        /// <summary>
        /// 通过用户名称定位用户ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int FindDBRoleID(string roleName)
        {
            int roleID = -1;
            if (!DictRoleName2ID.TryGetValue(roleName, out roleID))
            {
                return -1;
            }

            return roleID;
        }

        /// <summary>
        /// 添加用户信息到字典中
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        public void AddDBRoleInfo(DBRoleInfo dbRoleInfo)
        {
            MyWeakReference weakRef = null;
            lock (DictRoleInfos)
            {
                if (DictRoleInfos.TryGetValue(dbRoleInfo.RoleID, out weakRef))
                {
                    weakRef.Target = dbRoleInfo;
                }
                else
                {
                    DictRoleInfos.Add(dbRoleInfo.RoleID, new MyWeakReference(dbRoleInfo));
                }
            }

            lock (DictRoleName2ID)
            {
                string formatedRoleName = Global.FormatRoleName(dbRoleInfo);
                DictRoleName2ID[formatedRoleName] = dbRoleInfo.RoleID;
            }
        }

        /// <summary>
        /// 从字典中删除用户信息
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        public void RemoveDBRoleInfo(int roleID)
        {
            string formatedRoleName = null;
            MyWeakReference weakRef = null;
            lock (DictRoleInfos)
            {
                if (DictRoleInfos.TryGetValue(roleID, out weakRef))
                {
                    formatedRoleName = Global.FormatRoleName((weakRef.Target as DBRoleInfo));
                    weakRef.Target = null; //释放内存
                }
            }

            lock (DictRoleName2ID)
            {
                if (null != formatedRoleName)
                {
                    DictRoleName2ID.Remove(formatedRoleName);
                }
            }
        }

        /// <summary>
        /// 清空所有的角色(isdel=1, 清空t_money表, 清空t_inputlog表)
        /// </summary>
        public void ClearAllDBroleInfo()
        {
            lock (DictRoleInfos)
            {
                DictRoleInfos.Clear();
            }

            lock (DictRoleName2ID)
            {
                DictRoleName2ID.Clear();
            }
        }

        /// <summary>
        /// 从字典中获取缓存的用户信息
        /// </summary>
        /// <param name="dbRoleInfo"></param>
        public List<DBRoleInfo> GetCachingDBRoleInfoListByFaction(int faction)
        {
            List<DBRoleInfo> dbRoleInfoList = new List<DBRoleInfo>();
            MyWeakReference weakRef = null;
            DBRoleInfo dbRoleInfo = null;
            lock (DictRoleInfos)
            {
                foreach (var key in DictRoleInfos.Keys)
                {
                    weakRef = DictRoleInfos[key];
                    dbRoleInfo = weakRef.Target as DBRoleInfo;
                    if (null != dbRoleInfo && dbRoleInfo.Faction == faction)
                    {
                        dbRoleInfoList.Add(dbRoleInfo);
                    }
                }
            }

            return dbRoleInfoList;
        }

        /// <summary>
        /// 释放空闲的角色信息
        /// </summary>
        public void ReleaseIdleDBRoleInfos(int ticksSlot)
        {
            long nowTicks = DateTime.Now.Ticks / 10000;
            List<int> idleRoleIDList = new List<int>();
            lock (DictRoleInfos)
            {
                foreach (var weakRef in DictRoleInfos.Values)
                {
                    if (weakRef.IsAlive) //判断是否仍在存活
                    {
                        DBRoleInfo dbRoleInfo = (weakRef.Target as DBRoleInfo);
                        if (null != dbRoleInfo)
                        {
                            if (dbRoleInfo.ServerLineID <= 0 && nowTicks - dbRoleInfo.LastReferenceTicks >= ticksSlot)
                            {
                                idleRoleIDList.Add(dbRoleInfo.RoleID);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < idleRoleIDList.Count; i++)
            {
                RemoveDBRoleInfo(idleRoleIDList[i]);
                LogManager.WriteLog(LogTypes.Info, string.Format("释放空闲的角色数据: {0}", idleRoleIDList[i]));
            }
        }

        /// <summary>
        /// 释放某个指定的角色信息
        /// </summary>
        public void ReleaseDBRoleInfoByID(int roleID)
        {
            DBRoleInfo dbRoleInfo = FindDBRoleInfo(roleID);
            if (null == dbRoleInfo) return;

            //触发登出事件
            GlobalEventSource.getInstance().fireEvent(new PlayerLogoutEventObject(dbRoleInfo));

            RemoveDBRoleInfo(dbRoleInfo.RoleID);
            LogManager.WriteLog(LogTypes.SQL, string.Format("释放指定角色的数据: {0}", dbRoleInfo.RoleID));
        }

        /// <summary>
        /// 从数据库中加载最近登录过的角色列表做缓存
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userID"></param>
        public void LoadDBRoleInfos(DBManager dbMgr, MySQLConnection conn)
        {
            MySQLSelectCommand cmd = new MySQLSelectCommand(conn,
                new string[] { "rid", "userid", "rname", "sex", "occupation", "level", "pic", "faction", "money1", "money2", "experience", "pkmode", "pkvalue", "position", "regtime", "lasttime", "bagnum", "othername", "main_quick_keys", "other_quick_keys", "loginnum", "leftfightsecs", "horseid", "petid", "interpower", "totalonlinesecs", "antiaddictionsecs", "logofftime", "biguantime", "yinliang", "total_jingmai_exp", "jingmai_exp_num", "lasthorseid", "skillid", "autolife", "automagic", "numskillid", "maintaskid", "pkpoint", "lianzhan", "killboss", "battlenamestart", "battlenameindex", "cztaskid", "battlenum", "heroindex", "logindayid", "logindaynum", "zoneid", "bhname", "bhverify", "bhzhiwu", "bgdayid1", "bgmoney", "bgdayid2", "bggoods", "banggong", "huanghou", "jiebiaodayid", "jiebiaonum", "username", "lastmailid", "onceawardflag", "banchat", "banlogin" },
                new string[] { "t_roles" }, new object[,] { { "isdel", "=", 0 } }, null, null);
            if (cmd.Table.Rows.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < cmd.Table.Rows.Count; i++)
            {
                DBRoleInfo dbRoleInfo = new DBRoleInfo();

                /// 将数据库中获取的数据转换为角色数据
                DBRoleInfo.DBTableRow2RoleInfo(dbRoleInfo, cmd, i);

                //查询已经完成的任务信息
                MySQLSelectCommand cmd1 = new MySQLSelectCommand(conn,
                     new string[] { "pname", "pvalue" },
                     new string[] { "t_roleparams" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_角色参数表
                DBRoleInfo.DBTableRow2RoleInfo_Params(dbRoleInfo, cmd1);

                //查询已经完成的任务信息
                cmd1 = new MySQLSelectCommand(conn,
                     new string[] { "rid", "taskid", "count" },
                     new string[] { "t_taskslog" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_旧任务数据
                DBRoleInfo.DBTableRow2RoleInfo_OldTasks(dbRoleInfo, cmd1);

                //查询正在做的任务信息
                cmd1 = new MySQLSelectCommand(conn,
                     new string[] { "Id", "rid", "taskid", "value1", "value2", "focus", "addtime", "starlevel" },
                     new string[] { "t_tasks" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID }, { "isdel", "=", 0 } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_正在做任务数据
                DBRoleInfo.DBTableRow2RoleInfo_DoingTasks(dbRoleInfo, cmd1);

                //查询已经获取的物品列表信息
                cmd1 = new MySQLSelectCommand(conn,
                     new string[] { "id", "goodsid", "isusing", "forge_level", "starttime", "endtime", "site", "quality", "Props", "gcount", "binding", "jewellist", "bagindex", "salemoney1", "saleyuanbao", "saleyinpiao", "addpropindex", "bornindex", "lucky", "strong", "excellenceinfo", "appendproplev", "equipchangelife" },
                     new string[] { "t_goods" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID }, { "gcount", ">", 0 } }, null, null);
                /// 将数据库中获取的数据转换为角色数据_物品数据
                 DBRoleInfo.DBTableRow2RoleInfo_Goods(dbRoleInfo, cmd1);
                
                //查询已经获取的物品限制列表信息
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "goodsid", "dayid", "usednum" },
                         new string[] { "t_goodslimit" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_物品数据
                DBRoleInfo.DBTableRow2RoleInfo_GoodsLimit(dbRoleInfo, cmd1);

                //查询已经学习的节能列表信息
                cmd1 = new MySQLSelectCommand(conn,
                     new string[] { "Id", "otherid", "friendType" },
                     new string[] { "t_friends" }, new object[,] { { "myid", "=", dbRoleInfo.RoleID } }, null, null); ;

                /// 将数据库中获取的数据转换为角色数据_好友数据
                DBRoleInfo.DBTableRow2RoleInfo_Friends(dbRoleInfo, cmd1);

                //查询坐骑的列表信息
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "Id", "horseid", "bodyid", "propsNum", "PropsVal", "addtime", "failednum", "temptime", "tempnum", "faileddayid" },
                         new string[] { "t_horses" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID }, { "isdel", "=", 0 } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_坐骑数据
                DBRoleInfo.DBTableRow2RoleInfo_Horses(dbRoleInfo, cmd1);

                //查询宠物的列表信息
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "Id", "petid", "petname", "pettype", "feednum", "realivenum", "addtime", "props", "level" },
                         new string[] { "t_pets" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID }, { "isdel", "=", 0 } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_宠物数据
                DBRoleInfo.DBTableRow2RoleInfo_Pets(dbRoleInfo, cmd1);

                //查询经脉的列表信息
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "Id", "jmid", "jmlevel", "bodylevel" },
                         new string[] { "t_jingmai" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_经脉数据
                DBRoleInfo.DBTableRow2RoleInfo_JingMais(dbRoleInfo, cmd1);

                //查询经脉的列表信息
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "Id", "skillid", "skilllevel", "usednum" },
                         new string[] { "t_skills" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_技能数据
                DBRoleInfo.DBTableRow2RoleInfo_Skills(dbRoleInfo, cmd1);

                //查询Buffer的列表信息
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "bufferid", "starttime", "buffersecs", "bufferval", },
                         new string[] { "t_buffer" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_Buffer数据
                DBRoleInfo.DBTableRow2RoleInfo_Buffers(dbRoleInfo, cmd1);

                //查询跑环任务的列表信息
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "huanid", "rectime", "recnum", "taskClass", "extdayid", "extnum" },
                         new string[] { "t_dailytasks" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_跑环任务数据
                DBRoleInfo.DBTableRow2RoleInfo_DailyTasks(dbRoleInfo, cmd1);

                //查询每日冲穴次数的信息
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "jmtime", "jmnum" },
                         new string[] { "t_dailyjingmai" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_跑环任务数据
                DBRoleInfo.DBTableRow2RoleInfo_DailyJingMai(dbRoleInfo, cmd1);

                //查询随身仓库的信息
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "extgridnum" },
                         new string[] { "t_ptbag" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_随身仓库数据
                DBRoleInfo.DBTableRow2RoleInfo_PortableBag(dbRoleInfo, cmd1);

                //查询角色活动送礼的信息
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "loginweekid", "logindayid", "loginnum", "newstep", "steptime", "lastmtime", "curmid", "curmtime", "songliid", "logingiftstate", "onlinegiftstate", "lastlimittimehuodongid", "lastlimittimedayid", "limittimeloginnum", "limittimegiftstate", "serieslogingetawardstep", "seriesloginawarddayid", "seriesloginawardgoodsid", "everydayonlineawardgoodsid"},
                         new string[] { "t_huodong" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_送礼活动数据
                DBRoleInfo.DBTableRow2RoleInfo_HuodongData(dbRoleInfo, cmd1);

                //查询角色副本信息 --  修正注释 [11/15/2013 LiaoWei]
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "fubenid", "dayid", "enternum", "quickpasstimer", "finishnum" },
                         new string[] { "t_fuben" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                //将数据库中获取的数据转换为角色数据_副本数据
                DBRoleInfo.DBTableRow2RoleInfo_FuBenData(dbRoleInfo, cmd1);

                //查询角色日常数据的信息
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "expdayid", "todayexp", "linglidayid", "todaylingli", "killbossdayid", "todaykillboss", "fubendayid", "todayfubennum", "wuxingdayid", "wuxingnum" },
                         new string[] { "t_dailydata" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                //将数据库中获取的数据转换为角色数据_日常数据
                DBRoleInfo.DBTableRow2RoleInfo_DailyData(dbRoleInfo, cmd1);

                //查询角色押镖数据的信息
                cmd1 = new MySQLSelectCommand(conn,
                         new string[] { "yabiaoid", "starttime", "state", "lineid", "toubao", "yabiaodayid", "yabiaonum", "takegoods" },
                         new string[] { "t_yabiao" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                //将数据库中获取的数据转换为角色数据_押镖数据
                DBRoleInfo.DBTableRow2RoleInfo_YaBiaoData(dbRoleInfo, cmd1);

                //vip每日数据
                cmd = new MySQLSelectCommand(conn,
                         new string[] { "rid", "prioritytype", "dayid", "usedtimes" },
                         new string[] { "t_vipdailydata" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                //将数据库中获取的数据转换为角色数据_日常数据
                DBRoleInfo.DBTableRow2RoleInfo_VipDailyData(dbRoleInfo, cmd);

                //杨公宝库每日数据
                cmd = new MySQLSelectCommand(conn,
                         new string[] { "rid", "jifen", "dayid", "awardhistory" },
                         new string[] { "t_yangguangbkdailydata" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID } }, null, null);

                //将数据库中获取的数据转换为角色数据_日常数据
                DBRoleInfo.DBTableRow2RoleInfo_YangGongBKDailyJiFenData(dbRoleInfo, cmd);

                //查询翅膀的列表信息
                cmd = new MySQLSelectCommand(conn,
                         new string[] { "Id", "wingid", "forgeLevel", "addtime", "failednum", "equiped", "starexp" },
                         new string[] { "t_wings" }, new object[,] { { "rid", "=", dbRoleInfo.RoleID }, { "isdel", "=", 0 } }, null, null);

                /// 将数据库中获取的数据转换为角色数据_翅膀数据
                DBRoleInfo.DBTableRow2RoleInfo_Wings(dbRoleInfo, cmd);

                cmd1 = null;

                //更新点将积分数据
                DBQuery.QueryDJPointData(dbMgr, dbRoleInfo);

                AddDBRoleInfo(dbRoleInfo);
            }

            cmd = null;
        }
    }
}
