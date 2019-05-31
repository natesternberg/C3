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
        private readonly BankRecordConfig bankConfig;

        public CsvRecordReader(string bankName) 
        {
            this.bankName = bankName;
            this.bankConfig = BankConfigFactory.GetByName(this.bankName);
        }

        public List<CCRecord> ReadFromStream(Stream stream, C3Configuration config, bool hasHeader = true)
        {
            using (var reader = new StreamReader(stream))
            {
                return this.ReadFromTextReader(reader, config, hasHeader);
            }
        }

        private List<CCRecord> ReadFromTextReader(TextReader reader, C3Configuration config, bool hasHeader = true)
        {
            var csv = new CsvReader(reader);
            csv.Configuration.RegisterClassMap(this.bankConfig.classMap);
            var defaultValueDict = config.columns.ToDictionary(k => k.columnName, v => v.defaultValue);
            try
            {
                var results = csv.GetRecords<CCRecord>()
                    .Where(rec => !this.bankConfig.recordsToSkip.Contains(rec.Description))
                    .Select(rec => 
                    {
                        rec.PredictedValues = config.columns.ToDictionary(key => key.columnName, val => defaultValueDict[val.columnName]);
                        return rec;
                    })
                    .ToList();
                return results;
            }
            // here we convert the CsvHelper exception to a .NET exception so we don't need a CsvHelper reference in calling assemblies
            catch (BadDataException ex)
            {
                throw new FormatException(ex.Message);
            }
        }
    }
}