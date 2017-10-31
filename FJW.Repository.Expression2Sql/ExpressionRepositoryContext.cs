using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FJW.Expression2Sql;

namespace FJW.Repository.Expression2Sql
{
    public partial class ExpressionRepositoryContext : RepositoryContext
    {


        private readonly IList<Sql> _sqls;
        private readonly string _key;

        private readonly IClientProvider _provider;
        //public readonly IClient Client;
        public ExpressionRepositoryContext(string key)
        {
            _sqls = new List<Sql>();
            _key = key;
            _provider = new ClientProviderFactory().CreateProvider(key);
            Client = _provider.CreateClient();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Commit(bool isTranscantion = false)
        {
            if (isTranscantion)
            {
                CommitTranscation();
                return;
            }
            using (var conn = Client.CreateConnection())
            {
                try
                {
                    foreach (var sql in _sqls)
                    {
                        ExecuteSql(sql, conn, null);
                    }
                }
                catch (Exception)
                {
                    ClearRegistrations();
                    throw;
                }
            }
            ClearRegistrations();
        }

        private void CommitTranscation()
        {
            using (var conn = Client.CreateConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var sql in _sqls)
                        {
                            ExecuteSql(sql, conn, tran);
                        }
                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        ClearRegistrations();
                        throw;
                    }
                }
            }
            ClearRegistrations();
        }

        private void ExecuteSql(Sql sql, DbConnection conn, DbTransaction tran)
        {
            var parameter = new DynamicParameters();
            foreach (var it in sql.Parameters)
            {
                parameter.Add(it.Key, it.Value);
            }
            if (!sql.IsIdentity)
            {
                Client.Execute(conn, sql.Text, parameter, tran);
            }
            else
            {
                sql.Entity.Id = Client.ExecuteScalar<int>(conn, sql.Text, parameter, tran);
            }
        }

        public override Task CommitAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override void Rollback()
        {
            Committed = true;
            ClearRegistrations();
        }


        /// <summary>
        /// 清除
        /// </summary>
        /// <remarks>提交后调用</remarks>
        protected override void ClearRegistrations()
        {
            _sqls.Clear();
        }

        public ExpressionToSql<T> CreateSqlExpression<T>()
        {
            return _provider.CreateSqlExpression<T>();
        }

        #region IRepositoryContext Members



        public override void RegisterNew<T>(T obj)
        {
            var exp = CreateSqlExpression<T>().Insert(() => obj);

            var sql = ConvertToSql(exp.Sql, exp.DbParams);
            sql.IsIdentity = exp.IsIdentity;
            if (sql.IsIdentity)
            {
                sql.Text = sql.Text + Client.SelectIdentity();
                sql.Entity = obj;
            }
            _sqls.Add(sql);

            Committed = false;
        }


        public override void RegisterModified<T>(T obj)
        {
            var exp = CreateSqlExpression<T>().Update(() => obj).Where(it => it.Id == obj.Id);
            var sql = ConvertToSql(exp.Sql, exp.DbParams);
            _sqls.Add(sql);
            Committed = false;
        }

        public override void RegisterModified<T>(Expression<Func<T, bool>> predicate, Expression<Func<object>> expression)
        {
            var exp = CreateSqlExpression<T>().Update(expression).Where(predicate);
            var sql = ConvertToSql(exp.Sql, exp.DbParams);
            _sqls.Add(sql);
            Committed = false;
        }

        public override void RegisterDeleted<T>(Expression<Func<T, bool>> predicate, T obj) //where T : BaseEntity , new()
        {
            var exp = CreateSqlExpression<T>().Update(() => new T { IsDelete = 1 }).Where(predicate);
            var sql = ConvertToSql(exp.Sql, exp.DbParams);
            _sqls.Add(sql);
            Committed = false;
        }


        private Sql ConvertToSql(string sqlText, Dictionary<string, object> parameters)
        {
            var sql = new Sql { Text = sqlText, Parameters = parameters };

            return sql;
        }

        #endregion


        class Sql
        {
            public Sql()
            {
                Parameters = new Dictionary<string, object>();
            }

            public bool IsIdentity { get; set; }

            public string Text { get; set; }

            public Dictionary<string, object> Parameters { get; set; }

            public BaseEntity Entity { get; set; }

        }

    }


}
