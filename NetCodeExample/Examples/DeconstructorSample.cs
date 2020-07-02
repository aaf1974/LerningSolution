using NetCoreLibrary;
using System;
using System.Collections.Generic;

namespace NetCodeExample.Examples
{
    //https://habr.com/ru/post/509082/
    class DeconstructorSample
    {
        public int Id { get; }

        public string Name { get; }

        public DeconstructorSample(int id, string name)
        {
            Id = id;
            Name = name;
        }


        public void Deconstruct(out int id, out string name) 
        {
            (id, name) = (Id, Name);
        }
    }


    class DeconstructorSampleUsing
    {
        public static void PrintSample()
        {
            //CodeConsole.OutCode2Console(@"..\..\..\Examples\DeconstructorDictSample.cs", nameof(DeconstructorDictSample.Sample));
            List<(string, string)> outed = new List<(string, string)>
            {
                (string.Empty, "Expected method implementation:"),
                (@"..\..\..\Examples\DeconstructorSample.cs", nameof(DeconstructorSample.Deconstruct)),
                (string.Empty, Environment.NewLine),
                (string.Empty, "пример использования:"),
                (@"..\..\..\Examples\DeconstructorSample.cs", nameof(DeconstructorSampleUsing.SampleUisng1))
            };

            CodeConsole.OutCode2Console(outed);
        }


        public static void SampleUisng1()
        {
            var sample = new DeconstructorSample(1, "name");
            (int x, string y) = sample;
        }
    }

}
