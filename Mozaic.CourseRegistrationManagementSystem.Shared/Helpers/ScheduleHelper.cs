using Mozaic.CourseRegistrationManagementSystem.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mozaic.CourseRegistrationManagementSystem.Shared.Helpers
{
    public static class ScheduleHelper
    {
        private static readonly Dictionary<DaysOfTheWeek, string> DayAbbreviations = new()
        {
            { DaysOfTheWeek.Sunday, "Sun" },
            { DaysOfTheWeek.Monday, "Mon" },
            { DaysOfTheWeek.Tuesday, "Tue" },
            { DaysOfTheWeek.Wednesday, "Wed" },
            { DaysOfTheWeek.Thursday, "Thu" },
            { DaysOfTheWeek.Friday, "Fri" },
            { DaysOfTheWeek.Saturday, "Sat" }
        };

        public static string GetScheduleString(DaysOfTheWeek schedule)
        {
            if (schedule == DaysOfTheWeek.None)
            {
                return "None";
            }

            var days = DayAbbreviations
                .Where(d => (schedule & d.Key) == d.Key)
                .Select(d => d.Value);

            return string.Join(", ", days);
        }

        public static DaysOfTheWeek ParseScheduleString(string scheduleString)
        {
            if (string.IsNullOrWhiteSpace(scheduleString))
            {
                return DaysOfTheWeek.None;
            }

            DaysOfTheWeek result = DaysOfTheWeek.None;

            foreach (var day in scheduleString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                              .Select(d => d.Trim()))
            {
                if (Enum.TryParse(day, true, out DaysOfTheWeek parsed))
                {
                    result |= parsed;
                    continue;
                }

                var match = DayAbbreviations.FirstOrDefault(x =>
                    x.Value.Equals(day, StringComparison.OrdinalIgnoreCase));

                if (!match.Equals(default(KeyValuePair<DaysOfTheWeek, string>)))
                {
                    result |= match.Key;
                }
            }

            return result;
        }
    }
}