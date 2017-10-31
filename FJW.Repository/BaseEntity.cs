
using System.ComponentModel.DataAnnotations;
namespace FJW.Repository
{
    public abstract class BaseEntity 
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public virtual long Id { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public virtual int IsDelete { get; set; }
    }
}
