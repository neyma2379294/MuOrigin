using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Logic;
using GameDBServer.Server;
using Server.Protocol;
using Server.Tools;
using Server.Data;
using GameDBServer.DB;


namespace GameDBServer.Logic
{
    /// <summary>
    /// 游戏配置对象
    /// </summary>
    public class GameConfig
    {
        #region 基础数据

        /// <summary>
        /// 公告字典
        /// </summary>
        private Dictionary<string, string> _GameConfigDict = new Dictionary<string, string>();

        #endregion 基础数据

        #region 基础方法

        public void InitGameDBManagerFlags()
        {
            GameDBManager.Flag_t_goods_delete_immediately = GameDBManager.GameConfigMgr.GetGameConfigItemInt("flag_t_goods_delete_immediately", 1) > 0;
        }

        /// <summary>
        /// 从数据库中获取配置参数
        /// </summary>
        public void LoadGameConfigFromDB(DBManager dbMgr)
        {
            //查询游戏配置参数
            _GameConfigDict = DBQuery.QueryGameConfigDict(dbMgr);
            if (null == _GameConfigDict)
            {
                _GameConfigDict = new Dictionary<string, string>();
            }

            InitGameDBManagerFlags();
        }

        /// <summary>
        /// 更新游戏配置项
        /// </summary>
        public void UpdateGameConfigItem(string paramName, string paramValue)
        {
            lock (_GameConfigDict)
            {
                _GameConfigDict[paramName] = paramValue;
            }

            InitGameDBManagerFlags();
        }

        /// <summary>
        /// 获取游戏配置项
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public string GetGameConifgItem(string paramName)
        {
            string paramValue = null;
            lock (_GameConfigDict)
            {
                if (!_GameConfigDict.TryGetValue(paramName, out paramValue))
                {
                    paramValue = null;
                }
            }

            return paramValue;
        }

        /// <summary>
        /// 获取游戏字符串配置项
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public string GetGameConfigItemStr(string paramName, string defVal)
        {
            string ret = GetGameConifgItem(paramName);
            if (string.IsNullOrEmpty(ret))
            {
                return defVal;
            }

            return ret;
        }

        /// <summary>
        /// 获取游戏整数配置项
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public int GetGameConfigItemInt(string paramName, int defVal)
        {
            string str = GetGameConifgItem(paramName);
            if (string.IsNullOrEmpty(str))
            {
                return defVal;
            }

            int ret = 0;

            try
            {
                ret = Convert.ToInt32(str);
            }
            catch (Exception)
            {
                ret = defVal;
            }

            return ret;
        }

        /// <summary>
        /// 获取游戏浮点数配置项
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public double GetGameConfigItemInt(string paramName, double defVal)
        {
            string str = GetGameConifgItem(paramName);
            if (string.IsNullOrEmpty(str))
            {
                return defVal;
            }

            double ret = 0.0;

            try
            {
                ret = Convert.ToDouble(str);
            }
            catch (Exception)
            {
                ret = defVal;
            }

            return ret;
        }

        /// <summary>
        /// 获取游戏配置项的发送tcp对象
        /// </summary>
        public TCPOutPacket GetGameConfigDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
        {
            TCPOutPacket tcpOutPacket = null;
            lock (_GameConfigDict)
            {
                tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, string>>(_GameConfigDict, pool, cmdID);
            }

            return tcpOutPacket;
        }

        #endregion 基础方法
    }
}
