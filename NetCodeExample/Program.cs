using NetCoreLibrary;
using System;

namespace NetCodeExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select one is patterns or END for exite");
            Tools.OutEnum2Console<SampleEnum>();
            Run();
        }

        public static void Run()
        {
            var r = "begin";
            while (r.ToLower() != "end")
            {
                r = Console.ReadLine();

                int intVal;
                var parse = int.TryParse(r, out intVal);
                if (parse && Enum.IsDefined(typeof(SampleEnum), intVal))
                    SampleRunner.RunSample((SampleEnum)intVal);
                else
                    Tools.OutEnum2Console<SampleEnum>();
            }
        }
    }
}
