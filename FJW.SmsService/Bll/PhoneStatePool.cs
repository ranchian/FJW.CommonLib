using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FJW.SmsService.Bll
{
    /// <summary>
    /// 记录一段时间内发送过的手机号
    /// </summary>
    public static class PhoneStatePool
    {
        /// <summary>
        /// 记录手机发送时间
        /// </summary>
        private static readonly Dictionary<long, double> Dict = new Dictionary<long, double>();

        /// <summary>
        /// 满额手机记录的日期
        /// </summary>
        private static int _day = DateTime.Now.Day;

        /// <summary>
        /// 标准时间
        /// </summary>
        private static readonly DateTime TimeMark;

        /// <summary>
        /// 间隔时间
        /// </summary>
        private const int BlankSecond = 60;

        /// <summary>
        /// 发送次数满额手机号 - Change By Day
        /// </summary>
        private static readonly List<long> QuotaPhones = new List<long>();
        
        static PhoneStatePool()
        {
            Task.Run(() => SelfCheck());
            TimeMark = DateTime.Now;
        }

        /// <summary>
        /// 如果存在 就不允许发送
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static PhoneState Contains(long phone)
        {
            lock (QuotaPhones)
            {
                if (QuotaPhones.Contains(phone))
                {
                    return PhoneState.Quota;
                }
            }
            double n;
            lock (Dict)
            {
                if (!Dict.ContainsKey(phone))
                    return PhoneState.Normal;
                n = Dict[phone];
            }
            var time = DateTime.Now;
            var num = Seconds(time);
            return num - n > BlankSecond ? PhoneState.Normal : PhoneState.Blanking; 
        }

        /// <summary>
        /// 添加手机状态
        /// </summary>
        /// <param name="phone"></param>
        public static void AddPhoneState(long phone)
        {
            var time = DateTime.Now;
            var num = Seconds(time);
            lock (Dict)
            {
                Dict[phone] = num;
            }
        }

        /// <summary>
        /// 添加满额手机号
        /// </summary>
        /// <param name="phone"></param>
        public static void AddQuotaPhone(long phone)
        {
            lock (QuotaPhones)
            {
                QuotaPhones.Add(phone);
            }
        }

        /// <summary>
        /// 与标准时间的相差秒数
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static double Seconds(DateTime time)
        {
            return (time - TimeMark).TotalSeconds ;
        }

        /// <summary>
        /// 自检
        /// <remarks>不使用线程信号的原因是，执行一次后并不能保证全部清除。</remarks>
        /// </summary>
        private static void SelfCheck()
        {
            while (true)
            {
                var time = DateTime.Now.AddMinutes(-1);//1分钟前
                var num = Seconds(time);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(num);
                Console.ForegroundColor = ConsoleColor.White;
                lock (Dict)
                {
                    var keys = Dict.Where(it => it.Value < num).Select(it => it.Key).ToArray();
                    foreach (var it in keys)
                    {
                        Dict.Remove(it);
                    }
                }

                if (_day != DateTime.Now.Day)
                {
                    lock (QuotaPhones)
                    {
                        if (_day != DateTime.Now.Day)
                        {
                            _day = DateTime.Now.Day;
                            QuotaPhones.Clear();
                        }
                    }
                }
                Thread.Sleep(1000 * 1);
            }
        }
    }
}