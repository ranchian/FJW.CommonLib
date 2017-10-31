using System;
using System.Text;
using FJW.CommonLib.Configuration;

namespace FJW.CommonLib.Encrypt
{
    /// <summary>
    /// 加解密算法第一版
    /// </summary>
    public class EncryptV1 : IEncryptManager
    {
        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="ev">加密算法版本</param>
        /// <returns></returns>
        public string EncryptData(string data)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    int num = ConfigManager.GetWebConfig("EncryptNum", 64);
                    string s1 = XORWithNum(data, num);
                    result = Convert.ToBase64String(Encoding.UTF8.GetBytes(s1));
                }
                catch (Exception ex)
                {
                    throw new Exception("加密失败，原因：" + ex.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="dv">解密算法版本</param>
        /// <returns></returns>
        public string DecryptData(string data)
        {
            string result = string.Empty;
            try
            {
                string s1 = string.Empty;
                s1 = Encoding.UTF8.GetString(Convert.FromBase64String(data));
                int num = ConfigManager.GetWebConfig("EncryptNum", 64);
                result = XORWithNum(s1, num);
            }
            catch (Exception ex)
            {
                throw new Exception("解密失败，原因：" + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 异或
        /// </summary>
        /// <param name="source"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private static string XORWithNum(string source, int num)
        {
            char[] result = new char[source.Length];
            int size = source.Length;
            for (int i = 0; i < size; i++)
            {
                result[i] = (char)(source[i] ^ num);
            }
            string sresult = new string(result);
            return sresult;
        }
    }
}
