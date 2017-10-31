using System.Collections.Generic;
using Newtonsoft.Json;

namespace FJW.CommonLib.Entity
{
    /// <summary>
    /// 公用分页返回实体
    /// </summary>
    public class BasePageResponse<T> : BaseResponse
    {
        /// <summary>
        /// 数据集合
        /// </summary>
        [JsonProperty("grid")]
        public IList<T> Grid { get; set; }

        /// <summary>
        /// 每页行数
        /// </summary>
        [JsonProperty("rows")]
        public int Rows { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        [JsonProperty("records")]
        public int Records { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        [JsonProperty("total")]
        public int Total
        {
            get
            {
                if (Records > 0)
                {
                    return Records % Rows == 0 ? Records / Rows : Records / Rows + 1;
                }
                return 1;
            }
        }
    }
}