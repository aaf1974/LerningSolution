using NetCoreLibrary;
using System;

namespace NetCodeExample.Examples
{
    //https://habr.com/ru/company/microsoft/blog/423229/
    class SwitchSample
    {

        static int Sample1(object o)
        {
            switch(o)
            {
                case int n when n == 1: return 2;
                case int n: return 4;
                case string s: return 55;

                default: return -100; 
            }
        }

        public static void UsingSample1()
        {
            Console.WriteLine($"вызов:Sample1(1);  результат: {Sample1(1)}");
            Console.WriteLine($"вызов:Sample1(100);  результат: {Sample1(100)}");
            Console.WriteLine($"вызов:Sample1(\"Any text\");  результат: {Sample1("Any text")}");
            Console.WriteLine($"вызов:Sample1(new DateTime());  результат: {Sample1(new DateTime())}");
        }

        internal static void PrintSample()
        {
            CodeConsole.OutCode2Console(@"..\..\..\Examples\SwitchSample.cs", nameof(SwitchSample.Sample1));
            CodeConsole.OutCode2Console(@"..\..\..\Examples\SwitchSample.cs", nameof(SwitchSample.UsingSample1));
            CodeConsole.OutResultRun(UsingSample1);
        }
    }
}
