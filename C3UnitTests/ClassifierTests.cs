using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using C3;
using C3.Classifiers;
using C3.Readers;

namespace C3UnitTests 
{
    public enum Genre { ENGINEERING, INTERIORDECORATING, ACCOUNTING };

    [TestClass]
    public class ClassifierTests : C3UnitTests
    {
        private static List<KeyValuePair<string, Genre>> KeyValuePairFromTsv(string resourceName)
        {
            string s = TestUtils.RetrieveResourceAsString(resourceName);
            var records = s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                .Select(rec => new KeyValuePair<string, Genre>(rec.Split('\t')[1], (Genre)Enum.Parse(typeof(Genre), rec.Split('\t')[0])))
                .ToList();
            return records;
        }

        private C3Configuration GetWeirdConfig()
        {
            return new C3Configuration()
            {
                columns = new C3PredictedColumn[]
                {
                    new C3PredictedColumn() {
                        columnName = "Fish",
                        classifierName = "NaiveBayesClassifier",
                        defaultValue = "Bass",
                        validValues = (new string[] { "Bass", "Trout", "Salmon" }).ToHashSet(StringComparer.InvariantCultureIgnoreCase)
                    },
                    new C3PredictedColumn() {
                        columnName = "Diety",
                        classifierName = "NaiveBayesClassifier",
                        defaultValue = "Odin",
                        validValues = (new string[] { "Odin", "Zeus", "Anubis" }).ToHashSet(StringComparer.InvariantCultureIgnoreCase)
                    },
                    new C3PredictedColumn() {
                        columnName = "Hayvan",
                        classifierName = "NaiveBayesClassifier",
                        defaultValue = "Kaplumbağa",
                        validValues = (new string[] { "Sivrisinek", "Kaplumbağa" }).ToHashSet(StringComparer.InvariantCultureIgnoreCase)
                    }

                }
            };
        }

        [TestMethod]
        public void NaiveBayesClassifierSimpleTest()
        {
            var records = KeyValuePairFromTsv(bookExample);
            var classifier = ClassifierFactory.GetClassifierByName<Genre>("NaiveBayesClassifier");
            classifier.Train(records);
            //NaiveBayesClassifier <Genre> nbc = new NaiveBayesClassifier<Genre>();

            Assert.AreEqual(classifier.Categorize("Curtains and Drapes").Category, Genre.INTERIORDECORATING);
            Assert.AreEqual(classifier.Categorize("The Ventilation of Bridges").Category, Genre.ENGINEERING);
            Assert.AreEqual(classifier.Categorize("Tax Accounting").Category, Genre.ACCOUNTING);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void DuplicateDetectionTest()
        {
            var recordReader = new CsvRecordReader("USBank");
            Stream stream = TestUtils.RetrieveResource(fullChargeList);
            CCRecordSet recordSet = CCRecordSet.FromStream(stream, config);
            var duplicateRecord = new List<CCRecord>() { recordSet.ToArray()[0] };
            Updater.ClassifyAndUpdate(recordSet, duplicateRecord, recordReader, config);
        }

        [TestMethod]
        public void NaiveBayesClassificationIntegrationTest()
        {
            Stream oldRecordsStream = TestUtils.RetrieveResource(fullChargeList);
            CCRecordSet records = CCRecordSet.FromStream(oldRecordsStream, config);
            var nbc = ClassifierFactory.GetClassifierByName<string>("NaiveBayesClassifier");
            var trainingData = records
                .Select(rec => new KeyValuePair<string, string>(rec.Description, rec.PredictedValues["Category"]))
                .ToList();
            nbc.Train(trainingData);
            Assert.AreEqual(nbc.Categorize("Trader Joe's").Category, "GROC");
            Assert.AreEqual(nbc.Categorize("Shell Oil 27440482209 Seattle Wa").Category, "TRANS");

            trainingData = records
                .Select(rec => new KeyValuePair<string, string>(rec.Description, rec.PredictedValues["Owner"]))
                .ToList();
            nbc = ClassifierFactory.GetClassifierByName<string>("NaiveBayesClassifier");
            nbc.Train(trainingData);
            Assert.AreEqual(nbc.Categorize("Radio Shack 00133652 Knoxville").Category, "Bob");
        }

        [TestMethod]
        public void WeirdColumnTest()
        {
            Stream weirdStream = TestUtils.RetrieveResource(weirdColumnResource);
            var weirdConfig = GetWeirdConfig();
            CCRecordSet recordSet = CCRecordSet.FromStream(weirdStream, weirdConfig);
            var nbc = ClassifierFactory.GetClassifierByName<string>("NaiveBayesClassifier");
            nbc.Train(recordSet.Select(rec => new KeyValuePair<string, string>(rec.Description, rec.PredictedValues["Fish"])).ToList());
            Assert.AreEqual(nbc.Categorize("Seattle Lounge").Category, "Trout");
        }
    }
}
