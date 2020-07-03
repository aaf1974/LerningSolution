using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetCoreML.IrisFlowerClustering
{
    class IrisFlowerMlSample
    {

        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "IrisFlowerClustering", "Data", "iris.data");
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "IrisFlowerClustering", "Data", "IrisClusteringModel.zip");


        internal static void Start()
        {
            //Microsoft.ML.MLContext представляет среду машинного обучения и предоставляет механизмы для ведения журнала и 
            //точек входа для загрузки данных, обучения модели, прогнозирования и других задач. 
            var mlContext = new MLContext(seed: 0);

            //Настройка загрузки данных
            IDataView dataView = mlContext.Data.LoadFromTextFile<IrisData>(_dataPath, hasHeader: false, separatorChar: ',');

            string featuresColumnName = "Features";
            //Создание конвейера обучения
            var pipeline = mlContext.Transforms
                .Concatenate(featuresColumnName, "SepalLength", "SepalWidth", "PetalLength", "PetalWidth")
                .Append(mlContext.Clustering.Trainers.KMeans(featuresColumnName, numberOfClusters: 3));

            // выполнить загрузку данных и обучение модели
            var model = pipeline.Fit(dataView);

            //Сохранение модели
            using (var fileStream = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                mlContext.Model.Save(model, dataView.Schema, fileStream);
            }

            //Чтобы получить прогноз, используйте класс PredictionEngine<TSrc,TDst>, который принимает экземпляры входного 
            //типа через конвейер преобразователя и создает экземпляры выходного типа.
            var predictor = mlContext.Model.CreatePredictionEngine<IrisData, ClusterPrediction>(model);

            //узнать, к какому кластеру принадлежит указанный элемент
            var prediction = predictor.Predict(TestIrisData.Setosa);
            Console.WriteLine($"Cluster: {prediction.PredictedClusterId}");
            Console.WriteLine($"Distances: {string.Join(" ", prediction.Distances)}");
        }
    }
}
