using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using FJW.Expression2Sql;

namespace FJW.Repository.Expression2Sql
{
    public class ExpressionRepository : IExpressionRepository
    {
        private readonly ExpressionRepositoryContext _context;
        public ExpressionRepository(string key)
        {
            _context = new ExpressionRepositoryContext(key);
            Context = _context;
        }
        public IRepositoryContext Context
        {
            get; protected set;
        }


        #region Repository Members
        public void Add<T>(T entity) where T : BaseEntity, new()
        {

            entity.IsDelete = 0;
            Context.RegisterNew(entity);
        }



        public void Delete<T>(long id) where T : BaseEntity, new()
        {
            Delete<T>(it => it.Id == id);
        }


        public void Update<T>(T man) where T : BaseEntity, new()
        {
            Context.RegisterModified(man);
        }

        public void UpdateOnly<T>(Expression<Func<object>> onlyFields, Expression<Func<T, bool>> predicate) where T : BaseEntity, new()
        {
            Context.RegisterModified(predicate, onlyFields);
        }


        public void Delete<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity, new()
        {
            Context.RegisterDeleted(predicate);
        }
        
        public long Count<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity, new()
        {
            var exp = CreateSqlExpression<T>().Count(it => it.Id).Where(predicate);
            DynamicParameters parameters = null;
            if (exp.DbParams.Count > 0)
            {
                parameters = new DynamicParameters();
                foreach (var it in exp.DbParams)
                {
                    parameters.Add(it.Key, it.Value);
                }
            }

            using (var conn = _context.Client.CreateConnection())
            {
                return conn.ExecuteScalar<long>(exp.Sql, parameters);
            }
        
        }

        public IList<T> Query<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity, new()
        {
            var exp = _context.CreateSqlExpression<T>().Select<T>().Where(predicate);
            DynamicParameters parameters = null;
            if (exp.DbParams.Count > 0)
            {
                parameters = new DynamicParameters();
                foreach (var it in exp.DbParams)
                {
                    parameters.Add(it.Key, it.Value);
                }
            }

            using (var conn = _context.Client.CreateConnection())
            {
                return conn.Query<T>(exp.Sql, parameters).ToList();
            }
        }

        public IList<T> Query<T>(Expression<Func<T, bool>> predicate, int skip, int limit) where T : BaseEntity, new() 
        {
            return Query<T, T>(predicate, skip, limit);
        }

        public IList<TOut> Query<T, TOut>(Expression<Func<T, bool>> predicate, int skip, int limit) where T : BaseEntity, new()
        {
            var exp = _context.CreateSqlExpression<T>().Select().Where(predicate);
            return  Query<T, TOut>(exp, skip, limit);
        }

        public IList<T> Query<T>(Expression<Func<T, bool>> predicate, int skip, int limit, out int rowCount) where T : BaseEntity, new()
        {
            var exp = CreateSqlExpression<T>().Select().Where(predicate);
            return Query(exp, skip, limit, out rowCount);
        }

        public T Single<T>(long id) where T : BaseEntity, new()
        {
            return Single<T>(it => it.Id == id);
        }

        public T Single<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity, new()
        {
            return Single<T, T>(predicate);
        }

        public TOut Single<T, TOut>(Expression<Func<T, bool>> predicate) where T : BaseEntity, new()
        {
            var exp = CreateSqlExpression<T>().Select<T>().Where(predicate);
            DynamicParameters parameters = null;
            if (exp.DbParams.Count > 0)
            {
                parameters = new DynamicParameters();
                foreach (var it in exp.DbParams)
                {
                    parameters.Add(it.Key, it.Value);
                }
            }
            var sqlTxt = exp.Sql;
            var sql = string.Format("select top 1 {0}", sqlTxt.Substring(6, sqlTxt.Length - 6));
            using (var conn = _context.Client.CreateConnection())
            {
                return conn.Query<TOut>(sql, parameters).FirstOrDefault();
            }
        }


        #endregion


        #region IExpressionRepository Members

        public void Execute<T>(ExpressionToSql<T> expressionToSql) where T : BaseEntity, new()
        {
            var parameter = new DynamicParameters();
            foreach (var it in expressionToSql.DbParams)
            {
                parameter.Add(it.Key, it.Value);
            }
            var sqlTxt = expressionToSql.Sql;
            using (var conn = Context.Client.CreateConnection())
            {
                Context.Client.Execute(conn, sqlTxt, parameter);
            }
        }

