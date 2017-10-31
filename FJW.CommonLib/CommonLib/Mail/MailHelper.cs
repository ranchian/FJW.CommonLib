using System;
using System.Net.Mail;
using System.Collections.Generic;

using FJW.CommonLib.Entity;
using FJW.CommonLib.Mail.Enum;
using FJW.CommonLib.Configuration;

namespace FJW.CommonLib.Mail
{
    /// <summary>
    /// 邮件帮助类
    /// </summary>
    public class MailHelper
    {
        /// <summary>
        /// STMP客户端
        /// </summary>
        private static SmtpClient _StmpClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        static MailHelper()
        {
            string host = ConfigManager.GetWebConfig("Host", "");
            int port = ConfigManager.GetWebConfig("Port", 25);
            bool enableSsl = ConfigManager.GetWebConfig("EnableSsl", true);
            string stmpUesr = ConfigManager.GetWebConfig("EmailUesr", "");
            string stmpPwd = ConfigManager.GetWebConfig("EmailPwd", "");
            _StmpClient = new SmtpHelper(host, port, enableSsl, stmpUesr, stmpPwd).SmtpClient;
        }

        /// <summary>
        /// 设置STMP客户端信息
        /// </summary>
        /// <param name="host">stmp服务器</param>
        /// <param name="emailUser">用户名</param>
        /// <param name="emailPwd">用户密码</param>
        /// <param name="enableSsl">指定StmpClient是否使用安全套接字层(SSL)加密链接，默认为true</param>
        public static void SetStmpClient(string host, string emailUser, string emailPwd, int port = 25, bool enableSsl = true)
        {
            _StmpClient = new SmtpHelper(host, port, enableSsl, emailUser, emailPwd).SmtpClient;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailEntitys">邮件配置列表</param>
        public static void SendMail(List<MailEntity> mailEntitys)
        {
            if (mailEntitys == null || mailEntitys.Count == 0)
                return;

            if (_StmpClient == null)
                throw new Exception("StmpClient init failed.");

            MailManager mail = new MailManager(true);
            if (!mail.ExistsSmtpClient())
                mail.SetSmtpClient(_StmpClient, true);

            mail.SetBatchMailCount(mailEntitys.Count);
            foreach (var e in mailEntitys)
            {
                mail.From = e.MailFrom;
                mail.FromDisplayName = e.FromDisplayName;
                if (!string.IsNullOrWhiteSpace(e.MailTo))
                    mail.AddReceive(EmailAddrType.To, e.MailTo, e.ToDisplayName);
                if (!string.IsNullOrWhiteSpace(e.MailCC))
                    mail.AddReceive(EmailAddrType.To, e.MailCC, e.CCDisplayName);
                if (!string.IsNullOrWhiteSpace(e.MailBCC))
                    mail.AddReceive(EmailAddrType.To, e.MailBCC, e.BCCDisplayName);
                mail.Subject = e.Subject;
                mail.Body = e.MailBody;
                mail.IsBodyHtml = true;
                if (!string.IsNullOrWhiteSpace(e.Attachments))
                {
                    string[] AttFileArr = e.Attachments.Split(';');
                    foreach (string filePath in AttFileArr)
                    {
                        mail.AddAttachment(filePath);
                    }
                }

                Dictionary<MailInfoType, string> dic = mail.CheckSendMail();
                if (dic.Count > 0 && MailInfoHelper.ExistsError(dic))
                {
                    throw new Exception(MailInfoHelper.GetMailInfoStr(dic));
                }
                else
                {
                    string msg = String.Empty;
                    if (dic.Count > 0)
                        throw new Exception(MailInfoHelper.GetMailInfoStr(dic));

                    try
                    {
                        mail.SendBatchMail();
                    }
                    catch
                    {
                        throw;
                    }
                }
                mail.Reset();
            }
        }
    }
}
