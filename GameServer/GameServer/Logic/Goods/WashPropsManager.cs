using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using Server.Protocol;
using GameServer.Logic;
using GameServer.Server;
using Server.Tools;
using System.Xml.Linq;
using GameServer.Server.CmdProcesser;

namespace GameServer.Logic
{
    /// <summary>
    /// 标准错误返回码
    /// </summary>
    public static class StdErrorCode
    {
        public const int Error_Success = 1; //操作成功完成
        public const int Error_Success_Bind = 2; //操作成功完成但结果为绑定物品
        public const int Error_Invalid_DBID = -1; //无效的dbid
        public const int Error_Invalid_Index = -2; //无效的索引值
        public const int Error_Config_Fault = -3; //配置错误
        public const int Error_Data_Overdue = -4; //数据无效或已过期,请清除缓存重新操作
        public const int Error_Invalid_Operation = -5; //无效操作(洗炼无可洗练属性的装备或激活已激活洗炼的装备等)
        public const int Error_Goods_Not_Enough = -6; //物品不足
        public const int Error_Goods_Is_Using = -7; //物品在使用,不允许操作
        public const int Error_Goods_Not_Find = -8; //物品在使用,不允许操作
        public const int Error_JinBi_Not_Enough = -9; //金币不足
        public const int Error_ZuanShi_Not_Enough = -10; //钻石不足
        public const int Error_Operation_Faild = -11; //操作失败(传承失败等)
        public const int Error_Operation_Denied = -12; //拒绝操作(不符合操作条件)
        public const int Error_Type_Not_Match = -13; //拒绝操作(类型不匹配)
        public const int Error_MoneyType_Not_Select = -14; //未选择消耗钱类型
        public const int Error_DB_Faild = -15; //数据库操作失败
        public const int Error_No_Residue_Degree = -16; //无剩余次数
        public const int Error_BangZuan_Not_Enough = -17; //绑钻不足
    }

    /// <summary>
    /// 消耗钱的可选类型
    /// </summary>
    public static class UseMoneyTypes
    {
        public const int JinBiOrBindJinBi = 0;
        public const int ZuanShi = 1;
        public const int JinBiOnly = 2;
        public const int ZuanShiOrBindZuanShi = 3;
    }

    /// <summary>
    /// 洗炼类型配置表
    /// </summary>
    public class XiLianType
    {
        public int ID;
        public string Color;
        public int ShuXingNum;
        public string Text;
        public double FirstShuXing;
        public double ShuXingLimitMultiplying;
    }

    /// <summary>
    /// 洗炼配置表
    /// </summary>
    public class XiLianShuXing
    {
        public int ID;
        public string Name;
        public int NeedJinBi;
        public int NeedZuanShi;
        public List<int> NeedGoodsIDs = new List<int>();
        public List<int> NeedGoodsCounts = new List<int>();
        public Dictionary<int, List<long>> PromoteJinBiRange = new Dictionary<int, List<long>>();
        public Dictionary<int, List<long>> PromoteZuanShiRange = new Dictionary<int, List<long>>();
        public Dictionary<int, int> PromotePropLimit = new Dictionary<int, int>();

        public Dictionary<int, int> PromoteRangeMin = new Dictionary<int, int>();
        public Dictionary<int, int> PromoteRangeMax = new Dictionary<int, int>();
    }

    public class RandomSet
    {
        public int[] ResultList { get; private set; }

        int RandomCount = 0;
        int AllCount = 0;

        public RandomSet(int count)
        {
            AllCount = count;
            ResultList = new int[count];
            for (int i = 0; i < count; i++)
            {
                ResultList[i] = i;
            }
        }

        public int RandomNext()
        {
            int rand = Global.GetRandomNumber(RandomCount, AllCount); //在剩下的角色里面随机抽取一个
            int t = ResultList[RandomCount];
            ResultList[RandomCount] = ResultList[rand];
            ResultList[rand] = t;

            return t;
        }
    }

    /// <summary>
    /// 静态装备洗练管理类
    /// </summary>
    public static class WashPropsManager
    {
        /// <summary>
        /// 操作码
        /// </summary>
        public static class WashOperations
        {
            public const int WashProps = 0; //洗炼属性
            public const int WashPropsActive = -1; //洗炼激活
            public const int WashPropsQuantity = -2; //洗炼数值
            public const int WashPropsCommit = -3; //洗炼提交
            public const int WashPropsCancle = -4; //洗炼取消
            public const int WashPropsQuery = -5; //尚未提交的洗炼结果查询
        }

