using System;

using FJW.SmsServiceDto;

using FJW.SmsService.Bll;
using FJW.CommonLib.Utils;
using FJW.CommonLib.XService;

namespace FJW.SmsService.Service
{
    /// <summary>
    /// 发送邮件服务（提供给外界调用，只做邮件信息入库操作）
    /// </summary>
    public class SendMsg : IServiceHandler
    {
        public ServiceResult Invoke(ServiceRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.data))
                return ServiceResult.Exception(ServiceResultStatus.Error, "input not support");

            try
            {
                SmsRequest smsReq = JsonHelper.JsonDeserialize<SmsRequest>(req.data);
                if (smsReq.IsAsync)
                {
                    bool acceptRes = SmsLibrary.Instance.AcceptSmsRequest(smsReq, MessageStatus.NoSend);
                    if (acceptRes)
                    {
                        Logger.Info("AcceptSmsRequest Success. SmsRequest：【{0}】", req.data);
                        return ServiceResult.Success("Send Message Success");
                    }
                }
                else
                {
                    bool sendRes = SmsLibrary.Instance.SendMsg(smsReq);
                    if (sendRes)
                    {
                        Logger.Info("Send Message Success. SmsRequest：【{0}】", req.data);
                        return ServiceResult.Success("Send Message Success");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Send Message Failed. SmsRequest：【{0}】；Exception：【{1} {2}】", req.data, ex.Message, ex.StackTrace);
                return ServiceResult.Exception(ServiceResultStatus.Error, string.Format("Send Message Failed. {0}", ex.Message));
            }

            Logger.Info("Send Message Failed. SmsRequest：【{0}】", req.data);
            return ServiceResult.Exception(ServiceResultStatus.Error, "Send Message Failed.");
        }
    }
}