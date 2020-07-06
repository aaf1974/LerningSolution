using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.OnImageObjectDetection.YoloParser
{

    /*
     DimensionsBase имеет следующие свойства float:
        X содержит расположение объекта вдоль оси X.
        Y содержит расположение объекта вдоль оси Y.
        Height содержит высоту объекта.
        Width содержит ширину объекта.
     */
    public class DimensionsBase
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
    }
}
