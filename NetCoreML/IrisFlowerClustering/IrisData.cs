using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.IrisFlowerClustering
{

    /*
        Файл Iris.data содержит пять столбцов со следующими данными:
        длина чашелистика в сантиметрах;
        ширина чашелистика в сантиметрах;
        длина лепестка в сантиметрах;
        ширина лепестка в сантиметрах;
        тип ириса.     
    В этом примере кластеризации мы не будем учитывать последний столбец.
     */

    /// <summary>
    /// Класс IrisData содержит входные данные и определения для каждого признака в 
    /// наборе данных. Используйте атрибут LoadColumn, чтобы указать индексы исходных столбцов в файле набора данных.
    /// </summary>
    public class IrisData
    {
        [LoadColumn(0)]
        public float SepalLength;

        [LoadColumn(1)]
        public float SepalWidth;

        [LoadColumn(2)]
        public float PetalLength;

        [LoadColumn(3)]
        public float PetalWidth;
    }


    /// <summary>
    /// Класс ClusterPrediction представляет выходные данные модели кластеризации, примененные к экземпляру IrisData.
    /// Используйте атрибут ColumnName, чтобы привязать поля PredictedClusterId и Distances к столбцам PredictedLabel и Score соответственно.
    /// </summary>
    public class ClusterPrediction
    {
        /// <summary>
        /// Столбец PredictedLabel содержит идентификатор прогнозируемого кластера
        /// </summary>
        [ColumnName("PredictedLabel")]
        public uint PredictedClusterId;


        /// <summary>
        /// Столбец Score содержит массив с квадратом Евклидовых расстояний до центроидов кластера. Длина массива равна числу кластеров.
        /// </summary>
        [ColumnName("Score")]
        public float[] Distances;
    }
}
