using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Server.Data
{
    /// <summary>
    /// 帮会列表信息
    /// </summary>
    [ProtoContract]
    public class BangHuiListData
    {
        /// <summary>
        /// 帮成员总的个数
        /// </summary>
        [ProtoMember(1)]
        public int TotalBangHuiItemNum = 0;

        /// <summary>
        /// 帮成员列表
        /// </summary>
        [ProtoMember(2)]
        public List<BangHuiItemData> BangHuiItemDataList = null;
    }
}
