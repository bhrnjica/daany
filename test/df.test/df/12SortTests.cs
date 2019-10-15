using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class DataFrame_Sort_Tests
    {
      

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
            //internal variable to change sort algo
            DataFrame.qsAlgo = true;
            var result = df.SortBy(new string[] { "col1", "col2", "col3", "col4" });

            for (int i = 0; i < result.Values.Count; i++)
            {
                var expected = Convert.ToInt32(df1.Values[i]);
                var actual = Convert.ToInt32(result.Values[i]);
                Assert.Equal<int>(expected, actual);
            }

            var result2 = df.SortByDescending(new string[] { "col1", "col2", "col3", "col4" });

            for (int i = result2.Values.Count-1; i >=0; i--)
            {
                var expected = Convert.ToInt32(df1.Values[i]);
                var actual = Convert.ToInt32(result2.Values[i]);
                Assert.Equal<int>(expected, actual);
            }
        }

        [Fact]
        public void SortBy_QuickSort_Test03()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\sort_sample02.txt", sep: ',', names: null, dformat: null);
            var expectedDf1 = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\sort_sample02.txt", sep: ',', names: null, dformat: null);

            //internal variable to change sort algo
            DataFrame.qsAlgo = true;
            var result = sampleDf.SortBy(new string[] { "machineID", "datetime"});

            for (int i = 0; i < result.Values.Count; i++)
            {
                Assert.Equal<object>(expectedDf1.Values[i], result.Values[i]);
            }

            var result1 = sampleDf.SortByDescending(new string[] { "machineID", "datetime" });

            for (int i = result1.Values.Count-1; i >=0 ; i--)
            {
                Assert.Equal<object>(expectedDf1.Values[i], result1.Values[i]);
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
            //internal variable to change sort algo
            DataFrame.qsAlgo = false;
            var result = df.SortBy(new string[] { "col1", "col2", "col3", "col4" });

            for (int i = 0; i < result.Values.Count; i++)
            {
                var expected = Convert.ToInt32(df1.Values[i]);
                var actual = Convert.ToInt32(result.Values[i]);
                Assert.Equal<int>(expected, actual);
            }

            var result2 = df.SortByDescending(new string[] { "col1", "col2", "col3", "col4" });

            for (int i = result2.Values.Count-1; i >=0 ; i--)
            {
                var expected = Convert.ToInt32(df1.Values[i]);
                var actual = Convert.ToInt32(result2.Values[i]);
                Assert.Equal<int>(expected, actual);
            }

        }

        [Fact]
        public void SortBy_MergeSort_Test02()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\sort_sample.txt", sep: '\t', names: null, dformat: null);

            //col1,col2,col3,col4
            var expectedDf = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\sort_expected.txt", sep: '\t', names: null, dformat: null);
            var expectedDf1 = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\sort_sample.txt", sep: '\t', names: null, dformat: null);

            //internal variable to change sort algo
            DataFrame.qsAlgo = false;
            var result = sampleDf.SortBy(new string[] { "Col1", "Col2", "Col3", "Col4" });
            
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
            var sampleDf = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\sort_sample02.txt", sep: ',', names: null, dformat: null);
            var expectedDf1 = DataFrame.FromCsv(filePath: $"..\\..\\..\\testdata\\sort_sample02.txt", sep: ',', names: null, dformat: null);

            //internal variable to change sort algo
            DataFrame.qsAlgo = false;
            var result = sampleDf.SortBy(new string[] { "machineID", "datetime" });

            for (int i = 0; i < result.Values.Count; i++)
            {
                Assert.Equal<object>(expectedDf1.Values[i], result.Values[i]);
            }

        }

    }

}
