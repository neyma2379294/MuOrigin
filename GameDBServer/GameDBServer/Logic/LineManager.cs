using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 线路管理
    /// </summary>
    public class LineManager
    {
        #region 线路索引字典

        /// <summary>
        /// 线路索引字典
        /// </summary>
        private static Dictionary<int, LineItem> _LinesDict = new Dictionary<int, LineItem>();

        /// <summary>
        /// 线路索引列表
        /// </summary>
        private static List<LineItem> _LinesList = new List<LineItem>();

        #endregion 线路索引字典

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="xml"></param>
        public static void LoadConfig(XElement xml)
        {
            IEnumerable<XElement> mapItems = xml.Element("GameServer").Elements();
            foreach (var mapItem in mapItems)
            {
                LineItem lineItem = new LineItem()
                {
                    LineID = (int)Global.GetSafeAttributeLong(mapItem, "ID"),
                    GameServerIP = Global.GetSafeAttributeStr(mapItem, "ip"),
                    GameServerPort = (int)Global.GetSafeAttributeLong(mapItem, "port"),
                    OnlineCount = 0,
                };

                _LinesDict[lineItem.LineID] = lineItem;
                _LinesList.Add(lineItem);
            }
        }

        /// <summary>
        /// 更新某个线路的心跳
        /// </summary>
        /// <param name="lineID"></param>
        public static void UpdateLineHeart(int lineID, int onlineNum, String strMapOnlineNum = "")
        {
            lock (_LinesDict)
            {
                LineItem lineItem = null;
                if (_LinesDict.TryGetValue(lineID, out lineItem))
                {
                    lineItem.OnlineCount = onlineNum;
                    lineItem.OnlineTicks = DateTime.Now.Ticks / 10000;
                    lineItem.MapOnlineNum = strMapOnlineNum;
                }
            }
        }

        /// <summary>
        /// 获取线路的心跳状态
        /// </summary>
        /// <param name="lineID"></param>
        /// <returns></returns>
        public static int GetLineHeartState(int lineID)
        {
            long ticks = DateTime.Now.Ticks / 10000;
            int state = 0;
            lock (_LinesDict)
            {
                LineItem lineItem = null;
                if (_LinesDict.TryGetValue(lineID, out lineItem))
                {
                    if (ticks - lineItem.OnlineTicks < (60 * 1000)) //服务器是否在线
                    {
                        state = 1;
                    }
                }
            }

            return state;
        }

        /// <summary>
        /// 获取线路列表
        /// </summary>
        /// <returns></returns>
        public static List<LineItem> GetLineItemList()
        {
            List<LineItem> lineItemList = new List<LineItem>();

            long ticks = DateTime.Now.Ticks / 10000;
            for (int i = 0; i < _LinesList.Count; i++)
            {
                if (ticks - _LinesList[i].OnlineTicks < (60 * 1000)) //服务器是否在线
                {
                    lineItemList.Add(_LinesList[i]);
                }
            }

            return lineItemList;
        }

        /// <summary>
        /// 获取所有线路总的在线人数
        /// </summary>
        /// <returns></returns>
        public static int GetTotalOnlineNum()
        {
            int totalNum = 0;
            long ticks = DateTime.Now.Ticks / 10000;
            for (int i = 0; i < _LinesList.Count; i++)
            {
                if (ticks - _LinesList[i].OnlineTicks < (60 * 1000)) //服务器是否在线
                {
                    totalNum += _LinesList[i].OnlineCount;
                }
            }

            return totalNum;
        }

        /// <summary>
        /// 获取地图在线信息
        /// </summary>
        /// <returns></returns>
        public static String GetMapOnlineNum()
        {
            String strMapOnlineInfo = "";
            long ticks = DateTime.Now.Ticks / 10000;
            for (int i = 0; i < _LinesList.Count; i++)
            {
                if (ticks - _LinesList[i].OnlineTicks < (60 * 1000)) //服务器是否在线
                {
                    return _LinesList[i].MapOnlineNum;
                }
            }

            return strMapOnlineInfo;
        }
    }
}
