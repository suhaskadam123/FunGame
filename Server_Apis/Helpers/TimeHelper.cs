//namespace Server_Apis.Helpers
//{
//    public class TimeHelper
//    {
//    }
//}
namespace Server_Apis.Helpers
{
    public static class TimeHelper
    {
        public static DateTime GetIndiaTime()
        {
            TimeZoneInfo indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone);
        }

        public static string GetNextMinuteTime()
        {
            var now = GetIndiaTime();
            var nextMinute = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddMinutes(1);

            int hour = nextMinute.Hour % 12;
            if (hour == 0) hour = 12;
            string ampm = nextMinute.Hour >= 12 ? "PM" : "AM";

            return $"{hour}:{nextMinute.Minute.ToString("D2")} {ampm}";
        }
    }
}
