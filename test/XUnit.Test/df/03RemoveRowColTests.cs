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
				new List<string> { "Col1", "Col2" },
				null);

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
