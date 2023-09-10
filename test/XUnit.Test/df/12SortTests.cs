using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class DataFrameSortTests
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
            DataFrame.qsAlgo = true;
            var df = new DataFrame(dict);

            var dict1 = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
                { "col2",new List<object>() { 2,12,22,32,42,52,62,72,82,92 } },
                { "col3",new List<object>() { 3,13,23,43,33,63,53,73,83,93 } },
                { "col4",new List<object>() { 4,14,24,54,44,34,64,74,84,94} },
            };
            var df1 = new DataFrame(dict1);

            var result = df.SortBy(new string[] { "col1", "col2", "col3", "col4" });
            var STR = result.ToStringBuilder();


            var resultIndex = new object[] { 0, 5, 6, 1, 2, 3, 4, 7, 8, 9 };

            Assert.Equal(result.Index, result.Index);


            for (int i = 0; i < result.Values.Count; i++)
            {
                var expected = Convert.ToInt32(df1.Values[i]);
                var actual = Convert.ToInt32(result.Values[i]);
                Assert.Equal<int>(expected, actual);
            }
        }
        [Fact]
        public void SortByDescending__QuickSort_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,31,41,51,61,11,21,71,81,91} },
                { "col2",new List<object>() { 2,32,42,52,62,12,22,72,82,92 } },
                { "col3",new List<object>() { 3,43,33,63,53,13,23,73,83,93 } },
                { "col4",new List<object>() { 4,54,44,34,64,14,24,74,84,94} },

            };
            var dictExp = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 91,81,71,61,51,41,31,21,11,1} },
                { "col2",new List<object>() { 92,82,72,62,52,42,32,22,12,2} },
                { "col3",new List<object>() { 93,83,73,53,63,33,43,23,13,3} },
                { "col4",new List<object>() { 94,84,74,64,34,44,54,24,14,4} },

            };

            DataFrame.qsAlgo = true;
            var df = new DataFrame(dict);
            var dfExpected = new DataFrame(dictExp);
            //reverse sorting
            var result = df.SortByDescending(new string[] { "col1", "col2", "col3", "col4" });
            var expectedIndex = new object[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 8 };
            for (int i = 0; i < result.Values.Count; i++)
            {
                var expected = Convert.ToInt32(dfExpected.Values[i]);
                var actual = Convert.ToInt32(result.Values[i]);
                Assert.Equal<int>(expected, actual);
            }
        }


        [Fact]
        public void SortBy_Merge_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,31,41,51,61,11,21,71,81,91} },
                { "col2",new List<object>() { 2,32,42,52,62,12,22,72,82,92 } },
                { "col3",new List<object>() { 3,43,33,63,53,13,23,73,83,93 } },
                { "col4",new List<object>() { 4,54,44,34,64,14,24,74,84,94} },

            };
            //
            DataFrame.qsAlgo = false;
            var df = new DataFrame(dict);

            var dict1 = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
                { "col2",new List<object>() { 2,12,22,32,42,52,62,72,82,92 } },
                { "col3",new List<object>() { 3,13,23,43,33,63,53,73,83,93 } },
                { "col4",new List<object>() { 4,14,24,54,44,34,64,74,84,94} },
            };
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
        public void SortByDescending__MergeSort_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,31,41,51,61,11,21,71,81,91} },
                { "col2",new List<object>() { 2,32,42,52,62,12,22,72,82,92 } },
                { "col3",new List<object>() { 3,43,33,63,53,13,23,73,83,93 } },
                { "col4",new List<object>() { 4,54,44,34,64,14,24,74,84,94} },

            };
            var dictExp = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 91,81,71,61,51,41,31,21,11,1} },
                { "col2",new List<object>() { 92,82,72,62,52,42,32,22,12,2} },
                { "col3",new List<object>() { 93,83,73,53,63,33,43,23,13,3} },
                { "col4",new List<object>() { 94,84,74,64,34,44,54,24,14,4} },

            };

            DataFrame.qsAlgo = false;
            var df = new DataFrame(dict);
            var dfExpected = new DataFrame(dictExp);
            //reverse sorting
            var result = df.SortByDescending(new string[] { "col1", "col2", "col3", "col4" });

            for (int i = 0; i < result.Values.Count; i++)
            {
                var expected = Convert.ToInt32(dfExpected.Values[i]);
                var actual = Convert.ToInt32(result.Values[i]);
                Assert.Equal<int>(expected, actual);
            }
        }

       
        [Fact]
        public void SortBy_MergeSort_Test02()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filePath: $"testdata/sort_sample.txt", sep: '\t', names: null, dformat: null);

            //col1,col2,col3,col4
            var expectedDf = DataFrame.FromCsv(filePath: $"testdata/sort_expected_asc.txt", sep: '\t', names: null, dformat: null);
            var oroginalDf = DataFrame.FromCsv(filePath: $"testdata/sort_sample.txt", sep: '\t', names: null, dformat: null);

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
                var actual = Convert.ToInt32(oroginalDf.Values[i]);
                Assert.Equal<int>(expected, actual);
            }
        }

        [Fact]
        public void SortBy_QuickSort_Test02()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filePath: $"testdata/sort_sample.txt", sep: '\t', names: null, dformat: null);

            //col1,col2,col3,col4
            var expectedDf = DataFrame.FromCsv(filePath: $"testdata/sort_expected_asc.txt", sep: '\t', names: null, dformat: null);
            var oroginalDf = DataFrame.FromCsv(filePath: $"testdata/sort_sample.txt", sep: '\t', names: null, dformat: null);

            //internal variable to change sort algo
            DataFrame.qsAlgo = true;
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
                var actual = Convert.ToInt32(oroginalDf.Values[i]);
                Assert.Equal<int>(expected, actual);
            }
        }


        [Fact]
        public void SortByDescending_MergeSort_Test02()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filePath: $"testdata/sort_sample.txt", sep: '\t', names: null, dformat: null);

            //col1,col2,col3,col4
            var expectedDf = DataFrame.FromCsv(filePath: $"testdata/sort_expected_desc.txt", sep: '\t', names: null, dformat: null);
            var oroginalDf = DataFrame.FromCsv(filePath: $"testdata/sort_sample.txt", sep: '\t', names: null, dformat: null);

            //internal variable to change sort algo
            DataFrame.qsAlgo = false;
            var result = sampleDf.SortByDescending(new string[] { "Col1", "Col2", "Col3", "Col4" });

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
                var actual = Convert.ToInt32(oroginalDf.Values[i]);
                Assert.Equal<int>(expected, actual);
            }
        }

        [Fact]
        public void SortByDescending_QuickSort_Test02()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filePath: $"testdata/sort_sample.txt", sep: '\t', names: null, dformat: null);

            //col1,col2,col3,col4
            var expectedDf = DataFrame.FromCsv(filePath: $"testdata/sort_expected_desc.txt", sep: '\t', names: null, dformat: null);
            var oroginalDf = DataFrame.FromCsv(filePath: $"testdata/sort_sample.txt", sep: '\t', names: null, dformat: null);

            //internal variable to change sort algo
            DataFrame.qsAlgo = true;
            var result = sampleDf.SortByDescending(new string[] { "Col1", "Col2", "Col3", "Col4" });

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
                var actual = Convert.ToInt32(oroginalDf.Values[i]);
                Assert.Equal<int>(expected, actual);
            }
        }

        [Fact]
        public void SortBy_MergeSort_Test03()
        {
            var dtFormat = "M/dd/yyyy H:mm";
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filePath: $"testdata/sort_sample02.txt", sep: '\t', names: null, parseDate:true, dformat: dtFormat);
            var expectedDf1 = DataFrame.FromCsv(filePath: $"testdata/sort_sample02_asc_sorted.txt", sep: '\t', parseDate: true, names: null, dformat: dtFormat);

            //internal variable to change sort algo
            DataFrame.qsAlgo = false;
            var result = sampleDf.SortBy(new string[] { "machineID", "datetime", "name", "rotate" });

            for (int i = 0; i < result.Values.Count; i++)
            {
                Assert.Equal<object>(expectedDf1.Values[i], result.Values[i]);
            }

        }


        [Fact]
        public void SortBy_QuickSort_Test03()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filePath: $"testdata/sort_sample02.txt", sep: '\t', names: null, dformat: "M/dd/yyy hh:mm");
            var expectedDf1 = DataFrame.FromCsv(filePath: $"testdata/sort_sample02_asc_sorted.txt", sep: '\t', names: null, dformat: null);

            //internal variable to change sort algo
            DataFrame.qsAlgo = true;
            var result = sampleDf.SortBy(new string[] { "machineID", "datetime", "name", "rotate" });

            for (int i = 0; i < result.Values.Count; i++)
            {
                Assert.Equal<object>(expectedDf1.Values[i], result.Values[i]);
            }
        }

        [Fact]
        public void SortByDescending_MergeSort_Test03()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filePath: $"testdata/sort_sample02.txt", sep: '\t', names: null, dformat: "M/dd/yyy hh:mm");
            var expectedDf1 = DataFrame.FromCsv(filePath: $"testdata/sort_sample02_desc_sorted.txt", sep: '\t', names: null, dformat: null);

            //internal variable to change sort algo
            DataFrame.qsAlgo = false;
            var result = sampleDf.SortByDescending(new string[] { "machineID", "datetime", "name", "rotate" });

            for (int i = 0; i < result.Values.Count; i++)
            {
                Assert.Equal<object>(expectedDf1.Values[i], result.Values[i]);
            }

        }


        [Fact]
        public void SortByDescending_QuickSort_Test03()
        {
            //col1,col2,col3,col4
            var sampleDf = DataFrame.FromCsv(filePath: $"testdata/sort_sample02.txt", sep: '\t', names: null, dformat: "M/dd/yyy hh:mm");
            var expectedDf1 = DataFrame.FromCsv(filePath: $"testdata/sort_sample02_desc_sorted.txt", sep: '\t', names: null, dformat: null);

            //internal variable to change sort algo
            DataFrame.qsAlgo = true;
            var result = sampleDf.SortByDescending(new string[] { "machineID", "datetime", "name", "rotate" });

            for (int i = 0; i < result.Values.Count; i++)
            {
                Assert.Equal<object>(expectedDf1.Values[i], result.Values[i]);
            }
        }

    }

}
