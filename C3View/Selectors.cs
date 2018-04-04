using System;
using System.Collections.Generic;
using System.Linq;

using C3;

namespace C3View
{
    public static class Selectors
    {
        // Contents of the time specifier combo box
        public static Dictionary<string, DateTimeRange> timeFilters = new Dictionary<string, DateTimeRange>()
        {
            { Consts.TIMEEXPR_LASTMONTH, new DateTimeRange() { StartTime = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1), EndTime = DateTime.MaxValue } },
            { Consts.TIMEEXPR_CURRMONTH, new DateTimeRange() { StartTime = DateTime.Now.FloorToMonth(), EndTime = DateTime.MaxValue } },
            { Consts.TIMEEXPR_YTD, new DateTimeRange() { StartTime = DateTime.Now.FloorToYear(), EndTime = DateTime.MaxValue } },
            { Consts.TIMEEXPR_LASTYEAR, new DateTimeRange() { StartTime = new DateTime(DateTime.Now.AddYears(-1).Year, 1, 1), EndTime = new DateTime(DateTime.Now.Year, 1, 1) } },
            { Consts.TIMEEXPR_FOREVER, new DateTimeRange() { StartTime = DateTime.MinValue, EndTime = DateTime.MaxValue } }
        };

        // Contents of the time period specifier combo box
        public static Dictionary<string, Func<DateTime, DateTime>> periodSpecifiers = 
            new Dictionary<string, Func<DateTime, DateTime>>()
        {
            { Consts.PERIOD_SPECIFIER_DAY, (dt) => dt.FloorToDay() },
            { Consts.PERIOD_SPECIFIER_WEEK, (dt) => dt.FloorToWeek() },
            { Consts.PERIOD_SPECIFIER_MONTH, (dt) => dt.FloorToMonth() },
            { Consts.PERIOD_SPECIFIER_YEAR, (dt) => dt.FloorToYear() }
        };

        // Contents of the aggregation specifier combo box
        public static Dictionary<string, Func<IEnumerable<decimal>, decimal>> aggreations = 
            new Dictionary<string, Func<IEnumerable<decimal>, decimal>>()
        {
            { Consts.AGGREGATION_SUM, (elems) => elems.Sum() },
            { Consts.AGGREGATION_AVG, (elems) => elems.Average() },
            { Consts.AGGREGATION_MIN, (elems) => elems.Min() },
            { Consts.AGGREGATION_MAX, (elems) => elems.Max() }
        };
    }
}