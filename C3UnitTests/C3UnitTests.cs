using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using C3;

namespace C3UnitTests
{
    [TestClass]
    public class C3UnitTests
    {
        protected C3Configuration config;
        protected const string bookExample = "C3UnitTests.Resources.BookExample1.txt";
        protected const string fullChargeList = "C3UnitTests.Resources.charges.csv";
        protected const string testResource1 = "C3UnitTests.Resources.August.csv";
        protected const string testResource2 = "C3UnitTests.Resources.BadHeaders.csv";
        protected const string chaseTestResource = "C3UnitTests.Resources.ChaseSample1.csv";
        protected const string invalidValueResource = "C3UnitTests.Resources.InvalidValue.csv";
        protected const string weirdColumnResource = "C3UnitTests.Resources.WeirdColumns.csv";
        
        [TestInitialize]
        public void Init()
        {
            this.config = new C3Configuration()
            {
                columns = new C3PredictedColumn[]
                {
                    new C3PredictedColumn() {
                        columnName = "Category",
                        classifierName = "NaiveBayesClassifier",
                        defaultValue = "UNSPECIFIED",
                        validValues = (new string[] { "CHAR", "CLOTHES", "COMM", "CULTURAL", "EATO", "EDU", "GIFTS", "GROC", "HOME", "MED", "NA", "TOYS", "TRANS", "TRAVEL", "UNSPECIFIED" }).ToHashSet(StringComparer.InvariantCultureIgnoreCase)
                    },
                    new C3PredictedColumn() {
                        columnName = "Owner",
                        classifierName = "NaiveBayesClassifier",
                        defaultValue = "UNSPECIFIED",
                        validValues = (new string[] { "ALICE", "BOB", "BOTH", "UNSPECIFIED" }).ToHashSet(StringComparer.InvariantCultureIgnoreCase)
                    }
                }
            };
        }
    }
}
