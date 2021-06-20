using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class CreateDataFrameTests
    {
        [Fact]
        public void CreateFromList_Test01()
        {
            //list of object
            var list = new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            //defined columns
            var cols = new string[] { "col1", "col2" };

            //create data frame with two columns and 5 rows.
            var df = new DataFrame(list, cols);

            //check the size of the data frame
            Assert.Equal(5, df.RowCount());
            Assert.Equal(2, df.ColCount());
        }

        [Fact]
        public void CreateFromList_Failed_Test01()
        {
            //list of object
            var list = new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,11 };
            //defined columns
            var cols = new string[] { "col1", "col2" };

            //exception the number of list object is not divisible with column counts
            var exception = Assert.ThrowsAny<System.Exception>(() => new DataFrame(list, cols));
            Assert.Equal("The Columns count must be divisible by data length.", exception.Message);
        }

        [Fact]
        public void CreateFromDictionary_Test02()
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
            
            //check the size of the data frame
            Assert.Equal(10, df.RowCount());
            Assert.Equal(10, df.ColCount());
        }

        [Fact]
        public void CreateFromDictionary_Failed_Test02()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91,101} },
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
            
            //exception the number of list object must all be the same
            var exception = Assert.ThrowsAny<System.Exception>(() => new DataFrame(dict));
            Assert.Equal("All lists within dictionary must be with same length.", exception.Message);
        }

        [Fact]
        public void CreateFromCSVFile_Test01()
        {
            var filePath = $"testdata/group_sample_testdata.txt";
            var df = DataFrame.FromCsv(filePath: filePath, 
                                                sep: '\t', 
                                                names: null, dformat: null);
            //check the size of the data frame
            Assert.Equal(27, df.RowCount());
            Assert.Equal(6, df.ColCount());
        }
        [Fact]
        public void CreateFromCSVFile_Failed_Test01()
        {
            var filePath = $"../../../testdata/group_sample_testdata1.txt";
            
            //invalid path
            var exception = Assert.ThrowsAny<System.ArgumentException>(() => DataFrame.FromCsv(filePath: filePath,
                                                                                        sep: '\t',
                                                                                        names: null, dformat: null));
            Assert.Equal("filePath (Parameter 'File name does not exist.')", exception.Message);
        }


        [Fact]
        public void CreateTest01()
        {
            int row = 10;
            int nCols = 10;
            var nd = nc.ConsecutiveNum(row, nCols);
            var cols = Enumerable.Range(1, nCols).Select(x => $"col{x}").ToList();

            var df = new DataFrame(nd, cols);

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
        public void CreateDataFrameFromExisted_ByChecking_ColTypes_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "catId",new List<object>() { "A", "A", "B", "B" } },
                { "value1",new List<object>() { 1,2,3,4 } },
                { "value2",new List<object>() { true,false,true,true } },
            };

            //
            var df1 = new DataFrame(dict);
            var cT = new ColType[] { ColType.STR, ColType.IN, ColType.I32, ColType.I2 };
            df1.SetColumnType("catId",ColType.IN);

            Assert.Equal(ColType.IN, df1.ColTypes[1]);

            //create new dataframe
            var newdf = df1["itemID", "catId", "value1", "value2"];
            Assert.Equal(newdf.ColTypes, cT);
        }



    }
}