        #region 配置表数据

        /// <summary>
        /// 洗炼传承成功率
        /// </summary>
        private static int[] XiLianChuanChengGoodsRates = new int[5];

        /// <summary>
        /// 洗炼传承消耗金币
        /// </summary>
        private static int[] XiLianChuanChengXiaoHaoJinBi = new int[16];

        /// <summary>
        /// 洗炼传承消耗钻石
        /// </summary>
        private static int[] XiLianChuanChengXiaoHaoZhuanShi = new int[16];

        private static List<int> PropsIds = new List<int>();
        private static Dictionary<int, XiLianType> XiLianTypeDict = new Dictionary<int, XiLianType>();
        private static Dictionary<int, XiLianShuXing> XiLianShuXingDict = new Dictionary<int, XiLianShuXing>();

        public static bool initialize()
        {
            InitConfig();
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPS, 5, WashPropsCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPS));
            TCPCmdDispatcher.getInstance().registerProcessor((int)TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPSINHERIT, 4, WashPropsCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPSINHERIT));
            return true;
        }
        public static bool startup()
        {
            return true;
        }
        public static bool showdown()
        {
            return true;
        }
        public static bool destroy()
        {
            return true;
        }

        public static void InitConfig()
        {
            XiLianTypeDict.Clear();
            XiLianShuXingDict.Clear();
            PropsIds.Clear();
            PropsIds.Add((int)ExtPropIndexes.MaxLifeV);
            PropsIds.Add((int)ExtPropIndexes.AddAttackInjure);
            PropsIds.Add((int)ExtPropIndexes.DecreaseInjureValue);
            PropsIds.Add((int)ExtPropIndexes.AddAttack);
            PropsIds.Add((int)ExtPropIndexes.AddDefense);
            PropsIds.Add((int)ExtPropIndexes.HitV);
            PropsIds.Add((int)ExtPropIndexes.Dodge);
            PropsIds.Add((int)ExtPropIndexes.LifeSteal);

            XElement xml = null;
            string fileName = "Config/XiLianType.xml";
            try
            {
                string fullPathFileName = fullPathFileName = Global.GameResPath("Config/XiLianType.xml"); //Global.IsolateResPath(fileName);

                xml = XElement.Load(fullPathFileName);
                if (null == xml)
                {
                    throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fullPathFileName));
                }

                IEnumerable<XElement> nodes = null;
                nodes = xml.Elements();
                foreach (var node in nodes)
                {
                    XiLianType xiLianType = new XiLianType();
                    xiLianType.ID = (int)Global.GetSafeAttributeLong(node, "ID");
                    xiLianType.Color = Global.GetSafeAttributeStr(node, "Color");
                    xiLianType.ShuXingNum = (int)Global.GetSafeAttributeLong(node, "ShuXingNum");
                    xiLianType.Text = Global.GetSafeAttributeStr(node, "Text");
                    xiLianType.FirstShuXing = 0;// Global.GetSafeAttributeDouble(node, "FirstShuXing");
                    xiLianType.ShuXingLimitMultiplying = Global.GetSafeAttributeDouble(node, "Multiplying");
                    XiLianTypeDict.Add(xiLianType.ID, xiLianType);
                }
            }
            catch (Exception)
            {
                throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。", fileName));
            }

            fileName = "Config/XiLianShuXing.xml";
            try
            {
                string fullPathFileName = fullPathFileName = Global.GameResPath(fileName); //Global.IsolateResPath(fileName);

                xml = XElement.Load(fullPathFileName);
                if (null == xml)
                {
                    throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fullPathFileName));
                }

                IEnumerable<XElement> nodes = null;
                nodes = xml.Elements();

                foreach (var node in nodes)
                {
                    ParseXiLianShuXing(node);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
            }

            try
            {
                XiLianChuanChengGoodsRates = GameManager.systemParamsList.GetParamValueIntArrayByName("XiLianChuanChengGoodsRates");
                XiLianChuanChengXiaoHaoJinBi = GameManager.systemParamsList.GetParamValueIntArrayByName("XiLianChuanChengXiaoHaoJinBi");
                XiLianChuanChengXiaoHaoZhuanShi = GameManager.systemParamsList.GetParamValueIntArrayByName("XiLianChuanChengXiaoHaoZhuanShi");
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
            }
        }

        public static void ParseXiLianShuXing(XElement node)
        {
            XiLianShuXing xiLianShuXing = new XiLianShuXing();
            xiLianShuXing.ID = (int)Global.GetSafeAttributeLong(node, "ID");
            xiLianShuXing.Name = Global.GetSafeAttributeStr(node, "Name");
            xiLianShuXing.NeedJinBi = (int)Global.GetSafeAttributeLong(node, "NeedJinBi");
            xiLianShuXing.NeedZuanShi = (int)Global.GetSafeAttributeLong(node, "NeedZuanShi");
            long[] args = Global.GetSafeAttributeLongArray(node, "NeedGoods", 2);
            if (null != args)
            {
                xiLianShuXing.NeedGoodsIDs.Add((int)args[0]);
                xiLianShuXing.NeedGoodsCounts.Add((int)args[1]);
            }

            foreach (var propsID in PropsIds)
            {
                string attributeName;
                ExtPropIndexes propIndex = (ExtPropIndexes)propsID;
                attributeName = string.Format("JinBi{0}", propIndex.ToString());
                args = Global.GetSafeAttributeLongArray(node, attributeName);
                if (null != args && args.Length > 0)
                {
                    xiLianShuXing.PromoteJinBiRange.Add(propsID, new List<long>(args));
                }

                attributeName = string.Format("ZuanShi{0}", propIndex.ToString());
                args = Global.GetSafeAttributeLongArray(node, attributeName);
                if (null != args && args.Length > 0)
                {
                    xiLianShuXing.PromoteZuanShiRange.Add(propsID, new List<long>(args));
                }

                xiLianShuXing.PromotePropLimit.Add((int)propIndex, (int)Global.GetSafeAttributeLong(node, propIndex.ToString()));
            }

            XiLianShuXingDict.Add(xiLianShuXing.ID, xiLianShuXing);
        }

        /// <summary>
        /// 洗炼操作
        /// 返回值列表: (操作索引:错误码:DBID:绑定状态:属性列表键值对...)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="dbid"></param>
        /// <param name="washIndex"></param>
        /// <param name="useBinding"></param>
        /// <returns></returns>
        public static bool WashProps(GameClient client, int dbid, int washIndex, bool useBinding, int moneyType)
        {
            int nID = (int)TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPS;
            List<int> result = new List<int>();

            result.Add(washIndex);
            result.Add(StdErrorCode.Error_Success);
            result.Add(dbid);
            result.Add(0);

            if (washIndex > WashOperations.WashPropsQuantity || washIndex < WashOperations.WashPropsQuery)
            {
                result[1] = (StdErrorCode.Error_Invalid_Operation);
                client.sendCmd(nID, result);
                return true;
            }
            if (moneyType < 0 || moneyType > 1)
            {
                result[1] = (StdErrorCode.Error_MoneyType_Not_Select);
                client.sendCmd(nID, result);
                return true;
            }

            //查找物品
            GoodsData goodsData = Global.GetGoodsByDbID(client, dbid);
            if (null == goodsData)
            {
                result[1] = (StdErrorCode.Error_Invalid_DBID);
                client.sendCmd(nID, result);
                return true;
            }

            //if (goodsData.Using > 0)
            //{
            //    result.Add(StdErrorCode.Error_Goods_Is_Using);
            //    client.sendCmd(nID, result);
            //    return true;
            //}

            SystemXmlItem xml;
            if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out xml))
            {
                //错误的参数
                result[1] = (StdErrorCode.Error_Config_Fault);
                client.sendCmd(nID, result);
                return true;
            }

            int id = xml.GetIntValue("XiLian");
            XiLianShuXing xiLianShuXing;
            if (!XiLianShuXingDict.TryGetValue(id, out xiLianShuXing))
            {
                //错误的参数
                result[1] = (StdErrorCode.Error_Config_Fault);
                client.sendCmd(nID, result);
                return true;
            }

            if (washIndex == WashOperations.WashPropsQuantity || washIndex == WashOperations.WashPropsActive)
            {
                if (moneyType == 0)
                {
                    if (client.ClientData.Money1 + client.ClientData.YinLiang < xiLianShuXing.NeedJinBi)
                    {
                        result[1] = (StdErrorCode.Error_JinBi_Not_Enough);
                        client.sendCmd(nID, result);
                        return true;
                    }
                }
                else if (moneyType == 1)
                {
                    if (client.ClientData.UserMoney < xiLianShuXing.NeedZuanShi)
                    {
                        result[1] = (StdErrorCode.Error_ZuanShi_Not_Enough);
                        client.sendCmd(nID, result);
                        return true;
                    }
                }
            }

            // 根据客户端的请求分别处理洗炼数值和洗练属性两种操作
            if (washIndex == WashOperations.WashPropsActive)
            {
                //洗炼激活
                if (null != goodsData.WashProps && goodsData.WashProps.Count > 0)
                {
                    result[1] = (StdErrorCode.Error_Invalid_Operation);
                    client.sendCmd(nID, result);
                    return true;
                }

                int color = Global.GetEquipColor(goodsData);
                XiLianType xiLianType;
                if (color <= 0 || !XiLianTypeDict.TryGetValue(color, out xiLianType) || xiLianType.ShuXingNum <= 0)
                {
                    result[1] = (StdErrorCode.Error_Invalid_Operation);
                    client.sendCmd(nID, result);
                    return true;
                }

                UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs() { RoleID = client.ClientData.RoleID, DbID = dbid };
                updateGoodsArgs.WashProps = new List<int>();

                //扣除所需物品
                if (xiLianShuXing.NeedGoodsIDs[0] > 0 && xiLianShuXing.NeedGoodsCounts[0] > 0)
                {
                    bool bUsedBinding = useBinding;
                    bool bUsedTimeLimited = false;

                    //扣除物品
                    if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener,
                        Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, xiLianShuXing.NeedGoodsIDs[0], xiLianShuXing.NeedGoodsCounts[0], false, out bUsedBinding, out bUsedTimeLimited))
                    {
                        //索引值无效
                        result[1] = (StdErrorCode.Error_Goods_Not_Enough);
                        client.sendCmd(nID, result);
                        return true;
                    }
                    if (goodsData.Binding == 0 && bUsedBinding)
                    {
                        updateGoodsArgs.Binding = 1;
                    }
                }

                //随机激活属性
                for (int i = 0; i < xiLianType.ShuXingNum; i++ )
                {
                    int rand = Global.GetRandomNumber(0, PropsIds.Count);
                    int propID = PropsIds[rand];
                    int propLimit = xiLianShuXing.PromotePropLimit[propID];
                    int propValue = (int)Math.Ceiling(propLimit * xiLianType.FirstShuXing * xiLianType.ShuXingLimitMultiplying);
                    updateGoodsArgs.WashProps.Add(propID);
                    updateGoodsArgs.WashProps.Add(propValue);
                }

                //存盘并通知用户结果
                Global.UpdateGoodsProp(client, goodsData, updateGoodsArgs);
                //写入角色物品的得失行为日志(扩展)
                Global.ModRoleGoodsEvent(client, goodsData, 0, "装备洗炼激活");

                result[3] = (goodsData.Binding > 0 | updateGoodsArgs.Binding > 0) ? 1 : 0;
                result.AddRange(goodsData.WashProps);
                client.sendCmd(nID, result);
                return true;
            }
            else if (washIndex == WashOperations.WashPropsQuantity)
            {
                UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs() { RoleID = client.ClientData.RoleID, DbID = dbid };

                //扣除所需物品
                if (xiLianShuXing.NeedGoodsIDs[0] > 0 && xiLianShuXing.NeedGoodsCounts[0] > 0)
                {
                    bool bUsedBinding = useBinding;
                    bool bUsedTimeLimited = false;

                    //扣除物品
                    bool useGoodSuccess = false;
                    if (bUsedBinding)
                    {
                        useGoodSuccess = GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 
                            client, xiLianShuXing.NeedGoodsIDs[0], xiLianShuXing.NeedGoodsCounts[0], false, out bUsedBinding, out bUsedTimeLimited);
                        if (!useGoodSuccess)
                        {
                            useGoodSuccess = GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 
                                client, xiLianShuXing.NeedGoodsIDs[0], xiLianShuXing.NeedGoodsCounts[0], false, out bUsedBinding, out bUsedTimeLimited);
                        }
                    }
                    else
                    {
                        useGoodSuccess = GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 
                            client, xiLianShuXing.NeedGoodsIDs[0], xiLianShuXing.NeedGoodsCounts[0], false, out bUsedBinding, out bUsedTimeLimited);
                    }
                    if (!useGoodSuccess)
                    {
                        //索引值无效
                        result[1] = (StdErrorCode.Error_Goods_Not_Enough);
                        client.sendCmd(nID, result);
                        return true;
                    }
                    if (goodsData.Binding == 0 && bUsedBinding)
                    {
                        updateGoodsArgs.Binding = 1;
                    }
                }

                if (moneyType == 0)
                {
                    Global.SubBindTongQianAndTongQian(client, xiLianShuXing.NeedJinBi, "洗炼");
                }
                else if (moneyType == 1)
                {
                    GameManager.ClientMgr.SubUserMoney(client, xiLianShuXing.NeedZuanShi, "洗炼");
                }

                //从装备颜色获取洗炼类型配置,获取洗炼条目最大值
                int color = Global.GetEquipColor(goodsData);
                XiLianType xiLianType;
                if (color <= 0 || !XiLianTypeDict.TryGetValue(color, out xiLianType) || xiLianType.ShuXingNum <= 0)
                {
                    result[1] = (StdErrorCode.Error_Invalid_Operation);
                    client.sendCmd(nID, result);
                    return true;
                }

                //如果没有洗练属性,先生成一份
                if (null == goodsData.WashProps || goodsData.WashProps.Count == 0)
                {
                    List<int> washProps = new List<int>(xiLianType.ShuXingNum * 2);
                    int maxCount = xiLianType.ShuXingNum;
                    foreach (var kv in xiLianShuXing.PromotePropLimit)
                    {
                        if (kv.Value > 0)
                        {
                            washProps.Add(kv.Key);
                            washProps.Add(0);
                            if (--maxCount <= 0)
                            {
                                break;
                            }
                        }
                    }

                    ////索引值无效
                    //result[1] = (StdErrorCode.Error_Invalid_Operation);
                    //client.sendCmd(nID, result);
                    //return true;
                    updateGoodsArgs.WashProps = washProps;
                }
                else
                {
                    updateGoodsArgs.WashProps = new List<int>(goodsData.WashProps);
                }

                //洗炼数值
                for (int i = 0; i < updateGoodsArgs.WashProps.Count; i += 2 )
                {
                    int propID = updateGoodsArgs.WashProps[i];
                    if (!xiLianShuXing.PromotePropLimit.ContainsKey(propID))
                    {
                        //错误的参数
                        result[1] = (StdErrorCode.Error_Config_Fault);
                        client.sendCmd(nID, result);
                        return true;
                    }
                    int propValue = updateGoodsArgs.WashProps[i + 1];
                    int propLimit = (int)(xiLianShuXing.PromotePropLimit[propID] * xiLianType.ShuXingLimitMultiplying);
                    if (moneyType == UseMoneyTypes.JinBiOrBindJinBi)
                    {
                        int nRandNum = Global.GetRandomNumber(0, xiLianShuXing.PromoteJinBiRange[propID].Count);
                        propValue += (int)xiLianShuXing.PromoteJinBiRange[propID][nRandNum];
                    }
                    else if (moneyType == UseMoneyTypes.ZuanShi)
                    {
                        int nRandNum = Global.GetRandomNumber(0, xiLianShuXing.PromoteZuanShiRange[propID].Count);
                        propValue += (int)xiLianShuXing.PromoteZuanShiRange[propID][nRandNum];
                    }

                    propValue = Global.Clamp(propValue, 0, propLimit);
                    updateGoodsArgs.WashProps[i + 1] = propValue;
                }

                client.ClientData.TempWashPropsDict[updateGoodsArgs.DbID] = updateGoodsArgs;
                client.ClientData.TempWashPropOperationIndex = washIndex;

                result[3] = (goodsData.Binding > 0 | updateGoodsArgs.Binding > 0) ? 1 : 0;
                result.AddRange(updateGoodsArgs.WashProps);
                client.sendCmd(nID, result);
                return true;
            }
            else if (washIndex == WashOperations.WashPropsCommit)
            {
                //提交洗练结果
                UpdateGoodsArgs tempWashProps;
                if (!client.ClientData.TempWashPropsDict.TryGetValue(goodsData.Id, out tempWashProps))
                {
                    //索引值无效
                    result[1] = (StdErrorCode.Error_Invalid_Index);
                    client.sendCmd(nID, result);
                    return true;
                }

                Global.UpdateGoodsProp(client, goodsData, tempWashProps);
                //写入角色物品的得失行为日志(扩展)
                Global.ModRoleGoodsEvent(client, goodsData, 0, "装备洗炼");
                client.ClientData.TempWashPropsDict[goodsData.Id] = null;
                result[3] = (goodsData.Binding > 0) ? 1 : 0;
                result.AddRange(goodsData.WashProps);
                client.sendCmd(nID, result);
                return true;
            }
            else if (washIndex == WashOperations.WashPropsCancle)
            {
                //取消本次洗炼,与提交(替换)相对
                if (client.ClientData.TempWashPropsDict.ContainsKey(dbid))
                {
                    client.ClientData.TempWashPropsDict[dbid] = null;
                }
                client.sendCmd(nID, result);
                return true;
            }
            else if (washIndex == WashOperations.WashPropsQuery)
            {
                //查询未提交的洗炼结果
                UpdateGoodsArgs tempWashProps;
                if (!client.ClientData.TempWashPropsDict.TryGetValue(goodsData.Id, out tempWashProps))
                {
                    result[1] = StdErrorCode.Error_Data_Overdue;
                    client.sendCmd(nID, result);
                    return true;
                }
                result[0] = client.ClientData.TempWashPropOperationIndex;
                result[2] = tempWashProps.DbID;
                result[3] = tempWashProps.Binding;
                result.AddRange(tempWashProps.WashProps);
                client.sendCmd(nID, result);
                return true;
            }
            else if(washIndex >= WashOperations.WashProps)
            {
                //洗练属性
                if (washIndex < 0 || null == goodsData.WashProps || goodsData.WashProps.Count / 2 <= washIndex)
                {
                    //索引值无效
                    result[1] = (StdErrorCode.Error_Invalid_Index);
                    client.sendCmd(nID, result);
                    return true;
                }

                int color = Global.GetEquipColor(goodsData);
                XiLianType xiLianType;
                if (color <= 0 || !XiLianTypeDict.TryGetValue(color, out xiLianType) || xiLianType.ShuXingNum <= washIndex)
                {
                    result[1] = (StdErrorCode.Error_Invalid_Operation);
                    client.sendCmd(nID, result);
                    return true;
                }

                UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs() { RoleID = client.ClientData.RoleID, DbID = dbid };
                updateGoodsArgs.WashProps = new List<int>(goodsData.WashProps);

                //扣除所需物品
                if (xiLianShuXing.NeedGoodsIDs[0] > 0 && xiLianShuXing.NeedGoodsCounts[0] > 0)
                {
                    bool bUsedBinding = useBinding;
                    bool bUsedTimeLimited = false;

                    //扣除物品
                    if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener,
                        Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, xiLianShuXing.NeedGoodsIDs[0], xiLianShuXing.NeedGoodsCounts[0], false, out bUsedBinding, out bUsedTimeLimited))
                    {
                        //索引值无效
                        result[1] = StdErrorCode.Error_Goods_Not_Enough;
                        client.sendCmd(nID, result);
                        return true;
                    }
                    if (goodsData.Binding == 0 && bUsedBinding)
                    {
                        updateGoodsArgs.Binding = 1;
                    }
                }

                int rand = Global.GetRandomNumber(0, PropsIds.Count);
                int propID = PropsIds[rand];
                int propLimit = xiLianShuXing.PromotePropLimit[propID];
                int propValue = (int)Math.Ceiling(propLimit * xiLianType.FirstShuXing * xiLianType.ShuXingLimitMultiplying);

                updateGoodsArgs.WashProps[washIndex * 2] = propID;
                updateGoodsArgs.WashProps[washIndex * 2 + 1] = propValue;

                client.ClientData.TempWashPropsDict[updateGoodsArgs.DbID] = updateGoodsArgs;
                client.ClientData.TempWashPropOperationIndex = washIndex;

                //通知结果
                result[3] = (goodsData.Binding > 0 | updateGoodsArgs.Binding > 0) ? 1 : 0;
                result.Add(propID);
                result.Add(propValue);
                client.sendCmd(nID, result);
                return true;
            }
            else 
            {
                //错误的参数
                result[1] = (StdErrorCode.Error_Invalid_Index);
                client.sendCmd(nID, result);
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="leftGoodsDbID">左边物品ID，提供传承属性的物品，传承提供者</param>
        /// <param name="rightGoodsDbID">右边物品ID，获得传承属性的物品，传承接受者</param>
        /// <param name="nSubMoneyType">消耗钱类型 -- 1银两 2元宝</param>
        /// <returns></returns>
        public static bool WashPropsInherit(GameClient client, int leftGoodsDbID, int rightGoodsDbID, int nSubMoneyType)
        {
            int roleID = client.ClientData.RoleID;
            int nID = (int)TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPSINHERIT;
            List<int> result = new List<int>();

            result.Add(StdErrorCode.Error_Success);
            result.Add(leftGoodsDbID);
            result.Add(rightGoodsDbID);
            result.Add(0);

            //从物品包中获取传承提供者装备
            GoodsData leftGoodsData = Global.GetGoodsByDbID(client, leftGoodsDbID);
            if (null == leftGoodsData) //没有找到物品
            {
                result[0] = StdErrorCode.Error_Invalid_DBID;
                client.sendCmd(nID, result);
                return true;
            }

            //从物品包中获取传承接受者装备
            GoodsData rightGoodsData = Global.GetGoodsByDbID(client, rightGoodsDbID);
            if (null == rightGoodsData) //没有找到物品
            {
                result[0] = StdErrorCode.Error_Invalid_DBID;
                client.sendCmd(nID, result);
                return true;
            }

            SystemXmlItem xml;
            if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(rightGoodsData.GoodsID, out xml))
            {
                //错误的参数
                result.Add(StdErrorCode.Error_Config_Fault);
                client.sendCmd(nID, result);
                return true;
            }

            int id = xml.GetIntValue("XiLian");
            XiLianShuXing xiLianShuXing;
            if (!XiLianShuXingDict.TryGetValue(id, out xiLianShuXing))
            {
                //配置错误
                result.Add(StdErrorCode.Error_Config_Fault);
                client.sendCmd(nID, result);
                return true;
            }

            /* 规则：
             1.剥离追加的装备颜色不能低于继承追加的装备颜色
             2.剥离追加的装备追加级别不能低于继承追加的装备追加级别
                 
            */

            int nLeftColor = Global.GetEquipColor(leftGoodsData);
            int nRigthColor = Global.GetEquipColor(rightGoodsData);
            
            if (nLeftColor < 2 || nRigthColor < 2 || null == leftGoodsData.WashProps/* || null == rightGoodsData.WashProps*/)
            {
                result[0] = StdErrorCode.Error_Operation_Denied;
                client.sendCmd(nID, result);
                return true;
            }

            XiLianType xiLianType = null;
            if (!XiLianTypeDict.TryGetValue(nRigthColor, out xiLianType))
            {
                //配置错误
                result.Add(StdErrorCode.Error_Config_Fault);
                client.sendCmd(nID, result);
                return true;
            }

            int OccupationLeft = Global.GetGoodsToOccupation(leftGoodsData.GoodsID);
            int OccupationRight = Global.GetGoodsToOccupation(rightGoodsData.GoodsID);

            // 装备职业
            if (OccupationLeft != OccupationRight)
            {
                result[0] = StdErrorCode.Error_Operation_Denied;
                client.sendCmd(nID, result);
                return true;
            }

            int categoryLeft = Global.GetGoodsCatetoriy(leftGoodsData.GoodsID);
            int categoryRight = Global.GetGoodsCatetoriy(rightGoodsData.GoodsID);

            if (categoryLeft >= 0 && categoryLeft <= 6 && categoryLeft == categoryRight)
            {
                //装备类型相同
            }
            else if (categoryLeft == 10 && categoryLeft == categoryRight )
            {
                //装备类型相同
            }
            else if (categoryLeft >= (int)ItemCategories.WuQi_Jian && categoryLeft <= (int)ItemCategories.WuQi_NuJianTong &&
                        categoryRight >= (int)ItemCategories.WuQi_Jian && categoryRight <= (int)ItemCategories.WuQi_NuJianTong)
            {
                //11到21都算相同(武器类)
            }
            else
            {
                result[0] = StdErrorCode.Error_Type_Not_Match;
                client.sendCmd(nID, result);
                return true;
            }

            //如果物品不在背包中，拒绝操作
            if (leftGoodsData.Site != 0 || rightGoodsData.Site != 0)
            {
                result[0] = StdErrorCode.Error_Goods_Not_Find;
                client.sendCmd(nID, result);
                return true;
            }

            // 检测 银两或元宝
            if (nSubMoneyType < 1 || nSubMoneyType > 2)
            {
                result[0] = StdErrorCode.Error_MoneyType_Not_Select;
                client.sendCmd(nID, result);
                return true;
            }

            if (nSubMoneyType == 1)
            {
                if (XiLianChuanChengXiaoHaoJinBi[0] > 0 && !Global.SubBindTongQianAndTongQian(client, XiLianChuanChengXiaoHaoJinBi[0], "洗练属性传承"))
                {
                    result[0] = StdErrorCode.Error_JinBi_Not_Enough;
                    client.sendCmd(nID, result);
                    return true;
                }
            }
            else if (nSubMoneyType == 2)
            {
                if (XiLianChuanChengXiaoHaoZhuanShi[0] > 0 && !GameManager.ClientMgr.SubUserMoney(client, XiLianChuanChengXiaoHaoZhuanShi[0], "洗练属性传承"))
                {
                    result[0] = StdErrorCode.Error_ZuanShi_Not_Enough;
                    client.sendCmd(nID, result);
                    return true;
                }
            }

            //判断是否有需要传承的属性，没有就不传承了 当前只传承 强化
            int nBinding = 0;
            if (rightGoodsData.Binding == 1 || leftGoodsData.Binding == 1)
            {
                nBinding = 1;
            }

            int rnd = Global.GetRandomNumber(0, 101);
            if (null != XiLianChuanChengGoodsRates && rnd > XiLianChuanChengGoodsRates[nLeftColor])
            {
                result[0] = StdErrorCode.Error_Operation_Faild;
                client.sendCmd(nID, result);
                return true;
            }

            UpdateGoodsArgs argsLeft = new UpdateGoodsArgs() { RoleID = roleID, DbID = leftGoodsDbID};
            argsLeft.WashProps = null;
            UpdateGoodsArgs argsRight = new UpdateGoodsArgs() { RoleID = roleID, DbID = rightGoodsDbID };
            if (null == rightGoodsData.WashProps)
            {
                argsRight.WashProps = new List<int>(leftGoodsData.WashProps.GetRange(0, xiLianType.ShuXingNum * 2));
                for (int i = 0; i < xiLianType.ShuXingNum; i++)
                {
                    if (i * 2 + 1 >= leftGoodsData.WashProps.Count || i * 2 + 1 >= xiLianType.ShuXingNum * 2)
                    {
                        break;
                    }
                    int propID = leftGoodsData.WashProps[i * 2];
                    int propLimit = 0;
                    if (xiLianShuXing.PromotePropLimit.TryGetValue(propID, out propLimit))
                    {
                        argsRight.WashProps[i * 2] = propID;
                        argsRight.WashProps[i * 2 + 1] = (int)Math.Round(Global.Clamp(leftGoodsData.WashProps[i * 2 + 1], 0, propLimit * xiLianType.ShuXingLimitMultiplying));
                    }
                }
            }
            else
            {
                argsRight.WashProps = new List<int>(rightGoodsData.WashProps);
                for (int i = 0; i < xiLianType.ShuXingNum; i++ )
                {
                    if (i * 2 + 1 >= leftGoodsData.WashProps.Count || i * 2 + 1 >= rightGoodsData.WashProps.Count)
                    {
                        break;
                    }
                    int propID = leftGoodsData.WashProps[i * 2];
                    int propLimit = 0;
                    if (xiLianShuXing.PromotePropLimit.TryGetValue(propID, out propLimit))
                    {
                        argsRight.WashProps[i * 2] = propID;
                        argsRight.WashProps[i * 2 + 1] = (int)Math.Round(Global.Clamp(leftGoodsData.WashProps[i * 2 + 1], 0, propLimit * xiLianType.ShuXingLimitMultiplying));
                    }
                }
            }

            argsRight.Binding = nBinding;
            if (Global.UpdateGoodsProp(client, leftGoodsData, argsLeft) < 0)
            {
                result[0] = StdErrorCode.Error_DB_Faild;
                client.sendCmd(nID, result);
                return true;
            }
            if (Global.UpdateGoodsProp(client, rightGoodsData, argsRight) < 0)
            {
                result[0] = StdErrorCode.Error_DB_Faild;
                client.sendCmd(nID, result);
                return true;
            }

            //写入角色物品的得失行为日志(扩展)
            Global.ModRoleGoodsEvent(client, leftGoodsData, 0, "装备洗炼传承_提供方");
            Global.ModRoleGoodsEvent(client, rightGoodsData, 0, "装备洗炼传承_接受方");

            //Global.BroadcastAppendChuanChengOk(client, leftGoodsData, rightGoodsData);

            //如果有物品是穿戴的，更新角色属性
            if (leftGoodsData.Using > 0 || rightGoodsData.Using > 0)
            {
                Global.RefreshEquipPropAndNotify(client);
            }

            // 更新成就
            //ChengJiuManager.OnFirstJiCheng(client);

            result[3] = nBinding;
            result.AddRange(rightGoodsData.WashProps);
            client.sendCmd(nID, result);
            return true;
        }

        #endregion 翅膀管理
    }
}
