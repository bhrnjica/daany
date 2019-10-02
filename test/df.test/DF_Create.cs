using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class CreateDataFrame_Tests
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
        public void CreateTest01()
        {
            int row = 10;
            int col = 10;
            var nd = nc.ConsecutiveNum(row, col);
            //
            List<int> index = new List<int>();
            List<string> cols = new List<string>();
            CreateRowAndCol(row, col, ref index, ref cols);
            var df = new DataFrame(nd, index, cols);

            //row test
            var r1 = df[0].ToList();
            var r2 = df[4].ToList();
            var r3 = df[8].ToList();
            var e1 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var e2 = new int[] { 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };
            var e3 = new int[] { 81, 82, 83, 84, 85, 86, 87, 88, 89, 90 };
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r1[i], e1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r2[i], e2[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r3[i], e3[i]);

            //column test
            var c1 = new int[] { 1, 11, 21, 31, 41, 51, 61, 71, 81, 91 };
            var c2 = new int[] { 4, 14, 24, 34, 44, 54, 64, 74, 84, 94 };
            var c3 = new int[] { 8, 18, 28, 38, 48, 58, 68, 78, 88, 98 };
            var cc1 = df["col1"].ToList();
            var cc2 = df["col4"].ToList();
            var cc3 = df["col8"].ToList();
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c1[i], cc1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c2[i], cc2[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c3[i], cc3[i]);


            var cell = df["col1", 1];
            Assert.Equal(11, (int)cell);
            cell = df["col3", 5];
            Assert.Equal(53, (int)cell);


            cell = df[1, 1];
            Assert.Equal(12, (int)cell);
            cell = df[4, 3];
            Assert.Equal(44, (int)cell);
            cell = df[2, 8];
            Assert.Equal(29, (int)cell);
        }

        [Fact]
        public void CreateTest02()
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
            //row test
            var r1 = df[0].ToList();
            var r2 = df[4].ToList();
            var r3 = df[8].ToList();
            var e1 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var e2 = new int[] { 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };
            var e3 = new int[] { 81, 82, 83, 84, 85, 86, 87, 88, 89, 90 };
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r1[i], e1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r2[i], e2[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r3[i], e3[i]);

            //column test
            var c1 = new int[] { 1, 11, 21, 31, 41, 51, 61, 71, 81, 91 };
            var c2 = new int[] { 4, 14, 24, 34, 44, 54, 64, 74, 84, 94 };
            var c3 = new int[] { 8, 18, 28, 38, 48, 58, 68, 78, 88, 98 };
            var cc1 = df["col1"].ToList();
            var cc2 = df["col4"].ToList();
            var cc3 = df["col8"].ToList();
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c1[i], cc1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c2[i], cc2[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c3[i], cc3[i]);


            var cell = df["col1", 1];
            Assert.Equal(11, (int)cell);
            cell = df["col3", 5];
            Assert.Equal(53, (int)cell);


            cell = df[1, 1];
            Assert.Equal(12, (int)cell);
            cell = df[4, 3];
            Assert.Equal(44, (int)cell);
            cell = df[2, 8];
            Assert.Equal(29, (int)cell);
        }

        [Fact]
        public void SetCellValue_Test01()
        {
            int row = 10;
            int col = 10;
            var nd = nc.ConsecutiveNum(row, col);
            //
            List<int> index = new List<int>();
            List<string> cols = new List<string>();
            CreateRowAndCol(row, col, ref index, ref cols);
            var df = new DataFrame(nd, index, cols);

            var cell1 = (int)df["col1", 1];
            Assert.Equal(11, cell1);
            df["col1", 1] = 111;
            cell1 = (int)df["col1", 1];
            Assert.Equal(111, cell1);

            //
            var cell = df["col3", 5];
            Assert.Equal(53, (int)cell);
            df["col3", 5] = 555;
            cell = df["col3", 5];
            Assert.Equal(555, (int)cell);


            cell = df[1, 1];
            Assert.Equal(12, (int)cell);
            df[1, 1] = "str";
            var vall = df[1, 1];
            Assert.Equal("str", vall.ToString());


            cell = df[4, 3];
            Assert.Equal(44, (int)cell);
            df[4, 3] = 4444;
            cell = df[4, 3];
            Assert.Equal(4444, (int)cell);

            var cell2 = df[2, 8];
            Assert.Equal(29, (int)cell2);
            df[2, 8] = 2.345;
            cell2 = df[2, 8];
            Assert.Equal(2.345, (double)cell2);
        }


        [Fact]
        public void LoadromCSV_Test()
        {
            string path = "../../../testdata/titanic_full_1310.csv";
            var df = DataFrame.FromCsv(path, '\t', names:null); //

            //
            //row test
            var r1 = df[393].ToList();
            //2	0	Denbury Mr. Herbert	male	25	0	0	C.A. 31029	31.5000		S		
            var e1 = new object[] { 2, 0, "Denbury Mr. Herbert", "male", 25, 0, 0, "C.A. 31029", "31.5",DataFrame.NAN, "S", DataFrame.NAN, DataFrame.NAN, "Guernsey / Elizabeth NJ"};


            for (int i = 0; i < e1.Length; i++)
            {
                if (r1[i] == null)
                {
                    Assert.Null(r1[i]);
                    Assert.Null(e1[i]);
                }

                else
                {
                    object v1 = r1[i].ToString();
                    object v2 = e1[i].ToString();
                    Assert.True(v1.Equals(v2));
                }

            }
        }

        [Fact]
        public void LoadromCSV_Test2()
        {
            string path = "../../../testdata/titanic_train.csv";
            var df = DataFrame.FromCsv(path, ',', names: null); //

            //
            //row test
            var r1 = df[0].ToList();
            //1,0,3,Braund Mr. Owen Harris,male,22,1,0,A/5 21171,7.25,,S,youth		
            var e1 = new object[] { 1, 0, 3, "Braund Mr. Owen Harris", "male", 22, 1, 0, "A/5 21171", 7.25, DataFrame.NAN ,"S", "youth" };


            for (int i = 0; i < e1.Length; i++)
            {
                if(r1[i]==null)
                {
                    Assert.Null(r1[i]);
                    Assert.Null(e1[i]);
                }

                else
                {
                    object v1 = r1[i].ToString();
                    object v2 = e1[i].ToString();
                    Assert.True(v1.Equals(v2));
                }
                
            }
        }


        [Fact]
        public void SaveToCSV_Test()
        {

        }

        [Fact]
        public void CreateSeries_Test01()
        {
            var dFrom = new DateTime(2019, 01, 01,05,0,0);
            var dTo = new DateTime(2019, 01, 02, 05, 0, 0);
            var step = TimeSpan.FromHours(3);
            var serie = nc.GenerateDateSeries(dFrom, dTo, step);
            //
            Assert.Equal(new DateTime(2019, 01, 01, 05, 0, 0), serie[0]);
            Assert.Equal(new DateTime(2019, 01, 01, 08, 0, 0), serie[1]);
            Assert.Equal(new DateTime(2019, 01, 01, 11, 0, 0), serie[2]);
            Assert.Equal(new DateTime(2019, 01, 01, 14, 0, 0), serie[3]);
            Assert.Equal(new DateTime(2019, 01, 01, 17, 0, 0), serie[4]);
            Assert.Equal(new DateTime(2019, 01, 01, 20, 0, 0), serie[5]);
            Assert.Equal(new DateTime(2019, 01, 01, 23, 0, 0), serie[6]);
            Assert.Equal(new DateTime(2019, 01, 02, 02, 0, 0), serie[7]);
        }

        [Fact]
        public void CreateDoubleSeries_Test02()
        {
            var dFrom = 5.5;
            var dTo = 13.5;
            var count = 8;
            var serie = nc.GenerateDoubleSeries(dFrom, dTo, count);
            //
            Assert.Equal(5.5, serie[0]);
            Assert.Equal(6.5, serie[1]);
            Assert.Equal(7.5, serie[2]);
            Assert.Equal(8.5, serie[3]);
            Assert.Equal(9.5, serie[4]);
            Assert.Equal(10.5, serie[5]);
            Assert.Equal(11.5, serie[6]);
            Assert.Equal(12.5, serie[7]);
        }

        [Fact]
        public void JoinSingleColumn_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };
            var dict1 = new Dictionary<string, List<object>>
            {
                { "item2ID",new List<object>() {"foo", "bar", "baz","foo" } },
                { "value2",new List<object>() { 5,6,7,8 } },
            };
            //
            var df1 = new DataFrame(dict);
            var df2 = new DataFrame(dict1);
            var mergedDf = df1.Join(df2,new string[]{ "itemID"}, new string[] { "item2ID"}, JoinType.Inner,true);

            //row test
            var r1 = mergedDf[0].ToList();
            var r2 = mergedDf[1].ToList();
            var r3 = mergedDf[3].ToList();
            var e1 = new object[] { "foo", 1, "foo", 5 };
            var e2 = new object[] { "bar", 2, "bar", 6 };
            var e3 = new object[] { "foo", 4, "foo", 8 };
            for (int i = 0; i < r1.Count; i++)
                Assert.Equal(r1[i].ToString(), e1[i].ToString());
            for (int i = 0; i < r2.Count; i++)
                Assert.Equal(r2[i].ToString(), e2[i].ToString());
            for (int i = 0; i < r3.Count; i++)
                Assert.Equal(r3[i].ToString(), e3[i].ToString());

            //column test
            var c1 = new string[] { "foo","bar","baz","foo"};
            var c2 = new int[] { 1,2,3,4 };
            var c3 = new int[] { 5,6,7,8 };
            var cc1 = mergedDf["itemID"].ToList();
            var cc2 = mergedDf["value1"].ToList();
            var cc3 = mergedDf["value2"].ToList();
            for (int i = 0; i < c1.Count(); i++)
                Assert.Equal(c1[i].ToString(), cc1[i].ToString());
            for (int i = 0; i < c2.Count(); i++)
                Assert.Equal((int)c2[i], cc2[i]);
            for (int i = 0; i < c3.Count(); i++)
                Assert.Equal((int)c3[i], cc3[i]);
        }

        [Fact]
        public void JoinMultipleColumns_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "catId",new List<object>() { "A", "A", "B", "B" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };
            var dict1 = new Dictionary<string, List<object>>
            {
                { "item2ID",new List<object>() {"foo", "bar", "baz","foo" } },
                { "cat2ID",new List<object>() { "A", "B", "A", "B" } },
                { "value2",new List<object>() { 5,6,7,8 } },
            };
            //
            var df1 = new DataFrame(dict);
            var df2 = new DataFrame(dict1);
            var mergedDf = df1.Join(df2, new string[] { "itemID", "catID" }, new string[] { "item2ID", "cat2ID" }, JoinType.Inner);

            //row test
            var r1 = mergedDf[0].ToList();
            var r2 = mergedDf[1].ToList();
            
            var e1 = new object[] { "foo","A", 1, "foo","A", 5 };
            var e2 = new object[] { "foo","B", 4, "foo","B", 8 };
           
            for (int i = 0; i < r1.Count; i++)
                Assert.Equal(r1[i].ToString(), e1[i].ToString());
            for (int i = 0; i < r2.Count; i++)
                Assert.Equal(r2[i].ToString(), e2[i].ToString());
           
            //column test
            var c1 = new string[] { "foo", "foo" };
            var c2 = new string[] {"A","B"};
            var c3 = new int[] { 1,4 };
            var cc1 = mergedDf["itemID"].ToList();
            var cc2 = mergedDf["catID"].ToList();
            var cc3 = mergedDf["value1"].ToList();
            for (int i = 0; i < c1.Count(); i++)
                Assert.Equal(c1[i].ToString(), cc1[i].ToString());
            for (int i = 0; i < c2.Count(); i++)
                Assert.Equal(c2[i], cc2[i]);
            for (int i = 0; i < c3.Count(); i++)
                Assert.Equal((int)c3[i], cc3[i]);
        }

        [Fact]
        public void CreateDataFrameFromExisted_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "catId",new List<object>() { "A", "A", "B", "B" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };
            
            //
            var df1 = new DataFrame(dict);

            var df2 = df1.Create(("itemID",null), ("value1", "value"));

            //test
            var c1f1 = df1["itemID"].ToList();
            var c1f2 = df1["value1"].ToList();

            var c2f1 = df2["itemID"].ToList();
            var c2f2 = df2["value"].ToList();

            for (int i = 0; i < c1f1.Count(); i++)
                Assert.Equal(c1f1[i].ToString(), c2f1[i].ToString());
            for (int i = 0; i < c2f2.Count(); i++)
                Assert.Equal(c1f2[i], c2f2[i]);
           

        }

        [Fact]
        public void RemoveColumns_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "catId",new List<object>() { "A", "A", "B", "B" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };

            //
            var df1 = new DataFrame(dict);

            var df2 = df1.Remove("catId");

            //test
            var c1f1 = df1["itemID"].ToList();
            var c1f2 = df1["value1"].ToList();
            Assert.Equal(3, df1.Columns.Count);

            var c2f1 = df2["itemID"].ToList();
            var c2f2 = df2["value1"].ToList();
            Assert.Equal(2, df2.Columns.Count);

            for (int i = 0; i < c1f1.Count(); i++)
                Assert.Equal(c1f1[i].ToString(), c2f1[i].ToString());
            for (int i = 0; i < c2f2.Count(); i++)
                Assert.Equal(c1f2[i], c2f2[i]);


        }
    }
}
