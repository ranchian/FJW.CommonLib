using FJW.Expression2Sql;

namespace FJW.Repository.Expression2Sql
{
    public interface IClientProvider
    {
        IClient CreateClient();
        string ConnectionString { get; set; }

        ExpressionToSql<T> CreateSqlExpression<T>();
    }
}
