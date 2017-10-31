using System;
using System.Text;
using System.Collections.Generic;

namespace FJW.CommonLib.Utils
{
    /// <summary>
    /// 字典和字符串之间的转换
    /// </summary>
    public static class DictionaryHelper
    {
        /// <summary>
        /// 将字典中的KV键值按照kvSplit拼接成字符串，然后将这些拼接的字符串按照itemSplit拼接成最终字符串返回
        /// </summary>
        /// <typeparam name="TKey">key类型</typeparam>
        /// <typeparam name="TVlaue">value类型</typeparam>
        /// <param name="entities">字典</param>
        /// <param name="kvSplit">KV间字符</param>
        /// <param name="itemSplit">字典项之间字符</param>
        /// <returns></returns>
        public static string DicToString<TKey, TVlaue>(Dictionary<TKey, TVlaue> entities, string kvSplit, string itemSplit)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                foreach (KeyValuePair<TKey, TVlaue> pair in entities)
                {
                    builder.Append(string.Format("{0}{1}{2}", pair.Key, kvSplit, pair.Value));
                    builder.Append(itemSplit);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("转换失败，原因：" + ex.Message);
            }
            return builder.ToString().TrimEnd(itemSplit.ToCharArray());
        }

        /// <summary>
        /// 将字符串按照指定的itemSplit进行分割成数组，然后将分割数组中的每一项按照kvSplit分割后添加到字典中，生成字典
        /// </summary>
        /// <param name="content">字典字符串</param>
        /// <param name="kvSplit">KV间字符</param>
        /// <param name="itemSplit">字典项之间字符数组</param>
        /// <returns></returns>
        public static Dictionary<string, string> StringToDic(string content, char kvSplit, string[] itemSplit)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                if (!string.IsNullOrEmpty(content))
                {
                    string[] items = content.Split(itemSplit, StringSplitOptions.RemoveEmptyEntries);
                    if (items.Length > 0)
                    {
                        string[] pair;
                        foreach (string item in items)
                        {
                            if (item.Split(kvSplit).Length < 2)
                            {
                                continue;
                            }
                            pair = item.Split(kvSplit);
                            if (pair.Length == 2)
                                result[pair[0]] = pair[1];
                            else if (pair.Length > 2)
                                result[pair[0]] = pair[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("转换失败，原因：" + ex.Message);
            }
            return result;
        }
    }
}