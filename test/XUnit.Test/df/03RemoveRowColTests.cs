using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Daany;


namespace Unit.Test.DF
{
    public class RemoveRowsColsTests
    {
		[Fact]
		public void RemoveRows_ShouldRemoveMatchingRows1()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 5, "A", 15, "B", 25, "C" },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });

			Func<object[], int, bool> condition = (row, index) => (int)row[0] > 10;

			// Act
			var newDf = df.RemoveRows(condition);

			// Assert
			Assert.Equal(new List<object> { 5, "A" }, newDf.Values);
			Assert.Equal(new List<object> { "row1" }, newDf.Index);
			Assert.Equal(new List<string> { "col1", "col2" }, newDf.Columns);
		}

		[Fact]
		public void RemoveRows_ShouldKeepAllRows_WhenNoneMatchCondition1()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 5, "A", 6, "B", 7, "C" },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });

			Func<object[], int, bool> condition = (row, index) => (int)row[0] > 10;

			// Act
			var newDf = df.RemoveRows(condition);

			// Assert
			Assert.Equal(df.Values, newDf.Values);
			Assert.Equal(df.Index, newDf.Index);
			Assert.Equal(df.Columns, newDf.Columns);
		}

		[Fact]
		public void RemoveRows_ShouldRemoveAllRows_WhenAllMatchCondition1()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 15, "A", 25, "B", 35, "C" },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });

			Func<object[], int, bool> condition = (row, index) => (int)row[0] > 10;

			// Act
			var newDf = df.RemoveRows(condition);

			// Assert
			Assert.Empty(newDf.Values);
			Assert.Empty(newDf.Index);
			Assert.Equal(df.Columns, newDf.Columns);
		}

		[Fact]
		public void RemoveRows_ShouldThrowException_WhenConditionIsNull1()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 5, "A", 15, "B", 25, "C" },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });

			// Act & Assert
			Assert.Throws<ArgumentException>(() => df.RemoveRows((Func<IDictionary<string, object>, int, bool>)null));
		}


		[Fact]
		public void RemoveRows_ShouldThrowException_WhenConditionIsNull()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 5, "A", 15, "B", 25, "C" },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });

			// Act & Assert
			Assert.Throws<ArgumentException>(() => df.RemoveRows((Func<object[], int, bool>)null));
		}

		[Fact]
		public void RemoveRows_ShouldRemoveAllRows_WhenAllMatchCondition()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 15, "A", 25, "B", 35, "C" },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });

			Func<IDictionary<string, object>, int, bool> condition = (row, index) => (int)row["col1"] > 10;

			// Act
			var newDf = df.RemoveRows(condition);

			// Assert
			Assert.Empty(newDf.Values);
			Assert.Empty(newDf.Index);
			Assert.Equal(df.Columns, newDf.Columns);
		}

		[Fact]
		public void RemoveRows_ShouldKeepAllRows_WhenNoneMatchCondition()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 5, "A", 6, "B", 7, "C" },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });

			Func<IDictionary<string, object>, int, bool> condition = (row, index) => (int)row["col1"] > 10;

			// Act
			var newDf = df.RemoveRows(condition);

			// Assert
			Assert.Equal(df.Values, newDf.Values);
			Assert.Equal(df.Index, newDf.Index);
			Assert.Equal(df.Columns, newDf.Columns);
		}

		[Fact]
		public void RemoveRows_ShouldRemoveMatchingRows()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 5, "A", 15, "B", 25, "C" },
				new List<object> { "row1", "row2", "row3" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.STR });

			Func<IDictionary<string, object>, int, bool> condition = (row, index) => (int)row["col1"] > 10;

			// Act
			var newDf = df.RemoveRows(condition);

			// Assert
			Assert.Equal(new List<object> { 5, "A" }, newDf.Values);
			Assert.Equal(new List<object> { "row1" }, newDf.Index);
			Assert.Equal(new List<string> { "col1", "col2" }, newDf.Columns);
		}


		[Fact]
		public void AddRow_ValidRow_ShouldAppendRow()
		{
			// Arrange
			var dataFrame = new DataFrame(
				new List<object> { 1, "A", 2 ,"B", 4, "D" },
				new List<string> { "Col1", "Col2" });

			var newRow = new List<object> { 3, "C"};

			// Act
			dataFrame.AddRow(newRow);

			// Assert
			Assert.Equal(8, dataFrame.Values.Count); // Ensure new row is added.
			Assert.Equal(4, dataFrame.Index.Count); // Ensure index is updated.
			Assert.Equal("C", dataFrame["Col2",3]); // Check value in the first column of the added row.
		}

		[Fact]
		public void AddRow_NullRow_ShouldThrowArgumentException()
		{
			// Arrange
			var dataFrame = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<string> { "Col1", "Col2" }
				);

			// Act & Assert
			Assert.Throws<ArgumentException>(() => dataFrame.AddRow(null));
		}

		[Fact]
		public void AddRow_InconsistentRowLength_ShouldThrowArgumentException()
		{
			// Arrange
			var dataFrame = new DataFrame(
				new List<object> { 1, "A", 2, "B" },
				new List<object> { 0, 1 },
				new List<string> { "Col1", "Col2" });

			var invalidRow = new List<object> { 3, "B", 4 }; // Only 2 values instead of 3.

			// Act & Assert
			Assert.Throws<ArgumentException>(() => dataFrame.AddRow(invalidRow));
		}


		[Fact]
        public void RemoveColumns_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "catId",new List<object>() { "A", "A", "B", "B" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };

            //
            var df1 = new DataFrame(dict);

            var df2 = df1.Drop("catId");

            //test
            var c1f1 = df1["itemID"].ToList();
            var c1f2 = df1["value1"].ToList();
            Assert.Equal(3, df1.Columns.Count);

            var c2f1 = df2["itemID"].ToList();
            var c2f2 = df2["value1"].ToList();
            Assert.Equal(2, df2.Columns.Count);

            for (int i = 0; i < c1f1.Count(); i++)
                Assert.Equal(c1f1[i].ToString(), c2f1[i].ToString());
            for (int i = 0; i < c2f2.Count(); i++)
                Assert.Equal(c1f2[i], c2f2[i]);


        }
        [Fact]
        public void DropByIndex_SingleColumn_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "itemID",new List<object>() { "foo", "bar", "baz", "foo" } },
                { "catId",new List<object>() { "A", "A", "B", "B" } },
                { "value1",new List<object>() { 1,2,3,4 } },
            };

            var df1 = new DataFrame(dict);

            // Drop middle column by index (catId is at index 1)
            var df2 = df1.Drop(1);

            // Test
            Assert.Equal(3, df1.Columns.Count);
            Assert.Equal(2, df2.Columns.Count);
            Assert.Equal(new List<string> { "itemID", "value1" }, df2.Columns);

            // Verify data integrity
            var c1f1 = df1["itemID"].ToList();
            var c1f3 = df1["value1"].ToList();
            var c2f1 = df2["itemID"].ToList();
            var c2f2 = df2["value1"].ToList();

            for (int i = 0; i < c1f1.Count(); i++)
            {
                Assert.Equal(c1f1[i].ToString(), c2f1[i].ToString());
                Assert.Equal(c1f3[i], c2f2[i]);
            }
        }

        [Fact]
        public void DropByIndex_MultipleColumns_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1, 2, 3 } },
                { "col2",new List<object>() { "A", "B", "C" } },
                { "col3",new List<object>() { 10, 20, 30 } },
                { "col4",new List<object>() { true, false, true } },
            };

            var df1 = new DataFrame(dict);

            // Drop first and third columns (indices 0 and 2)
            var df2 = df1.Drop(0, 2);

            // Test
            Assert.Equal(4, df1.Columns.Count);
            Assert.Equal(2, df2.Columns.Count);
            Assert.Equal(new List<string> { "col2", "col4" }, df2.Columns);

            // Verify data integrity
            var originalCol2 = df1["col2"].ToList();
            var originalCol4 = df1["col4"].ToList();
            var newCol1 = df2["col2"].ToList();
            var newCol2 = df2["col4"].ToList();

            Assert.Equal(originalCol2, newCol1);
            Assert.Equal(originalCol4, newCol2);
        }

        [Fact]
        public void DropByIndex_NegativeIndices_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1, 2, 3 } },
                { "col2",new List<object>() { "A", "B", "C" } },
                { "col3",new List<object>() { 10, 20, 30 } },
            };

            var df1 = new DataFrame(dict);

            // Drop last column using negative index
            var df2 = df1.Drop(-1);

            // Test
            Assert.Equal(3, df1.Columns.Count);
            Assert.Equal(2, df2.Columns.Count);
            Assert.Equal(new List<string> { "col1", "col2" }, df2.Columns);

            // Drop last two columns using negative indices
            var df3 = df1.Drop(-1, -2);

            Assert.Equal(1, df3.Columns.Count);
            Assert.Equal(new List<string> { "col1" }, df3.Columns);
        }

        [Fact]
        public void DropByIndex_DuplicateIndices_Test()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1, 2, 3 } },
                { "col2",new List<object>() { "A", "B", "C" } },
                { "col3",new List<object>() { 10, 20, 30 } },
            };

            var df1 = new DataFrame(dict);

            // Drop the same column twice (should only drop once)
            var df2 = df1.Drop(1, 1);

            // Test
            Assert.Equal(3, df1.Columns.Count);
            Assert.Equal(2, df2.Columns.Count);
            Assert.Equal(new List<string> { "col1", "col3" }, df2.Columns);
        }

        [Fact]
        public void DropByIndex_EmptyArray_ShouldThrow()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1, 2, 3 } },
            };

            var df = new DataFrame(dict);

            // Should throw when no indices provided
            Assert.Throws<ArgumentException>(() => df.Drop(new int[0]));
        }

        [Fact]
        public void DropByIndex_NullArray_ShouldThrow()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1, 2, 3 } },
            };

            var df = new DataFrame(dict);

            // Should throw when null provided
            Assert.Throws<ArgumentException>(() => df.Drop((int[])null));
        }

        [Fact]
        public void DropByIndex_OutOfBounds_ShouldThrow()
        {
            var dict = new Dictionary<string, List<object>>
            {
                { "col1",new List<object>() { 1, 2, 3 } },
                { "col2",new List<object>() { "A", "B", "C" } },
            };

            var df = new DataFrame(dict);

            // Should throw for positive index out of bounds
            Assert.Throws<IndexOutOfRangeException>(() => df.Drop(2));
            
            // Should throw for negative index out of bounds
            Assert.Throws<IndexOutOfRangeException>(() => df.Drop(-3));
        }

        [Fact]
        public void Remove_Test01()
        {
            var dict = new Dictionary<string, List<object>>
            {
                {"product_id",new List<object>() {1,1,2,2,2,2,2 } },
                { "retail_price",new List<object>() { 2,2,5,5,5,5,5 } },
                { "quantity",new List<object>() { 1,2,4,8,16,32,64 } },
                { "city",new List<object>() { "SF","SJ","SF","SJ","Miami", "Orlando","SJ"} },
                { "state" ,new List<object>() { "CA","CA","CA","CA","FL","FL","PR" } },
            };


            var df = new DataFrame(dict);

            //remove rows with 'Miami'
            DataFrame newDf = null;
            df = df.RemoveRows((row, i) => row["city"].ToString() == "Miami");

            Assert.True(newDf == null);


            Assert.Equal(1f, Convert.ToSingle(df["product_id", 0]));
            Assert.Equal(2f, Convert.ToSingle(df["retail_price", 0]));
            Assert.Equal(1f, Convert.ToSingle(df["quantity", 0]));

            Assert.Equal(1f, Convert.ToSingle(df["product_id", 1]));
            Assert.Equal("SJ", Convert.ToString(df["city", 1]));
            Assert.Equal("CA", Convert.ToString(df["state", 1]));

            Assert.Equal("SF", Convert.ToString(df["city", 2]));
            Assert.Equal("CA", Convert.ToString(df["state", 2]));
            Assert.Equal(4f, Convert.ToSingle(df["quantity", 2]));

            Assert.Equal("Orlando", Convert.ToString(df["city", 4]));
            Assert.Equal(5f, Convert.ToSingle(df["retail_price", 4]));
            Assert.Equal("FL", Convert.ToString(df["state", 4]));

            Assert.Equal(2f, Convert.ToSingle(df["product_id", 5]));
            Assert.Equal("SJ", Convert.ToString(df["city", 5]));
            Assert.Equal("PR", Convert.ToString(df["state", 5]));


            //remove rows with 'Miami'
            var df1 = new DataFrame(dict);
            newDf = df1.RemoveRows((row, i) => row["city"].ToString() == "Miami");

            Assert.True(newDf != null);

            for (int i = 0; i < df.Values.Count; i++)
                Assert.True(df.Values[i].Equals(newDf.Values[i]));

            Assert.Equal(1f, Convert.ToSingle(newDf["product_id", 0]));
            Assert.Equal(2f, Convert.ToSingle(newDf["retail_price", 0]));
            Assert.Equal(1f, Convert.ToSingle(newDf["quantity", 0]));

            Assert.Equal(1f, Convert.ToSingle(newDf["product_id", 1]));
            Assert.Equal("SJ", Convert.ToString(newDf["city", 1]));
            Assert.Equal("CA", Convert.ToString(newDf["state", 1]));

            Assert.Equal("SF", Convert.ToString(newDf["city", 2]));
            Assert.Equal("CA", Convert.ToString(newDf["state", 2]));
            Assert.Equal(4f, Convert.ToSingle(newDf["quantity", 2]));

            Assert.Equal("Orlando", Convert.ToString(newDf["city", 4]));
            Assert.Equal(5f, Convert.ToSingle(newDf["retail_price", 4]));
            Assert.Equal("FL", Convert.ToString(newDf["state", 4]));

            Assert.Equal(2f, Convert.ToSingle(newDf["product_id", 5]));
            Assert.Equal("SJ", Convert.ToString(newDf["city", 5]));
            Assert.Equal("PR", Convert.ToString(newDf["state", 5]));

        }

        [Fact]
        public void Remove_Test02()
        {
            var dict = new Dictionary<string, List<object>>
            {
                {"product_id",new List<object>()    { 1,    1,      2,      2,      2,          2,          2 } },
                { "retail_price",new List<object>() { 2,    2,      5,      5,      5,          5,          5 } },
                { "quantity",new List<object>()     { 1,    2,      4,      8,      16,         32,         64 } },
                { "city",new List<object>()         { "SF", "SJ",   "SF",   "SJ",   "Miami",    "Orlando",  "SJ"} },
                { "state" ,new List<object>()       { "CA", "CA",   "CA",   "CA",   "FL",       "FL",       "PR" } },
            };


            var df = new DataFrame(dict);

            //remove rows with 'SJ'
            df = df.RemoveRows((row, i) => row["city"].ToString() == "SJ");

            Assert.True(df.RowCount() == 4);


            Assert.Equal(1f, Convert.ToSingle(df["product_id", 0]));
            Assert.Equal(2f, Convert.ToSingle(df["retail_price", 0]));
            Assert.Equal(1f, Convert.ToSingle(df["quantity", 0]));

            Assert.Equal(2f, Convert.ToSingle(df["product_id", 1]));
            Assert.Equal("SF", Convert.ToString(df["city", 1]));
            Assert.Equal("CA", Convert.ToString(df["state", 1]));

            Assert.Equal("Miami", Convert.ToString(df["city", 2]));
            Assert.Equal("FL", Convert.ToString(df["state", 2]));
            Assert.Equal(16f, Convert.ToSingle(df["quantity", 2]));

            Assert.Equal("Orlando", Convert.ToString(df["city", 3]));
            Assert.Equal(5f, Convert.ToSingle(df["retail_price", 3]));
            Assert.Equal("FL", Convert.ToString(df["state", 3]));

        }

		[Fact]
		public void Rename_ShouldRenameColumns()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, 2, 3},
				new List<string> { "col1", "col2", "col3" },
				new ColType[] { ColType.I32, ColType.I32, ColType.I32 });

			// Act
			var result = df.Rename(("col1", "newCol1"), ("col2", "newCol2"));

			// Assert
			Assert.True(result);
			Assert.Equal(new List<string> { "newCol1", "newCol2", "col3" }, df.Columns);
		}

        

		[Fact]
		public void Rename_ShouldThrow_WhenColumnDoesNotExist()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, 2, 3, 4 , 5, 6},
				new List<object> { "row1", "row2" },
				new List<string> { "col1", "col2", "col3" },
				new ColType[] { ColType.I32, ColType.I32, ColType.I32 });

			// Act & Assert
			Assert.Throws<ArgumentException>(() => df.Rename(("col4", "newCol4")));
		}

		[Fact]
		public void Rename_ShouldThrow_WhenNewNameCreatesDuplicate()
		{
			// Arrange
			var df = new DataFrame(
				new List<object> { 1, 2, 3, 4 },
				new List<object> { "row1", "row2" },
				new List<string> { "col1", "col2" },
				new ColType[] { ColType.I32, ColType.I32 });

			// Act & Assert
			Assert.Throws<ArgumentException>(() => df.Rename(("col1", "col2")));
		}

	}
}
