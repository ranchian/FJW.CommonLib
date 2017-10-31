using System;
using System.IO;
using System.Web;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using FJW.CommonLib.Utils;
using FJW.CommonLib.XService;

namespace FJW.CommonLib.XHttp
{
    /// <summary>
    /// 自定义httpHandler
    /// </summary>
    public class XHttpHandler : IHttpHandler
    {
        /// <summary>
        /// 指示其他请求是否可以使用 IHttpHandler 实例
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// http请求入口
        /// </summary>
        /// <param name="context">http上下文</param>
        public void ProcessRequest(HttpContext context)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string guid = "";
            try
            {
                Logger.Debug("ProcessRequest Path:" + context.Request.Url.AbsolutePath);
                string path = context.Request.Url.AbsolutePath;
                if (path != "/api")
                    throw new Exception("path not support");

                StreamReader sr = new StreamReader(context.Request.InputStream, Encoding.UTF8);
                string jsonReq = sr.ReadToEnd();
                if (string.IsNullOrEmpty(jsonReq))
                    throw new Exception("input not support");

                // HTML编码以及SQL特殊字符转译
                jsonReq = TranslateReq(jsonReq);

                XHttpReq req = JsonHelper.JsonDeserialize<XHttpReq>(jsonReq);
                if (req == null)
                    throw new Exception("input not support");
                req.Ts = DateTime.UtcNow.Ticks;
                guid = req.G;
                //保存原始请求参数
                HttpContext.Current.Items.Add("jsonReq", jsonReq);
                XHttpRes res = ServiceEngine.Handle(req, context);

                sw.Stop();
                res.Tt = sw.ElapsedMilliseconds;
                res.G = guid;
                string resJson = JsonHelper.JsonSerializer(res);
                context.Response.Write(resJson);
            }
            catch (MethodAccessException ex)
            {
                sw.Stop();
                context.Response.Write(JsonHelper.JsonSerializer(XHttpRes.Exception(sw.ElapsedMilliseconds, guid, (int)ServiceResultStatus.InvalidParameter, ex.Message)));
                Logger.Error("{3}:{0},{1} {2}", guid, ex.Message, ex.StackTrace, WebHelper.GetIP());
            }
            catch (ArgumentException ex)
            {
                sw.Stop();
                context.Response.Write(JsonHelper.JsonSerializer(XHttpRes.Exception(sw.ElapsedMilliseconds, guid, (int)ServiceResultStatus.InvalidParameter, ex.Message)));
                Logger.Error("{3}:{0},{1} {2}", guid, ex.Message, ex.StackTrace, WebHelper.GetIP());
            }
            catch (Exception ex)
            {
                sw.Stop();
                context.Response.Write(JsonHelper.JsonSerializer(XHttpRes.Exception(sw.ElapsedMilliseconds, guid, (int)ServiceResultStatus.Error, ex.Message)));
                Logger.Error("{2}:{0},{1}", guid, ex.Message, "");
            }
        }

