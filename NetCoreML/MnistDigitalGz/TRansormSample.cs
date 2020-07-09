using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreML.MnistDigitalGz
{
    public static class MapValueToKeyMultiColumn
    {
        /// This example demonstrates the use of the ValueToKeyMappingEstimator, by
        /// mapping strings to KeyType values. For more on ML.NET KeyTypes see:
        /// https://github.com/dotnet/machinelearning/blob/master/docs/code/IDataViewTypeSystem.md#key-types 
        /// It is possible to have multiple values map to the same category.
        public static void Example()
        {
            // Create a new ML context, for ML.NET operations. It can be used for
            // exception tracking and logging, as well as the source of randomness.
            var mlContext = new MLContext();

            // Get a small dataset as an IEnumerable.
            var rawData = new[] {
                new DataPoint() { StudyTime = "0-4yrs" , Course = "CS" },
                new DataPoint() { StudyTime = "6-11yrs" , Course = "CS" },
                new DataPoint() { StudyTime = "12-25yrs" , Course = "LA" },
                new DataPoint() { StudyTime = "0-5yrs" , Course = "DS" }
            };

            var data = mlContext.Data.LoadFromEnumerable(rawData);

            // Constructs the ML.net pipeline
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(new[] {
                new  InputOutputColumnPair("StudyTimeCategory", "StudyTime"),
                new  InputOutputColumnPair("CourseCategory", "Course")
                },
                keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue, 
                addKeyValueAnnotationsAsText: true);

            // Fits the pipeline to the data.
            IDataView transformedData = pipeline.Fit(data).Transform(data);

            // Getting the resulting data as an IEnumerable.
            // This will contain the newly created columns.
            IEnumerable<TransformedData> features = mlContext.Data.CreateEnumerable<TransformedData>(transformedData, reuseRowObject: false);

            Console.WriteLine($" StudyTime   StudyTimeCategory   Course    " + $"CourseCategory");

            foreach (var featureRow in features)
                Console.WriteLine($"{featureRow.StudyTime}\t\t" +
                    $"{featureRow.StudyTimeCategory}\t\t\t{featureRow.Course}\t\t" +
                    $"{featureRow.CourseCategory}");

            // TransformedData obtained post-transformation.
            //
            // StudyTime     StudyTimeCategory   Course    CourseCategory
            // 0-4yrs          1                   CS          1
            // 6-11yrs         4                   CS          1
            // 12-25yrs        3                   LA          3
            // 0-5yrs          2                   DS          2

            // If we wanted to provide the mapping, rather than letting the
            // transform create it, we could do so by creating an IDataView one
            // column containing the values to map to. If the values in the dataset
            // are not found in the lookup IDataView they will get mapped to the
            // missing value, 0. The keyData are shared among the columns, therefore
            // the keys are not contiguous for the column. Create the lookup map
            // data IEnumerable.  
            var lookupData = new[] {
                new LookupMap { Key = "0-4yrs" },
                new LookupMap { Key = "6-11yrs" },
                new LookupMap { Key = "25+yrs"  },
                new LookupMap { Key = "CS" },
                new LookupMap { Key = "DS" },
                new LookupMap { Key = "LA"  }
            };

            // Convert to IDataView
            var lookupIdvMap = mlContext.Data.LoadFromEnumerable(lookupData);

            // Constructs the ML.net pipeline
            var pipelineWithLookupMap = mlContext.Transforms.Conversion
                .MapValueToKey(new[] {
                    new  InputOutputColumnPair("StudyTimeCategory", "StudyTime"),
                    new  InputOutputColumnPair("CourseCategory", "Course")
                    },
                    keyData: lookupIdvMap);

            // Fits the pipeline to the data.
            transformedData = pipelineWithLookupMap.Fit(data).Transform(data);

            // Getting the resulting data as an IEnumerable.
            // This will contain the newly created columns.
            features = mlContext.Data.CreateEnumerable<TransformedData>(
                transformedData, reuseRowObject: false);

            Console.WriteLine($" StudyTime   StudyTimeCategory  " +
                $"Course CourseCategory");

            foreach (var featureRow in features)
                Console.WriteLine($"{featureRow.StudyTime}\t\t" +
                    $"{featureRow.StudyTimeCategory}\t\t\t{featureRow.Course}\t\t" +
                    $"{featureRow.CourseCategory}");

            // StudyTime    StudyTimeCategory  Course     CourseCategory
            // 0 - 4yrs          1              CS              4
            // 6 - 11yrs         2              CS              4
            // 12 - 25yrs        0              LA              6
            // 0 - 5yrs          0              DS              5

        }

        private class DataPoint
        {
            public string StudyTime { get; set; }
            public string Course { get; set; }
        }

        private class TransformedData : DataPoint
        {
            public uint StudyTimeCategory { get; set; }
            public uint CourseCategory { get; set; }
        }

        // Type for the IDataView that will be serving as the map
        private class LookupMap
        {
            public string Key { get; set; }
        }
    }


    public class KeyToValueToKey
    {
        public static void Example()
        {
            // Create a new ML context, for ML.NET operations. It can be used for
            // exception tracking and logging, as well as the source of randomness.
            var mlContext = new MLContext();

            // Get a small dataset as an IEnumerable.
            var rawData = new[] {
                new DataPoint() { Review = "animals birds cats dogs fish horse"},
                new DataPoint() { Review = "horse birds house fish duck cats"},
                new DataPoint() { Review = "car truck driver bus pickup"},
                new DataPoint() { Review = "car truck driver bus pickup horse"},
            };

            var trainData = mlContext.Data.LoadFromEnumerable(rawData);

            // A pipeline to convert the terms of the 'Review' column in 
            // making use of default settings.
            var defaultPipeline = mlContext.Transforms.Text.TokenizeIntoWords(
                "TokenizedText", nameof(DataPoint.Review)).Append(mlContext
                .Transforms.Conversion.MapValueToKey(nameof(TransformedData.Keys),
                "TokenizedText"));

            // Another pipeline, that customizes the advanced settings of the
            // ValueToKeyMappingEstimator. We can change the maximumNumberOfKeys to
            // limit how many keys will get generated out of the set of words, and
            // condition the order in which they get evaluated by changing
            // keyOrdinality from the default ByOccurence (order in which they get
            // encountered) to value/alphabetically.
            var customizedPipeline = mlContext.Transforms.Text.TokenizeIntoWords(
                "TokenizedText", nameof(DataPoint.Review)).Append(mlContext
                .Transforms.Conversion.MapValueToKey(nameof(TransformedData.Keys),
                "TokenizedText", maximumNumberOfKeys: 10, keyOrdinality:
                ValueToKeyMappingEstimator.KeyOrdinality.ByValue));

            // The transformed data.
            var transformedDataDefault = defaultPipeline.Fit(trainData).Transform(
                trainData);

            var transformedDataCustomized = customizedPipeline.Fit(trainData)
                .Transform(trainData);

            // Getting the resulting data as an IEnumerable.
            // This will contain the newly created columns.
            IEnumerable<TransformedData> defaultData = mlContext.Data.
                CreateEnumerable<TransformedData>(transformedDataDefault,
                reuseRowObject: false);

            IEnumerable<TransformedData> customizedData = mlContext.Data.
                CreateEnumerable<TransformedData>(transformedDataCustomized,
                reuseRowObject: false);

            Console.WriteLine($"Keys");
            foreach (var dataRow in defaultData)
                Console.WriteLine($"{string.Join(',', dataRow.Keys)}");
            // Expected output:
            //  Keys
            //  1,2,3,4,5,6
            //  6,2,7,5,8,3
            //  9,10,11,12,13
            //  9,10,11,12,13,6

            Console.WriteLine($"Keys");
            foreach (var dataRow in customizedData)
                Console.WriteLine($"{string.Join(',', dataRow.Keys)}");
            // Expected output:
            //  Keys
            //  1,2,4,5,7,8
            //  8,2,9,7,6,4
            //  3,10,0,0,0
            //  3,10,0,0,0,8
            // Retrieve the original values, by appending the KeyToValue estimator to
            // the existing pipelines to convert the keys back to the strings.
            var pipeline = defaultPipeline.Append(mlContext.Transforms.Conversion
                .MapKeyToValue(nameof(TransformedData.Keys)));

            transformedDataDefault = pipeline.Fit(trainData).Transform(trainData);

            // Preview of the DefaultColumnName column obtained.
            var originalColumnBack = transformedDataDefault.GetColumn<VBuffer<
                ReadOnlyMemory<char>>>(transformedDataDefault.Schema[nameof(
                TransformedData.Keys)]);

            foreach (var row in originalColumnBack)
            {
                foreach (var value in row.GetValues())
                    Console.Write($"{value} ");
                Console.WriteLine("");
            }

            // Expected output:
            //  animals birds cats dogs fish horse
            //  horse birds house fish duck cats
            //  car truck driver bus pickup
            //  car truck driver bus pickup horse
        }

        private class DataPoint
        {
            public string Review { get; set; }
        }

        private class TransformedData : DataPoint
        {
            public uint[] Keys { get; set; }
        }
    }
}
