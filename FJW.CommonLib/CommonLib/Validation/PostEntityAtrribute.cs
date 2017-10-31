using System;
using System.ComponentModel;

namespace FJW.CommonLib.Validation
{
    /// <summary>
    /// 验证枚举
    /// </summary>
    public enum VerificationType
    {
        /// <summary>
        /// 不能为空
        /// </summary>
        [Description("不能为空")]
        NOT_NULL_OR_EMPTY,

        /// <summary>
        /// 不能为0
        /// </summary>
        [Description("不能为0")]
        NOT_ZERO,

        /// <summary>
        /// 不能为空或为0
        /// </summary>
        [Description("不能为空或为0")]
        NOT_EMPTY_OR_ZERO,

        /// <summary>
        /// 为手机号码格式
        /// </summary>
        [Description("为手机号码格式")]
        IS_PHONE,

        /// <summary>
        /// 为手机号码格式
        /// </summary>
        [Description("为手机号码格式")]
        EMPTY_OR_IS_PHONE,

        /// <summary>
        /// 为邮件格式
        /// </summary>
        [Description("为邮件格式")]
        IS_EMAIL,

        /// <summary>
        /// 为身份证格式
        /// </summary>
        [Description("为身份证格式")]
        IS_ID_CARD,

        /// <summary>
        /// 为正整数
        /// </summary>
        [Description("为正整数")]
        IS_UINT,

        /// <summary>
        /// 中文名验证
        /// </summary>
        [Description("为中文姓名")]
        IS_CHINESE_NAME,

        /// <summary>
        /// 登录密码
        /// </summary>
        [Description("登录密码(6到16位字符)")]
        IS_LOGIN_PASSWORD,

        /// <summary>
        /// 登录密码
        /// </summary>
        [Description("交易密码(6位数字)")]
        IS_TRADING_PASSWORD
    }

    /// <summary>
    /// 验证实体的特性
    /// </summary>
    public class VerificationEntityAttribute : Attribute
    {
        /// <summary>
        /// 验证的属性
        /// </summary>
        public VerificationType Type;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage;
    }

    /// <summary>
    /// 设置默认值的特性(暂时只支持string类型)
    /// </summary>
    public class SetDefaultValueAttribute : Attribute
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue;
    }
}
