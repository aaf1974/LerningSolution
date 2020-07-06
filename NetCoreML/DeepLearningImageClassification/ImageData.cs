using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.DeepLearningImageClassification
{

    /*
     ImagePath — полный путь, по которому хранится изображение.
        Label — категория, к которой принадлежит это изображение. Это прогнозируемое значение.
     */
    class ImageData
    {
        public string ImagePath { get; set; }

        public string Label { get; set; }
    }

    /*
    Image является представлением изображения byte[]. Модель ожидает, что для обучения используются данные изображений этого типа.
    LabelAsKey является численным представлением Label.
    ImagePath — полный путь, по которому хранится изображение.
    Label — категория, к которой принадлежит это изображение. Это прогнозируемое значение.
     */
    /*
    Только Image и LabelAsKey используются для обучения модели и составления прогнозов. 
    Свойства ImagePath и Label хранятся на случай обращения к имени и категории исходного файла изображения.
     */
    class ModelInput
    {
        public byte[] Image { get; set; }

        public UInt32 LabelAsKey { get; set; }

        public string ImagePath { get; set; }

        public string Label { get; set; }
    }


    /*
     ImagePath — полный путь, по которому хранится изображение.
    Label — исходная категория, к которой принадлежит это изображение. Это прогнозируемое значение.
    PredictedLabel — значение, спрогнозированное моделью.
     */
    /*
     для создания прогнозов требуется только PredictedLabel, так как оно содержит прогноз, выполненный моделью. 
    Свойства ImagePath и Label хранятся на случай обращения к имени и категории исходного файла изображения.
     */
    class ModelOutput
    {
        public string ImagePath { get; set; }

        public string Label { get; set; }

        public string PredictedLabel { get; set; }
    }
}
