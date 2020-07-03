using NetCoreML.SentimentAnalysis;
using System;

namespace NetCoreML
{
    class SampleRunner
    {
        internal static void RunSample(MlSampleEnum sample)
        {
            switch (sample)
            {
                case MlSampleEnum.SentimentAnalysis:
                    SentimentMlSample.Start();
                    break;


                default:
                    throw new NotImplementedException();
            }
        }
    }
}
