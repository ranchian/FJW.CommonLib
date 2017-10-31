using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using FJW.CommonLib.IO;
using FJW.CommonLib.Cache;
using FJW.CommonLib.XHttp;
using FJW.CommonLib.Utils;
using FJW.CommonLib.Encrypt;
using FJW.CommonLib.Configuration;
using FJW.CommonLib.ExtensionMethod;
using FJW.DI;

namespace FJW.CommonLib.XService
{
    /// <summary>
    /// 服务引擎
    /// </summary>
    public class ServiceEngine
    {
        /// <summary>
        /// 调用其他接口的业务方法配置文件名
        /// </summary>
        const string MethodConfigFile = "Method.config";
        /// <summary>
        /// 业务方法配置文件名
        /// </summary>
        const string ServiceConfigFile = "Service.config";
        /// <summary>
        /// 业务方法字典
        /// </summary>
        private static readonly Dictionary<string, ServiceInfo> Dic = new Dictionary<string, ServiceInfo>();
        /// <summary>
        /// 参数验证字典
        /// </summary>
        private static readonly Dictionary<string, IValidationHandler> Valdic = new Dictionary<string, IValidationHandler>();
        /// <summary>
        /// 调用其他接口的业务方法配置字典
        /// </summary>
        private static readonly Dictionary<string, MethodNode> MethodDic = new Dictionary<string, MethodNode>();
        /// <summary>
        /// 请求开始执行Handler列表
        /// </summary>
        private static readonly List<IOnRequestStart> ReqSHandlerList = new List<IOnRequestStart>();
        /// <summary>
        /// 请求结束执行Handler列表
        /// </summary>
        private static readonly List<IOnRequestEnd> ReqEHandlerList = new List<IOnRequestEnd>();

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static ServiceEngine()
        {
            try
            {
                Logger.Info("ServiceEngine init start");
                
                //加载服务配置
                InitServiceConfig();

                //加载方法配置
                InitMethodConfig();

                Logger.Info("ServiceEngine init finished");
            }
            catch (Exception ex)
            {
                Logger.Error("ServiceEngine init failed", ex);
            }
        }

       
        /// <summary>
        /// 配置Service.Config
        /// </summary>
        private static void InitServiceConfig()
        {
            Logger.Info("InitServiceConfig"+ PathHelper.MergePathName(PathHelper.GetConfigPath(), ServiceConfigFile));
            var config = ConfigManager.GetObjectConfig<ServiceConfig>(PathHelper.MergePathName(PathHelper.GetConfigPath(), ServiceConfigFile));
            if (config == null)
            {
                Logger.Info("not find ServiceConfig");
                return;
            }
            Logger.Info("config.Services.Count:" + config.Services.Count);
            // 加载业务方法handler
            foreach (ServiceNode node in config.Services)
            {
                try
                {
                   
                    var typeFullName = node.Type;
                    Logger.Info("InitServiceConfig:" + typeFullName);
                    Type type = Type.GetType(typeFullName); 
                    if (type != null && (type.IsInterface || !type.GetInterfaces().Contains(typeof(IServiceHandler))))
                    {
                        Logger.Info("Not impl IServiceHandler:" + typeFullName);
                           continue;
                    }
                     

                    if (type == null) continue;
                    var handler = Activator.CreateInstance(type) as IServiceHandler;
                    var sInfo = new ServiceInfo
                    {
                        node = node,
                        handler = handler
                    };
                    Logger.Info("Dic Add:"+ node.MethodName);
                    Dic.Add(node.MethodName, sInfo);
                }
                catch (Exception ex)
                {
                    Logger.Error("MethodName:" + node.MethodName, ex);
                }
            }

            // 加载请求启动时要执行的handler
            foreach (var startType in config.ReqStartList)
            {
                try
                {
                    Type type = Type.GetType(startType);
                    if (type != null && (type.IsInterface || !type.GetInterfaces().Contains(typeof(IOnRequestStart))))
                        continue;

                    var handler = Activator.CreateInstance(type) as IOnRequestStart;
                    if (handler != null) ReqSHandlerList.Add(handler);
                    Logger.Debug("RequestStartHandler: {0} loaded.", startType);
                }
                catch (Exception ex)
                {
                    Logger.Error("load RequestStartHandler: {0} Failed. {1} {2}", startType, ex.Message, ex.StackTrace);
                }
            }

            // 加载请求结束时要执行的handler
            foreach (var endType in config.ReqEndList)
            {
                try
                {
                    Type type = Type.GetType(endType);
                    if (type.IsInterface || !type.GetInterfaces().Contains(typeof(IOnRequestEnd)))
                        continue;

                    var handler = Activator.CreateInstance(type) as IOnRequestEnd;
                    if (handler != null) ReqEHandlerList.Add(handler);
                    Logger.Debug("RequestEndHandler: {0} loaded.", endType);
                }
                catch (Exception ex)
                {
                    Logger.Error("load RequestEndHandler: {0} Failed. {1} {2}", endType, ex.Message, ex.StackTrace);
                }
            }

            //加载参数验证handler
            foreach (var vali in config.Validations)
            {
                try
                {
                    string typeFullName = vali.Type;
                    Type type = Type.GetType(typeFullName);
                    if (type.IsInterface || !type.GetInterfaces().Contains(typeof(IValidationHandler)))
                        continue;

                    var handler = Activator.CreateInstance(type) as IValidationHandler;
                    Valdic.Add(vali.ValName, handler);
                }
                catch (Exception ex)
                {
                    Logger.Error("ValName:" + vali.ValName, ex);
                }
            }
        }

