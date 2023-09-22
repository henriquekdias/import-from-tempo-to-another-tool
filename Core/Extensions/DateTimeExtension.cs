namespace ImportFromTempoToAnotherTool.Core.Extensions
{
    internal static class DateTimeExtension
    {
        internal static DateTime GetDayStartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        internal static bool IsOlderThan(this DateTime dt, DateTime compareTo)
        {
            int result = DateTime.Compare(dt, compareTo);

            if (result < 0)
                //It is earlier than;
                return true;
            else
                return false;
        }

        internal static bool IsNewerThan(this DateTime dt, DateTime compareTo)
        {
            int result = DateTime.Compare(dt, compareTo);

            if (result <= 0)
                //It is earlier than;
                return false;
            else
                return true;
        }

        internal static bool IsSameThan(this DateTime dt, DateTime compareTo)
        {
            int result = DateTime.Compare(dt, compareTo);

            if (result == 0)
                //It is same than;
                return true;
            else
                return false;
        }

        internal static string ConvertDateTimeToUTCStringFormat(this DateTime? dt)
        {
            //Input: {5/1/2023 8:00:00 AM}
            //Output: "2023-05-01T08:00:00.000Z"

            //Input: {5/3/2023 1:45:00 PM}
            //Output: "2023-05-03T13:45:00.000Z";

            //Input: {6/1/2023 2:00:00 PM}
            //Output: "2023-01-06T14:00:00.000Z"

            //"2023-05-05T11:00:00.000-03:00"; With timezone

            if(!dt.HasValue)
            {
                return string.Empty;
            }

            var startTogglFormat = dt.Value.ToString("yyyy-MM-ddTHH:mm:ss.000-03:00");

            return startTogglFormat;
        }
    }
}