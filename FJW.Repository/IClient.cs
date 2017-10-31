
using System.Data.Common;

namespace FJW.Repository
{
    /// <summary>
    /// 数据库端
    /// </summary>
	public interface IClient : IDbExecutor
    {
	    string ConnectionString { get; set; }

	    DbConnection CreateConnection();

        string SelectIdentity();

    }
}

