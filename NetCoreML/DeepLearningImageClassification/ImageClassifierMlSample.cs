using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.ML;
using static Microsoft.ML.DataOperationsCatalog;
using Microsoft.ML.Vision;


//https://docs.microsoft.com/ru-ru/dotnet/machine-learning/tutorials/image-classification-api-transfer-learning
namespace NetCoreML.DeepLearningImageClassification
{
    class ImageClassifierMlSample
    {
        static string projectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../", "DeepLearningImageClassification"));
        static string workspaceRelativePath = Path.Combine(projectDirectory, "workspace");
        static string assetsRelativePath = Path.Combine(projectDirectory, "assets");


        internal static void Start()
        {
            MLContext mlContext = new MLContext();
            IEnumerable<ImageData> images = LoadImagesFromDirectory(folder: assetsRelativePath, useFolderNameAsLabel: true);

            IDataView imageData = mlContext.Data.LoadFromEnumerable(images);
            //Данные загружаются в том порядке, в котором они были считаны из каталогов. Чтобы сбалансировать данные, 
            //перемешайте их в случайном порядке с помощью метода ShuffleRows
            IDataView shuffledData = mlContext.Data.ShuffleRows(imageData);

            var preprocessingPipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Label", outputColumnName: "LabelAsKey")
                    .Append(mlContext.Transforms.LoadRawImageBytes(outputColumnName: "Image", imageFolder: assetsRelativePath, inputColumnName: "ImagePath"));

            //Используйте метод Fit, чтобы применить данные к preprocessingPipelineEstimatorChain, а затем метод Transform, 
            //который возвращает IDataView, содержащий предварительно обработанные данные.
            IDataView preProcessedData = preprocessingPipeline
                    .Fit(shuffledData)
                    .Transform(shuffledData);


            /*
            Для обучения модели важно иметь набор данных для обучения и проверочный набор данных. 
            Модель обучается на наборе для обучения. То, насколько точно она делает прогнозы по неизвестным данным, оценивается 
            с точки зрения эффективности по сравнению с проверочным набором. В зависимости от достигнутой эффективности модель вносит 
            корректировки в то, что она изучила, для улучшения результата. Проверочный набор можно получить, разделив исходный 
            набор данных, либо из другого источника, который уже был подобран специально для этого. В данном случае 
            предварительно обработанный набор данных разделяется на наборы для обучения, проверки и тестирования.             
             */
            /*
                выполняется два разделения. Сначала предварительно обработанные данные разделяются в следующей 
            пропорции: 70 % для обучения и оставшиеся 30 % для проверки. Затем 30-процентный проверочный набор снова 
            разделяется на наборы для проверки и тестирования в следующей пропорции: 90 % для проверки и 10 % для тестирования.             
             */
            TrainTestData trainSplit = mlContext.Data.TrainTestSplit(data: preProcessedData, testFraction: 0.3);
            TrainTestData validationTestSplit = mlContext.Data.TrainTestSplit(trainSplit.TestSet, 0.1);

            /*
             Назначьте сегментам соответствующие значения для данных обучения, проверки и тестирования.
             */
            IDataView trainSet = trainSplit.TrainSet;
            IDataView validationSet = validationTestSplit.TrainSet;
            IDataView testSet = validationTestSplit.TestSet;


            /*
                ImageClassificationTrainer принимает несколько необязательных параметров:
                FeatureColumnName — столбец, используемый в качестве входных данных для модели.
                LabelColumnName — столбец для прогнозируемого значения.
                ValidationSet является IDataView, содержащим проверочные данные.
                Arch определяет, какую из архитектур предварительно обученной модели нужно использовать. В этом учебнике используется 101-слойный вариант модели ResNetv2.
                MetricsCallback привязывает функцию для отслеживания хода выполнения во время обучения.
                TestOnTrainSet указывает модели измерять эффективность относительно обучающего набора, если проверочный набор отсутствует.
                ReuseTrainSetBottleneckCachedValues сообщает модели, следует ли использовать кэшированные значения из этапа узкого места при последующих запусках. Этап узкого места представляет собой однократное сквозное вычисление, которое потребляет много ресурсов при первом выполнении. Если данные для обучения не изменяются и вы хотите поэкспериментировать с разным числом эпох или разными размерами пакета, использование кэшированных значений значительно сокращает время, необходимое для обучения модели.
                ReuseValidationSetBottleneckCachedValues аналогичен ReuseTrainSetBottleneckCachedValues, только в данном случае он предназначен для проверочного набора данных.
                WorkspacePathопределяет каталог, в котором хранятся вычисленные значения узких мест и версия .pb модели.             
             */
            var classifierOptions = new ImageClassificationTrainer.Options()
            {
                FeatureColumnName = "Image",
                LabelColumnName = "LabelAsKey",
                ValidationSet = validationSet,
                Arch = ImageClassificationTrainer.Architecture.ResnetV2101,
                MetricsCallback = (metrics) => Console.WriteLine(metrics),
                TestOnTrainSet = false,
                ReuseTrainSetBottleneckCachedValues = true,
                ReuseValidationSetBottleneckCachedValues = true,
                WorkspacePath = workspaceRelativePath
            };


            //Определите конвейер обучения EstimatorChain, состоящий из преобразований mapLabelEstimator и ImageClassificationTrainer.
            var trainingPipeline = mlContext.MulticlassClassification.Trainers.ImageClassification(classifierOptions)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            //Обучение
            ITransformer trainedModel = trainingPipeline.Fit(trainSet);

            ClassifySingleImage(mlContext, testSet, trainedModel);
            ClassifyImages(mlContext, testSet, trainedModel);
        }


