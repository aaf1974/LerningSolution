using System;
using System.Linq;
using System.Reflection;

namespace NetDesignPatternSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select one is patterns or END for exite");

            Tools.GetValues<PatternEnum>()
                .ToList()
                .ForEach(x => Console.WriteLine($"{x} = {(int)x}"));

            var r = "begin";
            while (r.ToLower() != "end")
            {
                r = Console.ReadLine();

                int intVal;
                var parse = int.TryParse(r, out intVal);
                if (parse && Enum.IsDefined(typeof(PatternEnum), intVal))
                    //Console.WriteLine((PatternEnum)intVal);
                    PatternRunner.RunSample((PatternEnum)intVal);
            }
        }
    }
}
