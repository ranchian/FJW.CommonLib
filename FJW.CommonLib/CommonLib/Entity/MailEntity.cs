namespace FJW.CommonLib.Entity
{
    /// <summary>
    /// 邮件信息
    /// </summary>
    public class MailEntity
    {
        /// <summary>
        /// 调用服务标识
        /// </summary>
        public string CallerName { get; set; }
        /// <summary>
        /// 发件人地址
        /// </summary>
        public string MailFrom { get; set; }
        /// <summary>
        /// 发件人别名
        /// </summary>
        public string FromDisplayName { get; set; }
        /// <summary>
        /// 收件人地址(多收件人以半角分号;隔开)
        /// </summary>
        public string MailTo { get; set; }
        /// <summary>
        /// 收件人别名
        /// </summary>
        public string ToDisplayName { get; set; }
        /// <summary>
        /// 抄送人地址
        /// </summary>
        public string MailCC { get; set; }
        /// <summary>
        /// 抄送人别名
        /// </summary>
        public string CCDisplayName { get; set; }
        /// <summary>
        /// 密件抄送人地址
        /// </summary>
        public string MailBCC { get; set; }
        /// <summary>
        /// 密件抄送人别名
        /// </summary>
        public string BCCDisplayName { get; set; }
        /// <summary>
        /// 附件地址（已半角分号;隔开）
        /// </summary>
        public string Attachments { get; set; }
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 邮件正文
        /// </summary>
        public string MailBody { get; set; }
    }
}
