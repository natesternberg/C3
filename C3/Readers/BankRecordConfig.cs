using CsvHelper.Configuration;

namespace C3.Readers
{
    public abstract class BankRecordConfig
    {
        // Class that maps bank's column definitions to a CCRecord
        public ClassMap<CCRecord> classMap;
        // Strings that, if present in the description, should be skipped
        public string[] recordsToSkip;
    }
}