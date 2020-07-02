using NetCoreLibrary;
using System.Collections.Generic;

namespace NetCodeExample.Examples
{
    class DeconstructorDictSample
    {
        public static void PrintSample()
        {
            CodeConsole.OutCode2Console(@"..\..\..\Examples\DeconstructorDictSample.cs", nameof(DeconstructorDictSample.Sample));
        }


        public static void Sample()
        {
            //Classic implement
            var dictionary = new Dictionary<int, string>();
            foreach (KeyValuePair<int, string> valuePair in dictionary)
            {
                int i = valuePair.Key;
                string s = valuePair.Value;
            }


            //New version of code
            foreach (var (key, value) in new Dictionary<int, string> { [1] = "val1", [2] = "val2" })
            {
                int i = key;
                string s = value;
            }
        }

    }
}
