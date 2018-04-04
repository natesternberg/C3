using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C3
{
    public interface IClassifier<T>
    {
        void Train(List<KeyValuePair<string, T>> data);
        Classification<T> Categorize(string input);
    }
}
