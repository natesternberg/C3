using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper;
using CsvHelper.Configuration;

namespace C3
{
    // Used by CSVHelper to specify the column formats while writing to disk
    public sealed class CCRecordClassMap : ClassMap<CCRecord>
    {
        public CCRecordClassMap()
        {
            AutoMap();
            Map(m => m.TransactionTime).ConvertUsing(rec => rec.TransactionTime.ToString(Consts.DATETIMEDISPLAYFORMAT));
        }
    }
}
