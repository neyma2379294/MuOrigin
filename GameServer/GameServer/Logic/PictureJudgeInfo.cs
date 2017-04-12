using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Server;
using System.Windows;

namespace GameServer.Logic
{
    // 图鉴信息类 [5/1/2014 LiaoWei]
    public class PictureJudgeInfo
    {
        public PictureJudgeInfo()
        {
            ResetPictureJudgeProps();
        }

        /// <summary>
        /// 一级属性数值
        /// </summary>
        private double[] m_FirstPropsValue = new double[(int)UnitPropIndexes.Max];

        /// <summary>
        /// 一级属性数值
        /// </summary>
        public double[] FirstPropsValue
        {
            get { return m_FirstPropsValue; }
        }

        /// <summary>
        /// 二级属性数值
        /// </summary>
        private double[] m_SecondPropsValue = new double[(int)ExtPropIndexes.Max];

        /// <summary>
        /// 二级属性数值
        /// </summary>
        public double[] SecondPropsValue
        {
            get { return m_SecondPropsValue; } 
        }

        /// <summary>
        /// 清空属性值
        /// </summary>
        public void ResetPictureJudgeProps()
        {
            for (int i = 0; i < (int)UnitPropIndexes.Max; i++)
            {
                m_FirstPropsValue[i] = 0;
            }

            for (int i = 0; i < (int)ExtPropIndexes.Max; i++)
            {
                m_SecondPropsValue[i] = 0;
            }
        }

    }
}
