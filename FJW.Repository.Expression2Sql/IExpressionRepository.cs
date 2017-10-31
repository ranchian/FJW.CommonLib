using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FJW.Expression2Sql;

namespace FJW.Repository.Expression2Sql
{
    public interface IExpressionRepository: IRepository
    {
        void Execute<T>(ExpressionToSql<T> expressionToSql) where T : BaseEntity, new();

        TOut ExecuteScalar<T, TOut>(ExpressionToSql<T> expressionToSql) where T : BaseEntity, new();

        IList<T> Query<T>(ExpressionToSql<T> expressionToSql) where T : BaseEntity, new();


        IList<TOut> Query<T, TOut>(ExpressionToSql<T> expression) where T : BaseEntity, new();

       

        IList<T> Query<T>(ExpressionToSql<T> expressionToSql, int skip, int limit) where T : BaseEntity, new();

        IList<T> Query<T>(ExpressionToSql<T> expressionToSql, int skip, int limit, out int rowCount) where T : BaseEntity, new();


        IList<TOut> Query<T, TOut>(ExpressionToSql<T> expressionToSql, int skip, int limit) where T : BaseEntity, new();


        IList<TOut> Query<T, TOut>(ExpressionToSql<T> expressionToSql, int skip, int limit, out int rowCount) where T : BaseEntity, new();

        ExpressionToSql<T> CreateSqlExpression<T>();
    }
}
