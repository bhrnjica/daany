using Daany;
using System;
using Xunit;

namespace Unit.Test.DF
{

	public class DataFrameIndexingTests
	{
		[Fact]
		public void Indexer_ShouldReturnCorrectValue_ByRowAndColumnIndex()
		{
			var df = new DataFrame(
				("col1", new object[] { 1, 2, 3 }),
				("col2", new object[] { "A", "B", "C" }));

			Assert.Equal(2, df[1, 0]); // Second row, first column
			Assert.Equal("C", df[2, 1]); // Third row, second column
		}

		[Fact]
		public void Indexer_ShouldReturnCorrectValue_ByColumnNameAndRow()
		{
			var df = new DataFrame(
				("col1", new object[] { 1, 2, 3 }),
				("col2", new object[] { "A", "B", "C" }));

			Assert.Equal("B", df["col2", 1]);
			Assert.Equal(3, df["col1", 2]);
		}

		[Fact]
		public void Indexer_ShouldThrowException_WhenColumnDoesNotExist()
		{
			var df = new DataFrame(("col1", new object[] { 1, 2, 3 }));

			Assert.Throws<ArgumentException>(() => df["invalidCol", 1]);
		}

		[Fact]
		public void Indexer_ShouldReturnSubsetDataFrame_WhenFilteringByColumns()
		{
			var df = new DataFrame(
				("col1", new object[] { 1, 2 }),
				("col2", new object[] { 10, 20 }),
				("col3", new object[] { "X", "Y" }));

			var subsetDf = df["col1", "col3"];

			Assert.Equal(2, subsetDf.RowCount());
			Assert.Equal("Y", subsetDf["col3", 1]);
		}
	}


}
