namespace ImportFromTempoToAnotherTool.Core.Extensions
{
    internal static class IntExtension
    {
        internal static TimeSpan? ConvertSecondsToHours(this int tempoInSeconds)
        {
            //Input: 10800
            //Output: {03:00:00}

            //Input: 2700
            //Output: {00:45:00}

            //Input: 6300
            //Output: {01:45:00}

            //Input: 9601
            //Output: {02:40:01.2000000}

            //Input: 1800
            //Output: {00:30:00}

            if (tempoInSeconds == 0)
                return null;

            var hours = TimeSpan.FromSeconds(tempoInSeconds);

            return hours;
        }
    }
}