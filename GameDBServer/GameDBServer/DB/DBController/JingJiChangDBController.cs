using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using MySQLDriverCS;
using GameDBServer.Logic;
using System.Data;
using Server.Tools;

namespace GameDBServer.DB.DBController
{
    /// <summary>
    /// 竞技场数据处理类
    /// </summary>
    public class JingJiChangDBController : DBController<PlayerJingJiData>
    {
        private static JingJiChangDBController instance = new JingJiChangDBController();

        private JingJiChangDBController() : base() { }

        public static JingJiChangDBController getInstance()
        {
            return instance;
        }

        /// <summary>
        /// 根据ID获取竞技场机器人数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public PlayerJingJiData getPlayerJingJiDataById(int Id)
        {
            string sql = string.Format("select * from t_jingjichang where roleId = {0};",Id);

            return this.queryForObject(sql);
        }

        /// <summary>
        /// 获取排名前500的机器人数据
        /// </summary>
        /// <returns></returns>
        public List<PlayerJingJiData> getPlayerJingJiDataList()
        {
            string sql = string.Format("select * from t_jingjichang where ranking != -1 order by ranking limit {0};", JingJiChangConstants.RankingList_Max_Num);

            return this.queryForList(sql);
        }

        /// <summary>
        /// 领取奖励后更新时间戳
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool updateNextRewardTime(int roleId, long nextRewardTime)
        {
            string sql = string.Format("update t_jingjichang set nextRewardTime={0} where roleId={1};", nextRewardTime, roleId);

            return this.update(sql) > 0;
        }

        /// <summary>
        /// 挑战失败后，更新竞技场
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool updateJingJiDataForFailed(int roleId, long nextChallengeTime)
        {
            string sql = string.Format("update t_jingjichang set winCount=0,nextChallengeTime={0} where roleId={1};", nextChallengeTime, roleId);

            return this.update(sql) > 0;
        }

        /// <summary>
        /// 被挑战成功，更新排名
        /// </summary>
        /// <returns></returns>
        public bool updateJingJiRanking(int roleId, int ranking)
        {
            string sql = string.Format("update t_jingjichang set ranking={0} where roleId={1};", ranking, roleId);

            return this.update(sql) > 0;
        }

        /// <summary>
        /// 更新挑战CD
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="nextChallengeTime"></param>
        /// <returns></returns>
        public bool updateNextChallengeTime(int roleId, long nextChallengeTime)
        {
            string sql = string.Format("update t_jingjichang set nextChallengeTime={0} where roleId={1};", nextChallengeTime, roleId);

            return this.update(sql) > 0;
        }

        /// <summary>
        /// 挑战胜利后，更新竞技场数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool updateJingJiDataForWin(PlayerJingJiData data)
        {
            data.convertString();

            string sql = "update t_jingjichang set level=@level,changeLiveCount=@changeLiveCount,winCount=@winCount,nextChallengeTime=@nextChallengeTime,baseProps=@baseProps,extProps=@extProps,equipDatas=@equipDatas,skillDatas=@skillDatas,CombatForce=@CombatForce where roleId=@roleId;";

            MySQLConnection conn = null;

            int resultCount = -1;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);

                MySQLCommand cmd = new MySQLCommand();
                
                cmd.Connection = conn;
                
                cmd.CommandType = CommandType.Text;
                
                cmd.CommandText = sql;

                cmd.Parameters.Add("@roleId", DbType.Int32);
                cmd.Parameters.Add("@level", DbType.Int32);
                cmd.Parameters.Add("@changeLiveCount", DbType.Int32);
                cmd.Parameters.Add("@winCount", DbType.Int32);
                //cmd.Parameters.Add("@ranking", DbType.Int32);
                cmd.Parameters.Add("@nextChallengeTime", DbType.Int64);
                cmd.Parameters.Add("@baseProps", DbType.String);
                cmd.Parameters.Add("@extProps", DbType.String);
                cmd.Parameters.Add("@equipDatas", DbType.String);
                cmd.Parameters.Add("@skillDatas", DbType.String);
                cmd.Parameters.Add("@CombatForce",DbType.Int32);
                

