namespace NetCoreLibrary.MnistLoader
{
    public class MnistItem
    {
        public byte Label { get; set; }
        public byte[,] Image { get; set; }

        public double[,] AsDouble()
        {
            var res = new double[Image.GetLength(0), Image.GetLength(1)];
            for (var x = 0; x < Image.GetLength(0); x++)
                for (var y = 0; y < Image.GetLength(1); y++)
                    res[x, y] = Image[x, y] / 256.0;
            return res;
        }
    }
}

