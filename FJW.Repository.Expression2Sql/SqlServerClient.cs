using System.Data.Common;
using System.Data.SqlClient;

namespace FJW.Repository.Expression2Sql
{
    public class SqlServerClient : DbExecutor, IClient
    {

        public string ConnectionString { get; set; }

        public DbConnection CreateConnection()
        {
           return  new SqlConnection(ConnectionString);
        }

        public string SelectIdentity()
        {
            return "select SCOPE_IDENTITY();";
        }
    }
}
