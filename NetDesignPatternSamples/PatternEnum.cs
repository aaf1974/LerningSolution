using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetDesignPatternSamples
{
    enum PatternEnum
    {
        Visitor = 1
    }

    public static class Tools
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
