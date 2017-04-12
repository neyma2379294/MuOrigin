using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using GameDBServer.DB;

namespace GameDBServer.Data
{
    /// <summary>
    /// 战盟事件数据
    /// 
    /// 战盟创建：用户名称（蓝色）+创建了战盟
    /// RoleName + "创建了战盟"
    /// 脱离战盟：用户名称（蓝色）+离开了战盟
    /// RoleName + "离开了战盟"
    /// 加入战盟：用户名称（蓝色）+加入了战盟
    /// RoleName + "加入了战盟"
    /// 玩家捐赠：用户名称（蓝色）+捐赠了+捐赠值+捐赠类型（钻石/金币）
    /// RoleName + "捐赠了" + SubValue1(捐赠值) + SubValue2（捐赠类型）
    /// 职位变更：用户名称（蓝色）+成为了+职位名称
    /// RoleName + "成为了" + SubValue1(职位ID)
    /// 建设升级：用户名称（蓝色）+将+建筑名称+等级提升到 + 提升后等级 + 级
    /// RoleName + "将" + SubValue1(建筑ID) + "等级提升到" + SubValue2（等级） + "级"
    /// </summary>
    [ProtoContract]
    public class ZhanMengShiJianData
    {

        /// <summary>
        /// 主键ID
        /// </summary>
        [DBMapping(ColumnName = "pkId")]
        public int PKId = 0;

        /// <summary>
        /// 帮派的ID
        /// </summary>
        [ProtoMember(1)]
        [DBMapping(ColumnName = "bhId")]
        public int BHID = 0;

        /// <summary>
        /// 事件类型
        /// </summary>
        [ProtoMember(2)]
        [DBMapping(ColumnName = "shijianType")]
        public int ShiJianType = 0;

        /// <summary>
        /// 用户名称
        /// </summary>
        [ProtoMember(3)]
        [DBMapping(ColumnName = "roleName")]
        public string RoleName = "";

        /// <summary>
        /// 触发事件时间
        /// </summary>
        [ProtoMember(4)]
        [DBMapping(ColumnName = "createTime")]
        public string CreateTime = "";

        /// <summary>
        /// 预留值
        /// </summary>
        [ProtoMember(5)]
        [DBMapping(ColumnName = "subValue1")]
        public int SubValue1 = -1;

        /// <summary>
        /// 预留值
        /// </summary>
        [ProtoMember(6)]
        [DBMapping(ColumnName = "subValue2")]
        public int SubValue2 = -1;

        /// <summary>
        /// 预留值
        /// </summary>
        [ProtoMember(7)]
        [DBMapping(ColumnName = "subValue3")]
        public int SubValue3 = -1;

//         / <summary>
//                 /// 预留值
//                 /// </summary>
//                 [ProtoMember(8)]
//                 [DBMapping(ColumnName = "SubValue4")]
//                 public int SubValue4 = -1;
//         
//                 /// <summary>
//                 /// 预留值
//                 /// </summary>
//                 [ProtoMember(9)]
//                 [DBMapping(ColumnName = "SubValue5")]
//                 public int SubValue5 = -1;
//         
//                 /// <summary>
//                 /// 预留值
//                 /// </summary>
//                 [ProtoMember(10)]
//                 [DBMapping(ColumnName = "SubValue6")]
//                 public int SubValue6 = -1;
//         
//                 /// <summary>
//                 /// 预留值
//                 /// </summary>
//                 [ProtoMember(11)]
//                 [DBMapping(ColumnName = "SubValue7")]
//                 public int SubValue7 = -1;
//         
//                 /// <summary>
//                 /// 预留值
//                 /// </summary>
//                 [ProtoMember(12)]
//                 [DBMapping(ColumnName = "SubValue8")]
//                 public int SubValue8 = -1;
//         
//                 /// <summary>
//                 /// 预留值
//                 /// </summary>
//                 [ProtoMember(13)]
//                 [DBMapping(ColumnName = "SubValue9")]
//                 public int SubValue9 = -1;
    }
}
