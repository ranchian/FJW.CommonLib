using System.Collections.Generic;
using System.Data;
using Dapper;

namespace FJW.Repository.Expression2Sql
{
    public class DbExecutor: IDbExecutor
    {
        public IEnumerable<T> Query<T>(IDbConnection conn, string sqlTxt, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text)
        {
            return conn.Query<T>(sqlTxt, param, tran, commandType: type);
        }

        public IDynamicReader DynamicReader(IDbConnection conn, string sqlTxt, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text)
        {
            var reader = conn.QueryMultiple(sqlTxt, param, tran, commandType: type);
            return new DynamicReader(reader);
        }


        public void Execute(IDbConnection conn, string sqlTxt, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text)
        {
            conn.Execute(sqlTxt, param, tran, commandType: type);
        }

        public T ExecuteScalar<T>(IDbConnection conn, string sqlTxt, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text)
        {
            return conn.ExecuteScalar<T>(sqlTxt, param, tran, commandType: type);
        }

        public IDataReader ExecuteReader(IDbConnection conn, string sqlTxt, object param = null, IDbTransaction tran = null, CommandType type = CommandType.Text)
        {
            return conn.ExecuteReader(sqlTxt, param, tran, commandType: type);
        }

        public IEnumerable<T> Query<T>(IDbConnection conn, string sqlTxt, IParameters param, IDbTransaction tran = null,
            CommandType type = CommandType.Text)
        {
            return Query<T>(conn, sqlTxt, param.GetParameters(), tran, type);
        }


        public IDynamicReader DynamicReader(IDbConnection conn, string sqlTxt, IParameters param,
            IDbTransaction tran = null, CommandType type = CommandType.Text)
        {
            return DynamicReader(conn, sqlTxt, param.GetParameters(), tran, type);
        }


        public void Execute(IDbConnection conn, string sqlTxt, IParameters param, IDbTransaction tran = null,
            CommandType type = CommandType.Text)
        {
            Execute(conn, sqlTxt, param.GetParameters(), tran, type);
        }

        public T ExecuteScalar<T>(IDbConnection conn, string sqlTxt, IParameters param, IDbTransaction tran = null,
            CommandType type = CommandType.Text)
        {
            return ExecuteScalar<T>(conn, sqlTxt, param.GetParameters(), tran, type);
        }

        public IDataReader ExecuteReader(IDbConnection conn, string sqlTxt, IParameters param,
            IDbTransaction tran = null, CommandType type = CommandType.Text)
        {
            return ExecuteReader(conn, sqlTxt, param.GetParameters(), tran, type);
        }
    }

    /// <summary>
    /// 动态读取
    /// </summary>
    public class DynamicReader : DisposableObject, IDynamicReader
    {
        private readonly SqlMapper.GridReader _reader;
        public DynamicReader(SqlMapper.GridReader gridReader)
        {
            _reader = gridReader;
        }
        public IEnumerable<T> Read<T>()
        {
           return _reader.Read<T>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_reader != null)
                {
                    _reader.Dispose();
                }
            }
        }

    }

    /// <summary>
    /// 动态参数
    /// </summary>
    public class DapperParameters : IParameters
    {
        private readonly DynamicParameters _parameters;

        public DapperParameters()
        {
            _parameters = new DynamicParameters();
        }

        public void Add(string name, object value = null, DbType? dbType = null, ParameterDirection direction = ParameterDirection.Input, int? size = null, byte? precision = null, byte? scale = null)
        {
            _parameters.Add(name, value, dbType, direction,  size, precision, scale);
        }

       
        public object GetParameters()
        {
            return _parameters;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetValue<T>(string name)
        {
            return _parameters.Get<T>(name);
        }
    }
}