        public TOut ExecuteScalar<T, TOut>(ExpressionToSql<T> expressionToSql) where T : BaseEntity, new()
        {
            var parameter = new DynamicParameters();
            foreach (var it in expressionToSql.DbParams)
            {
                parameter.Add(it.Key, it.Value);
            }
            var sqlTxt = expressionToSql.Sql;
            using (var conn = Context.Client.CreateConnection())
            {
               return Context.Client.ExecuteScalar<TOut>(conn, sqlTxt, parameter);
            }
        }

        public IList<T> Query<T>(ExpressionToSql<T> expressionToSql) where T : BaseEntity, new()
        {
            return Query<T, T>(expressionToSql);
        }

  
        public IList<TOut> Query<T, TOut>(Expression<Func<T, bool>> expression) where T : BaseEntity, new()
        {
            var exp = _context.CreateSqlExpression<T>().Select().Where(expression);
            return Query<T, TOut>(exp);
        }

       

        public IList<TOut> Query<T, TOut>(ExpressionToSql<T> expressionToSql) where T : BaseEntity, new()
        {
            var parameter = new DynamicParameters();
            foreach (var it in expressionToSql.DbParams)
            {
                parameter.Add(it.Key, it.Value);
            }
            var sqlTxt = expressionToSql.Sql;
            using (var conn = Context.Client.CreateConnection())
            {
                return Context.Client.Query<TOut>(conn, sqlTxt, parameter).ToList();
            }
        }

        

        public IList<T> Query<T>(ExpressionToSql<T> expressionToSql, int skip, int limit)
            where T : BaseEntity, new()
        {
            return Query<T, T>(expressionToSql, skip, limit);
        }

        public IList<T> Query<T>(ExpressionToSql<T> expressionToSql, int skip, int limit, out int rowCount)
            where T : BaseEntity, new()
        {
            return Query<T, T>(expressionToSql, skip, limit, out rowCount);
        }

        public IList<TOut> Query<T, TOut>(ExpressionToSql<T> expressionToSql, int skip, int limit) where T : BaseEntity, new()
        {
            var parameter = new DynamicParameters();

            if (string.IsNullOrEmpty(expressionToSql.OrderByText))
            {
                expressionToSql.OrderBy(it => it.Id);
            }
            var sql = expressionToSql.Sql.Substring(0, expressionToSql.Sql.Length - 1);
            var sqlTxt = string.Format("{0} OFFSET @Skip ROWS FETCH NEXT @Limit ROWS ONLY", sql);
            foreach (var it in expressionToSql.DbParams)
            {
                parameter.Add(it.Key, it.Value);
            }
            parameter.Add("@Skip", skip);
            parameter.Add("@Limit", limit);
            using (var conn = Context.Client.CreateConnection())
            {
                return Context.Client.Query<TOut>(conn, sqlTxt, parameter).ToList();
            }
        }



        public IList<TOut> Query<T, TOut>(ExpressionToSql<T> expressionToSql, int skip, int limit, out int rowCount) where T : BaseEntity, new()
        {
            //rowCount = 0;
            var parameter = new DynamicParameters();

            if (string.IsNullOrEmpty(expressionToSql.OrderByText))
            {
                expressionToSql.OrderBy(it => it.Id);
            }

            var sql = expressionToSql.Sql.Substring(0, expressionToSql.Sql.Length - 1);
            if (expressionToSql.OrderByText != null)
            {
                sql = sql.Replace(expressionToSql.OrderByText, "");
            }
            var i = sql.IndexOf("from", StringComparison.Ordinal);
            var sqlTxt = string.Format(@"select * from (
select ROW_NUMBER() OVER( {2} ) _RN,
{0}
) T1 where T1._RN > @RowBegin and T1._RN < @RowEnd;
select COUNT(1) {1}",
sql.Substring(6, sql.Length - 6), sql.Substring(i, sql.Length - i), expressionToSql.OrderByText ?? "order by a.Id");

            foreach (var it in expressionToSql.DbParams)
            {
                parameter.Add(it.Key, it.Value);
            }
            parameter.Add("@RowBegin", skip);
            parameter.Add("@RowEnd", skip + limit + 1);
            using (var conn = Context.Client.CreateConnection())
            using (var reader = Context.Client.DynamicReader(conn, sqlTxt, parameter))
            {
                var results = reader.Read<TOut>().ToList();
                rowCount = reader.Read<int>().Single();
                return results;
            }
        }

        public ExpressionToSql<T> CreateSqlExpression<T>()
        {
            return _context.CreateSqlExpression<T>();
        }

        #endregion


        
    }
}
