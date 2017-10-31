using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

using FJW.CommonLib.Utils;

namespace FJW.CommonLib.ExtensionMethod
{
    /// <summary>
    /// string的一些扩展方法
    /// </summary>
    public static class StringExtensionMethods
    {
        /// <summary>
        /// 将string类型转换成int类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static int ToInt(this string s, int DefaultValue = 0)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                int result;
                if (int.TryParse(s, out result))
                    return result;
            }

            return DefaultValue;
        }

        /// <summary>
        /// 将string类型转换成bool类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static bool ToBool(this string s, bool DefaultValue = false)
        {
            if (s == "false")
                return false;
            else if (s == "true")
                return true;

            return DefaultValue;
        }

        /// <summary>
        /// 将string类型转换成datetime类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string s)
        {
            DateTime result;
            if (!string.IsNullOrWhiteSpace(s) && DateTime.TryParse(s, out result))
            {
                return result;
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// 将string类型转换成decimal类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static decimal ToDecimal(this string s, decimal DefaultValue = 0m)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                decimal result;
                if (decimal.TryParse(s, out result))
                    return result;
            }

            return DefaultValue;

        }

        /// <summary>
        /// 将string类型转换成double类型
        /// </summary>
        /// <param name="s">目标字符串</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static double ToDouble(this string s, double DefaultValue = 0.00)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                double result;
                if (double.TryParse(s, out result))
                    return result;
            }

            return DefaultValue;

        }

        /// <summary>
        /// 转换类型浮点
        /// </summary>
        /// <param name="info">转换信息</param>
        public static float ToFloat(this string info)
        {
            float result;
            if (!float.TryParse(info, out result) || float.IsNaN(result))
            {
                result = 0F;
            }
            return result;
        }

        /// <summary>
        /// 转换类型长整型
        /// </summary>
        /// <param name="info">转换信息</param>
        public static long ToLong(this string info)
        {
            long result;
            long.TryParse(info, out result);
            return result;
        }

        /// <summary>
        /// 转换类型长整型
        /// </summary>
        /// <param name="info">转换信息</param>
        public static ulong ToULong(this string info)
        {
            ulong result;
            ulong.TryParse(info, out result);
            return result;
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string str)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }

        /// <summary>
        /// 字符串如果超过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string ToSubStr(this string p_SrcString, int p_Length, string p_TailString)
        {
            if (p_Length < p_SrcString.Length)
                return p_SrcString.Substring(0, p_Length) + p_TailString;
            else
                return p_SrcString.Substring(0);
        }

        /// <summary>
        /// 大数字每隔三位加逗号,保留两位小数
        /// </summary>
        /// <returns></returns>
        public static string ToBigNumStr(this string number, int tag = 2)
        {
            string numberStr = number;
            string endstr = "";
            int pos = numberStr.IndexOf('.');
            if (pos > -1)
            {
                numberStr = numberStr.Substring(0, pos);
                if (tag != 0)
                {
                    if (number.Length - pos > (tag + 1))
                    {
                        endstr = number.Substring(pos, tag + 1);
                    }
                    else
                    {
                        endstr = number.Substring(pos, number.Length - pos);
                    }
                }
            }
            int length = numberStr.Length;
            if (length <= 3)
            {
                return numberStr + endstr;
            }

            List<char> result = new List<char>(numberStr);
            for (int i = 0; i < length; ++i)
            {
                if ((i + 1) % 3 == 0 && (i + 1) < length)
                {
                    result.Insert(length - i - 1, ',');
                }
            }
            string s = new string(result.ToArray());
            return s + endstr;
        }

        /// <summary>
        /// 格式化字符串中的指定项为数组中相应对象的字符串表示形式(例："abc {0} {1}".ToFormat("d","e"))
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string ToFormat(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        /// <summary>
        /// 将指定字符串进行MD5加密
        /// </summary>
        /// <param name="pOriginalString">原始字符串</param>
        /// <returns>MD5值</returns>
        public static string ToMD5String(this string pOriginalString)
        {
            //将输入转换为ASCII 字符编码
            ASCIIEncoding enc = new ASCIIEncoding();
            //将字符串转换为字节数组
            byte[] buffer = enc.GetBytes(pOriginalString);
            //创建MD5实例
            MD5 md5Provider = new MD5CryptoServiceProvider();
            //进行MD5加密
            byte[] hash = md5Provider.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            //拼装加密后的字符
            for (int i = 0; i < hash.Length; i++)
            {
                sb.AppendFormat("{0:x2}", hash[i]);
            }
            //输出加密后的字符串
            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 扩展方法：根据JSON字符串反序列化成对象
        /// </summary>
        /// <param name="pObj">序列化字符串</param>
        /// <returns>解析后的对象</returns>
        public static T ToJsonObject<T>(this string pObj)
        {
            if (string.IsNullOrEmpty(pObj))
            {
                return default(T);
            }
            return JsonHelper.JsonDeserialize<T>(pObj);
        }

        /// <summary>
        /// 判断是否为GUID
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return Guid.Empty;

            Guid test;
            Guid.TryParse(str, out test);
            return test;
        }

        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns></returns>
        public static int GetStringLength(this string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }

        /// <summary>
        /// 转换成Base64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToBase64String(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "";
            return Convert.ToBase64String(Encoding.Default.GetBytes(str));
        }

        /// <summary>
        /// Base64转换成普通文本
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Base64ToString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "";
            return Encoding.Default.GetString(Convert.FromBase64String(str));
        }

        public static string ConvertPhone(this string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return string.Empty;
            }
            if (phone.Length < 7)
            {
                return phone;
            }
            return string.Format("{0}****{1}", phone.Substring(0, 3), phone.Substring(7, phone.Length - 7));
        }
    }
}
