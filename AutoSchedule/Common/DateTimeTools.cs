using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Common
{
    public static class DateTimeTools
    {
        #region 获取系统当前时间的几个方法（返回时间+格式化后的时间字符串）

        /// <summary>
        /// 获取系统当前时间
        /// </summary>
        /// <returns>系统当前时间</returns>
        public static DateTime GetSysDateTimeNow()
        {
            Instant now = SystemClock.Instance.GetCurrentInstant();
            var shanghaiZone = DateTimeZoneProviders.Tzdb["Asia/Shanghai"];
            return now.InZone(shanghaiZone).ToDateTimeUnspecified();
        }

        /// <summary>
        /// 获取系统当前时间格式化字符串 24小时制 被格式化为 (yyyy-MM-dd HH:mm:ss.fff)
        /// </summary>
        /// <returns>系统当前格式化的时间字符串(yyyy-MM-dd HH:mm:ss.fff)</returns>
        public static string GetSysDateTimeNowStringYMD24HMSF()
        {
            return GetSysDateTimeNow().ToStringYMD24HMSF();
        }

        /// <summary>
        /// 获取系统当前时间格式化字符串 12小时制 被格式化为 (yyyy-MM-dd hh:mm:ss.fff)
        /// </summary>
        /// <returns>系统当前格式化的时间字符串(yyyy-MM-dd hh:mm:ss.fff)</returns>
        public static string GetSysDateTimeNowStringYMD12HMSF(this DateTime time)
        {
            return GetSysDateTimeNow().ToStringYMD12HMSF();
        }

        /// <summary>
        /// 获取系统当前时间格式化字符串 24小时制 被格式化为 (yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <returns>系统当前格式化的时间字符串(yyyy-MM-dd HH:mm:ss)</returns>
        public static string GetSysDateTimeNowStringYMD24HMS(this DateTime time)
        {
            return GetSysDateTimeNow().ToStringYMD24HMS();
        }

        /// <summary>
        /// 获取系统当前时间格式化字符串 12小时制 被格式化为 (yyyy-MM-dd hh:mm:ss)
        /// </summary>
        /// <returns>系统当前格式化的时间字符串(yyyy-MM-dd hh:mm:ss)</returns>
        public static string GetSysDateTimeNowStringYMD12HMS(this DateTime time)
        {
            return GetSysDateTimeNow().ToStringYMD12HMS();
        }

        /// <summary>
        /// 获取系统当前时间格式化字符串  被格式化为 (yyyy-MM-dd)
        /// </summary>
        /// <returns>系统当前格式化的时间字符串(yyyy-MM-dd)</returns>
        public static string GetSysDateTimeNowStringYMD(this DateTime time)
        {
            return GetSysDateTimeNow().ToStringYMD();
        }

        #endregion

        #region DateTime 扩展几个 格式方法

        /// <summary>
        /// 时间 格式化 24小时制 被格式化为  (yyyy-MM-dd HH:mm:ss.fff)
        /// </summary>
        /// <param name="time">被格式的时间</param>
        /// <returns>格式化后的时间字符串(yyyy-MM-dd HH:mm:ss.fff)</returns>
        public static string ToStringYMD24HMSF(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        /// <summary>
        /// 时间 格式化 12小时制 被格式化为  (yyyy-MM-dd hh:mm:ss.fff)
        /// </summary>
        /// <param name="time">被格式化时间</param>
        /// <returns>格式化后的时间字符串(yyyy-MM-dd hh:mm:ss.fff)</returns>
        public static string ToStringYMD12HMSF(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd hh:mm:ss.fff");
        }

        /// <summary>
        /// 时间 格式化 24小时制 被格式化为  (yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="time">被格式化时间</param>
        /// <returns>格式化后的时间字符串(yyyy-MM-dd HH:mm:ss)</returns>
        public static string ToStringYMD24HMS(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 时间 格式化 12小时制 被格式化为  (yyyy-MM-dd hh:mm:ss)
        /// </summary>
        /// <param name="time">被格式化时间</param>
        /// <returns>格式化后的时间字符串(yyyy-MM-dd hh:mm:ss)</returns>
        public static string ToStringYMD12HMS(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd hh:mm:ss");
        }

        /// <summary>
        /// 时间 格式化  被格式化为  (yyyy-MM-dd)
        /// </summary>
        /// <param name="time">被格式化时间</param>
        /// <returns>格式化后的时间字符串(yyyy-MM-dd)</returns>
        public static string ToStringYMD(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }

        #endregion

        #region 获取时间戳

        /// <summary>
        /// 获取时间戳(秒)
        /// </summary>
        /// <returns>秒时间戳</returns>
        public static long GetSecondTimestamp()
        {
            // 以1970-1-1 为时间开始 同系统当前时间的秒差值即为秒时间戳
            TimeSpan ts = GetSysDateTimeNow() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// 获取时间戳（毫秒）
        /// </summary>
        /// <returns>毫秒时间戳</returns>
        public static long GetMilliSecondTimestamp()
        {
            // 以1970-1-1 为时间开始 同系统当前时间的毫秒差值即为毫秒时间戳
            TimeSpan ts = GetSysDateTimeNow() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        #endregion

        #region 将一个时间戳转换为一个时间

        /// <summary>
        /// 将一个秒时间戳转换为时间格式(秒)
        /// </summary>
        /// <param name="secondTimestamp">秒时间戳</param>
        /// <returns>转换后的时间</returns>
        public static DateTime? SecondStampToDateTime(long secondTimestamp)
        {
            //  做一个简单的判断
            if (secondTimestamp <= 0)
            {
                return null;
            }

            // 以1970-1-1 为时间开始，通过计算与之的时间差，来计算其对应的时间
            DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(secondTimestamp).ToLocalTime();
            return dateTime;
        }

        /// <summary>
        /// 将一个字符串秒时间戳转换为时间格式(秒)
        /// </summary>
        /// <param name="secondTimestampStr">字符串秒时间戳</param>
        /// <returns>转换后的时间</returns>
        public static DateTime? SecondStampToDateTime(string secondTimestampStr)
        {
            // 如果为空，那么直接返回null
            if (string.IsNullOrEmpty(secondTimestampStr))
            {
                return null;
            }

            // 首先将字符串时间戳转换为数字
            long secondTimestamp = 0;
            long.TryParse(secondTimestampStr, out secondTimestamp);

            // 调用
            return SecondStampToDateTime(secondTimestamp);
        }

        /// <summary>
        /// 将一个字符串毫秒时间戳转换为时间格式(毫秒)
        /// </summary>
        /// <param name="secondTimestampStr">字符串毫秒时间戳</param>
        /// <returns>转换后的时间</returns>
        public static DateTime? MilliSecondStampToDateTime(long secondTimestamp)
        {
            //  做一个简单的判断
            if (secondTimestamp <= 0)
            {
                return null;
            }

            // 以1970-1-1 为时间开始，通过计算与之的时间差，来计算其对应的时间
            DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddMilliseconds(secondTimestamp).ToLocalTime();

            return dateTime;
        }

        /// <summary>
        /// 将一个毫秒时间戳转换为时间格式(毫秒)
        /// </summary>
        /// <param name="milliSecondStampStr">毫秒时间戳</param>
        /// <returns>转换后的时间</returns>
        public static DateTime? MilliSecondStampToDateTime(string milliSecondStampStr)
        {
            // 如果为空，那么直接返回null
            if (string.IsNullOrEmpty(milliSecondStampStr))
            {
                return null;
            }

            // 首先将字符串时间戳转换为数字
            long milliSecondStamp = 0;
            long.TryParse(milliSecondStampStr, out milliSecondStamp);

            // 调用
            return MilliSecondStampToDateTime(milliSecondStamp);
        }

        #endregion
    }
}
