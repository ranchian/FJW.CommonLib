using System;
using System.Linq;
using System.Collections.Generic;

namespace FJW.CommonLib.Encrypt
{
    /// <summary>
    /// 加密管理器
    /// </summary>
    public interface IEncryptManager
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">明文</param>
        /// <returns></returns>
        string EncryptData(string data);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">密文</param>
        /// <returns></returns>
        string DecryptData(string data);
    }

    /// <summary>
    /// 加密工厂 构造加密版本实例
    /// </summary>
    public class EncryptFactory
    {
        private static Dictionary<EncryptVersion, string> _EncryptVerDic = new Dictionary<EncryptVersion, string>()
        {
            {
                EncryptVersion.V1,"FJW.CommonLib.Encrypt.EncryptV1"
            },
            {
                EncryptVersion.AES,"FJW.CommonLib.Encrypt.AESEncodeHelper"
            }
        };

        public static IEncryptManager CreateEncryptManager(EncryptVersion ver, params object[] args)
        {
            if (_EncryptVerDic.ContainsKey(ver))
            {
                Type type = Type.GetType(_EncryptVerDic[ver]);
                if (type.IsInterface || !type.GetInterfaces().Contains(typeof(IEncryptManager)))
                    return null;

                IEncryptManager manager = Activator.CreateInstance(type, args) as IEncryptManager;
                return manager;
            }

            return null;
        }
    }

    /// <summary>
    /// 加解密版本枚举
    /// </summary>
    public enum EncryptVersion
    {
        /// <summary>
        /// 第一版加解密
        /// </summary>
        V1 = 1,
        /// <summary>
        /// AES加解密 
        /// </summary>
        AES = 2,
    }
}