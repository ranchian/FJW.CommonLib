using System;
using System.Collections.Generic;

namespace FJW.Repository
{
    /// <summary>
    /// 多个结果集读取器
    /// </summary>
    public interface IDynamicReader: IDisposable
    {
        /// <summary>
        /// 顺序读取结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> Read<T>();
    }
}
