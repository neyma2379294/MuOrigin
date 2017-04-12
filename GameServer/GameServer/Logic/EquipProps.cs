using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;

namespace GameServer.Logic
{
    public class EquipPropItem
    {
        public EquipPropItem()
        {
            ResetProps();
        }

        /// <summary>
        /// 5个基础属性值
        /// </summary>
        private double[] _BaseProps = new double[5];

        /// <summary>
        /// 5个基础属性值
        /// </summary>
        public double[] BaseProps
        {
            get { return _BaseProps; }
        }

        /// <summary>
        /// 43个扩展属性值
        /// </summary>
        private double[] _ExtProps = new double[(int)ExtPropIndexes.Max];

        /// <summary>
        /// 43个基础属性值
        /// </summary>
        public double[] ExtProps
        {
            get { return _ExtProps; } 
        }

        /// <summary>
        /// 清空属性值
        /// </summary>
        public void ResetProps()
        {
            for (int i = 0; i < 5; i++)
            {
                _BaseProps[i] = 0;
            }

            for (int i = 0; i < (int)ExtPropIndexes.Max; i++)
            {
                _ExtProps[i] = 0;
            }
        }
    }

    /// <summary>
    /// 装备属性缓存
    /// </summary>
    public class EquipProps
    {
        /// <summary>
        /// 装备属性字典
        /// </summary>
        private Dictionary<int, EquipPropItem> _EquipPropsDict = new Dictionary<int, EquipPropItem>();

        /// <summary>
        /// 解析装备属性
        /// </summary>
        /// <param name="systemGoods"></param>
        /// <param name="equipPropItem"></param>
        private void ParseEquipProps(SystemXmlItem systemGoods, out EquipPropItem equipPropItem)
        {
            equipPropItem = null;
            string props = systemGoods.GetStringValue("EquipProps");
            string[] fields = props.Split(',');
            if (fields.Length != (int)ExtPropIndexes.Max_Configed) //属性个数不符合
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析物品属性失败: EquipID={0}", systemGoods.GetIntValue("ID")));
                return;
            }
            
            double[] arryDoubles = null;

            try
            {
                arryDoubles = Global.StringArray2DoubleArray(fields);
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("转换物品属性数组: EquipID={0}", systemGoods.GetIntValue("ID")));
                return;
            }

            equipPropItem = new EquipPropItem();
            //for (int i = 0; i < 5; i++)
            //{
            //    equipPropItem.BaseProps[i] = arryDoubles[i];
            //}

            for (int i = 0; i < (int)ExtPropIndexes.Max_Configed; i++)
            {
                equipPropItem.ExtProps[i] = arryDoubles[i];
            }
        }

        /// <summary>
        /// 通过物品ID获取属性
        /// </summary>
        /// <param name="equipID"></param>
        /// <returns></returns>
        public EquipPropItem FindEquipPropItem(int equipID)
        {
            EquipPropItem equipPropItem = null;
            lock (_EquipPropsDict)
            {
                if (_EquipPropsDict.TryGetValue(equipID, out equipPropItem))
                {
                    return equipPropItem;
                }
            }

            //先查找缓存
            SystemXmlItem systemGoods = null;
            if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(equipID, out systemGoods))
            {
                return null;
            }

            ParseEquipProps(systemGoods, out equipPropItem);
            if (null != equipPropItem)
            {
                lock (_EquipPropsDict)
                {
                    _EquipPropsDict[equipID] = equipPropItem;
                }
            }

            return equipPropItem;
        }
    }
}
