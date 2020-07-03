using Microsoft.ML;
using System;
using System.IO;

namespace NetCoreML.TaxiFarePrediction
{
    class TaxiFarePredictMlSample
    {
        static readonly string _trainDataPath = Path.Combine(Environment.CurrentDirectory, "TaxiFarePrediction", "Data", "taxi-fare-train.csv");
        static readonly string _testDataPath = Path.Combine(Environment.CurrentDirectory, "TaxiFarePrediction", "Data", "taxi-fare-test.csv");
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "TaxiFarePrediction", "Data", "Model.zip");


        public static void Start()
        {
            MLContext mlContext = new MLContext(seed: 0);
            var model = Train(mlContext, _trainDataPath);
            Evaluate(mlContext, model);
            TestSinglePrediction(mlContext, model);
        }

        /// <summary>
        /// Загрузка и преобразование данных;
        /// обучение модели;
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="dataPath"></param>
        /// <returns></returns>
        private static ITransformer Train(MLContext mlContext, string dataPath)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(dataPath, hasHeader: true, separatorChar: ',');
            var pipeline =
                //Используйте класс преобразования CopyColumnsEstimator, чтобы скопировать FareAmount
                mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "FareAmount")
                //Алгоритм, который обучает модель, принимает числовые признаки, поэтому значения категориальных 
                //данных (VendorId, RateCode и PaymentType) нужно преобразовать в числа (VendorIdEncoded, RateCodeEncoded и PaymentTypeEncoded). 
                //Для этого используйте класс преобразования OneHotEncodingTransformer
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "VendorIdEncoded", inputColumnName: "VendorId"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "RateCodeEncoded", inputColumnName: "RateCode"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PaymentTypeEncoded", inputColumnName: "PaymentType"))
                //Последний шаг на этапе подготовки данных заключается в объединении всех столбцов признаков в столбце Features с помощью класса 
                //преобразования mlContext.Transforms.Concatenate. По умолчанию алгоритм обучения обрабатывает только признаки, 
                //представленные в столбце Features.
                .Append(mlContext.Transforms.Concatenate("Features", "VendorIdEncoded", "RateCodeEncoded", "PassengerCount", "TripDistance", "PaymentTypeEncoded"))
                //Задача заключается в прогнозировании стоимости поездки в такси в Нью-Йорке. На первый взгляд может показаться, 
                //что плата зависит только от расстояния. Но службы такси в Нью-Йорке изменяют плату с учетом еще нескольких факторов, 
                //таких как наличие дополнительных пассажиров или оплата кредитной картой вместо наличных. 
                //Вы хотите спрогнозировать стоимость, которая является реальным значением, зависящим от других факторов в наборе данных. 
                //Для этого нужно выбрать задачу машинного обучения регрессия.
                .Append(mlContext.Regression.Trainers.FastTree());

            //Согласуйте модель с учебным dataview и получите обученную модель
            //Метод Fit() обучает модель путем преобразования набора данных и применения обучения.
            var model = pipeline.Fit(dataView);

            return model;
        }


        /// <summary>
        /// Оценка модели
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="model"></param>
        private static void Evaluate(MLContext mlContext, ITransformer model)
        {
            //Загрузите тестовый набор данных
            IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(_testDataPath, hasHeader: true, separatorChar: ',');

            //преобразуйте данные Test
            //Метод Transform() осуществляет прогнозирование для входных строк тестового набора данных
            var predictions = model.Transform(dataView);

            //Метод RegressionContext.Evaluate вычисляет метрики качества для PredictionModel на основе указанного набора данных.
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine("Коэффициент детерминации может иметь значения в диапазоне от 0 до 1. Чем ближе его значение к 1, тем лучше модель.");
            Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"Среднеквадратичное отклонение является одной из метрик, используемых для оценки регрессионной модели. Чем ниже это отклонение, тем лучше модель.");
            Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");
        }

        /// <summary>
        /// Использование модели для прогнозирования
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="model"></param>
        private static void TestSinglePrediction(MLContext mlContext, ITransformer model)
        {
            //Для прогнозирования цены используйте PredictionEngine
            var predictionFunction = mlContext.Model.CreatePredictionEngine<TaxiTrip, TaxiTripFarePrediction>(model);

            //Добавьте поездку для проверки прогнозирования стоимости обученной моделью с помощью метода TestSinglePrediction()
            var taxiTripSample = new TaxiTrip()
            {
                VendorId = "VTS",
                RateCode = "1",
                PassengerCount = 1,
                TripTime = 1140,
                TripDistance = 3.75f,
                PaymentType = "CRD",
                FareAmount = 0 // To predict. Actual/Observed = 15.5
            };

            //Функция Predict() создает прогноз по одному экземпляру данных
            var prediction = predictionFunction.Predict(taxiTripSample);
            Console.WriteLine($"**********************************************************************");
            Console.WriteLine($"Predicted fare: {prediction.FareAmount:0.####}, actual fare: 15.5");
            Console.WriteLine($"**********************************************************************");
        }

    }
}
