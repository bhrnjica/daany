using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Ext;
using Microsoft.ML;

namespace Unit.Test.DF
{
    public class DF_Describe_Tests
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
            var descDf = df.Describe();

            Assert.True(descDf.RowCount() == 11);
            Assert.True(descDf.ColCount() == 3);


            Assert.True(descDf.Index[0].ToString().Equals("count"));
            Assert.True(descDf.Index[1].ToString().Equals("unique"));
            Assert.True(descDf.Index[2].ToString().Equals("top"));
            Assert.True(descDf.Index[3].ToString().Equals("freq"));
            Assert.True(descDf.Index[4].ToString().Equals("mean"));
            Assert.True(descDf.Index[5].ToString().Equals("std"));
            Assert.True(descDf.Index[6].ToString().Equals("min"));
            Assert.True(descDf.Index[7].ToString().Equals("25%"));
            Assert.True(descDf.Index[8].ToString().Equals("50%"));
            Assert.True(descDf.Index[9].ToString().Equals("75%"));
            Assert.True(descDf.Index[10].ToString().Equals("max"));


            Assert.True(descDf["quantity", 1].ToString().Equals("NaN"));
            Assert.True(descDf["product_id", 2].ToString().Equals("NaN"));
            Assert.True(descDf["quantity", 3].ToString().Equals("NaN"));
            Assert.True(descDf["quantity", 4].ToString().Equals("18.14286"));
            Assert.True(descDf["product_id", 5].ToString().Equals("0.48795"));
            Assert.True(descDf["product_id", 6].ToString().Equals("1"));

           
            //describe all columns
            var descDf2 = df.Describe(numericOnly: false);

            Assert.True(descDf2.RowCount() == 11);
            Assert.True(descDf2.ColCount() == 5);


            Assert.True(descDf2.Index[ 0].ToString().Equals("count"));
            Assert.True(descDf2.Index[ 1].ToString().Equals("unique"));
            Assert.True(descDf2.Index[ 2].ToString().Equals("top"));
            Assert.True(descDf2.Index[ 3].ToString().Equals("freq"));
            Assert.True(descDf2.Index[ 4].ToString().Equals("mean"));
            Assert.True(descDf2.Index[ 5].ToString().Equals("std"));
            Assert.True(descDf2.Index[ 6].ToString().Equals("min"));
            Assert.True(descDf2.Index[ 7].ToString().Equals("25%"));
            Assert.True(descDf2.Index[ 8].ToString().Equals("50%"));
            Assert.True(descDf2.Index[ 9].ToString().Equals("75%"));
            Assert.True(descDf2.Index[ 10].ToString().Equals("max"));
                              

            Assert.True(descDf2["quantity", 2].ToString().Equals("NaN"));
            Assert.True(descDf2["product_id", 3].ToString().Equals("NaN"));
            Assert.True(descDf2["retail_price", 4].ToString().Equals("4.142857"));
            Assert.Equal("22.9378", descDf2["quantity", 5].ToString());
            Assert.True(descDf2["city", 1].ToString().Equals("4"));
            Assert.True(descDf2["state", 0].ToString().Equals("7"));




            //describe all columns
            var descDf3 = df.Describe(numericOnly: false, "quantity", "city");

            Assert.True(descDf3.RowCount() == 11);
            Assert.True(descDf3.ColCount() == 2);


            Assert.True(descDf3.Index[0].ToString().Equals("count"));
            Assert.True(descDf3.Index[ 1].ToString().Equals("unique"));
            Assert.True(descDf3.Index[ 2].ToString().Equals("top"));
            Assert.True(descDf3.Index[ 3].ToString().Equals("freq"));
            Assert.True(descDf3.Index[ 4].ToString().Equals("mean"));
            Assert.True(descDf3.Index[ 5].ToString().Equals("std"));
            Assert.True(descDf3.Index[ 6].ToString().Equals("min"));
            Assert.True(descDf3.Index[ 7].ToString().Equals("25%"));
            Assert.True(descDf3.Index[ 8].ToString().Equals("50%"));
            Assert.True(descDf3.Index[ 9].ToString().Equals("75%"));
            Assert.True(descDf3.Index[ 10].ToString().Equals("max"));


            Assert.True(descDf3["quantity", 2].ToString().Equals("NaN"));
            Assert.True(descDf3["quantity", 3].ToString().Equals("NaN"));
            Assert.True(descDf3["quantity", 4].ToString().Equals("18.14286"));
            Assert.Equal("22.9378", descDf3["quantity", 5].ToString());
            Assert.True(descDf3["city", 1].ToString().Equals("4"));
            Assert.True(descDf3["city", 0].ToString().Equals("7"));


        }

    }

}
