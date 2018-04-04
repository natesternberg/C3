using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

using C3;
using C3.Readers;

namespace C3UnitTests
{
    [TestClass]
    public class CCRecordSetTests : C3UnitTests
    {
        private static string serializationOutput = Path.GetTempFileName();

        [TestCleanup]
        private void Cleanup()
        {
            if (File.Exists(serializationOutput))
                File.Delete(serializationOutput);
        }

        [TestMethod]
        public void TestRecordSetFromBank()
        {
            var recordReader = new CsvRecordReader("USBank");
            Stream s = TestUtils.RetrieveResource(testResource1);
            var oldRecords = recordReader.ReadFromStream(s, this.config);
            
            CCRecordSet recordSet = new CCRecordSet(oldRecords, config);
            int creditCount = oldRecords.Count(x => x.Amount < 0);
            Assert.AreEqual(creditCount, 1);
            try
            {
                recordSet.SerializeToFile(serializationOutput);
                Assert.IsTrue(File.Exists(serializationOutput));
                CCRecordSet newRecordSet = CCRecordSet.FromFile(serializationOutput, config);
                CollectionAssert.AreEquivalent(recordSet.ToList(), newRecordSet.ToList());
            }
            finally
            {
                if (File.Exists(serializationOutput))
                    File.Delete(serializationOutput);
            }
        }

        [TestMethod]
        public void TestRecordSetFromCsv()
        {
            Stream s = TestUtils.RetrieveResource(fullChargeList);
            CCRecordSet recordSet = CCRecordSet.FromStream(s, config);

            recordSet.SerializeToFile(serializationOutput);
            Assert.IsTrue(File.Exists(serializationOutput));
            CCRecordSet newRecordSet = CCRecordSet.FromFile(serializationOutput, config);
            CollectionAssert.AreEquivalent(recordSet.ToList(), newRecordSet.ToList());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void TestRecordSetWithInvalidValues()
        {
            Stream s = TestUtils.RetrieveResource(invalidValueResource);
            CCRecordSet recordSet = CCRecordSet.FromStream(s, config);
        }
    }
}
