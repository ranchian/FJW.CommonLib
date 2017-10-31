using System;

namespace FJW.DI
{
    /// <summary>
    /// ioc容器接口
    /// </summary>
    public interface IObjectContainer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TImplementer"></typeparam>
        /// <param name="instance"></param>
        /// <param name="interceptor"></param>
        void RegisterInstance<TImplementer>(TImplementer instance, Type interceptor)where TImplementer : class;

        /// <summary>
        /// 注册implementation组件
        /// </summary>
        /// <param name="implementationType"></param>
        /// <param name="interceptor"></param>
        /// <param name="life"></param>
        void RegisterType(Type implementationType, Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient);

        /// <summary>
        /// 注册 接口+实现 组件 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="interceptor"></param>
        /// <param name="life"></param>
        void RegisterType(Type serviceType, Type implementationType, Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient);

        /// <summary>
        /// 注册 接口+实现 组件 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="keyed"></param>
        /// <param name="interceptor"></param>
        /// <param name="life"></param>
        void RegisterType(Type serviceType, Type implementationType, object keyed, Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient);

        /// <summary>
        /// 注册 接口+实现 组件 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementer"></typeparam>
        /// <param name="interceptor"></param>
        /// <param name="life"></param>
        void Register<TService, TImplementer>(Type interceptor,ComponentLifeStyle life = ComponentLifeStyle.Transient)
            where TService : class
            where TImplementer : class, TService;

        /// <summary>
        /// 注册 接口+实现 组件 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementer"></typeparam>
        /// <param name="keyed"></param>
        /// <param name="interceptor"></param>
        /// <param name="life"></param>
        void Register<TService, TImplementer>(object keyed, Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient)
            where TService : class
            where TImplementer : class, TService;

        /// <summary>
        /// 获取实例（泛型类）
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        TService Resolve<TService>() where TService : class;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="parameter"></param>
        /// <returns></returns>
        TService Resolve<TService, TParameter>(TParameter parameter) where TService : class;
        /// <summary>
        /// 获取实例（泛型类）
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        TService ResolveKeyed<TService>(object keyed) where TService : class;
        /// <summary>
        /// 获取实例（object类型）
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object Resolve(Type serviceType);

        /// <summary>
        /// 获取实例（object类型）
        /// </summary>
        /// <param name="keyed"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object ResolveKeyed(object keyed, Type serviceType);
    }
}
