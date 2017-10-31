
using System.ComponentModel.DataAnnotations.Schema;
using FJW.Repository;

namespace Test.Repository.ConsoleApp
{
    [Table("Address")]
    public class AddressEntity :BaseEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [Column("ContactName")]
        public string Name { get; set; }
        

        public string Address { get; set; }

        public string PostCode { get; set; }
    }
}
