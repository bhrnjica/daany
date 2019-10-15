using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class DataFrame_Grouping_Tests
    {
        [Fact]
        public void GroupBy_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                {"product_id",new List<object>() {1,1,2,2,2,2,2 } },
                { "retail_price",new List<object>() { 2,2,5,5,5,5,5 } },
                { "quantity",new List<object>() { 1,2,4,8,16,32,64 } },
                { "city",new List<object>() { "SF","SJ","SF","SJ","Miami", "Orlando","SJ"} },
                { "state" ,new List<object>() { "CA","CA","CA","CA","FL","FL","PR" } },
            };

            var c1 = new object[] { 1, 2, 1, "SF", "CA", 2, 5, 4, "SF", "CA" };

            //
            var df = new DataFrame(dict);
            var agg = new Dictionary<string, Aggregation>()
            { 
                { "quantity", Aggregation.Sum }, 
                { "retail_price", Aggregation.Avg },
                { "C", Aggregation.Avg } };

            //
            var group = df.GroupBy("city");

            Assert.Equal(4, group.Keys.Count);
            Assert.Equal((object)"SF", group.Keys[0]);
            Assert.Equal(2, group[group.Keys[0]].RowCount());

            Assert.Equal((object)"SJ", group.Keys[1]);
            Assert.Equal(3, group[group.Keys[1]].RowCount());

            Assert.Equal((object)"Miami", group.Keys[2]);
            Assert.Equal(1, group[group.Keys[2]].RowCount());

            Assert.Equal((object)"Orlando", group.Keys[3]);
            Assert.Equal(1, group[group.Keys[3]].RowCount());

            Assert.Equal(7, df.RowCount());

            var gdf = group[group.Keys[0]];
            for (int i = 0; i < 10; i++)
                Assert.Equal(gdf.Values[i], c1[i]);

        }

        [Fact]
        public void GroupBy_TwoColumns_Test02()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\group_sample_testdata.txt", sep: '\t', names: null, dformat: null);
            var resultDf = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\group_result_testdata.txt", sep: '\t', names: null, dformat: null);


            var result = sampleDf.GroupBy("col1", "col2" ).Aggregation(Aggregation.Sum);

            Assert.Equal(result.RowCount(), resultDf.RowCount());
            for (int i = 0; i < result.Values.Count; i++)
            {
                var expected = Convert.ToInt32(resultDf.Values[i]);
                var actual = Convert.ToInt32(result.Values[i]);
                Assert.Equal<int>(expected, actual);
            }

        }

        [Fact]
        public void RollingAggregation_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>()  { 1,2,3,4,5,6,7,8,9,10} },
                { "A",new List<object>()  { -2.385977,-1.004295,0.735167, -0.702657,-0.246845,2.463718, -1.142255,1.396598, -0.543425,-0.64050} },
                { "B",new List<object>()  { -0.102758,0.905829, -0.165272,-1.340923,0.211596, 3.157577, 2.340594, -1.647453,1.761277, 0.289374} },
                { "C",new List<object>()  { 0.438822, -0.954544,-1.619346,-0.706334,-0.901819,-1.380906,-0.039875,1.677227, -0.220481,-1.55067} },
                };
            //
            var df = new DataFrame(dict);
            var rollingdf = df.Rolling("ID", 3, Aggregation.Sum);

            //column test
            var c1 = new object[] { DataFrame.NAN, DataFrame.NAN, -2.655105, -0.971785, -0.214335, 1.514216, 1.074618, 2.718061, -0.289082, 0.212673 };

            var cc1 = rollingdf["A"].ToList();
            for (int i = 0; i < 10; i++)
            {
                if(cc1[i]!=null)
                    Assert.Equal((double)c1[i], (double)cc1[i], 6);
                else
                    Assert.Equal(c1[i], cc1[i]);
            }
                

        }

        [Fact]
        public void RollingAggregation_Test02()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>()  { 1,2,3,4,5,6,7,8,9,10} },
                { "A",new List<object>()  { -2.385977,-1.004295,0.735167, -0.702657,-0.246845,2.463718, -1.142255,1.396598, -0.543425,-0.64050} },
                { "B",new List<object>()  { -0.102758,0.905829, -0.165272,-1.340923,0.211596, 3.157577, 2.340594, -1.647453,1.761277, 0.289374} },
                { "C",new List<object>()  { 0.438822, -0.954544,-1.619346,-0.706334,-0.901819,-1.380906,-0.039875,1.677227, -0.220481,-1.55067} },
                };
            //
            var df = new DataFrame(dict);
            var agg = new Dictionary<string, Aggregation>() { { "A", Aggregation.Std }, { "B", Aggregation.Min }, { "C", Aggregation.Avg } };
            var rollingdf = df.Rolling("ID", 5, agg);

            //column test
            var c1 = new object[] { DataFrame.NAN, DataFrame.NAN, DataFrame.NAN, DataFrame.NAN, 1.139954626, 1.402105188, 1.433931328, 1.521038952, 1.494833302, 1.545961242 };
            var c2 = new object[] { DataFrame.NAN, DataFrame.NAN, DataFrame.NAN, DataFrame.NAN, -1.340923, -1.340923, -1.340923, -1.647453, -1.647453, -1.647453 };
            var c3 = new object[] { DataFrame.NAN, DataFrame.NAN, DataFrame.NAN, DataFrame.NAN, -0.7486442, -1.1125898, -0.929656, -0.2703414, -0.1731708, -0.302941 };

            var cc1 = rollingdf["A"].ToList();
            var cc2 = rollingdf["B"].ToList();
            var cc3 = rollingdf["C"].ToList();

            for (int i = 0; i < 10; i++)
            {
                if (c1[i] != DataFrame.NAN)
                    Assert.Equal((double)c1[i], (double)cc1[i], 5);
                else
                    Assert.Equal(c2[i], cc1[i]);
            }
                

            for (int i = 0; i < 10; i++)
            {
                if (c2[i] != DataFrame.NAN)
                    Assert.Equal((double)c2[i], (double)cc2[i], 5);
                else
                    Assert.Equal(c2[i], cc2[i]);
            }

            for (int i = 0; i < 10; i++)
            {
                if (c3[i] != DataFrame.NAN)
                    Assert.Equal((double)c3[i], (double)cc3[i], 5);
                else
                    Assert.Equal(c3[i], cc3[i]);
            }

        }

        [Fact]
        public void GroupByRolling_Test01()
        {
            //datetime, machineID, volt, rotate, pressure, vibration
            var telDf = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\group_rolling_testdata.txt", sep: '\t', names: null, dformat: null);


            //
            var agg = new Dictionary<string, Aggregation>()
            {
               { "datetime", Aggregation.Last }, { "volt", Aggregation.Avg }, { "rotate", Aggregation.Avg },
                { "pressure", Aggregation.Avg },{ "vibration", Aggregation.Avg }
            };

            var df = telDf.GroupBy("machineID").Rolling(3, 3, agg);

            var row1 = df[0].ToList();
            Assert.Equal("1", row1[5].ToString());
            Assert.Equal(170.0289916, (double)row1[1], 5);
            Assert.Equal(449.5338134, (double)row1[2], 5);
            Assert.Equal(94.59212239, (double)row1[3], 5);
            Assert.Equal(40.89350128, (double)row1[4], 5);
            Assert.Equal(new DateTime(2015, 1, 1, 8, 0, 0), row1[0]);
            var row2 = df[1].ToList();
            var row3 = df[2].ToList();
            var row4 = df[3].ToList();

            var row5 = df[17].ToList();
            Assert.Equal("2", row5[5].ToString());
            Assert.Equal(167.69433085123697, (double)row5[1], 5);
            Assert.Equal(437.20892333984375, (double)row5[2], 5);
            Assert.Equal(94.930048624674484, (double)row5[3], 5);
            Assert.Equal(40.247873942057289, (double)row5[4], 5);
            Assert.Equal(new DateTime(2015, 1, 1, 8, 0, 0), row1[0]);
            var row6 = df[18].ToList();
            var row7 = df[19].ToList();
            var row8 = df[20].ToList();


        }
    }

}
