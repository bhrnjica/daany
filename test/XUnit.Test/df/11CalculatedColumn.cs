using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;

namespace Unit.Test.DF
{
    public class CalculatedColumnsTests
    {
		private DataFrame CreateSampleDataFrame()
		{
			return new DataFrame(
				new List<object> { 1, 2, 3, 4, 5, 6 },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.I32 });
		}


		[Fact]
		public void Append_Vertically_ValidInput_ShouldCombineRows()
		{
			// Arrange
			var df1 = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			var df2 = new DataFrame(
				new List<object> { 3, "C", 4, "D" },
				new List<object> { 2, 3 },
				new List<string> { "Col1", "Col2" },
				null);

			// Act
			var appendedDf = df1.Append(df2, true);

			// Assert
			Assert.Equal(8, appendedDf.Values.Count);
			Assert.Equal(4, appendedDf.Index.Count);
		}

		[Fact]
		public void Append_Horizontally_ValidInput_ShouldCombineColumns()
		{
			// Arrange
			var df1 = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			var df2 = new DataFrame(
				new List<object> { 3, 4 },
				new List<object> { 0, 1 },
				new List<string> { "Col3" },
				null);

			// Act
			var appendedDf = df1.Append(df2, false);

			// Assert
			Assert.Equal(3, appendedDf.Columns.Count);
			Assert.Equal("Col3", appendedDf.Columns[2]);
		}

		[Fact]
		public void Append_Vertically_ColumnCountMismatch_ShouldThrowException()
		{
			// Arrange
			var df1 = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			var df2 = new DataFrame(
				new List<object> { 3, "C" },
				new List<object> { 2,3 },
				new List<string> { "Col1" }, // Only 1 column.
				null);

			// Act & Assert
			Assert.Throws<ArgumentException>(() => df1.Append(df2, true));
		}

		[Fact]
		public void Append_Horizontally_RowCountMismatch_ShouldThrowException()
		{
			// Arrange
			var df1 = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			var df2 = new DataFrame(
				new List<object> { 3, "C", 4, "D", 5, "E" },
				new List<object> { 0, 1, 2 },
				new List<string> { "Col3", "Col4" },
				null);

			// Act & Assert
			Assert.Throws<ArgumentException>(() => df1.Append(df2, false));
		}


		[Fact]
        public void AddCalculatedColumn_Test01()
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
            var df01 = df.AddCalculatedColumn("col11", (IDictionary<string, object> row, int i) => i + 11);
           
            //column test
            var c1 = new int[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            var cc1 = df["col11"].ToList();

            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c1[i], cc1[i]);

