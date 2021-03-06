﻿using Microsoft.ML;
using NetCoreML.OnImageObjectDetection.DataStructures;
using NetCoreML.OnImageObjectDetection.YoloParser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace NetCoreML.OnImageObjectDetection
{
    //https://docs.microsoft.com/ru-ru/dotnet/machine-learning/tutorials/object-detection-onnx
    internal class OnImageObjectDetectionMlSample
    {

        static string projectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../", "OnImageObjectDetection", "assets"));

        internal static void Start()
        {
            //var assetsRelativePath = @"../../../assets";
            //string assetsPath = GetAbsolutePath(assetsRelativePath);
            var modelFilePath = Path.Combine(projectDirectory, "Model", "TinyYolo2_model.onnx");
            var imagesFolder = Path.Combine(projectDirectory, "images");
            var outputFolder = Path.Combine(projectDirectory, "images", "output");

            MLContext mlContext = new MLContext();
            try
            {
                IEnumerable<ImageNetData> images = ImageNetData.ReadFromFile(imagesFolder);
                IDataView imageDataView = mlContext.Data.LoadFromEnumerable(images);

                //Затем создайте экземпляр OnnxModelScorer и используйте его для оценки загруженных данных.
                var modelScorer = new OnnxModelScorer(imagesFolder, modelFilePath, mlContext);
                // Use model to score data
                IEnumerable<float[]> probabilities = modelScorer.Score(imageDataView);

                /*
                 Теперь пора выполнить шаг постобработки. Создайте экземпляр YoloOutputParser и используйте его для обработки выходных данных модели.
                 */
                YoloOutputParser parser = new YoloOutputParser();

                var boundingBoxes =
                    probabilities
                    .Select(probability => parser.ParseOutputs(probability))
                    .Select(boxes => parser.FilterBoundingBoxes(boxes, 5, .5F));

                for (var i = 0; i < images.Count(); i++)
                {
                    string imageFileName = images.ElementAt(i).Label;
                    IList<YoloBoundingBox> detectedObjects = boundingBoxes.ElementAt(i);
                    DrawBoundingBox(imagesFolder, outputFolder, imageFileName, detectedObjects);
                    LogDetectedObjects(imageFileName, detectedObjects);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// рисования ограничивающих прямоугольников на изображении
        /// </summary>
        /// <param name="inputImageLocation"></param>
        /// <param name="outputImageLocation"></param>
        /// <param name="imageName"></param>
        /// <param name="filteredBoundingBoxes"></param>
        private static void DrawBoundingBox(string inputImageLocation, string outputImageLocation, string imageName, IList<YoloBoundingBox> filteredBoundingBoxes)
        {
            Image image = Image.FromFile(Path.Combine(inputImageLocation, imageName));

            var originalImageHeight = image.Height;
            var originalImageWidth = image.Width;

            foreach (var box in filteredBoundingBoxes)
            {
                var x = (uint)Math.Max(box.Dimensions.X, 0);
                var y = (uint)Math.Max(box.Dimensions.Y, 0);
                var width = (uint)Math.Min(originalImageWidth - x, box.Dimensions.Width);
                var height = (uint)Math.Min(originalImageHeight - y, box.Dimensions.Height);

                x = (uint)originalImageWidth * x / OnnxModelScorer.ImageNetSettings.imageWidth;
                y = (uint)originalImageHeight * y / OnnxModelScorer.ImageNetSettings.imageHeight;
                width = (uint)originalImageWidth * width / OnnxModelScorer.ImageNetSettings.imageWidth;
                height = (uint)originalImageHeight * height / OnnxModelScorer.ImageNetSettings.imageHeight;

                //шаблон для текста, который будет отображаться над каждым ограничивающим прямоугольником
                string text = $"{box.Label} ({(box.Confidence * 100).ToString("0")}%)";
                using (Graphics thumbnailGraphic = Graphics.FromImage(image))
                {
                    thumbnailGraphic.CompositingQuality = CompositingQuality.HighQuality;
                    thumbnailGraphic.SmoothingMode = SmoothingMode.HighQuality;
                    thumbnailGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    // Define Text Options
                    Font drawFont = new Font("Arial", 12, FontStyle.Bold);
                    SizeF size = thumbnailGraphic.MeasureString(text, drawFont);
                    SolidBrush fontBrush = new SolidBrush(Color.Black);
                    Point atPoint = new Point((int)x, (int)y - (int)size.Height - 1);

                    // Define BoundingBox options
                    Pen pen = new Pen(box.BoxColor, 3.2f);
                    SolidBrush colorBrush = new SolidBrush(box.BoxColor);
                    //Создайте и заполните прямоугольник над ограничивающей рамкой, которая будет содержать текст, с помощью метода FillRectangle.
                    thumbnailGraphic.FillRectangle(colorBrush, (int)x, (int)(y - size.Height - 1), (int)size.Width, (int)size.Height);
                    //Затем нарисуйте текст и ограничивающий прямоугольник на изображении с помощью методов DrawString и DrawRectangle.
                    thumbnailGraphic.DrawString(text, drawFont, fontBrush, atPoint);
                    // Draw bounding box on image
                    thumbnailGraphic.DrawRectangle(pen, x, y, width, height);


                }
            }

            if (!Directory.Exists(outputImageLocation))
            {
                Directory.CreateDirectory(outputImageLocation);
            }

            image.Save(Path.Combine(outputImageLocation, imageName));
        }

        /// <summary>
        /// вывода прогнозов на консоль
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="boundingBoxes"></param>
        private static void LogDetectedObjects(string imageName, IList<YoloBoundingBox> boundingBoxes)
        {
            Console.WriteLine($".....The objects in the image {imageName} are detected as below....");

            foreach (var box in boundingBoxes)
            {
                Console.WriteLine($"{box.Label} and its Confidence score: {box.Confidence}");
            }

            Console.WriteLine("");
        }
    }
}