                cmd.Parameters["@roleId"].Value = data.roleId;
                cmd.Parameters["@level"].Value = data.level;
                cmd.Parameters["@changeLiveCount"].Value = data.changeLiveCount;
                cmd.Parameters["@winCount"].Value = data.winCount;
                //cmd.Parameters["@ranking"].Value = data.ranking;
                cmd.Parameters["@nextChallengeTime"].Value = data.nextChallengeTime;
                cmd.Parameters["@baseProps"].Value = data.stringBaseProps;
                cmd.Parameters["@extProps"].Value = data.stringExtProps;
                cmd.Parameters["@equipDatas"].Value = data.stringEquipDatas;
                cmd.Parameters["@skillDatas"].Value = data.stringSkillDatas;
                cmd.Parameters["@CombatForce"].Value = data.combatForce;
                

                try
                {
                    resultCount = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("向数据库更新竞技场数据失败: {0},{1}", sql, ex));
                }

                cmd.Dispose();
                cmd = null;

            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return resultCount > 0;
        
        }

        public bool insertJingJiData(PlayerJingJiData data)
        {
            data.convertString();

            string sql = "insert into t_jingjichang (roleId,roleName,name,zoneId,level,changeLiveCount,occupationId,winCount,ranking,nextRewardTime,nextChallengeTime,baseProps,extProps,equipDatas,skillDatas,CombatForce,sex) values (@roleId,@roleName,@name,@zoneId,@level,@changeLiveCount,@occupationId,@winCount,@ranking,@nextRewardTime,@nextChallengeTime,@baseProps,@extProps,@equipDatas,@skillDatas,@CombatForce,@sex)";

            MySQLConnection conn = null;

            int resultCount = -1;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);

                MySQLCommand cmd = new MySQLCommand();
               
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.Parameters.Add("@roleId",DbType.Int32);
                cmd.Parameters.Add("@roleName",DbType.String);
                cmd.Parameters.Add("@name", DbType.String);
                cmd.Parameters.Add("@zoneId", DbType.Int32);
                cmd.Parameters.Add("@level",DbType.Int32);
                cmd.Parameters.Add("@changeLiveCount",DbType.Int32);
                cmd.Parameters.Add("@occupationId",DbType.Int32);
                cmd.Parameters.Add("@winCount",DbType.Int32);
                cmd.Parameters.Add("@ranking",DbType.Int32);
                cmd.Parameters.Add("@nextRewardTime",DbType.Int64);
                cmd.Parameters.Add("@nextChallengeTime", DbType.Int64);
                cmd.Parameters.Add("@baseProps",DbType.String);
                cmd.Parameters.Add("@extProps", DbType.String);
                cmd.Parameters.Add("@equipDatas", DbType.String);
                cmd.Parameters.Add("@skillDatas", DbType.String);
                cmd.Parameters.Add("@CombatForce", DbType.Int32);
                cmd.Parameters.Add("@sex", DbType.Byte);

                cmd.Parameters["@roleId"].Value = data.roleId;
                cmd.Parameters["@roleName"].Value = data.roleName;
                cmd.Parameters["@name"].Value = data.name;
                cmd.Parameters["@zoneId"].Value = data.zoneId;
                cmd.Parameters["@level"].Value = data.level;
                cmd.Parameters["@changeLiveCount"].Value = data.changeLiveCount;
                cmd.Parameters["@occupationId"].Value = data.occupationId;
                cmd.Parameters["@winCount"].Value = data.winCount;
                cmd.Parameters["@ranking"].Value = data.ranking;
                cmd.Parameters["@nextRewardTime"].Value = data.nextRewardTime;
                cmd.Parameters["@nextChallengeTime"].Value = data.nextChallengeTime;
                cmd.Parameters["@baseProps"].Value = data.stringBaseProps;
                cmd.Parameters["@extProps"].Value = data.stringExtProps;
                cmd.Parameters["@equipDatas"].Value = data.stringEquipDatas;
                cmd.Parameters["@skillDatas"].Value = data.stringSkillDatas;
                cmd.Parameters["@CombatForce"].Value = data.combatForce;
                cmd.Parameters["@sex"].Value = data.sex;

                try
                {
                    resultCount = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("向数据库插入竞技场数据失败: {0}", sql, ex));
                }

                cmd.Dispose();
                cmd = null;

            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return resultCount > 0;
        }
    }
}
