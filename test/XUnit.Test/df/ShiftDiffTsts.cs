using Daany;
using System;

namespace Unit.Test.DF
{
	using Xunit;

	public class DataFrameShiftDiffTests
	{
		[Fact]
		public void Shift_ShouldShiftColumnDownwards()
		{
			var df = new DataFrame(
				("col1", new object[] { 1, 2, 3, 4, 5 }));

			var shiftedCol = df.Shift(2, "col1", "col1_shifted");

			Assert.Equal(DataFrame.NAN, shiftedCol["col1_shifted"][0]);
			Assert.Equal(DataFrame.NAN, shiftedCol["col1_shifted"][1]);
			Assert.Equal(1, shiftedCol["col1_shifted"][2]);
			Assert.Equal(2, shiftedCol["col1_shifted"][3]);
			Assert.Equal(3, shiftedCol["col1_shifted"][4]);
		}

		[Fact]
		public void Shift_ShouldShiftColumnUpwards()
		{
			var df = new DataFrame(
				("col1", new object[] { 1, 2, 3, 4, 5 }));

			var shiftedCol = df.Shift(-2, "col1", "col1_shifted");

			Assert.Equal(3, shiftedCol["col1_shifted"][0]);
			Assert.Equal(4, shiftedCol["col1_shifted"][1]);
			Assert.Equal(5, shiftedCol["col1_shifted"][2]);
			Assert.Equal(DataFrame.NAN, shiftedCol["col1_shifted"][3]);
			Assert.Equal(DataFrame.NAN, shiftedCol["col1_shifted"][4]);
		}

		[Fact]
		public void Shift_ShouldThrowException_WhenStepsIsZero()
		{
			var df = new DataFrame(("col1", new object[] { 1, 2, 3 }));

			Assert.Throws<ArgumentException>(() => df.Shift(0, "col1", "col1_shifted"));
		}

		[Fact]
		public void Diff_ShouldComputeRowDifferences()
		{
			var df = new DataFrame(
				("col1", new object[] { 10, 20, 30, 40, 50 }));

			var diffDf = df.Diff(1, DiffType.Seasonal);

			Assert.Equal(DataFrame.NAN, diffDf["col1", 0]);
			Assert.Equal(10, diffDf["col1", 1]);
			Assert.Equal(10, diffDf["col1", 2]);
			Assert.Equal(10, diffDf["col1", 3]);
			Assert.Equal(10, diffDf["col1", 4]);
		}
	}

}
