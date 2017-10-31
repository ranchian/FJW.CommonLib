using System;

namespace FJW.CommonLib.Utils
{
    /// <summary>
    /// 人民币大小写转换
    /// </summary>
    public class RmbCapitalization
    {
        private const string Dxsz = "零壹贰叁肆伍陆柒捌玖";
        private const string Dxdw = "毫厘分角元拾佰仟萬拾佰仟亿拾佰仟萬兆拾佰仟萬亿京拾佰仟萬亿兆垓";
        private const string Scdw = "元拾佰仟萬亿京兆垓";

        /// <summary>
        /// 转换整数为大写金额
        /// 最高精度为垓，保留小数点后4位，实际精度为亿兆已经足够了，理论上精度无限制，如下所示：
        /// 序号:...30.29.28.27.26.25.24  23.22.21.20.19.18  17.16.15.14.13  12.11.10.9   8 7.6.5.4  . 3.2.1.0
        /// 单位:...垓兆亿萬仟佰拾        京亿萬仟佰拾       兆萬仟佰拾      亿仟佰拾     萬仟佰拾元 . 角分厘毫
        /// 数值:...1000000               000000             00000           0000         00000      . 0000
        /// 下面列出网上搜索到的数词单位：
        /// 元、十、百、千、万、亿、兆、京、垓、秭、穰、沟、涧、正、载、极
        /// </summary>
        /// <param name="capValue">整数值</param>
        /// <returns>返回大写金额</returns>
        public static string ConvertIntToUppercaseAmount(string capValue)
        {
            var currCap = "";    //当前金额
            var capResult = "";  //结果金额
            var prevChar = -1;      //上一位的值
            var posIndex = 4;       //位置索引，从"元"开始
            try
            {
                if (Convert.ToDouble(capValue) == 0) return "";
                for (var i = capValue.Length - 1; i >= 0; i--)
                {
                    //当前位的值
                    int currChar = Convert.ToInt16(capValue.Substring(i, 1));
                    if (posIndex > 30)
                    {
                        //已超出最大精度"垓"。注：可以将30改成22，使之精确到兆亿就足够了
                        break;
                    }
                    if (currChar != 0)
                    {
                        //当前位为非零值，则直接转换成大写金额
                        currCap = Dxsz.Substring(currChar, 1) + Dxdw.Substring(posIndex, 1);
                    }
                    else
                    {
                        //防止转换后出现多余的零,例如：3000020
                        switch (posIndex)
                        {
                            case 4: currCap = "元"; break;
                            case 8: currCap = "萬"; break;
                            case 12: currCap = "亿"; break;
                            case 17: currCap = "兆"; break;
                            case 23: currCap = "京"; break;
                            case 30: currCap = "垓"; break;
                        }
                        if (prevChar != 0)
                        {
                            if (currCap != "")
                            {
                                if (currCap != "元") currCap += "零";
                            }
                            else
                            {
                                currCap = "零";
                            }
                        }
                    }
                    //对结果进行容错处理               
                    if (capResult.Length > 0)
                    {
                        var resultUnit = capResult.Substring(0, 1); //结果单位           
                        var currentUnit = Dxdw.Substring(posIndex, 1);//当前单位
                        if (Scdw.IndexOf(resultUnit) > 0)
                        {
                            if (Scdw.IndexOf(currentUnit) > Scdw.IndexOf(resultUnit))
                            {
                                capResult = capResult.Substring(1);
                            }
                        }
                    }
                    capResult = currCap + capResult;
                    prevChar = currChar;
                    posIndex += 1;
                    currCap = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("转换失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            return capResult;
        }

        /// <summary>
        /// 转换小数为大写金额
        /// </summary>
        /// <param name="capValue">小数值</param>
        /// <param name="addZero">是否增加零位</param>
        /// <returns>返回大写金额</returns>
        private static string ConvertDecToUppercaseAmount(string capValue, bool addZero)
        {
            var currCap = "";
            var capResult = "";
            var prevChar = addZero ? -1 : 0;
            var posIndex = 3;
            try
            {
                if (Convert.ToInt16(capValue) == 0) return "";
                for (var i = 0; i < capValue.Length; i++)
                {
                    int currChar = Convert.ToInt16(capValue.Substring(i, 1));
                    if (currChar != 0)
                    {
                        currCap = Dxsz.Substring(currChar, 1) + Dxdw.Substring(posIndex, 1);
                    }
                    else
                    {
                        if (Convert.ToInt16(capValue.Substring(i)) == 0)
                        {
                            break;
                        }
                        if (prevChar != 0)
                        {
                            currCap = "零";
                        }
                    }
                    capResult += currCap;
                    prevChar = currChar;
                    posIndex -= 1;
                    currCap = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("转换失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            return capResult;
        }

        /// <summary>
        /// 人民币大写金额
        /// </summary>
        /// <param name="value">人民币数字金额值</param>
        /// <returns>返回人民币大写金额</returns>
        public static string RMBAmount(double value)
        {
            string capResult;
            try
            {
                //格式化
                var capValue = string.Format("{0:f4}", value);
                //小数点位置
                var dotPos = capValue.IndexOf(".");
                //是否在结果中加"整"
                var addInt = Convert.ToInt32(capValue.Substring(dotPos + 1)) == 0;
                //是否在结果中加"负"
                var addMinus = capValue.Substring(0, 1) == "-";
                //开始位置
                var beginPos = addMinus ? 1 : 0;
                //整数
                var capInt = capValue.Substring(beginPos, dotPos);
                //小数
                var capDec = capValue.Substring(dotPos + 1);

                if (dotPos > 0)
                {
                    capResult = ConvertIntToUppercaseAmount(capInt) + ConvertDecToUppercaseAmount(capDec, Convert.ToDouble(capInt) != 0);
                }
                else
                {
                    capResult = ConvertIntToUppercaseAmount(capDec);
                }
                if (addMinus) capResult = "负" + capResult;
                if (addInt) capResult += "整";
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("转换失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            return capResult;
        }

        /// <summary>
        /// 金额格式化
        /// </summary>
        /// <param name="value">要格式化的数据</param>
        /// <param name="tag">保留的小数点位数</param>
        /// <returns></returns>
        public static string MoneyFormat(decimal value, int tag = 2)
        {
            try
            {
                if (value > 100000000)
                {
                    return Math.Round(value / 100000000, tag) + "亿元";
                }
                if (value > 10000000)
                {
                    return Math.Round(value / 10000000, tag) + "千万元";
                }
                if (value > 1000000)
                {
                    return Math.Round(value / 1000000, tag) + "百万元";
                }
                if (value > 10000)
                {
                    return Math.Round(value / 10000, tag) + "万元";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("转换失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            return Math.Round(value, tag) + "元";
        }

        /// <summary>
        /// 金额格式化
        /// </summary>
        /// <param name="value">要格式化的数据</param>
        /// <param name="tag">保留的小数点位数</param>
        /// <param name="unit">返回当前单位</param>
        /// <returns></returns>
        public static decimal MoneyFormatEx(decimal value, out string unit, int tag = 2)
        {
            try
            {
                if (value > 100000000)
                {
                    unit = "亿元";
                    return Math.Round(value / 100000000, tag);
                }
                if (value > 10000000)
                {
                    unit = "千万元";
                    return Math.Round(value / 10000000, tag);
                }
                if (value > 1000000)
                {
                    unit = "百万元";
                    return Math.Round(value / 1000000, tag);
                }
                if (value > 10000)
                {
                    unit = "万元";
                    return Math.Round(value / 10000, tag);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("转换失败，原因：{0} {1}", ex.Message, ex.StackTrace));
            }
            unit = "元";
            return Math.Round(value, tag);
        }
    }
}
