using System;
using System.IO;
using Microsoft.ML;
using System.Collections.Generic;

namespace NetCoreML.ProductSalesAnomalyDetection
{
    //https://docs.microsoft.com/ru-ru/dotnet/machine-learning/tutorials/sales-anomaly-detection
    internal class ProductSalesAnomalyDetectionMlSample
    {
        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "ProductSalesAnomalyDetection", "Data", "product-sales.csv");
        //assign the Number of records in dataset file to constant variable
        const int _docsize = 36;


        internal static void Start()
        {
            MLContext mlContext = new MLContext();

            IDataView dataView = mlContext.Data.LoadFromTextFile<ProductSalesData>(path: _dataPath, hasHeader: true, separatorChar: ',');

            DetectSpike(mlContext, _docsize, dataView);
            DetectChangepoint(mlContext, _docsize, dataView);
        }



        static IDataView CreateEmptyDataView(MLContext mlContext)
        {
            // Create empty DataView. We just need the schema to call Fit() for the time series transforms
            IEnumerable<ProductSalesData> enumerableData = new List<ProductSalesData>();
            return mlContext.Data.LoadFromEnumerable(enumerableData);
        }


        /// <summary>
        /// Обнаружение пиковых значений
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="docSize"></param>
        /// <param name="productSales"></param>
        static void DetectSpike(MLContext mlContext, int docSize, IDataView productSales)
        {
            //Используйте IidSpikeEstimator, чтобы обучить модель для обнаружения пиковых значений.
            var iidSpikeEstimator = mlContext.Transforms.DetectIidSpike(
                outputColumnName: nameof(ProductSalesPrediction.Prediction), 
                inputColumnName: nameof(ProductSalesData.numSales), 
                confidence: 95, 
                pvalueHistoryLength: docSize / 4
                );

            //Создайте преобразование для обнаружения пиковых значений,
            ITransformer iidSpikeTransform = iidSpikeEstimator.Fit(CreateEmptyDataView(mlContext));
            IDataView transformedData = iidSpikeTransform.Transform(productSales);

            //Преобразуйте transformedData в строго типизированный IEnumerable для более удобного отображения с помощью метода CreateEnumerable()
            var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);


            /*
            Alert указывает на оповещение о пиковом значении для заданной точки данных.
            Score является значением ProductSales для заданной точки данных в наборе данных.
            P-Value — "P" означает вероятность. Чем ближе p-значение к 0, тем больше вероятность того, что точка данных аномальна.             
             */
            Console.WriteLine("Alert\tScore\tP-Value");

            foreach (var p in predictions)
            {
                var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}";

                if (p.Prediction[0] == 1)
                {
                    results += " <-- Spike detected";
                }

                Console.WriteLine(results);
            }
            Console.WriteLine("");
        }


        /// <summary>
        /// Обнаружение точек изменений
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="docSize"></param>
        /// <param name="productSales"></param>
        static void DetectChangepoint(MLContext mlContext, int docSize, IDataView productSales)
        {
            var iidChangePointEstimator = mlContext.Transforms
                .DetectIidChangePoint(outputColumnName: nameof(ProductSalesPrediction.Prediction), 
                        inputColumnName: nameof(ProductSalesData.numSales), 
                        confidence: 95, 
                        changeHistoryLength: docSize / 4);

            //Как и ранее, создайте преобразование из средства оценки
            var iidChangePointTransform = iidChangePointEstimator.Fit(CreateEmptyDataView(mlContext));
            IDataView transformedData = iidChangePointTransform.Transform(productSales);
            var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);


            /*
             Alert указывает на оповещение о точке изменений для заданной точки данных.
            Score является значением ProductSales для заданной точки данных в наборе данных.
            P-Value — "P" означает вероятность. Чем ближе p-значение к 0, тем больше вероятность того, что точка данных аномальна.
            Martingale value — используется для определения того, насколько "странной" является точка данных, на основе последовательности P-значений.
             */
            Console.WriteLine("Alert\tScore\tP-Value\tMartingale value");

            foreach (var p in predictions)
            {
                var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}\t{p.Prediction[3]:F2}";

                if (p.Prediction[0] == 1)
                {
                    results += " <-- alert is on, predicted changepoint";
                }
                Console.WriteLine(results);
            }
            Console.WriteLine("");
        }
    }
}