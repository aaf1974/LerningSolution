using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NetCodeExample.Examples
{
    //https://stackoverflow.com/questions/390578/creating-instance-of-type-without-default-constructor-in-c-sharp-using-reflectio
    class INstanceWitoutConstructorCall
    {
        internal static void PrintSample()
        {
            MyClass myClass = (MyClass)FormatterServices.GetUninitializedObject(typeof(MyClass)); //does not call ctor
            myClass.One = 1;
            Console.WriteLine(myClass.One); //write "1"
            Console.ReadKey();
        }
    }


    public class MyClass
    {
        public MyClass()
        {
            Console.WriteLine("MyClass ctor called.");
        }

        public int One
        {
            get;
            set;
        }
    }
}
