using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Tools
{
    public class StringUtil
    {
        #region 模拟AS3函数

        //模拟AS3中的StringUtil.substitute 函数
        public static string substitute(string format, params object[] args)
        {
            string ret = "";
            try
            {
                ret = string.Format(format, args);
            }
            catch (Exception)
            {
                ret = format;
            }

            return ret;
        }

        ///忽略大小写比较两个字符串是否相等
        public static Boolean IsEqualIgnoreCase(String a, String b)
		{
            return a.ToLower() == b.ToLower();
		}
        #endregion 模拟AS3函数
    }
}
