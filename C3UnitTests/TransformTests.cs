using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.IO;
using System.Linq;

using C3;
using C3.Readers;

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
            // DataTable summarizedByMonth = recordSet.GetPeriodSummary(x => x.FloorToMonth(), config.columns[0]);
            // TODO Assert something?
        }

    }
}
