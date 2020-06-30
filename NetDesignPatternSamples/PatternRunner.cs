using NetDesignPatternSamples.PatternSamples.Visitor;
using System;

namespace NetDesignPatternSamples
{
    class PatternRunner
    {
        internal static void RunSample(PatternEnum pattern)
        {

            switch (pattern)
            {
                case PatternEnum.Visitor:
                    SatertVisitorsample();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private static void SatertVisitorsample()
        {
            new VisitorSample().Main();
        }
    }
}
