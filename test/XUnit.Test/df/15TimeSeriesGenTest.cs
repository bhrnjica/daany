using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class TSGeneratorTests
    {

        [Fact]
        public void TAGenerator_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,2,3,4,5,6,7,8,9,10} },
            };
            //
            DataFrame.qsAlgo = true;
            var df = new DataFrame(dict);

            var dict1 = new Dictionary<string, List<object>>
            {
                { "col1_lag3",new List<object>() { 1,2,3,4,5,6,7} },
                { "col1_lag2",new List<object>() { 2,3,4,5,6,7,8 } },
                { "col1_lag1",new List<object>() { 3,4,5,6,7,8,9} },
                { "col1",new List<object>() { 4,5,6,7,8,9,10} },

            };
            var df1 = new DataFrame(dict1);

            var tsDf = DataFrame.CreateTimeSeries(df, 3, 1);

            //
            Assert.Equal(tsDf.Values, df1.Values);
        }
      



        [Fact]
        public void TAGenerator_Test04()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,2,3,4,5,6,7,8,9,10} },
                { "col2",new List<object>() { 11,12,13,14,15,16,17,18,19,20} },
               
            };
            //
            DataFrame.qsAlgo = true;
            var df = new DataFrame(dict);

            var dict1 = new Dictionary<string, List<object>>
            {
                { "col1_lag3",new List<object>() { 1,2,3,4,5,6,7} },
                { "col1_lag2",new List<object>() { 2,3,4,5,6,7,8 } },
                { "col1_lag1",new List<object>() { 3,4,5,6,7,8,9} },
                { "col1",new List<object>() { 4,5,6,7,8,9,10} },

                { "col2_lag3",new List<object>() { 11,12,13,14,15,16,17} },
                { "col2_lag2",new List<object>() {12,13,14,15,16,17,18} },
                { "col2_lag1",new List<object>() {13,14,15,16,17,18,19} },
                { "col2",new List<object>() {14,15,16,17,18,19,20} },

            };
            var df1 = new DataFrame(dict1);

            var tsDf = DataFrame.CreateTimeSeries(df, 3, 1);

            //
            Assert.Equal(tsDf.Values, df1.Values);
        }

        [Fact]
        public void TAGenerator_Test05()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,2,3,4,5,6,7,8,9,10} },
                { "col2",new List<object>() { 11,12,13,14,15,16,17,18,19,20} },

            };
            //
            DataFrame.qsAlgo = true;
            var df = new DataFrame(dict);

            var dict1 = new Dictionary<string, List<object>>
            {
                { "col1_lag3",new List<object>() { 1,2,3,4,5,6} },
                { "col1_lag2",new List<object>() { 2,3,4,5,6,7 } },
                { "col1_lag1",new List<object>() { 3,4,5,6,7,8} },
                { "col1_t1",new List<object>() { 4,5,6,7,8,9} },
                { "col1_t2",new List<object>() { 5,6,7,8,9,10} },
                { "col2_lag3",new List<object>() { 11,12,13,14,15,16} },
                { "col2_lag2",new List<object>() {12,13,14,15,16,17} },
                { "col2_lag1",new List<object>() {13,14,15,16,17,18} },
                { "col2_t1",new List<object>() {14,15,16,17,18,19} },
                { "col2_t2",new List<object>() {15,16,17,18,19,20} },
            };
            var df1 = new DataFrame(dict1);

            var tsDf = DataFrame.CreateTimeSeries(df, 3, 2);

            //
            Assert.Equal(tsDf.Values, df1.Values);
        }


        [Fact]
        public void TAGenerator_Test06()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,2,3,4,5,6,7,8,9,10} },
                { "col2",new List<object>() { 11,12,13,14,15,16,17,18,19,20} },

            };
            //
            DataFrame.qsAlgo = true;
            var df = new DataFrame(dict);

            var dict1 = new Dictionary<string, List<object>>
            {
                { "col1_lag3",new List<object>() { 1,2,3,4,5} },
                { "col1_lag2",new List<object>() { 2,3,4,5,6 } },
                { "col1_lag1",new List<object>() { 3,4,5,6,7} },
                { "col1_t1",new List<object>() { 4,5,6,7,8} },
                { "col1_t2",new List<object>() { 5,6,7,8,9} },
                { "col1_t3",new List<object>() { 6,7,8,9,10} },
                { "col2_lag3",new List<object>() { 11,12,13,14,15} },
                { "col2_lag2",new List<object>() {12,13,14,15,16} },
                { "col2_lag1",new List<object>() {13,14,15,16,17} },
                { "col2_t1",new List<object>() {14,15,16,17,18} },
                { "col2_t2",new List<object>() {15,16,17,18,19} },
                { "col2_t3",new List<object>() {16,17,18,19,20} },
            };
            var df1 = new DataFrame(dict1);

            var tsDf = DataFrame.CreateTimeSeries(df, 3, 3);

            //
            Assert.Equal(tsDf.Values, df1.Values);
        }




        [Fact]
        public void TAGenerator_Test07()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,2,3,4,5,6,7,8,9,10} },
                { "col2",new List<object>() { 11,12,13,14,15,16,17,18,19,20} },
                { "col3",new List<object>() { 21,22,23,24,25,26,27,28,29,30} },

            };
            //
            DataFrame.qsAlgo = true;
            var df = new DataFrame(dict);

            var dict1 = new Dictionary<string, List<object>>
            {
                { "col1_lag3",new List<object>() { 1,2,3,4,5,6,7} },
                { "col1_lag2",new List<object>() { 2,3,4,5,6,7,8 } },
                { "col1_lag1",new List<object>() { 3,4,5,6,7,8,9} },
                { "col1",new List<object>() { 4,5,6,7,8,9,10} },

                { "col2_lag3",new List<object>() { 11,12,13,14,15,16,17} },
                { "col2_lag2",new List<object>() {12,13,14,15,16,17,18} },
                { "col2_lag1",new List<object>() {13,14,15,16,17,18,19} },
                { "col2",new List<object>() {14,15,16,17,18,19,20} },

                { "col3_lag3",new List<object>() {21,22,23,24,25,26,27} },
                { "col3_lag2",new List<object>() {22,23,24,25,26,27,28} },
                { "col3_lag1",new List<object>() {23,24,25,26,27,28,29} },
                { "col3",new List<object>()      {24,25,26,27,28,29,30} },

            };
            var df1 = new DataFrame(dict1);

            var tsDf = DataFrame.CreateTimeSeries(df, 3, 1);

            //
            Assert.Equal(tsDf.Values, df1.Values);
        }

        [Fact]
        public void TAGenerator_Test08()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,2,3,4,5,6,7,8,9,10} },
                { "col2",new List<object>() { 11,12,13,14,15,16,17,18,19,20} },
                { "col3",new List<object>() { 21,22,23,24,25,26,27,28,29,30} },

            };
            //
            DataFrame.qsAlgo = true;
            var df = new DataFrame(dict);

            var dict1 = new Dictionary<string, List<object>>
            {
                { "col1_lag3",new List<object>() { 1,2,3,4,5,6} },
                { "col1_lag2",new List<object>() { 2,3,4,5,6,7 } },
                { "col1_lag1",new List<object>() { 3,4,5,6,7,8} },
                { "col1_t1",new List<object>() { 4,5,6,7,8,9} },
                { "col1_t2",new List<object>() { 5,6,7,8,9,10} },

                { "col2_lag3",new List<object>() { 11,12,13,14,15,16} },
                { "col2_lag2",new List<object>() {12,13,14,15,16,17} },
                { "col2_lag1",new List<object>() {13,14,15,16,17,18} },
                { "col2_t1",new List<object>() {14,15,16,17,18,19} },
                { "col2_t2",new List<object>() {15,16,17,18,19,20} },

                { "col3_lag3",new List<object>() { 21,22,23,24,25,26} },
                { "col3_lag2",new List<object>() {22,23,24,25,26,27} },
                { "col3_lag1",new List<object>() {23,24,25,26,27,28} },
                { "col3_t1",new List<object>() {24,25,26,27,28,29} },
                { "col3_t2",new List<object>() {25,26,27,28,29,30} },
            };
            var df1 = new DataFrame(dict1);

            var tsDf = DataFrame.CreateTimeSeries(df, 3, 2);

            //
            Assert.Equal(tsDf.Values, df1.Values);
        }


        [Fact]
        public void TAGenerator_Test09()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,2,3,4,5,6,7,8,9,10} },
                { "col2",new List<object>() { 11,12,13,14,15,16,17,18,19,20} },
                { "col3",new List<object>() { 21,22,23,24,25,26,27,28,29,30} },

            };
            //
            DataFrame.qsAlgo = true;
            var df = new DataFrame(dict);

            var dict1 = new Dictionary<string, List<object>>
            {
                { "col1_lag3",new List<object>() { 1,2,3,4,5} },
                { "col1_lag2",new List<object>() { 2,3,4,5,6 } },
                { "col1_lag1",new List<object>() { 3,4,5,6,7} },
                { "col1_t1",new List<object>() { 4,5,6,7,8} },
                { "col1_t2",new List<object>() { 5,6,7,8,9} },
                { "col1_t3",new List<object>() { 6,7,8,9,10} },

                { "col2_lag3",new List<object>() { 11,12,13,14,15} },
                { "col2_lag2",new List<object>() {12,13,14,15,16} },
                { "col2_lag1",new List<object>() {13,14,15,16,17} },
                { "col2_t1",new List<object>() {14,15,16,17,18} },
                { "col2_t2",new List<object>() {15,16,17,18,19} },
                { "col2_t3",new List<object>() {16,17,18,19,20} },

                { "col3_lag3",new List<object>() { 21,22,23,24,25} },
                { "col3_lag2",new List<object>() {22,23,24,25,26} },
                { "col3_lag1",new List<object>() {23,24,25,26,27} },
                { "col3_t1",new List<object>() {24,25,26,27,28} },
                { "col3_t2",new List<object>() {25,26,27,28,29} },
                { "col3_t3",new List<object>() {26,27,28,29,30} },
            };
            var df1 = new DataFrame(dict1);

            var tsDf = DataFrame.CreateTimeSeries(df, 3, 3);

            //
            Assert.Equal(tsDf.Values, df1.Values);
        }

    }

}
