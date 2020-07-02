using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        
    }
}
