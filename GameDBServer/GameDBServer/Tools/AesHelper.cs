using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Server.Tools
{
    /// <summary>
    /// AES encryption helper class
    /// </summary>
    class AesHelper
    {
        /// <summary>
        /// AES encrypted byte stream
        /// </summary>
        /// <param name="data">Plaintext stream</param>
        /// <param name="passwd">password</param>
        /// <param name="saltValue">Salt value</param>
        /// <returns>Encrypted byte stream</returns>
        public static byte[] AesEncryptBytes(byte[] data, string passwd, string saltValue)
        {
            // Salt value (encryption and decryption in the process of this value must be consistent)
            // Password value plus decryption in the program this value must be consistent)
            byte[] saltBytes = UTF8Encoding.UTF8.GetBytes(saltValue);

            // AesManaged - Advanced encryption standard (AES) symmetric algorithm management class
            AesManaged aes = new AesManaged();

            // Rfc2898DeriveBytes - By using a pseudo-random number generator based on HMACSHA1, a cryptographic-based key derivation function (PBKDF2 - a cryptographic-based key derivation function)
            // Derive the key with the password and salt
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(passwd, saltBytes);

            /*
            * AesManaged.BlockSize - The block size of the encryption operation (in units)：bit）
            * AesManaged.LegalBlockSizes - The symmetric algorithm supports block sizes (in units)：bit）
            * AesManaged.KeySize - The symmetric algorithm's key size (in units)：bit）
            * AesManaged.LegalKeySizes - The symmetric algorithm supports the key size (in units)：bit）
            * AesManaged.Key - The key of the symmetric algorithm
            * AesManaged.IV - 对称算法的密钥大小
            * Rfc2898DeriveBytes.GetBytes(int 需要生成的伪随机密钥字节数) - 生成密钥
            */

            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // 用当前的 Key 属性和初始化向量 IV 创建对称加密器对象
            ICryptoTransform encryptTransform = aes.CreateEncryptor();

            // 加密后的输出流
            MemoryStream encryptStream = new MemoryStream();

            // 将加密后的目标流（encryptStream）与加密转换（encryptTransform）相连接
            CryptoStream encryptor = new CryptoStream(encryptStream, encryptTransform, CryptoStreamMode.Write);

            // 将一个字节序列写入当前 CryptoStream （完成加密的过程）
            encryptor.Write(data, 0, data.Length);
            encryptor.Close();

            // 将加密后所得到的流转换成字节数组
            return encryptStream.ToArray();
        }

        /// <summary>
        /// AES解密字节流
        /// </summary>
        /// <param name="encryptData">密文字节流</param>
        /// <param name="passwd">密码</param>
        /// <param name="saltValue">盐值</param>
        /// <returns>解密后的明文字节流</returns>
        public static byte[] AesDecryptBytes(byte[] encryptData, string passwd, string saltValue)
        {
            // 盐值（与加密时设置的值必须一致）
            // 密码值（与加密时设置的值必须一致）
            byte[] saltBytes = Encoding.UTF8.GetBytes(saltValue);

            AesManaged aes = new AesManaged();
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(passwd, saltBytes);

            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // 用当前的 Key 属性和初始化向量 IV 创建对称解密器对象
            ICryptoTransform decryptTransform = aes.CreateDecryptor();

            // 解密后的输出流
            MemoryStream decryptStream = new MemoryStream();

            // 将解密后的目标流（decryptStream）与解密转换（decryptTransform）相连接
            CryptoStream decryptor = new CryptoStream(decryptStream, decryptTransform, CryptoStreamMode.Write);

            try
            {
                // 将一个字节序列写入当前 CryptoStream （完成解密的过程）
                decryptor.Write(encryptData, 0, encryptData.Length);
                decryptor.Close();
            }
            catch (System.Exception)
            {
                return null; //解密失败
            }

            // 将解密后所得到的流转换为字符串
            return decryptStream.ToArray();
        }
    }
}
