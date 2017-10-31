using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using FJW.CommonLib.ExtensionMethod;

namespace FJW.CommonLib.Validation
{
     
    /// <summary>
    /// 信息验证相关帮助类
    /// </summary>
    public class ValidateHelperNew
    {
        private static readonly ConcurrentDictionary<Type, List<PropertyMap>> CacheDic =
            new ConcurrentDictionary<Type, List<PropertyMap>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static List<PropertyMap> GetPropMaps(Type type)
        {
            List<PropertyMap> propertyMaps;

            if (CacheDic.TryGetValue(type, out propertyMaps))
                return propertyMaps;

            propertyMaps = type.GetProperties().Select(prop => new PropertyMap(prop)).ToList();

            CacheDic.TryAdd(type, propertyMaps);//加入缓存

            return propertyMaps;
        }

        /// <summary>
        /// 按照实体对象特性，验证实体对象，并按照特性给实体属性赋值默认值
        /// </summary>
        public static bool CheckEntity<T>(ref T t, out string errMsg) where T : class,new()
        {
            errMsg = string.Empty;

            foreach (var propertyMap in GetPropMaps(typeof(T)))
            {
                var value = propertyMap.PropertyGetHandler(t);//获取列名的值
                if (value == null)//如果没有传递该参数
                {
                    var attr = propertyMap.GetAttribute<SetDefaultValueAttribute>();
                    if (attr != null && string.IsNullOrEmpty(attr.DefaultValue))
                    {
                        value = attr.DefaultValue;
                        propertyMap.PropertySetHandler(t, value);
                    }
                }
               
                errMsg = ParameterCalibration(propertyMap, value); //验证是否符合规则
                if (!string.IsNullOrWhiteSpace(errMsg))
                    return false;
            }
            return true;
        } 

        #region 参数校验 ParameterCalibration
        /// <summary>
        /// 参数校验
        /// </summary>
        private static string ParameterCalibration(PropertyMap propertyMap, object value)
        {
            var verAttr = propertyMap.GetAttribute<VerificationEntityAttribute>(); //获取当前属性的特性
            if (verAttr == null) return string.Empty; 
            
            var rst = DetectionRuleHelper.DetectionRule(verAttr.Type, value);//检测该属性是否符合规则

            if (rst) return string.Empty;

            var paramName = propertyMap.Property.Name;
            var descrAttrArr = propertyMap.GetAttribute<DescriptionAttribute>();
            if (descrAttrArr != null)
            { 
                paramName = descrAttrArr.Description;
            }
            return !string.IsNullOrEmpty(verAttr.ErrorMessage)
                ? paramName + verAttr.ErrorMessage
                : paramName + verAttr.Type.GetDescription(); 
        }
        #endregion 
    }
}