using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace NetCoreLibrary.MnistLoader
{
    //https://github.com/guylangston/MNIST.IO/blob/master/FileReaderMNIST.cs
    public static class MnistReader
    {
        public static IEnumerable<MnistItem> LoadImagesAndLables(string labelFile, string imageFile)
        {
            var labels = LoadLabel(labelFile);
            var images = LoadImages(imageFile);

            var cc = 0;
            foreach (var img in images)
            {
                yield return new MnistItem()
                {
                    Label = labels[cc],
                    Image = img
                };

                cc++;
            }
        }

        public static IEnumerable<byte[,]> LoadImages(string imageFile)
        {
            /*
            TRAINING SET IMAGE FILE (train-images-idx3-ubyte):
            [offset] [type]          [value]          [description] 
            0000     32 bit integer  0x00000803(2051) magic number 
            0004     32 bit integer  60000            number of images 
            0008     32 bit integer  28               number of rows 
            0012     32 bit integer  28               number of columns 
            0016     unsigned byte   ??               pixel 
            0017     unsigned byte   ??               pixel 
            ........ 
            xxxx     unsigned byte   ??               pixel
            Pixels are organized row-wise. Pixel values are 0 to 255. 0 means background (white), 255 means foreground (black). 
            */

            using (var raw = File.OpenRead(imageFile))
            {
                using (var gz = new GZipStream(raw, CompressionMode.Decompress))
                {
                    using (var reader = new BinaryReaderMSB(gz))
                    {
                        var header = reader.ReadInt32MSB();
                        if (header != 0x803) throw new InvalidDataException(header.ToString("x"));
                        var itemCount = reader.ReadInt32MSB();
                        var rowCount = reader.ReadInt32MSB();
                        var colCount = reader.ReadInt32MSB();

                        for (var i = 0; i < itemCount; i++)
                        {
                            var image = new byte[rowCount, colCount];
                            for (var r = 0; r < rowCount; r++)
                                for (var c = 0; c < colCount; c++)
                                    image[r, c] = reader.ReadByte();

                            yield return image;
                        }
                    }
                }
            }
        }


        public static byte[] LoadLabel(string labelFile)
        {
            /*
            TRAINING SET LABEL FILE (train-labels-idx1-ubyte):
            [offset] [type]          [value]          [description] 
            0000     32 bit integer  0x00000801(2049) magic number (MSB first) 
            0004     32 bit integer  60000            number of items 
            0008     unsigned byte   ??               label 
            0009     unsigned byte   ??               label 
            ........ 
            xxxx     unsigned byte   ??               label
            The labels values are 0 to 9.
            */

            using (var raw = File.OpenRead(labelFile))
            {
                using (var gz = new GZipStream(raw, CompressionMode.Decompress))
                {
                    using (var reader = new BinaryReaderMSB(gz))
                    {
                        // Check Header / Magic Number
                        var header = reader.ReadInt32MSB();
                        if (header != 0x801) 
                            throw new InvalidDataException(header.ToString("x"));
                        var itemCount = reader.ReadInt32MSB();

                        return reader.ReadBytes(itemCount);
                    }
                }
            }
        }
    }
}

