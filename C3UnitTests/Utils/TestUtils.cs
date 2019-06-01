using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using System.Text;

namespace C3
{
    public class TestUtils
    {
        public static Stream RetrieveResource(string embeddedResourceName)
        {
            System.Reflection.Assembly thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream s = thisAssembly.GetManifestResourceStream(embeddedResourceName);
            return s;
        }
        public static string RetrieveResourceAsString(string embeddedResourceName)
        {
            using (var reader = new StreamReader(RetrieveResource(embeddedResourceName), Encoding.ASCII))
            {
                return reader.ReadToEnd();
            }
        }

        public static C3PredictedColumn[] GetMockC3PredictedColumns()
        {
            var ownerVals = new string[] { "Alice", "Bob", "Both" }.ToHashSet();
            var categoryVals = new string[] { "CHAR", "CLOTHES", "COMM", "CULTURAL", "EATO", "EDU", "GIFTS", "GROC", "HOME", "MED", "NA", "TOYS", "TRANS", "TRAVEL", "UNSPECIFIED" }.ToHashSet();
            return new C3PredictedColumn[]
            {
                new C3PredictedColumn { columnName = "Owner", defaultValue = "Both", classifierName = "NaiveBayesClassifier", validValues = ownerVals },
                new C3PredictedColumn { columnName = "Category", defaultValue = "UNSPECIFIED", classifierName = "NaiveBayesClassifier", validValues = categoryVals }
            };
        }

        public static void AssertApproximatelyEqual(decimal a, decimal b, decimal epsilon = 0.0001m)
        {
            Assert.IsTrue(Math.Abs(a - b) < epsilon);
        }
    }
}