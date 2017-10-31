namespace FJW.CommonLib.Utils
{
    /// <summary>
    ///  单件   禁止在继承此类的任何类，使用成员变量
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static readonly object SynRoot = new object();

        /// <summary>
        ///     获取该类的全局唯一实例
        /// </summary>
        /// <returns></returns>
        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (SynRoot)
                {
                    if (_instance != null)
                    {
                        return _instance;
                    }
                    return _instance = new T();
                }
            }
        }
    }
}
