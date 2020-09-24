using NetCoreLibrary;
using System.Collections.Generic;

namespace NetCodeExample.Examples.EfSoftDelete
{
    class EfSoftDeleteSample
    {
        internal static void PrintSoftDeleteSample()
        {
            List<(string, string)> outed = new List<(string, string)>
            {
                (string.Empty, "пример использования:"), 
                (@"..\..\..\Examples\EfSoftDelete\SampleContext.cs", nameof(SampleContext.ApplaySoftDeleteFilter)),
                (@"..\..\..\Examples\EfSoftDelete\EfExtension.cs", nameof(EfExtension.SetSoftDeleteFilter))
            };

            CodeConsole.OutCode2Console(outed);
        }
    }
}
