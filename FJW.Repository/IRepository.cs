using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FJW.Repository
{
    /**
    public interface IRepository<T>where T :  BaseEntity, new ()
    {
        /// <summary>
        /// Gets the instance of the repository context on which the repository was attached.
        /// </summary>
        IRepositoryContext Context { get; }
        
        void Add(T entity);

        void Update(T man);

        void UpdateOnly<TKey>(long id, T model, Expression<Func<T, TKey>> onlyFields);

        void UpdateOnly<TKey>(T model, Expression<Func<T, TKey>> onlyFields, Expression<Func<T, bool>> predicate);

        void Delete(Expression<Func<T, bool>> predicate);

        void Delete(long id);



        long Count(Expression<Func<T, bool>> predicate);

        IList<T> Query(Expression<Func<T, bool>> predicate);

        IList<T> Query(Expression<Func<T, bool>> predicate, int skip, int limit, out int rowCount);

        //IList<T> Query(Expressions<T> expressions, int skip, int limit, out int rowCount);

        T Single(Expression<Func<T, bool>> predicate);

        T Single(long id);
    }
   */

    public interface IRepository
    {
        /// <summary>
        /// Gets the instance of the repository context on which the repository was attached.
        /// </summary>
        IRepositoryContext Context { get; }

        void Add<T>(T entity) where T : BaseEntity, new();

        void Update<T>(T man) where T : BaseEntity, new();

        void UpdateOnly<T>( Expression<Func<object>> onlyFields, Expression<Func<T, bool>> predicate) where T : BaseEntity, new();

        void Delete<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity, new();

        void Delete<T>(long id) where T : BaseEntity, new();



        long Count<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity, new();

        IList<T> Query<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity, new();

        IList<TOut> Query<T, TOut>(Expression<Func<T, bool>> expression) where T : BaseEntity, new();

        IList<T> Query<T>(Expression<Func<T, bool>> predicate, int skip, int limit) where T : BaseEntity, new();

        IList<TOut> Query<T, TOut>(Expression<Func<T, bool>> predicate, int skip, int limit) where T : BaseEntity, new();

        IList<T> Query<T>(Expression<Func<T, bool>> predicate, int skip, int limit, out int rowCount) where T : BaseEntity, new();

        //IList<T> Query(Expressions<T> expressions, int skip, int limit, out int rowCount);

        T Single<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity, new();

        TOut Single<T, TOut>(Expression<Func<T, bool>> predicate) where T : BaseEntity, new();

        T Single<T>(long id) where T : BaseEntity, new();
    }
}
