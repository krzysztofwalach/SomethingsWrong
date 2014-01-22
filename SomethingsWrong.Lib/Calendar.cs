using System;
using System.Collections.Generic;

namespace SomethingsWrong.Lib
{
    public class Calendar
    {
        private static readonly IList<DateTime> FreeNonHolidayDays = BuildFreeNonHolidayDays();

        public static bool TimeIsInsideWorkingHours(DateTime time)
        {
            return new TimeSpan(8, 30, 0) <= time.TimeOfDay && time.TimeOfDay <= new TimeSpan(18, 0, 0);
        }

        public static bool IsHoliday(DateTime date)
        {
            return
                date.DayOfWeek == DayOfWeek.Saturday ||
                date.DayOfWeek == DayOfWeek.Sunday ||
                FreeNonHolidayDays.Contains(date.Date);
        }

        private static IList<DateTime> BuildFreeNonHolidayDays()
        {
            var results = new List<DateTime>();
            string[] strs =
                @"
1.1.2014
6.1.2014
21.4.2014
1.5.2014
2.5.2014
8.6.2014
19.6.2014
15.8.2014
11.11.2014
24.12.2014
25.12.2014
26.12.2014".Split('\n');

            foreach (var str in strs)
            {
                try
                {
                    results.Add(DateTime.Parse(str));
                }
                catch
                {
                }
            }
            return results;
        }
    }
}
