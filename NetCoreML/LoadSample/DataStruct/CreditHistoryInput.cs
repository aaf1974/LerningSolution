using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.LoadSample.DataStruct
{
    class CreditHistoryInput
    {
        [LoadColumn(0)]
        public string A1;

        [LoadColumn(1)]
        public float A2;

        [LoadColumn(2)]
        public float A3;

        [LoadColumn(3)]
        public string A4;

        [LoadColumn(4)]
        public string A5;

        [LoadColumn(5)]
        public string A6;

        [LoadColumn(6)]
        public string A7;

        [LoadColumn(7)]
        public float A8;

        [LoadColumn(8)]
        public string A9;

        [LoadColumn(9)]
        public string A10;

        [LoadColumn(10)]
        public int A11;

        [LoadColumn(11)]
        public string A12;

        [LoadColumn(12)]
        public string A13;

        [LoadColumn(13)]
        public int A14;

        [LoadColumn(14)]
        public int A15;

        [LoadColumn(15)]
        public string creaditClass;

    }
}
