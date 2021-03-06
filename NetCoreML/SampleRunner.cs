﻿using NetCoreML.BikeDemandForecasting;
using NetCoreML.DeepLearningImageClassification;
using NetCoreML.GitHubIssueClassification;
using NetCoreML.IrisFlowerClustering;
using NetCoreML.MnistDigital;
using NetCoreML.MovieRecommender;
using NetCoreML.OnImageObjectDetection;
using NetCoreML.ProductSalesAnomalyDetection;
using NetCoreML.SentimentAnalysis;
using NetCoreML.TaxiFarePrediction;
using NetCoreML.TextClassificationTF;
using NetCoreML.TransferLearningTF;
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

                case MlSampleEnum.MovieRecommender:
                    MovieRecommenderMlSample.Start();
                    break;


                case MlSampleEnum.DeepLearningImageClassification:
                    ImageClassifierMlSample.Start();
                    break;

                case MlSampleEnum.TransferLearningTF:
                    TransferLerningMlSample.Start();
                    break;

                case MlSampleEnum.BikeDemand:
                    BikeDemandMlSample.Start();
                    break;

                case MlSampleEnum.ProductSalesAnomalyDetection:
                    ProductSalesAnomalyDetectionMlSample.Start();
                    break;

                case MlSampleEnum.OnImageObjectDetection:
                    OnImageObjectDetectionMlSample.Start();
                    break;

                case MlSampleEnum.TextClassificationTF:
                    TextClassificationTFMlSample.Start();
                    break;


                /*                case MlSampleEnum.FashionMnistTF:
                                    FashionMnistTFMlSample.Start();
                                    break;


                                case MlSampleEnum.DigitalMnist:
                                    DigitalMnistMlSample.Start();
                                    break;*/

                case MlSampleEnum.MnistDigital:
                    MnistDigitalMlSample.Start();
                    break;

                case MlSampleEnum.MnistDigital_GZ:
                    MnistDigitalGzMlSample.Start();
                    break;

                case MlSampleEnum.LoadSamples:
                    LoadSamplesMlSample.Start();
                    break;

                case MlSampleEnum.NaiveBayes:
                    NaiveBayesMlSample.Start();
                    break;


                default:
                    throw new NotImplementedException();
            }
        }
    }
}
