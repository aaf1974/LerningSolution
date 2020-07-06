using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;

namespace NetCoreML.BikeDemandForecasting
{
    //https://docs.microsoft.com/ru-ru/dotnet/machine-learning/tutorials/time-series-demand-forecasting
    internal class BikeDemandMlSample
    {
        internal static void Start()
        {
            string rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../", "BikeDemandForecasting"));
            string dbFilePath = Path.Combine(rootDir, "Data", "DailyDemand.mdf");
            string modelPath = Path.Combine(rootDir, "MLModel.zip");
            var connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbFilePath};Integrated Security=True;Connect Timeout=30;";

            MLContext mlContext = new MLContext();

            string query = "SELECT RentalDate, CAST(Year as REAL) as Year, CAST(TotalRentals as REAL) as TotalRentals FROM Rentals";
            DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance, connectionString, query);
            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<ModelInput>();

            IDataView dataView = loader.Load(dbSource);

            /*
            Набор данных содержит данные за два года. Для обучения используются только данные за первый год, 
            а данные за второй год откладываются для сравнения фактических значений с прогнозом, созданным 
            моделью. Отфильтруйте данные с помощью преобразования FilterRowsByColumn.             
             */
            IDataView firstYearData = mlContext.Data.FilterRowsByColumn(dataView, "Year", upperBound: 1);
            IDataView secondYearData = mlContext.Data.FilterRowsByColumn(dataView, "Year", lowerBound: 1);

            //Определите конвейер, который использует SsaForecastingEstimator для прогнозирования значений в наборе данных временных рядов
            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedRentals",
                inputColumnName: "TotalRentals",
                windowSize: 7,
                seriesLength: 30,
                trainSize: 365,
                horizon: 7,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundRentals",
                confidenceUpperBoundColumn: "UpperBoundRentals");

            //обучить модель и подогнать данные по ранее определенным forecastingPipeline.
            SsaForecastingTransformer forecaster = forecastingPipeline.Fit(firstYearData);

            //Оценка модели
            Evaluate(secondYearData, forecaster, mlContext);

            //создайте TimeSeriesPredictionEngine. TimeSeriesPredictionEngine — это удобный метод для создания единичных прогнозов
            var forecastEngine = forecaster.CreateTimeSeriesEngine<ModelInput, ModelOutput>(mlContext);
            //Сохраните модель в файл с именем MLModel.zip, которое задано ранее определенной переменной 
            //modelPath. Выполните метод Checkpoint, чтобы сохранить модель.
            forecastEngine.CheckPoint(mlContext, modelPath);


            Forecast(secondYearData, 7, forecastEngine, mlContext);
        }


        static void Evaluate(IDataView testData, ITransformer model, MLContext mlContext)
        {
            IDataView predictions = model.Transform(testData);
            //Чтобы получить фактические значения из примера данных, используйте метод
            IEnumerable<float> actual = mlContext.Data.CreateEnumerable<ModelInput>(testData, true)
                .Select(observed => observed.TotalRentals);

            //Чтобы получить прогнозируемые значения, используйте метод CreateEnumerable.
            IEnumerable<float> forecast = mlContext.Data.CreateEnumerable<ModelOutput>(predictions, true)
                .Select(prediction => prediction.ForecastedRentals[0]);

            //Вычислите разницу между реальными и прогнозируемыми значениями, которая обычно называется ошибкой.
            var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);

            //Для оценки точности модели вычислите среднюю абсолютную погрешность и среднеквадратическую погрешность.
            /*
                Средняя абсолютная погрешность. Оценивает, насколько близки прогнозы к фактическому значению. Принимает 
            значения в диапазоне от 0 до бесконечности. Чем ближе это значение к 0, тем лучше качество модели.
                Среднеквадратическая погрешность. Суммирует ошибки в модели. Принимает значения в диапазоне от 0 до 
            бесконечности. Чем ближе это значение к 0, тем лучше качество модели.
             */
            var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
            var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error

            Console.WriteLine("Evaluation Metrics");
            Console.WriteLine("---------------------");
            Console.WriteLine($"Mean Absolute Error: {MAE:F3}");
            Console.WriteLine($"Root Mean Squared Error: {RMSE:F3}\n");
        }


        /// <summary>
        /// Применение модели для прогнозирования спроса
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="horizon"></param>
        /// <param name="forecaster"></param>
        /// <param name="mlContext"></param>
        static void Forecast(IDataView testData, int horizon, TimeSeriesPredictionEngine<ModelInput, ModelOutput> forecaster, MLContext mlContext)
        {
            ModelOutput forecast = forecaster.Predict();
            //Сравните фактические и прогнозируемые значения по семи периодам.
            IEnumerable<string> forecastOutput = mlContext.Data.CreateEnumerable<ModelInput>(testData, reuseRowObject: false)
                .Take(horizon)
                .Select((ModelInput rental, int index) => 
                {
                    string rentalDate = rental.RentalDate.ToShortDateString();
                    float actualRentals = rental.TotalRentals;
                    float lowerEstimate = Math.Max(0, forecast.LowerBoundRentals[index]);
                    float estimate = forecast.ForecastedRentals[index];
                    float upperEstimate = forecast.UpperBoundRentals[index];
                    return $"Date: {rentalDate}\n" +
                    $"Actual Rentals: {actualRentals}\n" +
                    $"Lower Estimate: {lowerEstimate}\n" +
                    $"Forecast: {estimate}\n" +
                    $"Upper Estimate: {upperEstimate}\n";
                });

            Console.WriteLine("Rental Forecast");
            Console.WriteLine("---------------------");
            foreach (var prediction in forecastOutput)
            {
                Console.WriteLine(prediction);
            }
        }
    }
}