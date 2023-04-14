using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.MathStuff.Random;

namespace Unit.Test.DF
{
    public class DataFrameSelectionTests
    {
       
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
        public void TakeNthRow_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() {  1,11,21,31,41,51,61,71,81,91} },
                { "col2",new List<object>() {  2,12,22,32,42,52,62,72,82,92 } },
                { "col3",new List<object>() {  3,13,23,33,43,53,63,73,83,93 } },
                { "col4",new List<object>() {  4,14,24,34,44,54,64,74,84,94} },
                { "col5",new List<object>() {  5,15,25,35,45,55,65,75,85,95 } },
                { "col6",new List<object>() {  6,16,26,36,46,56,66,76,86,96} },
                { "col7",new List<object>() {  7,17,27,37,47,57,67,77,87,97 } },
                { "col8",new List<object>() {  8,18,28,38,48,58,68,78,88,98} },
                { "col9",new List<object>() {  9,19,29,39,49,59,69,79,89,99} },
                { "col10",new List<object>(){ 10,20,30,40,50,60,70,80,90,100} },
            };
            //take first 3 rows
            var df = new DataFrame(dict);
            var df1 = df.Take(3);
            //row test
            var r1 = df1[0].ToList();
            var r2 = df1[1].ToList();
            var r3 = df1[2].ToList();
            var e1 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var e2 = new int[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var e3 = new int[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };

            //check for row count
            Assert.Equal(3, df1.RowCount());

            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r1[i], e1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r2[i], e2[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r3[i], e3[i]);

            //every 3rth
            var df2 = df.Take(4);
            //row test
            //row test
            var r21 = df2[0].ToList();
            var r22 = df2[1].ToList();
            var r23 = df2[2].ToList();
            var r24 = df2[3].ToList();
            var e21 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var e22 = new int[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var e23 = new int[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
            var e24 = new int[] { 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 };

            //check for row count
            Assert.Equal(4, df2.RowCount());

            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r21[i], e21[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r22[i], e22[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r23[i], e23[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r24[i], e24[i]);

        }

        [Fact]
        public void TakeLastNthRow_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() {  1,11,21,31,41,51,61,71,81,91} },
                { "col2",new List<object>() {  2,12,22,32,42,52,62,72,82,92 } },
                { "col3",new List<object>() {  3,13,23,33,43,53,63,73,83,93 } },
                { "col4",new List<object>() {  4,14,24,34,44,54,64,74,84,94} },
                { "col5",new List<object>() {  5,15,25,35,45,55,65,75,85,95 } },
                { "col6",new List<object>() {  6,16,26,36,46,56,66,76,86,96} },
                { "col7",new List<object>() {  7,17,27,37,47,57,67,77,87,97 } },
                { "col8",new List<object>() {  8,18,28,38,48,58,68,78,88,98} },
                { "col9",new List<object>() {  9,19,29,39,49,59,69,79,89,99} },
                { "col10",new List<object>(){ 10,20,30,40,50,60,70,80,90,100} },
            };
            //
            var df = new DataFrame(dict);
            var df1 = df.Tail(3);
            //row test
            var r1 = df1[0].ToList();
            var r2 = df1[1].ToList();
            var r3 = df1[2].ToList();
            var e1 = new int[] { 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 };
            var e2 = new int[] { 81, 82, 83, 84, 85, 86, 87, 88, 89, 90 };
            var e3 = new int[] { 91, 92, 93, 94, 95, 96, 97, 98, 99, 100 };

            //check for row count
            Assert.Equal(3, df1.RowCount());

            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r1[i], e1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r2[i], e2[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r3[i], e3[i]);

            //last 4 rows
            var df2 = df.Tail(4);
            //row test
            var r21 = df2[0].ToList();
            var r22 = df2[1].ToList();
            var r23 = df2[2].ToList();
            var r24 = df2[3].ToList();
            var e21 = new int[] { 61, 62, 63, 64, 65, 66, 67, 68, 69, 70 };
            var e22 = new int[] { 71, 72, 73, 74, 75, 76, 77, 78, 79, 80 };
            var e23 = new int[] { 81, 82, 83, 84, 85, 86, 87, 88, 89, 90 };
            var e24 = new int[] { 91, 92, 93, 94, 95, 96, 97, 98, 99, 100 };

            //check for row count
            Assert.Equal(4, df2.RowCount());

            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r21[i], e21[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r22[i], e22[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r23[i], e23[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r24[i], e24[i]);

        }

        [Fact]
        public void TakeRandomNthRow_Test01()
        {
            Constant.FixedRandomSeed = true;
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
            var df1 = df.TakeRandom(3);
            Assert.Equal(df1[0], df[9]);
            Assert.Equal(df1[1], df[5]);
            Assert.Equal(df1[2], df[3]);
            //row count test
            Assert.True(df1.RowCount()==3);
        }

    }

}
