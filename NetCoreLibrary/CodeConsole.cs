using System;
using System.Collections.Generic;
using System.Linq;


namespace NetCoreLibrary
{
    public static class CodeConsole
    {
        public static void OutCode2Console(string filename, string methodName)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Console.WriteLine(Tools.GetMethodSourceCode(filename, methodName));


            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }


        public static void OutCode2Console(IEnumerable<(string fileName, string methodName)> codeParts)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            codeParts.ToList().ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.fileName) || string.IsNullOrEmpty(x.methodName))
                        Console.WriteLine(x.fileName, x.methodName);
                    else
                        Console.WriteLine(Tools.GetMethodSourceCode(x.fileName, x.methodName));
                }
            );


            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void OutResultRun(Action usingSample)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Blue;

            usingSample();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }


        public static void WriteLineColor(string message, ConsoleColor bakgroundColor, ConsoleColor foregroundColor)
        {
            var backColor = Console.BackgroundColor;
            var foreColor = Console.ForegroundColor;

            Console.BackgroundColor = bakgroundColor;
            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(message);

            Console.BackgroundColor = backColor ;
            Console.ForegroundColor = foreColor;
        }
    }
}
