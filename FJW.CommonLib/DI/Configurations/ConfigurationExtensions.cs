using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;

namespace FJW.DI
{
    /// <summary> 
    /// 
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary> 
        /// </summary>
        /// <returns></returns>
        public static Configuration UseAutofac(this Configuration configuration, string configurationSectionName = "")
        {
            ObjectContainer.SetContainer(new AutofacObjectContainer(new ContainerBuilder()));
            return configuration; 
        }

        public static Configuration RegisterComponents(this Configuration configuration, Assembly assembly)
        {
           return RegisterComponents(configuration, new List<Assembly> {assembly});
        }
        /// <summary>
        /// ioc 容器自动注册
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static Configuration RegisterComponents(this Configuration configuration,List<Assembly> assemblies)
        {
            try
            {
                foreach (var assembly in assemblies)
                {
                    foreach (var type in assembly.GetTypes().Where(IsComponent))
                    {
                        var component = ParseComponent(type); 
                        var life = component.LifeStyle;
                        ObjectContainer.RegisterType(type,component.Interceptor,life);
                        foreach (var interfaceType in type.GetInterfaces())
                        {
                            ObjectContainer.RegisterType(interfaceType, type, component.ComponentKey, component.Interceptor, life);
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder(ex.StackTrace);
                ex.LoaderExceptions.ToList().ForEach(m => sb.AppendLine(m.Message));
                throw new Exception(sb.ToString());
            }
            return configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsComponent(Type type)
        {
            return type.IsClass && !type.IsAbstract && type.GetCustomAttributes(typeof(ComponentAttribute), false).Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ComponentAttribute ParseComponent(Type type)
        {
            return (ComponentAttribute)type.GetCustomAttributes(typeof(ComponentAttribute), false)[0];
        }
    }
}