using NetCoreLibrary;
using System;

namespace NetCoreKafkaSample
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
                new SimplePublisher().SendMessage();

                /*if (parse && Enum.IsDefined(typeof(SampleEnum), intVal))
                    SampleRunner.RunSample((SampleEnum)intVal);
                else
                    Tools.OutEnum2Console<SampleEnum>();*/
            }
        }
    }

    enum SampleEnum
    {
        SimpleGitHubSample = 1,
    }
}
