namespace FJW.DI
{
    public class ServerLocation
    {
        //private static readonly ContainerBuilder Builder;

        //private static IContainer _container;
        //private static readonly object LockObj = new object();
        //static ServerLocation()
        //{
        //    Builder = new ContainerBuilder();
        //    Builder.RegisterModule(new ConfigurationSettingsReader("autofac"));   
        //}

       

        //public static IContainer GetContainer()
        //{
        //    if (_container == null)
        //    {
        //        lock (LockObj)
        //        {
        //            if (_container == null)
        //            {
        //                _container = Builder.Build();
        //            }
        //        }
        //    }
        //    return _container;
        //}

        public static T Resolve<T>() where T : class
        {
            //return GetContainer().Resolve<T>();
            return ObjectContainer.Resolve<T>();
        }

        //public static void Register<T, TBase>() where T : TBase
        //{
        //    Builder.RegisterType<T>().As<TBase>();
        //}

        //public static void Register<T, TBase>(string serverfName) where T : TBase
        //{
        //    Builder.RegisterType<T>().As<TBase>().Named<T>(serverfName);
        //}

        ///// <summary>
        ///// 注册为单件模式
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="TBase"></typeparam>
        //public static void RegisterSingleInstance<T, TBase>() where T : TBase
        //{
        //    Builder.RegisterType<T>().As<TBase>().SingleInstance();
        //}
    }
}
