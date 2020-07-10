using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using NetCoreML.LoadSample.DataStruct;
using System;
using System.IO;
using System.Linq;

namespace NetCoreML
{
    //https://mlbootcamp.ru/ru/article/tutorial/
    internal class LoadSamplesMlSample
    {

        static string projectDataPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../", "LoadSample", "DataSets"));
        static string creditHistoryPath = Path.Combine(projectDataPath, "credit_x.data");

        internal static void Start()
        {
            CreditHistoryRun();
        }



        /// <summary>
        /// Какой то левый набор данных, хз что и как интерпретироавть
        /// </summary>
        private static void CreditHistoryRun()
        {
            var mlContext = new MLContext(seed: 0);
            IDataView loadedData = mlContext.Data.LoadFromTextFile<CreditHistoryInput>(creditHistoryPath, hasHeader: false, separatorChar: ',');

            var workingData = mlContext.Data.TrainTestSplit(loadedData, testFraction: 0.15, seed: 0);

            var testLoading = mlContext.Data.CreateEnumerable<CreditHistoryInput>(loadedData, true)
                .Take(3)
                .ToArray();

            var pipeline = ConcatFeaturesColumn(mlContext)
                .Append(mlContext.Transforms.CopyColumns("Label", nameof(CreditHistoryInput.A2)))
                .Append(mlContext.Transforms.CopyColumns("Score", nameof(CreditHistoryInput.A2)))
                .Append(mlContext.Regression.Trainers.FastForest(labelColumnName: "Label"))
            //.Append(mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: 3))
            //.Append(mlContext.Transforms.CopyColumns("Label", nameof(CreditHistoryInput.A2)))
            //.Append(mlContext.Transforms.CopyColumns("Score", nameof(CreditHistoryInput.A2)))
                .Append(mlContext.Transforms.ReplaceMissingValues(nameof(CreditHistoryInput.A2), replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean))
            ;
            TrainAndShowResult(mlContext, workingData, pipeline);

            pipeline
                .Append(mlContext.Transforms.Text.FeaturizeText(nameof(CreditHistoryInput.A1)))
                .Append(mlContext.Transforms.ReplaceMissingValues(nameof(CreditHistoryInput.A2), replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean))
                .Append(mlContext.Transforms.ReplaceMissingValues(nameof(CreditHistoryInput.A3), replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean))
                .Append(mlContext.Transforms.ReplaceMissingValues(nameof(CreditHistoryInput.A8), replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean))
                ;
            TrainAndShowResult(mlContext, workingData, pipeline);

            pipeline
                .Append(mlContext.Transforms.Text.FeaturizeText(nameof(CreditHistoryInput.A1)))
                .Append(mlContext.Transforms.Text.FeaturizeText(nameof(CreditHistoryInput.A4)))
                .Append(mlContext.Transforms.Text.FeaturizeText(nameof(CreditHistoryInput.A5)))
                .Append(mlContext.Transforms.Text.FeaturizeText(nameof(CreditHistoryInput.A6)))
                .Append(mlContext.Transforms.Text.FeaturizeText(nameof(CreditHistoryInput.A7)))
                .Append(mlContext.Transforms.Text.FeaturizeText(nameof(CreditHistoryInput.A9)))
                .Append(mlContext.Transforms.ReplaceMissingValues(nameof(CreditHistoryInput.A2), replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean))
                ;
            TrainAndShowResult(mlContext, workingData, pipeline);

            static ColumnConcatenatingEstimator ConcatFeaturesColumn(MLContext mlContext)
            {
                return mlContext.Transforms.Concatenate("Features",
                    nameof(CreditHistoryInput.A2),
                    nameof(CreditHistoryInput.A3),
                    nameof(CreditHistoryInput.A8)
                    );
            }
        }

        private static void TrainAndShowResult(MLContext mlContext, DataOperationsCatalog.TrainTestData workingData, 
            EstimatorChain<MissingValueReplacingTransformer> pipeline)
        {
            var model = pipeline.Fit(workingData.TrainSet);
            var predict = model.Transform(workingData.TestSet);
            var metric = mlContext.Regression.Evaluate(predict, "Label");

            ConsoleHelper.PrintRegressionMetrics("CreditHistoryRun - FastForest", metric);
        }
    }
}