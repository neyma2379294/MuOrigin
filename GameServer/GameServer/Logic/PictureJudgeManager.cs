using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Data;
using Server.Tools;
using GameServer.Server;
using Server.Protocol;

namespace GameServer.Logic
{
    // 图鉴系统管理器 [5/3/2014 LiaoWei]
    public class PictureJudgeManager
    {
        /// <summary>
        /// 图鉴信息用map存储，key是图鉴ID，Value是图鉴在DB中存储的字段的索引位
        /// </summary>
        private static Dictionary<int, int> m_PictureJudgeInfo = new Dictionary<int, int>();

        /// <summary>
        /// 初始化标志位索引
        /// </summary>
        public static void InitPictureJudgeFlagIndex()
        {
            m_PictureJudgeInfo.Clear();

            int index = 0;

            for (int n = PictureJudgeTypesID.YongZheDaLuPictureJudgeStart; n <= PictureJudgeTypesID.YongZheDaLuPictureJudgeEnd; n++)
            {
                m_PictureJudgeInfo.Add(n, index);
                index += 2;
            }

            for (int n = PictureJudgeTypesID.XianZongLinPictureJudgeStart; n <= PictureJudgeTypesID.XianZongLinPictureJudgeEnd; n++)
            {
                m_PictureJudgeInfo.Add(n, index);
                index += 2;
            }

            for (int n = PictureJudgeTypesID.BingFengGuPictureJudgeStart; n <= PictureJudgeTypesID.BingFengGuPictureJudgeEnd; n++)
            {
                m_PictureJudgeInfo.Add(n, index);
                index += 2;
            }

            for (int n = PictureJudgeTypesID.DiXiaChengPictureJudgeStart; n <= PictureJudgeTypesID.DiXiaChengPictureJudgeEnd; n++)
            {
                m_PictureJudgeInfo.Add(n, index);
                index += 2;
            }

            for (int n = PictureJudgeTypesID.ShiLuoZhiTaPictureJudgeStart; n <= PictureJudgeTypesID.ShiLuoZhiTaPictureJudgeEnd; n++)
            {
                m_PictureJudgeInfo.Add(n, index);
                index += 2;
            }

            for (int n = PictureJudgeTypesID.YaTeLanDiShiPictureJudgeStart; n <= PictureJudgeTypesID.YaTeLanDiShiPictureJudgeEnd; n++)
            {
                m_PictureJudgeInfo.Add(n, index);
                index += 2;
            }

            for (int n = PictureJudgeTypesID.SiWangShaMoPictureJudgeStart; n <= PictureJudgeTypesID.SiWangShaMoPictureJudgeEnd; n++)
            {
                m_PictureJudgeInfo.Add(n, index);
                index += 2;
            }

            for (int n = PictureJudgeTypesID.TianKongZhiChengPictureJudgeStart; n <= PictureJudgeTypesID.TianKongZhiChengPictureJudgeEnd; n++)
            {
                m_PictureJudgeInfo.Add(n, index);
                index += 2;
            }
        }

        /// <summary>
        /// 初始化玩家的图鉴属性
        /// </summary>
        public static void InitPlayerPictureJudgePorperty(GameClient client)
        {
            foreach (var Picturejudgeinfo in m_PictureJudgeInfo)
            {
                int nIndex = -1;
                nIndex = GetPictureJudgeFlagIndex(Picturejudgeinfo.Key);

                if (nIndex < 0)
                    continue;

                if (!PictureJudgeFlagIsTrue(client, Picturejudgeinfo.Key))
                    continue;

                PictureJudgeData tmpInfo = null;
                tmpInfo = Data.PicturejudgeData[Picturejudgeinfo.Key];

                if (tmpInfo == null)
                    continue;

                if (tmpInfo.PropertyID1 >= 0 && tmpInfo.AddPropertyMinValue1 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID1] += tmpInfo.AddPropertyMinValue1;
                }

                if (tmpInfo.PropertyID2 >= 0 && tmpInfo.AddPropertyMaxValue1 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID2] += tmpInfo.AddPropertyMaxValue1;
                }

