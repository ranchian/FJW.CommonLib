using System;


namespace FJW.Repository
{
    /// <summary>
    /// 表实体
    /// </summary>
    public abstract class TableEntity :BaseEntity
    {
        public virtual int CreateBy { get; set; }


        public virtual DateTime? CreateTime { get; set; }


        public virtual int LastUpdateBy { get; set; }


        public virtual DateTime? LastUpdateTime { get; set; }
    }
}
