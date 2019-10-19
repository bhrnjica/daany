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
