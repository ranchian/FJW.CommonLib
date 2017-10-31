using System;
using Autofac;
using Autofac.Builder; 

namespace FJW.DI
{
    /// <summary>
    /// Autofac 对象容器
    /// </summary>
    public class AutofacObjectContainer : IObjectContainer
    {
        private readonly IContainer _container;

        

        /// <summary>
        /// 参数化实例
        /// </summary>   
        /// <param name="containerBuilder"></param>
        public AutofacObjectContainer( ContainerBuilder containerBuilder)
        { 
            _container = containerBuilder.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        public IContainer Container
        {
            get { return _container; }
        }

        /// <summary>
        /// 注册单例
        /// </summary>
        /// <typeparam name="TImplementer">实现类型</typeparam>
        /// <param name="instance"></param>
        /// <param name="interceptor"></param>
        public void RegisterInstance<TImplementer>(TImplementer instance, Type interceptor) where TImplementer : class
        {
            var builder = new ContainerBuilder();
            var registrationBuilder = builder.RegisterInstance(instance);
            //if (interceptor != null)
            //    registrationBuilder.EnableInterfaceInterceptors().InterceptedBy(interceptor);

            builder.Update(_container);
        }

        /// <summary>
        /// 注册 接口+实现 组件 (默认单例)
        /// </summary>
        /// <typeparam name="TService">（父类或接口）</typeparam>
        /// <typeparam name="TImplementer">实现类型</typeparam>
        /// <param name="interceptor"></param>
        /// <param name="life"></param>
        public void Register<TService, TImplementer>(Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient)
            where TService : class
            where TImplementer : class, TService
        {
            Register<TService, TImplementer>(string.Empty, interceptor, life);
        }

        /// <summary>
        /// 注册 接口+实现 组件 (默认单例)
        /// </summary>
        /// <typeparam name="TService">（父类或接口）</typeparam>
        /// <typeparam name="TImplementer">实现类型</typeparam>
        /// <param name="keyed"></param>
        /// <param name="interceptor">aop切面类</param>
        /// <param name="life">生命周期</param>
        public void Register<TService, TImplementer>(object keyed, Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient)
            where TService : class
            where TImplementer : class, TService
        {
            RegisterType(typeof(TService), typeof(TImplementer), keyed, interceptor, life);
        }

        /// <summary>
        /// 注册implementation组件(默认单例)
        /// </summary>
        /// <param name="implementationType">实现类型</param> 
        /// <param name="interceptor">aop切面类</param>
        /// <param name="life">生命周期</param>
        public void RegisterType(Type implementationType, Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient)
        {
            RegisterType(null, implementationType, interceptor, life);
        }

        /// <summary>
        /// 注册 接口+实现 组件 (默认单例)
        /// </summary>
        /// <param name="serviceType">serviceType 运行为 null ,为 null 时候注册implementationType类（父类或接口）</param>
        /// <param name="implementationType">实现类型</param> 
        /// <param name="interceptor">aop切面类</param>
        /// <param name="life">生命周期</param>
        public void RegisterType(Type serviceType, Type implementationType, Type interceptor, ComponentLifeStyle life = ComponentLifeStyle.Transient)
        {
            RegisterType(serviceType, implementationType, string.Empty, interceptor, life);
        }

        /// <summary>
        /// 注册 接口+实现 组件 (默认单例)
        /// </summary>
        /// <param name="serviceType">serviceType 运行为 null ,为 null 时候注册implementationType类（父类或接口）</param>
        /// <param name="implementationType">实现类型</param> 
        /// <param name="keyed"></param>
        /// <param name="interceptor">aop切面类</param>
        /// <param name="life">生命周期</param>
        public void RegisterType(Type serviceType, Type implementationType, object keyed, Type interceptor,
            ComponentLifeStyle life = ComponentLifeStyle.Transient)
        {
            IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registrationBuilder;

            var builder = new ContainerBuilder();
            if (serviceType == null)
                registrationBuilder = builder.RegisterType(implementationType);
            else
            {
                registrationBuilder = keyed == null
                    ? builder.RegisterType(implementationType).As(serviceType)
                    : builder.RegisterType(implementationType).Keyed(keyed, serviceType);
            }
            //if (interceptor != null)
            //{
            //    if (serviceType == null || !serviceType.IsInterface)
            //        registrationBuilder = registrationBuilder.EnableClassInterceptors().InterceptedBy(interceptor);
            //    else
            //        registrationBuilder = registrationBuilder.EnableInterfaceInterceptors().InterceptedBy(interceptor);
            //    //builder.RegisterType(implementationType);
            //}

            if (life == ComponentLifeStyle.Singleton)
                registrationBuilder.SingleInstance();
            builder.Update(_container);
        }

        /// <summary>
        /// 获取实例（泛型类）
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public TService Resolve<TService>() where TService : class
        {
            return _container.Resolve<TService>();
        }

        /// <summary>
        /// 获取实例（泛型类）
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TParameter"></typeparam>
        /// <returns></returns>
        public TService Resolve<TService, TParameter>(TParameter parameter) where TService : class
        {
            return _container.Resolve<TService>(new TypedParameter(typeof(TParameter),parameter));
        }
        /// <summary>
        /// 获取实例（泛型类）
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public TService ResolveKeyed<TService>(object keyed) where TService : class
        {
            return _container.ResolveKeyed<TService>(keyed);
        }
        /// <summary>
        /// 获取实例（object类型）
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object Resolve(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }

        /// <summary>
        /// 获取实例（object类型）
        /// </summary>
        /// <param name="keyed"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object ResolveKeyed(object keyed, Type serviceType)
        {
            return _container.ResolveKeyed(keyed, serviceType);
        }
    }
}

