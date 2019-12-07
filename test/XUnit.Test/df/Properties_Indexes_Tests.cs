using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using System.Globalization;

namespace Unit.Test.DF
{
    public class Properties
    {
        string rootfolder = "..\\..\\..\\testdata\\";

        [Fact]
        public void DataFrame_Shape_Test()
        {
            var lst = new List<object>() {  1, "Sarajevo",  77000, "BiH", true,     3.14, DateTime.Now.AddDays(-20),
                                            2, "Seattle",   98101, "USA", false,    3.21, DateTime.Now.AddDays(-10),
                                            3, "Berlin",    10115, "GER", false,    4.55, DateTime.Now.AddDays(-5),
                                        };
            //define column header for the DataFrame
            var columns = new List<string>() { "ID", "City", "Zip Code", "State","IsHome", "Values", "Date" };

            //create data frame with 3 rows and 7 columns
            var df = new DataFrame(lst, columns);

            //check the size of the data frame
            Assert.Equal((3,7), df.Shape);
            var str = df.Shape.ToString();
            Assert.Equal("(3, 7)", str);

        }


        [Fact]
        public void FilterByCodition()
        {
            var lst = new List<object>() {  1, "Sarajevo",  77000, "BiH", true,     3.14, DateTime.Now.AddDays(-20),
                                            2, "Seattle",   98101, "USA", false,    3.21, DateTime.Now.AddDays(-10),
                                            3, "Berlin",    10115, "GER", false,    4.55, DateTime.Now.AddDays(-5),
                                        };
            //define column header for the DataFrame
            var columns = new List<string>() { "ID", "City", "Zip Code", "State", "IsHome", "Values", "Date" };

            //create data frame with 3 rows and 7 columns
            var df = new DataFrame(lst, columns);
           var sd =  df.Filter((row) => Convert.ToInt32(row["ID"]) >= 2);
            //check the size of the data frame
            Assert.Equal((2, 7), sd.Shape);
            var sd1 = df.Filter((row) => Convert.ToString(row["City"]) == "Sarajevo");
            var str = sd1.Shape.ToString();
            Assert.Equal("(1, 7)", str);

        }


        [Fact]
        public void Set_Index_Test()
        {
            var lst = new List<object>() {  1, "Sarajevo",  77000, "BiH", true,     3.14, DateTime.Now.AddDays(-20),
                                            2, "Seattle",   98101, "USA", false,    3.21, DateTime.Now.AddDays(-10),
                                            3, "Berlin",    10115, "GER", false,    4.55, DateTime.Now.AddDays(-5),
                                        };
            //define column header for the DataFrame
            var columns = new List<string>() { "ID", "City", "Zip Code", "State", "IsHome", "Values", "Date" };

            //create data frame with 3 rows and 7 columns
            var df = new DataFrame(lst, columns);

            //set Index
            var newDf = df.SetIndex("City");

            
            //check the size of the data frame
            Assert.Equal((3, 6), newDf.Shape);
            var iii = newDf.Index.ToList();
            Assert.Equal(new List<object> {"Sarajevo", "Seattle", "Berlin" }, iii);

            Assert.Equal("City", newDf.Index.Name);

        }


        [Fact]
        public void Reset_Index_Test()
        {
            var lst = new List<object>() {  1, "Sarajevo",  77000, "BiH", true,     3.14, DateTime.Now.AddDays(-20),
                                            2, "Seattle",   98101, "USA", false,    3.21, DateTime.Now.AddDays(-10),
                                            3, "Berlin",    10115, "GER", false,    4.55, DateTime.Now.AddDays(-5),
                                        };
            //define column header for the DataFrame
            var columns = new List<string>() { "ID", "City", "Zip Code", "State", "IsHome", "Values", "Date" };

            //create data frame with 3 rows and 7 columns
            var df = new DataFrame(lst, columns);

            //set Index
            var newDf = df.SetIndex("City");


            //check the size of the data frame
            Assert.Equal((3, 7), df.Shape);
            Assert.Equal((3, 6), newDf.Shape);
            var iii = newDf.Index.ToList();
            Assert.Equal(new List<object> { "Sarajevo", "Seattle", "Berlin" }, iii);

            Assert.Equal("City", newDf.Index.Name);


            //reset index, take out the index and put it as column in the data frame
            var newDf1 = newDf.ResetIndex(drop:false);
            //check the size of the data frame
            Assert.Equal((3, 7), df.Shape);
            var ij = newDf1.Index.ToList();
            Assert.Equal(new List<object> { 0, 1, 2 }, ij);
            Assert.Equal(new List<object> { "Sarajevo", "Seattle", "Berlin" }, newDf1["City"]);

            //reset index but dont include it in the column list
            var newDf2 = newDf.ResetIndex(drop: true);
            //check the size of the data frame
            Assert.Equal((3, 6), newDf2.Shape);
            var ik = df.Index.ToList();
            Assert.Equal(new List<object> { 0, 1, 2 }, ik);
            
        }

        [Fact]
        public void SortDataFrame_TestIndex()
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

            var result = df.SortBy(new string[] { "col1", "col2", "col3", "col4" });
            var expLst = new List<object> { 0, 5, 6, 1, 2, 3, 4, 7, 8, 9 };
            Assert.Equal(expLst, result.Index.ToList());
        }
    }
}
