using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using C3;

namespace C3View
{
    public struct CategoryAndValue
    {
        public string Name { get; set; }
        public decimal Sum { get; set; }
    }

    public struct DateTimeRange
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public static class Transforms
    {
        // Generate the table to populate the Summary tab
        public static IEnumerable<CategoryAndValue> GetSummaryTable(DataTable currentDataTable, DateTimeRange range, string aggregateColumn)
        {
            var table = GetFilteredRecordSet(currentDataTable, range)
                .GroupBy(row => row.Field<string>(aggregateColumn))
                .Select(g => new CategoryAndValue()
                {
                    Name = g.Key,
                    Sum = g.Sum(r => r.Field<decimal>(Consts.AMOUNT))
                })
                .OrderBy(row => row.Name)
                .ToList();
            if (table.Count() > 0)
            {
                table.Add(new CategoryAndValue { Name = "Total", Sum = table.Sum(rec => rec.Sum) });
            }
            return table;
        }

        /// <summary>
        /// Generate the table to populate the Report tab, sort of a pivot table
        /// </summary>
        /// <param name="currentDataTable"></param>
        /// <param name="periodGrouper">Function to group datetimes into time ranges (i.e. generates row headers)</param>
        /// <param name="predictedColumn">Specifies which predictedColumn to use as the column headers</param>
        /// <param name="aggregation">Function to aggregate grouped prices (usually with Sum)</param>
        /// <returns>A DataTable to display in the Report tab</returns>
        public static DataTable GetPeriodSummary(DataTable currentDataTable, Func<DateTime, DateTime> periodGrouper,
            C3PredictedColumn predictedColumn, Func<IEnumerable<decimal>, decimal> aggregation)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var groups = currentDataTable.AsEnumerable()
                .GroupBy(row => new {
                    category = row.Field<string>(predictedColumn.columnName),
                    period = periodGrouper(row.Field<DateTime>(Consts.TRANSACTIONTIME))
                })
                .Select(rec => new { k = rec.Key, s = aggregation(rec.Select(row => row.Field<decimal>(Consts.AMOUNT))) })
                .ToDictionary(rec => rec.k, rec => rec.s);

            DataTable dt = new DataTable();
            dt.Columns.Add("Period start", typeof(DateTime));
            foreach (string c in predictedColumn.validValues)
            {
                dt.Columns.Add(c.ToString(), typeof(decimal));
            }
            dt.Columns.Add("Period total", typeof(decimal));
            var groupQuery = groups.Keys.Select(g => g.period).Distinct().OrderBy(g => g);
            foreach (DateTime period in groupQuery)
            {
                decimal periodTotal = 0M;
                DataRow dr = dt.NewRow();
                dr[0] = period;
                int i = 1;
                foreach (string category in predictedColumn.validValues)
                {
                    groups.TryGetValue(new { category, period }, out decimal total);
                    periodTotal += total;
                    dr[i++] = total;
                }
                dr[i] = periodTotal;
                dt.Rows.Add(dr);
            }
            System.Diagnostics.Debug.WriteLine($"{sw.ElapsedMilliseconds} ms elapsed in {nameof(GetPeriodSummary)}()");
            return dt;
        }

        internal static EnumerableRowCollection<DataRow> GetFilteredRecordSet(DataTable currentDataTable, DateTimeRange range)
        {
            return currentDataTable.AsEnumerable()
                .Where(row => row.Field<DateTime>(Consts.TRANSACTIONTIME) >= range.StartTime
                && row.Field<DateTime>(Consts.TRANSACTIONTIME) < range.EndTime)
                .OrderBy(row => row.Field<DateTime>(Consts.TRANSACTIONTIME));
        }
    }
}