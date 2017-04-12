using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using GameDBServer.DB;
using Server.Tools;
using GameDBServer.Logic;
using Server.Data;

namespace GameDBServer.Data
{
    /// <summary>
    /// User active data
    /// </summary>
    class AccountActiveData
    {
        /// <summary>
        /// user account
        /// </summary>
        [DBMapping(ColumnName = "Account")]
        public string strAccount;

        /// <summary>
        /// Account creation date
        /// </summary>
        [DBMapping(ColumnName = "createTime")]
        public string strCreateTime;

        /// <summary>
        /// Number of consecutive logins
        /// </summary>
        [DBMapping(ColumnName = "seriesLoginCount")]
        public int nSeriesLoginCount;

        /// <summary>
        /// Last consecutive landing date
        /// </summary>
        [DBMapping(ColumnName = "lastSeriesLoginTime")]
        public string strLastSeriesLoginTime;
    }
}
