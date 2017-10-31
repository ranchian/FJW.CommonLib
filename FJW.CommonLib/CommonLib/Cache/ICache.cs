﻿using System;
using System.Collections.Generic;

namespace FJW.CommonLib.Cache
{
    public interface ICache
    {
        /// <summary>
        /// 设置服务器缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存数据</param>
        /// <param name="ttl">过期时间</param>
        void PutWebCache(string key, string value, int ttl = 10);

        /// <summary>
        /// 读取服务器缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        string GetWebCache(string key);

        /// <summary>
        /// 设置Redis缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存数据</param>
        /// <param name="ttl">过期时间</param>
        void PutRedisCache(string key, string value, int ttl = 10);

        /// <summary>
        /// 读取Redis缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        string GetRedisCache(string key);

        /// <summary>
        /// 获取Redis缓存中的对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <param name="makeIds"></param>
        /// <param name="getCacheObject"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        List<T> GetRedisCacheObjectList<T>(List<string> ids, Func<T, string> makeIds, Func<List<string>, List<T>> getCacheObject, int ttl = 10);

        /// <summary>
        /// 删除服务器缓存
        /// </summary>
        /// <param name="key"></param>
        void DelWebCache(string key);

        /// <summary>
        /// 删除  Redis 缓存
        /// </summary>
        /// <param name="key"></param>
        void DelRedisCache(string key);

        /// <summary>
        /// 删除Redis缓存
        /// </summary>
        /// <param name="key"></param>
        void DelRedisStreamCache(string key);
    }
}