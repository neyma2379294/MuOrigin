using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySQLDriverCS;
using System.Data;
using System.Windows;
using Server.Tools;
using GameDBServer.Logic;

namespace GameDBServer.DB
{
    /// <summary>
    /// 数据库管理类
    /// 1. 长连接
    /// 2. 多连接池，并发使用
    /// </summary>
    public class DBManager
    {
        private static DBManager instance = new DBManager();

        private DBManager() { }

        public static DBManager getInstance()
        {
            return instance;
        }

        /// <summary>
        /// 数据库连接池类
        /// </summary>
        private DBConnections _DBConns = new DBConnections();

        /// <summary>
        /// 数据库连接池
        /// </summary>
        public DBConnections DBConns
        {
            get { return _DBConns; }
        }

        /// <summary>
        /// 用户信息缓存对象
        /// </summary>
        private DBUserMgr _DBUserMgr = new DBUserMgr();

        /// <summary>
        /// 用户信息缓存对象属性
        /// </summary>
        public DBUserMgr dbUserMgr
        {
            get { return _DBUserMgr; }
        }

        /// <summary>
        /// 角色信息缓存对象
        /// </summary>
        private DBRoleMgr _DBRoleMgr = new DBRoleMgr();

        /// <summary>
        /// 角色信息缓存对象属性
        /// </summary>
        public DBRoleMgr DBRoleMgr
        {
            get { return _DBRoleMgr; }
        }

        /// <summary>
        /// 获取数据库连接池连接个数
        /// </summary>
        public int GetMaxConnsCount()
        {
            return _DBConns.GetDBConnsCount();
        }

        /// <summary>
        /// 创建内存表
        /// </summary>
        private void CreateMemTables()
        {
            //MySQLConnection dbConn = _DBConns.PopDBConnection();

            //try
            //{
            //    MySQLCommand cmd = null;
            //    cmd = new MySQLCommand("DROP TABLE IF EXISTS t_tempmoney", dbConn);
            //    cmd.ExecuteNonQuery();
            //    cmd.Dispose();

            //    cmd = new MySQLCommand("CREATE TABLE `t_tempmoney` (`uid` int(11) NOT NULL DEFAULT '0', `addmoney` int(11) NOT NULL DEFAULT '0') ENGINE=MEMORY DEFAULT CHARSET=utf8", dbConn);
            //    cmd.ExecuteNonQuery();
            //    cmd.Dispose();
            //    cmd = null;
            //}
            //finally
            //{
            //    _DBConns.PushDBConnection(dbConn);
            //}
        }

        /// <summary>
        /// 从数据库中加载数据缓存
        /// </summary>
        /// <param name="connstr">数据库连接串对象</param>
        public void LoadDatabase(MySQLConnectionString connstr, int MaxConns, int codePage)
        {
			TianMaCharSet.ConvertToCodePage = codePage;
			
            _DBConns.BuidConnections(connstr, MaxConns);
            MySQLConnection conn = _DBConns.PopDBConnection();

            try
            {
                //预先加载角色信息
                //_DBRoleMgr.LoadDBRoleInfos(this, conn);

                //查询系统公告
                GameDBManager.BulletinMsgMgr.LoadBulletinMsgFromDB(this);

                //从数据库中获取配置参数
                GameDBManager.GameConfigMgr.LoadGameConfigFromDB(this);

                //从数据库中加载品码
                LiPinMaManager.LoadLiPinMaDB(this);

                //从数据库中加载预先分配的名字
                PreNamesManager.LoadPremNamesFromDB(this);

                //副本历史记录管理
                FuBenHistManager.LoadFuBenHist(this);

                //处理排行榜
                PaiHangManager.ProcessPaiHang(this, true);

                //从数据库中获取帮旗列表
                GameDBManager.BangHuiJunQiMgr.LoadBangHuiJunQiItemFromDB(this);

                //从数据库中获取领地占领列表
                GameDBManager.BangHuiLingDiMgr.LoadBangHuiLingDiItemsDictFromDB(this);

                //皇帝特权数据项
                HuangDiTeQuanMgr.LoadHuangDiTeQuan(this);
            }
            finally
            {
                _DBConns.PushDBConnection(conn);
            }

            //创建内存表
            CreateMemTables();
        }

        /// <summary>
        /// 获取指定的用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DBUserInfo GetDBUserInfo(string userID)
        {
            DBUserInfo dbUserInfo = _DBUserMgr.FindDBUserInfo(userID);
            if (null == dbUserInfo)
            {
                dbUserInfo = new DBUserInfo();
                MySQLConnection conn = _DBConns.PopDBConnection();

                try
                {
                    if (!dbUserInfo.Query(conn, userID))
                    {
                        return null;
                    }
                }
                finally
                {
                    _DBConns.PushDBConnection(conn);
                }

                //放入缓存
                _DBUserMgr.AddDBUserInfo(dbUserInfo);
            }

            return dbUserInfo;
        }

        /// <summary>
        /// 获取指定的角色信息
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public DBRoleInfo GetDBRoleInfo(int roleID)
        {
            DBRoleInfo dbRoleInfo = _DBRoleMgr.FindDBRoleInfo(roleID);
            if (null == dbRoleInfo)
            {
                dbRoleInfo = new DBRoleInfo();
                MySQLConnection conn = _DBConns.PopDBConnection();

                try
                {
                    if (!dbRoleInfo.Query(conn, roleID))
                    {
                        return null;
                    }
                }
                finally
                {
                    _DBConns.PushDBConnection(conn);
                }

                //更新点将积分数据
                DBQuery.QueryDJPointData(this, dbRoleInfo);

                //放入缓存
                _DBRoleMgr.AddDBRoleInfo(dbRoleInfo);
            }

            return dbRoleInfo;
        }

        /// <summary>
        /// 获取指定的角色信息(不管角色是否删除)
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public DBRoleInfo GetDBAllRoleInfo(int roleID)
        {
            DBRoleInfo dbRoleInfo = _DBRoleMgr.FindDBRoleInfo(roleID);
            if (null == dbRoleInfo)
            {
                dbRoleInfo = new DBRoleInfo();
                MySQLConnection conn = _DBConns.PopDBConnection();

                try
                {
                    if (!dbRoleInfo.Query(conn, roleID, false))
                    {
                        return null;
                    }
                }
                finally
                {
                    _DBConns.PushDBConnection(conn);
                }

                //更新点将积分数据
                DBQuery.QueryDJPointData(this, dbRoleInfo);                

                //放入缓存
                _DBRoleMgr.AddDBRoleInfo(dbRoleInfo);
            }

            return dbRoleInfo;
        }


    }
}
