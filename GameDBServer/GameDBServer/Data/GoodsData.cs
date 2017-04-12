using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.Reflection;
using GameDBServer.DB;

namespace Server.Data
{
    /// <summary>
    /// 物品数据
    /// </summary>
    [ProtoContract]
    public class GoodsData
    {

        /// <summary>
        /// 数据库流水ID
        /// </summary>
        [ProtoMember(1)]
        [DBMapping(ColumnName="Id")]
        public int Id;

        /// <summary>
        /// 物品ID
        /// </summary>
        [ProtoMember(2)]
        [DBMapping(ColumnName = "goodsid")]
        public int GoodsID;

        /// <summary>
        /// 是否正在使用
        /// </summary>
        [ProtoMember(3)]
        [DBMapping(ColumnName = "isusing")]
        public int Using;

        /// <summary>
        /// 锻造级别
        /// </summary>
        [ProtoMember(4)]
        [DBMapping(ColumnName = "forge_level")]
        public int Forge_level;

        /// <summary>
        /// 开始使用的时间
        /// </summary>
        [ProtoMember(5)]
        [DBMapping(ColumnName = "starttime")]
        public string Starttime;

        /// <summary>
        /// 物品使用截止时间
        /// </summary>
        [ProtoMember(6)]
        [DBMapping(ColumnName = "endtime")]
        public string Endtime;

        /// <summary>
        /// 所在的位置(0: 包裹, 1:仓库)
        /// </summary>
        [ProtoMember(7)]
        [DBMapping(ColumnName = "site")]
        public int Site;

        /// <summary>
        /// 物品的品质(某些装备会分品质，不同的品质属性不同，用户改变属性后要记录下来)
        /// </summary>
        [ProtoMember(8)]
        [DBMapping(ColumnName = "quality")]
        public int Quality;

        /// <summary>
        /// 根据品质随机抽取的扩展属性的索引列表
        /// </summary>
        [ProtoMember(9)]
        [DBMapping(ColumnName = "Props")]
        public string Props;

        /// <summary>
        /// 物品数量
        /// </summary>
        [ProtoMember(10)]
        [DBMapping(ColumnName = "gcount")]
        public int GCount;

        /// <summary>
        /// 是否绑定的物品(绑定的物品不可交易, 不可摆摊)
        /// </summary>
        [ProtoMember(11)]
        [DBMapping(ColumnName = "binding")]
        public int Binding;

        /// <summary>
        /// 根据品质随机抽取的扩展属性的索引列表
        /// </summary>
        [ProtoMember(12)]
        [DBMapping(ColumnName = "jewellist")]
        public string Jewellist;

        /// <summary>
        /// 根据品质随机抽取的扩展属性的索引列表
        /// </summary>
        [ProtoMember(13)]
        [DBMapping(ColumnName = "bagindex")]
        public int BagIndex;

        /// <summary>
        /// 出售的金币价格
        /// </summary>
        [ProtoMember(14)]
        [DBMapping(ColumnName = "salemoney1")]
        public int SaleMoney1;

        /// <summary>
        /// 出售的元宝价格
        /// </summary>
        [ProtoMember(15)]
        [DBMapping(ColumnName = "saleyuanbao")]
        public int SaleYuanBao;

        /// <summary>
        /// 出售的银两价格
        /// </summary>
        [ProtoMember(16)]
        [DBMapping(ColumnName = "saleyinpiao")]
        public int SaleYinPiao;

        /// <summary>
        /// 出售的银两价格
        /// </summary>
        [ProtoMember(17)]
        [DBMapping(ColumnName = "addpropindex")]
        public int AddPropIndex;

        /// <summary>
        /// 增加一个天生属性的百分比
        /// </summary>
        [ProtoMember(18)]
        [DBMapping(ColumnName = "bornindex")]
        public int BornIndex;

        /// <summary>
        /// 装备的幸运值
        /// </summary>
        [ProtoMember(19)]
        [DBMapping(ColumnName = "lucky")]
        public int Lucky;

        /// <summary>
        /// 装备的耐久度
        /// </summary>
        [ProtoMember(20)]
        [DBMapping(ColumnName = "strong")]
        public int Strong;

        // 新增物品属性 [12/13/2013 LiaoWei]
        /// <summary>
        /// 卓越信息 -- 一个32位int 每位代表一个卓越属性
        /// </summary>
        [ProtoMember(21)]
        [DBMapping(ColumnName = "excellenceinfo")]
        public int ExcellenceInfo;

        // 新增物品属性 [12/18/2013 LiaoWei]
        /// <summary>
        /// 追加等级
        /// </summary>
        [ProtoMember(22)]
        [DBMapping(ColumnName = "appendproplev")]
        public int AppendPropLev;

        // 新增物品属性  [2/15/2014 LiaoWei]
        /// <summary>
        /// 装备的转生级别
        /// </summary>
        [ProtoMember(23)]
        [DBMapping(ColumnName = "equipchangelife")]
        public int ChangeLifeLevForEquip;

        /// <summary>
        /// 装备洗炼属性
        /// 结构: 属性ID|属性值|属性ID|属性值|属性ID|属性值...
        /// </summary>
        [ProtoMember(24)]
        public List<int> WashProps;

        /// <summary>
        /// 元素之心的相关属性
        /// </summary>
        [ProtoMember(25)]
        public List<int> ElementhrtsProps;

    }
}