        /// <summary>
        /// HTML编码以及SQL特殊字符转译
        /// </summary>
        /// <param name="reqJson"></param>
        /// <returns></returns>
        private string TranslateReq(string reqJson)
        {
            reqJson = Regex.Replace(reqJson, "^<$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^>$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^select$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^insert$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^update$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^create$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^delete from$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^count''$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^drop table$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^truncate$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^asc$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^mid$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^char$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^xp_cmdshell$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^exec master$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^net localgroup administrators$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^and$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^net user$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^or$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^Exec$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^Execute$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^net$", "", RegexOptions.IgnoreCase);
            reqJson = Regex.Replace(reqJson, "^script$", "", RegexOptions.IgnoreCase);

            return reqJson;
        }
    }

    /// <summary>
    /// 平台标识枚举
    /// </summary>
    public enum ReqPlatForm
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,
        /// <summary>
        /// Android
        /// </summary>
        Android = 1,
        /// <summary>
        /// IOS
        /// </summary>
        Ios = 2,
    }

    /// <summary>
    /// 请求Model
    /// </summary>
    public class XHttpReq
    {
        public XHttpReq()
        {
            D = "";
            M = "";
            V = 0;
            Did = "";
            G = Guid.NewGuid().ToString();
            Ie = false;
            E = "";
            T = "";
            Ts = DateTime.UtcNow.Ticks;
            P = (int)ReqPlatForm.Default;
            Mid = 0;
            Idv = "";
            Adv = "";
        }

        /// <summary>
        /// 数据 json格式
        /// Data
        /// </summary>
        public string D { get; set; }

        /// <summary>
        /// 调用方法名
        /// Method
        /// </summary>
        public string M { get; set; }

        /// <summary>
        /// 客户端版本号
        /// </summary>
        public int V { get; set; }

        /// <summary>
        /// 设备ID
        /// DeviceID
        /// </summary>
        public string Did { get; set; }

        /// <summary>
        /// Guid
        /// </summary>
        public string G { get; set; }

        /// <summary>
        /// 是否加密
        /// </summary>
        public bool Ie { get; set; }

        /// <summary>
        /// 加密版本号
        /// </summary>
        public string E { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string T { get; set; }

        /// <summary>
        /// 时间戳
        /// TimeStamp
        /// </summary>
        public long Ts { get; set; }

        /// <summary>
        /// 平台标识(PlatForm,1:Android;2:IOS)
        /// </summary>
        public int P { get; set; }

        /// <summary>
        /// 服务间调用用MemberID
        /// </summary>
        public long Mid { get; set; }

        /// <summary>
        /// 低频数据缓存版本号IOS用（IOSDataVersion）
        /// </summary>
        public string Idv { get; set; }

        /// <summary>
        /// 低频数据缓存版本号Android用（AndroidDataVersion）
        /// </summary>
        public string Adv { get; set; }
    }

    /// <summary>
    /// 返回结构体
    /// </summary>
    public class XHttpRes
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public XHttpRes()
        {
            D = "";
            Tt = 0;
            G = "";
            Ie = false;
            E = "";
            V = 0;
            S = 0;
            Es = "";
            Idv = "";
            Adv = "";
        }

        /// <summary>
        /// 数据
        /// Data
        /// </summary>
        [JsonProperty("d")]
        public string D { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("tt")]
        public long Tt { get; set; }

        /// <summary>
        /// APP版本号（APPVersion）
        /// </summary>
        [JsonProperty("v")]
        public int V { get; set; }

        /// <summary>
        /// Guid
        /// </summary>
        [JsonProperty("g")]
        public string G { get; set; }

        /// <summary>
        /// 是否加密
        /// </summary>
        [JsonProperty("ie")]
        public bool Ie { get; set; }

        /// <summary>
        /// 加密版本号
        /// </summary>
        [JsonProperty("e")]
        public string E { get; set; }

        /// <summary>
        /// 状态
        /// Status
        /// </summary>
        [JsonProperty("s")]
        public int S { get; set; }

        /// <summary>
        /// 异常消息
        /// </summary>
        [JsonProperty("es")]
        public string Es { get; set; }

        /// <summary>
        /// 低频数据缓存版本号IOS用（IOSDataVersion）
        /// </summary>
        [JsonProperty("idv")]
        public string Idv { get; set; }

        /// <summary>
        /// 低频数据缓存版本号Android用（AndroidDataVersion）
        /// </summary>
        [JsonProperty("adv")]
        public string Adv { get; set; }

        /// <summary>
        /// 异常情况
        /// </summary>
        /// <param name="timetaken">耗时</param>
        /// <param name="guid">GUID</param>
        /// <param name="status">状态</param>
        /// <param name="exception">异常信息</param>
        /// <returns>http响应实体</returns>
        public static XHttpRes Exception(long timetaken, string guid = "", int status = 0, string exception = "")
        {
            return new XHttpRes { Tt = timetaken, G = guid, Es = exception, S = status };
        }

        /// <summary>
        /// 异常情况
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="exception">异常信息</param>
        /// <param name="data">数据</param>
        /// <returns>http响应实体</returns>
        public static XHttpRes Exception(int status, string exception, string data)
        {
            return new XHttpRes { Es = string.IsNullOrEmpty(exception) ? "" : exception, S = status, D = data ?? string.Empty };
        }

        public static XHttpRes Exception(int status, string exception = "")
        {
            return new XHttpRes { Es = exception, S = status };
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">业务数据</param>
        /// <param name="encryptionV">加密版本</param>
        /// <param name="reqEHandlerList"></param>
        /// <param name="isEncryption">true:加密;false:不加密;</param>
        /// <param name="req"></param>
        /// <returns>http响应实体</returns>
        internal static XHttpRes Success(string data = "", XHttpReq req = null, List<IOnRequestEnd> reqEHandlerList = null, bool isEncryption = false, string encryptionV = "")
        {
            var res = new XHttpRes { S = 0, D = data, Ie = isEncryption, E = encryptionV ?? "" };

            foreach (var handler in reqEHandlerList)
            {
                try
                {
                    handler.Invoke(req, ref res);
                }
                catch (Exception ex)
                {
                    Logger.Error("RequestEndHandler Invoke Failed. ", ex);
                }
            }
            return res;
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data">业务数据</param>
        /// <param name="encryptionV">加密版本</param>
        /// <param name="reqEHandlerList"></param>
        /// <param name="isEncryption">true:加密;false:不加密;</param>
        /// <returns>http响应实体</returns>
        internal static XHttpRes Success<T>(T data, XHttpReq req = null, List<IOnRequestEnd> reqEHandlerList = null, bool isEncryption = false, string encryptionV = "") where T : class,new()
        {
            string dataStr = JsonHelper.JsonSerializer(data);
            return Success(dataStr, req, reqEHandlerList, isEncryption, encryptionV);
        }
    }
}