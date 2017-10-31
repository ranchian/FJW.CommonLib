namespace FJW.CommonLib.Entity
{
    /// <summary>
    /// 公用参数验证类
    /// </summary>
    public class BaseParameter
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long MemberId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public int? DeviceType { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }
    }
}