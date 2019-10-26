using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class RowColumnIndexersTests
    {
        [Fact]
        public void CreateTest01()
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
            var newDf = df["col3", "col9", "col5", "col1"];

            //row test
            var e1 = new int[] { 3, 9, 5, 1 };
            var newdf1 = newDf[0].Select(x => Convert.ToInt32(x)).ToList();
            var e2 = new int[] { 23, 29, 25, 21 };
            var newdf3 = newDf[2].Select(x => Convert.ToInt32(x)).ToList();
            var e3 = new int[] { 93, 99, 95, 91 };
            var newdf5 = newDf[9].Select(x => Convert.ToInt32(x)).ToList();
            for (int i = 0; i < 4; i++)
                Assert.Equal((int)e1[i], newdf1[i]);
            for (int i = 0; i < 4; i++)
                Assert.Equal((int)e2[i], newdf3[i]);
            for (int i = 0; i < 4; i++)
                Assert.Equal((int)e3[i], newdf5[i]);


            //column test
            var c1 = new int[] { 1, 11, 21, 31, 41, 51, 61, 71, 81, 91 };
            var c3 = new int[] { 3, 13, 23, 33, 43, 53, 63, 73, 83, 93 };
            var c5 = new int[] { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 };
            var c9 = new int[] { 9, 19, 29, 39, 49, 59, 69, 79, 89, 99 };

            var cc1 = df["col1"].ToList();
            var cc3 = df["col3"].ToList();
            var cc5 = df["col5"].ToList();
            var cc9 = df["col9"].ToList();

            //
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c1[i], cc1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)cc3[i], c3[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)cc5[i], c5[i]);

            for (int i = 0; i < 10; i++)
                Assert.Equal((int)cc9[i], c9[i]);


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
            var newDf = df["col1", "col3", "col5", "col9"];

            //row test
            var e1 = new int[] { 1, 3, 5, 9 };
            var newdf1 = newDf[0].Select(x => Convert.ToInt32(x)).ToList();
            var e2 = new int[] { 21, 23, 25, 29 };
            var newdf3 = newDf[2].Select(x => Convert.ToInt32(x)).ToList();
            var e3 = new int[] { 91, 93, 95, 99 };
            var newdf5 = newDf[9].Select(x => Convert.ToInt32(x)).ToList();
            for (int i = 0; i < 4; i++)
                Assert.Equal((int)e1[i], newdf1[i]);
            for (int i = 0; i < 4; i++)
                Assert.Equal((int)e2[i], newdf3[i]);
            for (int i = 0; i < 4; i++)
                Assert.Equal((int)e3[i], newdf5[i]);


            //column test
            var c1 = new int[] { 1, 11, 21, 31, 41, 51, 61, 71, 81, 91 };
            var c3 = new int[] { 3, 13, 23, 33, 43, 53, 63, 73, 83, 93 };
            var c5 = new int[] { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 };
            var c9 = new int[] { 9, 19, 29, 39, 49, 59, 69, 79, 89, 99 };

            var cc1 = df["col1"].ToList();
            var cc3 = df["col3"].ToList();
            var cc5 = df["col5"].ToList();
            var cc9 = df["col9"].ToList();

            //
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c1[i], cc1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)cc3[i], c3[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)cc5[i], c5[i]);

            for (int i = 0; i < 10; i++)
                Assert.Equal((int)cc9[i], c9[i]);


        }


        [Fact]
        public void Enumerators_Test()
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
            var row1 = df.GetRowEnumerator().Skip(1).First();
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)row1[i], 11 + i);

            var row7 = df.GetRowEnumerator().Skip(7).First();
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)row7[i], 71 + i);


            var row8 = df.GetEnumerator().Skip(8).First();
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal((int)row8[row8.Keys.ElementAt(i)], 81 + i);
            }

        }

        [Fact]
        public void DataFrameIndexer_Test02()
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

            //access column 1
            var c1 = new int[] { 1, 11, 21, 31, 41, 51, 61, 71, 81, 91 };
            var cc1 = df["col1"].ToList();
            //
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c1[i], cc1[i]);

            //access row 3
            var r4 = new int[] { 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 };
            var rr3 = df[3].ToList();
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)rr3[i], r4[i]);

            //access cell col4,7
            Assert.Equal(74, (int)df["col4",7]);

            //access cell 7,3
            Assert.Equal(74, (int)df[7, 3]);

        }
    }
}
