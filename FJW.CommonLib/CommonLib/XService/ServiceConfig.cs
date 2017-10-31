using System;
using System.Collections.Generic;
using FJW.CommonLib.Configuration;

namespace FJW.CommonLib.XService
{
    #region 调用接口的业务方法配置实体
    /// <summary>
    /// 调用接口的业务方法信息
    /// </summary>
    class MethodNode
    {
        [Node]
        public string MethodName { get; set; }

        [Node]
        public string EntryPoint { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    class MethodConfig
    {
        [Node("Methods/Method", NodeAttribute.NodeType.List)]
        public List<MethodNode> Method { get; set; }
    }
    #endregion

    #region 参数校验方法配置实体
    /// <summary>
    /// 参数校验方法配置信息
    /// </summary>
    class ValidationNode
    {
        [Node]
        public string ValName { get; set; }

        [Node]
        public string Type { get; set; }
    }

    class ServiceValidation
    {
        [Node]
        public string ValName { get; set; }

        private string param = "";
        [Node]
        public string Param
        {
            get { return param; }
            set { param = value; }
        }
    }
    #endregion

    #region 业务方法配置实体
    /// <summary>
    /// 业务方法配置信息
    /// </summary>
    class ServiceNode
    {
        [Node]
        public string MethodName { get; set; }

        [Node]
        public string Type { get; set; }

        [Node]
        public Boolean IsEncyption { get; set; }

        [Node]
        public string EncyptionVer { get; set; }

        [Node("Vals/SV", NodeAttribute.NodeType.List)]
        public List<ServiceValidation> SV { get; set; }
    }

    class ServiceConfig
    {
        [Node("Services/Service", NodeAttribute.NodeType.List)]
        public List<ServiceNode> Services { get; set; }

        [Node("Validations/Validation", NodeAttribute.NodeType.List)]
        public List<ValidationNode> Validations { get; set; }

        [Node("ReqStartList/ReqStart", NodeAttribute.NodeType.List)]
        public List<string> ReqStartList { get; set; }

        [Node("ReqEndList/ReqEnd", NodeAttribute.NodeType.List)]
        public List<string> ReqEndList { get; set; }
    }
    #endregion
}
