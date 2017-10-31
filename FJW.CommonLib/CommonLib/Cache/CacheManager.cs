using System;
using System.Web;
using System.Linq;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System.Collections.Generic;

using FJW.CommonLib.Redis;

namespace FJW.CommonLib.Cache
{
    /// <summary>
    /// 缓存相关操作
    /// </summary>
    public class CacheManager
    {
        /// <summary>
        /// Formatting
        /// </summary>
        const string formatting = "[{0}]:[{1}]";
        /// <summary>
        /// FormattingV
        /// </summary>
        const string formattingV = "[{0}]:[{1}]V";
        /// <summary>
        /// FormattingPage
        /// </summary>
        const string FormattingPage = "[{0}]:[{1}]:[{2}]";

        /// <summary>
        /// 插入web环境缓存
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存key</param>
        /// <param name="value">要插入的数据</param>
        /// <param name="ttl">过期时间</param>
        public static void PutWebCache(string namespaces, string key, string value, int ttl = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                HttpRuntime.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 0, ttl));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("web环境获取缓存失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">key</param>
        /// <returns>缓存数据</returns>
        public static string GetWebCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                return (string)HttpRuntime.Cache.Get(key);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("web环境获取缓存失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 插入缓存到Redis服务器
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <param name="value">要插入的数据</param>
        /// <param name="ttl">过期时间</param>
        public static void PutRedisCache(string namespaces, string key, string value, int ttl = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                if (ttl > 0)
                    RedisHelper.SetEntry(key, value, new TimeSpan(0, 0, ttl));
                else
                    RedisHelper.SetEntry(key, value);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("插入缓存到Redis服务器失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 获取Redis中的缓存数据
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存数据</returns>
        public static string GetRedisCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                return RedisHelper.GetValue(key);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("获取Redis中的缓存数据失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 插入缓存流到Redis中
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <param name="stream">要插入的数据流</param>
        /// <param name="ttl">过期时间</param>
        public static void PutRedisStreamCache(string namespaces, string key, byte[] stream, int ttl = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                if (stream.Length <= 0) throw new ArgumentNullException();
                key = string.Format(formattingV, namespaces, key);
                using (RedisClient client = RedisHelper.GetClient())
                {
                    client.Set(key, stream, new TimeSpan(0, 0, ttl));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("插入缓存流到Redis中失败，原因：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取Redis中的缓存流
        /// </summary>
        /// <param name="namespaces">命名空间</param>
        /// <param name="key">缓存Key</param>
        /// <returns>缓存流</returns>
        public static byte[] GetRedisStreamCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formattingV, namespaces, key);
                using (RedisClient client = RedisHelper.GetClient())
                {
                    return client.Get(key);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("获取Redis中的缓存流失败，原因：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取Redis缓存中的对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="namespaces"></param>
        /// <param name="ids"></param>
        /// <param name="makeIds"></param>
        /// <param name="getCacheObject"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public static List<T> GetRedisCacheObjectList<T>(string namespaces, List<string> ids, Func<T, string> makeIds, Func<List<string>, List<T>> getCacheObject, int ttl = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (ids == null || ids.Count <= 0) throw new ArgumentNullException();
                //拼接RedisKey
                var cacheKeys = new List<string>();
                ids.ForEach(s =>
                {
                    cacheKeys.Add(string.Format(formatting, namespaces, s));
                });
                var strList = RedisHelper.GetValues(cacheKeys);
                var resultObj = new List<T>();
                var resultDic = new Dictionary<string, T>();
                strList.ForEach(s =>
                {
                    if (string.IsNullOrEmpty(s)) return;
                    T temp = JsonConvert.DeserializeObject<T>(s);
                    if (temp == null) return;
                    resultObj.Add(temp);
                    string skey = makeIds(temp);
                    if (!string.IsNullOrEmpty(skey))
                        resultDic[skey] = temp;
                });
                List<string> resultIds = null;
                if (makeIds != null)
                {
                    //拼接结果集的ID
                    resultIds = resultObj.Select(makeIds).ToList();
                }
                if (resultIds == null)
                    return (from id in ids where resultDic.ContainsKey(id) select resultDic[id]).ToList();

                //原始ID和结果集ID取差集
                List<string> otherIds = ids.Except(resultIds).ToList();
                if (otherIds.Count != 0)
                {
                    if (getCacheObject != null)
                    {
                        //差集从数据库取对象
                        List<T> otherObject = getCacheObject(otherIds);
                        var dicOtherObjectJson = new Dictionary<string, string>();
                        if (otherObject != null && otherObject.Count > 0)
                        {
                            otherObject.ForEach(o =>
                            {
                                string json = JsonConvert.SerializeObject(o);
                                string sid = makeIds(o);
                                string key = string.Format(formatting, namespaces, sid);
                                strList.Add(json);
                                dicOtherObjectJson[key] = json;
                                if (!string.IsNullOrEmpty(sid))
                                    resultDic[sid] = o;
                            });
                            using (RedisClient client = RedisHelper.GetClient())
                            {
                                foreach (var key in dicOtherObjectJson.Keys)
                                {
                                    string objJson = dicOtherObjectJson[key];
                                    client.SetEntry(key, objJson, new TimeSpan(0, 0, ttl));
                                }
                            }
                        }
                    }
                }
                return (from id in ids where resultDic.ContainsKey(id) select resultDic[id]).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("获取Redis中的缓存数据失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 删除服务器缓存
        /// </summary>
        /// <param name="namespaces"></param>
        /// <param name="key"></param>
        public static void DelWebCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                HttpRuntime.Cache.Remove(key);
            }
            catch (Exception ex)
            {
                throw new Exception("web环境删除缓存失败，原因：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除Redis缓存
        /// </summary>
        /// <param name="namespaces"></param>
        /// <param name="key"></param>
        public static void DelRedisCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formatting, namespaces, key);
                using (RedisClient client = RedisHelper.GetClient())
                {
                    client.Remove(key);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("删除Redis中的缓存数据失败，原因：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="namespaces"></param>
        /// <param name="key"></param>
        public static void DelRedisStreamCache(string namespaces, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(namespaces)) throw new ArgumentNullException();
                if (string.IsNullOrEmpty(key)) throw new ArgumentNullException();
                key = string.Format(formattingV, namespaces, key);
                RedisHelper.Remove(key);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("删除Redis中的缓存流失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
        }
    }
}