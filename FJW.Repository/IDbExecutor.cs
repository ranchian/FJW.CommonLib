using System.Collections.Generic;
using System.Data;

namespace FJW.Repository
{
    public interface IDbExecutor
    {
        IEnumerable<T> Query<T>(IDbConnection conn, string sqlTxt, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text);


        IDynamicReader DynamicReader(IDbConnection conn, string sqlTxt, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text);


        void Execute(IDbConnection conn, string sqlTxt, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text);

        T ExecuteScalar<T>(IDbConnection conn, string sqlTxt, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text);

        IDataReader ExecuteReader(IDbConnection conn, string sqlTxt, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text);


        IEnumerable<T> Query<T>(IDbConnection conn, string sqlTxt, IParameters param , IDbTransaction tran = null, CommandType type = CommandType.Text);


        IDynamicReader DynamicReader(IDbConnection conn, string sqlTxt, IParameters param , IDbTransaction tran = null, CommandType type = CommandType.Text);


        void Execute(IDbConnection conn, string sqlTxt, IParameters param , IDbTransaction tran = null, CommandType type = CommandType.Text);

        T ExecuteScalar<T>(IDbConnection conn, string sqlTxt, IParameters param , IDbTransaction tran = null, CommandType type = CommandType.Text);

        IDataReader ExecuteReader(IDbConnection conn, string sqlTxt, IParameters param , IDbTransaction tran = null, CommandType type = CommandType.Text);
    }

    /// <summary>
    /// 参数集合
    /// </summary>
    public interface IParameters
    {

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        void Add(string name, object value = null, DbType? dbType = null, ParameterDirection direction = ParameterDirection.Input, int? size = null,  byte? precision = null, byte? scale = null);
 

        /// <summary>
        /// 获取参数实体
        /// </summary>
        /// <returns></returns>
        object GetParameters();


        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T GetValue<T>(string name);
    }
}
