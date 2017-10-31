using System;
using System.Reflection;

using FJW.CommonLib.ExtensionMethod;

namespace FJW.CommonLib.Validation
{
    public class DetectionRuleHelper
    {
        #region 检测是否符合规则 DetectionRule
        /// <summary>
        /// 检测是否符合规则
        /// </summary>
        /// <returns></returns>
        public static bool DetectionRule(VerificationType type, object columnValue)
        {
            switch (type)
            {
                case VerificationType.NOT_NULL_OR_EMPTY:
                    return (columnValue != null && !string.IsNullOrEmpty(columnValue.ToString()));
                case VerificationType.NOT_ZERO:
                    return columnValue.ToInt(0) != 0;
                case VerificationType.NOT_EMPTY_OR_ZERO:
                    return (columnValue != null && !string.IsNullOrEmpty(columnValue.ToString()) && columnValue.ToInt(0) != 0);
                case VerificationType.IS_PHONE:
                    return ValidateHelper.IsCellPhone(columnValue.ToString());
                case VerificationType.EMPTY_OR_IS_PHONE:
                    return (columnValue == null || string.IsNullOrEmpty(columnValue.ToString()) || ValidateHelper.IsCellPhone(columnValue.ToString()));
                case VerificationType.IS_ID_CARD:
                    return ValidateHelper.IsIDC(columnValue.ToString());
                case VerificationType.IS_EMAIL:
                    return ValidateHelper.IsValidEmail(columnValue.ToString());
                case VerificationType.IS_UINT:
                    return ValidateHelper.IsValidUInt(columnValue.ToString());
                case VerificationType.IS_LOGIN_PASSWORD:
                    return ValidateHelper.IsLoginPwd(columnValue.ToString());
                case VerificationType.IS_TRADING_PASSWORD:
                    return ValidateHelper.IsTreadingPwd(columnValue.ToString());
                case VerificationType.IS_CHINESE_NAME:
                    return ValidateHelper.IsChineseName(columnValue.ToString());
                default:
                    return true;
            }
        }
        #endregion

        #region 根据属性类型默认赋值 AssignmentDefaultByType
        /// <summary>
        /// 根据属性类型默认赋值
        /// </summary>
        /// <param name="columnType"></param>
        /// <returns></returns>
        public static object AssignmentDefaultByType(string columnType)
        {
            switch (columnType)
            {
                case "string":
                case "guid":
                case "char":
                    return string.Empty;
                case "int32":
                case "uint32":
                case "int16":
                case "uint16":
                case "int64":
                case "uint64":
                case "single":
                case "double":
                case "decimal":
                case "byte":
                case "sbyte":
                case "boolean":
                    return 0;
                case "datetime":
                    return DateTime.MinValue;
                default:
                    return null;
            }
        }
        #endregion

        #region 设置默认值 SetDefaultValue
        /// <summary>
        /// 设置默认值
        /// </summary>
        public static object SetDefaultValue(PropertyInfo propInfo)
        {
            //获取当前属性的特性
            object[] objAttrs = propInfo.GetCustomAttributes(typeof(SetDefaultValueAttribute), true);
            if (objAttrs.Length > 0)
            {
                SetDefaultValueAttribute attr = objAttrs[0] as SetDefaultValueAttribute;
                if (attr != null)
                {
                    return attr.DefaultValue;
                }
            }
            return null;
        }
        #endregion

        #region 根据属性类型赋值 AssignmentByType
        /// <summary>
        /// 根据属性类型赋值
        /// </summary>
        /// <param name="columnType"></param>
        /// <returns></returns>
        public static object AssignmentByType(string columnType, string columnValue)
        {
            switch (columnType)
            {
                case "string":
                    return columnValue;
                case "guid":
                    return Guid.Parse(columnValue);
                case "char":
                    return char.Parse(columnValue);
                case "int32":
                    int r = 0;
                    int.TryParse(columnValue, out r);
                    return r;
                case "uint32":
                    return UInt32.Parse(columnValue);
                case "int16":
                    return Int32.Parse(columnValue);
                case "uint16":
                    return UInt16.Parse(columnValue);
                case "int64":
                    return Int64.Parse(columnValue);
                case "uint64":
                    return UInt64.Parse(columnValue);
                case "single":
                    return Single.Parse(columnValue);
                case "double":
                    return Double.Parse(columnValue);
                case "decimal":
                    return Convert.ToDecimal(columnValue);
                case "byte":
                    return byte.Parse(columnValue);
                case "sbyte":
                    return byte.Parse(columnValue);
                case "boolean":
                    return bool.Parse(columnValue);
                case "datetime":
                    return Convert.ToDateTime(columnValue);
                default:
                    return null;
            }
        }
        #endregion
    }
}
