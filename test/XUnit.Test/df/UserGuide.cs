using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using System.Globalization;

namespace Unit.Test.DF
{
    public class UserGuide
    {
        string rootfolder = "..\\..\\..\\testdata\\";

        #region Create DF
        [Fact]
        public void CreateDataFrameFromList()
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
            Assert.Equal(3, df.RowCount());
            Assert.Equal(7, df.ColCount());
        }

        [Fact]
        public void CreateDataFrameFromDictionary()
        {
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { DateTime.Now.AddDays(-20) , DateTime.Now.AddDays(-10) , DateTime.Now.AddDays(-5) } },

            };

            //create data frame with 3 rows and 7 columns
            var df = new DataFrame(dict);

            //check the size of the data frame
            Assert.Equal(3, df.RowCount());
            Assert.Equal(7, df.ColCount());
        }

        [Fact]
        public void SaveLoadDataFrameToFromFile()
        {
            string filename = "df_file.txt";
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { DateTime.Now.AddDays(-20) , DateTime.Now.AddDays(-10) , DateTime.Now.AddDays(-5) } },

            };

            //create data frame with 3 rows and 7 columns
            var df = new DataFrame(dict);

            //first Save data frame on disk and load it
            DataFrame.ToCsv(filename, df);

            //create data frame with 3 rows and 7 columns
            var dfFromFile = DataFrame.FromCsv(filename, sep:',');

            //check the size of the data frame
            Assert.Equal(3, dfFromFile.RowCount());
            Assert.Equal(new string[] { "ID", "City", "Zip Code", "State", "IsHome", "Values", "Date" }, dfFromFile.Columns);
            Assert.Equal(7, dfFromFile.ColCount());
        }
        [Fact]
        public void SaveLoadDataFrameToFromFileWithColumnTypes()
        {
            string filename = "df_file.txt";
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { DateTime.Now.AddDays(-20) , DateTime.Now.AddDays(-10) , DateTime.Now.AddDays(-5) } },

            };

            //create data frame with 3 rows and 7 columns
            var df = new DataFrame(dict);

            //first Save data frame on disk and load it
            DataFrame.ToCsv(filename, df);

            //defined types of the column 
            var colTypes1 = new ColType[] { ColType.I32, ColType.IN, ColType.I32, ColType.STR, ColType.I2, ColType.F32, ColType.DT };
            //create data frame with 3 rows and 7 columns
            var dfFromFile = DataFrame.FromCsv(filename, sep: ',', colTypes: colTypes1);

            //check the size of the data frame
            Assert.Equal(3, dfFromFile.RowCount());
            Assert.Equal(new string[] { "ID", "City", "Zip Code", "State", "IsHome", "Values", "Date" }, dfFromFile.Columns);

            Assert.Equal(colTypes1, dfFromFile.ColTypes);
            Assert.Equal(7, dfFromFile.ColCount());
        }



        [Fact]
        public void CreateDataFrameFromExistingOne()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt", sep: ',', names: null, dformat: "MM/dd/yyyy");

            //now create a new data frame with only three columns
            var newDf = df["City", "Zip Code", "State"];

            //check the size of the data frame
            Assert.Equal(3, newDf.RowCount());
            Assert.Equal(new string[] { "City", "Zip Code", "State" }, newDf.Columns);
            Assert.Equal(3, newDf.ColCount());
        }

        [Fact]
        public void CreateDataFrameFromExistingOne1()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt", sep: ',', names: null, dformat: "MM/dd/yyyy");

            //now create a new data frame with only three columns
            var newDf = df["City", "Zip Code", "State", "State"];

            //check the size of the data frame
            Assert.Equal(3, newDf.RowCount());
            Assert.Equal(new string[] { "City", "Zip Code", "State", "State" }, newDf.Columns);
            Assert.Equal(4, newDf.ColCount());
        }

        [Fact]
        public void CreateDataFrameFromExistingOneUsingCrete()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt", sep: ',', names: null, dformat: "MM/dd/yyyy");

            //now create a new data frame with three columns which can be renamed during creation
            var newDf = df.Create(("City","Place"),  ("State", "Country"), ("Zip Code", null), ("Values", "Values"));

            //check the size of the data frame
            Assert.Equal(3, newDf.RowCount());
            Assert.Equal(new string[] { "Place", "Country", "Zip Code", "Values" }, newDf.Columns);
            Assert.Equal(4, newDf.ColCount());
        }

        [Fact]
        public void CreateDataFrameByDefiningNewColumns()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt", sep: ',', names: null, dformat: "MM/dd/yyyy");

            //now create a new data frame with three columns which can be renamed during creation
            var newDf = df.Create(("City", "Place"), ("State", "Country"), ("Zip Code", null), ("Values", "Values"), ("Values", "Values2"), ("Values", "Values3"));

            //check the size of the data frame
            Assert.Equal(3, newDf.RowCount());
            Assert.Equal(new string[] { "Place", "Country", "Zip Code", "Values", "Values2", "Values3" }, newDf.Columns);
            Assert.Equal(6, newDf.ColCount());
        }

        [Fact]
        public void CreateEmptyDataFrame()
        {
            var cols = new string[] { "Place", "Country", "Zip Code", "Values" };
            //create empty data frame with 4 columns
            var df = DataFrame.CreateEmpty(cols);

            //check the size of the data frame
            Assert.Equal(0, df.RowCount());
            Assert.Equal(new string[] { "Place", "Country", "Zip Code", "Values" }, df.Columns);
            Assert.Equal(4, df.ColCount());
        }
        #endregion

        #region Enumerate data frame
        [Fact]
        public void EnumerationByDictionary()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt");

            //get second data frame row
            //row2 is a dictionary with column names as keys
            var row2 = df.GetEnumerator().Skip(1).First();

            //check some data from the second row
            Assert.Equal("Seattle", row2["City"]);
            Assert.Equal("USA", row2["State"]);
            Assert.Equal(3.21f, row2["Values"]);
        }

        [Fact]
        public void EnumerationByRow()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt");

            //get second data frame row
            //row2 is a dictionary with column names as keys
            var row2 = df.GetRowEnumerator().FirstOrDefault();

            //check some data from the second row
            Assert.Equal("Sarajevo", row2[1]);
            Assert.Equal("BiH", row2[3]);
            Assert.Equal(3.14f, row2[5]);
        }

        [Fact]
        public void StronglyTypedEnumeration()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt");

            //convert data frame into strongly typed list
            List<Person> list = df.GetEnumerator<Person>((oRow) =>
            {
                //convert row object array into Iris row

                var prRow = new Person();
                prRow.ID = Convert.ToInt32(oRow["ID"]);
                prRow.City = Convert.ToString(oRow["City"]);
                prRow.Zip = Convert.ToInt32(oRow["Zip Code"]);
                prRow.State = Convert.ToString(oRow["State"]);
                prRow.IsHome = Convert.ToBoolean(oRow["IsHome"]);
                prRow.Values = Convert.ToSingle(oRow["Values"]);
                prRow.Date = Convert.ToDateTime(oRow["Date"]);
                //
                return prRow;
            }).ToList();

            //check some data from the second row
            Assert.Equal("Seattle", list[1].City);
            Assert.Equal("USA", list[1].State);
            Assert.Equal(3.21f, list[1].Values);
        }
        class Person
        {
            public int ID { get; set; }
            public string City { get; set; }
            public int Zip { get; set; }
            public string State { get; set; }
            public bool IsHome { get; set; }
            public float Values { get; set; }
            public DateTime Date { get; set; }
        }

        #endregion

        #region Selecting data
        [Fact]
        public void SelectByColumnIndex()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt");

            //select one column from the data frame
            var cities = df["City"].ToArray();
            var zipCodes = df["Zip Code"].ToList();

            //check for values
            Assert.Equal(3, cities.Length);
            Assert.Equal("Sarajevo", cities[0]);
            Assert.Equal("Seattle", cities[1]);
            Assert.Equal("Berlin", cities[2]);

            //check for values
            Assert.Equal(3, zipCodes.Count);
            Assert.Equal(71000, zipCodes[0]);
            Assert.Equal(98101, zipCodes[1]);
            Assert.Equal(10115, zipCodes[2]);
        }

        [Fact]
        public void SelectColumns()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt");

            //select one column from the data frame
            var citiesDf = df["City", "Zip Code"];

            //check for values
            Assert.Equal(3, citiesDf.RowCount());
            Assert.Equal(2, citiesDf.ColCount());

        }

        [Fact]
        public void SelectDataFrameRow()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt");

            //select third row from data frame
            //3, "Berlin",    10115, "GER", false,    4.55, DateTime.Now.AddDays(-5)
            var row = df[2].ToArray();

            //check for values
            Assert.Equal(7, row.Length);
            Assert.Equal(3, row[0]);
            Assert.Equal("Berlin", row[1]);

        }
        [Fact]
        public void SelectDataUsingPosition()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt");

            //select city from the third row
            var city = df[2,1];
            var city1 = df["City", 2];

            //check for values
            Assert.Equal(city,city1);
            Assert.Equal("Berlin", city1);

        }

        #endregion

        #region Add Columns and Rows
        [Fact]
        public void AddColumnsToDataFrame()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt");

            //add Age column
            var newCols =  new Dictionary<string, List<object>>(){ { "Age", new List<object>() { 31, 25, 45 } },
                                                    { "Gender", new List<object>() { "male", "female", "male" } } };

            //add column
            var newDf = df.AddColumns(newCols);

            //check for values
            Assert.Equal(9, newDf.ColCount());
            Assert.Equal(25, newDf["Age", 1]);
            Assert.Equal("female", newDf["Gender", 1]);

        }

        [Fact]
        public void AddCalculatedColumnsByList()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() {  1,13,25,37,49} },
                { "col2",new List<object>() {  2,14,26,38,50} },
                { "col3",new List<object>() {  3,15,27,39,51} },
                { "col4",new List<object>() {  4,16,28,40,52} },
                { "col5",new List<object>() {  5,17,29,41,53} },
                { "col6",new List<object>() {  6,18,30,42,54} },
                { "col7",new List<object>() {  7,19,31,43,55} },
                { "col8",new List<object>() {  8,20,32,44,56} },
                { "col9",new List<object>() {  9,21,33,45,57} },
                { "col10",new List<object>(){ 10,22,34,46,58} },
            };
            //
            var df = new DataFrame(dict);
            var sCols = new string[] { "col11", "col12" };
            var df01 = df.AddCalculatedColumns(sCols, (row, i) => calculate(row, i));

            for (int i = 0; i < df.Values.Count; i++)
                Assert.Equal(i + 1, df.Values[i]);

            //local function declaration
            object[] calculate(object[] row, int i)
            { 
                return new object[2]{  i * (row.Length +2) + row.Length + 1,
                                        i * (row.Length+ 2) + row.Length +2}; 
            }
        }

        [Fact]
        public void AddCalculatedColumnsByDictionary()
        {
                var dict = new Dictionary<string, List<object>>
                {
                    { "col1",new List<object>() {  1,13,25,37,49} },
                    { "col2",new List<object>() {  2,14,26,38,50} },
                    { "col3",new List<object>() {  3,15,27,39,51} },
                    { "col4",new List<object>() {  4,16,28,40,52} },
                    { "col5",new List<object>() {  5,17,29,41,53} },
                    { "col6",new List<object>() {  6,18,30,42,54} },
                    { "col7",new List<object>() {  7,19,31,43,55} },
                    { "col8",new List<object>() {  8,20,32,44,56} },
                    { "col9",new List<object>() {  9,21,33,45,57} },
                    { "col10",new List<object>(){ 10,22,34,46,58} },
                };
                //
                var df = new DataFrame(dict);
                var sCols = new string[] { "col11", "col12" };
                var df01 = df.AddCalculatedColumns(sCols, (row, i) => calculate(row, i));

                //column test
                var c1 = new int[] { 11, 23, 35, 47, 59};
                var c2 = new int[] { 12, 24, 36, 48, 60};

                for (int i = 0; i < df.Values.Count; i++)
                    Assert.Equal(i+1, df.Values[i]);

                //local function declaration
                object[] calculate(IDictionary<string, object> row, int i)
                {
                    return new object[2] {  i * (row.Count() +2) + row.Count() + 1, 
                                            i * (row.Count()+ 2) + row.Count() +2};
                }
        }

        [Fact]
        public void AddRowToDataFrame()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt");

            //new row
            var newRow = new List<object>() { 4, "London", 11000, "GB", false, 5.55, DateTime.Now.AddDays(-5) };

            //add column
            df.AddRow(newRow);

            //check for values
            Assert.Equal(7, df.ColCount());
            Assert.Equal(4, df.RowCount());
            Assert.Equal("GB", df["State", 3]);
            Assert.Equal(5.55, df["Values", 3]);

        }
        #endregion

        #region Aggregate
        [Fact]
        public void AggregateDataFrame1()
        {
            var date = DateTime.Now.AddDays(-5);
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { DateTime.Now.AddDays(-20) , DateTime.Now.AddDays(-10) , date } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", "female", "male" } }
            };

            //create df
            var df = new DataFrame(dict);

            //define aggregation
            var agg = new Dictionary<string, Aggregation>() { {"ID",Aggregation.Count},
                                                                {"City",Aggregation.Top},
                                                                {"Date", Aggregation.Max},
                                                                {"Values",Aggregation.Avg },
                                                            };
            var row = df.Aggragate(agg,allColumns:false);

            var val = new List<object>() { 3, "Sarajevo", 3.633333, date };

            Assert.Equal(val, row);
        }

        [Fact]
        public void AggregateDataFrame()
        {
            var date = DateTime.Now.AddDays(-5);
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { DateTime.Now.AddDays(-20) , DateTime.Now.AddDays(-10) , date } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", "female", "male" } }
            };

            //create df
            var df = new DataFrame(dict);

            //define aggregation
            var agg = new Dictionary<string, Aggregation[]>() { {"ID",new Aggregation[]{Aggregation.Count,Aggregation.Sum }},
                                                              {"City",new Aggregation[]{Aggregation.Top,Aggregation.Frequency }},
                                                              {"Date", new Aggregation[]{Aggregation.Max }},
                                                              {"Values",new Aggregation[]{Aggregation.Avg } },
                                                            };
            var newDf = df.Aggragate(agg);
            var val = new List<object>() { 3, null, null, null, 6, null, null, null, null, "Sarajevo", null, null, 
                null, 1, null, null, null, null, 3.633333, null, null, null, null, date};

            //
            Assert.Equal(new string[] {"Count", "Sum", "Top", "Freq", "Mean", "Max" }, newDf.Index);
            for (int i = 0; i < newDf.Values.Count; i++)
                Assert.Equal(val[i], newDf.Values[i]);
        }

        [Fact]
        public void DescribeDataFrame()
        {
            var date = DateTime.Now.AddDays(-5);
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { DateTime.Now.AddDays(-20) , DateTime.Now.AddDays(-10) , date } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", "female", "male" } }
            };

            //create df
            var df = new DataFrame(dict);
            var newDf = df.Describe();

            
           var str = newDf.ToStringBuilder();
            var val = new List<object>() { 3,3,3,3,3,3,3,3,1,71000,3.14, 31,1,1,1,1,2d,59738.666667,3.633333, 33.666667,1d,
                                            45061.039384,0.794628, 10.263203,1,10115,3.14, 25,1.5,40557.5,3.175,28d,2d,71000d,3.21, 
                                            31d,2.5,84550.5,3.88, 38d,3,98101,4.55,45 };

            //
            Assert.Equal(new string[] { "Count", "Unique", "Top", "Freq", "Mean", "Std", "Min", "25%", "Median", "75%", "Max" }, newDf.Index);
            for (int i = 0; i < newDf.Values.Count; i++)
                Assert.Equal(val[i], newDf.Values[i]);
        }
        [Fact]
        public void DescribeDataFrame1()
        {
            var date = DateTime.Now.AddDays(-5);
            var date2 = DateTime.Now.AddDays(-20);
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { date2 , DateTime.Now.AddDays(-10) , date } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", "female", "male" } }
            };

            //create df
            var df = new DataFrame(dict);
            var newDf = df.Describe(numericOnly:false);

            var val = new List<object>() {
                                        3,3,3,3,3,3,3,3,3,
                                        3,3,3,3,2,3,3,3,2,
                                        1,"Sarajevo",71000,"BiH",false,3.14,date2,31,"male",
                                        1,1,1,1,2,1,1,1,2,
                                        2d,null,59738.666667,null,null,3.633333,null,33.666667,null,
                                        1d,null,45061.039384,null,null,0.794628,null,10.263203,null,
                                        1,null,10115,null,null,3.14,date2,25,null,
                                        1.5, null,40557.5,null,null,3.175,null,28d,null,
                                        2d,null,71000d,null,null,3.21,null,31d,null,
                                        2.5, null,84550.5,null,null,3.88,null,38d,null,
                                        3,null,98101,null,null,4.55,date,45,null,             
            };

            //
            Assert.Equal(new string[] { "Count", "Unique", "Top", "Freq", "Mean", "Std", "Min", "25%", "Median", "75%", "Max" }, newDf.Index);
            for (int i = 0; i < newDf.Values.Count; i++)
                Assert.Equal(val[i], newDf.Values[i]);
        }

        [Fact]
        public void DescribeDataFrame2()
        {
            var date = DateTime.Now.AddDays(-5);
            var date2 = DateTime.Now.AddDays(-20);
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { date2 , DateTime.Now.AddDays(-10) , date } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", "female", "male" } }
            };

            //create df
            var df = new DataFrame(dict);
            var newDf = df.Describe(false, "ID", "City", "Zip Code", "State", "IsHome");

            var val = new List<object>() {
                                        3,3,3,3,3, 
                                        3,3,3,3,2, 
                                        1,"Sarajevo",71000,"BiH",false, 
                                        1,1,1,1,2, 
                                        2d,null,59738.666667,null,null, 
                                        1d,null,45061.039384,null,null, 
                                        1,null,10115,null,null, 
                                        1.5, null,40557.5,null,null, 
                                        2d,null,71000d,null,null, 
                                        2.5, null,84550.5,null,null, 
                                        3,null,98101,null,null, 
            };

            //
            Assert.Equal(new string[] { "Count", "Unique", "Top", "Freq", "Mean", "Std", "Min", "25%", "Median", "75%", "Max" }, newDf.Index);
            for (int i = 0; i < newDf.Values.Count; i++)
                Assert.Equal(val[i], newDf.Values[i]);
        }

        [Fact]
        public void DescribeDataFrame3()
        {
            var date = DateTime.Now.AddDays(-5);
            var date2 = DateTime.Now.AddDays(-20);
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { date2 , DateTime.Now.AddDays(-10) , date } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", "female", "male" } }
            };

            //create df
            var df = new DataFrame(dict);
            var newDf = df.Describe(true, "ID", "City", "Zip Code", "State", "IsHome");

            var val = new List<object>() {
                                        3,3,
                                        3,3,
                                        1,71000,
                                        1,1,
                                        2d,59738.666667,
                                        1d,45061.039384,
                                        1,10115,
                                        1.5, 40557.5,
                                        2d,71000d,
                                        2.5,84550.5,
                                        3,98101,
            };

            //
            Assert.Equal(new string[] { "Count", "Unique", "Top", "Freq", "Mean", "Std", "Min", "25%", "Median", "75%", "Max" }, newDf.Index);
            for (int i = 0; i < newDf.Values.Count; i++)
                Assert.Equal(val[i], newDf.Values[i]);
        }
        #endregion

        #region Drop and Missing Values
        [Fact]
        public void DropColumn()
        {
            var date1 = DateTime.Now.AddDays(-20);
            var date2 = DateTime.Now.AddDays(-10);
            var date3 = DateTime.Now.AddDays(-5);
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { date1 , date2 , date3 } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", "female", "male" } }
            };

            //create df
            var df = new DataFrame(dict);
            var df2 = df.Drop("ID", "Date", "Age", "Gender");
            var lst = new List<object>() {"Sarajevo", 71000, "BiH", true, 3.14,"Seattle", 98101, "USA", false, 3.21, "Berlin",10115, "GER", false, 4.55 };
            //
            Assert.Equal(new string[] { "City", "Zip Code", "State", "IsHome", "Values" }, df2.Columns);
            Assert.Equal(lst, df2.Values);

        }

        [Fact]
        public void DropNA()
        {
            var date = DateTime.Now.AddDays(-20);
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", DataFrame.NAN } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { date , DateTime.Now.AddDays(-10) , date } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", DataFrame.NAN, "male" } }
            };

            //create df
            var df = new DataFrame(dict);

            //drop rows with missing values
            var newDf = df.DropNA();

            //check for values
            Assert.Equal(9, newDf.ColCount());
            Assert.Equal(1, newDf.RowCount());
            Assert.Equal(new object[] {1, "Sarajevo", 71000, "BiH", true, 3.14,date, 31, "male" }, newDf[0]);

        }

        [Fact]
        public void FillNA1()
        {
            var date = DateTime.Now.AddDays(-20);
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", DataFrame.NAN } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { date , DateTime.Now.AddDays(-10) , date } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", "female", "male" } }
            };

            //create df
            var df = new DataFrame(dict);

            //drop rows with missing values
            string replValue = "Berlin";
            df.FillNA(replValue);

            //check for values
            Assert.Equal(9, df.ColCount());
            Assert.Equal(3, df.RowCount());
            Assert.Equal(new object[] { 3, "Berlin", 10115, "GER", false, 4.55, date, 45, "male" }, df[2]);

        }

        [Fact]
        public void FillNA2()
        {
            var date = DateTime.Now.AddDays(-20);
            var date2= DateTime.Now.AddDays(-10);

            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", DataFrame.NAN } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { DateTime.Now.AddDays(-10), date2 , date } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", DataFrame.NAN, "male" } }
            };

            //create df
            var df = new DataFrame(dict);

            //drop rows with missing values
            string replValue = "Berlin";
            var replValue2 = "female";
            df.FillNA("City",replValue);
            df.FillNA("Gender", replValue2);

            //check for values
            Assert.Equal(9, df.ColCount());
            Assert.Equal(3, df.RowCount());
            Assert.Equal(new object[] { 2, "Seattle", 98101, "USA", false, 3.21, date2, 25, "female" }, df[1]);
            Assert.Equal(new object[] { 3, "Berlin", 10115, "GER", false, 4.55, date, 45, "male" }, df[2]);

        }

        [Fact]
        public void FillNA3()
        {
            var date = DateTime.Now.AddDays(-20);
            var date2 = DateTime.Now.AddDays(-10);

            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101, DataFrame.NAN } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { DataFrame.NAN, 3.21, 4.55 } },
                { "Date",new List<object>() { date, date2 , date } },
                { "Age", new List<object>() { 31, 25, DataFrame.NAN } },
                { "Gender", new List<object>() { "male", "female", "male" } }
            };

            //create df
            var df = new DataFrame(dict);

            //drop rows with missing values
            int replValue = 40;
            var replValue2 = 10115;
            df.FillNA(new string[] { "Age", "Values" }, replValue);
            df.FillNA("Zip Code", replValue2);

            //check for values
            Assert.Equal(9, df.ColCount());
            Assert.Equal(3, df.RowCount());
            Assert.Equal(new object[] { 1, "Sarajevo", 71000, "BiH", true, 40, date, 31, "male" }, df[0]);
            Assert.Equal(new object[] { 2, "Seattle", 98101, "USA", false, 3.21, date2, 25, "female" }, df[1]);
            Assert.Equal(new object[] { 3, "Berlin", 10115, "GER", false, 4.55, date, 40, "male" }, df[2]);

        }

        #endregion

        #region GrouBy and Rolling
        [Fact]
        //more unit test ara one 09GroupAndRoll
        public void GroupByWithRolling()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>()  { 1,2,3,4,5,6,7,8,9,10} },
                { "A",new List<object>()  { -2.385977,-1.004295,0.735167, -0.702657,-0.246845,2.463718, -1.142255,1.396598, -0.543425,-0.64050} },
                { "B",new List<object>()  { -0.102758,0.905829, -0.165272,-1.340923,0.211596, 3.157577, 2.340594, -1.647453,1.761277, 0.289374} },
                { "C",new List<object>()  { 0.438822, -0.954544,-1.619346,-0.706334,-0.901819,-1.380906,-0.039875,1.677227, -0.220481,-1.55067} },
                { "D",new List<object>()  { "chair", "label", "item", "window", "computer", "label", "chair", "item", "abaqus", "window" } },
                {"E", new List<object>() { DateTime.ParseExact("12/20/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("6/13/2016" , "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("8/25/2016",  "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("11/4/2016" , "MM/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("6/18/2016",  "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("3/8/2016" ,  "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("9/3/2016" ,  "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("11/24/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("6/16/2016",  "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                           DateTime.ParseExact("1/31/2016",  "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None)}
                }
            };

            //
            var df = new DataFrame(dict);
            var rollingdf = df.Rolling(3, new Dictionary<string, Aggregation> { { "A", Aggregation.Sum } });
            
            //column test
            var c1 = new object[] { DataFrame.NAN, DataFrame.NAN, -2.655105, -0.971785, -0.214335, 1.514216, 1.074618, 2.718061, -0.289082, 0.212673 };

            var cc1 = rollingdf["A"].ToList();
            for (int i = 0; i < 10; i++)
            {
                if (cc1[i] != null)
                    Assert.Equal((double)c1[i], (double)cc1[i], 6);
                else
                    Assert.Equal(c1[i], cc1[i]);
            }

        }

        [Fact]
        public void GroupBy()
        {
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", "female", "male" } }
            };

            //create df
            var df = new DataFrame(dict);
            //group df by gender
            var gDf = df.GroupBy("Gender");
            var swqs = gDf.ToStringBuilder();
            var str = @"Group By Column: Gender
male                
        ID      City    Zip CodeState   IsHome  Values  Age     Gender  
0       1       Sarajevo71000   BiH     True    3.14    31      male    
1       3       Berlin  10115   GER     False   4.55    45      male    

female              
        ID      City    Zip CodeState   IsHome  Values  Age     Gender  
0       2       Seattle 98101   USA     False   3.21    25      female  

";
            Assert.Equal(str, swqs);

        }
        [Fact]
        public void GroupByTwoColumns()
        {
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Sarajevo", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", "female", "male" } }
            };

            //create df
            var df = new DataFrame(dict);
            //group df by gender
            var gDf = df.GroupBy("Gender","City");
            var swqs = gDf.ToStringBuilder();
            var str = @"Group By Column: Gender, City                
