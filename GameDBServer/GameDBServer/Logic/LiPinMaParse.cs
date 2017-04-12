using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;

namespace GameDBServer.Logic
{
    /// <summary>
    /// 礼品码生成和解析类
    /// </summary>
    public class LiPinMaParse
    {
        #region 唯一ID和名称

        /// <summary>
        /// 产生唯一的ID
        /// </summary>
        /// <returns></returns>
        private static string GenerateUniqueId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }

            return string.Format("{0:X2}", i - DateTime.Now.Ticks);
        }

        #endregion 唯一ID和名称

        /// <summary>
        /// 生成礼品码
        /// </summary>
        /// <param name="ptid"></param>
        /// <param name="ptrepeat"></param>
        /// <param name="zoneID"></param>
        /// <returns></returns>
        public static string GenerateLiPinMa(int ptid, int ptrepeat, int zoneID)
        {
            string randStr = GenerateUniqueId().Substring(0, 12);
            string lipinma_data = string.Format("NZ{0:000}{1:0}{2:000}{3}", ptid, ptrepeat, zoneID, randStr);
            byte[] bytesData = new UTF8Encoding().GetBytes(lipinma_data);

            CRC32 crc32 = new CRC32();
            crc32.update(bytesData);
            uint crc32Val = crc32.getValue() % 255;
            string str = string.Format("{0:X}", crc32Val);

            lipinma_data += str;
            return lipinma_data;
        }

        /// <summary>
        /// 解析礼品码
        /// </summary>
        /// <param name="lipinma"></param>
        /// <param name="ptid"></param>
        /// <param name="ptrepeat"></param>
        /// <param name="zoneID"></param>
        /// <returns></returns>
        public static bool ParseLiPinMa(string lipinma, out int ptid, out int ptrepeat, out int zoneID)
        {
            ptid = -1;
            ptrepeat = 0;
            zoneID = 0;

            if (lipinma.Length < 22 || lipinma.Length > 23)
            {
                return false;
            }

            lipinma = lipinma.ToUpper();
            if ("NZ" != lipinma.Substring(0, 2))
            {
                return false;
            }

            string crc32Str = lipinma.Substring(21, Math.Min(2, lipinma.Length - 21));
            int crc32Val = Convert.ToInt32(crc32Str, 16);

            byte[] bytesData = new UTF8Encoding().GetBytes(lipinma.Substring(0, 21));

            CRC32 crc32 = new CRC32();
            crc32.update(bytesData);
            uint check_crc32Val = crc32.getValue() % 255;
            if (crc32Val != (int)check_crc32Val)
            {
                return false;
            }

            ptid = Convert.ToInt32(lipinma.Substring(2, 3));
            ptrepeat = Convert.ToInt32(lipinma.Substring(5, 1));
            zoneID = Convert.ToInt32(lipinma.Substring(6, 3));

            return true;
        }
    }
}
