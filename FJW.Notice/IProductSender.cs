namespace FJW.Notice
{
    /// <summary>
    /// 产品发信器
    /// </summary>
    public interface IProductSender : ISender
    {
        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="productId"></param>
        void Add(long productId);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="productId"></param>
        void Modify(long productId);

        /// <summary>
        /// 售完
        /// </summary>
        /// <param name="productId"></param>
        void SoldOut(long productId);


    }
}
