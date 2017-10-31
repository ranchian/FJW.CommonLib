using System;

using System.Runtime.CompilerServices;


namespace FJW.CommonLib.Utils
{
    /// <summary>
    /// Twitter的分布式自增ID算法Snowflake
    /// </summary>
    public class IdWorker
    {
        /// <summary>
        /// 机器
        /// </summary>
        private readonly long _workerId;

        /// <summary>
        /// 数据中心
        /// </summary>
        private readonly long _datacenterId;

        private const long Twepoch = 1288834974657L;
        private long _sequence;
        private const int WorkerIdBits = 5;
        private const int DatacenterIdBits = 5;
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        private const int SequenceBits = 12;
        private const int WorkerIdShift = SequenceBits;
        private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);
        private long _lastTimestamp = -1L;

        public IdWorker(long workerId, long datacenterId)
        {
            // sanity check for workerId  
            if (workerId > MaxWorkerId || workerId < 0)
                throw new ArgumentException(string.Format("worker Id 不能大于 {0} 且不能小于 0", MaxWorkerId));

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
                throw new ArgumentException(string.Format("datacenter Id 不能大于 {0} 且不能小于 0", MaxDatacenterId));

            _workerId = workerId;
            _datacenterId = datacenterId;
        }

        public IdWorker()
        {
            _workerId = 10;
            _datacenterId = 10;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public long NextId()
        {
            var timestamp = TimeGen();
            if (timestamp < _lastTimestamp)
                throw new Exception(String.Format(
                    "Clock moved backwards.  Refusing to generate id for {0} milliseconds", _lastTimestamp - timestamp));
            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0)
                    timestamp = TilNextMillis(_lastTimestamp);
            }
            else
                _sequence = 0L;
            _lastTimestamp = timestamp;

            return ((timestamp - Twepoch) << TimestampLeftShift) | (_datacenterId << DatacenterIdShift) |
                   (_workerId << WorkerIdShift) | _sequence;
        }

        private static long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
                timestamp = TimeGen();
            return timestamp;
        }

        private static long TimeGen()
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long) ts.TotalMilliseconds;
        }
    }
}
