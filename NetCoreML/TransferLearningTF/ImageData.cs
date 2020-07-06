using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.TransferLearningTF
{

    /*
     ImageData является классом входных данных изображения и имеет следующие поля String:
        ImagePath содержит имя файла изображения.
        Label содержит значение для метки изображения.

     */
    public class ImageData
    {
        [LoadColumn(0)]
        public string ImagePath;

        [LoadColumn(1)]
        public string Label;
    }


    /*
     ImagePrediction является классом прогноза изображения и имеет следующие поля:
        Score содержит процентное значение достоверности для конкретной классификации изображения.
        PredictedLabelValue содержит значение для прогнозируемой метки классификации изображения.
    Класс ImagePrediction используется для прогнозирования после обучения модели. Он включает string (ImagePath) 
    с путем к изображению. Label используется для применения и обучения модели. 
    PredictedLabelValue используется для прогнозирования и оценки. Для оценки применяются входные 
    обучающие данные, прогнозируемые значения и модель.
     */
    public class ImagePrediction : ImageData
    {
        public float[] Score;

        public string PredictedLabelValue;
    }
}
