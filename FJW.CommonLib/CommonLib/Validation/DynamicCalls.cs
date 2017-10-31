using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FJW.CommonLib.Validation
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="parameter"></param>
    public delegate void FastPropertySetHandler(object target, object parameter);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="target"></param>
   /// <returns></returns>
    public delegate object FastPropertyGetHandler(object target);
    /// <summary>
    /// 
    /// </summary>
    public static class DynamicCalls
    {
        public static FastPropertyGetHandler GetPropertyGetter(PropertyInfo propInfo)
        {
            if (propInfo.DeclaringType == null)
                return null; 
            var dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object) }, propInfo.DeclaringType.Module);

            var ilGenerator = dynamicMethod.GetILGenerator(); 
            ilGenerator.Emit(OpCodes.Ldarg_0); 
            ilGenerator.EmitCall(OpCodes.Callvirt, propInfo.GetGetMethod(), null); 

            EmitBoxIfNeeded(ilGenerator, propInfo.PropertyType); 
            ilGenerator.Emit(OpCodes.Ret);
             
            return (FastPropertyGetHandler)dynamicMethod.CreateDelegate(typeof(FastPropertyGetHandler)); 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        public static FastPropertySetHandler GetPropertySetter(PropertyInfo propInfo)
        {
            var module = propInfo.DeclaringType != null ? propInfo.DeclaringType.Module : typeof(object).Module;

            var dynamicMethod = new DynamicMethod(string.Empty, null, new[] { typeof(object), typeof(object) }, module);

            var ilGenerator = dynamicMethod.GetILGenerator(); 
            ilGenerator.Emit(OpCodes.Ldarg_0); 
            ilGenerator.Emit(OpCodes.Ldarg_1); 
            EmitCastToReference(ilGenerator, propInfo.PropertyType);

            var setterMethod = propInfo.GetSetMethod() ?? propInfo.GetSetMethod(true); 
            ilGenerator.EmitCall(OpCodes.Callvirt, setterMethod, null); 
            ilGenerator.Emit(OpCodes.Ret);
            
            return (FastPropertySetHandler)dynamicMethod.CreateDelegate(typeof(FastPropertySetHandler)); 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="type"></param>
        private static void EmitCastToReference(ILGenerator ilGenerator, Type type)
        {
            ilGenerator.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="type"></param>
        private static void EmitBoxIfNeeded(ILGenerator ilGenerator, System.Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, type);
            }
        }
    }
}
