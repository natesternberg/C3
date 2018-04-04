using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

using C3;
using C3.Readers;

namespace C3UnitTests
{
    [TestClass]
    public class USBankRecordReaderTests : C3UnitTests
    {
        [TestMethod]
        public void TestValidUSBankRecordFile()
        {
            var recordReader = new CsvRecordReader("USBank");
            Stream s = TestUtils.RetrieveResource(testResource1);
            var records = recordReader.ReadFromStream(s, this.config);

            Assert.AreEqual(records[0].PredictedValues.Count(), 2);
            Assert.AreEqual(records[0].Amount, 8.88m);
            Assert.AreEqual(records[0].Description, "WHOLEFDS BKN, #10220 BROOKLYN NY");      // test commas
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestUSBankRecordFileWithBadColumns()
        {
            var recordReader = new CsvRecordReader("USBank");
            Stream s = TestUtils.RetrieveResource(testResource2);
            var records = recordReader.ReadFromStream(s, this.config).ToList();
        }
    }
}