using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using FJW.CommonLib.Utils;
using FJW.CommonLib.ExtensionMethod;

namespace FJW.CommonLib.Validation
{
    /// <summary>
    /// 身份证实体
    /// </summary>
    public class IDCardNo
    {
        /// <summary>
        /// 身份证号
        /// </summary>
        public string CardNo = string.Empty;

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday = DateTime.MinValue;

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex = string.Empty;

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age = -1;

        /// <summary>
        /// 地区
        /// </summary>
        public string Area = string.Empty;

        /// <summary>
        /// 是否是有效证件
        /// </summary>
        public bool isCard = false;
    }

    /// <summary>
    /// 信息验证相关帮助类
    /// </summary>
    public class ValidateHelper
    {
        /// <summary>
        /// 按照实体对象特性，验证实体对象，并按照特性给实体属性赋值默认值
        /// </summary>
        public static bool CheckEntity<T>(ref T t, out string errMsg) where T : class,new()
        {
            errMsg = string.Empty;
            Type type = typeof(T);
            //取实体的属性
            foreach (var propInfo in type.GetProperties())
            {
                //获取列名的值
                var columnValue = propInfo.GetValue(t, null);
                //获取当前属性的类型名称
                var columnTypeName = propInfo.PropertyType.Name.ToLower();
                //如果没有传递该参数
                if (columnValue == null)
                {
                    //检查是否有默认值，如果有，则赋值
                    object obj = DetectionRuleHelper.SetDefaultValue(propInfo);
                    if (obj != null)
                    {
                        columnValue = obj;
                        propInfo.SetValue(t, columnValue, null);
                    }
                }

                //验证是否符合规则
                errMsg = ParameterCalibration(propInfo, columnValue);
                if (!string.IsNullOrWhiteSpace(errMsg))
                    return false;
            }
            return true;
        }

        #region 参数校验 ParameterCalibration
        /// <summary>
        /// 参数校验
        /// </summary>
        private static string ParameterCalibration(PropertyInfo propInfo, object columnValue)
        {
            string em = string.Empty;
            //获取当前属性的特性
            var objAttar = propInfo.GetCustomAttributes(typeof(VerificationEntityAttribute), true);
            if (objAttar.Length <= 0) return em;
            var attar = objAttar[0] as VerificationEntityAttribute;
            if (attar == null) return em;

            //检测该属性是否符合规则
            var bl = DetectionRuleHelper.DetectionRule(attar.Type, columnValue);
            if (!bl)
            {
                string paramName = propInfo.Name;
                var descrAttrArr = propInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (descrAttrArr != null && descrAttrArr.Length > 0)
                {
                    var descrAttr = descrAttrArr[0] as DescriptionAttribute;
                    paramName = descrAttr.Description;
                }
                em = !string.IsNullOrEmpty(attar.ErrorMessage)
                    ? paramName + attar.ErrorMessage
                    : paramName + attar.Type.GetDescription();
            }

            return em;
        }
        #endregion

        /// <summary>
        /// 身份证校验
        /// </summary>
        public static IDCardNo CheckIDCard(string no)
        {
            IDCardNo result = new IDCardNo();
            if (!string.IsNullOrEmpty(no))
            {
                if (no.Length != 15 && no.Length != 18)
                {
                    return result;
                }

                //计算
                result.CardNo = no;
                DateTime birth = DateTime.MinValue;
                if (no.Length == 15)
                {

                    //新身份证号
                    string newIDCard = string.Empty;
                    string _cDate = "19" + no.Substring(6, 6);
                    bool isDate = DateTime.TryParseExact(_cDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out birth);
                    if (!isDate)
                    {

                        //身份证号码中所含日期不正确
                        return result;
                    }
                    int iS = 0;

                    //加权因子常数
                    int[] iW = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };

                    //校验码常数
                    string LastCode = "10X98765432";

                    newIDCard = no.Substring(0, 6);

                    //填在第6位及第7位上填上'1'，'9'两个数字
                    newIDCard += "19";
                    newIDCard += no.Substring(6, 9);

                    //进行加权求和
                    for (int i = 0; i < 17; i++)
                    {
                        iS += int.Parse(newIDCard.Substring(i, 1)) * iW[i];
                    }

                    //取模运算，得到模值
                    int iY = iS % 11;

                    //从LastCode中取得以模为索引号的值，加到身份证的最后一位，即为新身份证号。
                    newIDCard += LastCode.Substring(iY, 1);
                    no = newIDCard;
                }
                else
                {
                    string _cDate = no.Substring(6, 8);
                    birth = CommonUtils.ToDTFit(_cDate);

                    //身份证号码中所含日期不正确
                    if (birth == DateTime.MinValue)
                    {
                        return result;
                    }
                }
                result.Birthday = birth;
                result.CardNo = no;

                //数字验证
                long n = 0;
                if (long.TryParse(no.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(no.Replace('x', '0').Replace('X', '0'), out n) == false)
                {
                    return result;
                }
                Dictionary<int, string> area = new Dictionary<int, string>(){
                {11,"北京"}, {12,"天津"}, {13,"河北"}, {14,"山西"}, {15,"内蒙古"}, {21,"辽宁"}, 
                {22,"吉林"}, {23,"黑龙江"}, {31,"上海"}, {32,"江苏"}, {33,"浙江"}, {34,"安徽"}, 
                {35,"福建"}, {36,"江西"}, {37,"山东"}, {41,"河南"}, {42,"湖北"}, {43,"湖南"},
                {44,"广东"}, {45,"广西"}, {46,"海南"}, {50,"重庆"}, {51,"四川"}, {52,"贵州"},
                {53,"云南"}, {54,"西藏"}, {61,"陕西"}, {62,"甘肃"}, {63,"青海"}, {64,"宁夏"},
                {65,"新疆"}, {71,"台湾"}, {81,"香港"}, {82,"澳门"}, {91,"国外"}
            };

                //省份验证
                int areaId = 0;

                //身份证号不合法
                if (!int.TryParse(no.Substring(0, 2), out areaId) || !area.ContainsKey(areaId))
                {
                    return result;
                }
                result.Area = area[areaId];
                string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
                string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
                char[] Ai = no.Remove(17).ToCharArray();
                int sum = 0;
                for (int i = 0; i < 17; i++)
                {
                    sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
                }
                int y = -1;
                Math.DivRem(sum, 11, out y);

                //校验码验证
                if (arrVarifyCode[y] != no.Substring(17, 1).ToLower())
                {
                    return result;
                }
                result.Age = (int)(DateTime.Now.Date.Subtract(result.Birthday.Date).TotalDays / 365);
                int sexNum = int.Parse(no.Substring(16, 1));
                result.Sex = sexNum % 2 == 0 ? "女" : "男";

                //符合GB11643-1999标准
                result.isCard = true;
            }
            return result;
        }

