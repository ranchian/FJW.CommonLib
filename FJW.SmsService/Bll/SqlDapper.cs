using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;

using FJW.SmsServiceDto;
using FJW.CommonLib.RDBS;
using FJW.CommonLib.Utils;
using FJW.CommonLib.ExtensionMethod;

namespace FJW.SmsService.Bll
{
    public class SqlDapper
    {
        private string _connName = "SMSSqlConn";

        #region 单例
        private static SqlDapper _sqlDapper;
        private static readonly object LockObj = new object();

        public static SqlDapper Instance
        {
            get
            {
                if (_sqlDapper == null)
                {
                    lock (LockObj)
                    {
                        if (_sqlDapper == null)
                        {
                            _sqlDapper = new SqlDapper();
                        }
                    }
                }
                return _sqlDapper;
            }
        }
        #endregion

        /// <summary>
        /// 设置SQLHelper实例名
        /// </summary>
        /// <param name="connName"></param>
        public void SetConnName(string connName)
        {
            _connName = connName;
        }

        /// <summary>
        /// 短信信息插入数据库
        /// </summary>
        /// <param name="req"></param>
        /// <param name="status"></param>
        /// <param name="smsResp"></param>
        /// <returns></returns>
        public bool InsertMsgToDb(SmsRequest req, MessageStatus status, string smsResp = "")
        {
            string sql = @"
                    INSERT INTO [dbo].[SMSInfo]
                          ([CallerName]
                          ,[IsAsync]
                          ,[Phone]
                          ,[Message]
                          ,[Status]
                          ,[InputDate]
                          ,[SendDate]
                          ,[SMSResponse]
                          ,[Remark])
                    VALUES(@CName,@IsAsync,@Phone,@Msg,@Status,GETDATE(),@SendDate,@SMSResponse,@Remark)";

            try
            {
                SQLHelper smsSql = new SQLHelper(Instance._connName);
                object obj = DateTime.Now.ToString(TimeFormat.YMDHMS);
                if (status == MessageStatus.NoSend)
                    obj = DBNull.Value;
                DbParameter[] dbParam = { 
                    new SqlParameter("@CName",req.CallerName),  
                    new SqlParameter("@IsAsync",req.IsAsync),
                    new SqlParameter("@Phone",req.Phone),     
                    new SqlParameter("@Msg",req.Message),
                    new SqlParameter("@Status",status),
                    new SqlParameter("@SendDate",obj),
                    new SqlParameter("@SMSResponse",smsResp),
                    new SqlParameter("@Remark",req.Remark ?? string.Empty),
                };
                int count = smsSql.ExecuteNonQuery(CommandType.Text, sql, dbParam);
                if (count > 0)
                    return true;
            }
            catch (Exception ex)
            {
                Logger.Error("InsertMsgToDb Failed. ", ex);
            }

            return false;
        }

        /// <summary>
        /// 从数据库中获取待发送短信列表
        /// </summary>
        /// <returns></returns>
        public List<SmsRequest> GetSmsFromDb()
        {
            string sql = @"
                SELECT [ID]
                    ,[CallerName]
                    ,[IsAsync]
                    ,[Phone]
                    ,[Message]
                    ,[Status]
                    ,[InputDate]
                FROM [dbo].[SMSInfo] WITH (NOLOCK)
                WHERE Status = @Status
            ";

            SQLHelper smsSql = new SQLHelper(Instance._connName);
            DbParameter[] dbParam = { new SqlParameter("@Status", MessageStatus.NoSend) };
            DataSet ds = smsSql.ExecuteDataset(CommandType.Text, sql, dbParam);
            if (ds == null || ds.Tables.Count == 0)
                return null;

            return ds.Tables[0].DataTableToList<SmsRequest>();
        }

        /// <summary>
        /// 更新数据库中邮件状态
        /// </summary>
        /// <returns></returns>
        public bool UpdateSmsdb(int id, MessageStatus status, string smsRes)
        {
            string sql =
                @"UPDATE [dbo].[SMSInfo]
                  SET [Status] = @Status
                     ,[SendDate] = GETDATE()
                     ,[SMSResponse] = @SMSResponse
                WHERE ID = @ID
                 ";

            try
            {
                SQLHelper mailSql = new SQLHelper(Instance._connName);
                DbParameter[] dbParam = { 
                    new SqlParameter("@ID",id),
                    new SqlParameter("@Status",status),
                    new SqlParameter("@SMSResponse", smsRes),
                };
                int count = mailSql.ExecuteNonQuery(CommandType.Text, sql, dbParam);
                if (count > 0)
                    return true;
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateSmsdb Failed. ", ex);
            }

            return false;
        }
    }
}