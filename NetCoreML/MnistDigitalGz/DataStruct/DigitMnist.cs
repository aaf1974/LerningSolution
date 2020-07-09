using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.MnistDigitalGz.DataStruct
{
    /// <summary>
    /// The DigitMnist class represents one mnist digit.
    /// </summary>
    class DigitMnist
    {
        //[VectorType(785)] 
        //public byte[,] PixelValues;//System.ArgumentOutOfRangeException: 'Schema mismatch for feature column 'PixelValues': expected Vector<Single>, got Vector<Byte> '


        //[VectorType(784)]
        //public byte[] PixelValues;

        //[VectorType(196)]
        //public float[] PixelValues;

        [VectorType(784)]
        public byte[,] PixelValues;



        //[LoadColumn(64)]
        //public float Number;//Schema mismatch for label column 'Number': expected Key<UInt32>, got Single '

        //public Int32 Number;//'Schema mismatch for label column 'Number': expected Key<UInt32>, got Int32 '
        public UInt32 Number;
    }
}
