using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;
using Daany.Multikey;


namespace Unit.Test.DF
{
    public class CreateDataFrameTests
    {
		// Test for: public DataFrame(IDictionary<string, List<object>> data, IList<object>? index = null)
		[Fact]
		public void Constructor_FromDictionary_ShouldInitializeCorrectly()
		{
			// Arrange
			var data = new Dictionary<string, List<object>>
		    {
			    { "Column1", new List<object> { 1, 2, 3 } },
			    { "Column2", new List<object> { "A", "B", "C" } }
		    };
			var index = new List<object> { "Row1", "Row2", "Row3" };

			// Act
			var dataFrame = new DataFrame(data, index);

			// Assert
			Assert.Equal(data.Count, dataFrame.ColCount()); // Columns should match
			Assert.Equal(index, dataFrame.Index.ToList()); // Index should match
			Assert.Equal(6, dataFrame.Values.Count); // Flattened data length should match
		}

		[Fact]
		public void Constructor_FromDictionary_ShouldThrowForEmptyData()
		{
			// Arrange
			var data = new Dictionary<string, List<object>>();
			var index = new List<object> { "Row1", "Row2", "Row3" };

			// Act & Assert
			Assert.Throws<ArgumentException>(() => new DataFrame(data, index));
		}

		// Test for: public DataFrame(List<object> data, List<string> columns, ColType[]? colTypes = null)
		[Fact]
		public void Constructor_FromList_ShouldInitializeCorrectly()
		{
			// Arrange
			var data = new List<object> { 1, "A", 2, "B", 3, "C" };
			var columns = new List<string> { "Column1", "Column2" };
			var colTypes = new ColType[] { ColType.I32, ColType.STR };

			// Act
			var dataFrame = new DataFrame(data, columns, colTypes);

			// Assert
			Assert.Equal(data, dataFrame.Values); // Data should match
			Assert.Equal(columns, dataFrame.Columns); // Columns should match
			Assert.Equal(colTypes, dataFrame.ColTypes); // Column types should match
		}

		[Fact]
		public void Constructor_FromList_ShouldThrowForInvalidColumns()
		{
			// Arrange
			var data = new List<object> { 1, "A", 2, "B", 3 };
			var columns = new List<string> { "Column1", "Column2" };

			// Act & Assert
			Assert.Throws<ArgumentException>(() => new DataFrame(data, columns, null));
		}

		// Test for: public DataFrame(DataFrame dataFrame)
		[Fact]
		public void Constructor_FromExistingDataFrame_ShouldCloneCorrectly()
		{
			// Arrange
			var originalDataFrame = new DataFrame(
				new List<object> { 1, "A", 2, "B", 3, "C" },
				new List<string> { "Column1", "Column2" },
				new ColType[] { ColType.I32, ColType.STR }
			);

			// Act
			var clonedDataFrame = new DataFrame(originalDataFrame);

			// Assert
			Assert.Equal(originalDataFrame.Values, clonedDataFrame.Values); // Values should match
			Assert.NotSame(originalDataFrame.Values, clonedDataFrame.Values); // Ensure deep copy
			Assert.Equal(originalDataFrame.Columns, clonedDataFrame.Columns); // Columns should match
			Assert.NotSame(originalDataFrame.Columns, clonedDataFrame.Columns); // Ensure deep copy
		}

		[Fact]
		public void Constructor_FromExistingDataFrame_ShouldThrowForNullInput()
		{
			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new DataFrame(null,null,null));
		}

		// Test for: internal DataFrame(List<object> data, Index index, List<string> cols, ColType[] colsType)
		[Fact]
		public void Constructor_InternalWithIndex_ShouldInitializeCorrectly()
		{
			// Arrange
			var data = new List<object> { 1, "A", 2, "B", 3, "C" };
			var index = new Daany.Index(new List<object> { "Row1", "Row2", "Row3" });
			var columns = new List<string> { "Column1", "Column2" };
			var colTypes = new ColType[] { ColType.I32, ColType.STR };

			// Act
			var dataFrame = new DataFrame(data, index, columns, colTypes);

			// Assert
			Assert.Equal(data, dataFrame.Values); // Values should match
			Assert.Equal(index.ToList(), dataFrame.Index.ToList()); // Index should match
			Assert.Equal(columns, dataFrame.Columns); // Columns should match
			Assert.Equal(colTypes, dataFrame.ColTypes); // Column types should match
		}

