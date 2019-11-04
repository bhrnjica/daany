using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Ext;
using Microsoft.ML;

namespace Unit.Test.DF
{
    public class DataFrameDescribeTests
    {
        
        [Fact]
        public void Describe_Test01()
        {
            var mlContext = new MLContext();
            var dict = new Dictionary<string, List<object>>
            {
                {"product_id",new List<object>() {1,1,2,2,2,2,2 } },
                { "retail_price",new List<object>() { 2,2,5,5,5,5,5 } },
                { "quantity",new List<object>() { 1,2,4,8,16,32,64 } },
                { "city",new List<object>() { "SF","SJ","SF","SJ","Miami", "Orlando","SJ"} },
                { "state" ,new List<object>() { "CA","CA","CA","CA","FL","FL","PR" } },
            };


            var df = new DataFrame(dict);
            var descDf = df.Describe(false, "product_id", "quantity", "state");

            Assert.True(descDf.RowCount() == 11);
            Assert.True(descDf.ColCount() == 3);

            var str = descDf.ToStringBuilder();

            Assert.Equal("Count", descDf.Index[0]);
            Assert.Equal(7, descDf["product_id", 0]);
            Assert.Equal(7, descDf["quantity", 0]);
            Assert.Equal(7, descDf["state", 0]);

            Assert.Equal("Unique", descDf.Index[1]);
            Assert.Equal(2, descDf["product_id", 1]);
            Assert.Equal(7, descDf["quantity", 1]);
            Assert.Equal(3, descDf["state", 1]);

            Assert.Equal("Top", descDf.Index[2]);
            Assert.Equal(2, descDf["product_id", 2]);
            Assert.Equal(1, descDf["quantity", 2]);
            Assert.Equal("CA", descDf["state", 2]);

            Assert.Equal("Freq", descDf.Index[3]);
            Assert.Equal(5, descDf["product_id", 3]);
            Assert.Equal(1, descDf["quantity", 3]);
            Assert.Equal(4, descDf["state", 3]);

            Assert.Equal("Mean", descDf.Index[4]);
            Assert.Equal(1.714286, descDf["product_id", 4]);
            Assert.Equal(18.142857, descDf["quantity", 4]);
            Assert.Equal(DataFrame.NAN, descDf["state", 4]);

            Assert.Equal("Std", descDf.Index[5]);
            Assert.Equal(0.487950, descDf["product_id", 5]);
            Assert.Equal(22.937804, descDf["quantity", 5]);
            Assert.Equal(DataFrame.NAN, descDf["state", 5]);

            Assert.Equal("Min", descDf.Index[6]);
            Assert.Equal(1, descDf["product_id", 6]);
            Assert.Equal(1, descDf["quantity", 6]);
            Assert.Equal(DataFrame.NAN, descDf["state", 6]);

            Assert.Equal("25%", descDf.Index[7]);
            Assert.Equal(1.5, descDf["product_id", 7]);
            Assert.Equal(3.0, descDf["quantity", 7]);
            Assert.Equal(DataFrame.NAN, descDf["state", 7]);

            Assert.Equal("Median", descDf.Index[8]);
            Assert.Equal(2.0, descDf["product_id", 8]);
            Assert.Equal(8.0, descDf["quantity", 8]);
            Assert.Equal(DataFrame.NAN, descDf["state", 8]);

            Assert.Equal("75%", descDf.Index[9]);
            Assert.Equal(2d, descDf["product_id", 9]);
            Assert.Equal(24d, descDf["quantity", 9]);
            Assert.Equal(DataFrame.NAN, descDf["state", 9]);

            Assert.Equal("Max", descDf.Index[10]);
            Assert.Equal(2, descDf["product_id", 10]);
            Assert.Equal(64, descDf["quantity", 10]);
            Assert.Equal(DataFrame.NAN, descDf["state", 10]);

        }
        

        [Fact]
        public void Describe_Test02()
        {
            //columns: vendor_id,rate_code,passenger_count,trip_time_in_secs,trip_distance,payment_type,fare_amount  
            var tf = DataFrame.FromCsv("..\\..\\..\\testdata\\desc_test_ds.csv");

            var descDf = tf.Describe();
            var ss = descDf.ToStringBuilder();
            Assert.Equal(0.257315, descDf["rate_code", 5]);
            Assert.Equal(1.452008, descDf["passenger_count", 5]);
            Assert.Equal(459.957421, descDf["trip_time_in_secs", 5]);
            Assert.Equal(3.097768, descDf["trip_distance", 5]);
            Assert.Equal(8.864121, descDf["fare_amount", 5]);

            Assert.Equal(1.0, descDf["rate_code", 8]);
            Assert.Equal(1.0, descDf["passenger_count", 8]);
            Assert.Equal(553.0, descDf["trip_time_in_secs", 8]);
            Assert.Equal(1.7, descDf["trip_distance", 8]);
            Assert.Equal(9.0, descDf["fare_amount", 8]);

        }
    }
}
