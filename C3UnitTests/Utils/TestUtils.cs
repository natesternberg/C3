using System.IO;
using System.Text;

namespace C3
{
    public class TestUtils
    {
        public static Stream RetrieveResource(string embeddedResourceName)
        {
            System.Reflection.Assembly thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream s = thisAssembly.GetManifestResourceStream(embeddedResourceName);
            return s;
        }
        public static string RetrieveResourceAsString(string embeddedResourceName)
        {
            using (var reader = new StreamReader(RetrieveResource(embeddedResourceName), Encoding.ASCII))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
