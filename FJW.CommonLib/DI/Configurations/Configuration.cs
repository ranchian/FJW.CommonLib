using System;

namespace FJW.DI
{
    public class Configuration
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static readonly Configuration Instance = new Configuration();
        /// <summary>
        /// 
        /// </summary>
        private Configuration()
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Configuration Create()
        {
            return Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementer"></typeparam>
        /// <param name="interceptor"></param> 
        /// <param name="life"></param>
        /// <returns></returns>
        public Configuration SetDefault<TService, TImplementer>(Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Singleton)
            where TService : class

            where TImplementer : class, TService
        {
            ObjectContainer.Register<TService, TImplementer>(interceptor,life);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementer"></typeparam>
        /// <param name="instance"></param>
        /// <param name="interceptor"></param>
        /// <returns></returns>
        public Configuration SetDefault<TService, TImplementer>(TImplementer instance, Type interceptor)
            where TService : class
            where TImplementer : class, TService
        {
            ObjectContainer.RegisterInstance(instance, interceptor);
            return this;
        }
    }
}
