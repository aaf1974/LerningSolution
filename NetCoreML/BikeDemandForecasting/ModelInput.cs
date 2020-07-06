using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.BikeDemandForecasting
{

    /*
     Класс ModelInput содержит следующие столбцы:
        RentalDate: дата наблюдения.
        Year: закодированный год наблюдения (0 = 2011, 1 = 2012).
        TotalRentals: общее число арендованных велосипедов за этот день.
     */
    public class ModelInput
    {
        public DateTime RentalDate { get; set; }

        public float Year { get; set; }

        public float TotalRentals { get; set; }
    }

    /*
        Класс ModelOutput содержит следующие столбцы:
        ForecastedRentals: прогнозируемые значения для прогнозируемого периода.
        LowerBoundRentals: минимальные прогнозируемые значения для прогнозируемого периода.
        UpperBoundRentals: максимальные прогнозируемые значения для прогнозируемого периода.     
     */
    public class ModelOutput
    {
        public float[] ForecastedRentals { get; set; }

        public float[] LowerBoundRentals { get; set; }

        public float[] UpperBoundRentals { get; set; }
    }
}
