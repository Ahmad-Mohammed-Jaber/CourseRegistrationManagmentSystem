using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Helpers
{
    public static class ScheduleHelper
    {
        private static readonly Dictionary<Class.DaysOfWeek, string> DayAbbreviations = new()
        {
            { Class.DaysOfWeek.Sunday, "Sun" },
            { Class.DaysOfWeek.Monday, "Mon" },
            { Class.DaysOfWeek.Tuesday, "Tue" },
            { Class.DaysOfWeek.Wednesday, "Wed" },
            { Class.DaysOfWeek.Thursday, "Thu" },
            { Class.DaysOfWeek.Friday, "Fri" },
            { Class.DaysOfWeek.Saturday, "Sat" }
        };

        public static string GetScheduleString(Class.DaysOfWeek schedule)
        {
            if (schedule == Class.DaysOfWeek.None)
            {
                return "None";
            }

            var days = DayAbbreviations
                .Where(d => (schedule & d.Key) == d.Key)
                .Select(d => d.Value);

            return string.Join(", ", days);
        }

        public static Class.DaysOfWeek ParseScheduleString(string scheduleString)
        {
            if (string.IsNullOrWhiteSpace(scheduleString))
            {
                return Class.DaysOfWeek.None;
            }

            Class.DaysOfWeek result = Class.DaysOfWeek.None;

            foreach (var day in scheduleString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                              .Select(d => d.Trim()))
            {
                if (Enum.TryParse(day, true, out Class.DaysOfWeek parsed))
                {
                    result |= parsed;
                    continue;
                }

                var match = DayAbbreviations.FirstOrDefault(x =>
                    x.Value.Equals(day, StringComparison.OrdinalIgnoreCase));

                if (!match.Equals(default(KeyValuePair<Class.DaysOfWeek, string>)))
                {
                    result |= match.Key;
                }
            }

            return result;
        }
    }
}