using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using FJW.CommonLib.Utils;
using Newtonsoft.Json;

namespace FJW.CommonLib.XHttp
{
    /// <summary>
    /// HTTP请求帮助类
    /// </summary>
    public class XHttpHelper
    {
        /// <summary>
        /// 用户代理
        /// </summary>
        private static readonly string UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Maxthon/4.1.2.4000 Chrome/26.0.1410.43 Safari/537.1";

        /// <summary>
        /// http get 请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="headers">header信息</param>
        /// <param name="paramValues">参数键值对</param>
        /// <param name="encoding">编码</param>
        /// <param name="readTimeoutMs">超时时间</param>
        /// <returns>请求结果</returns>
        public static HttpResult HttpGet(string url, Dictionary<string, string> headers, Dictionary<string, string> paramValues, string encoding, int readTimeoutMs)
        {
            string encodedContent = EncodingParam(paramValues, encoding);

            url += (string.IsNullOrEmpty(encodedContent) ? "" : ("?" + encodedContent));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = readTimeoutMs;
            request.ReadWriteTimeout = readTimeoutMs;
            request.ContentType = "application/x-www-form-urlencoded;charset=" + encoding;
            SetHeaders(request, headers, encoding);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string resp = "";
            if (response.StatusCode == HttpStatusCode.OK)


                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
                    resp = reader.ReadToEnd();
            return new HttpResult(response.StatusCode, resp);
        }

        /// <summary>
        /// http post 请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="headers">header信息</param>
        /// <param name="paramValues">参数键值对</param>
        /// <param name="encoding">编码</param>
        /// <param name="readTimeoutMs">超时时间</param>
        /// <returns>请求结果</returns>
        public static HttpResult HttpPost(string url, Dictionary<string, string> headers, Dictionary<string, string> paramValues, string encoding, int readTimeoutMs)
        {
            string encodedContent = EncodingParam(paramValues, encoding);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = readTimeoutMs;
            request.ReadWriteTimeout = readTimeoutMs;
            request.ContentType = "application/x-www-form-urlencoded;charset=" + encoding;
            SetHeaders(request, headers, encoding);

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.GetEncoding(encoding)))
            {
                writer.Write(encodedContent);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string resp = "";
            if (response.StatusCode == HttpStatusCode.OK)

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
                    resp = reader.ReadToEnd();
            return new HttpResult(response.StatusCode, resp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static HttpResult PostJson(string url, object obj, string mediaType = "application/json")
        {
            return PostJson(url, obj, Encoding.UTF8, mediaType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <param name="code"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static HttpResult PostJson(string url, object obj, Encoding code, string mediaType = "application/json")
        {
            using (var handler = new HttpClientHandler())
            {
                handler.Proxy = WebRequest.GetSystemWebProxy();
                handler.UseProxy = true;
                using (var client = new HttpClient(handler))
                using (var byteContent = new StringContent(JsonHelper.JsonSerializer(obj), code, mediaType))
                {
                    if (byteContent.Headers.ContentType == null || byteContent.Headers.ContentType.MediaType != mediaType)
                    {
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
                    }
                    var result = client.PostAsync(url, byteContent).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var resp = result.Content.ReadAsStringAsync().Result;
                        return new HttpResult(result.StatusCode, resp);
                    }
                    return new HttpResult(result.StatusCode, "");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        /// <param name="encoding"></param>
        private static void SetHeaders(HttpWebRequest request, Dictionary<string, string> headers, string encoding)
        {
            if (null != headers)
                foreach (String key in headers.Keys)
                    request.Headers.Add(key, headers[key]);

        }

        private static string EncodingParam(Dictionary<string, string> paramValues, string encoding)
        {
            StringBuilder sb = new StringBuilder();
            if (null == paramValues)
                return null;

            int i = 0;
            foreach (string key in paramValues.Keys)
            {
                sb.Append(key);
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(paramValues[key], Encoding.GetEncoding(encoding)));
                if (i++ != paramValues.Count)
                    sb.Append("&");
            }
            return sb.ToString();
        }

        /// <summary>
        /// http post 请求
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="data">提交的数据</param>
        /// <param name="encoding">编码</param>
        /// <param name="readTimeoutMs">超时时间</param>
        /// <returns></returns>
        public static HttpResult httpPost(string url, string data, string encoding, int readTimeoutMs)
        {
            string encodedContent = data;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = readTimeoutMs;
            request.ReadWriteTimeout = readTimeoutMs;
            request.ContentType = "application/x-www-form-urlencoded;charset=" + encoding;

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.GetEncoding(encoding)))
            {
                writer.Write(encodedContent);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string resp = "";
            if (response.StatusCode == HttpStatusCode.OK)

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
                    resp = reader.ReadToEnd();
            return new HttpResult(response.StatusCode, resp);
        }

        /// <summary>
        /// 获取http状态枚举
        /// </summary>
        /// <param name="url">目标url</param>
        /// <param name="timeOut"></param>
        /// <param name="cc">cookie</param>
        /// <param name="refer">返回链接</param>
        /// <returns></returns>
        public static HttpStatusCode HeadHttpCode(string url, int timeOut = 5, CookieContainer cc = null, string refer = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = timeOut;
                request.UserAgent = UserAgent;
                request.Method = "HEAD";
                request.Referer = refer;
                request.CookieContainer = cc;
                HttpWebResponse httpWebResponse = request.GetResponse() as HttpWebResponse;
                if (httpWebResponse != null)
                    return httpWebResponse.StatusCode;
                return HttpStatusCode.ExpectationFailed;
            }
            catch
            {
                return HttpStatusCode.ExpectationFailed;
            }
        }
    }

    /// <summary>
    /// http响应封装
    /// </summary>
    public class HttpResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code">返回状态码</param>
        /// <param name="response">返回数据</param>
        public HttpResult(HttpStatusCode code, string response)
        {
            this.Code = (int)code;
            this.Content = response;
        }

        /// <summary>
        /// 状态码
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}