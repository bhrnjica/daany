using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class DataFrame_Tests
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
        public void AddCalculatedColumn_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
                { "col2",new List<object>() { 2,12,22,32,42,52,62,72,82,92 } },
                { "col3",new List<object>() { 3,13,23,33,43,53,63,73,83,93 } },
                { "col4",new List<object>() { 4,14,24,34,44,54,64,74,84,94} },
                { "col5",new List<object>() { 5,15,25,35,45,55,65,75,85,95 } },
                { "col6",new List<object>() { 6,16,26,36,46,56,66,76,86,96} },
                { "col7",new List<object>() { 7,17,27,37,47,57,67,77,87,97 } },
                { "col8",new List<object>() { 8,18,28,38,48,58,68,78,88,98} },
                { "col9",new List<object>() { 9,19,29,39,49,59,69,79,89,99} },
                { "col10",new List<object>() { 10,20,30,40,50,60,70,80,90,100} },
            };
            //
            var df = new DataFrame(dict);
            var df01 = df.AddCalculatedColumn("col11", (row, i) => i + 11);

            //column test
            var c1 = new int[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            var cc1 = df["col11"].ToList();
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c1[i], cc1[i]);

        }

        [Fact]
        public void RemoveMissingValue_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() {  1, 11, "", 31, 41, 51, "", 71, 81, "?"} },
                { "col2",new List<object>() {  2, 12, 22,"?", 42, 52, 62, 72, 82, 92 } },
                { "col3",new List<object>() {  3, "", 23, 33, 43, 53, 63, 73, 83, 93 } },
                { "col4",new List<object>() {  4, 14, 24, 34, 44, 54, 64, 74, 84, 94} },
                { "col5",new List<object>() {"?", 15, 25, 35, 45, "", 65, "", 85, 95 } },

            };
            //
            var df = new DataFrame(dict);

            //
            var c1 = new int[] { 41, 42, 43, 44, 45, 81, 82, 83, 84, 85 };
            var c2 = new object[] { 11, 12, DataFrame.NAN, 14, 15, 41, 42, 43, 44, 45, 81, 82, 83, 84, 85 };
            var df0 = df.DropNA();

            var df02 = new int[] { 4, 14, 24, 34, 44, 54, 64, 74, 84, 94 };
            for (int i = 0; i < c1.Length; i++)
                Assert.Equal(c1[i], (int)df0.Values[i]);

            var cc1 = df0["col1"].ToList();
            Assert.Equal(41, cc1[0]);
            Assert.Equal(81, cc1[1]);

            cc1 = df0["col2"].ToList();
            Assert.Equal(42, cc1[0]);
            Assert.Equal(82, cc1[1]);

            cc1 = df0["col3"].ToList();
            Assert.Equal(43, cc1[0]);
            Assert.Equal(83, cc1[1]);

            cc1 = df0["col4"].ToList();
            Assert.Equal(44, cc1[0]);
            Assert.Equal(84, cc1[1]);

            cc1 = df0["col5"].ToList();
            Assert.Equal(45, cc1[0]);
            Assert.Equal(85, cc1[1]);


            ///////////////////////////////////////////
            var df1 = df.DropNA("col1", "col2", "col5");
            for (int i = 0; i < c2.Length; i++)
            {
                if(c2[i] != DataFrame.NAN)
                    Assert.Equal(c2[i].ToString(), df1.Values[i].ToString());
                else
                    Assert.Equal(c2[i], df1.Values[i]);
            }
                

            cc1 = df1["col1"].ToList();
            Assert.Equal(11, cc1[0]);
            Assert.Equal(41, cc1[1]);
            Assert.Equal(81, cc1[2]);

            cc1 = df1["col2"].ToList();
            Assert.Equal(12, cc1[0]);
            Assert.Equal(42, cc1[1]);
            Assert.Equal(82, cc1[2]);

            cc1 = df1["col3"].ToList();
            Assert.Equal(DataFrame.NAN, cc1[0]);
            Assert.Equal(43, cc1[1]);
            Assert.Equal(83, cc1[2]);

            cc1 = df1["col4"].ToList();
            Assert.Equal(14, cc1[0]);
            Assert.Equal(44, cc1[1]);
            Assert.Equal(84, cc1[2]);

            cc1 = df1["col5"].ToList();
            Assert.Equal(15, cc1[0]);
            Assert.Equal(45, cc1[1]);
            Assert.Equal(85, cc1[2]);


        }

        [Fact]
        public void ReplaceMissingValue_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() {  1, 11, "", 31, 41, 51, "", 71, 81, "?"} },
                { "col2",new List<object>() {  2, 12, 22,"?", 42, 52, 62, 72, 82, 92 } },
                { "col3",new List<object>() {  3, "", 23, 33, 43, 53, 63, 73, 83, 93 } },
                { "col4",new List<object>() {  4, 14, 24, 34, 44, 54, 64, 74, 84, 94} },
                { "col5",new List<object>() {"?", 15, 25, 35, 45, "", 65, "", 85, 95 } },

            };
            //
            var df = new DataFrame(dict);
            var val = df["col1"].Where(x => x!=DataFrame.NAN).Average(x => (int)x);
            df.FillNA("col1", val);
            var col1 = df["col1"].ToList();
            Assert.Equal(col1[2], val);
            Assert.Equal(col1[2], val);
            Assert.Equal(df[2].ElementAt(0), val);
            Assert.Equal(df[6].ElementAt(0), val);


        }

        [Fact]
        public void TakeEveryNthRow_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
                { "col2",new List<object>() { 2,12,22,32,42,52,62,72,82,92 } },
                { "col3",new List<object>() { 3,13,23,33,43,53,63,73,83,93 } },
                { "col4",new List<object>() { 4,14,24,34,44,54,64,74,84,94} },
                { "col5",new List<object>() { 5,15,25,35,45,55,65,75,85,95 } },
                { "col6",new List<object>() { 6,16,26,36,46,56,66,76,86,96} },
                { "col7",new List<object>() { 7,17,27,37,47,57,67,77,87,97 } },
                { "col8",new List<object>() { 8,18,28,38,48,58,68,78,88,98} },
                { "col9",new List<object>() { 9,19,29,39,49,59,69,79,89,99} },
                { "col10",new List<object>() { 10,20,30,40,50,60,70,80,90,100} },
            };
            //
            var df = new DataFrame(dict);
            var df1 = df.TakeEvery(2);
            //row test
            var r1 = df1[0].ToList();
            var r2 = df1[1].ToList();
            var r3 = df1[2].ToList();
            var e1 = new int[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var e2 = new int[] { 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 };
            var e3 = new int[] { 51, 52, 53, 54, 55, 56, 57, 58, 59, 60 };

            //check for row count
            Assert.Equal(5, df1.RowCount());

            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r1[i], e1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r2[i], e2[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r3[i], e3[i]);

            //every 3rth
            var df2 = df.TakeEvery(3);
            //row test
            var r21 = df2[0].ToList();
            var r22 = df2[1].ToList();
            var r23 = df2[2].ToList();
            var e21 = new int[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
            var e22 = new int[] { 51, 52, 53, 54, 55, 56, 57, 58, 59, 60 };
            var e23 = new int[] { 81, 82, 83, 84, 85, 86, 87, 88, 89, 90 };

            //check for row count
            Assert.Equal(3, df2.RowCount());

            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r21[i], e21[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r22[i], e22[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r23[i], e23[i]);

        }

        [Fact]
        public void SortBy_QuickSort_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,31,41,51,61,11,21,71,81,91} },
                { "col2",new List<object>() { 2,32,42,52,62,12,22,72,82,92 } },
                { "col3",new List<object>() { 3,43,33,63,53,13,23,73,83,93 } },
                { "col4",new List<object>() { 4,54,44,34,64,14,24,74,84,94} },
               
            };
            //
            var df = new DataFrame(dict);

            var dict1 = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
                { "col2",new List<object>() { 2,12,22,32,42,52,62,72,82,92 } },
                { "col3",new List<object>() { 3,13,23,43,33,63,53,73,83,93 } },
                { "col4",new List<object>() { 4,14,24,54,44,34,64,74,84,94} },
            };
            //
            var df1 = new DataFrame(dict1);

            var result = df.SortBy(new string[] { "col1", "col2", "col3", "col4" });

            for (int i = 0; i < result.Values.Count; i++)
            {
                var expected = Convert.ToInt32(df1.Values[i]);
                var actual = Convert.ToInt32(result.Values[i]);
                Assert.Equal<int>(expected, actual);
            }
        }

        [Fact]
        public void SortBy_QuickSort_Test03()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filepath: $"..\\..\\..\\testdata\\sort_sample02.txt", sep: ",", names: null, dformat: null);
            var expectedDf1 = DataFrame.FromCsv(filepath: $"..\\..\\..\\testdata\\sort_sample02.txt", sep: ",", names: null, dformat: null);


            var result = sampleDf.SortBy(new string[] { "machineID", "datetime"});

            for (int i = 0; i < result.Values.Count; i++)
            {
                Assert.Equal<object>(expectedDf1.Values[i], result.Values[i]);
            }
          
        }

        [Fact]
        public void SortBy_MergeSort_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,31,41,51,61,11,21,71,81,91} },
                { "col2",new List<object>() { 2,32,42,52,62,12,22,72,82,92 } },
                { "col3",new List<object>() { 3,43,33,63,53,13,23,73,83,93 } },
                { "col4",new List<object>() { 4,54,44,34,64,14,24,74,84,94} },

            };
            //
            var df = new DataFrame(dict);

            var dict1 = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
                { "col2",new List<object>() { 2,12,22,32,42,52,62,72,82,92 } },
                { "col3",new List<object>() { 3,13,23,43,33,63,53,73,83,93 } },
                { "col4",new List<object>() { 4,14,24,54,44,34,64,74,84,94} },
            };
            //
            var df1 = new DataFrame(dict1);

            var result = df.SortBy(new string[] { "col1", "col2", "col3", "col4" }, qsAlgo: false);

            for (int i = 0; i < result.Values.Count; i++)
            {
                var expected = Convert.ToInt32(df1.Values[i]);
                var actual = Convert.ToInt32(result.Values[i]);
                Assert.Equal<int>(expected, actual);
            }
        }

        [Fact]
        public void SortBy_MergeSort_Test02()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filepath: $"..\\..\\..\\testdata\\sort_sample.txt", sep: "\t", names: null, dformat: null);

            //col1,col2,col3,col4
            var expectedDf = DataFrame.FromCsv(filepath: $"..\\..\\..\\testdata\\sort_expected.txt", sep: "\t", names: null, dformat: null);
            var expectedDf1 = DataFrame.FromCsv(filepath: $"..\\..\\..\\testdata\\sort_sample.txt", sep: "\t", names: null, dformat: null);


            var result = sampleDf.SortBy(new string[] { "Col1", "Col2", "Col3", "Col4" }, qsAlgo: false);

            for (int i = 0; i < result.Values.Count; i++)
            {
                var expected = Convert.ToInt32(expectedDf.Values[i]);
                var actual = Convert.ToInt32(result.Values[i]);
                Assert.Equal<int>(expected, actual);
            }
            //test the original dataframe remained unordered
            for (int i = 0; i < sampleDf.Values.Count; i++)
            {
                var expected = Convert.ToInt32(sampleDf.Values[i]);
                var actual = Convert.ToInt32(expectedDf1.Values[i]);
                Assert.Equal<int>(expected, actual);
            }
        }

        [Fact]
        public void SortBy_MergeSort_Test03()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filepath: $"..\\..\\..\\testdata\\sort_sample02.txt", sep: ",", names: null, dformat: null);
            var expectedDf1 = DataFrame.FromCsv(filepath: $"..\\..\\..\\testdata\\sort_sample02.txt", sep: ",", names: null, dformat: null);


            var result = sampleDf.SortBy(new string[] { "machineID", "datetime" }, qsAlgo:false);

            for (int i = 0; i < result.Values.Count; i++)
            {
                Assert.Equal<object>(expectedDf1.Values[i], result.Values[i]);
            }

        }

    }

}
