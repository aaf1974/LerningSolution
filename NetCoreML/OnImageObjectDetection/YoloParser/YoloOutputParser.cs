﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NetCoreML.OnImageObjectDetection.YoloParser
{
    class CellDimensions : DimensionsBase { }

    /// <summary>
    /// средства анализа
    /// </summary>
    class YoloOutputParser
    {
        /*
            ROW_COUNT задает число строк в сетке, на которые делится изображение.
            COL_COUNT задает число столбцов в сетке, на которые делится изображение.
            CHANNEL_COUNT задает общее число значений, содержащихся в одной ячейке сетки.
            BOXES_PER_CELL задает число ограничивающих прямоугольников в ячейке.
            BOX_INFO_FEATURE_COUNT — число компонентов, содержащихся в поле (X, Y, высота, ширина, достоверность).
            CLASS_COUNT — число прогнозов класса, содержащихся в каждом ограничивающем прямоугольнике.
            CELL_WIDTH — ширина одной ячейки в сетке изображения.
            CELL_HEIGHT — высота одной ячейки в сетке изображения.
            channelStride — начальная координата текущей ячейки в сетке.         
         */
        public const int ROW_COUNT = 13;
        public const int COL_COUNT = 13;
        public const int CHANNEL_COUNT = 125;
        public const int BOXES_PER_CELL = 5;
        public const int BOX_INFO_FEATURE_COUNT = 5;
        public const int CLASS_COUNT = 20;
        public const float CELL_WIDTH = 32;
        public const float CELL_HEIGHT = 32;

        private int channelStride = ROW_COUNT * COL_COUNT;

        /// <summary>
        /// список привязок. Привязки — это предварительно определенные коэффициенты высоты и ширины ограничивающих прямоугольников
        /// </summary>
        private float[] anchors = new float[]
        {
            1.08F, 1.19F, 3.42F, 4.41F, 6.63F, 11.38F, 9.42F, 5.11F, 16.62F, 10.52F
        };


        /// <summary>
        /// список меток.
        /// </summary>
        private string[] labels = new string[]
        {
            "aeroplane", "bicycle", "bird", "boat", "bottle",
            "bus", "car", "cat", "chair", "cow",
            "diningtable", "dog", "horse", "motorbike", "person",
            "pottedplant", "sheep", "sofa", "train", "tvmonitor"
        };



        private static Color[] classColors = new Color[]
        {
            Color.Khaki,
            Color.Fuchsia,
            Color.Silver,
            Color.RoyalBlue,
            Color.Green,
            Color.DarkOrange,
            Color.Purple,
            Color.Gold,
            Color.Red,
            Color.Aquamarine,
            Color.Lime,
            Color.AliceBlue,
            Color.Sienna,
            Color.Orchid,
            Color.Tan,
            Color.LightPink,
            Color.Yellow,
            Color.HotPink,
            Color.OliveDrab,
            Color.SandyBrown,
            Color.DarkTurquoise
        };


        /// <summary>
        /// Sigmoid применяет функцию-сигмоиду, которая выводит число от 0 до 1.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float Sigmoid(float value)
        {
            var k = (float)Math.Exp(value);
            return k / (1.0f + k);
        }

        /// <summary>
        /// Softmax нормализует входной вектор в распределение вероятности.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private float[] Softmax(float[] values)
        {
            var maxVal = values.Max();
            var exp = values.Select(v => Math.Exp(v - maxVal));
            var sumExp = exp.Sum();

            return exp.Select(v => (float)(v / sumExp)).ToArray();
        }


        /// <summary>
        /// GetOffset сопоставляет элементы в выходных данных одномерной модели с соответствующей позицией в тензоре 125 x 13 x 13.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private int GetOffset(int x, int y, int channel)
        {
            // YOLO outputs a tensor that has a shape of 125x13x13, which 
            // WinML flattens into a 1D array.  To access a specific channel 
            // for a given (x,y) cell position, we need to calculate an offset
            // into the array
            return (channel * this.channelStride) + (y * COL_COUNT) + x;
        }

        /// <summary>
        /// ExtractBoundingBoxes извлекает измерения ограничивающего прямоугольника с помощью метода GetOffset из выходных данных модели.
        /// </summary>
        /// <param name="modelOutput"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private BoundingBoxDimensions ExtractBoundingBoxDimensions(float[] modelOutput, int x, int y, int channel)
        {
            return new BoundingBoxDimensions
            {
                X = modelOutput[GetOffset(x, y, channel)],
                Y = modelOutput[GetOffset(x, y, channel + 1)],
                Width = modelOutput[GetOffset(x, y, channel + 2)],
                Height = modelOutput[GetOffset(x, y, channel + 3)]
            };
        }

        /// <summary>
        /// GetConfidence извлекает значение достоверности того, что модель обнаружила объект, и использует функцию Sigmoid, чтобы преобразовать ее в процент.
        /// </summary>
        /// <param name="modelOutput"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private float GetConfidence(float[] modelOutput, int x, int y, int channel)
        {
            return Sigmoid(modelOutput[GetOffset(x, y, channel + 4)]);
        }


        /// <summary>
        /// MapBoundingBoxToCell использует измерения ограничивающего прямоугольника и сопоставляет их с соответствующей ячейкой на изображении.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="box"></param>
        /// <param name="boxDimensions"></param>
        /// <returns></returns>
        private CellDimensions MapBoundingBoxToCell(int x, int y, int box, BoundingBoxDimensions boxDimensions)
        {
            return new CellDimensions
            {
                X = ((float)x + Sigmoid(boxDimensions.X)) * CELL_WIDTH,
                Y = ((float)y + Sigmoid(boxDimensions.Y)) * CELL_HEIGHT,
                Width = (float)Math.Exp(boxDimensions.Width) * CELL_WIDTH * anchors[box * 2],
                Height = (float)Math.Exp(boxDimensions.Height) * CELL_HEIGHT * anchors[box * 2 + 1],
            };
        }


        /// <summary>
        /// ExtractClasses извлекает прогнозы класса для ограничивающего прямоугольника из 
        /// выходных данных модели с помощью метода GetOffset и превращает их в распределение вероятности с помощью метода Softmax.
        /// </summary>
        /// <param name="modelOutput"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public float[] ExtractClasses(float[] modelOutput, int x, int y, int channel)
        {
            float[] predictedClasses = new float[CLASS_COUNT];
            int predictedClassOffset = channel + BOX_INFO_FEATURE_COUNT;
            for (int predictedClass = 0; predictedClass < CLASS_COUNT; predictedClass++)
            {
                predictedClasses[predictedClass] = modelOutput[GetOffset(x, y, predictedClass + predictedClassOffset)];
            }
            return Softmax(predictedClasses);
        }


        /// <summary>
        /// GetTopResult выбирает из списка прогнозируемых классов класс с наибольшей вероятностью.
        /// </summary>
        /// <param name="predictedClasses"></param>
        /// <returns></returns>
        private ValueTuple<int, float> GetTopResult(float[] predictedClasses)
        {
            return predictedClasses
                .Select((predictedClass, index) => (Index: index, Value: predictedClass))
                .OrderByDescending(result => result.Value)
                .First();
        }


        /// <summary>
        /// IntersectionOverUnion фильтрует перекрывающиеся ограничивающие прямоугольники с более низкими вероятностями.
        /// </summary>
        /// <param name="boundingBoxA"></param>
        /// <param name="boundingBoxB"></param>
        /// <returns></returns>
        private float IntersectionOverUnion(RectangleF boundingBoxA, RectangleF boundingBoxB)
        {
            var areaA = boundingBoxA.Width * boundingBoxA.Height;

            if (areaA <= 0)
                return 0;

            var areaB = boundingBoxB.Width * boundingBoxB.Height;

            if (areaB <= 0)
                return 0;

            var minX = Math.Max(boundingBoxA.Left, boundingBoxB.Left);
            var minY = Math.Max(boundingBoxA.Top, boundingBoxB.Top);
            var maxX = Math.Min(boundingBoxA.Right, boundingBoxB.Right);
            var maxY = Math.Min(boundingBoxA.Bottom, boundingBoxB.Bottom);

            var intersectionArea = Math.Max(maxY - minY, 0) * Math.Max(maxX - minX, 0);

            return intersectionArea / (areaA + areaB - intersectionArea);
        }


        /// <summary>
        /// для обработки выходных данных, создаваемых моделью
        /// </summary>
        /// <param name="yoloModelOutputs"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public IList<YoloBoundingBox> ParseOutputs(float[] yoloModelOutputs, float threshold = .3F)
        {
            var boxes = new List<YoloBoundingBox>();
            for (int row = 0; row < ROW_COUNT; row++)
            {
                for (int column = 0; column < COL_COUNT; column++)
                {
                    for (int box = 0; box < BOXES_PER_CELL; box++)
                    {
                        var channel = (box * (CLASS_COUNT + BOX_INFO_FEATURE_COUNT));
                        BoundingBoxDimensions boundingBoxDimensions = ExtractBoundingBoxDimensions(yoloModelOutputs, row, column, channel);
                        float confidence = GetConfidence(yoloModelOutputs, row, column, channel);
                        CellDimensions mappedBoundingBox = MapBoundingBoxToCell(row, column, box, boundingBoxDimensions);
                        if (confidence < threshold)
                            continue;

                        //получение вероятности распределения прогнозируемых классов для текущего ограничивающего прямоугольника
                        float[] predictedClasses = ExtractClasses(yoloModelOutputs, row, column, channel);
                        //получить значение и индекс класса с наибольшей вероятностью для текущего поля и вычислить его оценку
                        var (topResultIndex, topResultScore) = GetTopResult(predictedClasses);
                        var topScore = topResultScore * confidence;
                        if (topScore < threshold)
                            continue;

                        boxes.Add(new YoloBoundingBox()
                        {
                            Dimensions = new BoundingBoxDimensions
                            {
                                X = (mappedBoundingBox.X - mappedBoundingBox.Width / 2),
                                Y = (mappedBoundingBox.Y - mappedBoundingBox.Height / 2),
                                Width = mappedBoundingBox.Width,
                                Height = mappedBoundingBox.Height,
                            },
                            Confidence = topScore,
                            Label = labels[topResultIndex],
                            BoxColor = classColors[topResultIndex]
                        });
                    }
                }
            }

            return boxes;
        }


        /// <summary>
        /// Фильтрация перекрывающихся полей
        /// </summary>
        /// <param name="boxes"></param>
        /// <param name="limit"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public IList<YoloBoundingBox> FilterBoundingBoxes(IList<YoloBoundingBox> boxes, int limit, float threshold)
        {
            var activeCount = boxes.Count;
            var isActiveBoxes = new bool[boxes.Count];

            for (int i = 0; i < isActiveBoxes.Length; i++)
                isActiveBoxes[i] = true;

            var sortedBoxes = boxes.Select((b, i) => new { Box = b, Index = i })
                    .OrderByDescending(b => b.Box.Confidence)
                    .ToList();
            var results = new List<YoloBoundingBox>();

            for (int i = 0; i < boxes.Count; i++)
            {
                if (isActiveBoxes[i])
                {
                    var boxA = sortedBoxes[i].Box;
                    results.Add(boxA);

                    if (results.Count >= limit)
                        break;


                    for (var j = i + 1; j < boxes.Count; j++)
                    {
                        if (isActiveBoxes[j])
                        {
                            var boxB = sortedBoxes[j].Box;

                            if (IntersectionOverUnion(boxA.Rect, boxB.Rect) > threshold)
                            {
                                isActiveBoxes[j] = false;
                                activeCount--;

                                if (activeCount <= 0)
                                    break;
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}
