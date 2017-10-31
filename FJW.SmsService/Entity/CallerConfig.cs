using System.Collections.Generic;

using FJW.CommonLib.Configuration;

namespace FJW.SmsService.Entity
{
    #region 调用方与短信通道映射关系配置

    /// <summary>
    /// 映射关系配置
    /// </summary>
    public class CallerNode
    {
        /// <summary>
        /// 服务标识
        /// </summary>
        [Node]
        public string CallerName { get; set; }
        /// <summary>
        /// 通道标识（邮件服务是SMTP标识，短信服务是短信通道标识）
        /// </summary>
        [Node]
        public string Channel { get; set; }
    }

    public class CallerConfig
    {
        [Node("CallerList/Caller", NodeAttribute.NodeType.List)]
        public List<CallerNode> CallerList { get; set; }
    }
    #endregion
}