using CsvHelper.Configuration;

namespace C3.Readers
{
    // Maps fields from the USBank record format into a CCRecord object
    public sealed class USBankRecordMap : ClassMap<CCRecord>
    {
        public USBankRecordMap()
        {
            Map(m => m.TransactionTime).Name("Date");
            Map(m => m.Description).Name("Name");
            Map(m => m.Amount).Name("Amount");
            Map(m => m.Amount).ConvertUsing(row => -1 * row.GetField<decimal>("Amount"));
        }
    }

    public class USBankRecordConfig : BankRecordConfig
    {
        public USBankRecordConfig() 
        {
            this.classMap = new USBankRecordMap();
            this.recordsToSkip = new string[] { "INTERNET PAYMENT THANK YOU", "ELECTRONIC PAYMENT" };
        }
    }
}