            Assert.Equal(df.Columns.Count, df.ColTypes.Count);

        }

        [Fact]
        public void AddCalculatedColumn_Test02()
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
            var df01 = df.AddCalculatedColumn("col11", (object[] row, int i) => i + 11);

            //column test
            var c1 = new int[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            var cc1 = df["col11"].ToList();
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c1[i], cc1[i]);

            Assert.Equal(df.Columns.Count, df.ColTypes.Count);

        }


        [Fact]
        public void AddCalculatedColumn_Test03()
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
            var sCols = new string[] { "col11" };
            var df01 = df.AddCalculatedColumns(sCols, (row, i) => calculate(row,i) );
            
            
            //local function declaration
            object[] calculate(object[] row, int i)
                {return new object[1] { i + 11 };}


            //column test
            var c1 = new int[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            var cc1 = df["col11"].ToList();
            for (int i = 0; i < 10; i++)
                Assert.Equal((int)c1[i], cc1[i]);

            Assert.Equal(df.Columns.Count, df.ColTypes.Count);

        }

        [Fact]
        public void AddSameCalculatedColumn_Test01()
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
            var sCols = new string[] { "col10" };

            //exception the number of list object is not divisible with column counts
            var exception = Assert.ThrowsAny<System.Exception>(() => df.AddCalculatedColumns(sCols, (row, i) => calculate(row, i)));
            Assert.Equal("Column(s) 'col10' already exist(s) in the data frame.", exception.Message);

            
            //local function declaration
            object[] calculate(object[] row, int i)
            { return new object[1] { i + 11 }; }

        }

		[Fact]
		public void AddColumns_MismatchedRowCount_ShouldThrowException()
		{
			// Arrange
			var dataFrame = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			var invalidColumns = new Dictionary<string, List<object>>
	        {
		        { "Col3", new List<object> { 3 } } // Row count mismatch.
            };

			// Act & Assert
			Assert.Throws<ArgumentException>(() => dataFrame.AddColumns(invalidColumns));
		}

		[Fact]
        public void AddColumns_Test01()
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
                
            };
            //
            var df = new DataFrame(dict);

            //define three new columns
            var d = new Dictionary<string, List<object>>
            {
                { "col8",new List<object>() { 8,18,28,38,48,58,68,78,88,98} },
                { "col9",new List<object>() { 9,19,29,39,49,59,69,79,89,99} },
                { "col10",new List<object>() { 10,20,30,40,50,60,70,80,90,100} },

            };

            //add three new columns
            var newDf =  df.AddColumns(d);
            
            Assert.Equal(7, df.ColCount());
            Assert.Equal(10, newDf.ColCount());

            for (int i = 0; i < newDf.Values.Count; i++)
                Assert.Equal(i+1, newDf.Values[i]);

            Assert.Equal(df.Columns.Count, df.ColTypes.Count);

        }

        [Fact]
        public void AddSameColumns_Test01()
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

            };
            //
            var df = new DataFrame(dict);

            //define three new columns
            var d = new Dictionary<string, List<object>>
            {
                { "col5",new List<object>() { 8,18,28,38,48,58,68,78,88,98} },
                { "col2",new List<object>() { 9,19,29,39,49,59,69,79,89,99} },
                { "col10",new List<object>() { 10,20,30,40,50,60,70,80,90,100} },

            };

            //exception the number of list object is not divisible with column counts
            var exception = Assert.ThrowsAny<System.Exception>(() => df.AddColumns(d));
            Assert.Equal("Column(s) 'col2, col5' already exist(s) in the data frame.", exception.Message);


        }


		[Fact]
		public void InsertColumn_ShouldAppendColumn_WhenPositionIsMinusOne()
		{
			// Arrange
			var df = CreateSampleDataFrame();
			var newColValues = new List<object> { 7, 8 ,9};

			// Act
			var newDf = df.InsertColumn("col3", newColValues, -1);

			// Assert
			Assert.Equal(new List<object> { 1, 2, 7, 3, 4, 8, 5, 6, 9 }, newDf.Values);
			Assert.Equal(new List<string> { "col1", "col2", "col3" }, newDf.Columns);
		}

		[Fact]
		public void InsertColumn_ShouldInsertColumn_WhenPositionIsZero()
		{
			// Arrange
			var df = CreateSampleDataFrame();
			var newColValues = new List<object> { 9, 10, 11 };

			// Act
			var newDf = df.InsertColumn("col0", newColValues, 0);

			// Assert
			Assert.Equal(new List<object> { 9, 1, 2, 10, 3, 4, 11, 5, 6 }, newDf.Values);
			Assert.Equal(new List<string> { "col0", "col1", "col2" }, newDf.Columns);
		}

		[Fact]
		public void InsertColumn_ShouldThrowException_WhenValueIsNull()
		{
			// Arrange
			var df = CreateSampleDataFrame();

			// Act & Assert
			Assert.Throws<ArgumentException>(() => df.InsertColumn("colNew", null, -1));
		}

		[Fact]
		public void InsertColumn_ShouldThrowException_WhenRowCountsMismatch()
		{
			// Arrange
			var df = CreateSampleDataFrame();
			var invalidColValues = new List<object> { 7 }; // Mismatched row count

			// Act & Assert
			Assert.Throws<ArgumentException>(() => df.InsertColumn("colNew", invalidColValues, -1));
		}

		[Fact]
		public void InsertColumn_ShouldInsertColumn_WhenPositionIsMiddle()
		{
			// Arrange
			var df = CreateSampleDataFrame();
			var newColValues = new List<object> { 7, 8, 9 };

			// Act
			var newDf = df.InsertColumn("colMiddle", newColValues, 1);

			// Assert
			Assert.Equal(new List<object> { 1, 7, 2, 3, 8, 4, 5, 9 ,6 }, newDf.Values);
			Assert.Equal(new List<string> { "col1", "colMiddle", "col2" }, newDf.Columns);
		}

		[Fact]
		public void InsertColumn_ShouldThrowArgumentException()
		{
			// Arrange
			var df = new DataFrame(new List<object>(), new List<object>(), new List<string>(), new ColType[0]);
			var newColValues = new List<object> { 1, 2 };

			// Act & Assert
			Assert.Throws<ArgumentException>(() =>
				 df.InsertColumn("col1", newColValues));
		}

		[Fact]
        public void InsertColumn_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
                { "col2",new List<object>() { 2,12,22,32,42,52,62,72,82,92 } },
                { "col3",new List<object>() { 3,13,23,33,43,53,63,73,83,93 } },
                { "col4",new List<object>() { 4,14,24,34,44,54,64,74,84,94} },
               
                { "col6",new List<object>() { 6,16,26,36,46,56,66,76,86,96} },
                { "col7",new List<object>() { 7,17,27,37,47,57,67,77,87,97 } },
                { "col8",new List<object>() { 8,18,28,38,48,58,68,78,88,98} },
                { "col9",new List<object>() { 9,19,29,39,49,59,69,79,89,99} },
                { "col10",new List<object>() { 10,20,30,40,50,60,70,80,90,100} },
            };
            //
            var df = new DataFrame(dict);

            //define three new columns
            var d = new List<object>() { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 };

            //add three new columns
            df = df.InsertColumn("col5",d,4);
            Assert.Equal("col5", df.Columns[4]);

            for (int i = 0; i < df.Values.Count; i++)
                Assert.Equal(i + 1, df.Values[i]);

            Assert.Equal(df.Columns.Count, df.ColTypes.Count);

        }


        [Fact]
        public void InsertColumn_Test02()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
                { "col3",new List<object>() { 3,13,23,33,43,53,63,73,83,93 } },
                { "col4",new List<object>() { 4,14,24,34,44,54,64,74,84,94} },
                { "col7",new List<object>() { 7,17,27,37,47,57,67,77,87,97 } },
                { "col8",new List<object>() { 8,18,28,38,48,58,68,78,88,98} },
                { "col9",new List<object>() { 9,19,29,39,49,59,69,79,89,99} },
                { "col10",new List<object>() { 10,20,30,40,50,60,70,80,90,100} },
            };
            //
            var df = new DataFrame(dict);

            //define three new columns
            var d1 = new List<object>() { 2, 12, 22, 32, 42, 52, 62, 72, 82, 92 };
            var d2 = new List<object>() { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 };
            var d3 = new List<object>() { 6, 16, 26, 36, 46, 56, 66, 76, 86, 96 };

            //add three new columns
            df=df.InsertColumn("col2", d1, 1);
            df=df.InsertColumn("col5", d2, 4);
            df=df.InsertColumn("col6", d3, 5);


            Assert.Equal("col2", df.Columns[1]);
            Assert.Equal("col5", df.Columns[4]);
            Assert.Equal("col6", df.Columns[5]);

            for (int i = 0; i < df.Values.Count; i++)
                Assert.Equal(i + 1, df.Values[i]);

            Assert.Equal(df.Columns.Count, df.ColTypes.Count);

        }

        [Fact]
        public void InsertColumn_Test03()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1,11,21,31,41,51,61,71,81,91} },
                { "col2",new List<object>() { 2,12,22,32,42,52,62,72,82,92 } },
                { "col3",new List<object>() { 3,13,23,33,43,53,63,73,83,93 } },
                { "col4",new List<object>() { 4,14,24,34,44,54,64,74,84,94} },
                { "col5",new List<object>() { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 } },
                { "col6",new List<object>() { 6,16,26,36,46,56,66,76,86,96} },
                { "col7",new List<object>() { 7,17,27,37,47,57,67,77,87,97 } },
                { "col8",new List<object>() { 8,18,28,38,48,58,68,78,88,98} },
                { "col9",new List<object>() { 9,19,29,39,49,59,69,79,89,99} },
            };
            //
            var df = new DataFrame(dict);

            //define three new columns
            var d = new List<object>() { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

            //add three new columns
            df = df.InsertColumn("col10", d);
            Assert.Equal("col10", df.Columns[9]);

            for (int i = 0; i < df.Values.Count; i++)
                Assert.Equal(i + 1, df.Values[i]);

            Assert.Equal(df.Columns.Count, df.ColTypes.Count);

        }

		[Fact]
		public void AddCalculatedColumn_IDictionary_ValidInput_ShouldAddColumn()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			// Act
			var success = df.AddCalculatedColumn("NewCol", (row, index) => row["Col1"].ToString() + row["Col2"].ToString());

			// Assert
			Assert.True(success);
			Assert.Equal(3, df.Columns.Count);
			Assert.Equal("1A", df["NewCol", 0]);
			Assert.Equal("2B", df["NewCol", 1]);
		}

		[Fact]
		public void AddCalculatedColumn_IDictionary_NullColumnName_ShouldThrowException()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			// Act & Assert
			Assert.Throws<ArgumentException>(() =>
				df.AddCalculatedColumn(null, (row, index) => row["Col1"].ToString()));
		}

		[Fact]
		public void AddCalculatedColumn_IDictionary_DuplicateColumnName_ShouldThrowException()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			// Act & Assert
			Assert.Throws<ArgumentException>(() =>
				df.AddCalculatedColumn("Col1", (row, index) => row["Col2"].ToString()));
		}

		[Fact]
		public void AddCalculatedColumn_Array_ValidInput_ShouldAddColumn()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			// Act
			var success = df.AddCalculatedColumn("NewCol", (row, index) => row[0].ToString() + row[1].ToString());

			// Assert
			Assert.True(success);
			Assert.Equal(3, df.Columns.Count);
			Assert.Equal("1A", df["NewCol", 0]);
			Assert.Equal("2B", df["NewCol", 1]);
		}


		[Fact]
		public void AddCalculatedColumn_Array_NullCallback_ShouldThrowException()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() =>
				df.AddCalculatedColumn("NewCol", (Func<IDictionary<string,object>, int, object>)null));
		}

		[Fact]
		public void AddCalculatedColumns_IDictionary_ValidInput_ShouldAddColumns()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			// Act
			var success = df.AddCalculatedColumns(
				new[] { "Col3", "Col4" },
				(row, index) => new object[] { (int)row["Col1"] * 2, row["Col2"].ToString().ToLower() });

			// Assert
			Assert.True(success);
			Assert.Equal(4, df.Columns.Count);
			Assert.Equal(2, df["Col3", 0]);
			Assert.Equal("a", df["Col4", 0]);
		}

		[Fact]
		public void AddCalculatedColumns_IDictionary_MismatchedValues_ShouldThrowException()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			// Act & Assert
			Assert.Throws<ArgumentException>(() =>
				df.AddCalculatedColumns(
					new[] { "Col3", "Col4" },
					(row, index) => new object[] { (int)row["Col1"] * 2 })); // Only 1 value returned instead of 2
		}

		[Fact]
		public void AddCalculatedColumns_Array_ValidInput_ShouldAddColumns()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			// Act
			var success = df.AddCalculatedColumns(
				new[] { "Col3", "Col4" },
				(row, index) => new object[] { (int)row[0] * 2, row[1].ToString().ToLower() });

			// Assert
			Assert.True(success);
			Assert.Equal(4, df.Columns.Count);
			Assert.Equal(2, df["Col3", 0]);
			Assert.Equal("a", df["Col4", 0]);
		}

		[Fact]
		public void AddCalculatedColumns_Array_NullColumnNames_ShouldThrowException()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			// Act & Assert
			Assert.Throws<ArgumentException>(() =>
				df.AddCalculatedColumns(null, (row, index) => new object[] { row[0] }));
		}



		[Fact]
		public void AddCalculatedColumns_InconsistentCalculatedValues_ShouldThrowException()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" },
				null);

			// Act & Assert
			Assert.Throws<ArgumentException>(() => df.AddCalculatedColumns(
				new[] { "Col3", "Col4" },
				(row, index) => new object[] { row[0], row[1].ToString().ToLower(), "extra_value" })); // Too many values
		}

	}

}
