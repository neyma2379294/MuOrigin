using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 砸金蛋历史记录项数据
    /// </summary>
    [ProtoContract]
    public class ZaJinDanHistory
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [ProtoMember(1)]
        public int RoleID = 0;

        /// <summary>
        /// 角色名
        /// </summary>
        [ProtoMember(2)]
        public string RoleName = "";

        /// <summary>
        /// 次数选择
        /// </summary>
        [ProtoMember(3)]
        public int TimesSelected = 0;

        /// <summary>
        /// 消耗元宝
        /// </summary>
        [ProtoMember(4)]
        public int UsedYuanBao = 0;

        /// <summary>
        /// 消耗金蛋个数
        /// </summary>
        [ProtoMember(5)]
        public int UsedJinDan = 0;

        /// <summary>
        /// 得到物品id
        /// </summary>
        [ProtoMember(6)]
        public int GainGoodsId = 0;

        /// <summary>
        /// 得到物品数量[一个]
        /// </summary>
        [ProtoMember(7)]
        public int GainGoodsNum = 0;

        /// <summary>
        /// 得到金币
        /// </summary>
        [ProtoMember(8)]
        public int GainGold = 0;

        /// <summary>
        /// 得到银两
        /// </summary>
        [ProtoMember(9)]
        public int GainYinLiang = 0;

        /// <summary>
        /// 得到经验
        /// </summary>
        [ProtoMember(10)]
        public int GainExp = 0;

        /// <summary>
        /// 物品属性
        /// </summary>
        [ProtoMember(11)]
        public string GoodPorp = "";

        /// <summary>
        /// 砸金蛋操作时间
        /// </summary>
        [ProtoMember(12)]
        public string OperationTime = "";
    }
}
