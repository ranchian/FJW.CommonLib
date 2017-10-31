using FJW.SmsService.Bll;
using FJW.CommonLib.XService;

namespace FJW.SmsService.Service
{
    /// <summary>
    /// 短信发送服务
    /// </summary>
    public class SmsService : IBaseService
    {
        public void Invoke()
        {
            SmsLibrary.Instance.SendMsgAsync();
        }
    }
}