        private static void InitMethodConfig()
        {
            MethodConfig config = ConfigManager.GetObjectConfig<MethodConfig>(PathHelper.MergePathName(PathHelper.GetConfigPath(), MethodConfigFile));
            if (config == null) return;
            foreach (MethodNode mn in config.Method)
                MethodDic.Add(mn.MethodName, mn);
        }

        /// <summary>
        /// 调用业务DLL中的业务方法
        /// </summary>
        /// <param name="req"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static XHttpRes Handle(XHttpReq req, HttpContext context)
        {
            Logger.Debug("ReqSHandlerList Count:" + ReqSHandlerList.Count);
            // 请求开始要执行的操作
            foreach (var handler in ReqSHandlerList)
            {
                try
                {
                    handler.Invoke(req, context);
                }
                catch (Exception ex)
                {
                    Logger.Error("RequestStartHandler Invoke Failed. ", ex);
                }
            }

            try
            {
                Logger.Debug("Handle Dic Count" + Dic.Count);
                if (!Dic.ContainsKey(req.M))
                {
                    Logger.Error(req.M + " 无效的方法名");
                    return XHttpRes.Exception((int)ServiceResultStatus.Error, "无效的方法名");
                }

                ServiceInfo sInfo = Dic[req.M];
                // 参数校验
                string errMsg;
                int status = ParameterValidation(req, sInfo.node, out errMsg);
                if (status != 0)
                    return XHttpRes.Exception((int)ServiceResultStatus.InvalidParameter, errMsg);

                string afterData;
                if (!EnOrDecrypt(req.D, req.Ie, req.E, false, out afterData))
                    return XHttpRes.Exception(1, "DecryptVersion not found.");

                Logger.Info("reqData:【" + req.ToJSON() + "】");
                ServiceRequest r = new ServiceRequest
                {
                    data = afterData,
                    deviceid = req.Did,
                    guid = req.G,
                    remoteip = WebHelper.GetIP(),
                    platform = req.P,
                    version = req.V,
                    mid = string.IsNullOrWhiteSpace(req.T) ? (req.Mid <= 0 ? 0 : req.Mid) : CacheManager.GetRedisCache("MemberTokenCache", req.T) == null ? "0".ToLong() : CacheManager.GetRedisCache("MemberTokenCache", req.T).ToLong(),
                };

                Logger.Info("rData:【" + r.ToJSON() + "】");

                ServiceResult d = sInfo.handler.Invoke(r);

                if (d.Status == ServiceResultStatus.Ok)
                {
                    string reData;
                    return EnOrDecrypt(d.Content, sInfo.node.IsEncyption, sInfo.node.EncyptionVer, true, out reData) ?
                        XHttpRes.Success(reData, req, ReqEHandlerList, sInfo.node.IsEncyption, sInfo.node.EncyptionVer) :
                        XHttpRes.Exception(1, "EncryptVersion not found.");
                }
                return XHttpRes.Exception((int)d.Status, d.ExceptionMessage, d.Content);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("call mehtod {0} failed.", req.M), ex);
                return XHttpRes.Exception((int)ServiceResultStatus.Error, ex.Message);
            }
        }

