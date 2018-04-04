using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace C3
{
    // DateTime extension methods
    public static class DateTimeExtensions
    {
        public static DateTime FloorToYear(this DateTime c)
        {
            return new DateTime(c.Year, 1, 1);
        }
        public static DateTime FloorToMonth(this DateTime c)
        {
            return new DateTime(c.Year, c.Month, 1);
        }
        public static DateTime FloorToWeek(this DateTime c)
        {
            return c.Date.AddDays(-1 * (int)c.DayOfWeek);
        }
        public static DateTime FloorToDay(this DateTime c, int days = 1)
        {
            return new DateTime(c.Ticks - (c.Ticks % ((long)days * 864000000000L)));
        }
        public static DateTime FloorToHour(this DateTime c, int hours = 1)
        {
            return new DateTime(c.Ticks - (c.Ticks % ((long)hours * 36000000000L)));
        }
        public static DateTime FloorToMinute(this DateTime c, int minutes = 1)
        {
            return new DateTime(c.Ticks - (c.Ticks % ((long)minutes * 600000000L)));
        }
        public static DateTime FloorToSecond(this DateTime c, int seconds = 1)
        {
            return new DateTime(c.Ticks - (c.Ticks % ((long)seconds * 10000000L)));
        }
    }

    public static class LinqExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            return new HashSet<T>(source, comparer);
        }

        public static bool ContainsDuplicates<T>(this IEnumerable<T> source) 
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return source.Count() != source.ToHashSet().Count();
        }
    }

    public enum LoggingSeverity { DEBUG, WARN, ERROR };

    public class Utils
    {
        public static void Output(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        public static void Log(LoggingSeverity severity, params string[] messageAndParams)
        {
            string[] messageParams = messageAndParams.Skip(1).ToArray();
            string message = String.Format(messageAndParams[0], messageParams);
            switch (severity)
            {
                case LoggingSeverity.DEBUG:
                    Output(message); break;
                case LoggingSeverity.WARN:
                    Output(message); break;
                case LoggingSeverity.ERROR:
                    Output(message); break;
                default:
                    break;
            }
        }
    }
}
