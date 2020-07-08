using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.MnistDigital.DataStruct
{
    class OutPutData
    {
        [ColumnName("Score")]
        public float[] Score;
    }
}
