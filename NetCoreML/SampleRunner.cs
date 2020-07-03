using NetCoreML.GitHubIssueClassification;
using NetCoreML.IrisFlowerClustering;
using NetCoreML.SentimentAnalysis;
using NetCoreML.TaxiFarePrediction;
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

                case MlSampleEnum.GitHubIssueClassification:
                    GitHubIssueMlSample.Start();
                    break;


                case MlSampleEnum.TaxiFarePrediction:
                    TaxiFarePredictMlSample.Start();
                    break;

                case MlSampleEnum.IrisFlowerClustering:
                    IrisFlowerMlSample.Start();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
