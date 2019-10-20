using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class MissingValuesTests
    {
        [Fact]
        public void DetectMissingValue_Test01()
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
            var c1 = new object[] { 1, 11, DataFrame.NAN, 31, 41, 51, DataFrame.NAN, 71, 81, DataFrame.NAN };
            var c2 = new object[] { 3, DataFrame.NAN, 23, 33, 43, 53, 63, 73, 83, 93 };
            var c3 = new object[] { DataFrame.NAN, 15, 25, 35, 45, DataFrame.NAN, 65, DataFrame.NAN, 85, 95 };


            for (int i = 0; i < c1.Length; i++)
                Assert.Equal(c1[i], df["col1",i]);

            for (int i = 0; i < c2.Length; i++)
                Assert.Equal(c2[i], df["col3", i]);

            for (int i = 0; i < c3.Length; i++)
                Assert.Equal(c3[i], df["col5", i]);



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
            var df = new DataFrame(dict).DropNA();

            //
            var c1 = new object[] { 41, 81};
            var c2 = new object[] { 43, 83 };
            var c3 = new object[] { 45, 85 };


            for (int i = 0; i < c1.Length; i++)
                Assert.Equal(c1[i], df["col1", i]);

            for (int i = 0; i < c2.Length; i++)
                Assert.Equal(c2[i], df["col3", i]);

            for (int i = 0; i < c3.Length; i++)
                Assert.Equal(c3[i], df["col5", i]);


        }
        [Fact]
        public void RemoveMissingValues_Test02()
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
            var c1 = new int[] { 1, 11, 33333, 31, 41, 51, 33333, 71, 81, 33333 };
            var c2 = new object[] { 2, 12, 22, 33333, 42, 52, 62, 72, 82, 92 };
            var c5 = new object[] { 33333, 15, 25, 35, 45, 33333, 65, 33333, 85, 95 };

            df.FillNA(new string[] { "col1", "col2", "col5"}, 33333);

            for (int i = 0; i < c1.Length; i++)
                Assert.Equal(c1[i], df["col1", i]);

            for(int i = 0; i < c2.Length; i++)
                Assert.Equal(c2[i], df["col2", i]);

            for(int i = 0; i < c5.Length; i++)
                Assert.Equal(c5[i], df["col5", i]);


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

    }

}
