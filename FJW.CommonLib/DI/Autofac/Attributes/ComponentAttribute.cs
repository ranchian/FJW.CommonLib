using System;

namespace FJW.DI
{
    /// <summary>
    /// 依赖注入
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        /// <summary>
        /// Register Named
        /// </summary>
        public object  ComponentKey { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ComponentLifeStyle LifeStyle { get; private set; }
        /// <summary>
        /// 注册拦截类
        /// </summary>
        public Type Interceptor { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public ComponentAttribute() : this(null, null, ComponentLifeStyle.Transient)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyed"></param>
        public ComponentAttribute(object keyed) : this(keyed, null, ComponentLifeStyle.Transient)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lifeStyle"></param>
        public ComponentAttribute(ComponentLifeStyle lifeStyle) : this(null, null, lifeStyle)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interceptor"></param>
        public ComponentAttribute(Type interceptor) : this(null, interceptor, ComponentLifeStyle.Transient)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyed"></param>
        /// <param name="lifeStyle"></param>
        public ComponentAttribute(object keyed, ComponentLifeStyle lifeStyle)
            : this(keyed, null, lifeStyle)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyed"></param>
        /// <param name="interceptor"></param>
        public ComponentAttribute(object keyed, Type interceptor)
            : this(keyed, interceptor, ComponentLifeStyle.Transient)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyed"></param>
        /// <param name="interceptor"></param>
        /// <param name="lifeStyle"></param>
        public ComponentAttribute(object keyed, Type interceptor, ComponentLifeStyle lifeStyle)
        {
            ComponentKey = keyed;
            Interceptor = interceptor;
            LifeStyle = lifeStyle;
        }
    }
}
