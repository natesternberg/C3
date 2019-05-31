using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Data;

[assembly: InternalsVisibleTo("C3UnitTests")]
namespace C3
{
    public class CCRecord
    {
        public DateTime TransactionTime { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public Dictionary<string, string> PredictedValues { get; set; }

        public CCRecord() {}

        public CCRecord(DateTime currTransactionTime, string currDescription, decimal currAmount, Dictionary<string, string> predictedValues)
        {
            this.TransactionTime = currTransactionTime;
            this.Description = currDescription;
            this.Amount = currAmount;
            this.PredictedValues = predictedValues;
        }

        public CCRecord(DataRow dataRow, C3Configuration config)
        {
            if (dataRow == null)
            {
                throw new InvalidOperationException($"{nameof(dataRow)} must be provided.");
            }
            if (config == null)
            {
                throw new InvalidOperationException($"{nameof(config)} must be provided.");
            }
            this.TransactionTime = dataRow.Field<DateTime>(Consts.TRANSACTIONTIME);
            this.Description = dataRow.Field<string>(Consts.DESCRIPTION);
            this.Amount = dataRow.Field<decimal>(Consts.AMOUNT);
            var predictedValues = new Dictionary<string, string>();
            foreach (var column in config.columns)
            {
                predictedValues[column.columnName] = dataRow.Field<string>(column.columnName);
            }
            this.PredictedValues = predictedValues;
        }

        public override string ToString()
        {
            return String.Join(",", FormattedElements());
        }

        private List<string> FormattedElements()
        {
            return new List<string>()
            {
                this.TransactionTime.ToString(Consts.DATETIMEDISPLAYFORMAT),
                this.Description,
                this.Amount.ToString()
            }
            .Concat(this.PredictedValues.Values)
            .ToList();
        }

        public override bool Equals(object obj)
        {
            CCRecord other = (CCRecord)obj;
            return this.Amount == other.Amount &&
                this.Description == other.Description &&
                this.TransactionTime == other.TransactionTime &&
                Enumerable.SequenceEqual(this.PredictedValues, other.PredictedValues);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 23 + this.Amount.GetHashCode();
                hash = hash * 23 + this.Description.GetHashCode();
                hash = hash * 23 + this.TransactionTime.GetHashCode();
                foreach (string predictedValue in this.PredictedValues.Values)
                {
                    hash = hash * 23 + predictedValue.GetHashCode();
                }
            }
            return hash;
        }
    }

    // Compare records modulo the categorizations
    public class UncategorizedCCRecordComparer : IEqualityComparer<CCRecord>
    {
        public bool Equals(CCRecord x, CCRecord y)
        {
            return x.Amount == y.Amount && x.TransactionTime == y.TransactionTime && String.Equals(x.Description, y.Description);
        }

        public int GetHashCode(CCRecord rec)
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 23 + rec.Amount.GetHashCode();
                hash = hash * 23 + rec.Description.GetHashCode();
                hash = hash * 23 + rec.TransactionTime.GetHashCode();
            }
            return hash;
        }

    }
    
}