		[Fact]
		public void Constructor_InternalWithIndex_ShouldThrowForMismatchedColumnTypes()
		{
			// Arrange
			var data = new List<object> { 1, "A", 2, "B", 3, "C" };
			var index = new Daany.Index(new List<object> { "Row1", "Row2", "Row3" });
			var columns = new List<string> { "Column1", "Column2" };
			var colTypes = new ColType[] { ColType.I32 }; // Mismatched column type length

			// Act & Assert
			Assert.Throws<ArgumentException>(() => new DataFrame(data, index, columns, colTypes));
		}

		// Test for: internal DataFrame(TwoKeysDictionary<string, object, object> aggValues)
		[Fact]
		public void Constructor_FromTwoKeysDictionary_ShouldInitializeCorrectly()
		{
			// Arrange
			var aggValues = new TwoKeysDictionary<string, object, object>
		{
			{ "Column1", "Row1", 1 },
			{ "Column1", "Row2", 2 },
			{ "Column2", "Row1", "A" },
			{ "Column2", "Row2", "B" }
		};

			// Act
			var dataFrame = new DataFrame(aggValues);

			// Assert
			Assert.Equal(4, dataFrame.Values.Count); // Data should match
			Assert.Equal(new List<object> { "Row1", "Row2" }, dataFrame.Index.ToList()); // Index should match
			Assert.Equal(new List<string> { "Column1", "Column2" }, dataFrame.Columns); // Columns should match
		}

		[Fact]
		public void Constructor_FromTwoKeysDictionary_ShouldHandleMissingData()
		{
			// Arrange
			var aggValues = new TwoKeysDictionary<string, object, object>
		{
			{ "Column1", "Row1", 1 },
			{ "Column2", "Row2", "B" } // Missing "Row1" for Column2
        };

			// Act
			var dataFrame = new DataFrame(aggValues);

			// Assert
			Assert.Equal(DataFrame.NAN, dataFrame.Values[1]); // Ensure missing data is NAN
		}

		[Fact]
        public void CreateFromList_Test01()
        {
            //list of object
            var list = new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            //defined columns
            var cols = new string[] { "col1", "col2" };

            //create data frame with two columns and 5 rows.
            var df = new DataFrame(list, cols);

            //check the size of the data frame
            Assert.Equal(5, df.RowCount());
            Assert.Equal(2, df.ColCount());
        }

        [Fact]
        public void CreateFromList_Failed_Test01()
        {
            //list of object
            var list = new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,11 };
            //defined columns
            var cols = new string[] { "col1", "col2" };

