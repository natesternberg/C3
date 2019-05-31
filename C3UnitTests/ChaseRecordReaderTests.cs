using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using C3;
using C3.Readers;

namespace C3UnitTests
{
    [TestClass]
    public class ChaseRecordReaderTests : C3UnitTests
    {
        [TestMethod]
        public void TestValidChaseRecordFile()
        {
            var recordReader = new CsvRecordReader("Chase");
            Stream s = TestUtils.RetrieveResource(chaseTestResource);
            List<CCRecord> records = recordReader.ReadFromStream(s, this.config);
            Assert.AreEqual(records[0].PredictedValues.Count(), 2);
            Assert.AreEqual(records.Count, 21);
            Assert.AreEqual(records.Sum(r => r.Amount), 631.83m);
        }
    }
}
