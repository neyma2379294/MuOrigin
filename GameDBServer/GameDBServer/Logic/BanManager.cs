using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 角色禁止登陆管理
    /// </summary>
    public class BanManager
    {
        #region 基础数据

        /// <summary>
        /// 存储禁止登陆的角色字典
        /// </summary>
        private static Dictionary<string, int> _RoleNameDict = new Dictionary<string, int>();

        /// <summary>
        /// 存储禁止登陆的角色时间字典
        /// </summary>
        private static Dictionary<string, long> _RoleNameTicksDict = new Dictionary<string, long>();

        #endregion 基础数据

        #region 基本操作

        /// <summary>
        /// 设置是否禁止某个角色名称
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="state"></param>
        public static void BanRoleName(string roleName, int state)
        {
            lock (_RoleNameDict)
            {
                _RoleNameDict[roleName] = state;
            }

            lock (_RoleNameTicksDict)
            {
                if (state > 0)
                {
                    _RoleNameTicksDict[roleName] = DateTime.Now.Ticks / 10000;
                }
                else
                {
                    _RoleNameTicksDict[roleName] = 0;
                }
            }
        }

        /// <summary>
        /// 查询是否被禁止登陆
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static int IsBanRoleName(string roleName)
        {
            int state = 0;
            lock (_RoleNameDict)
            {
                if (!_RoleNameDict.TryGetValue(roleName, out state))
                {
                    state = 0;
                }
            }

            if (state > 0)
            {
                lock (_RoleNameTicksDict)
                {
                    long oldTicks = 0;
                    if (!_RoleNameTicksDict.TryGetValue(roleName, out oldTicks))
                    {
                        state = 0;
                    }
                    else
                    {
                        long nowTicks = DateTime.Now.Ticks / 10000;
                        if (nowTicks - oldTicks >= (state * 60 * 1000))
                        {
                            state = 0;
                        }
                    }
                }
            }

            return state;
        }

        #endregion 基本操作
    }
}
