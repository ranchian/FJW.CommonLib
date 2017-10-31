using System;

using System.ComponentModel.DataAnnotations.Schema;

using FJW.Repository;

namespace Test.Repository.ConsoleApp
{
    [Table("TC_MemberAccount")]
    public class MemberAccountEntity : TableEntity
    {
        #region 构造函数
        /// <summary>
        /// 构造函数 
        /// </summary>
        public MemberAccountEntity()
        {
            MemberId = 0;
            TotalProductPrice = 0;
            TotalIncome = 0;
            NotDueIncome = 0;
            YesterdayIncome = 0;
            AccountBalance = 0;
        }
        #endregion

        #region 属性集


        /// <summary>
        /// 会员ID
        /// </summary>
        public Int64 MemberId { get; set; }

        /// <summary>
        /// 产品总金额
        /// </summary>
        public Decimal TotalProductPrice { get; set; }

        /// <summary>
        /// 总到期收益
        /// </summary>
        public Decimal TotalIncome { get; set; }

        /// <summary>
        /// 未到期收益
        /// </summary>
        public Decimal NotDueIncome { get; set; }

        /// <summary>
        /// 昨日收益
        /// </summary>
        public Decimal YesterdayIncome { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public Decimal AccountBalance { get; set; }

        public string TradingPassword { get; set; }

        #endregion

    }
}
