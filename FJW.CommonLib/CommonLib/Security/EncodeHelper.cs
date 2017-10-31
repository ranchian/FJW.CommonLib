using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace FJW.CommonLib.Security
{
    /// <summary>
    /// 编码帮助类
    /// </summary>
    public class EncodeHelper
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string ToRandomMd5(string input, int len = 10)
        {
            using (var md5Provider = new MD5CryptoServiceProvider())
            {
                //获取一个256以内的随机数,用于充当 "盐"
                var salt = (byte)Math.Abs(new object().GetHashCode() % 256);
                input += salt;
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = md5Provider.ComputeHash(bytes);
                byte[] n = new byte[len];
                Array.Copy(hash, hash.Length - len, n, 0, len);
                n[0] = salt;
                return Convert.ToBase64String(n);
                //Encoding.ASCII.GetString(n);
            }
        }

        /// <summary>
        /// MD5验证
        /// </summary>
        /// <param name="input"></param>
        /// <param name="rmd5"></param>
        /// <returns></returns>
        public static bool EqualsRandomMd5(string input, string rmd5)
        {
            var arr = Convert.FromBase64String(rmd5);
            //Encoding.ASCII.GetBytes(rmd5);
            //将盐取出来
            var salt = arr[0];
            using (var md5Provider = new MD5CryptoServiceProvider())
            {
                input += salt;
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = md5Provider.ComputeHash(bytes);
                int idx = hash.Length - arr.Length;
                for (int i = 1; i < arr.Length; i++)
                {
                    if (hash[i + idx] != arr[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 获取文件的特征串
        /// </summary>
        /// <param name="sInputFilename">输入文件</param>
        /// <returns></returns>
        public string HashFile(string sInputFilename)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            FileStream inFile = new System.IO.FileStream(sInputFilename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            byte[] bInput = new byte[inFile.Length];
            inFile.Read(bInput, 0, bInput.Length);
            inFile.Close();
            string encoded = BitConverter.ToString(md5.ComputeHash(bInput)).Replace("-", "");
            return encoded;
        }

        /// <summary>
        /// 解码字符串
        /// </summary>
        /// <param name="sInputString">输入文本</param>
        /// <returns></returns>
        public static string DecryptString(string sInputString)
        {
            //char[] sInput = sInputString.ToCharArray();
            try
            {
                sInputString = sInputString.Replace('(', '/');
                sInputString = sInputString.Replace(')', '+');
                byte[] bOutput = Convert.FromBase64String(sInputString);
                return Encoding.UTF8.GetString(bOutput);
            }
            catch (ArgumentNullException)
            {
                //base 64 字符数组为null
                return "";
            }
            catch (FormatException)
            {
                //长度错误，无法整除4
                return "";
            }
        }

        /// <summary>
        /// 编码字符串
        /// </summary>
        /// <param name="sInputString">输入文本</param>
        /// <returns></returns>
        public static string EncryptString(string sInputString)
        {
            byte[] bInput = Encoding.UTF8.GetBytes(sInputString);
            try
            {
                string v = Convert.ToBase64String(bInput, 0, bInput.Length, Base64FormattingOptions.None);
                v = v.Replace('/', '(');
                v = v.Replace('+', ')');
                return v;
            }
            catch (ArgumentNullException)
            {
                //二进制数组为NULL.
                return "";
            }
            catch (ArgumentOutOfRangeException)
            {
                //长度不够
                return "";
            }
        }

        [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr destination, int length);

        /// <summary>
        /// 创建Key
        /// </summary>
        /// <returns></returns>
        public static string GenerateKey()
        {
            // 创建一个DES 算法的实例。自动产生Key
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();
            // 返回自动创建的Key 用于加密
            return ASCIIEncoding.ASCII.GetString(desCrypto.Key);
        }
        
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="sInputString">输入字符</param>
        /// <param name="sKey">Key</param>
        /// <returns>加密结果</returns>
        public static string EncryptString(string sInputString, string sKey)
        {
            byte[] data = Encoding.Default.GetBytes(sInputString);
            byte[] result;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = des.CreateEncryptor();
            result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            string desString = "";
            for (int i = 0; i < result.Length; i++)
            {
                desString += result[i] + "-";
            }
            //return desString.TrimEnd("-");
            return BitConverter.ToString(result);
        }
        
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="sInputString">输入字符</param>
        /// <param name="sKey">Key</param>
        /// <returns>解密结果</returns>
        public static string DecryptString(string sInputString, string sKey)
        {
            string[] sInput = sInputString.Split("-".ToCharArray());
            byte[] data = new byte[sInput.Length];
            byte[] result;
            for (int i = 0; i < sInput.Length; i++)
                data[i] = byte.Parse(sInput[i], System.Globalization.NumberStyles.HexNumber);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = des.CreateDecryptor();
            result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            return Encoding.Default.GetString(result);
        }
        
        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="sInputFilename">输入文件</param>
        /// <param name="sOutputFilename">输出文件</param>
        /// <param name="sKey">Key</param>
        public static void EncryptFile(string sInputFilename,
        string sOutputFilename,
        string sKey)
        {
            FileStream fsInput = new FileStream(sInputFilename,
              FileMode.Open,
              FileAccess.Read);
            FileStream fsEncrypted = new FileStream(sOutputFilename,
              FileMode.Create,
              FileAccess.Write);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = des.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fsEncrypted,
              desencrypt,
              CryptoStreamMode.Write);
            byte[] bytearrayinput = new byte[fsInput.Length];
            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Close();
            fsInput.Close();
            fsEncrypted.Close();
        }
        
        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="sInputFilename">输入文件</param>
        /// <param name="sOutputFilename">输出文件</param>
        /// <param name="sKey">Key</param>
        public static void DecryptFile(string sInputFilename,
        string sOutputFilename,
        string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            FileStream fsread = new FileStream(sInputFilename,
              FileMode.Open,
              FileAccess.Read);
            ICryptoTransform desdecrypt = des.CreateDecryptor();
            CryptoStream cryptostreamDecr = new CryptoStream(fsread,
              desdecrypt,
              CryptoStreamMode.Read);
            StreamWriter fsDecrypted = new StreamWriter(sOutputFilename);
            fsDecrypted.Write(new StreamReader(cryptostreamDecr).ReadToEnd());
            fsDecrypted.Flush();
            fsDecrypted.Close();
        }
    }
}
