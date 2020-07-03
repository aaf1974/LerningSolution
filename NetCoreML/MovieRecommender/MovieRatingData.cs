using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.MovieRecommender
{
    /// <summary>
    /// класс входных данных
    /// </summary>
    public class MovieRating
    {
        [LoadColumn(0)]
        public float userId;

        [LoadColumn(1)]
        public float movieId;

        [LoadColumn(2)]
        public float Label;
    }

    /// <summary>
    /// будет представлять результаты прогнозирования
    /// </summary>
    public class MovieRatingPrediction
    {
        public float Label;
        public float Score;
    }
}
