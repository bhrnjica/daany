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
        public void Describe_Test03()
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

            Assert.Equal("count", descDf.Index[0].ToString());
            Assert.Equal("unique", descDf.Index[1].ToString());
            Assert.Equal("top", descDf.Index[2].ToString());
            Assert.Equal("mode", descDf.Index[3].ToString());
            Assert.Equal("mean", descDf.Index[4].ToString());
            Assert.Equal("std", descDf.Index[5].ToString());
            Assert.Equal("min", descDf.Index[6].ToString());
            Assert.Equal("25%", descDf.Index[7].ToString());
            Assert.Equal("50%", descDf.Index[8].ToString());
            Assert.Equal("75%", descDf.Index[9].ToString());
            Assert.Equal("max", descDf.Index[10].ToString());


            Assert.Equal(7f, Convert.ToSingle(descDf["product_id", 0]));
            Assert.Equal(7f, Convert.ToSingle(descDf["state", 0]));
            Assert.Equal(7f, Convert.ToSingle(descDf["quantity", 0]));

            Assert.Equal(float.NaN, Convert.ToSingle(descDf["product_id", 1]));
            Assert.Equal(3, Convert.ToSingle(descDf["state", 1]));
            Assert.Equal(float.NaN, Convert.ToSingle(descDf["quantity", 1]));

            Assert.Equal(float.NaN, Convert.ToSingle(descDf["product_id", 2]));
            Assert.Equal("CA", descDf["state", 2].ToString());
            Assert.Equal(float.NaN, Convert.ToSingle(descDf["quantity", 2]));

            Assert.Equal(2, Convert.ToSingle(descDf[10, 0]));
            Assert.Equal(64, Convert.ToSingle(descDf[10, 1]));
            Assert.Equal(float.NaN, Convert.ToSingle(descDf[10, 2]));

           
        }

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
            var descDf = df.Describe();

            Assert.True(descDf.RowCount() == 8);
            Assert.True(descDf.ColCount() == 3);


            Assert.Equal("count", descDf.Index[0].ToString());
            Assert.Equal("mean", descDf.Index[1].ToString());
            Assert.Equal("std", descDf.Index[2].ToString());
            Assert.Equal("min", descDf.Index[3].ToString());
            Assert.Equal("25%",descDf.Index[4].ToString());
            Assert.Equal("50%", descDf.Index[5].ToString());
            Assert.Equal("75%",descDf.Index[6].ToString());
            Assert.Equal("max", descDf.Index[7].ToString());


            Assert.Equal(7f, Convert.ToSingle(descDf["product_id", 0]));
            Assert.Equal(7f, Convert.ToSingle(descDf["retail_price", 0]));
            Assert.Equal(7f, Convert.ToSingle(descDf["quantity", 0]));

            Assert.Equal(1.714286f, Convert.ToSingle(descDf["product_id", 1]));
            Assert.Equal(4.142857f, Convert.ToSingle(descDf["retail_price", 1]));
            Assert.Equal(18.142857f, Convert.ToSingle(descDf["quantity", 1]));

            Assert.Equal(0.48795f, Convert.ToSingle(descDf["product_id", 2]));
            Assert.Equal(1.46385f, Convert.ToSingle(descDf["retail_price", 2]));
            Assert.Equal(22.937804f, Convert.ToSingle(descDf["quantity", 2]));

            Assert.Equal(1.5f, Convert.ToSingle(descDf["product_id", 4]));
            Assert.Equal(3.5f, Convert.ToSingle(descDf["retail_price", 4]));
            Assert.Equal(3f, Convert.ToSingle(descDf["quantity", 4]));

            Assert.Equal(2f, Convert.ToSingle(descDf["product_id", 6]));
            Assert.Equal(5f, Convert.ToSingle(descDf["retail_price", 6]));
            Assert.Equal(24f, Convert.ToSingle(descDf["quantity", 6]));


            //describe all columns
            var descDf2 = df.Describe(numericOnly: false);

            Assert.True(descDf2.RowCount() == 11);
            Assert.True(descDf2.ColCount() == 5);


            Assert.Equal("count", descDf2.Index[0].ToString());
            Assert.Equal("unique", descDf2.Index[1].ToString());
            Assert.Equal("top", descDf2.Index[2].ToString());
            Assert.Equal("mode", descDf2.Index[3].ToString());
            Assert.Equal("mean", descDf2.Index[4].ToString());
            Assert.Equal("std", descDf2.Index[5].ToString());
            Assert.Equal("min", descDf2.Index[6].ToString());
            Assert.Equal("25%", descDf2.Index[7].ToString());
            Assert.Equal("50%", descDf2.Index[8].ToString());
            Assert.Equal("75%", descDf2.Index[9].ToString());
            Assert.Equal("max", descDf2.Index[10].ToString());


            Assert.Equal(float.NaN, Convert.ToSingle(descDf2["product_id", 3]));
            Assert.Equal(4.142857f, Convert.ToSingle(descDf2["retail_price", 4]));
            Assert.Equal(float.NaN, Convert.ToSingle(descDf2["quantity", 2]));

            //describe all columns
            var descDf3 = df.Describe(numericOnly: false, "quantity", "city");

            Assert.True(descDf3.RowCount() == 11);
            Assert.True(descDf3.ColCount() == 2);



            Assert.Equal("count", descDf3.Index[0].ToString());
            Assert.Equal("unique", descDf3.Index[1].ToString());
            Assert.Equal("top", descDf3.Index[2].ToString());
            Assert.Equal("mode", descDf3.Index[3].ToString());
            Assert.Equal("mean", descDf3.Index[4].ToString());
            Assert.Equal("std", descDf3.Index[5].ToString());
            Assert.Equal("min", descDf3.Index[6].ToString());
            Assert.Equal("25%", descDf3.Index[7].ToString());
            Assert.Equal("50%", descDf3.Index[8].ToString());
            Assert.Equal("75%", descDf3.Index[9].ToString());
            Assert.Equal("max", descDf3.Index[10].ToString());

            Assert.Equal(float.NaN, Convert.ToSingle(descDf2["quantity", 2]));
            Assert.Equal(float.NaN, Convert.ToSingle(descDf2["quantity", 3]));
            Assert.Equal(18.142857f, Convert.ToSingle(descDf2["quantity", 4]));
            Assert.Equal(22.937804f, Convert.ToSingle(descDf2["quantity", 5]));

            Assert.Equal(4f, Convert.ToSingle(descDf2["city", 1]));
            Assert.Equal(7f, Convert.ToSingle(descDf2["city", 0]));

        }

        [Fact]
        public void Describe_Test02()
        {
            //columns: vendor_id,rate_code,passenger_count,trip_time_in_secs,trip_distance,payment_type,fare_amount  
            var tf = DataFrame.FromCsv("..\\..\\..\\testdata\\desc_test_ds.csv");

            var descDf = tf.Describe();

            Assert.Equal(0.257315f, Convert.ToSingle(descDf["rate_code", 2]));
            Assert.Equal(1.452008f, Convert.ToSingle(descDf["passenger_count", 2]));
            Assert.Equal(459.957421f, Convert.ToSingle(descDf["trip_time_in_secs", 2]));
            Assert.Equal(3.097768f, Convert.ToSingle(descDf["trip_distance", 2]));
            Assert.Equal(8.864121f, Convert.ToSingle(descDf["fare_amount", 2]));

        }
    }
}