                if (tmpInfo.PropertyID3 >= 0 && tmpInfo.AddPropertyMinValue2 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID3] += tmpInfo.AddPropertyMinValue2;
                }

                if (tmpInfo.PropertyID4 >= 0 && tmpInfo.AddPropertyMaxValue2 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID4] += tmpInfo.AddPropertyMaxValue2;
                }

                if (tmpInfo.PropertyID5 >= 0 && tmpInfo.AddPropertyMinValue3 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID5] += tmpInfo.AddPropertyMinValue3;
                }

                if (tmpInfo.PropertyID6 >= 0 && tmpInfo.AddPropertyMaxValue3 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID6] += tmpInfo.AddPropertyMaxValue3;
                }

                if (tmpInfo.PropertyID7 >= 0 && tmpInfo.AddPropertyMinValue4 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID7] += tmpInfo.AddPropertyMinValue4;
                }

                if (tmpInfo.PropertyID8 >= 0 && tmpInfo.AddPropertyMaxValue4 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID8] += tmpInfo.AddPropertyMaxValue4;
                }

                if (tmpInfo.PropertyID9 >= 0 && tmpInfo.AddPropertyMinValue5 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID9] += tmpInfo.AddPropertyMinValue5;
                }

                if (tmpInfo.PropertyID10 >= 0 && tmpInfo.AddPropertyMinValue6 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID10] += tmpInfo.AddPropertyMinValue6;
                }

                if (tmpInfo.PropertyID11 >= 0 && tmpInfo.AddPropertyMinValue7 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID11] += tmpInfo.AddPropertyMinValue7;
                }

                int nCount = 0;
                if (!client.ClientData.PictureJudgeReferTypeInfo.TryGetValue(tmpInfo.PictureJudgeType, out nCount))
                {
                    ++nCount;
                    client.ClientData.PictureJudgeReferTypeInfo.Add(tmpInfo.PictureJudgeType, nCount);
                }
                else
                {
                    ++client.ClientData.PictureJudgeReferTypeInfo[tmpInfo.PictureJudgeType];
                }
            }

            ActivationPictureJudgeExtendPorp(client);

        }

        /// <summary>
        /// 激活图鉴
        /// </summary>
        public static int ActivationPictureJudge(GameClient client, int nPictureJudgeID)
        {
            if (nPictureJudgeID < 0)
                return -3;

            PictureJudgeData tmpInfo = null;
            tmpInfo = Data.PicturejudgeData[nPictureJudgeID];

            if (tmpInfo == null)
                return -4;

            int nGoods = tmpInfo.NeedGoodsID;
            int nNum = tmpInfo.NeedGoodsNum;

            GoodsData goods = null;
            goods = Global.GetGoodsByID(client, nGoods);

            if (goods == null || nNum > goods.GCount)
                return -5;
            
            bool usedBinding = false;
            bool usedTimeLimited = false;

            if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nGoods, nNum, false, out usedBinding, out usedTimeLimited))
                return -6;

            if (tmpInfo.PropertyID1 > 0 && tmpInfo.AddPropertyMinValue1 > 0)
            {
                client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID1] += tmpInfo.AddPropertyMinValue1;
            }

            if (tmpInfo.PropertyID2 > 0 && tmpInfo.AddPropertyMaxValue1 > 0)
            {
                client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID2] += tmpInfo.AddPropertyMaxValue1;
            }

            if (tmpInfo.PropertyID3 > 0 && tmpInfo.AddPropertyMinValue2 > 0)
            {
                client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID3] += tmpInfo.AddPropertyMinValue2;
            }

            if (tmpInfo.PropertyID4 > 0 && tmpInfo.AddPropertyMaxValue2 > 0)
            {
                client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID4] += tmpInfo.AddPropertyMaxValue2;
            }

            if (tmpInfo.PropertyID5 > 0 && tmpInfo.AddPropertyMinValue3 > 0)
            {
                client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID5] += tmpInfo.AddPropertyMinValue3;
            }

            if (tmpInfo.PropertyID6 > 0 && tmpInfo.AddPropertyMaxValue3 > 0)
            {
                client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID6] += tmpInfo.AddPropertyMaxValue3;
            }

            if (tmpInfo.PropertyID7 > 0 && tmpInfo.AddPropertyMinValue4 > 0)
            {
                client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID7] += tmpInfo.AddPropertyMinValue4;
            }

            if (tmpInfo.PropertyID8 > 0 && tmpInfo.AddPropertyMaxValue4 > 0)
            {
                client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID8] += tmpInfo.AddPropertyMaxValue4;
            }

            if (tmpInfo.PropertyID9 > 0 && tmpInfo.AddPropertyMinValue5 > 0)
            {
                client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID9] += tmpInfo.AddPropertyMinValue5;
            }

            if (tmpInfo.PropertyID10 > 0 && tmpInfo.AddPropertyMinValue6 > 0)
            {
                client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID10] += tmpInfo.AddPropertyMinValue6;
            }

            if (tmpInfo.PropertyID11 > 0 && tmpInfo.AddPropertyMinValue7 > 0)
            {
                client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID11] += tmpInfo.AddPropertyMinValue7;
            }

            // 更新标记
            PictureJudgeManager.UpdatePictureJudgeFlag(client, nPictureJudgeID);

            PictureJudgeManager.UpdatePictureJudgeFlag(client, nPictureJudgeID, true);

            GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);

            return 1;
        }

        /// <summary>
        /// 激活图鉴-第二个版本
        /// </summary>
        public static int ActivationPictureJudge2(GameClient client, int nPictureJudgeID)
        {
            if (nPictureJudgeID < 0)
                return -3;

            PictureJudgeData tmpInfo = null;

            if (!Data.PicturejudgeData.TryGetValue(nPictureJudgeID, out tmpInfo))
                return -10;
            else
            {
                if (tmpInfo == null)
                    return -4;
            }

            int nGoods = tmpInfo.NeedGoodsID;
            int nNum = tmpInfo.NeedGoodsNum;

            GoodsData goods = null;
            goods = Global.GetGoodsByID(client, nGoods);

            if (goods == null)
                return -5;

            bool bActivation = false;
            int nSub = 0;

            int nReferCount = 0;

            if (client.ClientData.PictureJudgeReferInfo == null)
            {
                client.ClientData.PictureJudgeReferInfo = new Dictionary<int, int>();
            }

            if (!client.ClientData.PictureJudgeReferInfo.TryGetValue(nPictureJudgeID, out nReferCount)) // 还没提交过图鉴信息
            {
                if (nNum > goods.GCount)
                    nSub = goods.GCount;
                else
                {
                    nSub = nNum;
                    bActivation = true;
                }   

                client.ClientData.PictureJudgeReferInfo.Add(nPictureJudgeID, nSub);
            }
            else
            {
                int ntotal = 0;
                ntotal = goods.GCount + nReferCount;

                if (nNum > ntotal)
                {
                    nSub = goods.GCount;
                    client.ClientData.PictureJudgeReferInfo[nPictureJudgeID] = ntotal;
                }
                else if (nNum == ntotal)
                {
                    nSub = goods.GCount;
                    bActivation = true;
                    client.ClientData.PictureJudgeReferInfo[nPictureJudgeID] = nNum;
                }
                else
                {
                    nSub = nNum - nReferCount;
                    bActivation = true;
                    client.ClientData.PictureJudgeReferInfo[nPictureJudgeID] = nNum;
                }
            }

            bool usedBinding = false;
            bool usedTimeLimited = false;

            if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nGoods, nSub, false, out usedBinding, out usedTimeLimited))
                return -6;

            // 通知DB   CMD_DB_REFERPICTUREJUDGE
            TCPOutPacket tcpOutPacket = null;
            string strDbCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, nPictureJudgeID, client.ClientData.PictureJudgeReferInfo[nPictureJudgeID]);

            TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer2(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 
                                                                                (int)TCPGameServerCmds.CMD_DB_REFERPICTUREJUDGE, strDbCmd, out tcpOutPacket);

            if (TCPProcessCmdResults.RESULT_FAILED == dbRequestResult)
                return -7;
            Global.PushBackTcpOutPacket(tcpOutPacket);
            if (bActivation == true )
            {
                if (tmpInfo.PropertyID1 > 0 && tmpInfo.AddPropertyMinValue1 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID1] += tmpInfo.AddPropertyMinValue1;
                }

                if (tmpInfo.PropertyID2 > 0 && tmpInfo.AddPropertyMaxValue1 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID2] += tmpInfo.AddPropertyMaxValue1;
                }

                if (tmpInfo.PropertyID3 > 0 && tmpInfo.AddPropertyMinValue2 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID3] += tmpInfo.AddPropertyMinValue2;
                }

                if (tmpInfo.PropertyID4 > 0 && tmpInfo.AddPropertyMaxValue2 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID4] += tmpInfo.AddPropertyMaxValue2;
                }

                if (tmpInfo.PropertyID5 > 0 && tmpInfo.AddPropertyMinValue3 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID5] += tmpInfo.AddPropertyMinValue3;
                }

                if (tmpInfo.PropertyID6 > 0 && tmpInfo.AddPropertyMaxValue3 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID6] += tmpInfo.AddPropertyMaxValue3;
                }

                if (tmpInfo.PropertyID7 > 0 && tmpInfo.AddPropertyMinValue4 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID7] += tmpInfo.AddPropertyMinValue4;
                }

                if (tmpInfo.PropertyID8 > 0 && tmpInfo.AddPropertyMaxValue4 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID8] += tmpInfo.AddPropertyMaxValue4;
                }

                if (tmpInfo.PropertyID9 > 0 && tmpInfo.AddPropertyMinValue5 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID9] += tmpInfo.AddPropertyMinValue5;
                }

                if (tmpInfo.PropertyID10 > 0 && tmpInfo.AddPropertyMinValue6 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID10] += tmpInfo.AddPropertyMinValue6;
                }

                if (tmpInfo.PropertyID11 > 0 && tmpInfo.AddPropertyMinValue7 > 0)
                {
                    client.ClientData.PictureJudgeProp.SecondPropsValue[tmpInfo.PropertyID11] += tmpInfo.AddPropertyMinValue7;
                }

                // 更新标记
                PictureJudgeManager.UpdatePictureJudgeFlag(client, nPictureJudgeID);

                PictureJudgeManager.UpdatePictureJudgeFlag(client, nPictureJudgeID, true);

                GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);

                int nCount = 0;
                if (!client.ClientData.PictureJudgeReferTypeInfo.TryGetValue(tmpInfo.PictureJudgeType, out nCount))
                {
                    ++nCount;
                    client.ClientData.PictureJudgeReferTypeInfo.Add(tmpInfo.PictureJudgeType, nCount);
                }
                else
                {
                    ++client.ClientData.PictureJudgeReferTypeInfo[tmpInfo.PictureJudgeType];
                }

                ActivationPictureJudgeExtendPorp(client);
            }

            return client.ClientData.PictureJudgeReferInfo[nPictureJudgeID];
        }

        /// <summary>
        /// 激活图鉴附加属性
        /// </summary>
        public static void ActivationPictureJudgeExtendPorp(GameClient client)
        {
            if (client == null || client.ClientData.PictureJudgeReferTypeInfo.Count == 0)
                return;

            foreach (var PictureJudgeReferTypeInfo in client.ClientData.PictureJudgeReferTypeInfo)
            {
                PictureJudgeTypeData pjTpyeTmp = null;
                if (Data.PicturejudgeTypeData.TryGetValue(PictureJudgeReferTypeInfo.Key, out pjTpyeTmp) && pjTpyeTmp != null)
                {
                    if (PictureJudgeReferTypeInfo.Value >= pjTpyeTmp.TotalNum)
                    {
                        if (pjTpyeTmp.PropertyID1 > 0 && pjTpyeTmp.AddPropertyMinValue1 > 0)
                        {
                            client.ClientData.PictureJudgeProp.SecondPropsValue[pjTpyeTmp.PropertyID1] += pjTpyeTmp.AddPropertyMinValue1;
                        }

                        if (pjTpyeTmp.PropertyID2 > 0 && pjTpyeTmp.AddPropertyMaxValue1 > 0)
                        {
                            client.ClientData.PictureJudgeProp.SecondPropsValue[pjTpyeTmp.PropertyID2] += pjTpyeTmp.AddPropertyMaxValue1;
                        }

                        if (pjTpyeTmp.PropertyID3 > 0 && pjTpyeTmp.AddPropertyMinValue2 > 0)
                        {
                            client.ClientData.PictureJudgeProp.SecondPropsValue[pjTpyeTmp.PropertyID3] += pjTpyeTmp.AddPropertyMinValue2;
                        }

                        if (pjTpyeTmp.PropertyID4 > 0 && pjTpyeTmp.AddPropertyMaxValue2 > 0)
                        {
                            client.ClientData.PictureJudgeProp.SecondPropsValue[pjTpyeTmp.PropertyID4] += pjTpyeTmp.AddPropertyMaxValue2;
                        }

                        if (pjTpyeTmp.PropertyID5 > 0 && pjTpyeTmp.AddPropertyMinValue3 > 0)
                        {
                            client.ClientData.PictureJudgeProp.SecondPropsValue[pjTpyeTmp.PropertyID5] += pjTpyeTmp.AddPropertyMinValue3;
                        }

                        if (pjTpyeTmp.PropertyID6 > 0 && pjTpyeTmp.AddPropertyMaxValue3 > 0)
                        {
                            client.ClientData.PictureJudgeProp.SecondPropsValue[pjTpyeTmp.PropertyID6] += pjTpyeTmp.AddPropertyMaxValue3;
                        }

                        if (pjTpyeTmp.PropertyID7 > 0 && pjTpyeTmp.AddPropertyMinValue4 > 0)
                        {
                            client.ClientData.PictureJudgeProp.SecondPropsValue[pjTpyeTmp.PropertyID7] += pjTpyeTmp.AddPropertyMinValue4;
                        }

                        if (pjTpyeTmp.PropertyID8 > 0 && pjTpyeTmp.AddPropertyMaxValue4 > 0)
                        {
                            client.ClientData.PictureJudgeProp.SecondPropsValue[pjTpyeTmp.PropertyID8] += pjTpyeTmp.AddPropertyMaxValue4;
                        }

                        if (pjTpyeTmp.PropertyID9 > 0 && pjTpyeTmp.AddPropertyMinValue5 > 0)
                        {
                            client.ClientData.PictureJudgeProp.SecondPropsValue[pjTpyeTmp.PropertyID9] += pjTpyeTmp.AddPropertyMinValue5;
                        }

                        if (pjTpyeTmp.PropertyID10 > 0 && pjTpyeTmp.AddPropertyMinValue6 > 0)
                        {
                            client.ClientData.PictureJudgeProp.SecondPropsValue[pjTpyeTmp.PropertyID10] += pjTpyeTmp.AddPropertyMinValue6;
                        }

                        if (pjTpyeTmp.PropertyID11 > 0 && pjTpyeTmp.AddPropertyMinValue7 > 0)
                        {
                            client.ClientData.PictureJudgeProp.SecondPropsValue[pjTpyeTmp.PropertyID11] += pjTpyeTmp.AddPropertyMinValue7;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 通过图鉴索引位置返回图鉴ID
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected static ushort GetPictureJudgeIDByIndex(int index)
        {
            for (int n = 0; n < m_PictureJudgeInfo.Count; n++)
            {
                if (m_PictureJudgeInfo.ElementAt(n).Value == index)
                {
                    return (ushort)m_PictureJudgeInfo.ElementAt(n).Key;
                }
            }
            return 0;
        }

        /// <summary>
        /// 根据图鉴id返回图鉴索引
        /// </summary>
        /// <param name="chengJiuID"></param>
        /// <returns></returns>
        protected static int GetPictureJudgeFlagIndex(int PictureJudgeID)
        {
            int index = -1;

            if (m_PictureJudgeInfo.TryGetValue(PictureJudgeID, out index))
            {
                return index;
            }

            return -1;
        }

        /// <summary>
        /// 更新图鉴标记
        /// </summary>
        /// <returns></returns>
        public static bool UpdatePictureJudgeFlag(GameClient client, int nID, bool ActivationFlag = false)
        {
            int index = GetPictureJudgeFlagIndex(nID);
            if (index < 0)
                return false;

            if (ActivationFlag)
                index++;
            
            List<ulong> lsLong = Global.GetRoleParamsUlongListFromDB(client, RoleParamName.PictureJudgeFlags);

            //根据 index 定位到相应的整数
            int arrPosIndex = index / 64;

            //填充64位整数
            while (arrPosIndex > lsLong.Count - 1)
                lsLong.Add(0);

            //定位到整数内部的某个具体位置
            int subIndex = index % 64;

            ulong destLong = lsLong[arrPosIndex];

            ulong flag = ((ulong)(1)) << subIndex;

            //设置标志位 为 1
            lsLong[arrPosIndex] = destLong | flag;

            //存储到数据库
            Global.SaveRoleParamsUlongListToDB(client, lsLong, RoleParamName.PictureJudgeFlags, true);

            return true;
        }

        /// <summary>
        /// 判断图鉴ID对应标志位是否是true ，forAward=false 图鉴是否完成 和 forAward = true图鉴是否激活
        /// </summary>
        /// <param name="chengJiuHexString"></param>
        /// <param name="chengJiuID"></param>
        /// <returns></returns>
        public static Boolean PictureJudgeFlagIsTrue(GameClient client, int nID, Boolean forAward = false)
        {
            //完成标志索引
            int index = GetPictureJudgeFlagIndex(nID);
            if (index < 0)
                return false;

            if (forAward)
                index++;

            List<ulong> lsLong = Global.GetRoleParamsUlongListFromDB(client, RoleParamName.PictureJudgeFlags);

            if (lsLong.Count <= 0)
                return false;

            //根据 index 定位到相应的整数
            int arrPosIndex = index / 64;

            if (arrPosIndex >= lsLong.Count)
                return false;

            //定位到整数内部的某个具体位置
            int subIndex = index % 64;

            UInt64 destLong = lsLong[arrPosIndex];

            //这个flag值比较特殊，这样写意味着 在 8 字节的 64位中处于从最小值开始，根据subIndex增加而增加
            //从64位存储的角度看，设计到大端序列和小端序列，看起来在不同的机器样子不一样
            ulong flag = ((ulong)(1)) << subIndex;

            //进行标志位判断
            bool bResult = (destLong & flag) > 0;

            return bResult;
        }

        /// <summary>
        /// 返回图鉴信息无符号整数数组，用于传递给客户端，每个图鉴信息 14位的图鉴id， 一位完成标志，一位激活标准
        /// </summary>
        /// <param name="chengJiuString"></param>
        /// <returns></returns>
        public static List<ushort> GetPictureJudgeFlag(GameClient client)
        {
            List<ulong> lsLong = Global.GetRoleParamsUlongListFromDB(client, RoleParamName.PictureJudgeFlags);

            //索引位置
            int curIndex = 0;

            List<ushort> lsUshort = new List<ushort>();

            for (int n = 0; n < lsLong.Count; n++)
            {
                ulong uValue = lsLong[n];

                for (int subIndex = 0; subIndex < 64; subIndex += 2)
                {
                    //采用 11 移动
                    ulong flag = (ulong)(((ulong)0x3) << (subIndex));//完成与否 激活与否

                    //得到2bit标志位
                    ushort realFlag = (ushort)((uValue & flag) >> (subIndex));//提取到两个标志位表示的数 到最右边，得到一个数

                    ushort PictureJudgeID = GetPictureJudgeIDByIndex(curIndex);

                    //14bit 的 图鉴ID
                    ushort preFix = (ushort)(PictureJudgeID << 2);

                    //14bit图鉴ID + 2bit标志位
                    ushort PictureJudge = (ushort)(preFix | realFlag);

                    lsUshort.Add(PictureJudge);

                    curIndex += 2;//注 索引也是2递增的，因为标志位是两个一组，一个是否完成，一个是否领取
                }
            }

            return lsUshort;
        }
    }

}
