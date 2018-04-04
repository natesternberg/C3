using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using System.Runtime.CompilerServices;

using C3.Classifiers;
using C3.Readers;

[assembly: InternalsVisibleTo("C3UnitTests")]
namespace C3
{
    public class Updater
    {
        /// <summary>
        /// Deserialize the old records from disk, read new records from file.  Predict the category of the new records, 
        /// append them to the old records, write the whole thing back to disk.
        /// </summary>
        public static void ClassifyAndUpdate(string oldFileName, string newFileName, IRecordReader reader, C3Configuration config)
        {
            CCRecordSet updatedRecords;
            using (FileStream oldFs = new FileStream(oldFileName, FileMode.Open, FileAccess.Read))
            {
                using (FileStream newFs = new FileStream(newFileName, FileMode.Open, FileAccess.Read))
                {
                    updatedRecords = ClassifyAndUpdate(oldFs, newFs, reader, config);
                }
            }
            updatedRecords.SerializeToFile(oldFileName);
        }

        internal static CCRecordSet ClassifyAndUpdate(Stream oldFileStream, Stream newFileStream, IRecordReader reader, C3Configuration config)
        {
            CCRecordSet oldRecordSet = CCRecordSet.FromStream(oldFileStream, config);
            List<CCRecord> newRecords = reader.ReadFromStream(newFileStream, config);
            return ClassifyAndUpdate(oldRecordSet, newRecords, reader, config);
        }

        internal static CCRecordSet ClassifyAndUpdate(CCRecordSet oldRecordSet, List<CCRecord> newRecords, 
            IRecordReader reader, C3Configuration config)
        {
            var classifers = new Dictionary<C3PredictedColumn, IClassifier<string>>();
            foreach (C3PredictedColumn predictedColumn in config.columns)
            {
                var trainingData = oldRecordSet
                    .Select(rec => new KeyValuePair<string, string>(rec.Description, rec.PredictedValues[predictedColumn.columnName]))
                    .ToList();

                var classifier = ClassifierFactory.GetClassifierByName<string>(predictedColumn.classifierName);
                Utils.Log(LoggingSeverity.DEBUG, 
                    $"Training {predictedColumn.classifierName} on column '{predictedColumn.columnName}' with {trainingData.Count} records");
                classifier.Train(trainingData);
                classifers.Add(predictedColumn, classifier);
            }

            AppendRecords(oldRecordSet, newRecords, classifers);
            return oldRecordSet;
        }

        internal static void AppendRecords(CCRecordSet oldRecords, List<CCRecord> newRecords, 
            Dictionary<C3PredictedColumn, IClassifier<string>> columnMap)
        {
            Utils.Log(LoggingSeverity.DEBUG, $"Classifying {newRecords.Count} new records");
            var recordHash = oldRecords.ToHashSet(new UncategorizedCCRecordComparer());

            foreach (var newRecord in newRecords)
            {
                if (recordHash.Contains(newRecord))
                {
                    throw new ApplicationException($"Attempt to add record {newRecord} to recordset, but equivalent record already exists.");
                }
                else
                {
                    foreach (var columnAndClassifier in columnMap)
                    {
                        var classification = columnAndClassifier.Value.Categorize(newRecord.Description);
                        newRecord.PredictedValues[columnAndClassifier.Key.columnName] = classification.Category;
                    }
                    oldRecords.AddRecord(newRecord);
                }
            }
        }
    }
}