male                Sarajevo            
        ID      City    Zip CodeState   IsHome  Values  Age     Gender  
0       1       Sarajevo71000   BiH     True    3.14    31      male    

Berlin              
        ID      City    Zip CodeState   IsHome  Values  Age     Gender  
0       3       Berlin  10115   GER     False   4.55    45      male    

female              Sarajevo            
        ID      City    Zip CodeState   IsHome  Values  Age     Gender  
0       2       Sarajevo98101   USA     False   3.21    25      female  

";
            Assert.Equal(str, swqs);
        }

        [Fact]
        public void GroupByThenRolling()
        {
            var date1 = DateTime.Now.AddDays(-20);
            var date2 = DateTime.Now.AddDays(-10);
            var date3 = DateTime.Now.AddDays(-5);
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3,4} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin", "Amsterdam" } },
                { "Zip Code",new List<object>() { 71000,98101,10115, 11000 } },
                { "State",new List<object>() {"BiH","USA","GER", "NL" } },
                { "IsHome",new List<object>() { true, false, false, true} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55, 5.55 } },
                { "Date",new List<object>() { date3 , date2 , date1 , date2} },
                { "Age", new List<object>() { 31, 25, 45, 33 } },
                { "Gender", new List<object>() { "male", "female", "male", "female" } }
            };

            //create df
            var df = new DataFrame(dict);
            //group df by gender
            var gDf = df.GroupBy("Gender").Rolling(2, 2, new Dictionary<string, Aggregation>() { { "Values", Aggregation.Sum }, 
                                                                                                 { "Age", Aggregation.Avg } });
            //check result
            Assert.Equal(7.69, gDf["Values", 0]);
            Assert.Equal(8.76, gDf["Values", 1]);
            Assert.Equal(38d, gDf["Age", 0]);
            Assert.Equal(29d, gDf["Age", 1]);

        }
        [Fact]
        public void ClipValues()
        {
            var dic = new Dictionary<string, List<object>>
            {
                { "col1", new List<object> {0.335232,-1.367855,0.027753,0.230930,1.261967} },
                { "col2", new List<object> { -1.256177,0.746646,-1.176076,-0.679613,0.570967} }
            };
            ///
            var df = new DataFrame(dic);
            var clipedDf = df.Clip(-1.0f,0.5f);
            var expected = new List<object>() { 0.335232, -1.000000, -1.000000, 0.500000, 0.027753, -1.000000, 0.230930, -0.679613, 0.500000, 0.500000 };
            Assert.Equal(expected, clipedDf.Values);
        }
        [Fact]
        public void ClipValuesOneColumn()
        {
            var dic = new Dictionary<string, List<object>>
            {
                { "col1", new List<object> {0.335232,-1.367855,0.027753,0.230930,1.261967} },
                { "col2", new List<object> { -1.256177,0.746646,-1.176076,-0.679613,0.570967} }
            };
            ///
            var df = new DataFrame(dic);
            var clipedDf = df.Clip(-1.0f, 0.5f, "col2");
            var expected = new List<object>() { 0.335232, -1.000000, -1.367855, 0.500000, 0.027753, -1.000000, 0.230930, -0.679613, 1.261967, 0.500000 };
            Assert.Equal(expected, clipedDf.Values);
        }
        #endregion

        #region Insert and rename
        [Fact]
        public void InserColumn()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt");

            //add Age column
            var newCol = new List<object>(){ 31, 25, 45 };

            //inser column at third position
            var newdf = df.InsertColumn("Age", newCol, 2 );

            var str = df.ToStringBuilder();
            Assert.Equal(new string[] { "ID","City","Age","Zip Code","State","IsHome","Values","Date" }, newdf.Columns);
            Assert.Equal(new string[] { "ID", "City", "Zip Code", "State", "IsHome", "Values", "Date" }, df.Columns);

            Assert.Equal(new object[] { 1, "Sarajevo", 31, 71000 }, newdf[0].Take(4));
            Assert.Equal(new object[] { 2, "Seattle", 25, 98101 }, newdf[1].Take(4));
            Assert.Equal(new object[] { 3, "Berlin", 45, 10115 }, newdf[2].Take(4));

            Assert.Equal(new object[] { 1, "Sarajevo", 71000,"BiH" }, df[0].Take(4));
            Assert.Equal(new object[] { 2, "Seattle", 98101, "USA"}, df[1].Take(4));
            Assert.Equal(new object[] { 3, "Berlin", 10115,"GER" }, df[2].Take(4));

            //check for value
            newdf[0, 4] = 71100;
            df[0, 3] = 71200;
            Assert.Equal(71100, newdf[0,4]);
            Assert.Equal(71200, df[0, 3]);

        }

        [Fact]
        public void RenameColumn()
        {
            //create data frame with 3 rows and 7 columns
            var df = DataFrame.FromCsv($"{rootfolder}/simple_data_frame.txt");

            //add Age column
            var newCol = new List<object>() { 31, 25, 45 };

            //add column
            df = df.InsertColumn("Age", newCol, 2);
            df.Rename(("Age", "How old are you"), ("State", "Country"));
            var str = df.ToStringBuilder();
            Assert.Equal(new string[] { "ID", "City", "How old are you", "Zip Code", "Country", "IsHome", "Values", "Date" }, df.Columns);
            Assert.Equal(new object[] { 1, "Sarajevo", 31, 71000 }, df[0].Take(4));
            Assert.Equal(new object[] { 2, "Seattle", 25, 98101 }, df[1].Take(4));
            Assert.Equal(new object[] { 3, "Berlin", 45, 10115 }, df[2].Take(4));


        }
        #endregion

        #region Sorting
        [Fact]
        public void SortBy()
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
        public void SortByDescending()
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

            //
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

        #endregion

        #region Filter and Remove Rows
        [Fact]
        public void Filter()
        {
            var date1 = DateTime.Now.AddDays(-20);
            var date2 = DateTime.Now.AddDays(-10);
            var date3 = DateTime.Now.AddDays(-5);
            //define a dictionary of data
            var dict = new Dictionary<string, List<object>>
            {
                { "ID",new List<object>() { 1,2,3} },
                { "City",new List<object>() { "Sarajevo", "Seattle", "Berlin" } },
                { "Zip Code",new List<object>() { 71000,98101,10115 } },
                { "State",new List<object>() {"BiH","USA","GER" } },
                { "IsHome",new List<object>() { true, false, false} },
                { "Values",new List<object>() { 3.14, 3.21, 4.55 } },
                { "Date",new List<object>() { date3 , date2 , date1 } },
                { "Age", new List<object>() { 31, 25, 45 } },
                { "Gender", new List<object>() { "male", "female", "male" } }
            };

            //create df
            var df = new DataFrame(dict);

            //filter data frame between dates
            var opers = new FilterOperator[2] { FilterOperator.Greather, FilterOperator.Less };
            var cols = new string[] { "Date", "Date" };
            var values = (new DateTime[] { DateTime.Now.AddDays(-7), DateTime.Now.AddDays(-3) }).Select(x => (object)x).ToArray();

            var filteredDF = df.Filter(cols, values, opers);

            Assert.Equal(1, filteredDF.RowCount());
            Assert.Equal(new List<object>() { 1, "Sarajevo", 71000, "BiH", true, 3.14, date3, 31, "male" }, filteredDF[0]);
        }

        #endregion

    }
}
