using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FJW.CommonLib.Validation
{
    public class PropertyMap
    {
        private readonly List<KeyValuePair<Type, object>> _propCustomerAttrs = new List<KeyValuePair<Type, object>>();
        public PropertyMap(PropertyInfo property)
        { 
            Property = property;
             
            foreach (var attr in property.GetCustomAttributes(true))
            {
                _propCustomerAttrs.Add(new KeyValuePair<Type, object>(attr.GetType(), attr));
            }
        }
        /// <summary>
        /// 活期属性的attr标签
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAttribute<T>() where T:Attribute
        {
            var attr = _propCustomerAttrs.FirstOrDefault(m => m.Key == typeof(T));
             
            return (T)attr.Value; 
        }
        /// <summary>
        /// 
        /// </summary>
        private FastPropertySetHandler _propertySetHandler;
        /// <summary>
        /// 获取属性值的委托
        /// </summary>
        public FastPropertySetHandler PropertySetHandler
        {
            get
            {
                if (_propertySetHandler != null)
                    return _propertySetHandler;

                _propertySetHandler = DynamicCalls.GetPropertySetter(Property);
                return _propertySetHandler;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private FastPropertyGetHandler _propertyGetHandler;
        /// <summary>
        /// 设置属性值的委托
        /// </summary>
        public FastPropertyGetHandler PropertyGetHandler
        {
            get
            {
                if (_propertyGetHandler != null)
                    return _propertyGetHandler;

                _propertyGetHandler = DynamicCalls.GetPropertyGetter(Property);
                return _propertyGetHandler;
            }
        }
        ///// <summary>
        ///// 属性名称
        ///// </summary>
        //public string PropertyName { get; private set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public PropertyInfo Property { get; private set; }
    }
}
