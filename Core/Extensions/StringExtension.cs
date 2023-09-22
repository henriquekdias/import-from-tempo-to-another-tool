using System.Globalization;

namespace ImportFromTempoToAnotherTool.Core.Extensions
{
    internal static class StringExtension
    {
        internal static bool IsEqual(this string string1, string string2)
        {
            return string1.Equals(string2, StringComparison.InvariantCultureIgnoreCase);
        }

        internal static int ConvertTempoDurationStringToSecondsInt(this string? tempoDuration)
        {
            //Input: "1"
            //Output: 3600

            //Input: "3"
            //Output: 10800

            //Input: "0.75"
            //Output: 2700

            //Input: "1.75"
            //Output: 6300

            //Input: "2.667"
            //Output: 9601

            //Input: "0,5"
            //Output: 1800

            if (string.IsNullOrEmpty(tempoDuration))
                return 0;

            if (tempoDuration.Contains(','))
            {
                tempoDuration = tempoDuration.Replace(',', '.');
            }

            var lTempoDuration = Convert.ToInt32(tempoDuration);

            var togglTempo = lTempoDuration * 3600; //1 hour = 3600 seconds

            return togglTempo;
        }

        internal static TimeSpan? ConvertTempoDurationStringToTimeSpan(this string? tempoDuration)
        {
            //Input: "3"
            //Output: {03:00:00}

            //Input: "0.75"
            //Output: {00:45:00}

            //Input: "1.75"
            //Output: {01:45:00}

            //Input: "2.667"
            //Output: {02:40:01.2000000}

            //Input: "0,5"
            //Output: {00:30:00}

            if (string.IsNullOrEmpty(tempoDuration))
                return null;

            if (tempoDuration.Contains(','))
            {
                tempoDuration = tempoDuration.Replace(',', '.');
            }

            double dTempoDuration = 0;

            if (!Double.TryParse(tempoDuration, NumberStyles.Any, CultureInfo.InvariantCulture, out dTempoDuration))
                return null;

            var hours = TimeSpan.FromHours(dTempoDuration);

            return hours;
        }

        internal static DateTime? ConvertStartTempoStringToDateTime(this string tempoDate)
        {
            //Input: "2023-05-01 08:00"
            //Output: {5/1/2023 8:00:00 AM}

            //Input: "2023-05-03 13:45"
            //Output: {5/3/2023 1:45:00 PM}

            //Input: "2023-06-01 14:00"
            //Output: {6/1/2023 2:00:00 PM}

            DateTime dt;

            if (!DateTime.TryParse(tempoDate, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
            {
                return null;
            }

            return dt;
        }

        internal static string? ConvertTempoDateStringToUTCStringFormat(this string? tempoDate)
        {
            //Input: "2023-05-01 08:00"
            //Output: "2023-05-01T08:00:00.000Z"

            //Input: "2023-05-03 13:45"
            //Output: "2023-05-03T13:45:00.000Z";

            //Input: "2023-06-01 14:00"
            //Output: "2023-01-06T14:00:00.000Z"

            //"2023-05-05T11:00:00.000-03:00"; With timezone

            if (string.IsNullOrEmpty(tempoDate))
                return null;

            var dt = tempoDate.ConvertStartTempoStringToDateTime();

            if (dt == null)
                return null;

            var dateTime = dt.ConvertDateTimeToUTCStringFormat();

            return dateTime;
        }

        internal static string? GetClockifyEndDateTime(this string? startClockify, string? duration)
        {
            if(string.IsNullOrEmpty(startClockify))
                return null;

            if(string.IsNullOrEmpty(duration))
                return null;

            DateTime dtStart;

            if (!DateTime.TryParse(startClockify, CultureInfo.CurrentCulture, DateTimeStyles.None, out dtStart))
            {
                return null;
            }

            var tsDuration = duration.ConvertTempoDurationStringToTimeSpan();

            if(tsDuration == null)
                return null;

            if(!tsDuration.HasValue)
                return null;

            DateTime? dtEnd = dtStart.Add(tsDuration.Value);

            var sEnd = dtEnd.ConvertDateTimeToUTCStringFormat();

            return sEnd;
        }
    }
}