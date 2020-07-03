using Microsoft.ML;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetCoreML.MovieRecommender
{
    //https://docs.microsoft.com/ru-ru/dotnet/machine-learning/tutorials/movie-recommendation
    class MovieRecommenderMlSample
    {
        internal static void Start()
        {
            MLContext mlContext = new MLContext();
            (IDataView trainingDataView, IDataView testDataView) = LoadData(mlContext);
            ITransformer model = BuildAndTrainModel(mlContext, trainingDataView);
            EvaluateModel(mlContext, testDataView, model);

            UseModelForSinglePrediction(mlContext, model);
            SaveModel(mlContext, trainingDataView.Schema, model);
        }


        public static (IDataView training, IDataView test) LoadData(MLContext mlContext)
        {
            var trainingDataPath = Path.Combine(Environment.CurrentDirectory, "MovieRecommender", "Data", "recommendation-ratings-train.csv");
            var testDataPath = Path.Combine(Environment.CurrentDirectory, "MovieRecommender", "Data", "recommendation-ratings-test.csv");

            IDataView trainingDataView = mlContext.Data.LoadFromTextFile<MovieRating>(trainingDataPath, hasHeader: true, separatorChar: ',');
            IDataView testDataView = mlContext.Data.LoadFromTextFile<MovieRating>(testDataPath, hasHeader: true, separatorChar: ',');

            return (trainingDataView, testDataView);
        }

        public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainingDataView)
        {
            IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: "userId")
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "movieIdEncoded", inputColumnName: "movieId"));

            /*
             MatrixFactorizationTrainer — это алгоритм обучения для предоставления рекомендаций. 
            Разложение матрицы — стандартный подход к формированию рекомендаций при наличии данных 
            о том, как пользователи оценивали продукты в прошлом, как в нашем случае. Есть и другие алгоритмы рекомендаций
             */
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "userIdEncoded",
                MatrixRowIndexColumnName = "movieIdEncoded",
                LabelColumnName = "Label",
                NumberOfIterations = 20,
                ApproximationRank = 100
            };

            var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));
            Console.WriteLine("=============== Training the model ===============");
            // Fit() обучает модель с помощью предоставленных наборов обучающих данных
            ITransformer model = trainerEstimator.Fit(trainingDataView);

            return model;
        }

        //Оценка модели
        public static void EvaluateModel(MLContext mlContext, IDataView testDataView, ITransformer model)
        {
            Console.WriteLine("=============== Evaluating the model ===============");
            //Метод Transform() делает прогнозы для нескольких входных строк тестового набора данных.
            var prediction = model.Transform(testDataView);
            var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");

            //root of mean squared error (RMS или RMSE) используется для измерения разницы между значениями, 
            //спрогнозированными моделью, и фактическими значениями в тестовом наборе данных. 
            //С технической точки зрения она вычисляется как квадратный корень из среднего значения квадратов погрешностей. 
            //Чем ниже это отклонение, тем лучше модель.
            Console.WriteLine("Root Mean Squared Error : " + metrics.RootMeanSquaredError.ToString());

            //R Squared указывает, насколько хорошо данные соответствуют модели. Значение находится в диапазоне от 0 до 1. 
            //Значение 0 означает, что данные случайные или по другим причинам не могут соответствовать модели. 
            //Значение 1 означает, что модель идеально соответствует этим данным. 
            //Значение R Squared должно быть максимально близко к 1.
            Console.WriteLine("RSquared: " + metrics.RSquared.ToString());
        }

        //Использование модели
        public static void UseModelForSinglePrediction(MLContext mlContext, ITransformer model)
        {
            Console.WriteLine("=============== Making a prediction ===============");
            var predictionEngine = mlContext.Model.CreatePredictionEngine<MovieRating, MovieRatingPrediction>(model);

            var testInput = new MovieRating { userId = 6, movieId = 10 };
            var movieRatingPrediction = predictionEngine.Predict(testInput);

            if (Math.Round(movieRatingPrediction.Score, 1) > 3.5)
            {
                Console.WriteLine("Movie " + testInput.movieId + " is recommended for user " + testInput.userId);
            }
            else
            {
                Console.WriteLine("Movie " + testInput.movieId + " is not recommended for user " + testInput.userId);
            }
        }

        /// <summary>
        /// Сохранение модели
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="trainingDataViewSchema"></param>
        /// <param name="model"></param>
        public static void SaveModel(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            var modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "MovieRecommenderModel.zip");

            Console.WriteLine("=============== Saving the model to a file ===============");
            mlContext.Model.Save(model, trainingDataViewSchema, modelPath);
        }
    }
}
