using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.OnImageObjectDetection.DataStructures
{

    /*
     ImageNetPrediction является классом прогноза данных и имеет следующее поле float[]:
        PredictedLabel содержит измерения, оценку объекта и вероятности класса для каждого ограничивающего прямоугольника, обнаруженного в изображении.
     */
    public class ImageNetPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels;
    }
}
