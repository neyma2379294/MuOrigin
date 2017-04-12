using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 用户邮件管理类
    /// </summary>
    public class UserMailManager
    {
        #region 扫描新邮件

        /// <summary>
        /// 上次扫描新邮件的时间
        /// </summary>
        private static long LastScanMailTicks = DateTime.Now.Ticks / 10000;

        /// <summary>
        /// 扫描新邮件
        /// </summary>
        public static void ScanLastMails(DBManager dbMgr)
        {
            long nowTicks = DateTime.Now.Ticks / 10000;
            if (nowTicks - LastScanMailTicks < (30 * 1000))
            {
                return;
            }

            LastScanMailTicks = nowTicks;

            //扫描新邮件
            Dictionary<int, int> lastMailDict = DBQuery.ScanLastMailIDListFromTable(dbMgr);

            if (null != lastMailDict && lastMailDict.Count > 0)
            {
                String gmCmd = "", mailIDsToDel = "";

                foreach (var item in lastMailDict)
                {
                    DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(item.Key);
                    if (null != dbRoleInfo)
                    {
                        //理论上新扫描到的肯定是最新的,如果同一个客户端同时有多封新邮件，lastMailDict中只有mailID值最大的那一封
                        if (gmCmd.Length > 0)
                        {
                            gmCmd += "_";
                        }

                        //如果本次提示用户没有打开邮件列表，下次还能继续提示
                        dbRoleInfo.LastMailID = item.Value;
                        gmCmd += String.Format("{0}|{1}", dbRoleInfo.RoleID, item.Value);
                    }
                    else
                    {
                        //缓存中没有的用户更新其数据库字段
                        DBWriter.UpdateRoleLastMail(dbMgr, item.Key, item.Value);
                    }

                    if (mailIDsToDel.Length > 0)
                    {
                        mailIDsToDel += ",";
                    }

                    mailIDsToDel += item.Value;
                }

                //通知客户端
                if (gmCmd.Length > 0)
                {
                    //添加GM命令消息
                    string gmCmdData = string.Format("-notifymail {0}", gmCmd);
                    ChatMsgManager.AddGMCmdChatMsg(-1, gmCmdData);
                }

                //清空邮件临时表中本次扫描到的信息，不能全部删除，因为处理过程中可能又有新邮件信息
                if (mailIDsToDel.Length >= 0)
                {
                    DBWriter.DeleteLastScanMailIDs(dbMgr, lastMailDict);
                }
            }
        }

        #endregion 扫描新邮件
    }
}
