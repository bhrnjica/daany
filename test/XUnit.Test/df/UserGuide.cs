using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

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
            DataFrame.SaveToCsv(filename, df);

            //create data frame with 3 rows and 7 columns
            var dfFromFile = DataFrame.FromCsv(filename, sep:',');

            //check the size of the data frame
            Assert.Equal(3, dfFromFile.RowCount());
            Assert.Equal(new string[] { "ID", "City", "Zip Code", "State", "IsHome", "Values", "Date" }, dfFromFile.Columns);
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
            Assert.Equal(2, row[0]);
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
        #endregion

        #region Drop and Missing Values
        #endregion

        #region GrouBy
        #endregion

        #region Insert and rename
        #endregion

        #region Sorting
        #endregion

        #region Rolling
        #endregion

        #region Take 
        #endregion

    }
}
