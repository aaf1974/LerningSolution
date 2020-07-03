using Microsoft.ML.Data;

namespace NetCoreML.TaxiFarePrediction
{

    /*
        vendor_id: идентификатор поставщика услуг такси — это признак.
        rate_code: тип тарифа для поездки на такси — это признак.
        passenger_count: число пассажиров в поездке — это признак.
        trip_time_in_secs: время, затраченное на поездку. Вам требуется спрогнозировать плату за одну поездку до того, как поездка будет завершена. На этот момент вам неизвестна продолжительность поездки. Таким образом, продолжительность поездки не является признаком и соответствующий столбец следует исключить из модели.
        trip_distance: расстояние поездки — это признак.
        payment_type: метод оплаты (наличные или кредитная карта) — это признак.
        fare_amount: общая сумма, уплаченная за поездку на такси, — это метка.
     */
    public class TaxiTrip
    {
        [LoadColumn(0)]
        public string VendorId;

        [LoadColumn(1)]
        public string RateCode;

        [LoadColumn(2)]
        public float PassengerCount;

        [LoadColumn(3)]
        public float TripTime;

        [LoadColumn(4)]
        public float TripDistance;

        [LoadColumn(5)]
        public string PaymentType;

        [LoadColumn(6)]
        public float FareAmount;
    }

    public class TaxiTripFarePrediction
    {
        [ColumnName("Score")]
        public float FareAmount;
    }
}
