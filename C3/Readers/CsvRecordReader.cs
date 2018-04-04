using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CsvHelper;

namespace C3.Readers
{
    public class CsvRecordReader : IRecordReader
    {
        private readonly string bankName;

        public CsvRecordReader(string bankName) 
        {
            this.bankName = bankName;
        }
        
        public List<CCRecord> ReadFromStream(Stream stream, C3Configuration config, bool hasHeader = true)
        {
            using (var reader = new StreamReader(stream))
            {
                return this.ReadFromTextReader(reader, config, hasHeader);
            }
        }

        private bool IsValidRow(string desc)
        {
            return true;
        }

        private List<CCRecord> ReadFromTextReader(TextReader reader, C3Configuration config, bool hasHeader = true)
        {
            var csv = new CsvReader(reader);
            var bankConfig = BankConfigFactory.GetByName(bankName);
            csv.Configuration.RegisterClassMap(bankConfig.classMap);
            var defaultValueDict = config.columns.ToDictionary(k => k.columnName, v => v.defaultValue);
            try
            {
                var results = csv.GetRecords<CCRecord>()
                    .Where(rec => !bankConfig.recordsToSkip.Contains(rec.Description))
                    .Select(rec => 
                    {
                        rec.PredictedValues = config.columns.ToDictionary(key => key.columnName, val => defaultValueDict[val.columnName]);
                        return rec;
                    })
                    .ToList();
                return results;
            }
            // change CsvHelper exception to a .NET exception so we don't need to import CsvHelper reference in calling assemblies
            catch (BadDataException ex)
            {
                throw new FormatException(ex.Message);
            }
        }
    }
}