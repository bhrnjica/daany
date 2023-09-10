using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Ext;


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
            df = df.TransformColumn("state", ColumnTransformer.OneHot).df;

            Assert.Equal("CA", df.Columns[5]);
            Assert.Equal("FL", df.Columns[6]);
            Assert.Equal("PR", df.Columns[7]);
            //
            Assert.Equal(1,df[0,5]);
            Assert.Equal(1,df[4, 6]);
            Assert.Equal(1,df[5, 6]);
            Assert.Equal(1, df[6, 7]);
        }

        [Fact]
        public void CategoryToOrdinal_Test01()
        {
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
            df = df.TransformColumn("state",ColumnTransformer.Ordinal, transformedColumnsOnly: true).df;
            var col = df["state_cvalues"].Select(x=>Convert.ToInt32(x)).ToArray();
            Assert.Equal(0, col[0]);
            Assert.Equal(0, col[1]);
            Assert.Equal(0, col[2]);
            Assert.Equal(0, col[3]);
            Assert.Equal(1, col[4]);
            Assert.Equal(1, col[5]);
            Assert.Equal(2, col[6]);

        }

        [Fact]
        public void CategoryEncoder_Test01()
        {
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
            (DataFrame edf,float[] fValues, string[] classes) = df.TransformColumn("state", ColumnTransformer.Dummy,true);

            Assert.Equal(classes.Length-1, edf.ColCount());
            Assert.Equal(new List<object> { 1, 0 }, edf[0]);
            Assert.Equal(new List<object> { 1, 0 }, edf[1]);
            Assert.Equal(new List<object> { 1, 0 }, edf[2]);
            Assert.Equal(new List<object> { 1, 0 }, edf[3]);
            Assert.Equal(new List<object> { 0, 1 }, edf[4]);
            Assert.Equal(new List<object> { 0, 1 }, edf[5]);
            Assert.Equal(new List<object> { 0, 0 }, edf[6]);

        }

        [Fact]
        public void CategoryEncoder_Test02()
        {
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
            (DataFrame edf,float[] fValues, string[] classes) = df.TransformColumn("state", ColumnTransformer.OneHot, true);

            Assert.Equal(classes.Length, edf.ColCount());
            Assert.Equal(new List<object> { 1, 0 ,0 }, edf[0]);
            Assert.Equal(new List<object> { 1, 0 ,0 }, edf[1]);
            Assert.Equal(new List<object> { 1, 0 ,0 }, edf[2]);
            Assert.Equal(new List<object> { 1, 0 ,0 }, edf[3]);
            Assert.Equal(new List<object> { 0, 1 ,0 }, edf[4]);
            Assert.Equal(new List<object> { 0, 1 ,0 }, edf[5]);
            Assert.Equal(new List<object> { 0, 0 ,1 }, edf[6]);

        }

        [Fact]
        public void CategoryEncoder_Test03()
        {
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
            (DataFrame edf, float[] fValues, string[] classes) = df.TransformColumn("state", ColumnTransformer.Ordinal, true);

            Assert.Equal(1, edf.ColCount());
            Assert.Equal(new List<object> { 0 }, edf[0]);
            Assert.Equal(new List<object> { 0 }, edf[1]);
            Assert.Equal(new List<object> { 0 }, edf[2]);
            Assert.Equal(new List<object> { 0 }, edf[3]);
            Assert.Equal(new List<object> { 1 }, edf[4]);
            Assert.Equal(new List<object> { 1 }, edf[5]);
            Assert.Equal(new List<object> { 2 }, edf[6]);

        }
        [Fact]
        public void DataFrame_Shape_Test()
        {
            var lst = new List<object>() {  1, "Sarajevo",  77000, "BiH", true,     3.14, DateTime.Now.AddDays(-20),
                                            2, "Seattle",   98101, "USA", false,    3.21, DateTime.Now.AddDays(-10),
                                            3, "Berlin",    10115, "GER", false,    4.55, DateTime.Now.AddDays(-5),
                                        };
            //define column header for the DataFrame
            var columns = new List<string>() { "ID", "City", "Zip Code", "State", "IsHome", "Values", "Date" };

            //create data frame with 3 rows and 7 columns
            var df = new DataFrame(lst, columns, null);
            var encodedDf = df.TransformColumn("City", ColumnTransformer.Ordinal);
            //check the size of the data frame
            Assert.Equal((3, 7), df.Shape);
            var str = df.Shape.ToString();
            Assert.Equal("(3, 7)", str);

        }

        [Fact]
        public void CategoryEncoder_Test04()
        {
            var dict = new Dictionary<string, List<object>>
            {
                {"product_id",new List<object>() {1,2,3,4,5,6,7 } },
                {"on_stock",new List<object>() {0,1,0,0,0,0,1 } },
                { "retail_price",new List<object>() { 2,2,5,5,5,5,5 } },
                { "quantity",new List<object>() { 1,2,4,8,16,32,64 } },
                { "city",new List<object>() { "SF","SJ","SF","SJ","Miami", "Orlando","SJ"} },
                { "state" ,new List<object>() { "CA","CA","CA","CA","FL","FL","PR" } },
            };


            //
            var df = new DataFrame(dict);

            //add one hot encoding columns
            (DataFrame edf, float[] fValues, string[] classes) = df.TransformColumn("on_stock", ColumnTransformer.Binary1, true);

            Assert.Equal(1, edf.ColCount());
            Assert.Equal(new List<object> { 0 }, edf[0]);
            Assert.Equal(new List<object> { 1 }, edf[1]);
            Assert.Equal(new List<object> { 0 }, edf[2]);
            Assert.Equal(new List<object> { 0 }, edf[3]);
            Assert.Equal(new List<object> { 0 }, edf[4]);
            Assert.Equal(new List<object> { 0 }, edf[5]);
            Assert.Equal(new List<object> { 1 }, edf[6]);

        }

        [Fact]
        public void CategoryEncoder_Test05()
        {
            var dict = new Dictionary<string, List<object>>
            {
                {"product_id",new List<object>() {1,2,3,4,5,6,7 } },
                {"on_stock",new List<object>() {0,1,0,0,0,0,1 } },
                { "retail_price",new List<object>() { 2,2,5,5,5,5,5 } },
                { "quantity",new List<object>() { 1,2,4,8,16,32,64 } },
                { "city",new List<object>() { "SF","SJ","SF","SJ","Miami", "Orlando","SJ"} },
                { "state" ,new List<object>() { "CA","CA","CA","CA","FL","FL","PR" } },
            };


            //
            var df = new DataFrame(dict);

            //add one hot encoding columns
            (DataFrame edf, float[] fValues, string[] classes) = df.TransformColumn("on_stock", ColumnTransformer.Binary2, true);

            Assert.Equal(1, edf.ColCount());
            Assert.Equal(new List<object> { -1 }, edf[0]);
            Assert.Equal(new List<object> { 1 }, edf[1]);
            Assert.Equal(new List<object> { -1 }, edf[2]);
            Assert.Equal(new List<object> { -1 }, edf[3]);
            Assert.Equal(new List<object> { -1 }, edf[4]);
            Assert.Equal(new List<object> { -1 }, edf[5]);
            Assert.Equal(new List<object> { 1 }, edf[6]);

        }

        [Fact]
        public void MinMaxNormalization_Test05()
        {
            var dict = new Dictionary<string, List<object>>
            {
                {"product_id",new List<object>() {1,2,3,4,5,6,7 } },
                {"on_stock",new List<object>() {0,1,0,0,0,0,1 } },
                { "retail_price",new List<object>() { 2f,10f,5f,15f,25f,31f,42f } },
                { "quantity",new List<object>() { 1,2,4,8,16,32,64 } },
                { "city",new List<object>() { "SF","SJ","SF","SJ","Miami", "Orlando","SJ"} },
                { "state" ,new List<object>() { "CA","CA","CA","CA","FL","FL","PR" } },
            };


            //
            var df = new DataFrame(dict);

            //add one hot encoding columns
            (DataFrame edf, float[] fValues, string[] classes) = df.TransformColumn("retail_price", ColumnTransformer.MinMax, true);

            Assert.Equal(1, edf.ColCount());
            Assert.Equal(0d ,     Convert.ToDouble(edf[0].First()),3);
            Assert.Equal(0.2d ,   Convert.ToDouble(edf[1].First()),3);
            Assert.Equal(0.075d , Convert.ToDouble(edf[2].First()),3);
            Assert.Equal(0.325d,  Convert.ToDouble(edf[3].First()),3);
            Assert.Equal(0.575d , Convert.ToDouble(edf[4].First()),3);
            Assert.Equal(0.725d , Convert.ToDouble(edf[5].First()),3);
            Assert.Equal(1d ,     Convert.ToDouble(edf[6].First()),3);

        }

        [Fact]
        public void ZScoreStandardization_Test05()
        {
            var dict = new Dictionary<string, List<object>>
            {
                {"product_id",new List<object>() {1,2,3,4,5,6,7 } },
                {"on_stock",new List<object>() {0,1,0,0,0,0,1 } },
                { "retail_price",new List<object>() { 2f,10f,5f,15f,25f,31f,42f } },
                { "quantity",new List<object>() { 1,2,4,8,16,32,64 } },
                { "city",new List<object>() { "SF","SJ","SF","SJ","Miami", "Orlando","SJ"} },
                { "state" ,new List<object>() { "CA","CA","CA","CA","FL","FL","PR" } },
            };


            //
            var df = new DataFrame(dict);

            //add one hot encoding columns
            (DataFrame edf, float[] fValues, string[] classes) = df.TransformColumn("retail_price", ColumnTransformer.Standardizer, true);

            Assert.Equal(1, edf.ColCount());
            Assert.Equal( -1.13029d, Convert.ToDouble(edf[0].First()), 5);
            Assert.Equal( -0.58463d, Convert.ToDouble(edf[1].First()), 5);
            Assert.Equal( -0.92567d, Convert.ToDouble(edf[2].First()), 5);
            Assert.Equal( -0.24360d, Convert.ToDouble(edf[3].First()), 5);
            Assert.Equal( 0.43847d,  Convert.ToDouble(edf[4].First()), 5);
            Assert.Equal( 0.84772d , Convert.ToDouble(edf[5].First()), 5);
            Assert.Equal( 1.59799d , Convert.ToDouble(edf[6].First()), 5);
        }



    }

}
