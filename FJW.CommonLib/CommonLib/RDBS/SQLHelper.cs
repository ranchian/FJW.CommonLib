using System;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;

using FJW.CommonLib.IO;
using FJW.CommonLib.Encrypt;
using FJW.CommonLib.Configuration;

namespace FJW.CommonLib.RDBS
{
    /// <summary>
    /// SQL帮助类
    /// </summary>
    public class SQLHelper
    {
        /// <summary>
        /// DB工厂
        /// </summary>
        private SqlClientFactory _DBFactory;
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _SqlConnStr;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connName">连接标识</param>
        public SQLHelper(string connName)
        {
            //设置数据工厂
            _DBFactory = SqlClientFactory.Instance;

            // 设置连接字符串
            try
            {
                string connFile = PathHelper.MergePathName(PathHelper.GetConfigPath(), "SQLConn.config");
                SQLConnList conns = ConfigManager.GetObjectConfig<SQLConnList>(connFile);
                string connFormat = "Server={0};Database={1};uid={2};pwd={3};Trusted_Connection=false;";
                List<SQLConnConfig> connList = conns.ConnList.Where(x => x.ConnName == connName).Select(x => x).ToList();
                if (connList != null && connList.Count > 0)
                {
                    SQLConnConfig conf = connList[0];
                    IEncryptManager em = EncryptFactory.CreateEncryptManager(EncryptVersion.AES, "SQL@CONN", "CONN@SQL");
                    if (em != null)
                    {
                        _SqlConnStr = string.Format(connFormat, conf.Server, conf.DataBase, conf.UID, em.DecryptData(conf.Pwd));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("创建数据库链接失败，原因：" + ex.Message);
            }
        }

        #region ExecuteNonQuery
        /// <summary>
        /// 执行一个SQL命令(不带SQL命令参数,命令类别为SQL语句),该命令不返回结果集
        /// </summary>
        /// <param name="pCommandText">T-SQL语句</param>
        /// <returns>受SQL命令影响的数据行数</returns>
        public int ExecuteNonQuery(string pCommandText)
        {
            return ExecuteNonQuery(CommandType.Text, pCommandText, null);
        }
        /// <summary>
        /// 执行一个SQL命令(不带SQL命令参数),该命令不返回结果集
        /// </summary>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <returns>受SQL命令影响的数据行数</returns>
        public int ExecuteNonQuery(CommandType pCommandType, string pCommandText)
        {
            return ExecuteNonQuery(pCommandType, pCommandText, null);
        }
        /// <summary>
        /// 执行一个SQL命令,该命令不返回结果集
        /// </summary>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <param name="pCommandParameters">SQL命令的参数</param>
        /// <returns>受SQL命令影响的数据行数</returns>
        public int ExecuteNonQuery(CommandType pCommandType, string pCommandText, params DbParameter[] pCommandParameters)
        {
            DbCommand cmd = _DBFactory.CreateCommand();

            using (DbConnection conn = _DBFactory.CreateConnection())
            {
                conn.ConnectionString = _SqlConnStr;
                PrepareCommand(cmd, conn, null, pCommandType, pCommandText, pCommandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 创建事务
        /// </summary>
        /// <returns></returns>
        public IDbTransaction CreateDbTransaction()
        {
            DbConnection conn = _DBFactory.CreateConnection();
            conn.ConnectionString = _SqlConnStr;
            if (conn.State != ConnectionState.Open)
                conn.Open();
            var tran = conn.BeginTransaction();
            return tran;
        }

        /// <summary>
        /// 在指定的事务下执行一个SQL命令(不带SQL命令参数),该命令不返回结果集
        /// </summary>
        /// <param name="pTransaction">事务</param>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <returns>受SQL命令影响的数据行数</returns>
        public int ExecuteNonQuery(DbTransaction pTransaction, CommandType pCommandType, string pCommandText)
        {
            return ExecuteNonQuery(pTransaction, pCommandType, pCommandText, null);
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="pTransaction">事务</param>
        /// <param name="pCommandType">SQL类型</param>
        /// <param name="pCommandText">T-SQL语句</param>
        /// <param name="pCommandParameters">参数</param>
        /// <returns>受SQL命令影响的数据行数</returns>
        public int ExecuteNonQuery(DbTransaction pTransaction, CommandType pCommandType, string pCommandText, params DbParameter[] pCommandParameters)
        {
            DbCommand cmd = _DBFactory.CreateCommand();
            PrepareCommand(cmd, pTransaction.Connection, pTransaction, pCommandType, pCommandText, pCommandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        #endregion

        #region ExecuteReader
        /// <summary>
        /// 执行一个SQL命令(不带SQL命令参数，命令类别为SQL语句)并返回一个向前的只读数据读取器
        /// </summary>
        /// <param name="pCommandText">SQL语句</param>
        /// <returns>向前的只读数据读取器</returns>
        public DbDataReader ExecuteReader(string pCommandText)
        {
            return ExecuteReader(CommandType.Text, pCommandText, (DbParameter[])null);
        }
        /// <summary>
        /// 执行一个SQL命令(不带SQL命令参数)并返回一个向前的只读数据读取器
        /// </summary>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <returns>向前的只读数据读取器</returns>
        public DbDataReader ExecuteReader(CommandType pCommandType, string pCommandText)
        {
            return ExecuteReader(pCommandType, pCommandText, null);
        }
        /// <summary>
        /// 执行一个SQL命令并返回一个向前的只读数据读取器
        /// </summary>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <param name="pCommandParameters">SQL命令的参数</param>
        /// <returns>向前的只读数据读取器</returns>
        public DbDataReader ExecuteReader(CommandType pCommandType, string pCommandText, params DbParameter[] pCommandParameters)
        {
            DbCommand cmd = _DBFactory.CreateCommand();
            DbConnection conn = _DBFactory.CreateConnection();
            conn.ConnectionString = _SqlConnStr;
            try
            {
                PrepareCommand(cmd, conn, null, pCommandType, pCommandText, pCommandParameters);
                DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        /// <summary>
        /// 在指定的事务下,执行一个SQL命令(不带SQL命令参数)并返回一个向前的只读数据读取器
        /// </summary>
        /// <param name="pTransaction">事务</param>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <returns>向前的只读数据读取器</returns>
        public DbDataReader ExecuteReader(DbTransaction pTransaction, CommandType pCommandType, string pCommandText)
        {
            return ExecuteReader(pTransaction, pCommandType, pCommandText, null);
        }
        /// <summary>
        /// 在指定的事务下,执行一个SQL命令并返回一个向前的只读数据读取器
        /// </summary>
        /// <param name="pTransaction">事务</param>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <param name="pCommandParameters">SQL命令的参数</param>
        /// <returns>向前的只读数据读取器</returns>
        public DbDataReader ExecuteReader(DbTransaction pTransaction, CommandType pCommandType, string pCommandText, params DbParameter[] pCommandParameters)
        {
            DbCommand cmd = _DBFactory.CreateCommand();
            PrepareCommand(cmd, pTransaction.Connection, pTransaction, pCommandType, pCommandText, pCommandParameters);
            DbDataReader rdr = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return rdr;
        }

        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行一个SQL命令(不带SQL命令参数，命令类别为SQL语句)，该命令返回一个 1x1 的结果集
        /// </summary>
        /// <param name="pCommandText">SQL语句</param>
        /// <returns>1x1 的结果集中所包含的对象</returns>
        public object ExecuteScalar(string pCommandText)
        {
            return ExecuteScalar(CommandType.Text, pCommandText, (DbParameter[])null);
        }
        /// <summary>
        /// 执行一个SQL命令(不带SQL命令参数)，该命令返回一个 1x1 的结果集
        /// </summary>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <returns>1x1 的结果集中所包含的对象</returns>
        public object ExecuteScalar(CommandType pCommandType, string pCommandText)
        {
            return ExecuteScalar(pCommandType, pCommandText, null);
        }
        /// <summary>
        /// 执行一个SQL命令，该命令返回一个 1x1 的结果集
        /// </summary>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <param name="pCommandParameters">SQL命令的参数</param>
        /// <returns>1x1 的结果集中所包含的对象</returns>
        public object ExecuteScalar(CommandType pCommandType, string pCommandText, params DbParameter[] pCommandParameters)
        {
            DbCommand cmd = _DBFactory.CreateCommand();

            using (DbConnection connection = _DBFactory.CreateConnection())
            {
                connection.ConnectionString = _SqlConnStr;
                PrepareCommand(cmd, connection, null, pCommandType, pCommandText, pCommandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }
        /// <summary>
        /// 在指定的事务下,执行一个SQL命令(不带SQL命令参数)，该命令返回一个 1x1 的结果集
        /// </summary>
        /// <param name="pTransaction">事务</param>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <returns>1x1 的结果集中所包含的对象</returns>
        public object ExecuteScalar(DbTransaction pTransaction, CommandType pCommandType, string pCommandText)
        {
            return ExecuteScalar(pTransaction, pCommandType, pCommandText, null);
        }
        /// <summary>
        /// 在指定的事务下,执行一个SQL命令，该命令返回一个 1x1 的结果集
        /// </summary>
        /// <param name="pTransaction">事务</param>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <param name="pCommandParameters">SQL命令的参数</param>
        /// <returns>1x1 的结果集中所包含的对象</returns>
        public object ExecuteScalar(DbTransaction pTransaction, CommandType pCommandType, string pCommandText, params DbParameter[] pCommandParameters)
        {
            DbCommand cmd = _DBFactory.CreateCommand();
            PrepareCommand(cmd, pTransaction.Connection, pTransaction, pCommandType, pCommandText, pCommandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        #endregion

        #region ExecuteDataset
        /// <summary>
        /// 执行一个SQL命令(不带SQL命令参数，命令类别为SQL语句)并返回一个数据集
        /// </summary>
        /// <param name="pCommandText">SQL语句</param>
        /// <returns>数据集</returns>
        public DataSet ExecuteDataset(string pCommandText)
        {
            return ExecuteDataset(CommandType.Text, pCommandText, (DbParameter[])null);
        }
        /// <summary>
        /// 执行一个SQL命令(不带SQL命令参数)并返回一个数据集
        /// </summary>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <returns>数据集</returns>
        public DataSet ExecuteDataset(CommandType pCommandType, string pCommandText)
        {
            return ExecuteDataset(pCommandType, pCommandText, null);
        }
        /// <summary>
        /// 执行一个SQL命令并返回一个数据集
        /// </summary>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <param name="pCommandParameters">SQL命令的参数</param>
        /// <returns>数据集</returns>
        public DataSet ExecuteDataset(CommandType pCommandType, string pCommandText, params DbParameter[] pCommandParameters)
        {
            DbCommand cmd = _DBFactory.CreateCommand();
            DbConnection conn = _DBFactory.CreateConnection();
            conn.ConnectionString = _SqlConnStr;
            DbDataAdapter adapter = _DBFactory.CreateDataAdapter();
            try
            {
                PrepareCommand(cmd, conn, null, pCommandType, pCommandText, pCommandParameters);
                adapter.SelectCommand = cmd;
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
            catch
            {
                throw;
            }
            finally
            {
                adapter.Dispose();
                conn.Close();
                conn.Dispose();
            }
        }
        /// <summary>
        /// 在指定的事务下,执行一个SQL命令(不带SQL命令参数)并返回一个数据集
        /// </summary>
        /// <param name="pTransaction">事务</param>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <returns>数据集</returns>
        public DataSet ExecuteDataset(DbTransaction pTransaction, CommandType pCommandType, string pCommandText)
        {
            return ExecuteDataset(pTransaction, pCommandType, pCommandText, null);
        }
        /// <summary>
        /// 在指定的事务下,执行一个SQL命令并返回一个数据集
        /// </summary>
        /// <param name="pTransaction">事务</param>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <param name="pCommandParameters">SQL命令的参数</param>
        /// <returns>数据集</returns>
        public DataSet ExecuteDataset(DbTransaction pTransaction, CommandType pCommandType, string pCommandText, params DbParameter[] pCommandParameters)
        {
            DbCommand cmd = _DBFactory.CreateCommand();
            DbDataAdapter adapter = _DBFactory.CreateDataAdapter();
            PrepareCommand(cmd, pTransaction.Connection, pTransaction, pCommandType, pCommandText, pCommandParameters);
            adapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            cmd.Parameters.Clear();
            return ds;
        }

        #endregion

        #region PrepareCommand
        /// <summary>
        /// 设置SQL命令
        /// </summary>
        /// <param name="pCommand">SQL命令</param>
        /// <param name="pConnection">数据库连接</param>
        /// <param name="pTransaction">事务</param>
        /// <param name="pCommandType">SQL命令的类别(存储过程,SQL语句等)</param>
        /// <param name="pCommandText">存储过程名称或者是SQL语句</param>
        /// <param name="pCommandParameters">SQL命令的参数</param>
        private void PrepareCommand(DbCommand pCommand, DbConnection pConnection, DbTransaction pTransaction, CommandType pCommandType, string pCommandText, DbParameter[] pCommandParameters)
        {
            if (pConnection.State != ConnectionState.Open)
                pConnection.Open();
            pCommand.Connection = pConnection;
            pCommand.CommandText = pCommandText;
            if (pTransaction != null)
                pCommand.Transaction = pTransaction;
            pCommand.CommandType = pCommandType;
            if (pCommandParameters != null)
            {
                foreach (DbParameter parm in pCommandParameters)
                {
                    if (parm != null)
                    {
                        if ((parm.Direction == ParameterDirection.InputOutput || parm.Direction == ParameterDirection.Input) &&
                            (parm.Value == null))
                        {
                            parm.Value = DBNull.Value;
                        }
                        pCommand.Parameters.Add(parm);
                    }
                }
            }
        }
        #endregion
    }

    #region SQL连接配置
    /// <summary>
    /// 数据库连接字符串配置
    /// </summary>
    class SQLConnConfig
    {
        /// <summary>
        /// 连接标识
        /// </summary>
        [Node]
        public string ConnName { get; set; }

        /// <summary>
        /// 数据库地址
        /// </summary>
        [Node]
        public string Server { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        [Node]
        public string DataBase { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Node]
        public string UID { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Node]
        public string Pwd { get; set; }
    }

    /// <summary>
    /// 数据库连接列表
    /// </summary>
    class SQLConnList
    {
        /// <summary>
        /// 数据库连接列表
        /// </summary>
        [Node("Conns/Conn", NodeAttribute.NodeType.List)]
        public List<SQLConnConfig> ConnList { get; set; }
    }
    #endregion
}
