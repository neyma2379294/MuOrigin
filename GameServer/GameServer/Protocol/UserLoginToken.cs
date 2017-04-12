using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;

namespace Server.Protocol
{
    /// <summary>
    /// 用户登陆口令
    /// </summary>
    class UserLoginToken
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID
        {
            get;
            set;
        }

        /// <summary>
        /// 随机的用户密码
        /// </summary>
        public int RandomPwd
        {
            get;
            set;
        }

        /// <summary>
        /// 获取加密并做了完整性保护的字节流
        /// </summary>
        /// <returns></returns>
        public byte[] GetEncryptBytes(string keySHA1, string keyData)
        {
            string userToken = string.Format("U:{0}:{1}:{2}:T", UserID, RandomPwd, DateTime.Now.Ticks);
            byte[] dataToken = new UTF8Encoding().GetBytes(userToken);
            byte[] macSHA1 = SHA1Helper.get_macsha1_bytes(dataToken, keySHA1);
            byte[] encryptToken = new byte[macSHA1.Length + dataToken.Length];
            DataHelper.CopyBytes(encryptToken, 0, macSHA1, 0, macSHA1.Length);
            DataHelper.CopyBytes(encryptToken, macSHA1.Length, dataToken, 0, dataToken.Length);

            //加密数据
            RC4Helper.RC4(encryptToken, keyData);
            return encryptToken;
        }

        /// <summary>
        /// 设置加密并做了完整性保护的字节流
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int SetEncryptBytes(byte[] buffer, string keySHA1, string keyData, long maxTicks)
        {
            //先解密数据
            RC4Helper.RC4(buffer, keyData);

            //拆分MACSHA1和DATATOKEN
            byte[] macSHA1 = new byte[20]; //固定的20个字节
            DataHelper.CopyBytes(macSHA1, 0, buffer, 0, macSHA1.Length);

            byte[] dataToken = new byte[buffer.Length - 20];
            DataHelper.CopyBytes(dataToken, 0, buffer, 20, dataToken.Length);

            //校验MACSHA1是否正确
            byte[] verifyMacSHA1 = SHA1Helper.get_macsha1_bytes(dataToken, keySHA1);
            if (!DataHelper.CompBytes(verifyMacSHA1, macSHA1))
            {
                return -1; //不想同，数据被篡改过了
            }

            //如果相同则解析用户ID和随机密码
            string strToken = new UTF8Encoding().GetString(dataToken);
            string[] parseTokens = strToken.Split(':');
            if (parseTokens.Length != 5) //个数不同，被篡改过了
            {
                return -2;
            }

            //是否是自己的协议
            if (parseTokens[0] != "U" || parseTokens[4] != "T")
            {
                return -3;
            }

            //时间戳
            long ticks = (long)Convert.ToUInt64(parseTokens[3]);
            if (DateTime.Now.Ticks - ticks >= maxTicks) //时间戳已经过期
            {
                return -4;
            }

            UserID = parseTokens[1];
            RandomPwd = (int)Convert.ToUInt32(parseTokens[2]);
            return 0;
        }

        /// <summary>
        /// 获取加密并做了完整性保护的字符串
        /// </summary>
        /// <returns></returns>
        public string GetEncryptString(string keySHA1, string keyData)
        {
            byte[] data = GetEncryptBytes(keySHA1, keyData);

            //用Base64编码将其转换为字符串
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// 设置加密并做了完整性保护的字节流
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int SetEncryptString(string s, string keySHA1, string keyData, long maxTicks)
        {
            byte[] buffer = null;
            try
            {
                buffer = Convert.FromBase64String(s);
            }
            catch (System.FormatException)
            {
                return -1000;
            }

            if (null == buffer)
            {
                return -1001;
            }

            return SetEncryptBytes(buffer, keySHA1, keyData, maxTicks);
        }
    }
}
