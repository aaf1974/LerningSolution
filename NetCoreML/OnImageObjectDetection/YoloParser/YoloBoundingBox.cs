using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NetCoreML.OnImageObjectDetection.YoloParser
{
    public class BoundingBoxDimensions : DimensionsBase { }


    /*
    YoloBoundingBox имеет следующие свойства:
        Dimensions содержит размеры ограничивающего прямоугольника.
        Label содержит класс объекта, обнаруженного в ограничивающем прямоугольнике.
        Confidence содержит достоверность класса.
        Rect содержит прямоугольное представление измерений ограничивающего прямоугольника.
        BoxColor содержит цвет, связанный с соответствующим классом, который используется для рисования изображения.     
     */
    public class YoloBoundingBox
    {
        public BoundingBoxDimensions Dimensions { get; set; }

        public string Label { get; set; }

        public float Confidence { get; set; }

        public RectangleF Rect
        {
            get { return new RectangleF(Dimensions.X, Dimensions.Y, Dimensions.Width, Dimensions.Height); }
        }

        public Color BoxColor { get; set; }
    }
}
