using EnumsNET;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace NetCoreLibrary
{
    public static class Tools
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }


        public static void OutEnum2Console<T>()
        {
            GetValues<T>()
                .ToList()
                .ForEach(x => Console.WriteLine($"{x} = {Convert.ToInt32(x)}"));
        }






        //https://stackoverflow.com/questions/20338068/getting-a-specific-method-source-code-from-cs-file-at-runtime
        //https://stackoverflow.com/questions/31175881/unable-to-get-method-syntaxtree-parsefile-in-new-nuget-of-roslyn
        public static string GetMethodSourceCode(string filename, string methodName)
        {
            var path = filename;// @"C:\...\SomeFile.cs";
            using (var stream = File.OpenRead(path))
            {
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(stream), path: path);


                var root = syntaxTree.GetRoot();
                var method = root.DescendantNodes()
                                 .OfType<MethodDeclarationSyntax>()
                                 .Where(md => md.Identifier.ValueText.Equals(methodName))
                                 .FirstOrDefault();
                return method.ToString();
            }
        }



        public static void OutEnum2Console2<TEnum>(this TEnum val)
            where TEnum : struct, Enum
        {
            val.LtcGetEnumDictionary()
                .ToList()
                .ForEach(x => Console.WriteLine($"{Convert.ToInt32(x.Key)}: {x.Key.LtcGetDescription()}"));
        }


        public static Dictionary<TEnum, string> LtcGetEnumDictionary<TEnum>(this TEnum val)
            where TEnum : struct, Enum
        {
            Dictionary<TEnum, string> result = LtcGetEnumCollection<TEnum>()
                .ToList()
                .ToDictionary(x => x, x => x.LtcGetDescription(EnumFormat.Description, EnumFormat.DisplayName));

            return result;
        }


        public static IEnumerable<TEnum> LtcGetEnumCollection<TEnum>()
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }

        public static string LtcGetDescription<TEnum>(this TEnum val, EnumFormat format = EnumFormat.Description, EnumFormat format2 = EnumFormat.DisplayName)
            where TEnum : struct, Enum
        {
            return val.AsString(format, format2) ?? string.Empty;
        }


        //TODO: to library
        public static T[] GetRow<T>(this T[,] array, int row)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("Not supported for managed types.");

            if (array == null)
                throw new ArgumentNullException("array");

            int cols = array.GetUpperBound(1) + 1;
            T[] result = new T[cols];

            int size;

            if (typeof(T) == typeof(bool))
                size = 1;
            else if (typeof(T) == typeof(char))
                size = 2;
            else
                size = Marshal.SizeOf<T>();

            Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

            return result;
        }
    }
}
