using System;

namespace FJW.DI
{
    /// <summary>
    /// 
    /// </summary>
    public class ObjectContainer
    {
        /// <summary>
        /// 
        /// </summary>
        private static IObjectContainer _current;
        /// <summary>设置 IObjectContainer.
        /// </summary>
        /// <param name="container"></param>
        public static void SetContainer(IObjectContainer container)
        {
            _current = container;
        }

        /// <summary>
        /// 注册implementation组件
        /// </summary>
        /// <param name="implementationType"></param>
        /// <param name="interceptor"></param>
        /// <param name="life"></param>
        public static void RegisterType(Type implementationType, Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient)
        {
            _current.RegisterType(implementationType, interceptor, life);
        }

        /// <summary>
        /// 注册implementation组件
        /// </summary>
        /// <param name="implementationType"></param>
        /// <param name="keyed"></param>
        /// <param name="interceptor"></param>
        /// <param name="life"></param>
        public static void RegisterType(Type implementationType, object keyed, Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient)
        {
            _current.RegisterType(implementationType, interceptor, life);
        }

        /// <summary>
        /// 注册 接口+实现 组件
        /// </summary>
        /// <param name="serviceType">serviceType 运行为 null ,为 null 时候注册implementationType类</param>
        /// <param name="implementationType"></param>
        /// <param name="interceptor"></param>
        /// <param name="life"></param>
        public static void RegisterType(Type serviceType, Type implementationType, Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient)
        {
            _current.RegisterType(serviceType, implementationType, interceptor, life);
        }

        /// <summary>
        /// 注册 接口+实现 组件
        /// </summary>
        /// <param name="serviceType">serviceType 运行为 null ,为 null 时候注册implementationType类</param>
        /// <param name="implementationType"></param>
        /// <param name="keyed"></param>
        /// <param name="interceptor"></param>
        /// <param name="life"></param>
        public static void RegisterType(Type serviceType, Type implementationType, object keyed, Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient)
        {
            _current.RegisterType(serviceType, implementationType, keyed, interceptor, life);
        }

        /// <summary>
        /// 注册 接口+实现 组件
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementer"></typeparam>
        /// <param name="interceptor"></param>
        /// <param name="life"></param>
        public static void Register<TService, TImplementer>(Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient)
            where TService : class
            where TImplementer : class, TService
        {
            _current.Register<TService, TImplementer>(interceptor, life);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementer"></typeparam>
        /// <param name="keyed"></param>
        /// <param name="interceptor"></param>
        /// <param name="life"></param>
        public static void Register<TService, TImplementer>(object keyed, Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient)
            where TService : class
            where TImplementer : class, TService
        {
            _current.Register<TService, TImplementer>(keyed, interceptor, life);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TImplementer"></typeparam>
        /// <param name="instance"></param>
        /// <param name="interceptor"></param>
        public static void RegisterInstance<TImplementer>(TImplementer instance, Type interceptor)where TImplementer : class
        {
            _current.RegisterInstance<TImplementer>(instance, interceptor);
        }

        /// <summary>
        /// 获取实例（泛型类）
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService Resolve<TService>() where TService : class
        {
            return _current.Resolve<TService>();
        }
        /// <summary>
        /// 获取实例（泛型类）
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TParameter"></typeparam>
        /// <returns></returns>
        public static TService Resolve<TService, TParameter>(TParameter parameter) where TService : class
        {
            return _current.Resolve<TService, TParameter>(parameter);
        }
        /// <summary>
        /// 获取实例（泛型类）
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService ResolveKeyed<TService>(object keyed) where TService : class
        {
            return _current.ResolveKeyed<TService>(keyed);
        }
        /// <summary>
        /// 获取实例（object类型）
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static object Resolve(Type serviceType)
        {
            return _current.Resolve(serviceType);
        }

        /// <summary>
        /// 获取实例（object类型）
        /// </summary>
        /// <param name="keyed"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static object Resolve(object keyed, Type serviceType)
        {
            return _current.ResolveKeyed(keyed, serviceType);
        }
    }
}
