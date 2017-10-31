

using FJW.Expression2Sql;
using FJW.Expression2Sql.DbSqlParser;

namespace FJW.Repository.Expression2Sql
{
    public  class ClientProvider : IClientProvider
    {

        public string ConnectionString { get; set; }


        public  IClient CreateClient( )
		{
		    //var conConfig = ConfigurationManager.ConnectionStrings[key];
            return new SqlServerClient { ConnectionString = this.ConnectionString };            
		}

        public ExpressionToSql<T> CreateSqlExpression<T>()
        {
            return new ExpressionToSql<T>(new SQLServerSqlParser());
        }
    }
}
