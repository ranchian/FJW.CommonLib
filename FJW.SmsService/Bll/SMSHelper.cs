using System;
using System.Threading;
using System.Collections.Generic;

using FJW.SmsServiceDto;
using FJW.SmsService.Entity;

using FJW.CommonLib.Utils;
using FJW.CommonLib.XHttp;
using FJW.CommonLib.ExtensionMethod;

namespace FJW.SmsService.Bll
{
    public class SmsHelper
    {
        public static bool SendMessage(SmsRequest req, ChannelNode cn)
        {
            bool result = false;
            string smsRep = string.Empty;
            MessageStatus status = MessageStatus.NoSend;
            var state = PhoneStatePool.Contains(req.Phone);
            switch (state)
            {
                case PhoneState.Blanking:
                    break;
                case PhoneState.Quota:
                    status = MessageStatus.Quota;
                    smsRep = "该号码已达到每日最大发送额度";
                    break;
                default:
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    string[] paramArr = cn.Param.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var param in paramArr)
                    {
                        if (param.Contains("="))
                        {
                            string[] kv = param.Split('=');
                            if (!dic.ContainsKey(kv[0]))
                            {
                                if (kv[1] == "@Phone")
                                    dic.Add(kv[0], req.Phone.ToString());
                                else if (kv[1] == "@Msg")
                                    dic.Add(kv[0], req.Message);
                                else
                                    dic.Add(kv[0], kv[1]);
                            }
                        }
                    }

                    status = SendInvoke(req.Phone, cn, dic, out smsRep);
                    break;
            }
            if (status == MessageStatus.Success)
            {
                result = true;
                PhoneStatePool.AddPhoneState(req.Phone);
            }

            if (req.Id == 0)
                SqlDapper.Instance.InsertMsgToDb(req, status, smsRep);
            else if (status != MessageStatus.NoSend)
                SqlDapper.Instance.UpdateSmsdb(req.Id, status, smsRep);
            return result;
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="cn"></param>
        /// <param name="dic"></param>
        /// <param name="smsRep"></param>
        /// <returns></returns>
        private static MessageStatus SendInvoke(long phone, ChannelNode cn, Dictionary<string, string> dic, out string smsRep)
        {
            int count = 0;
            smsRep = string.Empty;
            MessageStatus status = MessageStatus.Failure;
            do
            {
                count++;
                HttpResult result = XHttpHelper.HttpPost(cn.Url, null, dic, "utf-8", 10000);
                if (result.Code == 200)
                {
                    smsRep = result.Content;
                    MessageStatus respStatus = CheckResCode(cn.Name, result.Content);
                    switch (respStatus)
                    {
                        case MessageStatus.NoSend:
                        case MessageStatus.Failure:
                            Thread.Sleep(3000);
                            break;
                        case MessageStatus.Success:
                            status = MessageStatus.Success;
                            Logger.Info("Send Message Success. Param：【{0}】", dic.ToJSON());
                            return status;
                        case MessageStatus.Quota:
                            PhoneStatePool.AddQuotaPhone(phone);
                            return status;
                    }
                }
                else
                {
                    Logger.Error("Send Message Failed. Param：【{0}】", dic.ToJSON());
                    Thread.Sleep(60000);
                }
            } while (count < 3);
            return status;
        }

        /// <summary>
        /// 获取渠道返回发送状态
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        private static MessageStatus CheckResCode(string channelName, string res)
        {
            if (string.IsNullOrWhiteSpace(channelName) || string.IsNullOrWhiteSpace(res))
                return MessageStatus.Failure;

            switch (channelName.ToLower())
            {
                case "chuanglan":
                    return ChuanglanCheck(res);
                case "huyi":
                    return HuYiCheck(res);
                default:
                    return MessageStatus.Failure;
            }
        }

        #region 多渠道短信状态验证
        private static MessageStatus ChuanglanCheck(string res)
        {
            var code = string.Empty;
            if (res.Contains("\n"))
                code = res.Split('\n')[0].Split(',')[1];

            switch (code)
            {
                case "0":
                    return MessageStatus.Success;
                default:
                    return MessageStatus.Failure;
            }
        }

        private static MessageStatus HuYiCheck(string res)
        {
            var len1 = res.IndexOf("</code>", StringComparison.Ordinal);
            var len2 = res.IndexOf("<code>", StringComparison.Ordinal);
            var code = res.Substring(len2 + 6, len1 - len2 - 6);

            switch (code)
            {
                case "2"://成功
                    return MessageStatus.Success;
                case "406"://手机号码不正确
                    return MessageStatus.Failure;
                case "4080"://同一手机号码同一秒钟之内发送频率不能超过1条
                case "4081"://一分钟限制
                case "4083"://同内容每分钟限制：1条
                case "4086"://提交失败（2分钟只能提交一条）
                case "4010"://通道限制：每个号码1分钟内只能发1条
                    return MessageStatus.NoSend;
                case "-2": //PhoneStatePool 满额
                case "4082"://同号码每日限制：5条
                case "4084"://同内容每日限制：5条
                case "4085"://验证码短信每天每个手机号码只能发5条
                    return MessageStatus.Quota;
                default:
                    return MessageStatus.NoSend;
            }
        }
        #endregion
    }
}