        /// <summary>
        /// 加解密
        /// </summary>
        /// <param name="data">加解密数据</param>
        /// <param name="ie">是否需要加密</param>
        /// <param name="ver">加解密版本</param>
        /// <param name="flg">加解密识别flg</param>
        /// <param name="afterData">加解密后的数据</param>
        /// <returns></returns>
        static bool EnOrDecrypt(string data, bool ie, string ver, bool flg, out string afterData)
        {
            afterData = string.Empty;
            if (string.IsNullOrWhiteSpace(data))
                return true;

            if (!ie)
            {
                afterData = data;
                return true;
            }

            EncryptVersion eVersion = EncryptVersion.V1;
            if (Enum.TryParse(ver, out eVersion))
            {
                IEncryptManager manager = EncryptFactory.CreateEncryptManager(eVersion);
                if (manager == null)
                    return false;

                if (flg)
                    afterData = manager.EncryptData(data);
                else
                    afterData = manager.DecryptData(data);

                return true;
            }
            return false;
        }

        /// <summary>
        /// 参数验证
        /// </summary>
        /// <param name="req">请求体</param>
        /// <param name="node">业务方法配置</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        static int ParameterValidation(XHttpReq req, ServiceNode node, out string errMsg)
        {
            int status = 0;
            errMsg = string.Empty;
            foreach (var valNode in node.SV)
            {
                if (Valdic.ContainsKey(valNode.ValName))
                {
                    status = Valdic[valNode.ValName].Invoke(req, valNode.Param, out errMsg);
                    if (status != 0)
                        break;
                }
            }
            return status;
        }

        /// <summary>
        /// 调用method.config中配置的接口，业务方法中使用
        /// </summary>
        /// <typeparam name="T">请求实体类型</typeparam>
        /// <param name="method">方法名</param>
        /// <param name="o">请求对象</param>
        /// <param name="mid">用户ID</param>
        /// <param name="timeoutMs">超时时间 _1分钟</param>
        /// <returns>返回实体</returns>
        public static ServiceResult Request<T>(string method, T o, long mid = 0, int timeoutMs = 60000)
        {
            if (!MethodDic.ContainsKey(method))
                throw new ArgumentException("method not config");

            string url = MethodDic[method].EntryPoint;

            var req = o as ServiceRequest;
            int p = 0;
            int v = 0;
            if (req != null)
            {
                p = req.platform;
                v = req.version;
            }
            string data = JsonHelper.JsonSerializer(o);
            return Request(url, method, data, mid, p, v, timeoutMs);
        }

        static ServiceResult Request(string url, string method, string data, long mid = 0, int platform = 0, int version = 0, int timeoutMs = 60000)
        {
            XHttpReq req = new XHttpReq
            {
                D = data,
                M = method,
                Mid = mid,
                P = platform,
                V = version
            };

            HttpResult r = XHttpHelper.httpPost(url, JsonHelper.JsonSerializer(req), "utf-8", timeoutMs);

            if (r.Code == 200)
            {
                XHttpRes res = JsonHelper.JsonDeserialize<XHttpRes>(r.Content);

                var s = (ServiceResultStatus)res.S;

                return new ServiceResult()
                {
                    Status = s,
                    Content = res.D,
                    ExceptionMessage = res.Es,
                };
            }
            return ServiceResult.Exception(ServiceResultStatus.Error, "http exception");
        }
    }
}