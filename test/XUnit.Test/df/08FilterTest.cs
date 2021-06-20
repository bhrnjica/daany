using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class DataFrameFilterTests
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
        public void Filter_Single_Column_Test01()
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

            //
            var filteredDF = df.Filter("col1", 5, FilterOperator.GreatherOrEqual);
            
            Assert.True(filteredDF.RowCount() == 9);
            Assert.True(filteredDF[0,0].ToString() == "11");
            Assert.True(filteredDF[1, 1].ToString() == "22");
            Assert.True(filteredDF[5, 5].ToString() == "66");


        }


        [Fact]
        public void Filter_Multiple_Columns_Test01()
        {
            //Text,Tag,Datum, Double,IntCol
            var df = DataFrame.FromCsv(filePath: $"testdata/filter_dataFrameSample.txt", sep: '\t', names: null, parseDate:true);

            //filter by one numeric column
            var filteredDF = df.Filter("Double", 0.05, FilterOperator.Less);

            //filter double column
            Assert.True(filteredDF.RowCount() == 6);
            Assert.Equal(0.0145413186f, Convert.ToSingle(filteredDF[0, 3]));
            Assert.Equal(0.031656122f, Convert.ToSingle(filteredDF[1, 3]));
            Assert.Equal(0.032011067f, Convert.ToSingle(filteredDF[2, 3]));
            Assert.Equal(0.020254352f, Convert.ToSingle(filteredDF[3, 3]));
            Assert.Equal(0.039362376f, Convert.ToSingle(filteredDF[4, 3]));
            Assert.Equal(0.014804176f, Convert.ToSingle(filteredDF[5, 3]));


            //filter data frame between dates
            var opers = new FilterOperator[2] { FilterOperator.Greather, FilterOperator.Less };
            var cols = new string[] { "Datum", "Datum" };
            var values = (new DateTime[] { new DateTime(2019, 1, 5), new DateTime(2019, 2,2) }).Select(x=>(object)x).ToArray();

            filteredDF = df.Filter(cols,values, opers);
            Assert.True(filteredDF.RowCount() == 27);
            Assert.Equal(7f, Convert.ToSingle(filteredDF[0, 4]));
            Assert.Equal(52f, Convert.ToSingle(filteredDF[1, 4]));
            Assert.Equal(27f, Convert.ToSingle(filteredDF[2, 4]));
            Assert.Equal(54f, Convert.ToSingle(filteredDF[3, 4]));
            Assert.Equal(79f, Convert.ToSingle(filteredDF[4, 4]));
            Assert.Equal(88f, Convert.ToSingle(filteredDF[5, 4]));

            Assert.Equal(68f, Convert.ToSingle(filteredDF[6, 4]));
            Assert.Equal(61f, Convert.ToSingle(filteredDF[7, 4]));
            Assert.Equal(11f, Convert.ToSingle(filteredDF[8, 4]));
            Assert.Equal(87f, Convert.ToSingle(filteredDF[9, 4]));
            Assert.Equal(80f, Convert.ToSingle(filteredDF[10, 4]));
            Assert.Equal(22f, Convert.ToSingle(filteredDF[11, 4]));
            Assert.Equal(85f, Convert.ToSingle(filteredDF[12, 4]));
            Assert.Equal(83f, Convert.ToSingle(filteredDF[13, 4]));

            Assert.Equal(31f, Convert.ToSingle(filteredDF[23, 4]));
            Assert.Equal(21f, Convert.ToSingle(filteredDF[24, 4]));
            Assert.Equal(84f, Convert.ToSingle(filteredDF[25, 4]));
            Assert.Equal(74f, Convert.ToSingle(filteredDF[26, 4]));

            //filter data frame between dates and numbers
            opers = new FilterOperator[] { FilterOperator.Greather, FilterOperator.Less, FilterOperator.GreatherOrEqual };
            cols = new string[] { "Datum", "Datum", "IntCol" };
            var valls = new List<object>();
            valls.Add(new DateTime(2019, 1, 5)); valls.Add(new DateTime(2019, 2, 2));valls.Add(68);

            filteredDF = df.Filter(cols, valls.ToArray(), opers);
            Assert.True(filteredDF.RowCount() == 11);

            Assert.Equal(79f, Convert.ToSingle(filteredDF[0, 4]));
            Assert.Equal(88f, Convert.ToSingle(filteredDF[1, 4]));
            Assert.Equal(68f, Convert.ToSingle(filteredDF[2, 4]));
            Assert.Equal(87f, Convert.ToSingle(filteredDF[3, 4]));
            Assert.Equal(80f, Convert.ToSingle(filteredDF[4, 4]));
            Assert.Equal(85f, Convert.ToSingle(filteredDF[5, 4]));

            Assert.Equal(83f, Convert.ToSingle(filteredDF[6, 4]));
            Assert.Equal(96f, Convert.ToSingle(filteredDF[7, 4]));
            Assert.Equal(81f, Convert.ToSingle(filteredDF[8, 4]));
            Assert.Equal(84f, Convert.ToSingle(filteredDF[9, 4]));
            Assert.Equal(74f, Convert.ToSingle(filteredDF[10, 4]));

        }

    }
}
