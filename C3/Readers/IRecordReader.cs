using System.Collections.Generic;
using System.IO;

namespace C3.Readers
{
    public interface IRecordReader
    {
        List<CCRecord> ReadFromStream(Stream stream, C3Configuration config, bool hasHeader = true);
    }
}
