using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using MNIST.IO;
using NetCoreLibrary;
using NetCoreML.MnistDigitalGz.DataStruct;
using NumSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetCoreML
{

    //https://docs.microsoft.com/ru-ru/archive/msdn-magazine/2014/june/test-run-working-with-the-mnist-image-recognition-data-set
    //https://jamesmccaffrey.wordpress.com/2013/11/23/reading-the-mnist-data-set-with-c/
    internal class MnistDigitalGzMlSample
    {
        internal static void Start()
        {
            MLContext mlContext = new MLContext();
            var loadedData = LoadTrainData(mlContext);

            ITransformer trainData = TrainData(mlContext, loadedData);

            TestPrediction(mlContext, trainData, loadedData);
        }

        static (IDataView Traint, IDataView Test, DigitMnist[] mnistData) LoadTrainData(MLContext mLContext)
        {
            DigitMnist[] mnistTrainData = GetMnistData(MnistDataType.train);
            DigitMnist[] mnistTestData = GetMnistData(MnistDataType.test);

            var schema = SchemaDefinition.Create(typeof(DigitMnist));
            IDataView trainData = mLContext.Data.LoadFromEnumerable(mnistTrainData, schema);
            IDataView testData = mLContext.Data.LoadFromEnumerable(mnistTestData, schema);

            //var c = loaded.GetRowCount();

            return (trainData, testData, mnistTestData);
        }

        private static ITransformer TrainData(MLContext mlContext, (IDataView Traint, IDataView Test, DigitMnist[] mnistData) mnistData)
        {
            var options = new SdcaMaximumEntropyMulticlassTrainer.Options
            {
                FeatureColumnName = MnistEnumHelper.FeaturesCol, //"Features",//nameof(DigitMnist.PixelValues),
                LabelColumnName = MnistEnumHelper.LabelCol //"Label"//nameof(DigitMnist.Number),
            };
            var trainer = mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(options);

            var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey(MnistEnumHelper.LabelCol, nameof(DigitMnist.Number), keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue)
                .Append(mlContext.Transforms.Concatenate(MnistEnumHelper.FeaturesCol, nameof(DigitMnist.PixelValues)).AppendCacheCheckpoint(mlContext))
                .Append(trainer)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(nameof(DigitMnist.Number), MnistEnumHelper.LabelCol));


            Console.WriteLine("=============== Training the model ===============");
            ITransformer trainedModel = dataProcessPipeline.Fit(mnistData.Traint);

            var d = mlContext.Data.CreateEnumerable<DigitMnist>(mnistData.Test, true)
                .Take(5)
                .ToArray();

            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====");
            var predictions = trainedModel.Transform(mnistData.Test);
            var metrics = mlContext.MulticlassClassification.Evaluate(data: predictions, labelColumnName: MnistEnumHelper.LabelCol, scoreColumnName: "Score");
            ConsoleHelper.PrintMultiClassClassificationMetrics(trainer.ToString(), metrics);

            //save model???
            return trainedModel;
        }



        private static DigitMnist[] GetMnistData(MnistDataType mnistType)
        {
            var files = mnistType.GetFilePath();
            TestCase[] mnistData = FileReaderMNIST.LoadImagesAndLables(files.LabelPath, files.ImagePath).ToArray();

            var img0 = mnistData[2].Image;

            string s = mnistData[2].Label.ToString() + Environment.NewLine;
            for (int i = 0; i < 28; ++i)
            {
                for (int j = 0; j < 28; ++j)
                {
                    s += img0[i, j].ToString("X2") + " ";
                }
                s += Environment.NewLine;
            }
            Console.WriteLine(s);

            var res = mnistData.Select(x =>
                new DigitMnist 
                { 
                    Number = Convert.ToUInt32(x.Label),
                    //PixelValues = x.Image
                    PixelValues = TransformTo1D(x)
                })
                .ToArray();

            return res;
        }

        private static float[] TransformTo1D(TestCase x)
        {
            var res = new float[196];
            int resCnt = 0;
            for (int r = 0; r < 28; r++)
            {
                var row = x.Image.GetRow(r);
                for (int part = 0; part < 7; part++)
                { 
                    res[resCnt] = BitConverter.ToSingle(row, part * 4);
                    resCnt++;
                }
            }
            return res;
        }

        private static void TestPrediction(MLContext mlContext, ITransformer trainData, (IDataView Traint, IDataView Test, DigitMnist[] mnistData) mnistData)
        {
            PredictionEngine<DigitMnist, DigitPrediction> predictionEngine = mlContext.Model.CreatePredictionEngine<DigitMnist, DigitPrediction>(trainData);

            DigitMnist inputData = mnistData.mnistData[15];
            DigitPrediction resultprediction1 = predictionEngine.Predict(inputData);

            Console.WriteLine($"Actual: 1     Predicted probability:       zero:  {resultprediction1.Score[0]:0.####}");
            Console.WriteLine($"                                           One :  {resultprediction1.Score[1]:0.####}");
            Console.WriteLine($"                                           two:   {resultprediction1.Score[2]:0.####}");
            Console.WriteLine($"                                           three: {resultprediction1.Score[3]:0.####}");
            Console.WriteLine($"                                           four:  {resultprediction1.Score[4]:0.####}");
            Console.WriteLine($"                                           five:  {resultprediction1.Score[5]:0.####}");
            Console.WriteLine($"                                           six:   {resultprediction1.Score[6]:0.####}");
            Console.WriteLine($"                                           seven: {resultprediction1.Score[7]:0.####}");
            Console.WriteLine($"                                           eight: {resultprediction1.Score[8]:0.####}");
            Console.WriteLine($"                                           nine:  {resultprediction1.Score[9]:0.####}");
        }


    }

    enum MnistDataType
    {
        train,
        test
    }

    static class MnistEnumHelper
    {
        internal const string LabelCol = "Label";
        internal const string FeaturesCol = "Features";
        internal const string NumberCol = "Number";


        static string projectDataPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../", "MnistDigitalGz", "Data"));

        //http://yann.lecun.com/exdb/mnist/
        static string trainImagePath = Path.Combine(projectDataPath, "train-images-idx3-ubyte.gz");
        static string trainLabelPath = Path.Combine(projectDataPath, "train-labels-idx1-ubyte.gz");
        static string testImagePath = Path.Combine(projectDataPath, "t10k-images-idx3-ubyte.gz");
        static string testLablePath = Path.Combine(projectDataPath, "t10k-labels-idx1-ubyte.gz");


        public static (string LabelPath, string ImagePath) GetFilePath(this MnistDataType mnistDataType)
        {
            switch(mnistDataType)
            {
                case MnistDataType.test:
                    return (testLablePath, testImagePath);

                case MnistDataType.train:
                    return (trainLabelPath, trainImagePath);

                default:
                    return (string.Empty, string.Empty);
            }
        }
    }


}