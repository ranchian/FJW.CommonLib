using System;
using System.Linq.Expressions;

namespace FJW.Repository
{
    /// <summary>
    /// 数据单元
    /// </summary>
    public interface IRepositoryContext: IUnitOfWork, IDisposable
    {

        /// <summary>
        /// 标识
        /// </summary>
        Guid Id { get; }


        IClient Client { get; }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj"></param>
        void RegisterNew<T>(T obj) where T : BaseEntity;

        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        void RegisterModified<T>(T obj) where T : BaseEntity;

        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="onlyFields"></param>
        void RegisterModified<T>(Expression<Func<T, bool>> predicate, Expression<Func<object>> onlyFields) where T : BaseEntity;

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="obj"></param>
        void RegisterDeleted<T>(Expression<Func<T, bool>> predicate, T obj = null) where T : BaseEntity, new();




    }
}
