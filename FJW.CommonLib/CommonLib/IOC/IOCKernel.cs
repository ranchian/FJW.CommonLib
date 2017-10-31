//using System;
//using System.IO;
//using System.Reflection;
//using System.Threading.Tasks;
//using System.Collections.Generic;

//using FJW.CommonLib.IO;
//using FJW.CommonLib.Configuration;

//namespace FJW.CommonLib.IOC
//{
//    /// <summary>
//    /// IOC容器中依赖类的接口
//    /// </summary>
//    public interface IIOCObject
//    {
//        /// <summary>
//        /// 依赖类执行方法
//        /// </summary>
//        void Execute();
//    }

//    /// <summary>
//    /// IOC帮助类
//    /// </summary>
//    public class IOCKernel
//    {
//        /// <summary>
//        /// 依赖类字典
//        /// </summary>
//        private static Dictionary<string, IIOCObject> _InstanceDic = new Dictionary<string, IIOCObject>();

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        static IOCKernel()
//        {
//            /// 加载assmebly
//            {
//                String[] files = Directory.GetFiles(PathHelper.MapPath, "*.dll", SearchOption.TopDirectoryOnly);

//                Dictionary<string, Assembly> l = new Dictionary<string, Assembly>();

//                foreach (string file in files)
//                {
//                    Assembly a = Assembly.LoadFile(file);
//                    l.Add(file, a);
//                }
//            }

//            /// 加载依赖配置
//            string filePath = PathHelper.MergePathName(PathHelper.GetConfigPath(), "IOC.config");
//            IOCConfig config = ConfigManager.GetObjectConfig<IOCConfig>(filePath);
//            if (config != null)
//            {
//                foreach (var node in config.IOCList)
//                    try
//                    {
//                        IIOCObject handler = CreateInstance(node.Type) as IIOCObject;
//                        Type t = handler.GetType();
//                        foreach (var prop in node.PropertyList)
//                        {
//                            object p = CreateInstance(prop.Type);
//                            t.InvokeMember(prop.Name,
//                              BindingFlags.DeclaredOnly |
//                              BindingFlags.Public | BindingFlags.NonPublic |
//                              BindingFlags.Instance | BindingFlags.SetProperty, null, handler, new Object[] { p });
//                        }
//                        _InstanceDic.Add(node.Name, handler);
//                    }
//                    catch (Exception ex)
//                    { }
//            }
//        }

//        /// <summary>
//        /// 获取指定依赖类实例
//        /// </summary>
//        /// <param name="name">依赖类名称</param>
//        /// <returns>依赖类实例</returns>
//        public object GetInstance(string name)
//        {
//            if (_InstanceDic.ContainsKey(name))
//                return _InstanceDic[name];

//            return null;
//        }

//        /// <summary>
//        /// 执行所有依赖类
//        /// </summary>
//        public void ExecuteAll()
//        {
//            if (_InstanceDic == null)
//                return;

//            foreach (var o in _InstanceDic.Keys)
//            {
//                Task.Factory.StartNew(() =>
//                {
//                    _InstanceDic[o].Execute();
//                });
//            }
//        }

//        /// <summary>
//        /// 内部方法:创建实例
//        /// </summary>
//        /// <param name="typeFullName"></param>
//        /// <returns></returns>
//        private static object CreateInstance(string typeFullName)
//        {
//            Type type = Type.GetType(typeFullName);
//            if (!type.IsInterface)
//                return Activator.CreateInstance(type);
//            return null;
//        }
//    }

//    #region IOC配置
//    /// <summary>
//    /// IOC配置项
//    /// </summary>
//    public class IOCNode
//    {
//        /// <summary>
//        /// 依赖类名称
//        /// </summary>
//        [Node]
//        public string Name { get; set; }
//        /// <summary>
//        /// 依赖类全名(命名空间.类名, 程序集文件名)
//        /// </summary>
//        [Node]
//        public string Type { get; set; }
//        /// <summary>
//        /// 属性注入方式，属性列表
//        /// </summary>
//        [Node("PropertyList/Property", NodeAttribute.NodeType.List)]
//        public List<IOCNode> PropertyList { get; set; }
//    }

//    /// <summary>
//    /// IOC配置列表
//    /// </summary>
//    public class IOCConfig
//    {
//        [Node("IOCList/IOC", NodeAttribute.NodeType.List)]
//        public List<IOCNode> IOCList { get; set; }
//    }
//    #endregion
//}
