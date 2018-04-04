using CsvHelper.Configuration;

namespace C3.Readers
{
    // Maps fields from the Chase record format into a CCRecord object
    public sealed class ChaseRecordMap : ClassMap<CCRecord>
    {
        public ChaseRecordMap()
        {
            Map(m => m.TransactionTime).Name("Trans Date");
            Map(m => m.Description).Name("Description");
            Map(m => m.Amount).Name("Amount");
            Map(m => m.Amount).ConvertUsing(row => -1 * row.GetField<decimal>("Amount"));
        }
    }

    public class ChaseRecordConfig : BankRecordConfig
    {
        public ChaseRecordConfig()
        {
            this.classMap = new ChaseRecordMap();
            this.recordsToSkip = new string[] { "AUTOMATIC PAYMENT - THANK" };
        }
    }
}