using NetCoreLibrary;
using NetCoreML.SentimentAnalysis;
using System;

namespace NetCoreML
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select one is patterns or END for exite");
            MlSampleEnum.GitHubIssueClassification.OutEnum2Console2();

            //Tools.OutEnum2Console<MlSampleEnum>();
            Run();

            SentimentMlSample.Start();
        }

        public static void Run()
        {
            var r = "begin";
            while (r.ToLower() != "end")
            {
                r = Console.ReadLine();

                int intVal;
                var parse = int.TryParse(r, out intVal);
                if (parse && Enum.IsDefined(typeof(MlSampleEnum), intVal))
                    SampleRunner.RunSample((MlSampleEnum)intVal);
                else
                    MlSampleEnum.GitHubIssueClassification.OutEnum2Console2();
            }
        }
    }
}
