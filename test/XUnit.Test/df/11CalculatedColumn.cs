using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class CalculatedColumnsTests
    {
      

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
            var sCols = new string[] { "col11" };
            var df01 = df.AddCalculatedColumns(sCols, (row, i) => calculate(row,i) );
            //local function declaration
            object[] calculate(object[] row, int i)
                {return new object[1] { i + 11 };}
            //column test
            var c1 = new int[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            var cc1 = df["col11"].ToList();
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c1[i], cc1[i]);

        }



        [Fact]
        public void AddColumns_Test01()
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
                
            };
            //
            var df = new DataFrame(dict);

            //define three new columns
            var d = new Dictionary<string, List<object>>
            {
                { "col8",new List<object>() { 8,18,28,38,48,58,68,78,88,98} },
                { "col9",new List<object>() { 9,19,29,39,49,59,69,79,89,99} },
                { "col10",new List<object>() { 10,20,30,40,50,60,70,80,90,100} },

            };

            //add three new columns
            var newDf =  df.AddColumns(d);
            
            Assert.Equal(7, df.ColCount());
            Assert.Equal(10, newDf.ColCount());

            for (int i = 0; i < newDf.Values.Count; i++)
                Assert.Equal(i+1, newDf.Values[i]);

        }


        [Fact]
        public void InsertColumn_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
                { "col2",new List<object>() { 2,12,22,32,42,52,62,72,82,92 } },
                { "col3",new List<object>() { 3,13,23,33,43,53,63,73,83,93 } },
                { "col4",new List<object>() { 4,14,24,34,44,54,64,74,84,94} },
               
                { "col6",new List<object>() { 6,16,26,36,46,56,66,76,86,96} },
                { "col7",new List<object>() { 7,17,27,37,47,57,67,77,87,97 } },
                { "col8",new List<object>() { 8,18,28,38,48,58,68,78,88,98} },
                { "col9",new List<object>() { 9,19,29,39,49,59,69,79,89,99} },
                { "col10",new List<object>() { 10,20,30,40,50,60,70,80,90,100} },
            };
            //
            var df = new DataFrame(dict);

            //define three new columns
            var d = new List<object>() { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 };

            //add three new columns
            df.InsertColumn("col5",d,4);
            Assert.Equal("col5", df.Columns[4]);

            for (int i = 0; i < df.Values.Count; i++)
                Assert.Equal(i + 1, df.Values[i]);

        }


        [Fact]
        public void InsertColumn_Test02()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
                { "col3",new List<object>() { 3,13,23,33,43,53,63,73,83,93 } },
                { "col4",new List<object>() { 4,14,24,34,44,54,64,74,84,94} },
                { "col7",new List<object>() { 7,17,27,37,47,57,67,77,87,97 } },
                { "col8",new List<object>() { 8,18,28,38,48,58,68,78,88,98} },
                { "col9",new List<object>() { 9,19,29,39,49,59,69,79,89,99} },
                { "col10",new List<object>() { 10,20,30,40,50,60,70,80,90,100} },
            };
            //
            var df = new DataFrame(dict);

            //define three new columns
            var d1 = new List<object>() { 2, 12, 22, 32, 42, 52, 62, 72, 82, 92 };
            var d2 = new List<object>() { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 };
            var d3 = new List<object>() { 6, 16, 26, 36, 46, 56, 66, 76, 86, 96 };

            //add three new columns
            df.InsertColumn("col2", d1, 1);
            df.InsertColumn("col5", d2, 4);
            df.InsertColumn("col6", d3, 5);


            Assert.Equal("col2", df.Columns[1]);
            Assert.Equal("col5", df.Columns[4]);
            Assert.Equal("col6", df.Columns[5]);
            for (int i = 0; i < df.Values.Count; i++)
                Assert.Equal(i + 1, df.Values[i]);

        }

        [Fact]
        public void InsertColumn_Test03()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
                { "col2",new List<object>() { 2,12,22,32,42,52,62,72,82,92 } },
                { "col3",new List<object>() { 3,13,23,33,43,53,63,73,83,93 } },
                { "col4",new List<object>() { 4,14,24,34,44,54,64,74,84,94} },
                { "col5",new List<object>() { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 } },
                { "col6",new List<object>() { 6,16,26,36,46,56,66,76,86,96} },
                { "col7",new List<object>() { 7,17,27,37,47,57,67,77,87,97 } },
                { "col8",new List<object>() { 8,18,28,38,48,58,68,78,88,98} },
                { "col9",new List<object>() { 9,19,29,39,49,59,69,79,89,99} },
            };
            //
            var df = new DataFrame(dict);

            //define three new columns
            var d = new List<object>() { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

            //add three new columns
            df.InsertColumn("col10", d);
            Assert.Equal("col10", df.Columns[9]);

            for (int i = 0; i < df.Values.Count; i++)
                Assert.Equal(i + 1, df.Values[i]);

        }
    }

}
