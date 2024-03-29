﻿using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.ProductSalesAnomalyDetection
{
    public class ProductSalesData
    {
        [LoadColumn(0)]
        public string Month;

        [LoadColumn(1)]
        public float numSales;
    }

    public class ProductSalesPrediction
    {
        //vector to hold alert,score,p-value values
        [VectorType(3)]
        public double[] Prediction { get; set; }
    }
}
