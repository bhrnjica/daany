using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Ext;
using Microsoft.ML;

namespace Unit.Test.DF
{
    public class DataFrameExtensionTests
    {
        private void CreateRowAndCol(int row, int col, ref List<int> indexs, ref List<string> columns)
        {
            for (int r = 0; r < row; r++)
            {
                indexs.Add(r);
            }
            for (int c = 0; c < col; c++)
            {
                columns.Add($"col{c + 1}");
            }

        }

        [Fact]
        public void OneHotEncoding_Test01()
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

            
            //
            var df = new DataFrame(dict);

            //add one hot encoding columns
            df.EncodeColumn(mlContext, "state");

            Assert.Equal("CA", df.Columns[5]);
            Assert.Equal("FL", df.Columns[6]);
            Assert.Equal("PR", df.Columns[7]);
            //
            Assert.Equal(1f,df[0,5]);
            Assert.Equal(1f,df[4, 6]);
            Assert.Equal(1f,df[5, 6]);
            Assert.Equal(1f, df[6, 7]);
        }

        [Fact]
        public void KeyToValue_Test01()
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


            //
            var df = new DataFrame(dict);

            //add one hot encoding columns
            df.CategoryToKey(mlContext, "state");
            var col = df["state_cvalues"].Select(x=>Convert.ToInt32(x)).ToArray();
            Assert.Equal(1, col[0]);
            Assert.Equal(1, col[1]);
            Assert.Equal(1, col[2]);
            Assert.Equal(1, col[3]);
            Assert.Equal(2, col[4]);
            Assert.Equal(2, col[5]);
            Assert.Equal(3, col[6]);

        }




    }

}
