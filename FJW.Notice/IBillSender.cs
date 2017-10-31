namespace FJW.Notice
{
    public interface IBillSender : ISender
    {
        void Build(long id, BillType type);
    }

    /// <summary>
    /// 账单类型
    /// </summary>
    public enum BillType
    {
        /// <summary>
        /// 购买
        /// </summary>
        Buy = 1,

        /// <summary>
        /// 赎回
        /// </summary>
        Redeem,

        /// <summary>
        /// 收益
        /// </summary>
        Interest,

        /// <summary>
        /// 充值
        /// </summary>
        Recharge,

        /// <summary>
        /// 提现
        /// </summary>
        Withdraw
    }
}
