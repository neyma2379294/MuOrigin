using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 点将积分排行榜数据
    /// </summary>
    [ProtoContract]     
    public class DJPointsData
    {
        /// <summary>
        /// 自己的点将积分数据
        /// </summary>
        [ProtoMember(1)]        
        public DJPointData SelfDJPointData = null;

        /// <summary>
        /// 热门的点将积分数据列表
        /// </summary>
        [ProtoMember(2)]        
        public List<DJPointData> HotDJPointDataList = null;
    }
}
