using System.Collections.Generic;
using FJW.CommonLib.Configuration;

namespace FJW.SmsService.Entity
{
    #region 短信通道列表
    /// <summary>
    /// 短信通道列表
    /// </summary>
    public class ChannelNode
    {
        /// <summary>
        /// 通道标识
        /// </summary>
        [Node]
        public string Name { get; set; }
        /// <summary>
        /// 通道URL
        /// </summary>
        [Node]
        public string Url { get; set; }
        /// <summary>
        /// 通道参数字符串,以|分隔（例："account=XXX|Pwd=XXX|mobile=@Phone|msg=@Msg|needstatus=true|extno="）
        /// </summary>
        [Node]
        public string Param { get; set; }
    }

    /// <summary>
    /// SMTP列表
    /// </summary>
    public class SmsConfig
    {
        [Node("ChannelList/Channnel", NodeAttribute.NodeType.List)]
        public List<ChannelNode> ChannelList { get; set; }
    }
    #endregion
}