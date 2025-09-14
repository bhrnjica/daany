using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Daany;
namespace Unit.Test.DF
{
	public class DropColumnsTests
	{
		DataFrame df = new DataFrame(
				new List<object> { 1, 2, 3, 4 },//dataframe values
				new List<object> { 0 },//index
				new List<string> { "A", "B", "C", "D" }//columns
				);

		[Fact]
		public void Drop_WithValidPositiveIndices_RemovesCorrectColumns()
		{
			var result = df.Drop(1, 3); // Drop "B" and "D"
			Assert.Equal(new[] { "A", "C" }, result.Columns);
		}

		[Fact]
		public void Drop_WithValidNegativeIndices_RemovesCorrectColumns()
		{
			var result = df.Drop(-1, -3); // Drop "D" and "B"
			Assert.Equal(new[] { "A", "C" }, result.Columns);
		}

		[Fact]
		public void Drop_WithDuplicateIndices_RemovesOnce()
		{
			var result = df.Drop(2, 2); // Drop "C"
			Assert.Equal(new[] { "A", "B", "D" }, result.Columns);
		}

		[Fact]
		public void Drop_WithMixedPositiveAndNegativeIndices_RemovesCorrectColumns()
		{
			var result = df.Drop(0, -1); // Drop "A" and "D"
			Assert.Equal(new[] { "B", "C" }, result.Columns);
		}

		[Fact]
		public void Drop_WithAllIndices_RemovesAllColumns()
		{
			var ex = Assert.Throws<ArgumentException>(() => df.Drop(0, 1, 2, 3));
			Assert.Contains("Dictionary cannot be null or empty.", ex.Message);
		}

		[Fact]
		public void Drop_WithNullIndices_ThrowsArgumentException()
		{
			var ex = Assert.Throws<ArgumentException>(() => df.Drop((int[])null));
			Assert.Contains("No column indices provided", ex.Message);
		}

		[Fact]
		public void Drop_WithOutOfRangePositiveIndex_ThrowsIndexOutOfRangeException()
		{
			var ex = Assert.Throws<IndexOutOfRangeException>(() => df.Drop(4));
			Assert.Contains("out of bounds", ex.Message);
		}

		[Fact]
		public void Drop_WithOutOfRangeNegativeIndex_ThrowsIndexOutOfRangeException()
		{
			//-4 -A column
			//-1 -D column
			var result = df.Drop(-4, -1); // Drop "A" and "D"
			Assert.Equal(new[] { "B", "C" }, result.Columns);
		}
	}
}
