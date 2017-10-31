using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace FJW.CommonLib.Encrypt
{
    /// <summary>
    /// AES加密帮助类
    /// </summary>
    public class AESEncodeHelper : IEncryptManager
    {
        #region 秘钥对
        private string _Key { get; set; }
        private string _Vector { get; set; }

        public AESEncodeHelper(string key, string vector)
        {
            _Key = key;
            _Vector = vector;
        }
        #endregion

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">明文数据</param>
        /// <returns></returns>
        public string EncryptData(string data)
        {
            if (string.IsNullOrEmpty(data)) return "";
            Byte[] plainBytes = Encoding.UTF8.GetBytes(data);
            Byte[] bKey = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(_Key.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(_Vector.PadRight(bVector.Length)), bVector, bVector.Length);
            Byte[] Cryptograph = null; // 加密后的密文  
            Rijndael Aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流  
                using (MemoryStream Memory = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象  
                    using (CryptoStream Encryptor = new CryptoStream(Memory,
                     Aes.CreateEncryptor(bKey, bVector),
                     CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流  
                        Encryptor.Write(plainBytes, 0, plainBytes.Length);
                        Encryptor.FlushFinalBlock();

                        Cryptograph = Memory.ToArray();
                    }
                }
            }
            catch
            {
                Cryptograph = null;
            }
            return Convert.ToBase64String(Cryptograph);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">密文数据</param>
        /// <returns></returns>
        public string DecryptData(string data)
        {
            if (string.IsNullOrEmpty(data)) return "";
            Byte[] encryptedBytes = Convert.FromBase64String(data);
            Byte[] bKey = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(_Key.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(_Vector.PadRight(bVector.Length)), bVector, bVector.Length);
            Byte[] original = null; // 解密后的明文  
            Rijndael Aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流，存储密文  
                using (MemoryStream Memory = new MemoryStream(encryptedBytes))
                {
                    // 把内存流对象包装成加密流对象  
                    using (CryptoStream Decryptor = new CryptoStream(Memory,
                    Aes.CreateDecryptor(bKey, bVector),
                    CryptoStreamMode.Read))
                    {
                        // 明文存储区  
                        using (MemoryStream originalMemory = new MemoryStream())
                        {
                            Byte[] Buffer = new Byte[1024];
                            Int32 readBytes = 0;
                            while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                            {
                                originalMemory.Write(Buffer, 0, readBytes);
                            }

                            original = originalMemory.ToArray();
                        }
                    }
                }
            }
            catch
            {
                original = null;
            }
            return Encoding.UTF8.GetString(original);  
        }
    }
}
