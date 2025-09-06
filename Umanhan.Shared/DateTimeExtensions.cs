using TimeZoneConverter;

namespace Umanhan.Shared
{
    public static class DateTimeExtensions
    {
        private static readonly TimeZoneInfo ManilaTimeZone = TZConvert.GetTimeZoneInfo("Asia/Manila");

        /// <summary>
        /// Converts UTC DateTime to Manila Time (Asia/Manila).
        /// </summary>
        public static DateTime ToManilaTime(this DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, ManilaTimeZone);
        }

        /// <summary>
        /// Converts UTC DateTime? to Manila Time (returns null if input is null).
        /// </summary>
        public static DateTime? ToManilaTime(this DateTime? utcDateTime)
        {
            return utcDateTime.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(utcDateTime.Value, ManilaTimeZone)
                : null;
        }

        /// <summary>
        /// Converts UTC DateOnly to Manila DateOnly.
        /// Since DateOnly doesn't support time zones, we assume the start of the day in UTC.
        /// </summary>
        public static DateOnly ToManilaTime(this DateOnly utcDateOnly)
        {
            DateTime utcDateTime = utcDateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            return DateOnly.FromDateTime(utcDateTime.ToManilaTime());
        }
    }
}
