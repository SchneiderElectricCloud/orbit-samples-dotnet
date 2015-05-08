using System;

namespace SampleOrbitEventListenerService.Extensions
{
    static class DateTimeExtensions
    {
        // See: http://stackoverflow.com/questions/7983441/unix-time-conversions-in-c-sharp
        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long AsUnixTimestampMillis(this DateTime value)
        {
            return (long)(value.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
        }
    }

    static class DateTimeOffsetExtensions
    {
        // See: http://stackoverflow.com/questions/7983441/unix-time-conversions-in-c-sharp
        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long AsUnixTimestampMillis(this DateTimeOffset value)
        {
            return (long)(value.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
        }
    }

    static class GeoServiceHelpers
    {
        // See: http://stackoverflow.com/questions/7983441/unix-time-conversions-in-c-sharp
        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return (UnixEpoch.AddMilliseconds(millis));
        }

        public static DateTimeOffset DateTimeOffsetFromUnixTimestampMillis(long millis)
        {
            return (UnixEpoch.AddMilliseconds(millis));
        }
    }
}