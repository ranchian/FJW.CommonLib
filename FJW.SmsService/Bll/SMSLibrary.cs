using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using FJW.SmsServiceDto;
using FJW.SmsService.Entity;

using FJW.CommonLib.IO;
using FJW.CommonLib.Redis;
using FJW.CommonLib.Utils;
using FJW.CommonLib.Validation;
using FJW.CommonLib.Configuration;

namespace FJW.SmsService.Bll
{
    public class SmsLibrary
    {
        /// <summary>
        /// 调用方与邮件通道映射关系字典
        /// </summary>
        private readonly Dictionary<string, ChannelNode> _smsInfoDic = new Dictionary<string, ChannelNode>();

        #region 构造函数
        private SmsLibrary()
        {
            try
            {
                string configPath = PathHelper.GetConfigPath();
                string smsFile = PathHelper.MergePathName(configPath, "SMS.config");
                SmsConfig smsConfig = ConfigManager.GetObjectConfig<SmsConfig>(smsFile);
                string callerFile = PathHelper.MergePathName(configPath, "Caller.config");
                CallerConfig callerConfig = ConfigManager.GetObjectConfig<CallerConfig>(callerFile);
                if (callerConfig != null && smsConfig != null)
                {
                    List<ChannelNode> smsList = smsConfig.ChannelList;
                    foreach (var ca in callerConfig.CallerList)
                    {
                        List<ChannelNode> channel = smsList.Where(x => x.Name == ca.Channel).Select(x => x).ToList();
                        if (channel.Count == 0)
                            continue;

                        if (!_smsInfoDic.ContainsKey(ca.CallerName))
                            _smsInfoDic.Add(ca.CallerName, channel[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Init SmsConfig Failed. {0} {1}", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region 单例
        private static SmsLibrary _instance;
        private static readonly object LockObj = new object();
        public static SmsLibrary Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new SmsLibrary();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region 接收发送邮件请求，入库操作

        /// <summary>
        /// 接收发送邮件请求，入库操作
        /// </summary>
        /// <param name="req"></param>
        /// <param name="status"></param>
        /// <param name="smsResp"></param>
        /// <returns></returns>
        public bool AcceptSmsRequest(SmsRequest req, MessageStatus status, string smsResp = "")
        {
            if (req == null)
                throw new ArgumentNullException("req");

            // mail参数校验
            string errMsg;
            if (!ValidateHelper.CheckEntity(ref req, out errMsg))
                throw new ArgumentException(errMsg);

            return SqlDapper.Instance.InsertMsgToDb(req, status, smsResp);
        }
        #endregion

        #region 发送邮件
        /// <summary>
        /// 异步发送邮件
        /// </summary>
        public void SendMsgAsync()
        {
            try
            {
                // 从数据库中获取待发送邮件列表
                List<SmsRequest> smsList = SqlDapper.Instance.GetSmsFromDb();
                if (smsList == null)
                    return;

                foreach (SmsRequest sms in smsList)
                {
                    if (!_smsInfoDic.ContainsKey(sms.CallerName))
                        continue;

                    ChannelNode sn = _smsInfoDic[sms.CallerName];

                    // 分布式锁  锁SMS.ID
                    var sms1 = sms;
                    Task.Run(() =>
                    {
                        LockItem item = new LockItem(sms1.Phone.ToString(), typeof(SmsRequest));
                        string lockKey = RedisLockHelper.Instance.GetRedisLock(item, false, 1);
                        if (!string.IsNullOrWhiteSpace(lockKey))
                        {
                            try
                            {
                                if (SmsHelper.SendMessage(sms1, sn))
                                    Logger.Info("SendAsync Message Success. SMSRequest：【{0}】", JsonHelper.JsonSerializer(sms1));
                                else
                                    Logger.Error("SendAsync Message Failed. SMSRequest：【{0}】", JsonHelper.JsonSerializer(sms1));
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("Send Message failed. {0} {1}", ex.Message, ex.StackTrace);
                            }
                            finally
                            {
                                RedisLockHelper.Instance.ReleaseLock(lockKey);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("SendMsgAsync Exception. {0} {1}", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 同步发送邮件
        /// </summary>
        public bool SendMsg(SmsRequest smsReq)
        {
            try
            {
                if (!_smsInfoDic.ContainsKey(smsReq.CallerName))
                    throw new Exception("Caller not registered.");

                ChannelNode sn = _smsInfoDic[smsReq.CallerName];
                return SmsHelper.SendMessage(smsReq, sn);
            }
            catch (Exception ex)
            {
                Logger.Error("SendMsg Exception. SMSRequest：【{0}】; Exception：【{1} {2}】", smsReq, ex.Message, ex.StackTrace);
            }
            return false;
        }
        #endregion
    }
}