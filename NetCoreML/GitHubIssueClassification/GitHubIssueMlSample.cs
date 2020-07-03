using Microsoft.ML;
using NetCoreML.SentimentAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetCoreML.GitHubIssueClassification
{
    //https://docs.microsoft.com/ru-ru/dotnet/machine-learning/tutorials/github-issue-classification
    class GitHubIssueMlSample
    {

        private static string _appPath => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

        private static string _trainDataPath => Path.Combine(_appPath, "GitHubIssueClassification", "Data", "issues_train.tsv");
        private static string _testDataPath => Path.Combine(_appPath, "GitHubIssueClassification", "Data", "issues_test.tsv");
        private static string _modelPath => Path.Combine(_appPath, "GitHubIssueClassification", "Models", "model.zip");


        //private static string _trainDataPath => Path.Combine(_appPath, "..", "..", "..", "GitHubIssueClassification", "Data", "issues_train.tsv");
        //private static string _testDataPath => Path.Combine(_appPath, "..", "..", "..", "GitHubIssueClassification", "Data", "issues_test.tsv");
        //private static string _modelPath => Path.Combine(_appPath, "..", "..", "..", "GitHubIssueClassification", "Models", "model.zip");

        private static MLContext _mlContext;
        private static PredictionEngine<GitHubIssue, IssuePrediction> _predEngine;
        private static ITransformer _trainedModel;
        static IDataView _trainingDataView;


        public static void Start()
        {
            _mlContext = new MLContext(seed: 0);

            _trainingDataView = _mlContext.Data.LoadFromTextFile<GitHubIssue>(_trainDataPath, hasHeader: true);
            var pipeline = ProcessData();

            var trainingPipeline = BuildAndTrainModel(_trainingDataView, pipeline);
            Evaluate(_trainingDataView.Schema);
            PredictIssue();

            Console.WriteLine($"=============== End run sumple ===============");
        }


        /// <summary>
        /// Загрузка данных
        /// </summary>
        /// <returns></returns>
        private static IEstimator<ITransformer> ProcessData()
        {
            var pipeline =
                //поэтому стоит использовать метод MapValueToKey() для преобразования столбца Area в числовой тип ключа столбца 
                //Label (формат, который принимают алгоритмы классификации) и добавить его как новый столбец набора данных:
                _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Area", outputColumnName: "Label")
                //вызовите mlContext.Transforms.Text.FeaturizeText, который преобразует столбцы текста (Title и Description) в 
                //числовой вектор для вызываемых TitleFeaturized и DescriptionFeaturized соответственно. 
                //Добавьте присвоение признаков для обоих столбцов в конвейере, используя следующий код:
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Title", outputColumnName: "TitleFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Description", outputColumnName: "DescriptionFeaturized"))
                //Последний шаг на этапе подготовки данных заключается в объединении всех столбцов признаков 
                //в столбце Features с помощью метода преобразования Concatenate()
                .Append(_mlContext.Transforms.Concatenate("Features", "TitleFeaturized", "DescriptionFeaturized"))
                //добавьте AppendCacheCheckpoint для кэширования DataView, которое может ускорить обучение при многократных итерациях по данным
                .AppendCacheCheckpoint(_mlContext);

            return pipeline;
        }


        /// <summary>
        /// Сборка и обучение модели
        /// </summary>
        /// <param name="trainingDataView"></param>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        private static IEstimator<ITransformer> BuildAndTrainModel(IDataView trainingDataView, IEstimator<ITransformer> pipeline)
        {
            //Добавьте алгоритм машинного обучения к определениям преобразований данных
            var trainingPipeline = pipeline.Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            //Обучите модель на основе данных splitTrainSet и получите обученную модель
            //Метод Fit() обучает модель путем преобразования набора данных и применения обучения
            _trainedModel = trainingPipeline.Fit(trainingDataView);
            //Класс PredictionEngine представляет собой удобный API, позволяющий передать один экземпляр данных и 
            //осуществить прогнозирование на его основе.
            _predEngine = _mlContext.Model.CreatePredictionEngine<GitHubIssue, IssuePrediction>(_trainedModel);


            //Прогнозирование с помощью обученной модели
            GitHubIssue issue = new GitHubIssue()
            {
                Title = "WebSockets communication is slow in my machine",
                Description = "The WebSockets communication used under the covers by SignalR looks like is going slow in my development machine.."
            };
            //Используйте функцию Predict(), которая создает прогноз по одной строке данных.
            var prediction = _predEngine.Predict(issue);
            Console.WriteLine($"=============== Single Prediction just-trained-model - Result: {prediction.Area} ===============");

            return trainingPipeline;
        }


        /// <summary>
        /// Оценка модели
        /// </summary>
        /// <param name="trainingDataViewSchema"></param>
        private static void Evaluate(DataViewSchema trainingDataViewSchema)
        {
            //загрузите тестовый набор данных
            var testDataView = _mlContext.Data.LoadFromTextFile<GitHubIssue>(_testDataPath, hasHeader: true);
            //Метод Evaluate() вычисляет метрики качества для модели на основе указанного набора данных
            var testMetrics = _mlContext.MulticlassClassification.Evaluate(_trainedModel.Transform(testDataView));

            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for Multi-class Classification model - Test Data     ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       MicroAccuracy:    {testMetrics.MicroAccuracy:0.###}");
            Console.WriteLine($"*       MacroAccuracy:    {testMetrics.MacroAccuracy:0.###}");
            Console.WriteLine($"*       LogLoss:          {testMetrics.LogLoss:#.###}");
            Console.WriteLine($"*       LogLossReduction: {testMetrics.LogLossReduction:#.###}");
            Console.WriteLine($"*************************************************************************************************************");

            //Сохранение модели в файл
            SaveModelAsFile(_mlContext, trainingDataViewSchema, _trainedModel);
        }


        /// <summary>
        /// Сохранение модели в файл
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="trainingDataViewSchema"></param>
        /// <param name="model"></param>
        private static void SaveModelAsFile(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            mlContext.Model.Save(model, trainingDataViewSchema, _modelPath);
        }


        /// <summary>
        /// Развертывание и прогнозирование с помощью модели
        /// </summary>
        private static void PredictIssue()
        {
            //Загрузите сохраненную модель в приложение
            ITransformer loadedModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);

            // GitHub для тестирования прогноза обученной модели в методе Predict, создав экземпляр GitHubIssue:
            GitHubIssue singleIssue = new GitHubIssue() 
            { 
                Title = "Entity Framework crashes", 
                Description = "When connecting to the database, EF is crashing" 
            };

            //создайте экземпляр PredictionEngine
            //Класс PredictionEngine представляет собой удобный API, позволяющий осуществить прогнозирование на основе единственного экземпляра данных. 
            //PredictionEngine не является потокобезопасным.
            _predEngine = _mlContext.Model.CreatePredictionEngine<GitHubIssue, IssuePrediction>(loadedModel);
            //Используйте PredictionEngine для прогнозирования метки области GitHub
            var prediction = _predEngine.Predict(singleIssue);

            Console.WriteLine($"=============== Single Prediction - Result: {prediction.Area} ===============");
        }
    }
}
