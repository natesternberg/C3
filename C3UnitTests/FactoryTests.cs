using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using C3.Readers;

namespace C3UnitTests
{
    [TestClass]
    public class FactoryTests
    {
        [TestMethod]
        public void CreateRecordReaders()
        {
            //var r1 = RecordReaderFactory.GetRecordReaderByName("USBank");
            //var r2 = RecordReaderFactory.GetRecordReaderByName("Chase");
            //Assert.IsTrue(r1 is USBankRecordReader);
            //Assert.IsTrue(r2 is ChaseRecordReader);
            var c1 = BankConfigFactory.GetByName("USBank");
            var c2 = BankConfigFactory.GetByName("Chase");
            Assert.IsTrue(c1 is USBankRecordConfig);
            Assert.IsTrue(c2 is ChaseRecordConfig);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CreateInvalidReaderName()
        {
            var r1 = BankConfigFactory.GetByName("foobar");
        }
    }
}
