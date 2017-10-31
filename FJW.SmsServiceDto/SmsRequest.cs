using FJW.CommonLib.Validation;

namespace FJW.SmsServiceDto
{
    /// <summary>
    /// 短信信息
    /// </summary>
    public class SmsRequest
    {
        /// <summary>
        /// 短信ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 是否异步发送
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        /// 调用服务标识
        /// </summary>
        [VerificationEntity(Type = VerificationType.NOT_NULL_OR_EMPTY)]
        public string CallerName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [VerificationEntity(Type = VerificationType.IS_PHONE)]
        public long Phone { get; set; }

        /// <summary>
        /// 短信消息内容
        /// </summary>
        [VerificationEntity(Type = VerificationType.NOT_NULL_OR_EMPTY)]
        public string Message { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}