        /// <summary>
        /// Классификация одного изображения
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="data"></param>
        /// <param name="trainedModel"></param>
        public static void ClassifySingleImage(MLContext mlContext, IDataView data, ITransformer trainedModel)
        {
            PredictionEngine<ModelInput, ModelOutput> predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(trainedModel);
            ModelInput image = mlContext.Data.CreateEnumerable<ModelInput>(data, reuseRowObject: true).First();
            //Используйте метод Predict для классификации изображения.
            ModelOutput prediction = predictionEngine.Predict(image);

            Console.WriteLine("Classifying single image");
            OutputPrediction(prediction);
        }

        /// <summary>
        /// Классификация нескольких изображений
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="data"></param>
        /// <param name="trainedModel"></param>
        public static void ClassifyImages(MLContext mlContext, IDataView data, ITransformer trainedModel)
        {
            IDataView predictionData = trainedModel.Transform(data);
            //Чтобы выполнить итерацию по прогнозам, преобразуйте predictionData IDataView в IEnumerable с помощью 
            //метода CreateEnumerable, а затем получите первые 10 наблюдений.
            IEnumerable<ModelOutput> predictions = mlContext.Data.CreateEnumerable<ModelOutput>(predictionData, reuseRowObject: true).Take(10);

            Console.WriteLine("Classifying multiple images");
            foreach (var prediction in predictions)
            {
                OutputPrediction(prediction);
            }
        }



        /// <summary>
        /// Создание служебного метода для загрузки данных
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="useFolderNameAsLabel"></param>
        /// <returns></returns>
        public static IEnumerable<ImageData> LoadImagesFromDirectory(string folder, bool useFolderNameAsLabel = true)
        {
            var files = Directory.GetFiles(folder, "*", searchOption: SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if ((Path.GetExtension(file) != ".jpg") && (Path.GetExtension(file) != ".png"))
                    continue;

                var label = Path.GetFileName(file);

                if (useFolderNameAsLabel)
                    label = Directory.GetParent(file).Name;
                else
                {
                    for (int index = 0; index < label.Length; index++)
                    {
                        if (!char.IsLetter(label[index]))
                        {
                            label = label.Substring(0, index);
                            break;
                        }
                    }
                }

                yield return new ImageData()
                {
                    ImagePath = file,
                    Label = label
                };
            }
        }


        
        private static void OutputPrediction(ModelOutput prediction)
        {
            string imageName = Path.GetFileName(prediction.ImagePath);
            Console.WriteLine($"Image: {imageName} | Actual Value: {prediction.Label} | Predicted Value: {prediction.PredictedLabel}");
        }



    }
}
