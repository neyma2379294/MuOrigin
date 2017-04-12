using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
    /// <summary>
    /// 已经穿戴的装备的管理
    /// </summary>
    public class UsingEquipManager
    {
        #region 数据结构

        /// <summary>
        /// 装备字典
        /// </summary>
        private Dictionary<int, List<GoodsData>> EquipDict = new Dictionary<int, List<GoodsData>>();

        /// <summary>
        /// 武器
        /// </summary>
        private GoodsData WeaponEquip = null;

        /// <summary>
        /// 武器列表 -- 耐久改造 [4/14/2014 LiaoWei]
        /// </summary>
        private List<GoodsData> WeaponEquipList = new List<GoodsData>();

        /// <summary>
        /// 武器之外的装备列表
        /// </summary>
        private List<GoodsData> EquipList = new List<GoodsData>();

        #endregion 数据结构

        #region 接口方法

        /// <summary>
        /// 能否佩戴装备
        /// </summary>
        /// <param name="goodsData"></param>
        /// <returns></returns>
        public bool CanUsingEquip(GameClient client, GoodsData goodsData, int toBagIndex, bool hintClient = false)
        {
            if (null == goodsData)
            {
                return false;
            }

            SystemXmlItem systemGoods = null;
            if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
            {
                return false;
            }

            // 增加一级属性条件 [6/16/2014 LiaoWei]
            int nRet = 1;

            nRet = EquipFirstPropCondition(client, systemGoods);

            if (nRet == -1)
            {
                GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client,
                            StringUtil.substitute(Global.GetLang("您的力量没有达到该装备的需求")), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);

                return false;
            }

            if (nRet == -2)
            {
                GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client,
                            StringUtil.substitute(Global.GetLang("您的智力没有达到该装备的需求")), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);

                return false;
            }

            if (nRet == -3)
            {
                GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client,
                            StringUtil.substitute(Global.GetLang("您的敏捷没有达到该装备的需求")), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);

                return false;
            }

            if (nRet == -4)
            {
                GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client,
                            StringUtil.substitute(Global.GetLang("您的体力没有达到该装备的需求")), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);

                return false;
            }

            int usingEquipResult = _CanUsingEquip(client, goodsData, toBagIndex, systemGoods);
            if (usingEquipResult < 0)
            {
                if (hintClient)
                {
                    string goodsName = systemGoods.GetStringValue("Title");

                    if (-3 == usingEquipResult)
                    {
                        GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client,
                            StringUtil.substitute(Global.GetLang("{0}佩戴数量超过最大限制"), goodsName), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);
                    }
                    else if (-2 == usingEquipResult)
                    {
                        GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client,
                            StringUtil.substitute(Global.GetLang("{0}守护宠物与跟随宠物只能选择一个携带"), goodsName), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);
                    }
                    else if (-1 == usingEquipResult)
                    {
                        GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client,
                            StringUtil.substitute(Global.GetLang("无法对{0}进行穿戴验证"), goodsName), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);
                    }
                    else if (-5 == usingEquipResult)
                    {
                        GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client,
                            StringUtil.substitute(Global.GetLang("{0}装备类型与已穿戴武器不匹配，请卸下其他武器再穿戴"), goodsName), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);
                    }
                    else if (-4 == usingEquipResult)
                    {
                        GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client,
                            StringUtil.substitute(Global.GetLang("同一个装备位置不能佩戴多个{0}"), goodsName), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);
                    }
                    else if (-44 == usingEquipResult)
                    {
                        GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client,
                            StringUtil.substitute(Global.GetLang("蕴神灵戒, 只能佩戴一个")), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);
                    }
                    else if (-444 == usingEquipResult)
                    {
                        GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client,
                            StringUtil.substitute(Global.GetLang("腾云手镯, 只能佩戴一个")), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);
                    }
                    else if (-55 == usingEquipResult)
                    {
                        GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client,
                            StringUtil.substitute(Global.GetLang("此位置不能佩戴{0}"), goodsName), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox);
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 能否佩戴装备
        /// </summary>
        /// <param name="goodsData"></param>
        /// <returns></returns>
        public int _CanUsingChongWu(int nCategoriy)
        {
            List<GoodsData> listSpecial = null;
            // 守护宠物与跟随宠物只能带一个
            if ((int)ItemCategories.ShouHuChong == nCategoriy)
            {
                if (!EquipDict.TryGetValue((int)ItemCategories.ChongWu, out listSpecial))
                {
                    return 0;
                }
            }

            // 守护宠物与跟随宠物只能带一个
            if ((int)ItemCategories.ChongWu == nCategoriy)
            {
                if (!EquipDict.TryGetValue((int)ItemCategories.ShouHuChong, out listSpecial))
                {
                    return 0;
                }
            }

            if (null != listSpecial && listSpecial.Count > 0)
            {
                return -2;
            }

            return 0;
        }

        /// <summary>
        /// 能否佩戴装备
        /// </summary>
        /// <param name="goodsData"></param>
        /// <returns></returns>
        public int _CanUsingEquip(GameClient client, GoodsData goodsData, int toBagIndex, SystemXmlItem systemGoods = null)
        {
            if (null == systemGoods)
            {
                if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
                {
                    return -1;
                }
            }

            int categoriy = systemGoods.GetIntValue("Categoriy");
            if (categoriy < 0 || categoriy >= (int)ItemCategories.EquipMax)
            {
                return -2;
            }

            /*if ((int)ItemCategories.Ring == categoriy || (int)ItemCategories.Bracelet == categoriy)
            {
                //这样的戒指和手镯无法佩戴上，所以也是非法的
                if (toBagIndex < 0 || toBagIndex > 1)
                {
                    return -55;
                }
            }*/


            // 校验武器是否能装备，不管是否穿戴过，都要进行验证[7/12/2013 ChenXiaojun]
            int nHandType = systemGoods.GetIntValue("HandType");
            if (categoriy < (int)ItemCategories.HuFu && categoriy >= (int)ItemCategories.WuQi_Jian)
            {                
                int nActionType = systemGoods.GetIntValue("ActionType");

                // 进行穿戴类型验证
                int nRet = WeaponAdornManager.VerifyWeaponCanEquip(Global.CalcOriginalOccupationID(client),
                                                                   nHandType,
                                                                   nActionType,
                                                                   EquipDict);

                if (nRet < 0)
                {
                    return nRet;
                }
            }

            List<GoodsData> list = null;            
            if (!EquipDict.TryGetValue(categoriy, out list))
            {
                return _CanUsingChongWu(categoriy);
            }
            else                    // MU 增加逻辑 [11/19/2013 LiaoWei]
            {
                // 如果是武器
                int nCount = list.Count;
                if (categoriy < (int)ItemCategories.HuFu && categoriy >= (int)ItemCategories.WuQi_Jian)
                {
                    if (nHandType == (int)WeaponHandType.WHT_BOTHTYPE)
                    {
                        if (nCount >= 2)
                            return -3;
                        else
                            return 0;
                    }
                }   
                // 如果是戒指
                else if (categoriy == (int)ItemCategories.JieZhi)
                {
                    if (nCount >= 2)
                        return -3;
                    else
                        return 0;
                }
                // 如果是宠物
                else if (categoriy == (int)ItemCategories.ShouHuChong || categoriy == (int)ItemCategories.ChongWu)
                {
                    int nRet = _CanUsingChongWu(categoriy);
                    if (nRet < 0)
                    {
                        return nRet;
                    }
                }
            }

           /* if ((int)ItemCategories.Ring == categoriy || (int)ItemCategories.Bracelet == categoriy)
            {
                //蕴神灵戒 只能佩戴一个，特殊处理下
                if ((goodsData.GoodsID >= 1305000 && goodsData.GoodsID <= 1305003) && null != list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if ((list[i].GoodsID >= 1305000 && list[i].GoodsID <= 1305003))
                        {
                            return -44;
                        }
                    }
                }

                //腾云手镯 只能佩戴一个，特殊处理下
                if ((goodsData.GoodsID >= 1303000 && goodsData.GoodsID <= 1303003) && null != list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if ((list[i].GoodsID >= 1303000 && list[i].GoodsID <= 1303003))
                        {
                            return -444;
                        }
                    }
                }

                if (list.Count >= 2)
                {
                    return -3;
                }

                for (int i = 0; i < list.Count; i++)
                {
                    if (toBagIndex == list[i].BagIndex)
                    {
                        return -4; //不能重复的使用
                    }
                }

                return 0;
            }*/
            
            return (list.Count < 1) ? 0 : -3;
        }

        /// <summary>
        /// 装备一级属性判断
        /// </summary>
        /// <param name="goodsData"></param>
        /// <returns></returns>
        public int EquipFirstPropCondition(GameClient client, SystemXmlItem systemGoods = null)
        {
            int nNeedStrength       = 0;    // 力量
            int nNeedIntelligence   = 0;    // 智力
            int nNeedDexterity      = 0;    // 敏捷
            int nNeedConstitution   = 0;    // 体力

            nNeedStrength       = systemGoods.GetIntValue("Strength");
            nNeedIntelligence   = systemGoods.GetIntValue("Intelligence");
            nNeedDexterity      = systemGoods.GetIntValue("Dexterity");
            nNeedConstitution   = systemGoods.GetIntValue("Constitution");

            if (nNeedStrength > 0 && nNeedStrength > RoleAlgorithm.GetStrength(client))
                return -1;
            
            if (nNeedIntelligence > 0 && nNeedIntelligence > RoleAlgorithm.GetIntelligence(client))
                return -2;
            
            if (nNeedDexterity > 0 && nNeedDexterity > RoleAlgorithm.GetDexterity(client))
                return -3;
            
            if (nNeedConstitution > 0 && nNeedConstitution > RoleAlgorithm.GetConstitution(client))
                return -4;
            
            return 1;
        }

        /// <summary>
        /// 刷新装备
        /// </summary>
        /// <param name="goodsData"></param>
        public void RefreshEquip(GoodsData goodsData)
        {
            if (null == goodsData)
            {
                return;
            }

            SystemXmlItem systemGoods = null;
            if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
            {
                return;
            }

            int categoriy = systemGoods.GetIntValue("Categoriy");
            if (categoriy < 0 || categoriy >= (int)ItemCategories.EquipMax)
            {
                return;
            }

            List<GoodsData> list = null;
            if (!EquipDict.TryGetValue(categoriy, out list))
            {
                list = new List<GoodsData>();
                EquipDict[categoriy] = list;
            }

            if (goodsData.Using <= 0)
            {
                list.Remove(goodsData);
                if (categoriy == (int)ItemCategories.XiangLian || (categoriy >= (int)ItemCategories.WuQi_Jian && categoriy <= (int)ItemCategories.WuQi_NuJianTong))
                {
                    //WeaponEquip = null;
                    
                    // 耐久改造 [4/14/2014 LiaoWei]
                    lock (WeaponEquipList)
                    {
                        WeaponEquipList.Remove(goodsData);
                    }
                }
                else
                {
                    lock (EquipList)
                    {
                        EquipList.Remove(goodsData);
                    }
                }
            }
            else
            {
                if (list.IndexOf(goodsData) < 0)
                {
                    list.Add(goodsData);
                }

                if (categoriy == (int)ItemCategories.XiangLian || (categoriy >= (int)ItemCategories.WuQi_Jian && categoriy <= (int)ItemCategories.WuQi_NuJianTong))
                {
                    //WeaponEquip = goodsData;

                    // 耐久改造 [4/14/2014 LiaoWei]
                    lock (WeaponEquipList)
                    {
                        if (WeaponEquipList.IndexOf(goodsData) < 0)
                        {
                            WeaponEquipList.Add(goodsData);
                        }
                    }
                }
                else
                {
                    lock (EquipList)
                    {
                        if (EquipList.IndexOf(goodsData) < 0)
                        {
                            EquipList.Add(goodsData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刷新装备
        /// </summary>
        /// <param name="goodsData"></param>
        public void RefreshEquips(GameClient client)
        {
            if (null == client.ClientData.GoodsDataList)
            {
                return;
            }

            if (client.ClientData.GoodsDataList.Count <= 0)
            {
                return;
            }

            lock (client.ClientData.GoodsDataList)
            {
                List<GoodsData> toCorrectGoodsDataList = new List<GoodsData>();
                for (int i = 0; i < client.ClientData.GoodsDataList.Count; i++)
                {
                    if (client.ClientData.GoodsDataList[i].Using <= 0)
                    {
                        continue;
                    }

                    if (_CanUsingEquip(client, client.ClientData.GoodsDataList[i], client.ClientData.GoodsDataList[i].BagIndex, null) < 0)
                    {
                        toCorrectGoodsDataList.Add(client.ClientData.GoodsDataList[i]);
                        continue;
                    }

                    //刷新装备
                    RefreshEquip(client.ClientData.GoodsDataList[i]);
                }

                for (int i = 0; i < toCorrectGoodsDataList.Count; i++)
                {
                    GoodsData goodsData = toCorrectGoodsDataList[i];

                    goodsData.Using = 0;
                    Global.ResetBagGoodsData(client, goodsData);
                }
            }
        }

        #endregion 接口方法

        #region 耐久度管理

        /// <summary>
        /// 攻击某人时减少装备耐久度
        /// </summary>
        /// <param name="client"></param>
        public void AttackSomebody(GameClient client)
        {
            // 耐久改造 [4/14/2014 LiaoWei]
            /*if (null == WeaponEquip)
            {
                return;
            }

            GoodsData goodsData = WeaponEquip;*/

            if (WeaponEquipList.Count <= 0)
                return;
            
            GoodsData goodsData = null;
            lock (WeaponEquipList)
            {
                goodsData = WeaponEquipList[Global.GetRandomNumber(0, WeaponEquipList.Count)];
            }

            //减少指定装备的耐久度(反向的, 增加后，最大值减去这个值，就是减少)
            GameManager.ClientMgr.AddEquipStrong(client, goodsData, 1);
        }

        /// <summary>
        /// 被攻击时减少装备耐久度
        /// </summary>
        /// <param name="client"></param>
        public void InjuredSomebody(GameClient client)
        {
            if (EquipList.Count <= 0)
            {
                return;
            }

            GoodsData goodsData = null;
            lock (EquipList)
            {
                goodsData = EquipList[Global.GetRandomNumber(0, EquipList.Count)];
            }

            //减少指定装备的耐久度(反向的, 增加后，最大值减去这个值，就是减少)
            GameManager.ClientMgr.AddEquipStrong(client, goodsData, 1);
        }

        /// <summary>
        /// GM命令增减装备耐久值
        /// </summary>
        /// <param name="client"></param>
        /// <param name="val"></param>
        public void GMAddEquipStrong(GameClient client, int val)
        {
            if (EquipList.Count <= 0)
            {
                return;
            }

            List<GoodsData> goodsDataList = new List<GoodsData>();
            lock (EquipList)
            {
                goodsDataList.AddRange(EquipList);
            }
            lock (WeaponEquipList)
            {
                goodsDataList.AddRange(WeaponEquipList);
            }
            foreach (var goodsData in goodsDataList)
            {
                //减少指定装备的耐久度(反向的, 增加后，最大值减去这个值，就是减少)
                GameManager.ClientMgr.AddEquipStrong(client, goodsData, val * 500);
            }
        }

        #endregion 耐久度管理

        #region 根据类型获取装备接口

        /// <summary>
        /// 根据传入的角色对象,以及装备类型,获取装备的GoodsData对象
        /// </summary>
        /// <param name="client"></param>
        /// <param name="categoriy"></param>
        /// <returns></returns>
        public GoodsData GetGoodsDataByCategoriy(GameClient client, int categoriy)
        {
            List<GoodsData> list = null;
            if (!EquipDict.TryGetValue(categoriy, out list))
            {
                return null;
            }

            if (null == list || list.Count <= 0)
            {
                return null;
            }

            return list[0];
        }

        #endregion 根据类型获取装备接口
    }
}
