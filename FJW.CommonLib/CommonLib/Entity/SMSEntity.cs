namespace FJW.CommonLib.Entity
{
    /// <summary>
    /// 短信信息
    /// </summary>
    public class SMSEntity
    {
        /// <summary>
        /// 调用服务标识
        /// </summary>
        public string CallerName { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }
        /// <summary>
        /// 短信消息内容
        /// </summary>
        public string Msg { get; set; }
    }
}
