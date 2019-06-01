using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.IO;
using System.Linq;

using C3;
using C3.Readers;
using C3View;

namespace C3UnitTests
{
    [TestClass]
    public class TransformTests : C3UnitTests
    {
        [TestMethod]
        public void TestGrouping()
        {
            Stream s = TestUtils.RetrieveResource(fullChargeList);
            CCRecordSet recordSet = CCRecordSet.FromStream(s, config);
            var periodSpec = Selectors.periodSpecifiers[Consts.PERIOD_SPECIFIER_MONTH];
            var aggregation = Selectors.aggreations[Consts.AGGREGATION_AVG];
            var predictedColumns = TestUtils.GetMockC3PredictedColumns();
            var report = Transforms.GetPeriodSummary(recordSet.ToDataTable(), periodSpec, predictedColumns[0], aggregation);

            var expectedHeaders = new DateTime[] { DateTime.Parse("1/1/2004 12:00:00 AM"), DateTime.Parse("2/1/2004 12:00:00 AM"), DateTime.Parse("3/1/2004 12:00:00 AM"), DateTime.Parse("4/1/2004 12:00:00 AM") };
            var actualHeaders = report.AsEnumerable().Select(row => row["Period start"]).Cast<DateTime>().ToArray();
            CollectionAssert.AreEqual(expectedHeaders, actualHeaders);
            TestUtils.AssertApproximatelyEqual((decimal)report.Rows[0][1], 65.73384615m);
            TestUtils.AssertApproximatelyEqual((decimal)report.Rows[1][2], 44.94272727m);
            TestUtils.AssertApproximatelyEqual((decimal)report.Rows[2][3], 32.17625m);
            TestUtils.AssertApproximatelyEqual((decimal)report.Rows[3][4], 111.4819697m);
        }

    }
}