            //exception the number of list object is not divisible with column counts
            var exception = Assert.ThrowsAny<System.Exception>(() => new DataFrame(list, cols));
            Assert.Equal("The number of columns must evenly divide the length of the data.", exception.Message);
        }

        [Fact]
        public void CreateFromDictionary_Test02()
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
            
            //check the size of the data frame
            Assert.Equal(10, df.RowCount());
            Assert.Equal(10, df.ColCount());
        }

        [Fact]
        public void CreateFromDictionary_Failed_Test02()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91,101} },
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
            
            //exception the number of list object must all be the same
            var exception = Assert.ThrowsAny<System.Exception>(() => new DataFrame(dict));
            Assert.Equal("All lists within dictionary must be of the same length.", exception.Message);
        }

        [Fact]
        public void CreateFromCSVFile_Test01()
        {
            var filePath = $"testdata/group_sample_testdata.txt";
            var df = DataFrame.FromCsv(filePath: filePath, 
                                                sep: '\t', 
                                                names: null, dformat: null);
            //check the size of the data frame
            Assert.Equal(27, df.RowCount());
            Assert.Equal(6, df.ColCount());
        }

        [Fact]
        public void CreateFromCSVFile_Failed_Test01()
        {
            var filePath = $"../../../testdata/group_sample_testdata1.txt";
            
            //invalid path
            var exception = Assert.ThrowsAny<System.ArgumentException>(() => DataFrame.FromCsv(filePath: filePath,
                                                                                        sep: '\t',
                                                                                        names: null, dformat: null));
            
            Assert.Equal("filePath (Parameter 'File name does not exist.')", exception.Message);
        }

        [Fact]
        public void CreateTest01()
        {
            int row = 10;
            int nCols = 10;
            var nd = nc.ConsecutiveNum(row, nCols);
            var cols = Enumerable.Range(1, nCols).Select(x => $"col{x}").ToList();

            var df = new DataFrame(nd, cols);

            //row test
            var r1 = df[0].ToList();
            var r2 = df[4].ToList();
            var r3 = df[8].ToList();
            var e1 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var e2 = new int[] { 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };
            var e3 = new int[] { 81, 82, 83, 84, 85, 86, 87, 88, 89, 90 };
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r1[i], e1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r2[i], e2[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r3[i], e3[i]);

            //column test
            var c1 = new int[] { 1, 11, 21, 31, 41, 51, 61, 71, 81, 91 };
            var c2 = new int[] { 4, 14, 24, 34, 44, 54, 64, 74, 84, 94 };
            var c3 = new int[] { 8, 18, 28, 38, 48, 58, 68, 78, 88, 98 };
            var cc1 = df["col1"].ToList();
            var cc2 = df["col4"].ToList();
            var cc3 = df["col8"].ToList();
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c1[i], cc1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c2[i], cc2[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c3[i], cc3[i]);


            var cell = df["col1", 1];
            Assert.Equal(11, (int)cell);
            cell = df["col3", 5];
            Assert.Equal(53, (int)cell);


            cell = df[1, 1];
            Assert.Equal(12, (int)cell);
            cell = df[4, 3];
            Assert.Equal(44, (int)cell);
            cell = df[2, 8];
            Assert.Equal(29, (int)cell);
        }
       
        [Fact]
        public void CreateTest02()
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
            //row test
            var r1 = df[0].ToList();
            var r2 = df[4].ToList();
            var r3 = df[8].ToList();
            var e1 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var e2 = new int[] { 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };
            var e3 = new int[] { 81, 82, 83, 84, 85, 86, 87, 88, 89, 90 };
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r1[i], e1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r2[i], e2[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)r3[i], e3[i]);

            //column test
            var c1 = new int[] { 1, 11, 21, 31, 41, 51, 61, 71, 81, 91 };
            var c2 = new int[] { 4, 14, 24, 34, 44, 54, 64, 74, 84, 94 };
            var c3 = new int[] { 8, 18, 28, 38, 48, 58, 68, 78, 88, 98 };
            var cc1 = df["col1"].ToList();
            var cc2 = df["col4"].ToList();
            var cc3 = df["col8"].ToList();
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c1[i], cc1[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c2[i], cc2[i]);
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c3[i], cc3[i]);


            var cell = df["col1", 1];
            Assert.Equal(11, (int)cell);
            cell = df["col3", 5];
            Assert.Equal(53, (int)cell);


            cell = df[1, 1];
            Assert.Equal(12, (int)cell);
            cell = df[4, 3];
            Assert.Equal(44, (int)cell);
            cell = df[2, 8];
            Assert.Equal(29, (int)cell);
        }

		[Fact]
		public void Create_ShouldRenameSpecifiedColumns()
		{
			// Arrange
			var originalDf = new DataFrame(
				new List<object> { 1, "A", 2, "B", 3, "C" },
				new List<string> { "Column1", "Column2" },
				null
			);

			// Act
			var newDf = originalDf.Create(("Column1", "NewColumn1"), ("Column2", "NewColumn2"));

			// Assert
			Assert.Equal("NewColumn1", newDf.Columns[0]);
			Assert.Equal("NewColumn2", newDf.Columns[1]);
		}

		[Fact]
		public void Create_ShouldKeepOriginalNamesIfNewNamesAreEmpty()
		{
			// Arrange
			var originalDf = new DataFrame(
				new List<object> { 1, "A", 2, "B", 3, "C" },
				new List<string> { "Column1", "Column2" },
				null
			);

			// Act
			var newDf = originalDf.Create(("Column1", ""), ("Column2", null));

			// Assert
			Assert.Equal("Column1", newDf.Columns[0]);
			Assert.Equal("Column2", newDf.Columns[1]);
		}

		[Fact]
		public void Create_ShouldThrowExceptionForInvalidOldName()
		{
			// Arrange
			var originalDf = new DataFrame(
				new List<object> { 1, "A", 2, "B", 3, "C" },
				new List<string> { "Column1", "Column2" },
				null
			);

			// Act & Assert
			Assert.Throws<ArgumentException>(() => originalDf.Create(("NonExistentColumn", "NewColumn")));
		}

		[Fact]
		public void Create_ShouldPreserveColumnOrderAsSpecified()
		{
			// Arrange
			var originalDf = new DataFrame(
				new List<object> { 1, "A", 2, "B", 3, "C" },
				new List<string> { "Column1", "Column2", "Column3" },
				null
			);

			// Act
			var newDf = originalDf.Create(("Column2", "RenamedColumn2"), ("Column1", "RenamedColumn1"));

			// Assert
			Assert.Equal("RenamedColumn2", newDf.Columns[0]);
			Assert.Equal("RenamedColumn1", newDf.Columns[1]);
		}


		[Fact]
        public void CreateDataFrameFromExisted_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "catId",new List<object>() { "A", "A", "B", "B" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };
            
            //
            var df1 = new DataFrame(dict);

            var df2 = df1.Create(("itemID",null), ("value1", "value"));

            //test
            var c1f1 = df1["itemID"].ToList();
            var c1f2 = df1["value1"].ToList();

            var c2f1 = df2["itemID"].ToList();
            var c2f2 = df2["value"].ToList();

            for (int i = 0; i < c1f1.Count(); i++)
                Assert.Equal(c1f1[i].ToString(), c2f1[i].ToString());
            for (int i = 0; i < c2f2.Count(); i++)
                Assert.Equal(c1f2[i], c2f2[i]);
           

        }

        [Fact]
        public void CreateDataFrameFromExisted_ByChecking_ColTypes_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "catId",new List<object>() { "A", "A", "B", "B" } },
                { "value1",new List<object>() { 1,2,3,4 } },
                { "value2",new List<object>() { true,false,true,true } },
            };

            //
            var df1 = new DataFrame(dict);
            var cT = new ColType[] { ColType.STR, ColType.IN, ColType.I32, ColType.I2 };
            df1.SetColumnType("catId",ColType.IN);

            Assert.Equal(ColType.IN, df1.ColTypes[1]);

            //create new dataframe
            var newdf = df1["itemID", "catId", "value1", "value2"];
            Assert.Equal(newdf.ColTypes, cT);
        }

		[Fact]
		public void CreateEmpty_ShouldInitializeEmptyDataFrameWithColumns()
		{
			// Arrange
			var columns = new List<string> { "Column1", "Column2", "Column3" };

			// Act
			var emptyDf = DataFrame.CreateEmpty(columns);

			// Assert
			Assert.NotNull(emptyDf.Values);
			Assert.Empty(emptyDf.Values); // Values should be empty
			Assert.NotNull(emptyDf.Index);
			Assert.Empty(emptyDf.Index.ToList()); // Index should be empty
			Assert.Equal(columns, emptyDf.Columns); // Columns should match input
		}

		[Fact]
		public void CreateEmpty_ShouldThrowForNullColumns()
		{
			// Act & Assert
			Assert.Throws<ArgumentException>(() => DataFrame.CreateEmpty(null));
		}

		[Fact]
		public void CreateEmpty_ShouldThrowForEmptyColumns()
		{
			// Arrange
			var columns = new List<string>();

			// Act & Assert
			Assert.Throws<ArgumentException>(() => DataFrame.CreateEmpty(columns));
		}


	}
}
