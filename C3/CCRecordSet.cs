using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;

using CsvHelper;

namespace C3
{
    // Class to represent a set of CC records, and utility functions for serialization, transformation to/from DataTables, etc.
    public class CCRecordSet : IEnumerable<CCRecord>
    {
        private C3Configuration config;
        private List<CCRecord> records;

        private readonly Dictionary<string, Type> requiredHeaders = 
            new Dictionary<string, Type>() {
                { Consts.TRANSACTIONTIME, typeof(DateTime) },
                { Consts.DESCRIPTION , typeof(string) },
                { Consts.AMOUNT, typeof(decimal) },
            };
        private readonly Dictionary<string, Type> predictedHeaders;

        public IEnumerable<string> RequiredHeaderNames { get { return this.requiredHeaders.Keys; } }
        public IEnumerable<string> PredictedHeaderNames { get { return this.predictedHeaders.Keys; } }
        public IEnumerable<string> AllHeaderNames { get { return requiredHeaders.Keys.Concat(config.columns.Select(x => x.columnName)); } }

        internal CCRecordSet(C3Configuration config)
        {
            this.config = config ?? throw new InvalidOperationException($"{nameof(config)} may not be null.");
            this.predictedHeaders = this.config.columns
                .ToDictionary(c => c.columnName, t => typeof(string));
        }

        internal CCRecordSet(List<CCRecord> records, C3Configuration config) : this(config)
        {
            this.records = records ?? throw new InvalidOperationException($"{nameof(records)} may not be null.");
        }

        public static CCRecordSet FromFile(string filename, C3Configuration config)
        {
            if (string.IsNullOrEmpty(filename))
                throw new InvalidOperationException($"{nameof(filename)} must be specified.");
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return FromStream(fs, config);
            }
        }

        public static CCRecordSet FromStream(Stream stream, C3Configuration config)
        {
            using (var reader = new StreamReader(stream))
            {
                return FromReader(reader, config);
            }
        }

        private static CCRecordSet FromReader(TextReader reader, C3Configuration config)
        {
            var recordSet = new CCRecordSet(config);
            recordSet.records = recordSet.LoadCCRecordsFromTextReader(reader, config).ToList();
            return recordSet;
        }

        private IEnumerable<CCRecord> LoadCCRecordsFromTextReader(TextReader reader, C3Configuration config)
        {
            Utils.Log(LoggingSeverity.DEBUG, "Deserializing existing records");
            var csv = new CsvReader(reader);
            while (csv.Read())
            {
                var currentRecord = csv.GetRecord<CCRecord>();
                currentRecord.PredictedValues = config.columns
                    .ToDictionary(k => k.columnName, v => csv[v.columnName]);
                foreach (var column in config.columns)
                {
                    if (!column.validValues.Contains(csv[column.columnName]))
                    {
                        throw new InvalidDataException(
                            $"Specified input ({csv[column.columnName]}) is not a valid value for column {column.columnName}.");
                    }
                }
                yield return currentRecord;
            }
        }

        public void AddRecord(CCRecord ccRecord)
        {
            this.records.Add(ccRecord);
        }

        // We use datatables as a proxy object in WPF.  This is a functional but unsatisfying compromise.  
        // CCRecordSet.ToDataTable() creates them to be used for data binding in the GUI code, and .FromDataTable()
        // creates a CCRecordSet from the DataTable present in the UI.  Ideally we'd databind directly to the 
        // CCRecordSet, but it's not clear how to implement ObservableCollection for an object whose properties aren't 
        // known until runtime.  This solution is fine for small recordsets, but won't scale well.
        public DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            foreach (var requiredHeader in requiredHeaders)
            {
                dt.Columns.Add(new DataColumn(requiredHeader.Key, requiredHeader.Value));
            }
            foreach (var predictedHeader in predictedHeaders)
            {
                dt.Columns.Add(new DataColumn(predictedHeader.Key, predictedHeader.Value));
            }
            foreach (var record in this.records)
            {
                int i = 0;
                var dr = dt.NewRow();
                dr[i++] = record.TransactionTime;
                dr[i++] = record.Description;
                dr[i++] = record.Amount;
                foreach (var predictedHeader in predictedHeaders)
                {
                    dr[i++] = record.PredictedValues[predictedHeader.Key];
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static CCRecordSet FromDataTable(DataTable dt, C3Configuration config)
        {
            if (dt == null)
                throw new InvalidOperationException($"{nameof(dt)} may not be null.");
            List<CCRecord> tempRecords = new List<CCRecord>();
            foreach (DataRow dr in dt.Rows)
            {
                tempRecords.Add(new CCRecord(dr, config));
            }
            return new CCRecordSet(tempRecords, config);
        }

        public void SerializeToFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new InvalidOperationException($"{nameof(filename)} must be specified.");
            Utils.Log(LoggingSeverity.DEBUG, "Serializing records to disk to {0}", filename);
            if (File.Exists(filename))
            {
                Utils.Log(LoggingSeverity.WARN, "File {0} already exists, overwriting.", filename);
            }
            
            using (var writer = new StreamWriter(filename))
            {
                SerializeToFile(writer);
            }
        }

        private void SerializeToFile(TextWriter writer)
        {
            var csv = new CsvWriter(writer);
            csv.Configuration.RegisterClassMap<CCRecordClassMap>();
            csv.WriteHeader<CCRecord>();
            foreach (var predictedColumn in this.config.columns)
            {
                csv.WriteField<string>(predictedColumn.columnName);
            }
            csv.NextRecord();
            foreach (var record in this.records.OrderBy(rec => rec.TransactionTime))
            {
                csv.WriteRecord(record);
                foreach (var predictedColumn in this.config.columns)
                {
                    csv.WriteField<string>(record.PredictedValues[predictedColumn.columnName]);
                }
                csv.NextRecord();
            }
        }

        // Required for IEnumerable<CCRecord> 
        public IEnumerator<CCRecord> GetEnumerator()
        {
            return records.GetEnumerator();
        }

        // Also required for IEnumerable<CCRecord> 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}