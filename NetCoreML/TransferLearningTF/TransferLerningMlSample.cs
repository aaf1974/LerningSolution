using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace NetCoreML.TransferLearningTF
{
    class TransferLerningMlSample
    {
        static string projectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../", "TransferLearningTF"));

        static readonly string _assetsPath = Path.Combine(projectDirectory, "assets");
        static readonly string _imagesFolder = Path.Combine(_assetsPath, "images");
        static readonly string _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
        static readonly string _testTagsTsv = Path.Combine(_imagesFolder, "test-tags.tsv");
//        static readonly string _predictSingleImage = Path.Combine(_imagesFolder, "toaster3.jpg");
        static readonly string _predictSingleImage = Path.Combine(_imagesFolder, "arbus2.jfif");
        static readonly string _inceptionTensorFlowModel = Path.Combine(_assetsPath, "inception", "tensorflow_inception_graph.pb");


        internal static void Start()
        {
            MLContext mlContext = new MLContext();

            ITransformer model = GenerateModel(mlContext);
            ClassifySingleImage(mlContext, model);
        }


        private struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }


        /// <summary>
        /// метода для формирования прогноза
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="model"></param>
        public static void ClassifySingleImage(MLContext mlContext, ITransformer model)
        {
            var imageData = new ImageData()
            {
                ImagePath = _predictSingleImage
            };
            // Make prediction function (input = ImageData, output = ImagePrediction)
            var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
            var prediction = predictor.Predict(imageData);

            Console.WriteLine($"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
        }


        /// <summary>
        /// конвейера модели ML.NET
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        public static ITransformer GenerateModel(MLContext mlContext)
        {
            IEstimator<ITransformer> pipeline = mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: _imagesFolder, inputColumnName: nameof(ImageData.ImagePath))
                // The image transforms transform the images into the model's expected format.
                //изображения загружаются в память, преобразуются в соответствующий размер, а пиксели извлекаются в числовой вектор
                .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: "input"))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))

                //средство оценки, чтобы загрузить модель TensorFlow и оценить ее:
                //Этот этап конвейера загружает модель TensorFlow в память, а затем обрабатывает 
                //вектор значений пикселей через сеть модели TensorFlow. Применение входных данных к модели глубокого 
                //обучения и формирование выходных данных с помощью модели называется оценкой. При полном 
                //использовании модели оценка делает вывод или прогноз.
                .Append(mlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel)
                .ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
                // средство оценки, чтобы сопоставлять строковые метки в данных для обучения с целочисленными значениями ключа
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
                //Добавьте алгоритм обучения ML.NET:
                .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
                //Добавьте средство оценки для преобразования значения прогнозируемого ключа обратно в строку
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                .AppendCacheCheckpoint(mlContext);

            //Обучение модели
            IDataView trainingData = mlContext.Data.LoadFromTextFile<ImageData>(path: _trainTagsTsv, hasHeader: false);
            ITransformer model = pipeline.Fit(trainingData);

            //Оценка точности модели
            IDataView testData = mlContext.Data.LoadFromTextFile<ImageData>(path: _testTagsTsv, hasHeader: false);
            IDataView predictions = model.Transform(testData);

            // Create an IEnumerable for the predictions for displaying results
            IEnumerable<ImagePrediction> imagePredictionData = mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, true);
            DisplayResults(imagePredictionData);
            MulticlassClassificationMetrics metrics = mlContext.MulticlassClassification
                .Evaluate(predictions, labelColumnName: "LabelKey", predictedLabelColumnName: "PredictedLabel");

            //Log-loss — см. раздел Логарифмические потери. Значение логарифмических потерь должно быть максимально близко к нулю.
            Console.WriteLine($"LogLoss is: {metrics.LogLoss}");
            //Per class Log-loss. Значение логарифмических потерь для каждого класса должно быть максимально близко к нулю.
            Console.WriteLine($"PerClassLogLoss is: {String.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");

            return model;
        }


        private static void DisplayResults(IEnumerable<ImagePrediction> imagePredictionData)
        {
            foreach (ImagePrediction prediction in imagePredictionData)
            {
                Console.WriteLine($"Image: {Path.GetFileName(prediction.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
            }
        }


        public static IEnumerable<ImageData> ReadFromTsv(string file, string folder)
        {
            //код анализирует файл tags.tsv, после чего добавляет путь к файлу к имени файла 
            //изображения для свойства ImagePath и загружает его и Label в объект ImageData
            return File.ReadAllLines(file)
                .Select(line => line.Split('\t'))
                .Select(line => new ImageData()
                {
                    ImagePath = Path.Combine(folder, line[0])
                });
        }
    }
}
