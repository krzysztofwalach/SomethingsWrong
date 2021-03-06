﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace SomethingsWrong.Lib
{
    public class Calendar
    {
        private static readonly TimeSpan OfficeHoursStart = new TimeSpan(8, 20, 0);
        private static readonly TimeSpan OfficeHoursEnd = new TimeSpan(17, 0, 0);
        private static readonly IList<DateTime> FreeNonHolidayDays = BuildFreeNonHolidayDays();

        public static bool TimeIsInsideWorkingHours(DateTime time)
        {
            return OfficeHoursStart <= time.TimeOfDay && time.TimeOfDay <= OfficeHoursEnd;
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

            string[] strs = @"
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
26.12.2014".Replace("\n",",").Replace("\r", "").Split(',');

            foreach (var str in strs)
            {
                try
                {
                    results.Add(DateTime.ParseExact(str, "d.M.yyyy",CultureInfo.InvariantCulture));
                }
                catch
                {
                }
            }
            return results;
        }
    }
}