        /// <summary>
        /// 身份证校验
        /// </summary>
        public static bool IsIDC(string no)
        {
            return CheckIDCard(no).isCard;
        }

        /// <summary>
        /// 验证是否为Email地址
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string strIn)
        {
            return Regex.IsMatch(strIn, @"^\w+((-w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 判断指定的对象是否是数字，性能高于正则表达式版本，但有功能限制，仅支持正数
        /// </summary>
        /// <param name="value">指定的对象</param>
        /// <returns>如果是数字，返回true，否则返回false</returns>
        public static bool IsValidNumber(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            else
            {
                string rule = "0123456789.";
                for (int i = 0; i < value.Length; i++)
                {
                    if (rule.IndexOf(value[i]) == -1)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 验证是否为正数
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidUInt(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[1-9]\d*|0$");
        }

        /// <summary>
        /// 验证是否为整数
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidInt(string strIn)
        {
            return Regex.IsMatch(strIn, @"^-?[1-9]\d*$");
        }

        /// <summary>
        /// 验证是否为小数
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidFloat(string strIn)
        {
            return Regex.IsMatch(strIn, @"^-?([1-9]\d*|0(?!\.0+$))\.\d+?$");
        }

        /// <summary>
        /// 验证是否为正的小数
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidUFloat(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[1-9]\d*.\d*|0.\d*[1-9]\d*$");
        }

        /// <summary>
        /// 验证是否为日期（匹配规则为：2013.12.23）
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidDate(string strIn)
        {
            DateTime dt;
            return DateTime.TryParse(strIn, out dt);
        }

        /// <summary>
        /// 验证是否为大写字母
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidUpperStr(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[A-Z]+$");
        }

        /// <summary>
        /// 验证是否为小写字母
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidLowerStr(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[a-z]+$");
        }

        /// <summary>
        /// 验证是否为色码值
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidColorCode(string strIn)
        {
            return Regex.IsMatch(strIn, @"^#[a-fA-F0-9]{6}$");
        }

        /// <summary>
        /// 验证是否为正确的手机号码
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsCellPhone(string strIn)
        {
            return Regex.IsMatch(strIn, @"^(1(([0-9][0-9])|(47)|[8][012356789]))\d{8}$");
        }

        /// <summary>
        /// 验证是否为正确的座机号码
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsTelephone(string strIn)
        {
            return Regex.IsMatch(strIn, @"^0\d{2,3}-\d{5,9}|0\d{2,3}-\d{5,9}$");
        }

        /// <summary>
        /// 验证是否为IP地址
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidIp(string strIn)
        {
            return Regex.IsMatch(strIn, @"^((?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d?\d))$");
        }

        /// <summary>
        /// 验证是否为中文
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsChinese(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[\u4e00-\u9fa5]+$");
        }

        /// <summary>
        /// 验证是否为有效的登录密码(6到16位任意字符)
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsLoginPwd(string strIn)
        {
            return Regex.IsMatch(strIn, @"^.{6,16}$");
        }

        /// <summary>
        /// 验证是否为有效的交易密码(6位数字)
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsTreadingPwd(string strIn)
        {
            return Regex.IsMatch(strIn, @"^\d{6}$");
        }

        /// <summary>
        /// 验证中文姓名
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsChineseName(string strIn)
        {
            return Regex.IsMatch(strIn, @"^[\u4E00-\u9FA5]+(?:((·|\.|\．)[\u4E00-\u9FA5]+))*$");
        }
    }
}