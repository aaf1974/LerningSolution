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
        IrisFlowerClustering = 4
    }
}
