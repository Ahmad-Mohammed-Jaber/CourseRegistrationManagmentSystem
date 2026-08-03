using CourseRegistrationManagmentSystem.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseRegistrationManagmentSystem.Shared.Helpers
{
    public static class ScheduleHelper
    {
        /// <summary>
        /// Converts the DaysOfWeek flags enum to a human-readable comma-separated string.
        /// </summary>
        /// <param name="schedule">The schedule flags.</param>
        /// <returns>A string representation of the selected days.</returns>
        public static string GetScheduleString(Class.DaysOfWeek schedule)
        {
            if (schedule == Class.DaysOfWeek.None)
            {
                return "No days selected";
            }

            // Get all defined names in the enum and filter those that are set in the current flag
            var days = Enum.GetValues(typeof(Class.DaysOfWeek))
                           .Cast<Class.DaysOfWeek>()
                           .Where(d => d != Class.DaysOfWeek.None && (schedule & d) == d)
                           .Select(d => d.ToString());

            return string.Join(", ", days);
        }

        /// <summary>
        /// Converts a comma-separated string of days back into DaysOfWeek flags.
        /// </summary>
        /// <param name="scheduleString">The string of days (e.g., "Monday, Wednesday").</param>
        /// <returns>The corresponding DaysOfWeek flags.</returns>
        public static Class.DaysOfWeek ParseScheduleString(string scheduleString)
        {
            if (string.IsNullOrWhiteSpace(scheduleString))
            {
                return Class.DaysOfWeek.None;
            }

            var days = scheduleString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(d => d.Trim());

            Class.DaysOfWeek result = Class.DaysOfWeek.None;

            foreach (var day in days)
            {
                if (Enum.TryParse(typeof(Class.DaysOfWeek), day, true, out var parsedDay))
                {
                    result |= (Class.DaysOfWeek)parsedDay;
                }
            }

            return result;
        }
    }
}
