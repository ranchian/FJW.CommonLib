namespace FJW.SmsService.Bll
{
    /// <summary>
    /// 短信发送状态
    /// </summary>
    public enum MessageStatus
    {
        /// <summary>
        /// 未发送 0
        /// </summary>
        NoSend = 0,

        /// <summary>
        /// 发送成功 1
        /// </summary>
        Success = 1,

        /// <summary>
        /// 失败 2
        /// </summary>
        Failure = 2,

        /// <summary>
        /// 每天条数限制 3
        /// </summary>
        Quota = 3
    }

    /// <summary>
    /// 手机号码当前的状态
    /// </summary>
    public enum PhoneState
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal,

        /// <summary>
        /// 处于间隔时间
        /// </summary>
        Blanking,

        /// <summary>
        /// 达到每日最大发送额度
        /// </summary>
        Quota
    }
}