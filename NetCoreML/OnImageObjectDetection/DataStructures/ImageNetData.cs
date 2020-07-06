using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetCoreML.OnImageObjectDetection.DataStructures
{
    /*
     ImageNetData является классом входных данных изображения и имеет следующие поля String:
        ImagePath содержит путь, по которому хранится изображение.
        Label содержит имя файла.
     */
    public class ImageNetData
    {
        [LoadColumn(0)]
        public string ImagePath;

        [LoadColumn(1)]
        public string Label;


        /// <summary>
        /// метод ReadFromFile, который загружает несколько файлов изображений, хранящихся по указанном пути imageFolder, и возвращает их в виде коллекции объектов ImageNetData
        /// </summary>
        /// <param name="imageFolder"></param>
        /// <returns></returns>
        public static IEnumerable<ImageNetData> ReadFromFile(string imageFolder)
        {
            return Directory
                .GetFiles(imageFolder)
                .Where(filePath => Path.GetExtension(filePath) != ".md")
                .Select(filePath => new ImageNetData { ImagePath = filePath, Label = Path.GetFileName(filePath) });
        }
    }
}
