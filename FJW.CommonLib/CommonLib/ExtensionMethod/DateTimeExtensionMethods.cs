﻿using System;

namespace FJW.CommonLib.ExtensionMethod
{
    /// <summary>
    /// DateTime的扩展方法
    /// </summary>
    public static class DateTimeExtensionMethods
    {
        /// <summary>
        /// 扩展方法：计算日期是年内的第几周,一周从星期天开始
        /// </summary>
        /// <param name="pCaller"></param>
        /// <returns></returns>
        public static int GetWeeksOfYear(this DateTime pCaller)
        {
            return pCaller.GetWeeksOfYear(DayOfWeek.Sunday);
        }

        /// <summary>
        /// 扩展方法：计算日期是年内的第几周
        /// </summary>
        /// <param name="pCaller">调用发起者</param>
        /// <param name="pFirstDayOfWeek">星期几算是一周的第一天</param>
        /// <returns></returns>
        public static int GetWeeksOfYear(this DateTime pCaller, DayOfWeek pFirstDayOfWeek)
        {
            try
            {
                var start = new DateTime(pCaller.Year, 1, 1);
                var margin = pCaller.DayOfYear;
                var firstWeekDays = (int)pFirstDayOfWeek - (int)start.DayOfWeek;//年内第一周的天数
                if (firstWeekDays <= 0)
                    firstWeekDays += 7;
                if (margin <= firstWeekDays)
                    return 1;
                else
                {
                    int remainer = 0;
                    int divisor = Math.DivRem(margin - firstWeekDays, 7, out remainer);
                    return 1 + divisor + (remainer > 0 ? 1 : 0);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetWeeksOfYear error：" + ex.Message);
            }
        }

        /// <summary>
        /// 扩展方法：计算日期是月内的第几周
        /// </summary>
        /// <param name="pCaller">调用发起者</param>
        /// <param name="pFirstDayOfWeek">星期几算是一周的第一天</param>
        /// <returns></returns>
        public static int GetWeeksOfMonth(this DateTime pCaller, DayOfWeek pFirstDayOfWeek)
        {
            try
            {
                var start = new DateTime(pCaller.Year, pCaller.Month, 1);
                var margin = pCaller.Day;
                var firstWeekDays = (int)pFirstDayOfWeek - (int)start.DayOfWeek;//月内第一周的天数
                if (firstWeekDays <= 0)
                    firstWeekDays += 7;
                if (margin <= firstWeekDays)
                    return 1;
                else
                {
                    int remainer = 0;
                    int divisor = Math.DivRem(margin - firstWeekDays, 7, out remainer);
                    return 1 + divisor + (remainer > 0 ? 1 : 0);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetWeeksOfMonth error：" + ex.Message);
            }
        }

        /// <summary>
        /// 扩展方法：获取指定日期所属月份的天数
        /// </summary>
        /// <param name="pCaller"></param>
        /// <returns></returns>
        public static int GetDaysOfMonth(this DateTime pCaller)
        {
            switch (pCaller.Month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 4:
                case 6:
                case 9:
                case 11:
                    return 30;
                case 2:
                    {
                        var year = pCaller.Year;
                        if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0))
                            return 29;
                        else
                            return 28;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static int ToUnixStr(this DateTime time)
        {
            try
            {
                var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                return (int)(time - startTime).TotalSeconds;
            }
            catch (Exception ex)
            {
                throw new Exception("ToUnixStr error：" + ex.Message);
            }
        }

        /// <summary>
        /// 转化为年月天时分秒
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToYMdhms(this DateTime dateTime)
        {
            return dateTime.Second + dateTime.Minute * 100 + dateTime.Hour * 10000 + dateTime.Day * 1000000L + dateTime.Month * 100000000L + dateTime.Year * 10000000000L;
        }
    }
}