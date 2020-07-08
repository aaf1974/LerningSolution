using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NetCoreML
{
    enum MlSampleEnum
    {
        [Description("Анализ мнений пользователей в комментариях с веб-сайта с помощью двоичной классификации в ML.NET")]
        SentimentAnalysis = 1,

        [Description("Классификация заявок на поддержку с использованием мультиклассовой классификации с помощью ML.NET")]
        GitHubIssueClassification = 2,

        [Description("Прогнозирование цен с помощью регрессии с ML.NET")]
        TaxiFarePrediction = 3,

        [Description("Категоризация цветов ириса с использованием кластеризации k-средних в ML.NET")]
        IrisFlowerClustering = 4,

        [Description("Создание приложения для рекомендации фильмов с использованием матричной факторизации и ML.NET")]
        MovieRecommender = 5,

        [Description("(Ahtung!!!) Автоматизированная визуальная проверка с использованием передачи обучения и API классификации изображений ML.NET")]
        DeepLearningImageClassification = 6,

        [Description("Создание модели классификации изображений ML.NET на основе предварительно обученной модели TensorFlow")]
        TransferLearningTF = 7,


        [Description("Прогнозирование спроса для службы проката велосипедов с помощью анализа временных рядов и ML.NET")]
        BikeDemand = 8,

        [Description("Обнаружение аномалий в данных о продажах товаров с помощью ML.NET")]
        ProductSalesAnomalyDetection = 9,


        [Description("Обнаружение объектов с помощью ONNX в ML.NET")]
        OnImageObjectDetection = 10,

        [Description("Анализ тональности отзывов о фильмах с помощью предварительно обученной модели TensorFlow в ML.NET")]
        TextClassificationTF = 11,

        /*
                [Description(" Fashion MNIST TF")]
                FashionMnistTF = 12,

                [Description(" Digital MNIST")]
                DigitalMnist = 13,
        */

        [Description(" Digital MNIST (second try")]
        MnistDigital = 14,
    }
}
