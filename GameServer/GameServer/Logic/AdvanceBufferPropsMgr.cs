using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Logic
{
    /// <summary>
    /// 成就、武学、经脉等高级buffer管理
    /// </summary>
    public class AdvanceBufferPropsMgr
    {
        #region 缓存字典

        /// <summary>
        /// 缓存字典
        /// </summary>
        private static Dictionary<int, int[]> CachingIDsDict = new Dictionary<int, int[]>();

        /// <summary>
        /// 根据BufferID获取缓存的物品ID列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static int[] GetCachingIDsByID(int id)
        {
            int[] ids = null;
            lock (CachingIDsDict)
            {
                if (!CachingIDsDict.TryGetValue(id, out ids))
                {
                    string paramName = "";
                    if (id == (int)BufferItemTypes.ChengJiu)
                    {
                        paramName = "ChengJiuBufferGoodsIDs";
                    }
                    else if (id == (int)BufferItemTypes.JingMai)
                    {
                        paramName = "JingMaiBufferGoodsIDs";
                    }
                    else if (id == (int)BufferItemTypes.WuXue)
                    {
                        paramName = "WuXueBufferGoodsIDs";
                    }
                    else if (id == (int)BufferItemTypes.ZuanHuang)
                    {
                        paramName = "ZhuanhuangBufferGoodsIDs";
                    }
                    else if (id == (int)BufferItemTypes.ZhanHun)
                    {
                        paramName = "ZhanhunBufferGoodsIDs";
                    }
                    else if (id == (int)BufferItemTypes.RongYu)
                    {
                        paramName = "RongyaoBufferGoodsIDs";
                    }
                    else if (id == (int)BufferItemTypes.JunQi)
                    {
                        paramName = "JunQiBufferGoodsIDs";
                    }
                    else if (id == (int)BufferItemTypes.MU_FRESHPLAYERBUFF)
                    {
                        paramName = "FreshPlayerBufferGoodsIDs";
                    }
                    else if (id == (int)BufferItemTypes.MU_ANGELTEMPLEBUFF1)
                    {
                        paramName = "AngelTempleGoldBuffGoodsID";

                    }
                    else if (id == (int)BufferItemTypes.MU_ANGELTEMPLEBUFF2)
                    {
                        paramName = "AngelTempleGoldBuffGoodsID";
                    }
                    else if (id == (int)BufferItemTypes.MU_JINGJICHANG_JUNXIAN)
                    {
                        paramName = "JunXianBufferGoodsIDs";
                    }
                    else if (id == (int)BufferItemTypes.MU_WORLDLEVEL)
                    {
                        paramName = "WorldLevelGoodsIDs";
                    }
                    else if (id == (int)BufferItemTypes.MU_ZHANMENGBUILD_ZHANQI)
                    {
                        paramName = "ZhanMengZhanQiBUFF";
                    }
                    else if (id == (int)BufferItemTypes.MU_ZHANMENGBUILD_JITAN)
                    {
                        paramName = "ZhanMengJiTanBUFF";
                    }
                    else if (id == (int)BufferItemTypes.MU_ZHANMENGBUILD_JUNXIE)
                    {
                        paramName = "ZhanMengJunXieBUFF";
                    }
                    else if (id == (int)BufferItemTypes.MU_ZHANMENGBUILD_GUANGHUAN)
                    {
                        paramName = "ZhanMengGuangHuanBUFF";
                    }

                    ids = GameManager.systemParamsList.GetParamValueIntArrayByName(paramName);
                    CachingIDsDict[id] = ids;
                }
            }

            return ids;
        }

        #endregion 缓存字典

        #region 属性接口

        /// <summary>
        /// 获取物品ID
        /// </summary>
        /// <param name="bufferItemType"></param>
        /// <param name="goodsIndex"></param>
        /// <returns></returns>
        public static int GetGoodsID(BufferItemTypes bufferItemType, int goodsIndex)
        {
            /// 根据BufferID获取缓存的物品ID列表
            int[] goodsIds = GetCachingIDsByID((int)bufferItemType);
            if (null == goodsIds)
            {
                return -1;
            }

            if (goodsIndex < 0 || goodsIndex >= goodsIds.Length)
            {
                return -1;
            }

            int goodsID = goodsIds[goodsIndex];
            return goodsID;
        }

        /// <summary>
        /// 获取扩展属性接口
        /// </summary>
        /// <param name="bufferItemType"></param>
        /// <param name="extPropIndexe"></param>
        /// <returns></returns>
        public static double GetExtProp(BufferItemTypes bufferItemType, ExtPropIndexes extPropIndexe, int goodsIndex)
        {
            /// 根据BufferID获取缓存的物品ID列表
            int[] goodsIds = GetCachingIDsByID((int)bufferItemType);
            if (null == goodsIds)
            {
                return 0.0;
            }

            if (goodsIndex < 0 || goodsIndex >= goodsIds.Length)
            {
                return 0.0;
            }

            int goodsID = goodsIds[goodsIndex];
            EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(goodsID);
            if (null == item)
            {
                return 0.0;
            }

            return item.ExtProps[(int)extPropIndexe];
        }

        /// <summary>
        /// 获取扩展属性接口
        /// </summary>
        /// <param name="bufferItemType"></param>
        /// <param name="extPropIndexe"></param>
        /// <returns></returns>
        public static double GetExtPropByGoodsID(BufferItemTypes bufferItemType, ExtPropIndexes extPropIndexe, int goodsID)
        {
            EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(goodsID);
            if (null == item)
            {
                return 0.0;
            }

            return item.ExtProps[(int)extPropIndexe];
        }

        #endregion 属性接口
    }
}
