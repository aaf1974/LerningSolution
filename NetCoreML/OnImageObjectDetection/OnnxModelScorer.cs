using Microsoft.ML;
using Microsoft.ML.Data;
using NetCoreML.OnImageObjectDetection.DataStructures;
using NetCoreML.OnImageObjectDetection.YoloParser;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.OnImageObjectDetection
{
    /// <summary>
    /// Использование модели для оценки
    /// </summary>
    class OnnxModelScorer
    {
        private readonly string imagesFolder;
        private readonly string modelLocation;
        private readonly MLContext mlContext;

        private IList<YoloBoundingBox> _boundingBoxes = new List<YoloBoundingBox>();



        public OnnxModelScorer(string imagesFolder, string modelLocation, MLContext mlContext)
        {
            this.imagesFolder = imagesFolder;
            this.modelLocation = modelLocation;
            this.mlContext = mlContext;
        }

        /// <summary>
        /// содержать высоту и ширину, ожидаемые в качестве входных данных для модели
        /// </summary>
        public struct ImageNetSettings
        {
            public const int imageHeight = 416;
            public const int imageWidth = 416;
        }

        /// <summary>
        /// содержит имена входных и выходных слоев модели. Чтобы визуализировать имя входного и выходного 
        /// слоев модели, можно использовать такой инструмент, как Netron.
        /// </summary>
        public struct TinyYoloModelSettings
        {
            // for checking Tiny yolo2 Model input and  output  parameter names,
            //you can use tools like Netron, 
            // which is installed by Visual Studio AI Tools

            // input tensor name
            public const string ModelInput = "image";

            // output tensor name
            public const string ModelOutput = "grid";
        }


        private ITransformer LoadModel(string modelLocation)
        {
            Console.WriteLine("Read model");
            Console.WriteLine($"Model location: {modelLocation}");
            Console.WriteLine($"Default parameters: image size=({ImageNetSettings.imageWidth},{ImageNetSettings.imageHeight})");

            var data = mlContext.Data.LoadFromEnumerable(new List<ImageNetData>());

            /*
                Конвейер будет состоять из четырех преобразований.
                LoadImages загружает изображение в виде точечного рисунка.
                ResizeImages изменяет масштаб изображения до указанного размера (в данном случае 416 x 416).
                ExtractPixels изменяет пиксельное представление изображения с точечного рисунка на числовой вектор.
                ApplyOnnxModel загружает модель ONNX и использует ее для оценки предоставленных данных.              
             */
            var pipeline = mlContext.Transforms
                .LoadImages(outputColumnName: "image", imageFolder: "", inputColumnName: nameof(ImageNetData.ImagePath))
                .Append(mlContext.Transforms.ResizeImages(outputColumnName: "image", imageWidth: ImageNetSettings.imageWidth, imageHeight: ImageNetSettings.imageHeight, inputColumnName: "image"))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "image"))
                .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: modelLocation, outputColumnNames: new[] { TinyYoloModelSettings.ModelOutput }, inputColumnNames: new[] { TinyYoloModelSettings.ModelInput }));

            var model = pipeline.Fit(data);

            return model;
        }


        private IEnumerable<float[]> PredictDataUsingModel(IDataView testData, ITransformer model)
        {
            Console.WriteLine($"Images location: {imagesFolder}");
            Console.WriteLine("");
            Console.WriteLine("=====Identify the objects in the images=====");
            Console.WriteLine("");

            IDataView scoredData = model.Transform(testData);

            IEnumerable<float[]> probabilities = scoredData.GetColumn<float[]>(TinyYoloModelSettings.ModelOutput);

            return probabilities;
        }


        public IEnumerable<float[]> Score(IDataView data)
        {
            var model = LoadModel(modelLocation);

            return PredictDataUsingModel(data, model);
        }
    }
}
